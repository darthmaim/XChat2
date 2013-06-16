using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Common.Configuration
{
    public class ConfigSection : IEnumerable<ConfigEntry>
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private Dictionary<string, ConfigEntry> _entries;

        public ConfigEntry this[string name]
        {
            get
            {
                return get<object>(name);
            }
        }

        public ConfigEntry<T> get<T>(string name)
        {
            if(!_entries.ContainsKey(name))
                _entries.Add(name, new ConfigEntry<T>(name));
            return _entries[name] as ConfigEntry<T>;
        }

        public ConfigSection(string name)
        {
            _name = name;

            _entries = new Dictionary<string, ConfigEntry>();
        }

        public bool ContainsEntry(string name)
        {
            return _entries.ContainsKey(name);
        }

        public IEnumerator<ConfigEntry> GetEnumerator()
        {
            foreach(ConfigEntry ce in _entries.Values)
                yield return ce;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach(ConfigEntry ce in _entries.Values)
                yield return ce;
        }
    }
}
