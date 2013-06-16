using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace XChat2.Client.Controls.TabControl
{
    public class BaseTabPage<T> : ITabPage
    {
        private T _object;
        public T Object
        {
            get { return _object; }
            set { _object = value; }
        }

        private bool _canClose;
        public bool CanClose
        {
            get { return _canClose; }
            set { _canClose = value; }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        private Font _font = new Font("Tahoma", 8.5f);
        public Font Font
        {
            get { return _font; }
            set { _font = value; }
        }
        

        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value;
            this.Size = TextRenderer.MeasureText(_text, _font);
            }
        }

        private Size _size;
        public Size Size
        {
            get { return _size; }
            set { _size = value; }
        }        
    }
}
