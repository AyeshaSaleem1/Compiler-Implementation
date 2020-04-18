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
    class Syntax
    {
        code_generator cg = new code_generator();
        public bool function_se_aya = false;
        public string oe_ic;
        public int i_ic;
        public bool main_exist = false;
        public int main_count = 0;
        public static string error = "";
        public static string sym_error = "";
        string[,] token_array = Token.token_array;
        public static int g = 0;
        int a = 0;
        int scope = 0;
        public static Stack scope_stack = new Stack();
        CT re;
        Object reff;
        public bool s()
        {
            int len = File.ReadAllLines("token.txt").Length;
            int openbracket = 0;
            int closebracket = 0;
            for (int i = 0; i < len; i++)
            {
                if (token_array[i, 0] == "{")
                {
                    openbracket++;
                }
                else if (token_array[i, 0] == "}")
                {
                    closebracket++;
                }
            }



            if (_class())
            {

                if (s())
                {

                    if (main_exist)//main exist symantic or syntax
                    {
                      
                        return true;
                      
                    }

                }
            }
            else if (_interface())//interface
            {

                if (s())
                {
                    return true;

                }
            }
            else if (token_array[a, 0] == "$" && closebracket == openbracket)
            {
                if (main_exist)
                {

                return true;
                }

            }
            else if ( main_count==0 && sym_error == "")
            {

                error = "One Main function must be present in a project";

            }
            else if (token_array[a, 0] == "$" && closebracket != openbracket)
            {

                error = "bracket missing";

            }
            else if (main_count > 1)
            {
                error = "only one main function is allowed in whole project";

            }
            else
            {

                error = "Error occur at line number = " + token_array[a, 2];
                return false;
            }
            if (main_exist == false && sym_error == "")
            {
                error = "One Main function must be present in a project";
                return false;

            }
            return false;

        }

        public bool _class()//AM <sealed> class ID <inherit_interface> {<non_class_MST>}
        {
            string am, name, type, cat, par;
            if (token_array[a, 0] == "AM")
            {
                am = token_array[a, 1];
                a = a + 1;
                string name_ic = "";
                if (_sealed(out cat,out name_ic))//add<sealed>
                {
                    if (token_array[a, 0] == "class")
                    {
                        type = token_array[a, 1];
                        a = a + 1;
                        if (token_array[a, 0] == "ID")
                        {
                            string tem_ic;
                            if (name_ic!="")
                            {
                                tem_ic = name_ic + "Class_" + token_array[a, 1];
                            }
                            else
                            {
                                tem_ic = "General_Class_" + token_array[a, 1];
                            }
                            name = token_array[a, 1];
                            
                            a = a + 1;



                            if (inherit_interface(out par))// add <inherit_interface>
                            {
                                

                                re = new CT();
                                if (!re.insert(name, type, par, cat, am))
                                {
                                    sym_error = "redeclaration of classs" + par;
                                    return false;
                                }
                                Name1 = name;


                                if (token_array[a, 0] == "{")
                                {
                                    a = a + 1;

                                    if (non_class_MST(tem_ic))// add <non_class_MST>
                                    {
                                        if (token_array[a, 0] == "}")
                                        {
                                            if(!re.lookup_class(name,"void_ctor","void"))
                                            {
                                                re.insert_class(name,"void_ctor","void","public");
                                            }
                                            a = a + 1;
                                            return true;
                                        }

                                    }
                                }
                            }
                        }

                    }
                }

            }


            return false;

        }
        string Name1;
        public bool _sealed(out string cat,out string name_ic)//sealed|€
        {
            name_ic = "";
            if (token_array[a, 0] == "sealed")
            {
                name_ic = "Sealed_";
                a = a + 1;
                cat = "sealed";
                return true;

            }
            else if (token_array[a, 0] == "class")
            {
                cat = "General";
                return true;
            }
            cat = "General";
            return false;
        }
        public bool inherit_interface(out string par)//: ID | <multi_interface>| €
        {

            bool class_flag = false;
            par = "";
            if (token_array[a, 0] == ":")
            {
                a = a + 1;

                if (token_array[a, 0] == "ID")
                {
                    
                    if (!re.lookup(token_array[a, 1]))
                    {
                        sym_error = "Parent not exist..";
                        return false;
                    }
                    else if (CT.parent_type == "class")
                    {
                        class_flag = true;
                        if (CT.parent_cat == "sealed")
                        {
                            sym_error = "sealed class can't be inherit";
                            return false;
                        }

                        else
                        {
                            par = token_array[a, 1];
                            CT.parent_type = "";
                            CT.parent_cat = "";
                        }

                    }
                    else if (CT.parent_type == "interface")
                    {

                        par = token_array[a, 1];
                        CT.parent_type = "";
                        CT.parent_cat = "";


                    }

                    a = a + 1;
                    if (multi_interface(out par, par, class_flag))
                    {
                        return true;
                    }
                }
            }

            else if (token_array[a, 0] == "{")// null 
            {
            
                return true;
            }
            
            return false;
        }
        public bool multi_interface(out string out_par, string in_par, bool class_flag)//<multi_interface>→,ID<multi_interface>|€
        {
            
            
            out_par = in_par;
            if (token_array[a, 0] == ",")
            {
                a = a + 1;
                if (token_array[a, 0] == "ID")
                {


                    if (!re.lookup(token_array[a, 1]))
                    {
                        sym_error = "Parent not exist..";


                        return false;
                    }
                    else if (CT.parent_type == "class" && !class_flag)
                    {
                        class_flag = true;
                        if (CT.parent_cat == "sealed")
                        {
                            sym_error = "sealed class can't be inherit";
                            return false;

                        }

                        else
                        {
                            out_par = out_par + "," + token_array[a, 1];
                            CT.parent_type = "";
                            CT.parent_cat = "";
                        }

                    }
                    else if (CT.parent_type == "interface")
                    {

                        out_par = out_par + "," + token_array[a, 1];
                        CT.parent_type = "";
                        CT.parent_cat = "";

                    }
                    else if (CT.parent_type == "class" && class_flag)
                    {
                        sym_error = "multiple class can't be inherit";
                        return false;
                    }

                    a = a + 1;
                    CT.parent_type = "";
                    CT.parent_cat = "";
                    if (multi_interface(out out_par, out_par, class_flag))
                    {
                        return true;
                    }
                }
            }
            else if (token_array[a, 0] == "{") //null
            {
                return true;
            }
            return false;
        }


        public bool _interface()//interface ID { <func_interface> }
        {
            string name, type,par = "";
            if (token_array[a, 0] == "interface")
            {
                string tem_ic = token_array[a, 0];
                type = token_array[a, 1];
                a = a + 1;
                if (token_array[a, 0] == "ID")
                {
                    tem_ic += "_" + token_array[a, 1];//ic_code

                    name = token_array[a, 1];
                    a = a + 1;

                    re = new CT();
                    scope_stack.Push(++scope);
                    if(inherit_interface_1(out par)){
                    if (!re.insert(name, type, par , "General", "public"))
                    {
                        sym_error = "redeclaration of interface";
                        return false;
                    }
                    if (token_array[a, 0] == "{")
                    {
                        a = a + 1;
                        if (func_interface(tem_ic))//add <func_interface>
                        {
                            if (token_array[a, 0] == "}")
                            {
                                scope_stack.Pop();
                                a = a + 1;
                                return true;

                            }

                        }

                    }

                }
            }
            }
            return false;

        }

        public bool func_interface(string tem_ic)//<ret_T> ID (<parameter>); <func_interface>|€
        {
            string tem2_ic;
            string name, type, type1, category;
            type1 = "";


            if (ret_T_S(out type,out tem2_ic,tem_ic))//add <ret_T>
            {
               
                if (token_array[a, 0] == "ID")
                {
                    tem2_ic += "_" + token_array[a, 1];//ic_code
                    name = token_array[a, 1];

                    scope_stack.Push(++scope);  

                    a = a + 1;
                    if (token_array[a, 0] == "(")
                    {
                        
                        a = a + 1;
                        string temp_ic;
                        if (parameter_S(out type1, type1,out temp_ic,tem2_ic))//<parameter>
                        {

                            if (token_array[a, 0] == ")")
                            {
                                scope_stack.Pop();
                                if (type1 == "")
                                {
                                    type1 = "void";
                                }
                                if (!re.insert_class(name, type, type1, "public"))
                                {
                                    sym_error = "redeclartion of function";
                                    return false;
                                }
                                temp_ic += " proc";
                                cg.create_label(temp_ic);
                                a = a + 1;
                                if (token_array[a, 0] == ";")
                                {
                                    cg.create_label("end proc");
                                    a = a + 1;
                                    if (func_interface(tem_ic))
                                    {
                                        return true;
                                    }

                                }

                            }
                        }

                    }

                }
            }
            else if (token_array[a, 0] == "}")
            {
                return true;
            }
            return false;
        }

        public bool inherit_interface_1(out string par)//: ID | <multi_interface>| €
        {
            par = "";
            if (token_array[a, 0] == ":")
            {
                a = a + 1;

                if (token_array[a, 0] == "ID")
                {

                    if (!re.lookup(token_array[a, 1]))
                    {
                        sym_error = "Parent not exist..";
                        return false;
                    }
                    else if (CT.parent_type == "class")
                    {
                        sym_error = "Type ' " + token_array[a, 1] + "' in interface list is not an interface	";
                        return false;
                    }
                    else if (CT.parent_type == "interface")
                    {

                        par = token_array[a, 1];
                        CT.parent_type = "";
                        CT.parent_cat = "";


                    }

                    a = a + 1;
                    if (multi_interface_1(out par, par))
                    {
                        return true;
                    }
                }
            }

            else if (token_array[a, 0] == "{")// null 
            {

                return true;
            }

            return false;
        }
        public bool multi_interface_1(out string out_par, string in_par)//<multi_interface>→,ID<multi_interface>|€
        {


            out_par = in_par;
            if (token_array[a, 0] == ",")
            {
                a = a + 1;
                if (token_array[a, 0] == "ID")
                {


                    if (!re.lookup(token_array[a, 1]))
                    {
                        sym_error = "Parent not exist..";


                        return false;
                    }
                    else if (CT.parent_type == "class" )
                    {
                        sym_error = "Type ' " + token_array[a, 1] + "' in interface list is not an interface	";
                        return false;
                    }
                    else if (CT.parent_type == "interface")
                    {

                        out_par = out_par + "," + token_array[a, 1];
                        CT.parent_type = "";
                        CT.parent_cat = "";

                    }
                 

                    a = a + 1;
                    CT.parent_type = "";
                    CT.parent_cat = "";
                    if (multi_interface_1(out out_par, out_par))
                    {
                        return true;
                    }
                }
            }
            else if (token_array[a, 0] == "{") //null
            {
                return true;
            }
            return false;
        }


        public bool list(string name,string type,string name_in_ic)//<list> → <list1> | = <L1>
        {
            if (list1(name,type,name_in_ic))//add <list1>
            {
                return true;
            }
            else if (token_array[a, 0] == "Equal")
            {
                name_in_ic += token_array[a, 1];
                a = a + 1;
                
                if (token_array[a - 2, 0] == "ID")//7=9 not satisfied
                {
                    if (L_Dec(name,type,name_in_ic))//ADD <L1>
                    {

                        return true;
                    }
                }

            }
            return false;
        }
        public bool list1(string name, string type,string name_in_ic)//<list1> → ; | ,ID<list>
        {
            if (token_array[a, 0] == ";")
            {
                if (!re.insert_func(name, type, Convert.ToInt32(scope_stack.Peek())))
                {
                    sym_error = "redeclaration of attribute";
                    return false;
                }
                cg.create_label(name_in_ic);
                a = a + 1;
                return true;
            }
            else if (token_array[a, 0] == ",")
            {
                if (!re.insert_func(name, type, Convert.ToInt32(scope_stack.Peek())))
                {
                    sym_error = "redeclaration of attribute";
                    return false;
                }
                cg.create_label(name_in_ic);
                name_in_ic = name_in_ic.Split('_')[0];
                a = a + 1;
                if (token_array[a, 0] == "ID")
                {
                    name_in_ic += "_" + token_array[a, 1];
                    name = token_array[a, 1];
                    a = a + 1;
                    if (list(name,type,name_in_ic))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool L_Dec(string name,string type,string name_in_ic)//<L1> →<OE> <list>   
        {
           
           
           
            string type_R;
            if (OE(out type_R))
            {
                if (type_R.Contains("["))
                {
                    type_R = type_R.Split('[')[0];
                }
                type = re.dec_type_match(type_R, type);
                if (type == "type mismatch")
                {
                    sym_error = "invalid Assignment to local variable ";
                    return false;
                }
                name_in_ic = name_in_ic + oe_ic;
                if (list(name,type,name_in_ic))
                {
                    return true;
                }
            }
            return false;
        }

        public bool A_Array(string name,string type,out string name_ic,string name_in_ic)//,<OE>] = {<2D_cond>}; | ] = {<condition>};   
        {
            name_ic = name_in_ic;
            string arguments,type2,type_check;
            type_check = type.Split('[')[0];
            if (token_array[a, 0] == ",")//,<OE>] = {<2D_cond>};
            {
                name_in_ic += token_array[a,0];
                a = a + 1;
                if (OE(out type2))
                {
                    if (type2 != "int" && type2 != "char")
                    {

                        sym_error = "index must be integer";
                        return false;
                    }
                    if (token_array[a, 0] == "]")
                    {
                        name_in_ic +=oe_ic+ token_array[a, 0];
                        type += ",]";   
                        a++;
                        if (token_array[a, 0] == "Equal")
                        {
                            name_in_ic += token_array[a, 1];
                            a++;
                            if (token_array[a, 0] == "{")
                            {
                                name_in_ic += token_array[a, 0];
                                a++;
                                if (_2D_cond(type_check))
                                {
                                    name_in_ic += ic_code;
                                    if (token_array[a, 0] == "}")
                                    {
                                        name_in_ic += token_array[a, 0];

                                        a++;
                                        if (token_array[a, 0] == ";")
                                        {
                                            if (!re.insert_func(name, type, Convert.ToInt32(scope_stack.Peek())))
                                            {
                                                sym_error = "redeclaration of local variable -> \"" + name + "\"";
                                                return false;
                                            }
                                            name_in_ic += token_array[a, 0];
                                            cg.create_label(name_in_ic);
                                            ic_code = "";
                                            a++;

                                            return true;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }

            else if (token_array[a, 0] == "]")//] = {<condition>};
            {
                type += "]";
                a++;
                if (token_array[a, 0] == "Equal")
                {
                    a++;
                    if (token_array[a, 0] == "{")
                    {
                        name_in_ic += "]={";
                        a++;
                        if (condition(out arguments,out name_ic,name_in_ic))
                        {
                            name_in_ic = name_ic;
                            if (arguments.Contains(','))
                            {

                                string[] argument1 = arguments.Split(',');
                                for (int i = 0; i < argument1.Length; i++)
                                {
                                    type_check = re.dec_type_match(argument1[i],type_check);

                                if (type_check == "type mismatch")
                                {
                                    sym_error = "invalid Assignment to local variable ";
                                    return false;
                                }
                                }

                            }
                            else
                            {
                                type_check = re.dec_type_match(arguments,type_check);

                                if (type_check == "type mismatch")
                                {
                                    sym_error = "invalid Assignment to local  variable ";
                                    return false;
                                }

                            }
                            if (token_array[a, 0] == "}")
                            {
                                a++;
                                if (token_array[a, 0] == ";")
                                {

                                    if (!re.insert_func(name, type, Convert.ToInt32(scope_stack.Peek())))
                                    {
                                        sym_error = "redeclaration of local variable -> \"" + name + "\"";
                                        return false;
                                    }
                                    name_in_ic += "};";
                                    cg.create_label(name_in_ic);
                                    ic_code = "";
                                    a++;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        public string ic_code;
        public bool _2D_cond(string type)//{<condition>}<2D_cond1>
        {
            string arguments;
            if (token_array[a, 0] == "{")
            {
                ic_code += "{";
                a++;
                string name_ic;
                if (condition(out arguments,out name_ic,ic_code))   //condition_MST(out type,type.Split('[')[0])
                {



                    if (arguments.Contains(','))
                    {

                        string[] argument1 = arguments.Split(',');
                        for (int i = 0; i < argument1.Length; i++)
                        {
                            type = re.dec_type_match(argument1[i], type);

                            if (type == "type mismatch")
                            {
                                sym_error = "invalid Assignment to local variable ";
                                return false;
                            }
                        }

                    }
                    else
                    {
                        type = re.dec_type_match(arguments, type);

                        if (type == "type mismatch")
                        {
                            sym_error = "invalid Assignment to local  variable ";
                            return false;
                        }

                    }

                    if (token_array[a, 0] == "}")
                    {
                        ic_code += "}";
                        a++;
                        if (_2D_cond1(type))
                        {
                            return true;
                        }
                    }
                }
            }


            return false;
        }

        public bool _2D_cond1(string type)//,<2D_cond> | €
        {
            if (token_array[a, 0] == ",")
            {
                ic_code += ",";
                a++;
                if (_2D_cond(type))
                {
                    return true;
                }
            }
            else if (token_array[a, 0] == "}")
            {

                return true;
            }
            return false;
        }

        public bool condition(out string arguments,out string name_ic,string name_in_ic)
        {
            name_ic = name_in_ic;
            arguments = "";
            if (argument(out arguments,arguments,out name_ic,name_in_ic))
            {
                
                return true;
            }
            else if (token_array[a, 0] == "}" || token_array[a, 0] == ")")
            {
                return true;
            }

            return false;
        }

        public bool argument(out string type,string type_in,out string name_ic,string name_in_ic)//<argument> → <OE><a_arguments> | null
        {
            name_ic = name_in_ic;
            type = type_in;
         
            if (OE(out type))
            {
                name_in_ic += "_" + oe_ic;
              type_in += type;
                if (a_argument(out type,type_in,out name_ic,name_in_ic))
                {
                    
                    return true;
                }
            }
            else if (token_array[a, 0] == ")")
            {
                if (type == "")
                {
                    type = "void";
                }
                return true;
            }

            return false;
        }
        public Stack s1_ic = new Stack();
        public bool a_argument(out string type, string type_in, out string name_ic, string name_in_ic)//<a_argument> → ,<argument> | €
        {
            name_ic = name_in_ic;
            type = type_in;
            if (token_array[a, 0] == ",")
            {
                s1_ic.Push(oe_ic);
                i_ic++;
                type_in += token_array[a, 1];
                a++;
                if (argument(out type,type_in,out name_ic,name_in_ic))
                {
                    return true;
                }
            }
            else if (token_array[a, 0] == "}" || token_array[a, 0] == ")")
            {
                s1_ic.Push(oe_ic);
                for (int i = 0; i < i_ic; i++)
                {
                    try
                    {
                        string s_ic = s1_ic.Pop().ToString();
                        if (function_se_aya)
                        {
                            cg.create_label("param: " + s_ic);
                        }
                    }
                    catch (Exception e)
                    {
                        
                        
                    }
                }
                return true;
            }

            return false;
        }

        /*********** new code ******************/
        public bool A_Array_MST(string name, string type, string AM)//,<OE>] = {<2D_cond>}; | ] = {<condition>};   
        {
            string arguments, type2;
            if (token_array[a, 0] == ",")//,<OE>] = {<2D_cond>};
            {

                a = a + 1;
                if (OE_class_MST(out type2))
                {
                    if (type2 != "int" && type2 != "char")
                    {

                        sym_error = "index must be integer";
                        return false;
                    }
                    if (token_array[a, 0] == "]")
                    {
                        type += ",]";
                        a++;
                        if (token_array[a, 0] == "Equal")
                        {
                            a++;
                            if (token_array[a, 0] == "{")
                            {
                                a++;
                                if (_2D_cond_MST(type))
                                {
                                    if (token_array[a, 0] == "}")
                                    {
                                        a++;
                                        if (token_array[a, 0] == ";")
                                        {
                                            if (!re.insert_class_attribute(name,type,"---",AM))
                                            {
                                                sym_error = "redeclaration of global variable -> \"" + name + "\"";
                                                return false;
                                            }
                                            a++;
                                            return true;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
            else if (token_array[a, 0] == "]")//] = {<condition>};public int a[]={5,6}, int[,]
            {
                type += "]";
                a++;
                if (token_array[a, 0] == "Equal")
                {
                    a++;
                    if (token_array[a, 0] == "{")
                    {
                        a++;
                        if (condition_MST( out arguments,type.Split('[')[0]))  //iint,int
                        {
                            if (token_array[a, 0] == "}")
                            {
                                a++;
                                if (token_array[a, 0] == ";")
                                {
                                    if (!re.insert_class_attribute(name, type, "---", AM))
                                    {
                                        sym_error = "redeclaration of global variable -> \"" + name + "\"";
                                        return false;
                                    }
                                    a++;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool _2D_cond_MST(string type)//{<condition>}<2D_cond1>
        {
            
            if (token_array[a, 0] == "{")
            {
                a++;
                if (condition_MST(out type,type.Split('[')[0]))
                {
                    if (token_array[a, 0] == "}")
                    {
                        a++;
                        if (_2D_cond1_MST(type))
                        {
                            return true;
                        }
                    }
                }
            }


            return false;
        }

        public bool _2D_cond1_MST(string type)//,<2D_cond> | €
        {
            if (token_array[a, 0] == ",")
            {
                a++;
                if (_2D_cond_MST(type))
                {
                    return true;
                }
            }
            else if (token_array[a, 0] == "}")
            {

                return true;
            }
            return false;
        }

        public bool condition_MST(out string type, string type_in)//<argument> → <OE><a_arguments> | null
        {
            type = "";                                   //int

            if (OE_class_MST(out type))
            {
                type = re.dec_type_match(type, type_in);

                if (type == "type mismatch")
                {
                    sym_error = "invalid Assignment to global variable ";
                    return false;
                }
                type_in = type;
                if (condition_MST_1(out type, type_in))
                {

                    return true;
                }
            }
            
            else if (token_array[a, 0] == "}")
            {
                return true;
            }
            return false;
        }
        public bool condition_MST_1(out string type, string type_in)//<a_argument> → ,<argument> | €
        {
            type = type_in;
            if (token_array[a, 0] == ",")
            {
              
                a++;
                if (condition_MST(out type, type_in))
                {
                    return true;
                }
            }
            else if (token_array[a, 0] == "}" || token_array[a, 0] == ")")
            {
                return true;
            }

            return false;
        }
        /**********************************************new function end*****************************************************/
        int temp_scope = 0;
        public bool TCF(string type) //<TCF> → try{ < non_func_MST> }catch {< non_func_MST>} finally {<non_func_MST>}
        {
            if (token_array[a, 0] == "try")
            {
                a = a + 1;
                
                scope_stack.Push(++scope);
                if (token_array[a, 0] == "{")
                {
                    a = a + 1;
                    if (non_func_MST(type))
                    {

                        if (token_array[a, 0] == "}")
                        {
                            scope_stack.Pop();
                            
                            
                            a = a + 1;
                            if (token_array[a, 0] == "catch")
                            {
                                a = a + 1;
                                if (token_array[a, 0] == "{")
                                {
                                    
                                    scope_stack.Push(++scope);
                                    a = a + 1;

                                    if (non_func_MST(type))
                                    {

                                        if (token_array[a, 0] == "}")
                                        {
                                            scope_stack.Pop();
                                            
                                            a = a + 1;
                                            if (token_array[a, 0] == "finally")
                                            {
                                              
                                                a = a + 1;

                                                if (token_array[a, 0] == "{")
                                                {
                                                    
                                                    scope_stack.Push(++scope);
                                                    a = a + 1;
                                                    if (non_func_MST(type))
                                                    {


                                                        if (token_array[a, 0] == "}")
                                                        {
                                                            scope_stack.Pop();
                                                           
                                                            a = a + 1;
                                                            return true;
                                                        }

                                                    }


                                                }

                                            }

                                        }


                                    }



                                }


                            }

                        }
                    }
                }
            }
            return false;
        }


        public bool Dec_in() //<Dec_inc> → DI | €
        {

            if (token_array[a, 0] == "DI")
            {


                a = a + 1;
                return true;
            }
            else if (token_array[a, 0] == "MDM" || token_array[a, 0] == "PM" || token_array[a, 0] == "ROP" || token_array[a, 0] == "AND" || token_array[a, 0] == "OR" || token_array[a, 0] == ";" || token_array[a, 0] == "," || token_array[a, 0] == "Equal" || token_array[a, 0] == "]" || token_array[a, 0] == "AsgOp" || token_array[a, 0] == "}" || token_array[a, 0] == ")")
            {
                return true;
            }

            return false;
        }

        public bool ID(string name, out string type, out string name_ic, string name_in_ic) //<ID>→.ID<ID>|€ 
        {
           
            name_ic = name_in_ic;
            type = "";
            if (token_array[a, 0] == ".")
            {
                if (!re.lookup(CT.attribute_type))///A a B b a.b
                {
                    CT.parent_type = "";
                    if (!re.lookup(name))
                    {
                    sym_error = name + " is not a reference variable";
                    return false;
                    }
                    _name = name;

                }
                else
                {
                  
                    CT.parent_type = "";
                }
                a = a + 1; ;
                inherit_func = false;
                type = CT.attribute_type;
                if (token_array[a, 0] == "ID")
                {
                    name = token_array[a, 1];
                    count++;
                L1:
                    if (count == 0)
                    {
                        if (!re.lookup_class(name, "", ""))
                        {
                            sym_error = "undeclared global variable -- '" + name + " '";
                            return false;
                        }
                        count++;
                    }
                    else
                    {
                        overloadcount = 0;
                        foreach (array_node aaa in CT.parent_ref)
                        {

                            if (name == aaa.name)
                            {

                                CT.attribute_type = aaa.return_type;
                                CT.attribute_cat = aaa.parameter_list;
                                CT.attribute_AM = aaa.AM;
                                overloadcount++;
                                
                            }
                        }
                        //if inherited member is called by child object
                        if (CT.parent != "" && overloadcount == 0)
                        {
                            string[] split_parent = CT.parent.Split(',');//claa B:A,C{}

                            for (int i = 0; i < split_parent.Length; i++)
                            {
                                if (re.lookup(split_parent[i]))
                                {
                                    if (CT.parent_type == "class")
                                    {
                                        CT.parent_type = "";
                                        goto L1;
                                    }
                                }
                            }

                        }          
                        if (overloadcount == 0)
                        {
                            sym_error = "undeclared variable " + name + " in class "+ _name;
                            return false;
                        }

                    }
                    if (CT.attribute_cat == "static")
                    {
                        if (_name != re.name_class)
                        {
                            sym_error = "Static variable '" + name + "' cannot be accessed with an instance reference";
                            return false;
                        }
                      
                    }
                    if (CT.attribute_AM == "private" && count-1 !=0)
                    {
                        sym_error = "Private variable '" + name + "' can be accessed only within class";
                        return false;
                    }
                    if ((CT.parent_type == "class" || CT.parent_type == "interface") && CT.attribute_cat != "static")
                    {
                        sym_error = " '" + name + "' is not a static variable";
                        return false;
                    }
                    
                    name_in_ic += "_" + token_array[a, 1];

                    a = a + 1;

                    if (ID(name,out  type,out name_ic,name_in_ic))
                    {

                        return true;
                    }

                }
            }
            else if (token_array[a, 0] == "[" || token_array[a, 0] == "," || token_array[a, 0] == "AsgOp" || token_array[a, 0] == "Equal" || token_array[a, 0] == "(" || token_array[a, 0] == "DI" || token_array[a, 0] == ";" || token_array[a, 0] == ")")
            {
                type = CT.attribute_type;
             
                return true;
            }

            return false;
        }
        string _name;
        public bool _base(string name)//<base>→:base(<argument>)|€:base("",2,"") //  
        {
            string arguments = "";
            if (token_array[a, 0] == ":")
            {
                a = a + 1;
                if (token_array[a, 0] == "base")
                {
                    a = a + 1;
                    if (token_array[a, 0] == "(")
                    {
                        a = a + 1;
                        string name_in;
                        if (argument(out arguments,arguments,out name_in,""))
                        {

                            if (!re.child_parent_lookup_base(name, arguments))
                            {
                                sym_error = "parent constructor doesn't exist or invalid arguments or parent doesn't exist....";
                                return false;

                            }

                            
                            if (token_array[a, 0] == ")")
                            {
                                a = a + 1;
                                return true;
                            }

                        }

                    }

                }

            }
            else if (token_array[a, 0] == "{")
            {

                return true;
            }
            return false;
        }


    
        public bool _while(string type1)//<while_loop>→while(<OE>){<non_func_MST>}
        {
            if (token_array[a, 0] == "while")
            {
                string l1_ic = cg.label();
                cg.create_label(l1_ic + ":");
                a = a + 1;
                if (token_array[a, 0] == "(")
                {
                    string type;
                    a = a + 1;
                    if (OE(out type))
                    {
                        if (type != "bool")
                        {
                            sym_error = "Condition must be boolean";
                            return false;
                        }
                        if (token_array[a, 0] == ")")
                        {
                            string l2_ic = cg.label();
                            cg.create_label("if(" + oe_ic + "==false) jmp" + l2_ic);
                            a = a + 1;
                            
                            scope_stack.Push(++scope);
                           
                            if (token_array[a, 0] == "{")
                            {
                                a = a + 1;
                                if (non_func_MST(type1)) //add <non_func_MST>
                                {
                                    cg.create_label("jmp" + l1_ic);
                                    if (token_array[a, 0] == "}")
                                    {
                                        cg.create_label(l2_ic + ":");
                                        scope_stack.Pop();
                                        
                                        a = a + 1;
                                        return true;

                                    }
                                }
                            }
                        }

                    }

                }

            }
            return false;

        }
        public int if_else_count = 0;
        public bool if_else(int Scope,string type1,string l_ic)//<if-else>→ if(<OE>){<non_func_MST>}<else>    
        {
            string type;
            if (token_array[a, 0] == "if")
            {
                if (l_ic!="")
                {
                    cg.create_label(l_ic + ":");
                }
                a = a + 1;
                scope_stack.Push(++scope);
                
                if (token_array[a, 0] == "(")
                {
                    a = a + 1;
                    if (OE(out type))
                    {
                        if (type != "bool")
                        {
                            sym_error = "Condition must be boolean";
                            return false;
                        }
                        if (token_array[a, 0] == ")")
                        {
                            string l1_ic = cg.label();
                            cg.create_label("if(" + oe_ic + "==false) jmp" + l1_ic);
                            
                            a = a + 1;
                            if (token_array[a, 0] == "{")
                            {
                                a = a + 1;
                                if (non_func_MST(type1))//add <non_func_MST>
                                {
                                    if (token_array[a, 0] == "}")
                                    {
                                        scope_stack.Pop();
                                        
                                        a = a + 1;
                                        if (token_array[a, 0] == "else")
                                        {

                                            if_else_count++;
                                        }
                                        else
                                        {

                                            StringBuilder newFile = new StringBuilder();

                                            string temp_ic = "";

                                            string[] file = File.ReadAllLines("code_generator.txt");

                                            Stack q = new Stack();
                                            foreach (string line in file.Reverse())
                                            {

                                                try
                                                {
                                                    if (line.Substring(0, 3) == "jmp" && if_else_count > 0)
                                                    {
                                                        q.Push(line);

                                                        if_else_count--;
                                                    }

                                                }
                                                catch (Exception e)
                                                {

                                                }

                                            }
                                            string temp_icc = "";

                                            if (q.Count > 0)
                                            {
                                                temp_icc = q.Pop().ToString();
                                            }

                                            foreach (string line in file)
                                            {

                                                if (line.Contains(temp_icc) && temp_icc != "")
                                                {

                                                    temp_ic = line.Replace(line, "jmp" + l1_ic);


                                                    if (q.Count != 0)
                                                    {

                                                        temp_icc = q.Pop().ToString();



                                                    }


                                                    newFile.Append(temp_ic + "\r\n");

                                                    continue;
                                                }


                                                newFile.Append(line + "\r\n");

                                            }

                                            File.WriteAllText("code_generator.txt", newFile.ToString());


                                        }
                                      


                                        if (_else(scope,type1,l1_ic)) //add <else>
                                        {
                                            return true;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }


            return false;
        }

        public bool _else(int scope,string type,string l1_ic)//<else> → else <else_if> | €
        {
            if (token_array[a, 0] == "else")
            {
                string l2_ic = cg.label();
                cg.create_label("jmp " + l2_ic);
                a = a + 1;
                if (else_if(type,l1_ic,l2_ic))//add <else_if>
                {
                    return true;
                }
            }
            else if (token_array[a, 0] == "DT" || token_array[a, 0] == "this" || token_array[a, 0] == "ID" || token_array[a, 0] == "for" || token_array[a, 0] == "while" || token_array[a, 0] == "if" || token_array[a, 0] == "try" || token_array[a, 0] == "DI" || token_array[a, 0] == "return" || token_array[a, 0] == "break" || token_array[a, 0] == "continue" || token_array[a, 0] == "}")
            {
                cg.create_label("jmp" + l1_ic);
                cg.create_label(l1_ic + ":");
                if_else_count = 0;
                return true;
            }
            return false;

        }
        public bool else_if(string type,string l1_ic,string l2_ic)//<else_if> → {<non_func_MST>} | <if-else>
        {
            if (token_array[a, 0] == "{")
            {
                cg.create_label(l1_ic + ":");

                scope_stack.Push(++scope);
                a = a + 1;
                if (non_func_MST(type))//add <non_func
                {
                    if (token_array[a, 0] == "}")
                    {
                        cg.create_label(l2_ic + ":");
                       scope_stack.Pop();
                        
                        a = a + 1;
                        return true;
                    }

                }

            }
            else if (if_else(scope,type,l1_ic))
            {
                return true;
            }
            return false;

        }

        //        <for_loop> → for(<c1><c2>;<c3>){<non_func_MST>}
        //<c1> → DT ID <list> | <c3>;
        //<c2> → <OE> | € 
        //<c3> → <Asg_Op_loop> |€

        public bool enter_ic = false;
        public int concat_count_ic = 0;
        public bool for_loop(string type)
        {
            string name_ic;
            
            if (token_array[a, 0] == "for")
            {
                enter_ic = false;
                concat_count_ic = 0;
                a++;
               
                scope_stack.Push(++scope);
                if (token_array[a, 0] == "(")
                {
                    a++;
                    if (c1(out name_ic))
                    {
                        string l1_ic = cg.label();
                        cg.create_label(l1_ic + ":");
                        if (c2())
                        {
                            string l2_ic = cg.label();
                            cg.create_label("if(" + oe_ic + "==false) jmp" + l2_ic);

                            if (token_array[a, 0] == ";")
                            {
                                a++;
                                if (c3(out name_ic))
                                {

                                    if (token_array[a, 0] == ")")
                                    {
                                        a++;
                                        if (token_array[a, 0] == "{")
                                        {
                                            a++;
                                            if (non_func_MST(type))
                                            {
                                                foreach (string get in for_inc_ic)
                                                {
                                                    if (get.Contains(","))
                                                    {
                                                        for (int i = 0; i < get.Split(',').Length; i++)
                                                        {
                                                            cg.create_label(get.Split(',')[i]);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        cg.create_label(get);
                                                    }
                                                }
                                                for_inc_ic = new Queue();
                                                cg.create_label("jmp " + l1_ic);
                                                if (token_array[a, 0] == "}")
                                                {
                                                    cg.create_label(l2_ic + ":");
                                                    scope_stack.Pop();
                                                   
                                                    a++;
                                                    return true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }






        public bool c1(out string name_ic)//<c1> → DT ID <list> | <c3>;
        {
            
            string type,name;
            name_ic= "";
            if (token_array[a, 0] == "DT")
            {
                string name1_ic=token_array[a,1];


                type = token_array[a, 1];
                a++;
                if (token_array[a, 0] == "ID")
                {
                    name1_ic += "_"+token_array[a, 1];
                    name = token_array[a, 1];
                    a++;
                    if (list(name,type,name1_ic))
                    {
                        return true;

                    }

                }
            }
            else if (c3(out name_ic))
            {
                if (token_array[a, 0] == ";")
                {
                    a = a + 1;
                    return true;
                }
            }

            return false;
        }

        public bool c2()//<c2> → <OE> | € 
        {
            string type;
            if (OE(out type))
            {
                if (type != "bool")
                {
                    sym_error = "Condition must be boolean";
                    return false;
                }
                return true;
            }
            else if (token_array[a, 0] == ";")
            {
                return true;
            }

            return false;
        }
        public bool c3(out string name_ic)//<c3> → <Asg_Op_loop> |€
        {
            if (Asg_Op_loop(out name_ic,""))
            {
                return true;
            }
            else if (token_array[a, 0] == ")" || token_array[a, 0] == ";")
            {
                return true;
            }

            return false;
        }

        public bool Asg_Op_loop(out string name_ic, string name_in_ic)//this. <Asg_Op_1> | <Asg_Op_1> | DI ID <ID_array> <Asg_Op_Comma>
        {
            name_ic = name_in_ic;
            string type;
            if (token_array[a, 0] == "this")
            {
                name_in_ic += "this";
                count = 0;
                a++;
                if (token_array[a, 0] == ".")
                {
                  
                    a++;
                    if (Asg_Op_1(out type,false,out name_ic,name_in_ic))
                    {
                        return true;
                    }
                }
            }

            else if (Asg_Op_1(out type,false,out name_ic,""))
            {
                return true;
            }

            else if (token_array[a, 0] == "DI")
            {
                string operator_ic=token_array[a,1];
               
                a++;
                if (token_array[a, 0] == "ID")
                {
                    string name = token_array[a, 1];
                    if (!re.scope_lookup(name))
                    {

                        if (!re.inherited_func_lookup(Name1, name))
                        {
                            if (sym_error == "")
                            {
                                sym_error = "undeclared variable -- '" + name + " '";
                            }
                            return false;
                        }
                    }
                     if (!CT.attribute_type.Contains("int") && !CT.attribute_type.Contains("float") && !CT.attribute_type.Contains("char"))
                {
                    sym_error = "increment/decrement invalid type -->" + CT.attribute_type;
                    return false;
                }
                    string type4 = CT.attribute_type;
                    string name_in1_ic = token_array[a, 1];
                    a++;
                    string type3;
                    
                    if (ID_array(out type3,out name_ic,name_in1_ic))
                    {
                        if (!type4.Contains(type3))
                        {
                            sym_error = name + " array dimension mismatch [" + type4.Split('[')[1];
                            return false;
                        }
                        if (type3 == "" && type4.Contains('['))
                        {
                            sym_error = name + " is an array";
                            return false;
                        }
                        if (!type4.Contains("int") && !type4.Contains("char"))
                        {
                            sym_error = "iterative part must be number";
                            return false;
                        }
                        if (type4.Contains("["))
                        {
                            type4 = type4.Split('[')[0];
                        }
                        if (operator_ic == "++")
                        {
                            name_ic = name_ic + "= 1 + " + name_ic;

                        }
                        else
                        {
                            name_ic = name_ic + "= 1 - " + name_ic;

                        }
                        name_ic = name_in_ic + name_ic;
                        
                        
                        if (Asg_Op_Comma(out name_ic,name_ic))
                        {

                            return true;
                        }
                    }
                }
            }

            return false;
        }
        public bool Asg_Op_1(out string type,bool check,out string name_ic,string name_in_ic)//<Asg_Op_1> → ID<ID><ID_array> <Asg_Op_11> 
        {
            string name="";
            type = "";
            name_ic = name_in_ic;
         
            if (token_array[a, 0] == "ID")
            {
                name = token_array[a, 1];   
                if (!re.scope_lookup(name))
                {
                    if (!re.inherited_func_lookup(Name1, name))
                    {
                        if (sym_error == "")
                        {
                            sym_error = "undeclared variable -- '" + name + " '";
                        }
                        return false;
                    }
                }

                if (count == 0 && !check)
                {
                    if (!re.lookup_class(name, "", ""))
                    {
                        sym_error = "undeclared global variable -- '" + name + " '";
                        return false;
                    }
                    count++;
                }
                else if(check)
                {
                    if (!re.scope_lookup(name))
                    {
                        if (!re.inherited_func_lookup(Name1, name))
                        {
                            if (sym_error == "")
                            {
                                sym_error = "undeclared variable -- '" + name + " '";
                            }
                            return false;
                        }
                    }
                    count++;
                    check = false;
                }
                else
                {
                    foreach (array_node aaa in CT.parent_ref)
                    {

                        if (name == aaa.name)
                        {

                            CT.attribute_type = aaa.return_type;
                            CT.attribute_cat = aaa.parameter_list;
                            CT.attribute_AM = aaa.AM;

                        }
                    }

                }
                if (CT.attribute_cat == "static")
                {
                    sym_error = "Static variable '" + name + "' cannot be accessed with an instance reference";

                    return false;
                }
                if (CT.attribute_AM == "private" && count - 1 != 0)
                {
                    sym_error = "Private variable '" + name + "' can be accessed only within class";
                    return false;
                }

                if (name_in_ic.Contains("this"))
                {
                    name_in_ic += "_" + token_array[a, 1];

                }
                else
                {
                    name_in_ic +=  token_array[a, 1];

                }

                a++;
                if (ID(name,out type,out name_ic,name_in_ic))
                {
                    name_in_ic = name_ic;
                    string type4 = CT.attribute_type;
                    string type3;
                    if (ID_array(out type3,out name_ic,name_in_ic))
                    {
                        if (!type4.Contains(type3))
                        {
                            sym_error = name + " array dimension mismatch [" + type4.Split('[')[1];
                            return false;
                        }
                        if (type3 == "" && type4.Contains('['))
                        {
                            sym_error = name + " is an array";
                            return false;
                        }
                        if (!type4.Contains("int") && !type4.Contains("char"))
                        {
                            sym_error = "iterative part must be number";
                            return false;
                        }
                        if (type4.Contains("["))
                        {
                            type4 = type4.Split('[')[0];
                        }

                        name_in_ic = name_ic;
                        if (Asg_Op_11(type4,out name_ic,name_in_ic))
                        {
                            return true;
                        }
                    }
                }


            }


            return false;
        }
        public bool Asg_Op_11(string type, out string name_ic, string name_in_ic)//<Asg_Op’’> → , <Asg_Op_loop><Asg_Op_loop1>| <Asg_Op_loop1> | DI <Asg_Op_Comma>
        {
            name_ic = "";
            if (token_array[a, 0] == ",")
            {
                a++;
                if (Asg_Op_loop(out name_ic,name_in_ic))
                {
                    if (Asg_Op_loop1(type,out name_ic,""))
                    {
                        return true;
                    }
                }
            }
           
            else if (token_array[a, 0] == "DI")
            {
                if (!type.Contains("int") && !type.Contains("char") && !type.Contains("float"))
                {
                    sym_error = "increment/decrement invalid type -->" + type;
                    return false;
                }

                string n1_ic_1 = "";
                string n1_ic = "";
                if (name_in_ic.Contains("_"))
                {
                    n1_ic_1 = cg.temp_var();
                    n1_ic = n1_ic_1 + "=" + name_in_ic;
                    if (token_array[a, 1] == "++")
                    {
                        n1_ic += "," + n1_ic_1 + "=" + n1_ic_1 + "+1";
                    }
                    else
                    {
                        n1_ic = "," + n1_ic_1 + "=" + n1_ic_1 + "-1";
                    }
                }
                else
                {
                    if (token_array[a, 1] == "++")
                    {
                        name_ic += name_in_ic + "=" + name_in_ic + "+1";
                    }
                    else
                    {
                        name_ic = name_in_ic + "=" + name_in_ic + "-1";
                    }
                    n1_ic = name_ic;
                }


             


                a++;
                if (Asg_Op_Comma(out name_ic,n1_ic))
                {
                    return true;
                }
            }




            else if (Asg_Op_loop1(type,out name_ic,name_in_ic))
            {
                return true;
            }

            return false;
        }

        public bool Asg_Op_loop1(string type_L, out string name_ic, string name_in_ic)//<Asg_Op_loop1> →  <AsgOp_Equal> <OE><Asg_Op_loop1><Asg_Op_Comma>|€
        {
            string op_ic = "";
            name_ic = name_in_ic;
            string type_R;
            if (AsgOp_Equal(out op_ic))
            {
                if (OE(out type_R))
                {
                 
                    if (concat_count_ic==0)
                    {
                        name_in_ic = name_in_ic + op_ic + oe_ic;
                        concat_count_ic++;
                    }
                    else
                    {
                        name_in_ic += op_ic + oe_ic;
                    }
                    if (Asg_Op_loop1(type_R,out name_ic,name_in_ic))
                    {
                        if (Asg_Op_Comma(out name_ic,name_ic))
                        {
                            return true;
                        }
                    }
                }
            }
            else if (token_array[a, 0] == "AsgOp" || token_array[a, 0] == "Equal" || token_array[a, 0] == ")" || token_array[a, 0] == ",")
            {
                name_ic = name_in_ic;
                return true;
            }
            return false;

        }
        Queue for_inc_ic = new Queue();
        bool enter = false;
        public bool Asg_Op_Comma(out string name_ic, string name_in_ic)//<AsgOp_Comma> →  ,<Asg_Op_loop> |€
        {
            name_ic = name_in_ic;
            if (token_array[a, 0] == ",")
            {
                for_inc_ic.Enqueue(name_in_ic);
                name_in_ic = "";
                a++;
                if (Asg_Op_loop(out name_ic,name_in_ic))
                {
                    return true;
                }
            }
            else if (token_array[a, 0] == "AsgOp" || token_array[a, 0] == "Equal" || token_array[a, 0] == ")" || token_array[a, 0] == ",")
            {
                if (!enter)
                {
                    for_inc_ic.Enqueue(name_in_ic);
                    name_in_ic = "";
                    enter = true;
                }
                return true;
            }

            return false;
        }

        public bool AsgOp_Equal(out string name_ic)// <AsgOp_Equal> → AsgOp | equal 
        {
            name_ic = "";
            if (token_array[a, 0] == "AsgOp" || token_array[a, 0] == "Equal")
            {
                name_ic = token_array[a, 1];
                a++;
                return true;

            }
            return false;
        }
        public bool _object()// <object> → ID ID = new ID(<condition>);
        {
            string arguments ;
            if (token_array[a, 0] == "ID")
            {
                a++;
                if (token_array[a, 0] == "ID")
                {
                    a++;
                    if (token_array[a, 0] == "Equal")
                    {
                        a++;
                        if (token_array[a, 0] == "new")
                        {
                            a++;
                            if (token_array[a, 0] == "ID")
                            {
                                a++;
                                if (token_array[a, 0] == "(")
                                {
                                    a++;
                                    string n_ic;
                                    if (condition(out arguments,out n_ic,""))
                                    {
                                        if (token_array[a, 0] == ")")
                                        {
                                            a++;
                                            if (token_array[a, 0] == ";")
                                            {
                                                a++;
                                                return true;
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }



            }
            return false;
        }


        public bool ID_array(out string type,out string result_ic,string name1_ic)//<ID_array> → [<OE><arr_call>] | €
        {
            result_ic = name1_ic;
            type = ""; string type1;
            if (token_array[a, 0] == "[")
            {
                name1_ic += token_array[a, 1];
                
                a++;
                if (OE(out type1))
                {
                    if (!type1.Contains("int") && !type1.Contains("char"))
                    {
                        sym_error = "index must be integer";
                        return false;
                    }
                    name1_ic += oe_ic;
                    string name_ic;
                    if (arr_call(out type,out name_ic,""))
                    {
                        name1_ic += name_ic;
                        if (token_array[a, 0] == "]")
                        {
                            type += "]";
                            name1_ic += token_array[a, 0];
                            result_ic = name1_ic;
                            a++;
                            return true;
                        }
                    }
                }
            }
            else if (token_array[a, 0] == "DT" || token_array[a, 0] == "this" || token_array[a, 0] == "ID" || token_array[a, 0] == "for" || token_array[a, 0] == "while" || token_array[a, 0] == "if" || token_array[a, 0] == "try" || token_array[a, 0] == "DI" || token_array[a, 0] == "return" || token_array[a, 0] == "break" || token_array[a, 0] == "continue" || token_array[a, 0] == "}" || token_array[a, 0] == "AsgOp" || token_array[a, 0] == "Equal" || token_array[a, 0] == ")" || token_array[a, 0] == "MDM" || token_array[a, 0] == "PM" || token_array[a, 0] == "ROP" || token_array[a, 0] == "AND" || token_array[a, 0] == "OR" || token_array[a, 0] == ";" || token_array[a, 0] == "," || token_array[a, 0] == "]")
            {
                return true;
            }

            return false;
        }
        public bool arr_call(out string type, out string name_ic, string name_in_ic)//arr_call>→,<OE>|€
        {
            name_ic = name_in_ic;
            type = "[";//[,
            string type1;
            if (token_array[a, 0] == ",")
            {
                name_ic += token_array[a, 0];
                type += ",";
                a++;
                
                if (OE(out type1))
                {
                    if (!type1.Contains("int") && !type1.Contains("char"))
                    {
                        sym_error = "index must be integer";
                        return false;
                    }
                    name_ic += oe_ic;
                    return true;
                }
            }
            else if (token_array[a, 0] == "]")
            {
                return true;
            }

            return false;
        }
        public bool return_exist = false;
        public bool return_array = false;
        
        public bool _return(string type)//<_return> → return <return’>
        {
            if (token_array[a, 0] == "return")
            {
                return_exist = true;
                a++;
                if (return1(type))
                {
                    return true;
                }
            }

            return false;
        }

        public bool return1(string Ret_type)//<return’>-><OE>; | €
        {
            string type;
            return_array = true;
            if (OE(out type))
            {
                if (!(type == Ret_type) && (Ret_type != "int" && type != "char") && (Ret_type != "float" && (type != "char" || type != "int")))
                {
                  
                    sym_error = "return type mismatch";
                    return false;
                }
                if (token_array[a, 0] == ";")
                {
                    return_array = false;
                    a++;
                    return true;
                }
            }
            else if (token_array[a, 0] == ";")
            {
               
                a++;
                return true;
            }
            
            return false;
        }

        public bool non_func_MST(string type)//<non_func_MST> → <SST_F><non_func_MST> | €
        {
            if (SST_F(type))
            {
                if (non_func_MST(type))
                {
                    return true;
                }
            }
            else if (token_array[a, 0] == "}")
            {

                return true;
            }
            return false;
        }

        bool inherit_func = false;

        public bool SST_F(string type1)//<SST_F> →DT ID <SST_F’’> | this. <SST_F1’>|ID <SST_F1_obj> | <for_loop> | <while_loop> | <if-else> | <TCF> | DI ID <ID_array> | break; | continue; | <return>
        {

            string type, name;
            string n_ic;
            if (token_array[a, 0] == "DT")
            {
                type = token_array[a, 1];
                n_ic = token_array[a, 1];
                a++;
                if (token_array[a, 0] == "ID")
                {
                    name = token_array[a, 1];
                    n_ic += "_" + token_array[a, 1];
                    a++;
                    string name_ic;
                    if (SST_F_11(name,type,out name_ic,n_ic))
                    {
                        return true;
                    }

                }
            }
               
            else if (token_array[a, 0] == "this")
            {
                n_ic = token_array[a, 0];
                a++;
                if (token_array[a, 0] == ".")
                {
                    _name = "";
                    count = 0;
                    n_ic += "_";
                    a++;
                    string name1_ic;
                    if (SST_F1_1(out type,out name1_ic,n_ic))
                    {
                        return true;
                    }

                }
            }
            else if (token_array[a, 0] == "ID")///A a   a= 0;
            {
                count = 0;
                type = "";

                overloadcount = 0;
                name = token_array[a, 1];
               
                if (token_array[a + 1, 0] == "ID")
                {
                    if (!re.lookup(name))
                    {
                        sym_error = "class doesn't exist";
                        return false;
                    }
                    _name = name;
                }


                else if (!re.scope_lookup(name))
                {
                    if (!re.inherited_func_lookup(Name1, name))
                    {
                        if (sym_error == "")
                        {
                        sym_error = "undeclared  variable -- '" + name + " '";
                        }
                        return false;
                    }
                }
                count++;

                n_ic = token_array[a, 1];
                a++;
                string name2_ic;
                if (SST_F1_obj(name,out name2_ic,n_ic))
                {
                    return true;
                }

            }
            else if (for_loop(type1))
            {
                return true;
            }
            else if (_while(type1))
            {
                return true;
            }
            else if (TCF(type1))
            {
                return true;
            }
            else if (if_else(scope,type1,""))
            {
                return true;
            }
            else if (_return(type1))
            {
                return true;
            }
            else if (token_array[a, 0] == "break")
            {
                a++;
                if (token_array[a, 0] == ";")
                {
                    a++;
                    return true;
                }
            }
            else if (token_array[a, 0] == "continue")
            {
                a++;
                if (token_array[a, 0] == ";")
                {
                    a++;
                    return true;
                }
            }
            else if (token_array[a, 0] == "DI")
            {
                string opr_ic = token_array[a, 1];
                a++;
                if (token_array[a, 0] == "ID")
                {
                    n_ic = token_array[a, 1];
                    if (!re.scope_lookup(token_array[a, 1]))
                    {
                        if (!re.inherited_func_lookup(Name1, token_array[a,1]))
                        {
                            if (sym_error == "")
                            {
                                sym_error = "undeclared  variable -- '" + token_array[a,1]+ " '";
                            }
                            return false;
                        }
                    }
                    if (!CT.attribute_type.Contains("int") && !CT.attribute_type.Contains("float") && !CT.attribute_type.Contains("char"))
                    {
                        sym_error = "increment/decrement invalid type -->" + CT.attribute_type;
                        return false;
                    }
                    a++;
                    string type3;
                    string name_ic;
                    if (ID_array(out type3,out name_ic,n_ic))
                    {

                        if (token_array[a, 0] == ";")
                        {
                            if (opr_ic=="++")
                            {
                                cg.create_label(name_ic + "= 1 + " + name_ic);
                            }
                            else
                            {
                                cg.create_label(name_ic + "= 1 - " + name_ic);
                            }
                            a++;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool SST_F_11(string name, string type, out string name_ic, string name_in_ic)//<SST_F_11>→<list>|[<OE><A’>
        {
            string type1;
            name_ic = name_in_ic;
            if (list(name,type,name_in_ic))
            {
                return true;
            }
            else if (token_array[a, 0] == "[")
            {
                type += "[";
                name_in_ic += token_array[a, 1];
                a++;
                if (OE(out type1))
                {
                    name_in_ic += oe_ic;
                    if (type1 != "int" && type1 != "char")
                    {

                        sym_error = "index must be integer";
                        return false;
                    }  
                    if (A_Array(name,type,out name_ic,name_in_ic))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool SST_F1_1(out string type, out string name_ic, string name_in_ic)//<SST_F1’>→ID<ID><SST_F1’’> 
        {
            name_ic = name_in_ic;
            string name;
            type = "";
            if (token_array[a, 0] == "ID")
            {
                name = token_array[a, 1];
                if (count == 0)
                {
                    if (!re.lookup_class(name, "", ""))
                    {
                        sym_error = "undeclared global variable -- '" + name + " '";
                        return false;
                    }
                    count++;
                }
                else
                {
                    foreach (array_node aaa in CT.parent_ref)
                    {

                        if (name == aaa.name)
                        {

                            CT.attribute_type = aaa.return_type;
                            CT.attribute_cat = aaa.parameter_list;
                            CT.attribute_AM = aaa.AM;

                        }
                    }

                }
                if (CT.attribute_cat == "static")
                {
                    if (_name != re.name_class)
                    {
                    sym_error = "Static variable '" + name + "' cannot be accessed with an instance reference";
                    return false;
                    }
                }
                if (CT.attribute_AM == "private" && count-1!=0)
                {
                    sym_error = "Private variable '" + name + "' can be accessed only within class";
                    return false;
                }


                name_in_ic += token_array[a, 1];
               
                a++;
                if (ID(name,out type,out name_ic,name_in_ic))
                {
                    if (SST_F1_11(name,out type,out name_ic,name_ic))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool SST_F1_obj(string type, out string name_ic, string name_in_ic)//<SST_F1_obj>-> ID Equal new ID(<condition>);| <ID><SST_F1’’>
        {
            name_ic = name_in_ic;
            string name = "";
            string name1 = type;
            
            if (token_array[a, 0] == "ID")//A a
            {
                inherit_func = false;
                if (!re.lookup(type))
                {

                    sym_error = "class doesn't exist";
                    return false;
                }

                CT.parent_type = "";
                CT.parent_cat = "";
                name = token_array[a, 1];
                name_in_ic += "_" + token_array[a, 1];
                a++;
                if (SST_obj_array(name,type,out name_ic,name_in_ic))
                {
                    return true;
                }
            }
               
            else if (ID(name1,out type,out name_ic,name_in_ic))
            {

                if (SST_F1_11(type,out type,out name_ic,name_ic))
                {
                    return true;
                }
            }
            return false;
        }

        public bool SST_F1_11(string name, out string type, out string name_ic, string name_in_ic)//<SST_F1’’>→[<OE><arr_call>] <ID_arr>|  <Asg_Op_MST>  | DI; |(<condition>)<ID><SST_F1_11> 
        {
            name_ic = name_in_ic;
            type = "";  //array bna ra hy A a[]---int[] h;   
            string arguments ;
         
            
           if (token_array[a, 0] == "[")
            {
                string type1 = CT.attribute_type;
                name_in_ic += token_array[a, 0];
                a++;
                string type2 = "";
                if (OE(out type2))
                {
                    if (type2 != "int" && type2 != "char")
                    {
                        
                        sym_error = "index must be integer";
                        return false;
                    }
                    string type3;
                    name_in_ic += oe_ic;
                    if (arr_call(out type3,out name_ic,name_in_ic))
                    {
                        if (token_array[a, 0] == "]")
                        {
                            name_ic += token_array[a, 0];
                            a++;
                            CT.attribute_type = type1;
                            if (CT.attribute_type.Contains('['))
                            {

                                string arr = CT.attribute_type.Split('[')[1];

                                if (type3 == "[" && arr == "]")
                                {
                                    if (ID_arr(name, out type,out name_ic,name_ic))
                                    {
                                        type = type.Split('[')[0];
                                        
                                        return true;
                                    }
                                }
                                else if (type3 != "[" && arr == ",]")
                                {
                                    if (ID_arr(name, out type,out name_ic,name_ic))
                                    {
                                        type = type.Split('[')[0];
                                      
                                        return true;
                                    }
                                }
                                else
                                {
                                    sym_error = name + " array dimension mismatch [" + CT.attribute_type.Split('[')[1];
                                    return false;
                                }
                            }
                            else
                            {
                                sym_error = name + " is not declared as array";
                                return false;
                            }
                        }   
                    }
                }
            }
            else if (token_array[a, 0] == "DI")
            {
                if (token_array[a, 1] == "++")
                {
                    cg.create_label(name_in_ic +" = "+name_in_ic+"+ 1");
                }
                else
                {
                    cg.create_label(name_in_ic + " = " + name_in_ic + "- 1");

                }
                a++;
                if (token_array[a, 0] == ";")
                {
                    if (!CT.attribute_type.Contains("int") && !CT.attribute_type.Contains("float") && !CT.attribute_type.Contains("char"))
                    {
                        sym_error = "increment/decrement invalid type -->" + CT.attribute_type;
                        return false;
                    }
             
                    a++;
                    return true;

                }
            }
            else if (Asg_Op_MST(out type,CT.attribute_type,out name_ic,name_in_ic))
            {
                return true;
            }
            else if (token_array[a, 0] == "(")
            {

                string cat = CT.attribute_cat;
                string att_type = CT.attribute_type;
                overload.Push(overloadcount);
                string name1_ic;
                a++;
                if (condition( out arguments,out name1_ic,name_in_ic))
                {
                
                    if (token_array[a, 0] == ")")
                    {
                        if (overloadcount > 1)
                        {
                            foreach (array_node aaa in CT.parent_ref)
                            {

                                if (name == aaa.name)
                                {
                                    if (aaa.parameter_list == arguments)
                                    {
                                        if (aaa.AM == "public")
                                        {
                                            att_type = aaa.return_type;
                                            cat = aaa.parameter_list;
                                            CT.attribute_AM = aaa.AM;
                                        }
                                    }
                                    else
                                    {
                                        sym_error = "private variable" + name + "can only accessed within class";
                                        return false;
                                    }

                                }
                            }
                        }
                      
                        CT.attribute_cat = cat;
                        CT.attribute_type = att_type;
                      
                        if (overload.Count > 0)
                        {
                            overload.Pop();
                            if (overload.Count > 0)
                            {
                            overloadcount = Convert.ToInt16(overload.Peek());

                            }
                        }
                    if (CT.attribute_cat != arguments)
                    {
                        sym_error = "invalid number or invalid type arguments of function";
                        return false;
                    }
                    if (CT.attribute_type.Contains("int") || CT.attribute_type.Contains("bool") || CT.attribute_type.Contains("float") || CT.attribute_type.Contains("string") || CT.attribute_type.Contains("char") )
                    {
                        sym_error = "return type must be VOID of function";
                        return false;
                    }
                      
                        a++;
                        if (ID(name,out type,out name_ic,name1_ic))
                        {
                            if (SST_F1_11(name,out type,out name_ic,name_ic))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        
        public bool ID_arr(string name,out string type,out string name_ic,string name_in_ic)//<ID_arr> → <Asg_Op_MST> | DI ;
        {
            type="";
            name_ic = name_in_ic;

            if (CT.attribute_type.Contains("["))
            {
                if (Asg_Op_MST(out type, CT.attribute_type.Split('[')[0],out name_ic,name_in_ic))
                {
                    return true;
                }
                else if (token_array[a, 0] == "DI")
                {
                    if (!CT.attribute_type.Contains("int") && !CT.attribute_type.Contains("float") && !CT.attribute_type.Contains("char"))
                    {
                        sym_error = "increment/decrement invalid type -->" + type;
                        return false;
                    }
                    type = CT.attribute_type;
                    string n_ic = cg.temp_var();
                    name_in_ic = n_ic + " = " + name_in_ic;
                    cg.create_label(name_in_ic);
                    if (token_array[a, 1] == "++")
                    {
                        cg.create_label(n_ic + " = " + n_ic + " + 1");
                    }
                    else
                    {
                        cg.create_label(n_ic + " = " + n_ic + " - 1");

                    }
                    name_ic = name_in_ic;
                    a++;
                    if (token_array[a, 0] == ";")
                    {
                        a++;
                        return true;
                    }
                }
            }

                else
                {
                    if (Asg_Op_MST(out type, CT.attribute_type,out name_ic,name_in_ic))
                    {
                        return true;
                    }
               else if (token_array[a, 0] == "DI")
                {
                if (!CT.attribute_type.Contains("int") && !CT.attribute_type.Contains("float") && !CT.attribute_type.Contains("char"))
                {
                    sym_error = "increment/decrement invalid type -->" + type;
                    return false;
                }
                type = CT.attribute_type;
                string n_ic = cg.temp_var();
                name_in_ic = n_ic + " = " + name_in_ic;
                cg.create_label(name_in_ic);
                if (token_array[a, 1] == "++")
                {
                    cg.create_label(n_ic + " = " + n_ic + " + 1");
                }
                else
                {
                    cg.create_label(n_ic + " = " + n_ic + " - 1");

                }
                name_ic = name_in_ic;
                   a++;
                if (token_array[a, 0] == ";")
                {
                    a++;
                    return true;
                }
            }

            

                }
            
         

            return false;
        }

        public bool Asg_Op_MST(out string type, string type_in, out string name_ic, string name_in_ic)//<Asg_Op_MST> →  <AsgOp_Equal> <OE> <Asg_Op_MST>|;
        {
            type = "";

            if (AsgOp_Equal(out name_ic))
            {
                name_in_ic += name_ic;
                if (OE(out type))//a=g+k=b+c
                {
                    if (type.Contains("["))
                    {
                        type = type.Split('[')[0];
                    }
                    name_in_ic += oe_ic;
                    cg.create_label(name_in_ic);
                    if (type == type_in || (type_in == "float" && (type == "int" || type == "char" ) )|| (type_in == "int" && type == "char"))   //check
                    {
                        if (Asg_Op_MST(out type, type,out name_ic,name_in_ic))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        sym_error = "type mismatch ";
                        return false;
                    }
             
             
                }
            }
            else if (token_array[a, 0] == ";")
            {
              //  cg.create_label("call_"+name_in_ic);  extra ara h 
                a++;
                return true;

            }

            return false;
        }

        public bool non_class_MST(string tem_ic)//<non_class_MST> → <SST_C><non_class_MST> | null
        {

            if (token_array[a, 0] == "Main")
            {
                tem_ic += "_Main";
                main_exist = true;
                main_count++;
                if (main_count == 1)
                {
                    cg.create_label(tem_ic + " proc");
                    a++;
                    if (token_array[a, 0] == "(")
                    {
                        a++;
                        if (token_array[a, 0] == ")")
                        {
                            re.insert_class("Main", "void", "void", "public");/**************************************************************main function entry************************************************************/
                            a++;
                            if (token_array[a, 0] == "{")
                            {
                                
                                scope_stack.Push(++scope);
                                a++;
                                if (non_func_MST("void"))
                                {
                                    cg.create_label("end proc");
                                    tem_ic = tem_ic.Substring(0, tem_ic.IndexOf(tem_ic.Split('_')[tem_ic.Split('_').Length - 1]) - 1);
                                    if (token_array[a, 0] == "}")
                                    {
                                        scope_stack.Pop();
                                        
                                        
                                        
                                        a++;
                                        if (no_main_func(tem_ic))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (SST_C(tem_ic))
            {
                if (non_class_MST(tem_ic))
                {

                    return true;
                }
            }
            else if (token_array[a, 0] == "}")
            {
                return true;
            }

            return false;
        }

        public bool no_main_func(string tem_ic)
        {
            if (SST_C(tem_ic))
            {
                if (no_main_func(tem_ic))
                {
                    return true;
                }
            }

            else if (token_array[a, 0] == "}")
            {
                return true;
            }



            return false;
        }

        public bool SST_C(string name_ic)////<SST_C>→AM <SST_C_1>|<_object>
        {
            string type, name;
            string AM;
            if (token_array[a, 0] == "AM")
            {
                AM = token_array[a, 1];

                name_ic += "_"+token_array[a, 1];
                a++;
                if (SST_C_1(AM,name_ic))
                {
                    return true;

                }
            }

            else if (token_array[a, 0] == "ID")
            {
                type = token_array[a, 1];
                if (!re.lookup(type))
                {

                    sym_error = "class doesn't exist";
                    return false;
                }
                //if (CT.parent_type == "interface")
                //{
                //    sym_error = "interface ka object nahi bante";
                //    return false;
                //}
                //CT.parent_type = "";
                //CT.parent_cat = "";
                a++;
                if (token_array[a, 0] == "ID")
                {
                    name = token_array[a, 1];
                    a++;
                    if (obj_array(name,type))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public bool SST_C_obj(string name, string type)
        {
            string arguments;
            if (token_array[a, 0] == ";")
            {
                if (!re.insert_class_attribute(name, type, "---", "public"))
                {
                    sym_error = "redeclaration of attribute";
                    return false;

                }
                a++;
                return true;
            }
            else if (token_array[a, 0] == "Equal")
            {
                a++;
                if (token_array[a, 0] == "new")
                {
                    a++;
                    if (token_array[a, 0] == "ID")
                    {
                        string name1 = token_array[a, 1];
                        a++;
                        if (token_array[a, 0] == "(")
                        {
                            function_se_aya = false;
                            a++;
                            string name_ic;
                            if (condition(out arguments,out name_ic,""))
                            {
                                function_se_aya = false;
                                if (CT.parent_type == "interface")
                                {

                                }
                                if (!re.child_parent_lookup(type,name1,arguments))
                                {
                                    sym_error = "invalid Arguments or invalid Object Instance";
                                    return false;
                                }
                                if (!re.insert_class_attribute(name, type, "---", "public"))
                                {
                                    sym_error = "redeclaration of attribute";
                                    return false;
                                }

                                if (token_array[a, 0] == ")")
                                {
                                    a++;
                                    if (token_array[a, 0] == ";")
                                    {
                                        a++;
                                        return true;

                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
        public bool obj_array(string name,string type)
        {
      
            if (SST_C_obj(name,type))
            {
                return true;
            }
            else if (token_array[a, 0] == "[")//A[
            {
                string type1 = type;
                type += "[";
                a++;
                string type3 = "";
                if (OE(out type3))
                {
                    if (type3 != "int" && type3 != "char")
                    {
                        sym_error = "index must be integer";
                        return false;
                    }
                    if (A_Array_obj(name,type,type1))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool A_Array_obj(string name,string type,string type1)
        {
            string type3 = "";
            if (token_array[a, 0] == ",")
            {
                
                a++;
                if (OE(out type3))
                {
                    if (type3 != "int" && type3 != "char")
                    {
                        sym_error = "index must be integer";
                        return false;
                    }
                    if (token_array[a, 0] == "]")
                    {
                        type += ",]";
                        a++;
                        if (token_array[a, 0] == "Equal")
                        {
                            a++;
                            if (token_array[a, 0] == "{")
                            {
                                a++;
                                if (_2D_cond_obj(type1))
                                {
                                    if (!re.insert_class_attribute(name, type, "---", "public"))
                                    {
                                        sym_error = "redeclaration of attribute";
                                        return false;
                                    }
                                    if (token_array[a, 0] == "}")
                                    {
                                        a++;
                                        if (token_array[a, 0] == ";")
                                        {
                                            a++;
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (token_array[a, 0] == "]")
            {
                type += "]";
                a++;
                if (token_array[a, 0] == "Equal")
                {
                    a++;
                    if (token_array[a, 0] == "{")
                    {
                        a++;
                        if (condition_obj(type1))
                        {
                            if (!re.insert_class_attribute(name, type, "---", "public"))
                            {
                                sym_error = "redeclaration of attribute";
                                return false;
                            }

                            if (token_array[a, 0] == "}")
                            {
                                a++;
                                if (token_array[a, 0] == ";")
                                {
                                    a++;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        public bool _2D_cond_obj(string type)
        {
            
            if (token_array[a, 0] == "{")
            {
                a++;
                if (condition_obj(type))
                {


                    if (token_array[a, 0] == "}")
                    {
                        a++;
                        if (_2D_cond_obj_1(type))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        public bool _2D_cond_obj_1(string type)
        {
            if (token_array[a, 0] == ",")
            {
                a++;
                if (_2D_cond_obj(type))
                {
                    return true;
                }
            }
            else if (token_array[a, 0] == "}")
            {
                return true;
            }
            return false;
        }
        public bool condition_obj( string type)
        {
            string arguments ;
            if (token_array[a, 0] == "new")
            {
                a++;
                if (token_array[a, 0] == "ID")
                {
                    string name1 = token_array[a, 1];
                    a++;
                    if (token_array[a, 0] == "(")
                    {
                        function_se_aya = false;
                        a++;
                        string name_ic;
                        if (condition(out arguments,out name_ic,""))
                        {
                            function_se_aya = false;
                            if (!re.child_parent_lookup(type, name1, arguments))
                            {
                                sym_error = "invalid Arguments or invalid Object Instance";
                                return false;
                            }
                            
                            if (token_array[a, 0] == ")")
                            {
                                a++;
                                if (condition_obj_1(type))
                                {
                                    return true;
                                }

                            }
                        }
                    }
                }
            }
            else if (token_array[a, 0] == "}")
            {
                return true;
            }
            return false;
        }
        public bool condition_obj_1(string type)
        {
            
            if (token_array[a, 0] == ",")
            {
                a++;
                if (condition_obj(type))
                {
                    return true;
                }
            }
            else if (token_array[a, 0] == "}")
            {
                return true;
            }
            return false;
        }

        public bool SST_obj(string name,string type,out string name_ic,string name_in_ic)
        {
            string arguments ;
            name_ic = name_in_ic;
            if (token_array[a, 0] == ";")
            {
                if (!re.insert_func(name, type,Convert.ToInt16( scope_stack.Peek())))
                {
                    sym_error = "redeclaration of local variable";
                    return false;

                }
                cg.create_label(name_in_ic);
                a++;
                return true;
            }
            else if (token_array[a, 0] == "Equal")
            {
                name_in_ic += token_array[a, 1];
                a++;
                if (token_array[a, 0] == "new")
                {
                    name_in_ic += token_array[a, 0];

                    a++;
                    if (token_array[a, 0] == "ID")
                    {
                        string name1 = token_array[a, 1];
                        name_in_ic +="_"+ token_array[a, 1];

                        a++;
                        if (token_array[a, 0] == "(")
                        {
                            function_se_aya = false;
                            a++;

                            if (condition(out arguments,out name_ic,""))
                            {
                                function_se_aya = false;
                                if (!re.child_parent_lookup(type, name1, arguments))
                                {
                                    sym_error = "invalid Arguments or invalid Object Instance";
                                    return false;
                                }
                                //if (!re.insert_class_attribute(name, type, "---", "public"))
                                //{
                                //    sym_error = "redeclaration of attribute";
                                //    return false;
                                //}
                                if (!re.insert_func(name, type, Convert.ToInt16(scope_stack.Peek())))
                                {
                                    sym_error = "redeclaration of local variable";
                                    return false;

                                }
                                name_in_ic += name_ic;
                                if (token_array[a, 0] == ")")
                                {
                                    a++;
                                    if (token_array[a, 0] == ";")
                                    {
                                        cg.create_label(name_in_ic);
                                        a++;
                                        return true;

                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
        public bool SST_obj_array(string name, string type, out string name_ic, string name_in_ic)
        {
            name_ic = name_in_ic;
            if (SST_obj(name ,type,out name_ic,name_in_ic))
            {
                return true;
            }
            else if (token_array[a, 0] == "[")
            {
                name_in_ic += token_array[a, 0];
                string type1 = type;

                type += "[";
                a++;
                string type3 = "";
                if (OE(out type3))
                {
                    if (type3 != "int" && type3 != "char")
                    {
                        sym_error = "index must be integer";
                        return false;
                    }
                    name_in_ic += oe_ic;
                    if (SST_F_Array_obj(name,type,type1))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool SST_F_Array_obj(string name, string type, string type1)
        {
            string type3="";
            if (token_array[a, 0] == ",")
            {
                a++;
                if (OE(out type3))
                {
                    if (type3 != "int" && type3 != "char")
                    {
                        sym_error = "index must be integer";
                        return false;
                    }
                    if (token_array[a, 0] == "]")
                    {
                        type += ",]";
                        a++;
                        if (token_array[a, 0] == "Equal")
                        {
                            a++;
                            if (token_array[a, 0] == "{")
                            {
                                a++;
                                if (SST_2D_cond_obj(type1))
                                {

                                    if (!re.insert_func(name, type, Convert.ToInt16(scope_stack.Peek())))
                                    {
                                        sym_error = "redeclaration of local variable---> "+name;
                                        return false;
                                    }
                                    if (token_array[a, 0] == "}")
                                    {
                                        a++;
                                        if (token_array[a, 0] == ";")
                                        {
                                            a++;
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (token_array[a, 0] == "]")
            {
                type += "]";
                a++;
                if (token_array[a, 0] == "Equal")
                {
                    a++;
                    if (token_array[a, 0] == "{")
                    {
                        a++;
                        if (SST_condition_obj(type1))
                        {
                            if (!re.insert_func(name, type, Convert.ToInt16(scope_stack.Peek())))
                            {
                                sym_error = "redeclaration of local variable---> " + name;
                                return false;
                            }
                            if (token_array[a, 0] == "}")
                            {
                                a++;
                                if (token_array[a, 0] == ";")
                                {
                                    a++;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        public bool SST_2D_cond_obj(string type)
        {
            if (token_array[a, 0] == "{")
            {
                a++;
                if (SST_condition_obj(type))
                {
                    if (token_array[a, 0] == "}")
                    {
                        a++;
                        if (SST_2D_cond_obj_1(type))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        public bool SST_2D_cond_obj_1(string type)
        {
           
            if (token_array[a, 0] == ",")
            {
                a++;
                if (SST_2D_cond_obj(type))
                {
                    return true;
                }
              

            }
            else if (token_array[a, 0] == "}")
            {
                return true;
            }
            return false;
        }
        public bool SST_condition_obj(string type)
        {
            string type1;
            string arguments ;
        
            if (token_array[a, 0] == "new")
            {
                a++;
                if (token_array[a, 0] == "ID")
                {
                    string name1 = token_array[a, 1];
                    a++;
                    if (token_array[a, 0] == "(")
                    {
                        function_se_aya = false;
                        a++;
                        string name_ic;
                        if (condition(out arguments,out name_ic,""))
                        {
                            function_se_aya = false;
                            if (!re.child_parent_lookup(type, name1, arguments))
                            {
                                sym_error = "invalid Arguments or invalid Object Instance";
                                return false;
                            }
                            if (token_array[a, 0] == ")")
                            {
                                a++;
                                if (SST_condition_obj_1(type))
                                {
                                    return true;
                                }

                            }
                        }
                    }
                }
            }
               
            else if (OE(out type1))
            {
                if (type1.Contains("int") || type1.Contains("float") || type1.Contains("bool") || type1.Contains("string") || type1.Contains("char"))
                {
                    sym_error = "Object array content must contain reference variable";
                    return false;
                }
                if (!re.child_parent_lookup(type, type1, "array object"))
                {
                    sym_error = "invalid Object Instance";
                    return false;
                }
                if (SST_condition_obj_1(type))
                {
                    return true;
                }
            }
            else if (token_array[a, 0] == "}")
            {
                return true;
            }
            return false;
        }

        public bool SST_condition_obj_1(string type)
        {
            if (token_array[a, 0] == ",")
            {
                a++;
                if (SST_condition_obj(type))
                {
                    return true;
                }
            }
            else if (token_array[a, 0] == "}")
            {
                return true;
            }
            return false;
        }

        Stack temp_Stack = new Stack();

        public bool SST_C_1(string AM,string name_ic)
        {
            string name, category = "---", type,type1="";
            if (token_array[a, 0] == "static")
            {
                category = token_array[a, 1];

                a++;
                if (token_array[a, 0] == "DT")
                {
                    type = token_array[a, 1];
                    a++;
                    if (token_array[a, 0] == "ID")
                    {
                        name = token_array[a, 1];
                        a++;
                        if (list_attributes(name,AM,type,category))
                        {
                            return true;
                        }

                    }

                }
            }
            else if (token_array[a, 0] == "DT")
            {
                type = token_array[a, 1];
                name_ic += "_" + token_array[a, 1];
                a++;
                if (SST_C_11(AM, type,category,name_ic))
                {
                    return true;
                }
            }

            else if (token_array[a, 0] == "void")
            {
                type = "void";
                a++;
                function_se_aya = true;
                if (token_array[a, 0] == "ID")
                {
                    name = token_array[a, 1];
                    name_ic += "_" + token_array[a, 1];
                    a++;
                    
                    scope_stack.Push(++scope);
                    

                    if (token_array[a, 0] == "(")
                    {
                        a++;
                        
                        if (parameter_S(out type1,type1,out name_ic,name_ic))
                        {
                            

                            if (token_array[a, 0] == ")")
                            {
                                function_se_aya = false;
                                name_ic += " proc";
                                cg.create_label(name_ic);
                                a++;
                                if (!re.insert_class(name, type, type1, AM))
                                {
                                    sym_error = "redelaration of function...";
                                    return false;
                                }
                                if (token_array[a, 0] == "{")
                                {
                                    a++;
                                    if (non_func_MST(type))
                                    {
                                        if (token_array[a, 0] == "}")
                                        {
                                            scope_stack.Pop();
                                            cg.create_label("end proc");
                                           
                                            a++;
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            else if (token_array[a, 0] == "ID")
            {
                name = token_array[a, 1];
                name_ic += "_" + token_array[a, 1];
                a++;
                
                if (constructor_func(name ,AM,name_ic))
                {
                    return true;
                }
            }
            return false;
        }
        public bool constructor_func(string name, string AM,string name_ic) //<constructor_func> ->  (<parameter>) <_base> {<MST_constructor>} | <SST_C_11>
        {

            string type1= "";
            //create scope
            if (!re.lookup(name))
            {
                sym_error = "class doesnot exist";
                return false;
            }
            if (token_array[a, 0] == "(")
            {
                string name1_ic = "";
                for (int i = 0; i < name_ic.Split('_').Length-1; i++)
                {
                    name1_ic = name1_ic + name_ic.Split('_')[i]+"_";
                }
                name_ic = name1_ic;
                name_ic += "Constructor";
            scope_stack.Push(++scope);


            if (!(re.name_class == name))
            {
                sym_error = "constructor name invalid";
                return false;
            }
                //public A(inta) : base(int a, ---->ref->parent--->class--->cons-->class
                a++;
                string tem_ic;
                if (parameter_S(out type1,type1,out tem_ic,name_ic))
                {
                    name_ic += tem_ic;
                    cg.create_label(tem_ic + " proc");

                    if (token_array[a, 0] == ")")
                    {
                        a++;
                        if (_base(name))

                        {




                        if (!re.insert_class(name, "void_ctor", type1, AM)) 
                        {
                            sym_error = "redeclartion of constructor";
                            return false;
                        }
                            if (token_array[a, 0] == "{")
                            {
                                a++;
                                if (MST_constructor("void_ctor"))
                                {
                                    cg.create_label("end proc");
                                    if (token_array[a, 0] == "}")
                                    {
                                        //destroy scope
                                        scope_stack.Pop();
                                        a++;
                                        return true;
                                    }

                                }

                            }
                        }
                    }
                }
            }
            else if (SST_C_11(AM,name,"---",name_ic))
            {
                return true;
            }
            return false;
        }

        public bool SST_C_11(string AM, string type, string cat,string tem_ic)//<SST_C’’>->  ID<SST_C’’’>|<C’ > ID (<parameter>){<non_func_MST>}
        {
            string name,type1="";
            if (token_array[a, 0] == "ID")
            {
                tem_ic += "_" + token_array[a, 1]; 
                name = token_array[a, 1];
                a++;
                if (SST_C_111(name, AM, type, cat,tem_ic))
                {
                    return true;
                }

            }
            else if (C_ret_S(out type,type))  //return type
            {
                tem_ic += "[" + type.Split('[')[1];
                    
                if (token_array[a, 0] == "ID")
                {
                    name = token_array[a, 1];
                    a++;

                    if (token_array[a, 0] == "(")
                    {
                        function_se_aya = true;
                    scope_stack.Push(++scope);
                        a++;
                        if (parameter_S(out type1,type1,out tem_ic,tem_ic))
                        {
                            if (token_array[a, 0] == ")")
                            {
                                cg.create_label(tem_ic + " proc");
                                function_se_aya = false;

                                if (!re.insert_class(name, type, type1, AM))
                                {
                                    sym_error = "redeclartion of function";
                                    return false;
                                }

                                a++;
                                if (token_array[a, 0] == "{")
                                {
                                    a++;
                                    if (non_func_MST(type))
                                    {
                                        if (!return_exist)
                                        {
                                            if (sym_error == "")
                                            { 
                                            sym_error = "return keyword must exist";
                                            }
                                            return false;
                                        }
                                        return_exist = false;
                                        cg.create_label("end proc");
                                        if (token_array[a, 0] == "}")
                                        {
                                            scope_stack.Pop();
                                            a++;
                                            return true;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        public bool SST_C_111(string name, string AM, string type, string cat,string tem_ic)//<SST_C’’’>-><SST_F’’>|(<parameter>){<non_func_MST>}
        {
            string type1 = "";
            if (SST_C_1111(name, AM, type, cat))
            {
                return true;
            }

            else if (token_array[a, 0] == "(")
            {
                //create scope
                function_se_aya = true;
                scope_stack.Push(++scope);  
                a++;
                string temp_ic;
                if (parameter_S(out type1,type1,out temp_ic,tem_ic))
                {
                    if (token_array[a, 0] == ")")
                    {
                        function_se_aya = false;

                        if (type1 == "")
                        {
                            type1 = "void";
                        }
                        if (!re.insert_class(name, type, type1, AM))
                        {
                            sym_error = "redeclartion of function";
                            return false;
                        }
                        temp_ic += " proc";
                        cg.create_label(temp_ic);
                        a++;
                        if (token_array[a, 0] == "{")
                        {
                            a++;
                            if (non_func_MST(type))
                            {
                                if (!return_exist)
                                {
                                    if (sym_error == "")
                                    {
                                    sym_error = "return keyword must exist";
                                    }
                                    return false;
                                }
                                return_exist = false;
                                if (token_array[a, 0] == "}")
                                {
                                    //destroy scope
                                    scope_stack.Pop();
                                    cg.create_label("end proc");
                                    a++;
                                    return true;
                                }
                            }

                        }
                    }
                }
            }

            return false;
        }

        public bool MST_constructor(string type1)//<MST_constructor> → <MST_constructor’><MST_constructor>|null
        {
            if (MST_constructor_1(type1))
            {
                if (MST_constructor(type1))
                {
                    return true;
                }
            }
            else if (token_array[a, 0] == "DT" || token_array[a, 0] == "this" || token_array[a, 0] == "ID" || token_array[a, 0] == "for" || token_array[a, 0] == "while" || token_array[a, 0] == "if" || token_array[a, 0] == "try" || token_array[a, 0] == "DI" || token_array[a, 0] == "return" || token_array[a, 0] == "}")
            {
                return true;
            }

            return false;
        }
        public bool MST_constructor_1(string type1)//<MST_constructor’> →DT ID <SST_F’’> | this. <SST_F1’>|ID <SST_F1_obj> | <for_loop> | <while_loop> | <if-else> | <TCF> | DI ID <ID_array>|return;
        {
            string type, name;
            string n_ic;
            if (token_array[a, 0] == "DT")
            {
                type = token_array[a, 1];
                n_ic = token_array[a, 1];
                a++;

                if (token_array[a, 0] == "ID")
                {
                    name = token_array[a, 1];
                    n_ic += "_" + token_array[a, 1];
                    a++;
                    string name_ic;
                    if (SST_F_11(name,type,out name_ic,n_ic))
                    {
                        return true;
                    }

                }
            }
            else if (token_array[a, 0] == "this")
            {
               
                a++;
                if (token_array[a, 0] == ".")
                {
                    _name = "";
                    count = 0;
                    a++;
                    string name_ic;
                    if (SST_F1_1(out type,out name_ic,"this_"))
                    {
                        return true;
                    }

                }
            }
            else if (token_array[a, 0] == "ID")
            {
                count = 0;
                type = "";
                name = token_array[a, 1];

                if (!re.scope_lookup(name))
                {
                    if (!re.inherited_func_lookup(Name1, name))
                    {
                        if (sym_error == "")
                        {
                            sym_error = "undeclared variable -- '" + name + " '";
                        }
                        return false;
                    }

                }
                count++;
                n_ic = token_array[a, 1];
                a++;
                string name2_ic;
                if (SST_F1_obj(name,out name2_ic,n_ic))
                {
                    return true;
                }

            }
            else if (for_loop(type1))
            {
                return true;
            }
            else if (_while(type1))
            {
                return true;
            }
            else if (TCF(type1))
            {
                return true;
            }
            else if (if_else(scope,type1,""))
            {
                return true;
            }

            else if (token_array[a, 0] == "return")
            {
                a++;
                if (token_array[a, 0] == ";")
                {
                    a++;
                    return true;
                }
            }

            else if (token_array[a, 0] == "DI")
            {
                string oper_ic = token_array[a, 1];
                a++;
                if (token_array[a, 0] == "ID")
                {
                    name = token_array[a, 1];
                    if (!re.scope_lookup(token_array[a, 1]))
                    {
                        if (!re.inherited_func_lookup(Name1, token_array[a, 1]))
                        {
                            if (sym_error == "")
                            {
                                sym_error = "undeclared  variable -- '" + token_array[a, 1] + " '";
                            }
                            return false;
                        }
                    }
                    if (!CT.attribute_type.Contains("int") && !CT.attribute_type.Contains("float") && !CT.attribute_type.Contains("char"))
                    {
                        sym_error = "increment/decrement invalid type -->" + CT.attribute_type;
                        return false;
                    }
                    n_ic = token_array[a, 1];
                    a++;
                    string type3;
                    string name_ic;
                    if (ID_array(out type3,out name_ic,n_ic))
                    {
                        if (token_array[a, 0] == ";")
                        {
                            if (oper_ic=="++")
                            {
                                cg.create_label(name_ic + " = 1 + " + name_ic);
                            }
                            else
                            {
                                cg.create_label(name_ic + " = 1 - " + name_ic);

                            }
                            a++;
                        return true;
                        }

                    }
                }
            }

            return false;
        }


        public bool list_attributes(string name, string AM, string type, string cat)//<list_attributes> → <list_attributes1>| = <L_A’>
        {
         
            if (list_attributes1(name, AM, type, cat))
            {

                return true;

            }
            else if (token_array[a, 0] == "Equal")
            {
                a++;
                
                if (L_A_1(name,AM,cat, type))
                {
                   
                return   true;
                    } 
                    //till here
                }
                //int a=0,
            

            return false;
        }

        public bool list_attributes1(string name, string AM, string type, string cat)//<list_attributes1> → ; | ,ID<list_attributes>
        {
            if (token_array[a, 0] == ";")
            {
                if (!re.insert_class_attribute(name, type, cat, AM))
                {
                    sym_error = "redecalartion of attribute";
                    return false;
                }
                a++;
                return true;
            }


            else if (token_array[a, 0] == ",")
            {
                if (!re.insert_class_attribute(name, type, cat,AM))
                {
                    sym_error = "redecalartion of attribute";
                    return false;
                }
                a++;
                if (token_array[a, 0] == "ID")
                {
                    name = token_array[a, 1];
                    a++;
                    if (list_attributes(name,AM,type,cat))
                    {
                        return true;
                    }

                }

            }
            return false;

        }
   
        public bool L_A_1(string name,string AM,string cat,string L_type) //<L_A’>→<OE_class_MST> <list_attributes>
        {
            string type;
            if (OE_class_MST(out type))
            {
                if (type == L_type || (L_type == "Float_Constant" && type == "Integer_Constant"))   //check
                {
                    if (list_attributes1(name, AM, L_type, cat))
                    {
                        return true;
                    }
                }
                else
                {        
                    sym_error = "type mismatch ";
                    return false;
                }
             
            }
            return false;
        }

        public bool OE(out string type)//<OE>→<A_OE><OE1>
        {
            string name_ic;
            string type1;
            type = "";
            if (A_OE(out type1,out name_ic)) //<A_OE>
            {


                string name1_ic;
                if (OE1(type1, out type,out name1_ic,name_ic))//<OE1>
                {
                
                    return true;
                }
            }
            return false;
        }
        public bool OE1(string type1, out string type,out string result_ic,string name1_ic)//<OE1>→OR<A_OE><OE1>|€
        {
            
            string operators;
            string class_part;
            string type2;
            string type3;
            type = "";
            result_ic = name1_ic;
            string e_ic = name1_ic;
            try
            {
            oe_ic = name1_ic.Split('=')[0];

            }
            catch (Exception e)
            {
                
                
            }
            if (token_array[a, 0] == "OR")
            {

                operators = token_array[a, 1];
                class_part = token_array[a, 0];
                string tem_ic = token_array[a, 0];
                a = a + 1;
                string name_ic ;
                if (A_OE(out  type2,out name_ic)) //<A_OE>
                {

                    type3 = re.type_type(type1, type2, operators, class_part);
                    if (type3 == "type mismatch")
                    {
                        sym_error = type3;
                        return false;
                    }
                    string T_ic=cg.temp_var();
                    cg.create_label(T_ic + " = " + name1_ic+tem_ic+name_ic);
                    result_ic = T_ic;
                    if (OE1(type3, out type,out result_ic,T_ic))
                    {
                        return true;
                    }
                }
            }
            else if (token_array[a, 0] == "}" || token_array[a, 0] == "AsgOp"  || token_array[a, 0] == "Equal" || token_array[a, 0] == ")" || token_array[a, 0] == ";" || token_array[a, 0] == "," || token_array[a, 0] == "]")
            {
                type = type1;
                return true;
            }
            return false;

        }

        public bool A_OE(out string type, out string name_ic1) //<A_OE>→<R_OE><A_OE1>
        {
            type = "";
            string type1;
            name_ic1 = "";
            string name_ic = "";
            if (R_OE(out type1,out name_ic))//<R_OE>
            {
                if (A_OE1(type1, out type,out name_ic1, name_ic))//<A_OE1>
                {
         
                    return true;
                }

            }
            return false;

        }
        public bool A_OE1(string type1, out string type,out string result_ic, string name1_ic)//<A_OE1>→AND <R_OE><A_OE1>|€
        {
            type = "";
            string operators;
            string class_part;
            string type2;
            string type3;
             result_ic = name1_ic;
            if (token_array[a, 0] == "AND")
            {
                operators = token_array[a, 1];
                class_part = token_array[a, 0];
                 string tem_ic = token_array[a, 1];
                a = a + 1;
                string name_ic;
                if (R_OE(out type2,out name_ic)) //<R_OE>
                {
                    type3 = re.type_type(type1, type2, operators, class_part);
                    if (type3 == "type mismatch")
                    {
                        sym_error = type3;
                        return false;
                    }
                    string T_ic = cg.temp_var();///CREATE TEMP
                    cg.create_label(T_ic + "=" + name1_ic + tem_ic + name_ic);
                    result_ic = T_ic;
                    if (A_OE1(type3, out type,out result_ic, T_ic))
                    {
                        return true;
                    }
                }
            }
            else if (token_array[a, 0] == "OR"  || token_array[a, 0] == "}" || token_array[a, 0] == "AsgOp" || token_array[a, 0] == "Equal" || token_array[a, 0] == ")" || token_array[a, 0] == ";" || token_array[a, 0] == "," || token_array[a, 0] == "]")
            {
                type = type1;
                return true;
            }

            return false;

        }
        public bool R_OE(out string type,out string name1_ic) //<R_OE>→<E_OE><R_OE1>
        {
            type = "";
            string type1;
            name1_ic = "";
            string name_ic = "";
            if (E_OE(out type1,out name_ic))//<E_OE>
            {
                if (R_OE1(type1, out type,out name1_ic, name_ic))//<R_OE1>
                {
                    return true;
                }

            }
            return false;

        }
        public bool R_OE1(string type1, out string type,out string result_ic, string name1_ic)//<R_OE1>→ROP<E_OE><R_OE1>|€
        {
            type = "";
            string operators;
            string class_part;
            string type2;
            string type3;
            result_ic = name1_ic;
            if (token_array[a, 0] == "ROP")
            {
                operators = token_array[a, 1];
                class_part = token_array[a, 0];
                string tem_ic = token_array[a, 1];
                a = a + 1;
                string name_ic;
                if (E_OE(out  type2,out name_ic)) //<E_OE>
                {
                    type3 = re.type_type(type1, type2, operators, class_part);
                    if (type3 == "type mismatch")
                    {
                        sym_error = type3;
                        return false;
                    }
                    string T_ic = cg.temp_var();///CREATE TEMP
                    cg.create_label(T_ic + "=" + name1_ic + tem_ic + name_ic);
                    result_ic = T_ic;
                    if (R_OE1(type3, out type,out result_ic, T_ic))
                    {
                        return true;
                    }
                }
            }
            else if (token_array[a, 0] == "AND"|| token_array[a, 0] == "OR" || token_array[a, 0] == "}" || token_array[a, 0] == "AsgOp" || token_array[a, 0] == "Equal" || token_array[a, 0] == ")" || token_array[a, 0] == ";" || token_array[a, 0] == "," || token_array[a, 0] == "]")
            {
                type = type1;
                return true;
            }

            return false;

        }

        public bool E_OE(out string type,out string name1_ic) //<E_OE>→<T_OE><E_OE1>
        {
            type = "";
            string type1;
             name1_ic = "";
            string name_ic = "";
            if (T_OE(out type1,out name_ic))//<T_OE>
            {
               
                if (E_OE1(type1, out type,out name1_ic, name_ic))//<E_OE1>
                {
                    return true;
                }

            }
            return false;

        }
        public bool E_OE1(string type1, out string type,out string result_ic, string name1_ic)//<E_OE’>→PM<T_OE><E_OE1>|€
        {
            type = "";
            string operators;
            string class_part;
            string type2;
            string type3;
            result_ic = name1_ic;
            if (token_array[a, 0] == "PM")
            {
                operators = token_array[a, 1];
                class_part = token_array[a, 0];
                string tem_ic = token_array[a, 1];
                a = a + 1;
                 string name_ic;
                if (T_OE(out  type2,out name_ic)) //<T_OE>
                {


                    type3 = re.type_type(type1, type2, operators, class_part);
                    if (type3 == "type mismatch")
                    {
                        sym_error = type3;
                        return false;
                    }
                    string T_ic = cg.temp_var();
                    cg.create_label(T_ic + "=" + name1_ic + tem_ic + name_ic);
                    result_ic = T_ic;
                    if (E_OE1(type3, out type,out result_ic, T_ic))
                    {
                        return true;
                    }
                }
            }
            else if (token_array[a, 0] == "ROP"|| token_array[a, 0] == "AND" || token_array[a, 0] == "OR" || token_array[a, 0] == "}" || token_array[a, 0] == "AsgOp" || token_array[a, 0] == "Equal" || token_array[a, 0] == ")" || token_array[a, 0] == ";" || token_array[a, 0] == "," || token_array[a, 0] == "]")
            {
                type = type1;
                return true;
            }
            return false;

        }


        public bool T_OE(out string type,out string name1_ic) //<T_OE>→<F_OE><T_OE1>
        {
            type = "";
            string type1;
              name1_ic = "";
            string name_ic;
            if (F_OE(out type1,out name_ic))//<F_OE>
            {
                
                if (T_OE1(type1, out type,out name1_ic, name_ic))//<T_OE1>
                {
                    return true;
                }

            }
            return false;

        }
        public bool T_OE1(string type1, out string type,out string result_ic, string name1_ic)//<T_OE1>→MDM<F_OE><T_OE1>|€
        {
            type = "";
            string operators;
            string class_part;
            string type2;
            string type3;
            result_ic = name1_ic;
            if (token_array[a, 0] == "MDM")
            {
                operators = token_array[a, 1];
                class_part = token_array[a, 0];
                string tem_ic = token_array[a, 1];
                a = a + 1;
                string name_ic;
                if (F_OE(out  type2,out name_ic)) //<F_OE>
                {
                    type3 = re.type_type(type1, type2, operators, class_part);
                    
                    if (type3 == "type mismatch")
                    {
                        sym_error = type3;
                        return false;
                    }
                    string T_ic = cg.temp_var();///CREATE TEMP
                    cg.create_label(T_ic + "=" + name1_ic + tem_ic + name_ic);
                    result_ic = T_ic;
                    
                    if (T_OE1(type3, out type,out result_ic, T_ic))
                    {
                        return true;
                    }
                }
            }

            else if (token_array[a, 0] == "PM"  || token_array[a, 0] == "ROP" || token_array[a, 0] == "AND" || token_array[a, 0] == "OR" || token_array[a, 0] == "}" || token_array[a, 0] == "AsgOp" || token_array[a, 0] == "Equal" || token_array[a, 0] == ")" || token_array[a, 0] == ";" || token_array[a, 0] == "," || token_array[a, 0] == "]")
            {
                type = type1;
                return true;
            }

            return false;

        }

        public bool F_OE(out string type,out string name1_ic)//<F_OE>→<const>|!(<OE>)|DI ID<ID_array>| this. <F’>|ID <F’’>
        {
            string operators,name;
            name1_ic = "";
            if (_const(out type,out name1_ic)) //add <const>
            {
                
                return true;
            }
            if (token_array[a, 0] == "Not")//!(<OE>)
            {
                operators = token_array[a, 1];
                a = a + 1;
                if (token_array[a, 0] == "(")
                {
                    a = a + 1;
                    if (OE(out type))
                    {
                        if (re.unary_type(type, operators) == "invalid type")
                        {
                            sym_error = "invalid type";
                            return false;
                        }
                        string T_ic = cg.temp_var();///CREATE TEMP
                        name1_ic = T_ic + "=" + "!(" + oe_ic + ")";
                        cg.create_label(name1_ic);

                        if (token_array[a, 0] == ")")
                        {
                            a = a + 1;
                            return true;

                        }
                    }
                }

            }
            else if (token_array[a, 0] == "DI")//DI ID<ID_array>| 
            {
              
                   operators = token_array[a, 1];
                if (token_array[a, 1] == "++")
                {
                    name1_ic += "1+_";
                }
                else
                {
                    name1_ic += "1-_";
                }
                
                a = a + 1;
                if (token_array[a, 0] == "ID")
                {
                    overloadcount = 0;
                    name = token_array[a, 1]; // lookup
                    if (!re.scope_lookup(name))
                    {
                        if (!re.inherited_func_lookup(Name1, name))
                        {
                           if (sym_error == "")
                            { sym_error = "undeclared variable-- '" + name + "'"; }
                            return false;
                        }
                       
                    }
                      string T_ic = cg.temp_var();
                    cg.create_label(T_ic + "=" + name1_ic + token_array[a, 1]);
                    name1_ic += T_ic;
               
                    a = a + 1;
                    string type1="";
                    string name_ic;
                    if (ID_array(out type1,out name_ic, "")) //add <ID_array>
                    {
                        //int h=++l[];
                        if (CT.attribute_type.Contains(type1)&& type1!="")  //int[,]=
                        {
                            if (!CT.attribute_type.Contains("int") && !CT.attribute_type.Contains("float") && !CT.attribute_type.Contains("char"))
                            {
                                sym_error = "increment/decrement invalid type -->" + CT.attribute_type;
                                return false;
                            }
                            type = CT.attribute_type.Split('[')[0];
                        }
                        else if(type1=="")
                        {
                            if (CT.attribute_type != "int" && CT.attribute_type != "float" && CT.attribute_type != "char")
                            {
                                sym_error = "increment/decrement invalid type -->" + CT.attribute_type;
                                return false;
                            }
                            type = CT.attribute_type;
                        }
                        CT.attribute_type = "";
                        name1_ic += name_ic;
                        return true;
                    }

                }

            }
            else if (token_array[a, 0] == "this")//this. <F1>
            {
                string n_ic = token_array[a, 0];
                a = a + 1;
                if (token_array[a, 0] == ".")
                {
                    count = 0;
                    a = a + 1;
                    string name_ic;

                    if (F_1(out type,"",out name_ic, n_ic)) //add <F_1>
                    {
                        name1_ic = name_ic;
                 
                        return true;
                    }


                }

            }
            else if (token_array[a, 0] == "ID")//ID <F11>
            {
                count = 0;

                type = "";

                name = token_array[a, 1];

                if (!re.scope_lookup(name))
                {
                    if (!re.inherited_func_lookup(Name1, name))
                    {
                        if (sym_error == "")
                        { sym_error = "undeclared variable-- '" + name + "'"; }
                        return false;
                    }

                }
                    count++;

                name1_ic = token_array[a, 1];
                
                a = a + 1;
                string name_ic;
                if (F_11(name,out type,out name_ic, name1_ic)) //add <F_11>a[]
                {
                    name1_ic = name_ic;

                    return true;
                }
            }
            return false;
        }
        public int count = 0;
      
        //s int a;  b.c.a;
       public static int overloadcount=0;
       Stack overload = new Stack();

        public bool F_1(out string type,string name1,out string name_ic, string name_in_ic)
        {
            string name;
            type = "";
            name_ic = name_in_ic;

            if (token_array[a, 0] == "ID")  //int d = this.c.b.a;
            {
                name = token_array[a, 1];
          L1:              
                if (count == 0)
                {
                if (!re.lookup_class(name, "", ""))
                {
                    sym_error = "undeclared global variable -- '" + name+ " '";
                    return false;
                }
                CT.parent_type = "";
                count++;
                }
                else
                {
                    overloadcount = 0;
                foreach (array_node aaa in CT.parent_ref)
                {

                    if (name == aaa.name)
                    {
                    
                    
                    CT.attribute_type = aaa.return_type;
                    CT.attribute_cat = aaa.parameter_list;
                    CT.attribute_AM = aaa.AM;
                    overloadcount++;

                    }
                   
                }
                
                    
                    if (CT.parent != "" && overloadcount == 0)
                {
                    string[] split_parent = CT.parent.Split(',');//claa B:A,C{}

                    for (int i = 0; i < split_parent.Length; i++)
                    {
                        if (re.lookup(split_parent[i]))
                        {
                            if (CT.parent_type == "class")
                            {
                                CT.parent_type = "";
                                goto L1;
                            } 
                        }
                    }
                  
                }          
                if (overloadcount == 0)
                {
                    sym_error = "undeclared variable " + name + " in class " + re.name_class;
                    return false;
                }
                }
                if (CT.attribute_cat == "static" )
                {
                    if(name1 !=re.name_class)
                    {
                        sym_error = "Static variable '" + name + "' cannot be accessed with an instance reference";
                        return false;
                    }
                   
                }
                if (CT.attribute_AM == "private" && count -1!=0 && overloadcount <= 1)    
                {
                    sym_error = "Private variable '" + name + "' can be accessed only within class";
                    return false;
                }
                if ((CT.parent_type == "class" || CT.parent_type == "interface") && CT.attribute_cat != "static")
                {
                    sym_error = " '"+ name + "' is not a static variable";
                    return false;
                }
                name_in_ic += "_" + token_array[a, 1];
                
               
                a = a + 1;
                if (F_11(name,out type,out name_ic, name_in_ic)) //add <F11>
                {

                    

                    return true;
                }
            }
            return false;

        }
       
        public bool F_11(string name,out string type,out string name_ic, string name_in_ic)//<F11>→.<F1> | [<OE><token_array_call>] <Dec_inc> | DI| €
        {
            string arguments = "";
            type = "";
            name_ic = name_in_ic;
            if (token_array[a, 0] == ".")//.<F1>
            {
                if (!re.lookup(CT.attribute_type))///A a B b a.b
                {
                    CT.parent_type = "";
                    if (!re.lookup(name))
                    {
                    sym_error =  name +" is not a reference variable";
                    return false;
                    }
                }
                else
                {
                    CT.parent_type = "";
                }
                //ref
                count++;
                a = a + 1;
                type = CT.attribute_type;
                if (F_1(out type,name,out name_ic, name_in_ic))
                {
                   
                    return true;
                }

            }
            else if (token_array[a, 0] == "[")//[<OE><token_array_call>] <Dec_inc> 
            {
               string type1 = CT.attribute_type;
                name_ic += token_array[a, 0];
                a = a + 1;
                string type2 = "";
                if (OE(out type2))
                {

                    if (type2 != "int" && type2 != "char")
                    {
                        sym_error = "index must be integer";
                        return false;
                    }
                    string type3;
                    string name_ic1 = oe_ic;
                    name_ic += name_ic1;
                    string n_ic;
                    if (arr_call(out type3,out n_ic, ""))//a[10,]++ int[,]
                    {
                        name_ic += n_ic;
                        if (token_array[a, 0] == "]")
                        {
                            name_ic += token_array[a, 0];

                            a = a + 1;

                           CT.attribute_type = type1;//int[]
                            if (CT.attribute_type.Contains('['))
                            {
                               
                                string arr = CT.attribute_type.Split('[')[1];
                               
                                if (type3 == "[" && arr == "]")
                                {
                                    string tem_ic;
                                    if (F_111(name, out type, out tem_ic, ""))//
                                    {
                                        
                                        type = type.Split('[')[0];

                                        name_ic += tem_ic;
                                        
                                        
                                        
                                        return true;
                                    }
                                }
                                else if (type3 != "[" && arr == ",]")
                                {
                                    string tem_ic;
                                    if (F_111(name,out type, out tem_ic, ""))
                                    {
                                     
                                        type = type.Split('[')[0];
                                        name_ic += tem_ic;
                                        return true;
                                    }
                                }
                                else
                                {
                                    sym_error = name + " array dimension mismatch ["+CT.attribute_type.Split('[')[1];
                                    return false;
                                }
                            }
                            else
                            {
                                sym_error = name + " is not declared as array" ;
                                return false;
                            }
                                
                           
                            
                        }
                    }



                }

            }
                
            else if (token_array[a, 0] == "DI")
            {

                    if (CT.attribute_type != "int" && CT.attribute_type != "float" && CT.attribute_type != "char")
                    {
                        sym_error = "increment/decrement invalid type -->" + CT.attribute_type;
                        return false;
                    }
                    type = CT.attribute_type;
                string T_ic = cg.temp_var();
                if (token_array[a, 1] == "++")
                {
                    name_in_ic += "+1";//A_+1
                    cg.create_label(T_ic + "=" + name_in_ic);
                    name_ic = T_ic;
                }
                else
                {
                    name_in_ic += "-1";
                    cg.create_label(T_ic + "=" + name_in_ic);
                    name_ic = T_ic;

                }
                a = a + 1;
        //        CT.attribute_type = "";

             
              
                return true;

            }
                
            else if (token_array[a, 0] == "(")//(<argument>)<F_OE11> 
            {

                string PL = CT.attribute_cat;
                string type1 = CT.attribute_type;
                overload.Push(overloadcount);
                a++;
                if (argument(out arguments,arguments, out name_ic, name_in_ic))//p A[],A,A[,]
                {
                   
                    if (token_array[a, 0] == ")")
                    {
                        if (overloadcount > 1)
                        {
                            foreach (array_node aaa in CT.parent_ref)
                            {

                                if (name == aaa.name)
                                {
                                    if (aaa.parameter_list == arguments)
                                    {
                                        if (aaa.AM == "public")
                                        {
                                            type1 = aaa.return_type;
                                            PL = aaa.parameter_list;
                                            CT.attribute_AM = aaa.AM;
                                        }
                                        else
                                        {
                                            sym_error = "private variable" + name + "can only accessed within class";
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                            
                            if (overload.Count > 0)
                            {
                                overload.Pop();
                                if (overload.Count > 0)
                                {
                                overloadcount = Convert.ToInt16(overload.Peek());
                                }
                            }
                        CT.attribute_cat = PL;
                        CT.attribute_type = type1;
                       
                        if (arguments != CT.attribute_cat)
                        {
                            sym_error = "invalid arguments of function --'" + name + "'";
                            return false;
                        }
                        if (CT.attribute_type == "void_ctor")//
                        {
                            sym_error="Use the new keyword if hiding was intended.";

                            return false;
                        }
                        a++;
                        CT.parent_type = "";/////////////
                         string name1_ic;
                        name_in_ic = "call " + name_in_ic + "," + (i_ic++);
                        i_ic = 0;
                        string T_ic = cg.temp_var();
                        cg.create_label(T_ic + "=" + name_in_ic);
                        if (OE_func(name,out type, out name1_ic, T_ic))
                        {
                            name_ic = name1_ic;
                            return true;
                        }
                    }
                }
            }
                //int a[];----a int[]
            else if (token_array[a, 0] == "MDM" || token_array[a, 0] == "}" || token_array[a, 0] == "PM" || token_array[a, 0] == "ROP" || token_array[a, 0] == "AND" || token_array[a, 0] == "OR" || token_array[a, 0] == "}" || token_array[a, 0] == "AsgOp" || token_array[a, 0] == "Equal" || token_array[a, 0] == ")" || token_array[a, 0] == ";" || token_array[a, 0] == "," || token_array[a, 0] == "]")
            {
               
               
                type = CT.attribute_type;
              
                if (type.Contains("[")&&!return_array)
                {
                    sym_error = name + " is an array";
                    return false;
                }
               // CT.attribute_type = "";
                return true;
            }

            return false;
        }
     
        public bool OE_func(string name,out string type,out string name_ic, string name_in_ic) //<OE_func> -> .<F_OE1> | null///A---a[5].--A[]
        {

            type = CT.attribute_type;
             name_ic = name_in_ic;

            if (token_array[a, 0] == ".")
            {
                count++;
                if (CT.attribute_type.Contains('['))
                {
                    type = CT.attribute_type.Split('[')[0];
                    if (!re.lookup(type))
                    {
                        sym_error = name + " is not a reference variable";
                        return false;
                    }
                }
                else
                {
                    if (!re.lookup(type))
                    {
                        sym_error = name + " is not a reference variable";
                        return false;
                    }
                }

                a++;
                CT.parent_type = "";
                if (F_1(out type,"",out name_ic, name_in_ic))
                {
                    return true;
                }
               
            }
          
            else if (token_array[a, 0] == "MDM"  || token_array[a, 0] == "}" || token_array[a, 0] == "PM" || token_array[a, 0] == "ROP" || token_array[a, 0] == "AND" || token_array[a, 0] == "OR" || token_array[a, 0] == "}" || token_array[a, 0] == "AsgOp" || token_array[a, 0] == "Equal" || token_array[a, 0] == ")" || token_array[a, 0] == ";" || token_array[a, 0] == "," || token_array[a, 0] == "]")
            {
                
                type = CT.attribute_type;
               
                if (overload.Count > 0)
                {
                    overload.Pop();
                    if (overload.Count >0)
                    {
                    overloadcount = Convert.ToInt16(overload.Peek());
                    }
                }
               // CT.attribute_type = "";
                return true;
            }


            return false;
        }

        public bool F_111(string name, out string type,out string name_ic, string name_in_ic) //<OE_func> -> .<F_OE1> | null///A---a[5].--A[]
        {

            type = CT.attribute_type;
             name_ic = "";

            if (token_array[a, 0] == ".")
            {
                count++;
                if (CT.attribute_type.Contains('['))
                {
                    type = CT.attribute_type.Split('[')[0];
                    if (!re.lookup(type))
                    {
                        sym_error = name + " is not a reference variable";
                        return false;
                    }
                }
                else
                {
                    if (!re.lookup(type))
                    {
                        sym_error = name + " is not a reference variable";
                        return false;
                    }
                }

                a++;
                CT.parent_type = "";
                if (F_1(out type, "",out name_ic, name_in_ic))
                {
                    return true;
                }
            }
            else 
                if (token_array[a, 0] == "DI")
            {

                if (CT.attribute_type.Contains(type) && type != "")  //int[,]=
                {
                    if (!CT.attribute_type.Contains("int") && !CT.attribute_type.Contains("float") && !CT.attribute_type.Contains("char"))
                    {
                        sym_error = "increment/decrement invalid type -->" + CT.attribute_type;
                        return false;
                    }
                    type = CT.attribute_type.Split('[')[0];
                }
                else if (type == "")
                {
                    if (CT.attribute_type != "int" && CT.attribute_type != "float" && CT.attribute_type != "char")
                    {
                        sym_error = "increment/decrement invalid type -->" + CT.attribute_type;
                        return false;
                    }
                    type = CT.attribute_type;
                }
                type = CT.attribute_type;
               // CT.attribute_type = "";





                //if (CT.attribute_type != "int" && CT.attribute_type != "float" && CT.attribute_type != "char")
                //{
                //    sym_error = "increment/decrement invalid type -->" + CT.attribute_type;
                //    return false;
                //}
                if (token_array[a, 1] == "++")
                {
                    name_ic = "_+1";
                }
                else
                {
                    name_ic = "_-1";
                }
                a++;
                return true;
            }
            else if (token_array[a, 0] == "MDM" || token_array[a, 0] == "}" || token_array[a, 0] == "PM" || token_array[a, 0] == "ROP" || token_array[a, 0] == "AND" || token_array[a, 0] == "OR" || token_array[a, 0] == "}" || token_array[a, 0] == "AsgOp" || token_array[a, 0] == "Equal" || token_array[a, 0] == ")" || token_array[a, 0] == ";" || token_array[a, 0] == "," || token_array[a, 0] == "]")
            {

                type = CT.attribute_type;
             //   CT.attribute_type = "";
                return true;
            }


            return false;
        }
        //public bool F_111() //<OE_func> -> .<F_OE1> | null
        //{
        //    if (token_array[a, 0] == "DI")
        //    {
        //        a++;
        //        return true;
        //    }
        //    else if (token_array[a, 0] == ".")
        //    {
        //        a++;
        //        string type3;
        //        if (F_1(out type3))
        //        {
        //            return true;
        //        }
        //    }
        //    else if (token_array[a, 0] == "MDM" || token_array[a, 0] == "}" || token_array[a, 0] == "PM" || token_array[a, 0] == "ROP" || token_array[a, 0] == "AND" || token_array[a, 0] == "OR" || token_array[a, 0] == "}" || token_array[a, 0] == "AsgOp" || token_array[a, 0] == "Equal" || token_array[a, 0] == ")" || token_array[a, 0] == ";" || token_array[a, 0] == "," || token_array[a, 0] == "]")
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        public bool OE_class_MST(out string type)//<OE>→<A_OE><OE1>
        {
            string type1;
            type = "";
            if (A_OE_class_MST(out type1)) //<A_OE><T>
            {
                if (OE_class_MST_1(type1,out type))//<OE1><E'>
                {
                    
                    return true;
                }
            }
            return false;
        }
        public bool OE_class_MST_1(string type1,out string type)//<OE1>→OR<A_OE><OE1>|€-----<E>'
        {
            string operators;
            string class_part;
            string type2;
            string type3;
            type = "";
            if (token_array[a, 0] == "OR")
            {
                operators = token_array[a, 1];
                class_part = token_array[a, 0];


                a = a + 1;
                if (A_OE_class_MST(out  type2)) //<A_OE>
                {
                    type3 = re.type_type(type1, type2, operators, class_part);
                    if(type3=="type mismatch")
                    {
                        sym_error = type3;
                        return false;
                    }
                    if (OE_class_MST_1(type3,out type))
                    {

                        return true;
                    }
                }
            }
            else if (token_array[a, 0] == "Equal" || token_array[a, 0] == "}" || token_array[a, 0] == "]" || token_array[a, 0] == ";" || token_array[a, 0] == "," || token_array[a, 0] == ")")
            {
                type = type1;

                return true;
            }

            return false;

        }

        public bool A_OE_class_MST(out string type) //<A_OE>→<R_OE><A_OE1>
        {
            type = "";
            string type1;
            if (R_OE_class_MST(out type1))//<R_OE>
            {
                if (A_OE_class_MST_1(type1,out type))//<A_OE1>
                {
                    return true;
                }

            }
            return false;

        }
        public bool A_OE_class_MST_1(string type1 ,out string type)//<A_OE1>→AND <R_OE><A_OE1>|€
        {
            type = "";
            string operators;
            string class_part;
            string type2;
            string type3;
            if (token_array[a, 0] == "AND")
            {
                operators = token_array[a, 1];
                class_part = token_array[a, 0];
                a = a + 1;
                if (R_OE_class_MST(out type2)) //<R_OE>
                {
                    type3 = re.type_type(type1, type2, operators, class_part);
                    if (type3 == "type mismatch")
                    {
                        sym_error = type3;
                        return false;
                    }
                    if (A_OE_class_MST_1(type3,out type))
                    {
                        return true;
                    }
                }
            }
            else if (token_array[a, 0] == "OR" || token_array[a, 0] == "}" || token_array[a, 0] == "]" || token_array[a, 0] == "Equal" || token_array[a, 0] == ";" || token_array[a, 0] == "," || token_array[a, 0] == ")")
            {
                type = type1;
                return true;
            }


            return false;

        }
        public bool R_OE_class_MST(out string type) //<R_OE>→<E_OE><R_OE1>
        {
            type = "";
            string type1;
            if (E_OE_class_MST(out type1))//<E_OE>
            {
                if (R_OE_class_MST_1(type1, out type))//<R_OE1>
                {
                    return true;
                }

            }
            return false;

        }
        public bool R_OE_class_MST_1(string type1, out string type)//<R_OE1>→ROP<E_OE><R_OE1>|€
        {
            type = "";
            string operators;
            string class_part;
            string type2;
            string type3;
            if (token_array[a, 0] == "ROP")
            {
                operators = token_array[a, 1];
                class_part = token_array[a, 0];

                a = a + 1;
                if (E_OE_class_MST(out  type2)) //<E_OE>
                {
                    type3 = re.type_type(type1, type2, operators, class_part);
                    if (type3 == "type mismatch")
                    {
                        sym_error = type3;
                        return false;
                    }
                    if (R_OE_class_MST_1(type3, out type))
                    {
                        return true;
                    }
                }
            }
            else if (token_array[a, 0] == "AND" || token_array[a, 0] == "}" || token_array[a, 0] == "]" || token_array[a, 0] == "OR" || token_array[a, 0] == "Equal" || token_array[a, 0] == ";" || token_array[a, 0] == "," || token_array[a, 0] == ")")
            {
                type = type1;
                return true;
            }

            return false;

        }

        public bool E_OE_class_MST(out string type) //<E_OE>→<T_OE><E_OE1>
        {
            type = "";
            string type1;
            if (T_OE_class_MST(out type1))//<T_OE>
            {
                if (E_OE_class_MST_1(type1, out type))//<R_OE1>
                {
                    return true;
                }

            }
            return false;

        }
        public bool E_OE_class_MST_1(string type1, out string type)//<E_OE’>→PM<T_OE><E_OE1>|€
        {
            type = "";
            string operators;
            string class_part;
            string type2;
            string type3;
            if (token_array[a, 0] == "PM")
            {
                operators = token_array[a, 1];
                class_part = token_array[a, 0];

                a = a + 1;
                if (T_OE_class_MST(out  type2)) //<T_OE>
                {
                    type3 = re.type_type(type1, type2, operators, class_part);
                    if (type3 == "type mismatch")
                    {
                        sym_error = type3;
                        return false;
                    }
                    if (E_OE_class_MST_1(type3, out type))
                    {
                        return true;
                    }
                }
            }
            else if (token_array[a, 0] == "ROP" || token_array[a, 0] == "}" || token_array[a, 0] == "]" || token_array[a, 0] == "AND" || token_array[a, 0] == "OR" || token_array[a, 0] == "Equal" || token_array[a, 0] == ";" || token_array[a, 0] == "]" || token_array[a, 0] == "," || token_array[a, 0] == ")")
            {
                type = type1;
                return true;
            }

            return false;

        }


        public bool T_OE_class_MST(out string type) //<T_OE>→<F_OE><T_OE1>
        {
            type = "";
            string type1;
            if (F_OE_class_MST(out type1))//<F_OE>
            {
                if (T_OE_class_MST_1(type1, out type))//<T_OE1>
                {
                    return true;
                }

            }
            return false;

        }
        public bool T_OE_class_MST_1(string type1, out string type)//<T_OE1>→MDM<F_OE><T_OE1>|€
        {
            type = "";
            string operators;
            string class_part;
            string type2;
            string type3;
            if (token_array[a, 0] == "MDM")
            {
                operators = token_array[a, 1];
                class_part = token_array[a, 0];
                a = a + 1;
                if (F_OE_class_MST(out  type2)) //<F_OE>
                {
                    type3 = re.type_type(type1, type2, operators, class_part);
                    if (type3 == "type mismatch")
                    {
                        sym_error = type3;
                        return false;
                    }
                    if (T_OE_class_MST_1(type3,out type))
                    {
                        return true;
                    }
                }
            }
            else if (token_array[a, 0] == "PM" || token_array[a, 0] == "}" || token_array[a, 0] == "ROP" || token_array[a, 0] == "AND" || token_array[a, 0] == "OR" || token_array[a, 0] == "Equal" || token_array[a, 0] == ";" || token_array[a, 0] == "," || token_array[a, 0] == ")" || token_array[a, 0] == "]")
            {
                
                type = type1;
                return true;
            }

            return false;

        }

        public bool F_OE_class_MST(out string type)//<F_OE>→<const>|!(<OE>)|DI ID<ID_array>| this. <F’>|ID <F’’>
        {
            type = "";
            string operators;
            string name_ic;
            if (_const(out type,out name_ic)) //add <const>
            {
                
                return true;
            }
            else if (token_array[a, 0] == "Not")//!(<OE>)
            {
                operators = token_array[a, 1];
                a = a + 1;
                if (token_array[a, 0] == "(")
                {
                    a = a + 1;
                    if (OE_class_MST(out type))
                    {
                        if (re.unary_type(type, operators) == "invalid type")
                        {
                            sym_error = "invalid type";
                            return false;
                        }


                        if (token_array[a, 0] == ")")
                        {
                            type = "bool";
                            a = a + 1;
                            return true;

                        }
                    }
                }

            }

            
            return false;
        }
        public bool SST_C_1111(string name, string AM, string type, string cat)//<F_OE>→<const>|!(<OE>)|DI ID<ID_array>| this. <F’>|ID <F’’>
        {
           
            if (list_attributes(name, AM, type, cat))
            {
                return true;
            }
            else if (token_array[a, 0] == "[")
            {
                type += "[";
                a++;
                string type1 = "";
                if (OE_class_MST(out type1)) //must be int [10]
                {
                    if (type1 != "int" && type1 !="char")
                    {
                        sym_error = "index must be integer";
                        return false;
                    }
                    if (A_Array_MST(name,type,AM))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /*******************extra function for arguments******************/
        public string type_conversion(string type)
        
        {
            switch (type)
            {
                case "Integer_Constant":
                    {
                        return "int";
                        
                    }
                case "Float_Constant":
                    {
                        return "float";

                    }
                case "String_Constant":
                    {
                        return "string";

                    }
                case "Char_Constant":
                    {
                        return "char";

                    }
                case "Bool_Constant":
                    {
                        return "bool";

                    }
            }
        return "";
        }
        /*******************extra function for arguments******************/


        public bool _const(out string type,out string name_ic)
        {
            type = "";
            name_ic = "";

            if (token_array[a, 0] == "Integer_Constant" || token_array[a, 0] == "Float_Constant" || token_array[a, 0] == "String_Constant" || token_array[a, 0] == "Char_Constant" || token_array[a, 0] == "Bool_Constant")
            {

                type = type_conversion(token_array[a,0]);
                name_ic = token_array[a, 1];

                a++;
                return true;
            }
            return false;
        }


        /*-------------------------------------------------------EXTRA------------------------------------------------------------*/
        public bool ret_T_S(out string type,out string tem2_ic,string tem_ic)//<ret_T> → void | DT<const_array> |  ID<const_array> // 
        {
            tem2_ic = tem_ic;
            type = "";
            if (token_array[a, 0] == "void")
            {
                type = token_array[a, 1];
                a = a + 1;
                return true;

            }
            else if (token_array[a, 0] == "DT")//add <const>
            {
                tem_ic += "_" + token_array[a, 1];//ic_code
                tem2_ic = tem_ic;//ic_code

                type = token_array[a, 1];
                a = a + 1;
                if (const_array_S(out type, type))//add <const_token_arrayay>
                {
                   
                    if (type.Contains('['))
                    {
                        tem2_ic += "[" + type.Split('[')[1];
                      
                    }
                    return true;
                }
            }
            else if (token_array[a, 0] == "ID")
            {

                type = token_array[a, 1];
                a = a + 1;
                if (const_array_S(out type, type))//add <const_token_arrayay>
                {
                    tem2_ic += "_" + type;
                    return true;
                }
            }
            return false;
        }
        public bool const_array_S(out string type_out, string type_in)
        {
           
            type_out = type_in;
            if (C_ret_S(out type_out, type_out))
            {
                return true;
            }
            else if (token_array[a, 0] == "ID")
            {

                return true;
            }
            return false;
        }
        public bool C_ret_S(out string type_out, string type_in)
        { 
            type_out = type_in;
            if (token_array[a, 0] == "[")
            {
                
           
                type_out = type_out + token_array[a, 1];
                a = a + 1;
                if (C_ret1_S(out type_out, type_out))
                {
                    return true;
                }
            }
            


            return false;

        }

        public bool C_ret1_S(out string type_out, string type_in)
        {
            
            type_out = type_in;
            if (token_array[a, 0] == ",")
            {
                
                type_out = type_out + token_array[a, 1];
                a = a + 1;
                if (token_array[a, 0] == "]")
                {

                    type_out = type_out + token_array[a, 1];
                   
                    a = a + 1;
                    return true;

                }

            }
            else if (token_array[a, 0] == "]")
            {

              
                type_out = type_out + token_array[a, 1];
                a = a + 1;
                return true;

            }



            return false;
        }

        public bool parameter_S(out string type_out, string type_in,out string temp_ic,string temp_in_ic) //<parameter>→DT <const_array> ID<add_para> |ID <const_array> ID <add_para>| €
        {
            string type_array;
            temp_ic = temp_in_ic;
            type_out = type_in;

            if (token_array[a, 0] == "DT")
            {
                temp_in_ic += "_" + token_array[a, 1];
                type_array = token_array[a, 1]; 
                type_in += token_array[a, 1];  //int,strinng
                a = a + 1;
                if (const_array_S(out type_in,type_in))
                {
                    
                    if (token_array[a, 0] == "ID")
                    {
                        
                        if (token_array[a, 0] == "ID")
                        {
                            string[] array = type_in.Split(',');
                            string temp;

                            if (array[array.Length - 1] == "]")
                            {
                                temp = array[array.Length - 2] +","+ array[array.Length - 1];
                            }
                            else
                            {
                                temp = array[array.Length - 1];
                            }
                            if (!re.insert_func(token_array[a, 1], temp,Convert.ToInt32( scope_stack.Peek())))
                            {
                                sym_error = "redeclaration of local variable";
                                return false;
                            }

                            a++;
                            if (add_para_S(out type_out, type_in,out temp_ic,temp_in_ic))
                            {
                                return true;
                            }
                        }
                    }

                }
            }
            else if (token_array[a, 0] == "ID")
            {
                temp_in_ic += "_" + token_array[a, 1];
                type_array = token_array[a, 1];
                type_in += token_array[a, 1];
             
                a = a + 1;
                if (const_array_S(out type_in, type_in))
                {
                    if (token_array[a, 0] == "ID")
                    {
                        string[] array = type_in.Split(',');
                        string temp;

                        if (array[array.Length - 1] == "]")
                        {
                            temp = array[array.Length - 2] + "," + array[array.Length - 1];
                        }
                        else
                        {
                            temp = array[array.Length - 1];
                        }
                        if (!re.insert_func(token_array[a, 1], temp, Convert.ToInt32(scope_stack.Peek())))
                        {
                            sym_error = "redeclaration of local variable";
                            return false;
                        }

                        a++;
                        if (add_para_S(out type_out, type_in,out temp_ic,temp_in_ic))
                        {
                            return true;
                        }
                    }

                }
            }
            else if (token_array[a, 0] == ")")
            {
                if (type_out == "")
                {
                    type_out = "void";
                }
                temp_ic = temp_in_ic;
                return true;
            }
            return false;
        }
        public bool add_para_S(out string type_out, string type_in, out string temp_ic, string temp_in_ic)//<add_para>→,<parameter> | €
        {
            temp_ic = temp_in_ic;
            type_out = type_in;
            if (token_array[a, 0] == ",")
            {
                
                type_in += token_array[a, 1]; //int,
                a = a + 1;

                if (parameter_S(out type_out, type_in,out temp_ic,temp_in_ic))//int
                {
                    return true;
                }

            }
            else if (token_array[a, 0] == ")")//null
            {
                temp_ic = temp_in_ic;
                return true;
            }
            return false;
        }
    }
}

