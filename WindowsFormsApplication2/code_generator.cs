using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WindowsFormsApplication2
{
    class code_generator
    {
        public int a = 0;
        public int i=0;
        public void create_label(string c)
        {
            StreamWriter sw=new StreamWriter("code_generator.txt",true);
            sw.WriteLine(c);
            sw.Close();
        }
        public string label()
        {
            return "L"+a++;
        }
        public string temp_var()
        {
            return "T"+i++;
        }
    }
}
