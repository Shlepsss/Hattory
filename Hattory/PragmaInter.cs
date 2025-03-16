using System;
using System.Collections.Generic;
using System.Drawing;
using Cosmos;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using sus = Cosmos.System;
using ConsoleKeyy = Cosmos.System.ConsoleKeyEx;
using lol = Hattory.Kernel;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using Cosmos.System;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace Hattory
{
    public class PragmaInterpritator
    {
        public static List<string> variables = new List<string>(); // переменные
        public static List<string> code = new List<string>();
        public static bool inIf = false;
        public static bool inElse = false;
        public static bool IfTrue = false;
        public static int linee = 1;
        public static void Execute()
        {
            try
            {
                foreach (string line in code)
                {
                    string lin = line;
                    while (lin.StartsWith(' ') || lin.EndsWith(' ')) { lin = lin.Trim(); }

                    // УСЛОВНЫЕ ОПЕРАТОРЫ
                    if(inElse || inIf)
                    {
                        if (lin.StartsWith("endcond", true, null))
                        {
                            inIf = false;
                            inElse = false;
                            IfTrue = false;
                        }
                        else if (lin.StartsWith("else", true, null))
                        {
                            if (inIf == true && IfTrue == false)
                            {
                                inElse = true;
                                inIf = false;
                            }
                            else
                            {
                                inElse = false;
                                inIf = true;
                                IfTrue = false;
                            }
                        } // ELSE
                    }
                    
                    else if (lin.StartsWith("else", true, null) && (inIf == false || inElse == true))
                    {
                        FpsShower.Msg("Error in line " + linee, "Wrong If-Else construction!", false);
                        FpsShower.playSound = true;
                        Kernel.isExecute = false;
                    } // ELSE без IF

                    // MAIN CODE

                    if ((!inIf) || (inIf && IfTrue) || (inElse)) 
                    {
                        #region graphics
                        if (lin.StartsWith("text ", true, null))
                        {
                            lin = lin.Substring(5);
                            string[] parts = lin.Split("||");
                            string textx = lin.Split("||")[0].Trim();
                            string texty = lin.Split("||")[1].Trim();
                            string text = lin.Split("||")[2].Trim();
                            string color = lin.Split("||")[3].Trim();
                            foreach (var item in parts)
                            {
                                if (item.StartsWith("&&"))
                                {
                                    string ikalka = item.Substring(2).Trim();
                                    foreach (var var in variables)
                                    {
                                        string[] varParts = var.Split(':');
                                        if (varParts[0] == ikalka)
                                        {
                                            string value = varParts[1];
                                            if (parts[0].Substring(2) == ikalka)
                                            {
                                                textx = value;
                                            }
                                            if (parts[1].Substring(2) == ikalka)
                                            {
                                                texty = value;
                                            }
                                            if (parts[2].Substring(2) == ikalka)
                                            {
                                                text = value;
                                            }
                                            if (parts[3].Substring(2) == ikalka)
                                            {
                                                color = value;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            Otrisovka.Write(text, int.Parse(textx), int.Parse(texty), Color.FromName(color));
                        } // ТЕКСТ
                        else if (lin.StartsWith("rect ", true, null))
                        {
                            lin = lin.Substring(5);
                            string[] parts = lin.Split("||");
                            // Парсинг начальных значений
                            string x = parts[0].Trim();
                            string y = parts[1].Trim();
                            string wi = parts[2].Trim();
                            string he = parts[3].Trim();
                            string color = parts[4].Trim();
                            // Обработка переменных
                            foreach (var item in parts)
                            {
                                if (item.StartsWith("&&"))
                                {
                                    string ikalka = item.Substring(2).Trim();
                                    foreach (var var in variables)
                                    {
                                        string[] varParts = var.Split(':');
                                        if (varParts[0] == ikalka)
                                        {
                                            string value = varParts[1];
                                            if (parts[0].Substring(2) == ikalka)
                                            {
                                                x = value;
                                            }
                                            if (parts[1].Substring(2) == ikalka)
                                            {
                                                y = value;
                                            }
                                            if (parts[2].Substring(2) == ikalka)
                                            {
                                                wi = value;
                                            }
                                            if (parts[3].Substring(2) == ikalka)
                                            {
                                                he = value;
                                            }
                                            if (parts[4].Substring(2) == ikalka)
                                            {
                                                color = value;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            Kernel.canvas.DrawFilledRectangle(Color.FromName(color), int.Parse(x), int.Parse(y), int.Parse(wi), int.Parse(he));
                        } // ПРЯМОУГОЛЬНИК
                        else if (lin.StartsWith("circle ", true, null))
                        {
                            lin = lin.Remove(0, 7);
                            string[] parts = lin.Split("||");
                            string x = parts[0].Trim();
                            string y = parts[1].Trim();
                            string rad = parts[2].Trim();
                            string color = parts[3].Trim();
                            foreach (var item in parts)
                            {
                                if (item.StartsWith("&&"))
                                {
                                    string ikalka = item.Substring(2).Trim();
                                    foreach (var var in variables)
                                    {
                                        string[] varParts = var.Split(':');
                                        if (varParts[0] == ikalka)
                                        {
                                            string value = varParts[1];
                                            if (parts[0].Substring(2) == ikalka)
                                            {
                                                x = value;
                                            }
                                            if (parts[1].Substring(2) == ikalka)
                                            {
                                                y = value;
                                            }
                                            if (parts[2].Substring(2) == ikalka)
                                            {
                                                rad = value;
                                            }
                                            if (parts[3].Substring(2) == ikalka)
                                            {
                                                color = value;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            Kernel.canvas.DrawFilledCircle(Color.FromName(color), int.Parse(x), int.Parse(y), int.Parse(rad));
                        } // КРУГ
                        else if (lin.StartsWith("pixel ", true, null))
                        {
                            lin = lin.Remove(0, 6);
                            string[] parts = lin.Split("||");
                            string x = parts[0].Trim();
                            string y = parts[1].Trim();
                            string color = parts[2].Trim();
                            foreach (var item in parts)
                            {
                                if (item.StartsWith("&&"))
                                {
                                    string ikalka = item.Substring(2).Trim();
                                    foreach (var var in variables)
                                    {
                                        string[] varParts = var.Split(':');
                                        if (varParts[0] == ikalka)
                                        {
                                            string value = varParts[1];
                                            if (parts[0].Substring(2) == ikalka)
                                            {
                                                x = value;
                                            }
                                            if (parts[1].Substring(2) == ikalka)
                                            {
                                                y = value;
                                            }
                                            if (parts[2].Substring(2) == ikalka)
                                            {
                                                color = value;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            Kernel.canvas.DrawPoint(Color.FromName(color), int.Parse(x), int.Parse(y));
                        } // ПИКСЕЛЬ
                        #endregion
                        #region system
                        else if (lin == "shutdown")
                        {
                            sus.Power.Shutdown();
                        } // ВЫКЛЮЧЕНИЕ ПК
                        else if (lin == "reboot")
                        {
                            sus.Power.Reboot();
                        } // ПЕРЕЗАПУСК ПК
                        else if (lin == "stop")
                        {
                            Kernel.isExecute = false;
                        } // ОСТАНОВИТЬ КОД
                        else if (lin.StartsWith("beep ", true, null))
                        {
                            lin = lin.Remove(0, 5);
                            int hz = int.Parse(lin.Split("||")[0].Trim());
                            int sec = int.Parse(lin.Split("||")[1].Trim());
                            System.Console.Beep(hz, sec);
                        } // СПИКЕР ПК
                        #endregion
                        // ДОДЕЛАТЬ: ПРОВЕРКУ КЛИКА ПО КООРДИНАТАМ
                        #region code
                        else if (lin.StartsWith("var ", true, null))
                        {
                            lin = lin.Remove(0, 4);
                            variables.Add(lin.Trim());
                        } // СОЗДАНИЕ ПЕРЕМЕННОЙ
                        else if (lin.StartsWith("set ", true, null))
                        {
                            lin = lin.Remove(0, 4);
                            string name = lin.Split("||")[0].Trim();
                            string value = lin.Split("||")[1].Trim();
                            foreach (var item in variables)
                            {
                                if (item.Split(':')[0] == name)
                                {
                                    variables.Remove(item);
                                    variables.Add(item + ':' + value);
                                    break;
                                }
                            }
                        } // ЗАДАВАНИЕ ЗНАЧЕНИЕ ПЕРЕМЕННОЙ
                        else if (lin.StartsWith("if ", true, null) && inIf == false)
                        {
                            lin = lin.Remove(0, 3);
                            string name = lin.Split(" ")[0].Trim();
                            string op = lin.Split(" ")[1].Trim();
                            string comp = lin.Split(" ")[2].Trim();
                            foreach (var item in variables)
                            {
                                if (item.Split(':')[0] == name)
                                {
                                    if (op == ">" || op == "<" || op == "==") 
                                    {
                                        inIf = true;
                                        if (op == ">")
                                        {
                                            if (int.Parse(item.Split(':')[1]) > int.Parse(lin.Split(" ")[2]))
                                            {
                                                IfTrue = true;
                                            }
                                            else { IfTrue = false; }
                                        } // Int
                                        else if (op == "<")
                                        {
                                            if (int.Parse(item.Split(':')[1]) < int.Parse(lin.Split(" ")[2]))
                                            {
                                                IfTrue = true;
                                            }
                                            else { IfTrue = false; }
                                        } // Int
                                        else if (op == "==")
                                        {
                                            try 
                                            {
                                                if (int.Parse(item.Split(':')[1]) == int.Parse(lin.Split(" ")[2]))
                                                {
                                                    IfTrue = true;
                                                }
                                                else { IfTrue = false; }
                                            }
                                            catch(Exception e)
                                            {
                                                if (item.Split(':')[1] == lin.Split(" ")[2])
                                                {
                                                    IfTrue = true;
                                                }
                                                else { IfTrue = false; }
                                            }
                                        } // String или Int
                                    }
                                    else
                                    {
                                        FpsShower.Msg("Error in line " + linee, "Wrong IF operator!", false);
                                        FpsShower.playSound = true;
                                        Kernel.isExecute = false;
                                    }
                                }
                            }
                        } // УСЛОВИЕ IF
                        else if (lin.StartsWith("if ", true, null) && inIf == true)
                        {
                            FpsShower.Msg("Error in line " + linee, "If in If not implemented!", false);
                            FpsShower.playSound = true;
                            Kernel.isExecute = false;
                        } // ЕСЛИ IF В IF'Е
                        #endregion
                        // ДОДЕЛАТЬ: УСЛОВИЯ IF И BREAK КОНЕЦ УСЛОВИЯ
                        #region Maths
                        else if (lin.StartsWith("plus ", true, null))
                        {
                            int oldval = 0;
                            lin = lin.Substring(5);
                            string var = lin.Split("||")[0].Trim();
                            string value = lin.Split("||")[1].Trim();
                            if (lin.Split("||")[1].StartsWith("&&"))
                            {
                                string ikalka = lin.Split("||")[1].Substring(2).Trim();
                                foreach (var vari in variables)
                                {
                                    string[] varParts = vari.Split(':');
                                    if (varParts[0] == ikalka)
                                    {
                                        value = varParts[1];
                                        break;
                                    }
                                }
                            }
                            foreach (var item in variables)
                            {
                                if (item.Split(':')[0] == var)
                                {
                                    variables.Remove(item);
                                    oldval = int.Parse(item.Split(':')[1]);
                                    variables.Add(var + ':' + (oldval + int.Parse(value)).ToString());
                                    break;
                                }
                            }
                        } // ПЛЮС
                        else if (lin.StartsWith("minus ", true, null))
                        {
                            int oldval = 0;
                            lin = lin.Substring(6);
                            string var = lin.Split("||")[0].Trim();
                            string value = lin.Split("||")[1].Trim();
                            if (lin.Split("||")[1].StartsWith("&&"))
                            {
                                string ikalka = lin.Split("||")[1].Substring(2).Trim();
                                foreach (var vari in variables)
                                {
                                    string[] varParts = vari.Split(':');
                                    if (varParts[0] == ikalka)
                                    {
                                        value = varParts[1];
                                        break;
                                    }
                                }
                            }
                            foreach (var item in variables)
                            {
                                if (item.Split(':')[0] == var)
                                {
                                    variables.Remove(item);
                                    oldval = int.Parse(item.Split(':')[1]);
                                    variables.Add(var + ':' + (oldval - int.Parse(value)).ToString());
                                    break;
                                }
                            }
                        } // МИНУС
                        else if (lin.StartsWith("multiply ", true, null))
                        {
                            int oldval = 0;
                            lin = lin.Substring(9);
                            string var = lin.Split("||")[0].Trim();
                            string value = lin.Split("||")[1].Trim();
                            if (lin.Split("||")[1].StartsWith("&&"))
                            {
                                string ikalka = lin.Split("||")[1].Substring(2).Trim();
                                foreach (var vari in variables)
                                {
                                    string[] varParts = vari.Split(':');
                                    if (varParts[0] == ikalka)
                                    {
                                        value = varParts[1];
                                        break;
                                    }
                                }
                            }
                            foreach (var item in variables)
                            {
                                if (item.Split(':')[0] == var)
                                {
                                    variables.Remove(item);
                                    oldval = int.Parse(item.Split(':')[1]);
                                    variables.Add(var + ':' + (oldval * int.Parse(value)).ToString());
                                    break;
                                }
                            }
                        } // УМНОЖЕНИЕ
                        else if (lin.StartsWith("divide ", true, null))
                        {
                            int oldval = 0;
                            lin = lin.Substring(7);
                            string var = lin.Split("||")[0].Trim();
                            string value = lin.Split("||")[1].Trim();
                            if (lin.Split("||")[1].StartsWith("&&"))
                            {
                                string ikalka = lin.Split("||")[1].Substring(2).Trim();
                                foreach (var vari in variables)
                                {
                                    string[] varParts = vari.Split(':');
                                    if (varParts[0] == ikalka)
                                    {
                                        value = varParts[1];
                                        break;
                                    }
                                }
                            }
                            foreach (var item in variables)
                            {
                                if (item.Split(':')[0] == var)
                                {
                                    variables.Remove(item);
                                    oldval = int.Parse(item.Split(':')[1]);
                                    variables.Add(var + ':' + (oldval / int.Parse(value)).ToString());
                                    break;
                                }
                            }
                        } // ДЕЛЕНИЕ
                        else if (lin.StartsWith("power ", true, null))
                        {
                            int oldval = 0;
                            lin = lin.Substring(6);
                            string var = lin.Split("||")[0].Trim();
                            string value = lin.Split("||")[1].Trim();
                            if (lin.Split("||")[1].StartsWith("&&"))
                            {
                                string ikalka = lin.Split("||")[1].Substring(2).Trim();
                                foreach (var vari in variables)
                                {
                                    string[] varParts = vari.Split(':');
                                    if (varParts[0] == ikalka)
                                    {
                                        value = varParts[1];
                                        break;
                                    }
                                }
                            }
                            foreach (var item in variables)
                            {
                                if (item.Split(':')[0] == var)
                                {
                                    variables.Remove(item);
                                    oldval = int.Parse(item.Split(':')[1]);
                                    variables.Add(var + ':' + Math.Pow(oldval, int.Parse(value)).ToString());
                                    break;
                                }
                            }
                        } // СТЕПЕНЬ
                        else if (lin.StartsWith("sqrt ", true, null))
                        {
                            int oldval = 0;
                            lin = lin.Substring(5);
                            foreach (var item in variables)
                            {
                                if (item.Split(':')[0] == lin)
                                {
                                    variables.Remove(item);
                                    oldval = int.Parse(item.Split(':')[1]);
                                    variables.Add(lin + ':' + Math.Sqrt(oldval).ToString());
                                    break;
                                }
                            }
                        } // КОРЕНЬ
                        else if (lin.StartsWith("endcond", true, null) || lin.StartsWith("else", true, null)) { }
                        else if (lin.StartsWith("", true, null)) { }
                        #endregion
                        else
                        {
                            FpsShower.Msg("Error in line " + linee, "Invalid Command!", false);
                            FpsShower.playSound = true;
                            Kernel.isExecute = false;
                        }
                    }
                    linee += 1;
                }
                inIf = false;
                inElse = false;
                IfTrue = false;
                linee = 1;
                variables.Clear();
            }
            catch (Exception e)
            {
                FpsShower.Msg("Error in line " + linee, e.ToString(), false);
                FpsShower.playSound = true;
                Kernel.isExecute = false;
            }
        }
    }
}
