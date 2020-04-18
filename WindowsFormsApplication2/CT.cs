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
    class CT

    {
       
        public static string parent_type = "", parent = "", attribute_type = "", attribute_cat = "", attribute_AM="";
        public static string parent_cat = "";
        public  ArrayList al = new ArrayList();//12365
        public node n;
        public static ArrayList parent_ref;
        
        public string name_class="";
        //public string name, type, cat, parent, am;
        public bool insert(string name, string type, string par, string cat, string am)
        {
            
            if (!lookup(name))
            {
               
                Class_Table.class_node_list.Add(new class_node(name, type, par, cat, am, al));
                return true;
               
            }
            return false;
        }
        public bool lookup(string name)
        {
            //for (int i = 0; i < Class_Table.dt.Rows.Count; i++)
            //{
            //    if (name == Class_Table.dt.Rows[i][0].ToString())
            //    {
            //        parent_type = Class_Table.dt.Rows[i][1].ToString();
            //        parent_cat = Class_Table.dt.Rows[i][3].ToString();
            //        name_class = Class_Table.dt.Rows[i][0].ToString();
            //    return true;
            //    }
            //}
            foreach (class_node i in Class_Table.class_node_list)
            {
                if (name == i.name)
                {
                    parent_type = i.type;
                    parent_cat = i.category;
                    name_class =  i.name;
                    parent = i.paernt;
                    parent_ref = i.list;
                    return true;
                }
            }
                return false;
        }
        public bool lookup_class(string name,string return_type,string parameterlist)
        {
            foreach (array_node a in al)
            {
                
                if (a.name == name)
                {
                    if (parameterlist == "" || parameterlist =="static")
                    {
                        attribute_cat = a.parameter_list;
                        attribute_type = a.return_type;
                        attribute_AM = a.AM;
                        return true;
                    }
                       
                    if(a.parameter_list==parameterlist)
                    {
                        attribute_cat = a.parameter_list;
                        attribute_type = a.return_type;
                        attribute_AM = a.AM;

                        return true;
                    }

                }
            }
            return false;
        }

        public bool insert_class(string name, string return_type, string parameterlist, string AM)
        {
            if (!lookup_class(name,return_type,parameterlist))
            {
            array_node ar = new array_node(name, return_type,parameterlist,AM);
            al.Add(ar);
            return true;

            }
            return false;
        }
        public bool insert_class_attribute(string name, string type, string cat, string AM)
        {
            
            if (!lookup_class(name, "", ""))
            {
                array_node ar = new array_node(name, type, cat, AM);
                al.Add(ar);
                return true;

            }
            return false;
        }

        public bool lookup_function(string name,int scope)
        {
            foreach (Func_node a in Class_Table.function_scope)
            {

                if (a.name == name)
                {
                    if (scope == a.scope)//int[]
                    {
                        attribute_type = a.type;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool insert_func(string name, string type, int scope)
        {
         
            if (!lookup_function(name, scope))
            {
                Class_Table.function_scope.Add(new Func_node(name,type,scope));
                return true;
            }
            return false;
        }


        /********************************datatype compatibility*************************************************/

        public string type_type(string type1, string type2, string oper,string class_part)  //type_compatibility()
        {
            
          


            if ((oper == "==" || oper == "!=") && ((type1 == type2) || (type1 == "float" && type2 == "int") || (type2 == "float" && type1 == "int") || (type1 == "int" && type2 == "char") || (type1 == "char" && type2 == "int") || (type1 == "float" && type2 == "char") || (type1 == "char" && type2 == "float")))
            {
               

                return "bool";
           }
            if (class_part == "ROP" && (oper != "==" || oper != "!="))
            {
                if ((type1 == "float" && type2 == "float") || (type1 == "int" && type2 == "int") || (type1 == "char" && type2 == "char") || (type2 == "float" && type1 == "int") || (type1 == "int" && type2 == "char")||(type1 == "char" && type2 == "int")|| (type1 == "float" && type2 == "char")||(type1 == "char" && type2 == "float"))
                {
                    return "bool";
                }
            }
           
            if ((class_part == "AND" || class_part == "OR") && (type1 == "bool" && type1 == "bool"))
            {
                return "bool";
            }
                //and , or bool constant ... < > <= >= float int.... == , != all
          
            else if (oper == "+" && (type1 == "string" || type2 == "string"))
            {
                
                return "string";
                
            }
            else if (oper == "+" || oper == "-" || oper == "*" || oper == "/" || oper == "%")
            {
                if ((type1 == "float" && type2 == "int") || (type1 == "float" && type2 == "float") || (type1 == "float" && type2 == "char"))
                {
                    return "float";
                }

                else if ((type1 == "int" && type2 == "int") || (type1 == "int" && type2 == "char"))
                {
                    
                    
                    
                    
                    return "int";
                }
            }
            
            return "type mismatch";
            
        }
        public string unary_type(string type, string oper)  //unary_compatibility
        {
            
            if (type == "bool")
            {
                return "bool";
            }

            return "invalid type";
            
        }
        public string dec_type_match(string type_R,string type_L)
        {
         
            if (type_L == type_R)
            {
                return type_L;
            }
            else if (type_L == "float" && (type_R == "int" || type_R == "char"))
            {
                return type_L;
            }
            else if (type_L == "int" && type_R == "char")
            {
                return type_L;
            }
            else if (type_R == "")
            {
                return type_L;
            }
                return "type mismatch";

            //class A{ B b = new C();
        
        }

        public bool this_check() 
        {
            return false;
        }


        public bool scope_lookup(string name)
        {
            foreach (int i in Syntax.scope_stack)
            {
                if (lookup_function(name, i))
                {
                    return true;
                }
            }
            
            return lookup_class(name,"","");
        }


        //public bool child_parent_lookup_interface(string name, string name1, string parameter_list)
        //{
        //    if (lookup(name1))
        //    {
        //        if ((parent == "" || parent == null))
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            string[] split_parent = parent.Split(',');//claa B:A,C{}

        //            for (int i = 0; i < split_parent.Length; i++)
        //            {
        //                if (split_parent[i] == name)
        //                {
        //                    if (lookup(name1))
        //                    {
        //                            goto L1;
        //                    }
        //                }
        //            }
        //            name1 = 
        //        }
        //    }
        //    return false;
        //}

        public bool child_parent_lookup(string name,string name1,string parameter_list)  //left,right
        {
            if (lookup(name1))   //A
            {
            L2:
                if ((parent == "" || parent == null))
                {
                    if (name == name1)
                    {
                        goto L1;
                    }
                  
                }
                    
                else
                {

                    if (name == name1)
                    {
                        goto L1;
                    }
                    string[] split_parent = parent.Split(',');//claa B:A,C{}
                    
                    for (int i = 0; i < split_parent.Length; i++)
                    {
                        if (split_parent[i] == name)
                        {
                            if (lookup(name1))
                            {
                                if (parent_type == "class")
                                {
                                    goto L1;

                                }
                                
                               
                            }
                        }
                    }
                    for (int i = 0; i < split_parent.Length; i++)
                    {
                        if (lookup(split_parent[i]))
                        {
                            if (parent_type == "class")
                            {
                                goto L2;
                            }
                        }
                    }
                }
            }

            parent = "";
            parent_cat = "";
            parent_ref = null;
            parent_type = "";
            return false;
           
      L1:
            if (parameter_list == "array object")
            {
                return true;
            }
            else
            {
                foreach (array_node a in parent_ref)
                {
                    if (a.return_type == "void_ctor")
                    {
                        if (a.parameter_list == parameter_list)
                        {
                            parent = "";
                            parent_cat = "";
                            parent_ref = null;
                            parent_type = "";
                            return true;
                        }
                        else
                        {
                            int c = 0;
                            string[] pl = a.parameter_list.Split(',');/////int,int,char
                            string[] pl1 = parameter_list.Split(',');//int char,int
                            if (pl.Length == pl1.Length)////
                            {
                                for (int i = 0; i < pl1.Length; i++)
                                {
                                    if ((pl[i].Contains("int") && (pl1[i].Contains("int") || pl1[i].Contains("char"))))
                                    {
                                        c++;
                                        continue;
                                    }
                                    else if ((pl[i].Contains("float") && (pl1[i].Contains("int") || pl1[i].Contains("float") || pl1[i].Contains("char"))))
                                    {
                                        c++;  
                                        continue;
                                    }
                                    else if (pl[i] == pl1[i])
                                    {
                                        c++;
                                        continue;

                                    }
                                    else if (pl1[i] != "void" )
                                    {
                                        return false;
                                    }
                                  

                                }

                                return true;
                            }
                        }

                    }


                }
            }

      return false;
        }

        public bool inherited_func_lookup(string classname, string name)
        {
            Syntax.overloadcount = 0;
            string multi_inherit = "";
        L2: if (lookup(classname))
            {

                if (parent != "" || parent != null)
                {

                    string[] split_parent = parent.Split(',');//claa B:A,C{}

                    for (int i = 0; i < split_parent.Length; i++)
                    {
                        lookup(split_parent[i]);
                        if (parent_type == "class")
                        {
                            multi_inherit = split_parent[i];
                            goto L1;
                        }
                    }
                }
            }
        L1:
            {
                foreach (array_node a in parent_ref)
                {
                    if (name == a.name)
                    {
                        Syntax.overloadcount++;

                        if (a.AM == "public")
                        {

                            attribute_cat = a.parameter_list;
                            attribute_type = a.return_type;
                            Syntax.sym_error = "";
                            countt++;
                          //  return true;
                        }
                        else
                        {
                            
                            Syntax.sym_error = "private variable can only be accessed within class";
                        }
                    }
                } 
            if (Syntax.overloadcount == 1 && Syntax.sym_error =="")
                {
                    countt = 0;
                    return true;
                }
            else if (Syntax.overloadcount > 1)
            {
                countt = 0;
                return true;
            }
                classname = multi_inherit;
                if (classname != "")
                { 
                           goto L2;
                }

            }
      
            
            return false;


        }
        int countt = 0;
        public bool child_parent_lookup_base(string name,string parameter_list)
        {


            if (lookup(name))
            {

                if (parent != "" || parent != null)
                {

                    string[] split_parent = parent.Split(',');//claa B:A,C{}

                    for (int i = 0; i < split_parent.Length; i++)
                    {
                        lookup(split_parent[i]);
                        if (parent_type == "class")
                        {
                            goto L1;
                        }
                    }

                }


            }
            

        L1: foreach (array_node a in parent_ref)
            {
                if (a.return_type == "void_ctor")
                {
                    if (a.parameter_list == parameter_list)
                    {
                        parent = "";
                        parent_cat = "";
                        parent_ref = null;
                        parent_type = "";
                        return true;
                    }
                    else
                    {
                        string[] pl = a.parameter_list.Split(',');/////int,int,char
                        string[] pl1 = parameter_list.Split(',');//int char,int
                        if (pl.Length == pl1.Length)////
                        {
                        for (int i = 0; i < pl1.Length; i++)
                        {
                            if((pl[i].Contains("int")  &&(pl1[i].Contains("int")||pl1[i].Contains("char"))) )
                            {
                                continue;
                            }
                            else if ((pl[i].Contains("float") && (pl1[i].Contains("int") || pl1[i].Contains("float") || pl1[i].Contains("char"))))
                            {
                                
                                continue;
                            }
                            else if (pl[i] == pl1[i])
                            {
                                continue;
                            }
                            else
                            {
                                return false;
                            }

                        }
                        return true;
                        }
                    }

                }

            }
            parent = "";
            parent_cat = "";
            parent_ref = null;
            parent_type = "";
            return false;
        }
       
        
    }
    class array_node
    {
        public string name;
        public string return_type;
        public string parameter_list, AM;
        
        public array_node(string name, string return_type, string parameterlist, string AM)
        {
            this.name = name;
            this.return_type = return_type;
            this.parameter_list = parameterlist;
            this.AM = AM;
            

        }
    } 

    class Func_node
    {
        public string name;
        public string type;
        public int scope;

        public Func_node(string name, string type, int scope)
        {
            this.name = name;
            this.type =type;
            this.scope = scope;

        }
    }}
   

