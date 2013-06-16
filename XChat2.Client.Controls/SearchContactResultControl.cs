using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace XChat2.Client.Controls
{
    public class SearchContactResultControl : ScrollableControl
    {
        public SearchContactResultControl()
        {
            _usernames = new Dictionary<uint, string>();

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.Paint += new PaintEventHandler(SearchContactResultControl_Paint);
            this.MouseMove += new MouseEventHandler(SearchContactResultControl_MouseMove);
            this.MouseDown += new MouseEventHandler(SearchContactResultControl_MouseDown);
            this.MouseUp += new MouseEventHandler(SearchContactResultControl_MouseUp);

            _addButtonSize = TextRenderer.MeasureText(_addButtonText, _addButtonFont);
        }

        int _clickedButton = -1;
        void SearchContactResultControl_MouseDown(object sender, MouseEventArgs e)
        {
            _clickedButton = _hoveredButton;
            this.Invalidate(GetButtonRectangle(_clickedButton));
        }

        void SearchContactResultControl_MouseUp(object sender, MouseEventArgs e)
        {
            if(_clickedButton != -1)
            {
                int oldClicked = _clickedButton;
                _clickedButton = -1;
                this.Invalidate(GetButtonRectangle(oldClicked));
                if(_hoveredButton == oldClicked)
                {
                    OnAddButtonClicked(oldClicked);
                }
            }
        }

        int _hoveredButton = -1;
        int _hoveredEntry = -1;
        void SearchContactResultControl_MouseMove(object sender, MouseEventArgs e)
        {
            int oldHovered = _hoveredButton;
            _hoveredButton = -1;
            for(int i = 0; i < _usernames.Count; i++)
                if(GetButtonRectangle(i).Contains(e.Location))
                {
                    _hoveredButton = i;
                    break;
                }
            if(oldHovered != _hoveredButton)
            {
                if(oldHovered != -1)
                    this.Invalidate(GetButtonRectangle(oldHovered));
                if(_hoveredButton != -1)
                    this.Invalidate(GetButtonRectangle(_hoveredButton));
            }

            int oldHoveredEntry = _hoveredEntry;
            int temp = e.Location.Y - 2 + this.AutoScrollPosition.Y;
            temp /= 16;
            if(temp >= 0 && temp < _usernames.Count)
                _hoveredEntry = temp;
            else
                _hoveredEntry = -1;

            if(oldHoveredEntry != _hoveredEntry)
            {
                if(oldHoveredEntry != -1)
                    this.Invalidate(new Rectangle(0, 2 + oldHoveredEntry * 16 + this.AutoScrollPosition.Y, this.Width, 16));
                if(_hoveredEntry != -1)
                    this.Invalidate(new Rectangle(0, 2 + _hoveredEntry * 16 + this.AutoScrollPosition.Y, this.Width, 16));
            }
        }

        private Dictionary<uint, string> _usernames;

        public void SetUsernames(Dictionary<uint, string> usernames)
        {
            _usernames = usernames;
            this.AutoScrollMinSize = new Size(0, 4 + 16 * usernames.Count);
            this.Invalidate();
        }


        private static Color _entryHoverColorTop = Color.FromArgb(0xEE, 0xEE, 0xFF);
        private static Color _entryHoverColorBottom = Color.FromArgb(0xD0, 0xD0, 0xFF);
        void SearchContactResultControl_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.None;
            if(_usernames.Count == 0)
            {
                e.Graphics.FillRectangle(Brushes.WhiteSmoke, new Rectangle(0, 0, this.Width, this.Height));
                TextRenderer.DrawText(e.Graphics, "Es konnten keine mit der Suche übereinstimmenden Benutzer gefunden werden.", _usernameFont, new Rectangle(new Point(), this.Size), Color.Black, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.WordBreak);
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, this.Width, this.Height));

                for(int i = 0; i < _usernames.Count; i++)
                {
                    e.Graphics.SmoothingMode = SmoothingMode.None;
                    if(i == _hoveredEntry)
                    {
                        using(LinearGradientBrush lgb = new LinearGradientBrush(new Point(0, 2 + i * 16), new Point(0, 2 + (i+1) * 16), _entryHoverColorTop, _entryHoverColorBottom))
                            e.Graphics.FillRectangle(lgb, 0, (2 + i * 16) + this.AutoScrollPosition.Y, this.Width, 16);
                    }
                    DrawUsername(e.Graphics, _usernames.Values.ElementAt(i), (2 + i * 16) + this.AutoScrollPosition.Y);
                    DrawAddButton(e.Graphics, i);
                }
            }
        }

        Font _usernameFont = new Font("Tahoma", 8.5f, FontStyle.Regular);
        private void DrawUsername(Graphics graphics, string username, int top)
        {
            TextRenderer.DrawText(graphics, username, _usernameFont, new Point(2, top + 2), Color.Black);
        }

        private string _addButtonText = "+ Add";
        private Font _addButtonFont = new Font("Tahoma", 8.5f, FontStyle.Regular);
        private Color _addButtonFontColor = Color.White;
        private Size _addButtonSize;
        private Pen _addButtonDefaultOutlinePen = new Pen(Color.Navy, 1f);
        private Pen _addButtonHoverOutlinePen = new Pen(Color.Navy, 1f);
        private Color _addButtonDefaultGradientTop = Color.CornflowerBlue;
        private Color _addButtonDefaultGradientBottom = Color.Navy;
        private Color _addButtonHoverGradientTop = Color.FromArgb(0xA9, 0xC0, 0xE8);
        private Color _addButtonHoverGradientBottom = Color.FromArgb(0x00, 0x00, 0x8D);
        private Color _addButtonClickedGradientTop = Color.Navy;
        private Color _addButtonClickedGradientBottom = Color.CornflowerBlue;
        private void DrawAddButton(Graphics graphics, int index)
        {
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            Color gradientTop = _hoveredButton == index ? (_clickedButton == index ? (_addButtonClickedGradientTop) : (_addButtonHoverGradientTop)) : (_addButtonDefaultGradientTop);
            Color gradientBottom = _hoveredButton == index ? (_clickedButton == index ? (_addButtonClickedGradientBottom) : (_addButtonHoverGradientBottom)) : (_addButtonDefaultGradientBottom);

            Rectangle buttonRectangle = GetButtonRectangle(index);

            using(LinearGradientBrush lgb = new LinearGradientBrush(new Point(0, buttonRectangle.Top), new Point(0, buttonRectangle.Bottom), gradientTop, gradientBottom))
                graphics.FillRoundedRectangle(buttonRectangle, 5, lgb);

            graphics.DrawRoundedRectangle(buttonRectangle, 5, _hoveredButton == index ? _addButtonHoverOutlinePen : _addButtonDefaultOutlinePen);

            TextRenderer.DrawText(graphics, _addButtonText, _addButtonFont, buttonRectangle, _addButtonFontColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private Rectangle GetButtonRectangle(int index)
        {
            return new Rectangle(((this.Width - (this.VScroll ? System.Windows.Forms.SystemInformation.VerticalScrollBarWidth : 0)) - _addButtonSize.Width) - 8, ((2 + index * 16) + 1) + this.AutoScrollPosition.Y, _addButtonSize.Width + 4, 14);
        }


        private void OnAddButtonClicked(int index)
        {
            if(AddButtonClicked != null)
                AddButtonClicked.Invoke(_usernames.Keys.ElementAt(index));
        }

        public delegate void AddButtonClickedHandler(uint userID);
        /// <summary>
        /// Add Button clicked
        /// </summary>
        public event AddButtonClickedHandler AddButtonClicked;
    }
}
