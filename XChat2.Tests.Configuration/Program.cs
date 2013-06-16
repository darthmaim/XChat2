using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XChat2.Common.Configuration;

namespace XChat2.Tests.Configuration
{
    class Program
    {
        static void Main(string[] args)
        {
            Config c = new Config("test.cfg");
            int testCount = c["test"].get<int>("testCount").Value;
            Console.WriteLine("This is test nr. {0}.", testCount);
            testCount++;
            c["test"].get<int>("testCount").Value = testCount;
            c.Save();
            Console.ReadLine();
        }
    }
}
