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

namespace Hattory
{
    public class PragmaCode
    {
        static public int nowstroka = 1; // текущая строка
        static public string stroka = ""; // текущая строка (стринг)
        static public int earlystroka; // номер прошлых строк
        static public int earlyyy; // координата y прошлых строк
        static public int yy = 90; // координата y
        static public List<string> code = new List<string>(); // весь код в листе
        static public void Pragma()
        {
            lol.canvas.DrawFilledRectangle(Color.FromArgb(75, 75, 75), 30, 60, 550, 500);
            Otrisovka.Write("Visual Stupidio IDE: Tab - save, F1 - run, Esc - exit", 70, 70, Color.SpringGreen);
            earlyyy = 90;
            earlystroka = 1;
            foreach (string s in code)
            {
                if(nowstroka > 1) {
                    Otrisovka.Write(earlystroka + " " + s, 35, earlyyy, Color.White);
                    earlystroka++;
                    earlyyy += 16;
                }
            } // отрисовка прошлых строк
            Otrisovka.Write(nowstroka + " " + stroka, 35, yy, Color.MediumSpringGreen); // отрисовка строки
            try
            {
                sus.KeyEvent k;
                bool IsKeyPressed = sus.KeyboardManager.TryReadKey(out k);
                klavaypr.YPRklava(k);
                if (k.Key == ConsoleKeyy.Spacebar)
                {
                    stroka += " ";
                } // Пробел
                else if (k.Key == ConsoleKeyy.Backspace)
                {
                    if (stroka.Length > 0) { stroka = stroka.Remove(stroka.Length - 1); }
                } // Бек спейс
                else if (k.Key == ConsoleKeyy.F1) 
                {
                    PragmaInterpritator.code = code;
                    PragmaInterpritator.Execute(); // передача кода интепритатору
                    Kernel.isExecute = true;
                    klavaypr.On = true;
                    Kernel.isPragma = false;
                } // RUN CODE
                else if (k.Key == ConsoleKeyy.Tab)
                {
                    try
                    {
                        string goodpart = "";
                        string totalcode = "";
                        string filenam;
                        foreach (var partition in Kernel.fs.Disks[Kernel.globaldskcnt].Partitions)
                        {
                            if (partition.RootPath == "" || partition.RootPath == null) { }
                            else
                            {
                                goodpart = partition.RootPath;
                            }
                        }
                        foreach (string s in code)
                        {
                            totalcode += s;
                            totalcode += "#";
                        }
                        totalcode = totalcode.TrimEnd('#');
                        if (goodpart != "")
                        {
                            for (int i = 0; ; i++)
                            {
                                filenam = "MyProgram" + i.ToString();
                                if (!Directory.Exists(goodpart + "PragmaCode")) { Directory.CreateDirectory(goodpart + "PragmaCode"); }
                                if (!File.Exists(goodpart + @"PragmaCode\" + filenam + ".pcf"))
                                {
                                    File.Create(goodpart + @"PragmaCode\" + filenam + ".pcf");
                                    File.WriteAllText(goodpart + @"PragmaCode\" + filenam + ".pcf", totalcode);
                                    FpsShower.Msg("Successfully saved!");
                                    FpsShower.playSound = true;
                                    break;
                                }
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        FpsShower.Msg("Saving error!", e.ToString(), false);
                        FpsShower.playSound = true;
                    }
                    
                } // сохрание кода
                else if (k.Key == ConsoleKeyy.Escape) 
                {
                    klavaypr.On = true;
                    lol.isPragma = false;
                } // выход из прагмыКода
                else if (k.Key == ConsoleKeyy.Enter)
                {
                    code.Add(stroka);
                    yy += 16;
                    nowstroka++;
                    stroka = "";
                } // Отступ
                /*else if (k.Key == ConsoleKeyy.UpArrow)
                {
                    if (nowstroka > 1)
                    {
                        yy -= 16;
                        nowstroka--;
                        index = code.IndexOf(code[nowstroka]);
                        stroka = code[nowstroka];
                        code.Remove(code[nowstroka]);
                    }
                } // Вверх
                else if (k.Key == ConsoleKeyy.DownArrow)
                {
                    if (code[nowstroka+=1] != null)
                    {
                        yy -= 16;
                        nowstroka--;
                        stroka = code[nowstroka];
                    }
                } // Вниз*/
                else
                {
                    if (!char.IsControl(k.KeyChar) && k.KeyChar != ' ' && char.IsAscii(k.KeyChar))
                    {
                        stroka += k.KeyChar;
                    }
                } // Отклонение неизвестных символов
            }
            catch (Exception) { }
        }
        public static void Load(string text)
        {
            nowstroka = 1;
            stroka = ""; // текущая строка (стринг)
            yy = 90; // координата y
            code.Clear();
            string[] lines = text.Split('#');
            foreach (string line in lines)
            {
                yy += 16;
                code.Add(line);
                nowstroka++;
                stroka = "";
            }
        }
    }   
}