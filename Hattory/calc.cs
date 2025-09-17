using Cosmos.System.Graphics;
using System;
using System.Drawing;
using kern = Hattory.Kernel;
//calc
namespace Hattory
{
    public class Calc
    {
        private static Canvas canvas = kern.canvas;
        public static bool iscalcready = false;
        public static string first = kern.first;
        public static string second = kern.second;
        public static float firstI = kern.firstI;
        public static float secondI = kern.secondI;
        public static float c = kern.c;

        //ыаыаыаыаыаыаыаы

        public static void Calculator()
        {
            try
            {
                canvas.DrawFilledRectangle(Color.FromArgb(128, 0, 128), 25, 60, 225, 325);
                canvas.DrawFilledRectangle(Color.FromArgb(45, 0, 45), 27, 62, 221, 321);
                canvas.DrawFilledRectangle(Color.FromArgb(75, 0, 130), 27, 62, 221, 25);
                Otrisovka.Write("Calculator", 35, 67, Color.Lavender);

                // Кнопка закрытия
                canvas.DrawFilledRectangle(Color.FromArgb(220, 20, 60), 25 + 225 - 22, 60 + 4, 18, 18);
                Otrisovka.Write("X", 25 + 225 - 17, 60 + 7, Color.White);
                //cifri buttons - 1
                canvas.DrawFilledRectangle(Color.Gray, 35, 190, 40, 40);
                canvas.DrawFilledRectangle(Color.Gray, 85, 190, 40, 40);
                canvas.DrawFilledRectangle(Color.Gray, 135, 190, 40, 40);
                canvas.DrawFilledRectangle(Color.Gray, 185, 190, 40, 40);
                Otrisovka.Write("1", 55, 205, Color.Black);
                Otrisovka.Write("2", 105, 205, Color.Black);
                Otrisovka.Write("3", 155, 205, Color.Black);
                Otrisovka.Write("BS", 199, 205, Color.Black);
                //2
                canvas.DrawFilledRectangle(Color.Gray, 35, 240, 40, 40);
                canvas.DrawFilledRectangle(Color.Gray, 85, 240, 40, 40);
                canvas.DrawFilledRectangle(Color.Gray, 135, 240, 40, 40);
                canvas.DrawFilledRectangle(Color.Gray, 185, 240, 40, 40);
                Otrisovka.Write("4", 55, 255, Color.Black);
                Otrisovka.Write("5", 105, 255, Color.Black);
                Otrisovka.Write("6", 155, 255, Color.Black);
                Otrisovka.Write("sqrt", 189, 255, Color.Black);
                //3
                canvas.DrawFilledRectangle(Color.Gray, 35, 290, 40, 40);
                canvas.DrawFilledRectangle(Color.Gray, 85, 290, 40, 40);
                canvas.DrawFilledRectangle(Color.Gray, 135, 290, 40, 40);
                canvas.DrawFilledRectangle(Color.Gray, 185, 290, 40, 40);
                Otrisovka.Write("7", 55, 305, Color.Black);
                Otrisovka.Write("8", 105, 305, Color.Black);
                Otrisovka.Write("9", 155, 305, Color.Black);
                Otrisovka.Write("sqr", 193, 305, Color.Black);
                //0
                canvas.DrawFilledRectangle(Color.Gray, 35, 340, 40, 40);
                Otrisovka.Write("-", 55, 355, Color.Black);
                canvas.DrawFilledRectangle(Color.Gray, 85, 340, 40, 40);
                Otrisovka.Write("0", 105, 355, Color.Black);
                canvas.DrawFilledRectangle(Color.Gray, 135, 340, 40, 40);
                Otrisovka.Write(".", 155, 355, Color.Black);
                canvas.DrawFilledRectangle(Color.Gray, 185, 340, 40, 40);
                Otrisovka.Write("x^y", 193, 355, Color.Black);
                //end
                //button press
                //1
                if (kern.Click(35, 190, 40, 40))
                {
                    if (iscalcready == false)
                    {
                        if (first.Length < 22)
                        {
                            first += "1";
                        }
                    }
                    else
                    {
                        if (second.Length < 22)
                        {
                            second += "1";
                        }
                    }
                }
                //2
                if (kern.Click(85, 190, 40, 40))
                {
                    if (iscalcready == false)
                    {
                        if (first.Length < 22)
                        {
                            first += "2";
                        }
                    }
                    else
                    {
                        if (second.Length < 22)
                        {
                            second += "2";
                        }
                    }
                }
                //3
                if (kern.Click(135, 190, 40, 40))
                {
                    if (iscalcready == false)
                    {
                        if (first.Length < 22)
                        {
                            first += "3";
                        }
                    }
                    else
                    {
                        if (second.Length < 22)
                        {
                            second += "3";
                        }
                    }
                }
                //4
                if (kern.Click(35, 240, 40, 40))
                {
                    if (iscalcready == false)
                    {
                        if (first.Length < 22)
                        {
                            first += "4";
                        }
                    }
                    else
                    {
                        if (second.Length < 22)
                        {
                            second += "4";
                        }
                    }
                }
                //5
                if (kern.Click(85, 240, 40, 40))
                {
                    if (iscalcready == false)
                    {
                        if (first.Length < 22)
                        {
                            first += "5";
                        }
                    }
                    else
                    {
                        if (second.Length < 22)
                        {
                            second += "5";
                        }
                    }
                }
                //6
                if (kern.Click(135, 240, 40, 40))
                {
                    if (iscalcready == false)
                    {
                        if (first.Length < 22)
                        {
                            first += "6";
                        }
                    }
                    else
                    {
                        if (second.Length < 22)
                        {
                            second += "6";
                        }
                    }
                }
                //7
                if (kern.Click(35, 290, 40, 40))
                {
                    if (iscalcready == false)
                    {
                        if (first.Length < 22)
                        {
                            first += "7";
                        }
                    }
                    else
                    {
                        if (second.Length < 22)
                        {
                            second += "7";
                        }
                    }
                }
                //8
                if (kern.Click(85, 290, 40, 40))
                {
                    if (iscalcready == false)
                    {
                        if (first.Length < 22)
                        {
                            first += "8";
                        }
                    }
                    else
                    {
                        if (second.Length < 22)
                        {
                            second += "8";
                        }
                    }
                }
                //9
                if (kern.Click(135, 290, 40, 40))
                {
                    if (iscalcready == false)
                    {
                        if (first.Length < 22)
                        {
                            first += "9";
                        }
                    }
                    else
                    {
                        if (second.Length < 22)
                        {
                            second += "9";
                        }
                    }
                }
                //0
                if (kern.Click(85, 340, 40, 40))
                {
                    if (iscalcready == false)
                    {
                        if (first.Length < 22)
                        {
                            first += "0";
                        }
                    }
                    else
                    {
                        if (second.Length < 22)
                        {
                            second += "0";
                        }
                    }
                }
                if (kern.Click(35, 340, 40, 40))
                {
                    if (iscalcready == false)
                    {
                        if (first.Length < 1)
                        {
                            first += "-";
                        }
                    }
                    else
                    {
                        if (second.Length < 1)
                        {
                            second += "-";
                        }
                    }
                }
                if (kern.Click(185, 190, 40, 40))
                {
                    if (iscalcready == false)
                    {
                        if (first.Length >= 1)
                        {
                            first = first.Remove(first.Length - 1, 1);
                        }
                    }
                    else
                    {
                        if (second.Length >= 1)
                        {
                            second = second.Remove(second.Length - 1, 1);
                        }

                    }
                }
                //.
                if (kern.Click(135, 340, 40, 40))
                {
                    if (iscalcready == false)
                    {
                        if (first.Length < 22)
                        {
                            first += ".";
                        }
                    }
                    else
                    {
                        if (second.Length < 22)
                        {
                            second += ".";
                        }
                    }
                }
                //line 1
                if (kern.Click(35, 100, 190, 15))
                {
                    iscalcready = false;
                }
                //line 2
                if (kern.Click(35, 135, 190, 15))
                {
                    iscalcready = true;
                }
                //end
                if (iscalcready == false)
                {
                    canvas.DrawFilledRectangle(Color.Green, 35, 100, 190, 15);
                    canvas.DrawFilledRectangle(Color.White, 35, 135, 190, 15);
                }
                if (iscalcready == true)
                {
                    canvas.DrawFilledRectangle(Color.White, 35, 100, 190, 15);
                    canvas.DrawFilledRectangle(Color.Green, 35, 135, 190, 15);
                }
                canvas.DrawFilledRectangle(Color.White, 35, 165, 190, 15);
                Otrisovka.Write(first, 40, 100, Color.Black);
                Otrisovka.Write(second, 40, 135, Color.Black);
                try
                {
                    Otrisovka.Write(c.ToString(), 40, 165, Color.Black);
                }
                catch (Exception) { }
                Otrisovka.Write("Plus", 30, 120, Color.Thistle); // 32
                Otrisovka.Write("Minus", 70, 120, Color.Thistle); // 40 + 5
                Otrisovka.Write("Devide", 120, 120, Color.Thistle); // 48 + 5
                Otrisovka.Write("Multiply", 175, 120, Color.Thistle); //64 + 5
                try
                {
                    firstI = Convert.ToSingle(first);
                    secondI = Convert.ToSingle(second);
                }
                catch (Exception) { }
                if (kern.Click(30, 120, 32, 16))
                {
                    c = firstI + secondI;
                }
                if (kern.Click(70, 120, 40, 16))
                {
                    c = firstI - secondI;
                }
                if (kern.Click(120, 120, 48, 16))
                {
                    c = firstI / secondI;
                }
                if (kern.Click(175, 120, 64, 16))
                {
                    c = firstI * secondI;
                }
                //SQRT()
                if (kern.Click(185, 240, 40, 40))
                {
                    c = !iscalcready ? (float)Math.Sqrt(firstI) : (float)Math.Sqrt(secondI);
                }
                //SQR()
                if (kern.Click(185, 290, 40, 40))
                {
                    c = !iscalcready ? (float)Math.Pow(firstI,2) : (float)Math.Pow(secondI,2);
                }
                //pow()
                if (kern.Click(185, 340, 40, 40))
                {
                    c = (float)Math.Pow(firstI, secondI);
                }
                if (kern.Click(25 + 225 - 22, 60 + 4, 18, 18))
                {
                    c = kern.c;
                    first = kern.first;
                    second = kern.second;
                    firstI = kern.firstI;
                    secondI = kern.secondI;
                    iscalcready = false;
                    Kernel.op = "";
                }
                Cosmos.Core.Memory.Heap.Collect();
            }
            catch (Exception)
            {
                Otrisovka.Write("ERROR 1000-7", 30, 145, Color.Black);
            }
        }
    }
}