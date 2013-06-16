using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Client.Contacts
{
    public class Contact
    {
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        
    }
}
