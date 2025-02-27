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
        public static void Execute()
        {
            try
            {
                foreach (string line in code)
                {
                    string lin = line;
                    while (lin.StartsWith(' ') || lin.EndsWith(' ')) { lin = lin.Trim(); }
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
                    if (lin.StartsWith("rect ", true, null))
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
                    if (lin.StartsWith("circle ", true, null))
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
                    if (lin.StartsWith("pixel ", true, null))
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
                    if (lin == "shutdown")
                    {
                        sus.Power.Shutdown();
                    } // ВЫКЛЮЧЕНИЕ ПК
                    if (lin == "reboot")
                    {
                        sus.Power.Reboot();
                    } // ПЕРЕЗАПУСК ПК
                    if (lin == "stop") 
                    {
                        Kernel.isExecute = false;
                    } // ОСТАНОВИТЬ КОД
                    if (lin.StartsWith("beep ", true, null))
                    {
                        lin = lin.Remove(0, 5);
                        int hz = int.Parse(lin.Split("||")[0].Trim());
                        int sec = int.Parse(lin.Split("||")[1].Trim());
                        System.Console.Beep(hz, sec);
                    } // СПИКЕР ПК
                    #endregion
                    // ДОДЕЛАТЬ: ПРОВЕРКУ КЛИКА ПО КООРДИНАТАМ
                    #region code
                    if (lin.StartsWith("var ", true, null))
                    {
                        lin = lin.Remove(0, 4);
                        variables.Add(lin.Trim());
                    } // СОЗДАНИЕ ПЕРЕМЕННОЙ
                    if (lin.StartsWith("set ", true, null))
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
                    #endregion
                    // ДОДЕЛАТЬ: УСЛОВИЯ IF И BREAK КОНЕЦ УСЛОВИЯ
                    #region Maths
                    if (lin.StartsWith("plus ", true, null))
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
                    if (lin.StartsWith("minus ", true, null))
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
                    if (lin.StartsWith("multiply ", true, null))
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
                    if (lin.StartsWith("divide ", true, null))
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
                    if (lin.StartsWith("power ", true, null))
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
                    if (lin.StartsWith("sqrt ", true, null))
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
                    #endregion
                }
                variables.Clear();
            }
            catch (Exception e)
            {
                Otrisovka.Write("Error in code: " + e, 20, 700, Color.Red);
            }
        }
    }
}
