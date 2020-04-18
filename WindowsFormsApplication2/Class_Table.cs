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

namespace WindowsFormsApplication2
{

    public partial class Class_Table : Form
    {
        public Class_Table()
        {
            InitializeComponent();
        }

        public static DataTable dt = new DataTable();

        public static ArrayList class_node_list = new ArrayList();
        public static ArrayList function_scope = new ArrayList();


         
        
        private void Class_Table_Load(object sender, EventArgs e)
        {
            dt = new DataTable();
            dt.Columns.Add("NAME", typeof(string));
            dt.Columns.Add("TYPE", typeof(string));
            dt.Columns.Add("PARENT", typeof(string));
            dt.Columns.Add("GT", typeof(string));
            dt.Columns.Add("AM", typeof(string));
            dt.Columns.Add("Ref", typeof(string));
            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].Width = 150;
            dataGridView1.Columns[1].Width = 130;
            dataGridView1.Columns[2].Width = 130;
            dataGridView1.Columns[3].Width = 130;
            dataGridView1.Columns[4].Width = 130;
            dataGridView1.Columns[5].Width = 130;
            dataGridView1.DataSource = dt;

            foreach (class_node i in class_node_list)
            {
               dt.Rows.Add(i.name, i.type, i.paernt, i.category, i.am, i.list);
          
            }
            int x = 5;
            DataTable class_definition_dt;

            foreach (class_node i in class_node_list)
            {
                Label ll = new Label();
                ll.Text = i.type+" " + i.name;
                ll.ForeColor = Color.Black;
                ll.Font = new Font("Constantia", 14, FontStyle.Bold);
                ll.SetBounds(x, 5, 300,45);
                class_definition_dt = new DataTable();
                class_definition_dt.Columns.Add("NAME", typeof(string));
                class_definition_dt.Columns.Add("TYPE", typeof(string));
                class_definition_dt.Columns.Add("CATEGORY", typeof(string));
                class_definition_dt.Columns.Add("AM", typeof(string));
                
                DataGridView dgv = new DataGridView();
                dgv.SetBounds(x, 40, 400, 400);
                dgv.BackgroundColor = Color.White;
                dgv.DataSource = class_definition_dt;
                
                

                
                foreach (array_node g in i.list)
                {
                    if (g.parameter_list == "static" || g.parameter_list == "---")
                    {
                    class_definition_dt.Rows.Add(g.name,g.return_type,g.parameter_list,g.AM);
                    }
                    else
                    {
                        class_definition_dt.Rows.Add(g.name, g.parameter_list + "===>" + g.return_type ,"---", g.AM);
                    }
                }
                dgv.DataSource = class_definition_dt;
                panel1.Controls.Add(dgv);
                panel1.Controls.Add(ll);
                x += 410;
            }

            DataTable function_table = new DataTable();
            function_table.Columns.Add("NAME", typeof(string));
            function_table.Columns.Add("TYPE", typeof(string));
            function_table.Columns.Add("SCOPE", typeof(string));
          
            foreach (Func_node g in function_scope)
            {
                function_table.Rows.Add(g.name, g.type, g.scope);
            }
            dataGridView2.DataSource = function_table;
            dataGridView2.Columns[0].Width = 120;
            dataGridView2.Columns[1].Width = 115;
            dataGridView2.Columns[2].Width = 115;


        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

    }
    public class class_node
    {

        public string name;
        public string type;
        public string am;
        public string category;
        public string paernt;

        public ArrayList list;
        public class_node(string n, string t, string par, string cat, string AM, ArrayList li)
        {
            name = n;
            type = t;
            am = AM;
            category = cat;
            paernt = par;
            list = li;

        }
    }
}
