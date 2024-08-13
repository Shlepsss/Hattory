﻿using Cosmos.System.Graphics;
using System;
using System.Drawing;
using System.IO;
using ConsoleKeyy = Cosmos.System.ConsoleKeyEx;
using sus = Cosmos.System;
#pragma warning disable CA1416 // Проверка совместимости платформы

namespace Hattory
{
    internal class Notepad
    {
        public static Canvas canvas = Hattory.Kernel.canvas;
        public static string path = @"0:\";
        public static string fileeepath = "";
        public static string txta = "";
        public static string filename = "test.txt";
        public static byte[] bite;
        public static sus.Graphics.Bitmap image;
        public static int iznyy;
        public static int iznai;
        public static string filenameeee;
        public static bool isended = true;
        public static bool isendedbmp = true;
        public static bool islock = false;

        public static void enternote()
        {
            canvas.DrawFilledRectangle(Color.Wheat, 10, 60, 270 + (path.Length * 2), 250);
            Otrisovka.Write("File Manager: " + path, 30, 70, Color.Black);
            try
            {
                var files_list = Directory.GetFiles(path);
                var directory_list = Directory.GetDirectories(path);
                //Otrisovka.Write(string.Join('`',directory_list), 15, 90, Color.Black, true);
                //Otrisovka.Write(string.Join('`',files_list), 110, 90, Color.Black, true);
                if (Hattory.Kernel.Click(30, 70, 112, 10))
                {
                    //path = @"0:\" ;
                    path = Hattory.Kernel.globaldskcnt.ToString() + @":\";
                }
                iznyy = 90;
                iznai = 90;
                //directories
                foreach (string dir in directory_list)
                {
                    canvas.DrawFilledRectangle(Color.Gold, 15, iznyy, dir.Length * 8, 15);
                    Otrisovka.Write(dir, 15, iznyy, Color.Black);
                    if (Hattory.Kernel.Click(15, iznyy, dir.Length * 8, 16))
                    {
                        path = path + dir + @"\";
                    }
                    iznyy += 16;
                }
                //files
                foreach (string pathhh in files_list)
                {
                    canvas.DrawFilledRectangle(Color.Gold, 150, iznai, pathhh.Length * 8, 15);
                    Otrisovka.Write(pathhh, 150, iznai, Color.Black);
                    //OPEN FILE TXT | BMP
                    if (Hattory.Kernel.Click(150, iznai, pathhh.Length * 8, 15))
                    {
                        try
                        {
                            if (pathhh.EndsWith(".txt") || pathhh.EndsWith(".Txt") || pathhh.EndsWith(".tXt") || pathhh.EndsWith(".txT") || pathhh.EndsWith(".TXt") || pathhh.EndsWith(".tXT") || pathhh.EndsWith(".TxT") || pathhh.EndsWith(".TXT"))
                            {
                                txta = File.ReadAllText(path + @"\" + pathhh);
                                @fileeepath = pathhh;
                                isended = false;
                            }
                            if (pathhh.EndsWith(".cfi") || pathhh.EndsWith(".Cfi") || pathhh.EndsWith(".cFi") || pathhh.EndsWith(".cfI") || pathhh.EndsWith(".CFi") || pathhh.EndsWith(".cFI") || pathhh.EndsWith(".CfI") || pathhh.EndsWith(".CFI"))
                            {
                                txta = File.ReadAllText(path + @"\" + pathhh);
                                Colorfull.Loadd(txta);
                                Kernel.isColorfull = true;
                                Kernel.isfilemanager = false;
                            }
                            if (pathhh.EndsWith(".bmp") || pathhh.EndsWith(".Bmp") || pathhh.EndsWith(".bMp") || pathhh.EndsWith(".bmP") || pathhh.EndsWith(".BMp") || pathhh.EndsWith(".bMP") || pathhh.EndsWith(".BmP") || pathhh.EndsWith(".BMP"))
                            {
                                try
                                {
                                    bite = File.ReadAllBytes(path + @"\" + pathhh);
                                    image = new sus.Graphics.Bitmap(bite);
                                    if (image.Width <= 1100 && image.Height <= 700 && isendedbmp == true)
                                    {
                                        isendedbmp = false;
                                    }
                                }
                                catch (Exception) { }
                            }
                        }
                        catch (Exception) { }
                    }
                    //DELETE
                    else if (Hattory.Kernel.ClickMiddle(150, iznai, pathhh.Length * 8, 15) && islock == false)
                    {
                        try
                        {
                            islock = true;
                            filenameeee = pathhh;
                            Hattory.Kernel.isformatsure = true;
                        }
                        catch (Exception) { }
                    }
                    iznai += 16;
                }
                //RENAME
                if (Hattory.Kernel.ClickMiddle(150, 90, 270 + (path.Length * 2), 220) && islock == false)
                {
                    islock = true;
                    klavaypr.On = false;
                    Hattory.Kernel.isfilerename = true;
                }
                if (isended == false)
                {
                    entname(fileeepath);
                    klavaypr.On = false;
                }
                if (isendedbmp == false)
                {
                    canvas.DrawImageAlpha(image, 350, 60);
                    if (Hattory.Kernel.ClickRight(350, 60, (int)image.Width, (int)image.Height))
                    {
                        isendedbmp = true;
                    }
                }
                Cosmos.Core.Memory.Heap.Collect();
            }
            catch (Exception) { }
        }
        public static void entname(string pathFile)
        {
            try
            {
                sus.KeyEvent k;
                bool IsKeyPressed = sus.KeyboardManager.TryReadKey(out k);
                Otrisovka.Write("Redact " + pathFile + ":", 10, 330, Color.Red);
                Otrisovka.Write(txta, 10, 350, Color.White);
                klavaypr.YPRklava(k);
                if (k.Key == ConsoleKeyy.Spacebar)
                {
                    txta += " ";
                }
                else if (k.Key == ConsoleKeyy.Backspace)
                {
                    if (txta.Length >= 1) { txta = txta.Remove(txta.Length - 1); }
                }
                else if (k.Key == ConsoleKeyy.Escape)
                {
                    klavaypr.On = true;
                    txta = "";
                    isended = true;
                }
                else if (k.Key == ConsoleKeyy.Enter)
                {
                    try
                    {
                        File.WriteAllText(path + @"\" + @pathFile, txta);
                        txta = "";
                        klavaypr.On = true;
                        isended = true;
                    }
                    catch (Exception)
                    {
                        sus.Power.Shutdown();
                    }
                }
                else
                {
                    if (char.IsAscii(k.KeyChar) && k.KeyChar != ' ')
                    {
                        txta += k.KeyChar;
                    }
                }
            }
            catch (Exception) { }
        }
    }
}