using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WindowsFormsApplication2
{
    class Token
    {
        public  int columnNo;
        int LineNumber;
        string Class_part;
        string Value_part;
        public string[] invalidWords = new string[4];
        public string filePath = @"token.txt";
        public static string[,] token_array = new string[1000, 3];
        public static int i = 0;
        
       public Token(string cp, string vp, int ln,int cn)
        {
            Class_part = cp;
            Value_part = vp;
            LineNumber = ln;
            columnNo = cn;
        }
       public Token()
       {
            
       }
       public void clearFile()
       {
           File.WriteAllText(filePath, String.Empty);
           token_array = new string[1000, 3];
       }
       public void generateToken()
       {
           //Form1 f = new Form1();
           StreamWriter sw = new StreamWriter(filePath, true);

           sw.WriteLine("token(" + Class_part + " , " + Value_part + " , " + LineNumber + ")");
           sw.Close();
           if (Class_part == "invalid")
           {
               columnNo -= Value_part.Length - 1;
           }
           add_token();
      
       }
       public void add_token()
       {
           token_array[i, 0] = Class_part;
           token_array[i, 1] = Value_part;
           token_array[i, 2] = Convert.ToString(LineNumber);
           i++;
       }
       public void add_endmarker()
       {
           token_array[i, 0] = "$";
           token_array[i, 1] = "";
           token_array[i, 2] = "";
           i = 0;
       }
    }
    }
