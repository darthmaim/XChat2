using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using XChat2.Common.Compression;
using System.IO;

namespace XChat2.Client.Controls
{
    public class SendMessageControl : Control
    {
        TextBox _textBox;
        int _rightMargin = 20;

        Font _font = new Font("Tahoma", 8.5f);
        Pen _pen = new Pen(Color.Black, 2f) { LineJoin = System.Drawing.Drawing2D.LineJoin.Round };

        Font _sendButtonFont = new Font("Tahoma", 8.5f, FontStyle.Bold);

        string _sendButtonText = "Send";

        public SendMessageControl()
        {
            _rightMargin = TextRenderer.MeasureText(_sendButtonText, _sendButtonFont).Width + 10;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.Selectable | ControlStyles.ResizeRedraw, true);
            this.Paint += new PaintEventHandler(SendMessageControl_Paint);
            _textBox = new TextBox();
            _textBox.Font = _font;
            _textBox.Location = new Point(22, 5);
            _textBox.Multiline = true;
            _textBox.BorderStyle = BorderStyle.None;
            _textBox.Width = this.Width - 22 - _rightMargin;
            _textBox.Height = this.Height - 5 - 5;
            _textBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            this.Controls.Add(_textBox);
            this.MouseDown += SendMessageControl_MouseDown;
            this.MouseUp += SendMessageControl_MouseUp;
            this.MouseMove += SendMessageControl_MouseMove;
            this.KeyUp += SendMessageControl_KeyUp;
            _textBox.KeyUp += SendMessageControl_KeyUp;

            this.KeyDown += SendMessageControl_KeyDown;
            _textBox.KeyDown += SendMessageControl_KeyDown;

            _currentPoints = new List<Point>();
            _lines = new List<Point[]>();

            _buffer = new Bitmap(1,1, System.Drawing.Imaging.PixelFormat.Format32bppArgb); 
            _bufferGraphics = Graphics.FromImage(_buffer);
        }

