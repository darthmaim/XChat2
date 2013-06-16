using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Common.Configuration
{
    public class ConfigEntry
    {
        protected string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected object _value;
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public ConfigEntry(string name) : this(name, null) { }

        public ConfigEntry(string name, object value)
        {
            _name = name;
            Value = value;
        }
    }

    public class ConfigEntry<T> : ConfigEntry
    {
        private T _tValue;
        public new T Value
        {
            get { return _tValue; }
            set { _tValue = value; _value = value; }
        }

        public ConfigEntry(string name) : base(name, default(T)) { }

        public ConfigEntry(string name, T value) : base(name, value) { }
    }
}
