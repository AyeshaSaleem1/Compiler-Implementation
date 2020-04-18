using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        Class_Table ct = new Class_Table();
        public Form1()
        {

            InitializeComponent();
        }

        private void newToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (richTextBox1.Text != "")
            {
                exitPrompt();

                if (DialogResult == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click_1(sender, e);
                    richTextBox1.Text = "";
                    path = "";
                }
                else if (DialogResult == DialogResult.No)
                {
                    richTextBox1.Text = "";
                    path = "";
                }

            }
        }

        private void openToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.Text = File.ReadAllText(path = openFileDialog1.FileName);
            }
        }
     
        private void exitPrompt()
        {
            DialogResult = MessageBox.Show("Do you want to save current file?",
                "Notepad",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);
        }
        string path = "";
      
        private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        public void StreamWrite(string fPath)
        {
                TextWriter wri = new StreamWriter(path);
                for (int i = 0; i < richTextBox1.Lines.Length; i++)
                {
                    wri.WriteLine(richTextBox1.Lines[i]);
                }
                wri.Close();
        }

       

        private void saveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (path != "")
            {
                File.WriteAllText(path, richTextBox1.Text);
            }
            else
            {
                saveAsToolStripMenuItem_Click_1(sender, e);
            }

        }

        private void saveAsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                path = saveFileDialog1.FileName;
                StreamWrite(path);

            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            File.WriteAllLines(@"code_generator.txt", new string[0]);

            if (path != "")
            {
             
                Class_Table.class_node_list = new ArrayList();//new work ui
                Class_Table.function_scope = new ArrayList();//new work ui

                CT.attribute_AM = "";
                CT.attribute_cat = "";
                CT.attribute_type = "";
                CT.parent = "";
                CT.parent_cat = "";
                CT.parent_ref = null;
                CT.parent_type = "";
                Syntax.overloadcount = 0;
                Syntax.sym_error = "";
                Syntax.scope_stack = new Stack();
                Syntax.g = 0;
                Syntax.error = "";


                Form2 form2 = new Form2();
                Token token = new Token();
                token.clearFile();
                form2.StreamRead(path);
                form2.mainFun();
                form2.Show();
                form2.StreamRead1(token.filePath);
                token.add_endmarker();
                Syntax syn = new Syntax();
                this.Hide();
                form2.lexime();
                //ct.Hide();

                if (syn.s())
                {
                    
                    
                    form2.success("Succesfully compiled.............");
                    form2.getref(ct);
                //ct.Show();
                }
                else
                {
                    form2.error(Syntax.error + " ----> " + Syntax.sym_error);
                }
               
                    form2.code_gene();
            }
            else
            {
                MessageBox.Show("Save file first!");
            }
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void copyToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void pasteToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        private void selectAllToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
        }

        private void customizeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.Font = richTextBox1.Font = new Font(fontDialog1.Font, fontDialog1.Font.Style);
                richTextBox1.ForeColor = fontDialog1.Color;
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

     
    }
}