        void SendMessageControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Control)
            {
                OnSend();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        void SendMessageControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (Mode == Modes.Paint && e.KeyCode == Keys.Delete)
            {
                ClearPainting();
            }
            else if (Mode == Modes.Paint && e.KeyCode == Keys.Escape && _drawing)
            {
                _drawing = false;
                this.Invalidate();
            }
            else if (Mode == Modes.Paint && (e.KeyCode == Keys.Back || (e.KeyCode == Keys.Z && e.Control)) && _lines.Count > 0)
            {
                _lines.RemoveAt(_lines.Count - 1);
                RedrawBuffer();
                this.Invalidate();
            }
            else if(e.KeyCode == Keys.Enter && !e.Control)
            {
            }
        }

        private void ClearPainting()
        {
            _lines = new List<Point[]>();
            _buffer = new Bitmap(1, 1);
            _bufferGraphics = Graphics.FromImage(_buffer);
            this.Invalidate();
        }

        int _hoveredButton = -1;
        bool _hoverSendButton = false;
        bool _clickedSendButton = false;
        void SendMessageControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_drawing)
            {
                int oldHoveredButton = _hoveredButton;
                if (isInSelectionButton(e.Location, 0))
                    _hoveredButton = 0;
                else if (isInSelectionButton(e.Location, 1))
                    _hoveredButton = 1;
                //else if (isInSelectionButton(e.Location, 2))
                //    _hoveredButton = 2;
                else
                    _hoveredButton = -1;

                if (oldHoveredButton != _hoveredButton)
                    this.Invalidate(new Rectangle(0, 0, 20, this.Height));

                if(isInSendButton(e.Location))
                {
                    if(!_hoverSendButton)
                    {
                        _hoverSendButton = true;
                        this.Invalidate(new Rectangle(this.Width - _rightMargin, 0, _rightMargin, this.Height));
                    }
                }
                else
                {
                    if(_hoverSendButton)
                    {
                        _hoverSendButton = false;
                        this.Invalidate(new Rectangle(this.Width - _rightMargin, 0, _rightMargin, this.Height));
                    }
                }
            }

            Cursor.Current = (Mode == Modes.Paint && isInPaintingArea(e.Location)) ? Cursors.Hand : Cursors.Default;
            if (_drawing)
            {
                _currentPoints.Add(e.Location);
                this.Invalidate();
            }
        }

        void SendMessageControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (_drawing)
            {
                _drawing = false;
                AddToBuffer(_currentPoints.ToArray());
                this.Invalidate();
            }
            if(_clickedSendButton)
            {
                OnSend();
                _clickedSendButton = false;
                this.Invalidate(new Rectangle(this.Width - _rightMargin, 0, _rightMargin, this.Height));
            }
        }

        private void AddToBuffer(Point[] points)
        {
            if (points.Length < 2)
                return;
            Point[] newPoints = new Point[points.Length];
            int newBufferWidth = _buffer.Width;
            int newBufferHeight = _buffer.Height;
            int i = 0;
            foreach (Point point in points)
            {
                newPoints[i++] = new Point(point.X - 22, point.Y - 5);
                if (point.X - 22 > newBufferWidth)
                    newBufferWidth = point.X - 22;
                else if (point.Y - 5 > newBufferHeight)
                    newBufferHeight = point.Y - 5;
            }
            if (_buffer.Width < newBufferWidth || _buffer.Height < newBufferHeight)
            {
                using (Bitmap oldBuffer = (Bitmap)_buffer.Clone())
                {
                    _buffer = new Bitmap(newBufferWidth + 5, newBufferHeight + 5, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    _bufferGraphics = Graphics.FromImage(_buffer);
                    _bufferGraphics.DrawImage(oldBuffer, 0, 0);
                }
            }
            _lines.Add(newPoints);
            _bufferGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            _bufferGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            _bufferGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            _bufferGraphics.DrawLines(_pen, newPoints);
        }


        Bitmap _buffer;
        Graphics _bufferGraphics;
        private void RedrawBuffer()
        {
            _bufferGraphics.Clear(Color.Transparent);
            //_bufferGraphics.FillRectangle(Brushes.White, 0, 0, _buffer.Width, _buffer.Height);
            foreach (Point[] line in _lines)
            {
                _bufferGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                _bufferGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                _bufferGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                _bufferGraphics.DrawLines(_pen, line);
            }
        }

        private bool isInSelectionButton(Point p, int button)
        {
            return p.X >= 2 && p.X < 2 + 16 && p.Y >= 7 + (18 * button) && p.Y < 7 + 16 + (18*button);
        }

        private bool isInPaintingArea(Point p)
        {
            return p.X >= 22 && p.X < this.Width - _rightMargin && p.Y >= 5 && p.Y < this.Height - 5;
        }

        private bool isInSendButton(Point p)
        {
            return p.X >= this.Width - _rightMargin + 4 && p.X < this.Width - 4 && p.Y >= this.Height / 2 - 10 && p.Y < this.Height / 2 + 10;
        }

        private bool _drawing = false;
        private List<Point> _currentPoints;
        private List<Point[]> _lines;
        void SendMessageControl_MouseDown(object sender, MouseEventArgs e)
        {
            this.Focus();
            if(isInSelectionButton(e.Location, 0))
                Mode = Modes.Text;
            else if(isInSelectionButton(e.Location, 1))
                Mode = Modes.Paint;
            //else if (isInSelectionButton(e.Location, 2))
            //{
            //    OpenFile();
            //}
            else if (isInSendButton(e.Location))
            {
                _clickedSendButton = true;
                this.Invalidate(new Rectangle(this.Width - _rightMargin, 0, _rightMargin, this.Height));
            }
            else if (Mode == Modes.Paint && isInPaintingArea(e.Location) && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _drawing = true;
                _currentPoints.Clear();
                _currentPoints.Add(e.Location);
            }
        }

        public enum Modes { Paint, Text }
        private Modes _mode = Modes.Text;
        public Modes Mode
        {
            get { return _mode; }
            set
            {
                _mode = value; 
                this.Invalidate(); 
                _textBox.Visible = this.Mode == Modes.Text;
                if(_mode == Modes.Text) 
                    _textBox.Focus(); 
                else 
                    this.Focus();
            }
        }
        
        void SendMessageControl_Paint(object sender, PaintEventArgs e)
        {
            DrawBackground(e.Graphics);
            DrawModeSelection(e.Graphics);
            DrawSendButton(e.Graphics);
            if (_mode == Modes.Paint)
                DrawPaintings(e.Graphics);
        }

        private void DrawBackground(Graphics graphics)
        {
            graphics.FillRectangle(Brushes.White, 0, 0, this.Width, this.Height);
        }

        //private Brush _modeButtonSelectedBrush = new SolidBrush(Color.FromArgb(0x32, 0x4A, 0xB6));
        private Brush _modeButtonSelectedBrush = Brushes.Navy;
        private Brush _modeButtonDefaultBrush = Brushes.LightGray;

        private void DrawModeSelection(System.Drawing.Graphics graphics)
        {
            graphics.DrawLine(Pens.Silver, 20, 5, 20, this.Height - 5);
            graphics.FillRectangle(_mode == Modes.Text ? _modeButtonSelectedBrush : _modeButtonDefaultBrush, 2, 7, 16, 16);
            graphics.FillRectangle(_mode == Modes.Paint ? _modeButtonSelectedBrush : _modeButtonDefaultBrush, 2, 25, 16, 16);
            //graphics.FillRectangle(_modeButtonDefaultBrush, 2, 43, 16, 16);
            TextRenderer.DrawText(graphics, "T", SystemFonts.DefaultFont, new Point(4, 9), _mode == Modes.Text ? Color.White : Color.Navy);
            TextRenderer.DrawText(graphics, "P", SystemFonts.DefaultFont, new Point(4, 27), _mode == Modes.Paint ? Color.White : Color.Navy);
            // graphics.DrawImageUnscaled(XChat2.Client.Resources.Images.file_blue, new Point(2, 43));
            if (_hoveredButton > -1)
            {
                graphics.DrawRectangle(Pens.Navy, 1, 6 + 18 * _hoveredButton, 17, 17);
            }
        }

        private void DrawPaintings(System.Drawing.Graphics graphics)
        {
            if (Mode == Modes.Paint)
            {
                graphics.DrawImage(_buffer, new Rectangle(22, 5, this.Width - 22 - _rightMargin, this.Height - 5 - 5), new Rectangle(0, 0, this.Width - 22 - _rightMargin, this.Height - 5 - 5), GraphicsUnit.Pixel);
                if (_drawing  && _currentPoints.Count >= 2)
                {
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                    graphics.DrawLines(Pens.Red, _currentPoints.ToArray());
                }
            }
        }

        private Pen _sendButtonDefaultOutlinePen = new Pen(Color.Navy, 1f);
        private Pen _sendButtonHoverOutlinePen = new Pen(Color.Navy, 1f);

        private Color _sendButtonDefaultGradientTop = Color.CornflowerBlue;
        private Color _sendButtonDefaultGradientBottom = Color.Navy;

        private Color _sendButtonHoverGradientTop = Color.FromArgb(0xA9, 0xC0, 0xE8);
        private Color _sendButtonHoverGradientBottom = Color.FromArgb(0x00, 0x00, 0x8D);

        //private Color _sendButtonHoverGradientTop = Color.White;
        //private Color _sendButtonHoverGradientBottom = Color.Navy;

        private Color _sendButtonClickedGradientTop = Color.Navy;
        private Color _sendButtonClickedGradientBottom = Color.CornflowerBlue;

        private void DrawSendButton(System.Drawing.Graphics graphics)
        {
            graphics.DrawLine(Pens.Silver, this.Width - _rightMargin, 5, this.Width - _rightMargin, this.Height - 5);
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;


            Color gradientTop = _hoverSendButton ? (_clickedSendButton ? (_sendButtonClickedGradientTop) : (_sendButtonHoverGradientTop)) : (_sendButtonDefaultGradientTop);
            Color gradientBottom = _hoverSendButton ? (_clickedSendButton ? (_sendButtonClickedGradientBottom) : (_sendButtonHoverGradientBottom)) : (_sendButtonDefaultGradientBottom);

            using(LinearGradientBrush lgb = new LinearGradientBrush(new Point(0, this.Height / 2 - 10), new Point(0, this.Height / 2 + 10), gradientTop, gradientBottom))
                graphics.FillRoundedRectangle(new Rectangle(this.Width - _rightMargin + 4, this.Height / 2 - 10, _rightMargin - 8, 20), 6, lgb);
            
            graphics.DrawRoundedRectangle(new Rectangle(this.Width - _rightMargin + 4, this.Height / 2 - 10, _rightMargin - 8, 20), 6, _hoverSendButton ? _sendButtonHoverOutlinePen : _sendButtonDefaultOutlinePen);
            
            TextRenderer.DrawText(graphics, _sendButtonText, _sendButtonFont, new Point(this.Width - _rightMargin + 6, this.Height / 2 - (_clickedSendButton ? 6 : 7)), Color.White);
        }

        private void OpenFile()
        {
            return;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Multiselect = true;
            ofd.Title = "XChat2 - Send file...";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in ofd.FileNames)
                {
                    if (File.Exists(file))
                    {
                        OnSendFile(file);
                    }
                }
            }
        }

        private void OnSend()
        {
            if(Mode == Modes.Text && !string.IsNullOrEmpty(_textBox.Text.Trim()) && SendText != null)
            {
                SendText(this._textBox.Text);
                _textBox.Text = "";
            }
            else if(Mode == Modes.Paint && _lines.Count >= 1 && SendImage != null)
            {
                Bitmap x = (Bitmap)_buffer.Clone();
                SendImage(x, Imaging.BitmapToByteArray(x));
                ClearPainting();
            }
        }

        private void OnSendFile(string file)
        {
            return;
            if (!string.IsNullOrEmpty(file) && SendFile != null)
                SendFile(file);
        }

        public delegate void SendTextHandler(string text);
        public delegate void SendImageHandler(Bitmap image, byte[] imageBuffer);
        public delegate void SendFileHandler(string file);

        public event SendTextHandler SendText;
        public event SendImageHandler SendImage;
        public event SendFileHandler SendFile;
    }

    public static class Extensions
    {
        public static void DrawRoundedRectangle(this Graphics g, Rectangle r, int d, Pen p)
        {
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();

            gp.AddArc(r.X, r.Y, d, d, 180, 90);
            gp.AddArc(r.X + r.Width - d, r.Y, d, d, 270, 90);
            gp.AddArc(r.X + r.Width - d, r.Y + r.Height - d, d, d, 0, 90);
            gp.AddArc(r.X, r.Y + r.Height - d, d, d, 90, 90);
            gp.CloseFigure();
            //gp.AddLine(r.X, r.Y + r.Height - d, r.X, r.Y + d/2);

            g.DrawPath(p, gp);
        }

        public static void FillRoundedRectangle(this Graphics g, Rectangle r, int d, Brush b)
        {
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();

            gp.AddArc(r.X, r.Y, d, d, 180, 90);
            gp.AddArc(r.X + r.Width - d, r.Y, d, d, 270, 90);
            gp.AddArc(r.X + r.Width - d, r.Y + r.Height - d, d, d, 0, 90);
            gp.AddArc(r.X, r.Y + r.Height - d, d, d, 90, 90);
            gp.CloseFigure();
            
            g.FillPath(b, gp);
        }
    }
}
