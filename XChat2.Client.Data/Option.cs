using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Client.Data
{
    public class Option<T>
    {
        private T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnOptionChanged(_value);
            }
        }

        public Option(T value)
        {
            _value = value;
        }

        public static implicit operator T(Option<T> option)
        {
            return option.Value;
        }

        public static implicit operator Option<T>(T value)
        {
            return new Option<T>(value);
        }

        private void OnOptionChanged(T newValue)
        {
            if (OptionChanged != null)
                OptionChanged(this, newValue);
        }
        
        public delegate void OptionChangedHandler(Option<T> sender, T newValue);
        public event OptionChangedHandler OptionChanged;
    }
}
