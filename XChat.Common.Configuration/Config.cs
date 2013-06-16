using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Common.Configuration
{
    public class Config : IEnumerable<ConfigSection>
    {
        private string _filename;
        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }
        
        public Config(string filename)
        {
            _sections = new Dictionary<string, ConfigSection>();

            if(!System.IO.File.Exists(filename))
                System.IO.File.Create(filename).Close();

            _filename = filename;

            Load();
        }

        Dictionary<string, ConfigSection> _sections;
        public ConfigSection this[string name]
        {
            get
            {
                if(!_sections.ContainsKey(name))
                    _sections.Add(name, new ConfigSection(name));
                return _sections[name];
            }
        }

        public bool ContainsSection(string name)
        {
            return _sections.ContainsKey(name);
        }

        public void AddSection(string name)
        {
            if(!_sections.ContainsKey(name))
                _sections.Add(name, new ConfigSection(name));
            else
                throw new Exception("ConfigSection '"+ name + "' already exists.");
        }

        IEnumerator<ConfigSection> IEnumerable<ConfigSection>.GetEnumerator()
        {
            foreach(ConfigSection cs in _sections.Values)
                yield return cs;
        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach(ConfigSection cs in _sections.Values)
                yield return cs;
        }

        private void Load()
        {
            ConfigSection cs = null;
            string[] lines = System.IO.File.ReadAllLines(_filename);
            for(int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if(string.IsNullOrEmpty(line.Trim()))
                    continue;

                if(line.StartsWith("[") && line.EndsWith("]"))
                {
                    cs = this[line.Substring(1, line.Length - 2)];
                }

                if(line.Contains("=") && cs != null)
                {
                    string key = line.Substring(0, line.IndexOf('='));
                    string val = line.Substring(key.Length + 1);
                    //if(CanConvertTo<float>(val))
                    //    cs.get<float>(key).Value = ConvertTo<float>(val);
                    if(CanConvertTo<int>(val))
                        cs.get<int>(key).Value = ConvertTo<int>(val);
                    else
                        cs.get<string>(key).Value = val;
                }
            }
        }

        public void Save()
        {
            using(System.IO.StreamWriter sw = new System.IO.StreamWriter(_filename, false))
            {
                foreach(ConfigSection cf in this)
                {
                    sw.WriteLine("[{0}]", cf.Name);
                    foreach(ConfigEntry ce in cf)
                    {
                        sw.WriteLine("{0}={1}", ce.Name, ce.Value);
                    }
                    sw.WriteLine();
                }
                sw.Flush();
            }
        }

        public static bool CanConvertTo<T>(string input)
        {
            object converted = ConvertToObject<T>(input);
            return converted != null;
        }

        public static T ConvertTo<T>(string input)
        {
            object converted = ConvertToObject<T>(input);
            if(converted != null)
                return (T)converted;
            else
                throw new InvalidCastException();
        }

        public static object ConvertToObject<T>(string input)
        {
            object converted;
            try
            {
                converted = (T)Convert.ChangeType(input, typeof(T));
            }
            catch
            {
                converted = null;
            }
            return converted;
        }


    }
}
