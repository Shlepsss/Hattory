using Cosmos.Core;
using System.Drawing;
using ConsoleKeyy = Cosmos.System.ConsoleKeyEx;
using sus = Cosmos.System;
#pragma warning disable CA1416 // Проверка совместимости платформы

namespace Hattory
{
    internal class klavaypr
    {
        public static bool On = true;
        public static string tt = "";
        public static bool iscorrectletter;
        public static void YPRklava(sus.KeyEvent k, bool Control = true)
        {
            /*if (k.Key == ConsoleKeyy.Spacebar)
            {
                tt += " ";
            }
            else if (k.Key == ConsoleKeyy.Backspace)
            {
                Notepad.ntepad.filename.Remove(Notepad.ntepad.filename.Length - 1);
            }
            else if (k.Key == ConsoleKeyy.Escape)
            {
                tt = "";
            }
            else if (k.Key == ConsoleKeyy.Enter)
            {
                try
                {
                    File.WriteAllText(@path + @"\" + @pathFile, txta);
                    txta = "";
                    isended = true;
                }
                catch (Exception e)
                {
                    sus.Power.Shutdown();
                }
            }*/
            if (Control)
            {
                if (k.Key == ConsoleKeyy.Minus) { sus.MouseManager.MouseState = sus.MouseState.Left; }
                else if (k.Key == ConsoleKeyy.Equal) { sus.MouseManager.MouseState = sus.MouseState.Right; }
                else if (k.Key == ConsoleKeyy.UpArrow)
                {
                    sus.MouseManager.Y -= 7;
                }
                else if (k.Key == ConsoleKeyy.DownArrow)
                {
                    sus.MouseManager.Y += 7;
                }
                else if (k.Key == ConsoleKeyy.RightArrow)
                {
                    sus.MouseManager.X += 7;
                }
                else if (k.Key == ConsoleKeyy.LeftArrow)
                {
                    sus.MouseManager.X -= 7;
                }
            }
            
            else if (k.Key == ConsoleKeyy.F2)
            {
                Kernel.Render = false;
            }
            else if (k.Key == ConsoleKeyy.F3)
            {
                Kernel.Render = true;
            }
        }
        public static void control()
        {
            if (On)
            {
                sus.KeyEvent k;
                bool IsKeyPressed = sus.KeyboardManager.TryReadKey(out k);
                //if (!char.IsAscii(k.KeyChar)) { sus.MouseManager.MouseState = sus.MouseState.None; }
                if (k.Key == ConsoleKeyy.Minus) { sus.MouseManager.MouseState = sus.MouseState.Left; sus.MouseManager.MouseState = sus.MouseState.None; }
                else if (k.Key == ConsoleKeyy.Equal) { sus.MouseManager.MouseState = sus.MouseState.Right; sus.MouseManager.MouseState = sus.MouseState.None; }
                else if (k.Key == ConsoleKeyy.UpArrow)
                {
                    sus.MouseManager.Y -= 7;
                }
                else if (k.Key == ConsoleKeyy.DownArrow)
                {
                    sus.MouseManager.Y += 7;
                }
                else if (k.Key == ConsoleKeyy.RightArrow)
                {
                    sus.MouseManager.X += 7;
                }
                else if (k.Key == ConsoleKeyy.LeftArrow)
                {
                    sus.MouseManager.X -= 7;
                }
                else if (k.Key == ConsoleKeyy.F2)
                {
                    Kernel.Render = false;
                }
                else if (k.Key == ConsoleKeyy.F3)
                {
                    Kernel.Render = true;
                }
            }
        }
    }
}
