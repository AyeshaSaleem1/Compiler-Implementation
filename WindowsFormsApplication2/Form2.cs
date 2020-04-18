using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace WindowsFormsApplication2
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        
        string[] LineArray;
        int a,j;
        int i=0, LineNo;
        public void lexime()
        {
            int y = 10;
            StreamReader re = new StreamReader("token.txt");
            while (!re.EndOfStream)
            {
                string T_lex = re.ReadLine();
                if (T_lex.Contains("Invalid"))
                {
                    Label l1 = new Label();
                    l1.Text = T_lex;
                    l1.ForeColor = Color.DarkRed;
                    l1.Font = new Font("Arial", 12, FontStyle.Regular);
                    l1.SetBounds(10, y, 700, 30);
                    tabPage3.Controls.Add(l1);
                    y += 30;

                }
            }
        }
        public void code_gene()
        {
            int y = 10;
            StreamReader re = new StreamReader("code_generator.txt");
            while (!re.EndOfStream)
            {
                
            Label l1 = new Label();
            l1.Text = re.ReadLine();
            l1.ForeColor = Color.DarkRed;
            l1.Font = new Font("Arial", 12, FontStyle.Regular);
            l1.SetBounds(10, y, 700, 30);
            tabPage4.Controls.Add(l1);
            y += 30;
            }
            re.Close();
        }
       
        public void error(string ui_status)
        {
            Label l1 = new Label();
            l1.Text = ui_status;
            l1.ForeColor = Color.DarkRed;
            l1.Font = new Font("Calibri Light", 15, FontStyle.Bold);
            l1.SetBounds(10, 10, 1000, 40);
            tabPage1.Controls.Add(l1);
        }
        
        public void success(string ui_status)
        {
            Label l1 = new Label();
            l1.Text = ui_status;
            l1.ForeColor = Color.Green;
            l1.Font = new Font("Constantia", 15, FontStyle.Bold);
            l1.SetBounds(10, 10,1000,40);
            tabPage1.Controls.Add(l1);
        }
        public void StreamRead(string fPath)
        {
            StreamReader sr = new StreamReader(fPath);
            int a = File.ReadAllLines(fPath).Length;
            LineArray = new string[a];
            int count1 = 0, c=00;
            int b = 1;
            while (!sr.EndOfStream)
            {
                Label l = new Label();
                if(c<9)
                    l.Text = "0" + ++c;
                else
                    l.Text = "" + ++c;
                l.Font = new Font("Consolas", 10);
                l.ForeColor = Color.DarkGreen;
                l.SetBounds(1, b * 2, 25, 15);
                b = b + 9;
                //panel2.Controls.Add(l);
                ///tabPage3.Controls.Add(l);
                LineArray[count1] = sr.ReadLine();
                richTextBox1.Text += LineArray[count1] + "\n";
                count1++;
            }
            sr.Close();
        }
 
        public void StreamRead1(string fPath)
        {
            StreamReader sr = new StreamReader(fPath);
            int b = File.ReadAllLines(fPath).Length;
            a = 10;
            while (!sr.EndOfStream)
            {
                Label l = new Label();
                l.Text = sr.ReadLine();
                l.Font = new Font("Cons tantia", 12);
                if (l.Text.Contains(".") || l.Text.Contains(",") || l.Text.Contains(":") || l.Text.Contains(";") || l.Text.Contains("(")
                    || l.Text.Contains(")") || l.Text.Contains("[") || l.Text.Contains("]") || l.Text.Contains("{") || l.Text.Contains("}"))
                    l.ForeColor = Color.SaddleBrown;
                if (l.Text.Contains("+") || l.Text.Contains("-") || l.Text.Contains("*") || l.Text.Contains("/") || l.Text.Contains("=")
                   || l.Text.Contains("!") || l.Text.Contains("&") || l.Text.Contains("%") || l.Text.Contains("<") || l.Text.Contains(">"))
                    l.ForeColor = Color.Navy;
                if (l.Text.Contains("invalid"))
                    l.ForeColor = Color.DarkRed;
                if (l.Text.Contains("_Constant"))
                    l.ForeColor = Color.DarkGreen;
                if (l.Text.Contains("Is_ID"))
                    l.ForeColor = Color.DarkSlateGray;
                l.SetBounds(12, a * 2, 405, 17);
                a = a + 12;
                //panel1.Controls.Add(l);
                tabPage2.Controls.Add(l);
            }
            sr.Close();
            

        }
                public void mainFun()
        {
            LineNo = 0;
            for (i = 0; i < LineArray.Length; i++)
            {
                LineNo++;
                breakword(LineArray[i]);
            }
        }

        
        public void breakword(string input)
        {

            char[] punctuator = { ',', ';', '{', '}', '[', ']', '(', ')', ':' };
            char[] opr = { '+', '-', '/', '*', '%', '=', '!', '<', '>' };
             int size = 0, count, lengthOfTemp = 0;
            string temp = null;

            for (j = 0; j < input.Length; j++)
            {
                   count = 0;
                //space
                if (char.IsWhiteSpace(input[j]))
                {
                    if (temp != null)
                    {
                        Classification(temp);
                        temp = null;

                    }
                    goto L1;
                }
                //punctuator
                while (count <= punctuator.Length - 1)
                {

                    if (input[j] == punctuator[count])
                    {
                        Classification(temp);
                        temp = null;
                        Classification(input[j].ToString());
                        goto L1;
                    }
                    count++;
                }

                count = 0;
                //operator
                while (count <= opr.Length - 1)
                {
                    if (input[j] == opr[count])
                    {
                        if ((j != input.Length - 1 && lengthOfTemp < 1) && (input[j + 1] == '=' || (input[j] == '+' && input[j + 1] == '+') || (input[j + 1] == '-' && input[j] == '-')))
                        {
                            if (temp != null)
                            {
                                Classification(temp);
                                temp = null;
                            }
                            temp += input[j];
                            lengthOfTemp++;
                            goto L1;

                        }
                        else if (lengthOfTemp == 0 )
                        {
                            if (temp != null)
                            {
                                Classification(temp);
                                temp = null;
                            }

                            //if (j != input.Length - 1 && (char.IsNumber(input[j + 1]) || input[j+1]=='.'))
                            //{
                            //    if ((j == 0) || (j > 0 && (!char.IsNumber(input[j - 1]) && !char.IsLetter(input[j - 1]) && input[j - 1] != '@')))
                            //    {
                            //          temp += input[j];
                            //          goto L4;
                            //    }
                                
                            //}
                          

                        }
                        
                        temp += input[j];
                        Classification(temp);
                        temp = null;
                    L4:       
                        lengthOfTemp = 0;
                        goto L1;
                    }
                    count++;
                }

                // and and or      
                if (input[j] == '&' || input[j] == '|')
                {
                    Classification(temp);
                    temp = null;
                    if ((j < input.Length-1) && (input[j + 1] == '&' || input[j + 1] == '|'))
                    {
                        temp += input[j];
                        j++;
                        size++;
                        temp += input[j];
                        Classification(temp);
                        temp = null;
                    }
                    else
                    {
                        Classification(input[j].ToString());  //error of single & and |
                    }
                    goto L1;
                }
             
                //comment   
                if (input[j] == '#')
                {
                    Classification(temp);
                    temp = null;
                    if (input.Contains("##"))
                    {

                        int m = 0;
                        m++;
                        if (input.IndexOf("##") != input.LastIndexOf("##"))
                        {
                            j = input.LastIndexOf('#');
                            goto L1;
                        }
                        while (!input.Contains("##") || m == 1)
                        {
                            m = 0;
                            if (i < LineArray.Length - 1)
                            {
                                i++;
                                input = LineArray[i]; 
                            }
                            else
                            { goto L2; }
                        }
                        if (input.Length - 2 != input.IndexOf('#'))
                        {
                            j = input.IndexOf('#') + 1;
                            goto L1;
                        }
                    }
                    
                    goto L2;
                }

                //dot

                if (input[j] == '.')
                {
                    //if((temp != null) && (temp[0]=='+' ||temp[0] == '-'))
                    //{
                    //    string temp1 = null;
                    //    for (int g = 1; g < temp.Length; g++)
                    //     temp1 += temp[g]; 

                    //    if ((temp1!=null) &&(!temp1.All(char.IsNumber) || (j == input.Length - 1) || (!char.IsNumber(input[j + 1]))))   //|| temp.Contains('.')
                    //    {
                    //        Classification(temp);
                    //        temp = null;
                    //    }
                    //}
                    //else
                        if ((temp != null && (!temp.All(char.IsNumber))) || (j == input.Length - 1) || (!char.IsNumber(input[j + 1])))   //|| temp.Contains('.')
                    {
                        Classification(temp);
                        temp = null;
                    }
                  
                    temp += input[j];
                    if (j != input.Length-1 && !char.IsNumber(input[j + 1]))
                    {
                        Classification(temp);
                        temp = null;
                    }
                   
                    goto L1;
                }

                //string with "
                if (input[j] == '"')
                {
                    Classification(temp);
                    temp = null;
                    temp += input[j];
                    j++;
                    int templength;
                    int cnt=0;
                    if ((j < input.Length))
                    {
                        while ((input[j] != '"' || temp[temp.Length - 1] == '\\'))
                        {
                            if(input[j] == '"')
                            {
                               templength = temp.Length-1;
                                cnt = 0;
                            while ( temp[templength] == '\\')
                            {
                                cnt++;
                                templength--;
                            }
                            if (cnt % 2 == 0)
                            {  goto add; }
                            }
                            if (j == input.Length - 1)
                                goto add;
                            temp += input[j];
                            if (j == input.Length - 1)
                            {
                                goto add;
                            }
                            j++;
                            size++;
                        }
         add:
                        temp += input[j];
                    }
                    Classification(temp);
                    temp = null;
                    goto L1;
                }

                //char
                if (input[j] == '\'')
                {
                    int t = 0;
                    Classification(temp);
                    temp = null;
                    temp += input[j];
                    while (j <= input.Length - 2 && t < 2)
                    {
                        j++;
                        size++;
                        t++;
                        temp += input[j];

                    }
                    if (temp.Contains('\\') && (j != input.Length - 1))
                    {
                        j++;
                        size++;
                        temp += input[j];
                    }
                    Classification(temp);
                    temp = null;
                    goto L1;
                }
               

                if (input[j] != ' ')
                {
                    temp += input[j];
                }
            L1:
                if ((j == input.Length - 1) && input[j] != ' ')
                {
         
                    Classification(temp);
                    temp = null;
                }

            }

        L2:
            return;

        }



        public void Classification(string wb)
        {
            string classPart = null;
            string word = null;
            bool validity = false;
            if (wb != null)
            {

                if (char.IsLetter(wb[0]) || wb[0]=='@')
                {
                    validity = IsId(wb);
                    if (validity == true)
                    {
                        word = IsKw(wb);
                        if (word == null)
                        {
                            classPart = "ID";
                        }
                        else
                        {
                            classPart = word;
                        }
                    }
                    else
                    {
                        classPart = "Invalid";
                    }
                } /// if letter
                else if (char.IsNumber(wb[0]) || wb[0] == '.' || wb[0] == '+' || wb[0] == '-')   
                {
                    validity = IsIntConst(wb);
                    if (validity == true)
                    {
                        classPart = "Integer_Constant";  //cp = int cons
                      
                    }
                    else
                    {
                        validity = IsFloatConst(wb);
                        if (validity == true)
                        {
                            classPart = "Float_Constant";    //cp = float cons
                           
                        }
                      
                    }   //for dot
                    if (validity == false && wb.Length != 1 && ( wb[0] != '+' && wb[0] != '-'))
                    {
                        classPart = "Invalid";
                    }
                    else 
                    {
                        word = IsPunc(wb);
                        if(word != null)
                        classPart = word;
                        else
                        {
                            word = IsOpr(wb);
                            if (word != null)
                                classPart = word;
                        }
                    }                   //cp = .

                }
                else if (wb[0] == '"')
                {
                    validity = IsStringConst(wb);
                    if (validity == true)
                    {
                        string wb1 = wb;
                        wb = null;
                        for (int g = 1; g < wb1.Length - 1; g++)
                            wb += wb1[g];
                            
                        classPart = "String_Constant"; //cp = str cons
                    }
                    else
                    {
                        classPart = "Invalid";
                    }
                }
                else if (wb[0] == '\'')
                {
                    validity = IsCharConst(wb);
                    if (validity == true)
                    {
                        string wb1 = wb;
                        wb = null;
                        for (int g = 1; g < wb1.Length - 1; g++)
                            wb += wb1[g];
                        classPart = "Char_Constant";  //cp = char cons
                    }
                    else
                    {
                        classPart = "Invalid";//invalid
                    }
                }
                  /// 
                else
                {
                    word = IsOpr(wb);
                    if (word == null)
                    {
                        word = IsPunc(wb);
                    }
                    else
                    {
                        classPart = word;
                    }
                    if (word == null)
                    {
                        classPart = "Invalid";
                    }
                    else
                    {
                        classPart = word;
                    }
                }

           Token token = new Token(classPart, wb, LineNo,j);
            token.generateToken();
            if (classPart == "Invalid")
            {
                richTextBox2.Text += "error occurs on line number: " + LineNo + " at column number: " + token.columnNo + "....\n";
            }

            }//if temp != null
        }


        string IsKw(string word)
        {
            string[,] keyWords = new string[,]{ {"DT","int"}, {"DT","float"}, 
            {"DT","char"},
            {"DT","string"},
            {"DT","bool"},
            {"for","for"},
            {"while","while"},
            {"AM","public"},
            {"AM","private"},
            {"static","static"},
            {"break","break"},
            {"continue","continue"},
            {"void","void"},
            {"return","return"},
            {"static","static"},
            {"class","class"},
            {"this","this"},
            {"new","new"},
            {"interface","interface"},
            {"Bool_Constant","true"},
            {"Bool_Constant","false"},
            {"if","if"},
            {"else","else"},
            {"base","base"},
            {"sealed","sealed"},
            {"Main","Main"},
            {"try","try"},
            {"catch","catch"},
            {"finally","finally"},

        };
            for (int j = 0; j < 29; j++)
            {
                if (keyWords[j, 1] == word)
                {
                    return keyWords[j, 0];   //return class part
                }
            }

            return null;
        }


        string IsPunc(string word)
        {
            string[] punc = new string[]{
            ",",
            ";",
            "{",
            "}",
            "(",
            ")",
            "[",
            "]",
            ".",
            ":"
        };
            for (int j = 0; j < punc.Length; j++)
            {
                if (punc[j] == word)
                {
                    return punc[j];
                }
            }

            return null;

        }


        string IsOpr(string word)
        {
            string[,] oper = new string[,]{
            {"PM","+"},
            {"PM","-"},
            {"MDM","*"},
            {"MDM","/"},
            {"MDM","%"},
            {"ROP","<"},
            {"ROP",">"},
            {"ROP","<="},
            {"ROP",">="},
            {"ROP","!="},
            {"ROP","=="},
            {"Equal","="},
            {"AsgOp","+="},
            {"AsgOp","-="},
            {"AsgOp","*="},
            {"AsgOp","/="},
            {"AsgOp","%="},
            {"AND","&&"},
            {"OR","||"},
            {"Not","!"},
            {"DI","++"},
            {"DI","--"},

        };
            for (int j = 0; j < 22; j++)
            {
                if (oper[j, 1] == word)
                {
                    return oper[j, 0];
                }
            }

            return null;

        }



        bool IsId(string word)
        {
            Regex reg = new Regex("^[A-Za-z@]{1}[A-Za-z0-9@]{0,}$");//ID const final
            bool ans = reg.IsMatch(word);
            return ans;
        }

        bool IsIntConst(string word)
        {
            Regex reg = new Regex("^[+-]{0,1}[0-9]{1,}$");  //int const final
            bool ans = reg.IsMatch(word);
            return ans;
        }

        bool IsFloatConst(string word)
        {
            Regex reg = new Regex("^([+-]{0,1}[0-9]{0,}){0,1}[.]{1}[0-9]{1,}$");//float const final
            bool ans = reg.IsMatch(word);
            return ans;
        }

        bool IsStringConst(string word)
        {
            Regex reg = new Regex("^\"([a-zA-Z0-9 ]|(\\\\\\\\)|[~`!@#$%^&*()-=+{}|:;<>,.?/']|\\[|\\]|_|\\\\[abnrtv0f]|\"|\\\\\"|\\\\')*\"$");
            bool ans = reg.IsMatch(word);
            return ans;
        }
        bool IsCharConst(string word)
        {

            
            Regex reg = new Regex("'[a-zA-Z0-9 ]'|'(\\\\\\\\)'|'[~`!@#$%^&*()-=+{}|:;<>,.?/]'|'\\['|'\\]'|'_'|'\\\\[abnrtv0f]'|'\"'|'\\\\\"'|'\\\\''");

            if (reg.IsMatch(word) == true)
            { return true; }
         
            else
            { return false; }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
           
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        Class_Table ct1 ;
        public void getref(Class_Table c)
        {
            ct1 = c;
        }
        public void show_symbol(Class_Table c)
        {
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
         
            ct1.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 f1 = new Form1();
           
            this.Hide();
            f1.Show();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
        //public void form1_ref(Form1 f)
        //{
        //    f1 = f;
        //}
    }
}
