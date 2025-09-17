using System;
using System.Collections.Generic;
using System.Drawing;
using Cosmos;
using System.Linq;
using System.Text;
using System.IO;
using sus = Cosmos.System;
using ConsoleKeyy = Cosmos.System.ConsoleKeyEx;
using lol = Hattory.Kernel;
using Cosmos.System.Graphics;

namespace Hattory
{
    public class PragmaCode
    {
        public static int CurrentLine = 1;
        public static string CurrentLineText = "";
        public static int PreviousLinesCount = 1;
        public static int PreviousLinesY = 90;
        public static List<string> Code = new List<string>();
        public static int ScrollOffset = 0;
        public static int CursorColumn = 0;

        public static void Pragma()
        {
            RenderIDE();
            HandleInput();
        }

        private static void RenderIDE()
        {
            lol.canvas.DrawFilledRectangle(Color.FromArgb(128, 0, 128), 30, 60, 625, 500);
            lol.canvas.DrawFilledRectangle(Color.FromArgb(45, 0, 45), 32, 62, 621, 496);
            lol.canvas.DrawFilledRectangle(Color.FromArgb(75, 0, 130), 32, 62, 621, 25);
            Otrisovka.Write("Visual Stupidio IDE", 40, 67, Color.SpringGreen);

            // Кнопка закрытия
            lol.canvas.DrawFilledRectangle(Color.FromArgb(220, 20, 60), 30 + 625 - 22, 60 + 4, 18, 18);
            Otrisovka.Write("X", 30 + 625 - 17, 60 + 7, Color.White);

            // Рамка IDE
            //lol.canvas.DrawFilledRectangle(Color.FromArgb(75, 75, 75), 30, 60, 625, 500);
            Otrisovka.Write("Tab - save, F1 - run, Esc - exit, F5 - clear", 212, 67, Color.SpringGreen);

            if (lol.Click(30 + 625 - 22, 60 + 4, 18, 18))
            {
                klavaypr.On = true;
                Kernel.op = "";
            }

            // Отрисовка кода с номерами строк
            RenderCodeLines();

            // Курсор
            RenderCursor();

            // Панель статуса
            RenderStatusBar();
        }

        private static void RenderCodeLines()
        {
            int y = 90;
            int startLine = Math.Max(0, ScrollOffset);
            int endLine = Math.Min(startLine + 25, Code.Count);

            for (int i = startLine; i < endLine; i++)
            {
                // Номер строки
                Otrisovka.Write($"{i + 1:00}", 40, y, Color.Gray);

                // Текст строки
                string lineText = i < Code.Count ? Code[i] : "";
                Otrisovka.Write(lineText, 70, y, i == CurrentLine - 1 ? Color.MediumSpringGreen : Color.White);

                y += 16;
            }
        }

        private static void RenderCursor()
        {
            if (CurrentLine - 1 >= ScrollOffset && CurrentLine - 1 < ScrollOffset + 25)
            {
                int cursorX = 70 + CursorColumn * 8;
                int cursorY = 90 + (CurrentLine - 1 - ScrollOffset) * 16;
                lol.canvas.DrawLine(Color.White, cursorX, cursorY, cursorX, cursorY + 14);
            }
        }

        private static void RenderStatusBar()
        {
            lol.canvas.DrawFilledRectangle(Color.FromArgb(75, 0, 130), 32, 528, 621, 30);
            Otrisovka.Write($"Line: {CurrentLine}, Col: {CursorColumn + 1} | Total lines: {Code.Count}", 40, 533, Color.SpringGreen);
        }

        private static void HandleInput()
        {
            if (sus.KeyboardManager.TryReadKey(out var key))
            {
                HandleKeyPress(key);
            }
        }

        private static void HandleKeyPress(sus.KeyEvent key)
        {
            klavaypr.YPRklava(key, false);

            switch (key.Key)
            {
                case ConsoleKeyy.Escape:
                    ExitIDE();
                    break;

                case ConsoleKeyy.F5:
                    ClearCode();
                    break;

                case ConsoleKeyy.F1:
                    RunCode();
                    break;

                case ConsoleKeyy.Tab:
                    SaveCode();
                    break;

                case ConsoleKeyy.Enter:
                    InsertNewLine();
                    break;

                case ConsoleKeyy.Backspace:
                    HandleBackspace();
                    break;

                case ConsoleKeyy.UpArrow:
                    MoveCursorUp();
                    break;

                case ConsoleKeyy.DownArrow:
                    MoveCursorDown();
                    break;

                case ConsoleKeyy.LeftArrow:
                    MoveCursorLeft();
                    break;

                case ConsoleKeyy.RightArrow:
                    MoveCursorRight();
                    break;

                case ConsoleKeyy.PageUp:
                    ScrollUp();
                    break;

                case ConsoleKeyy.PageDown:
                    ScrollDown();
                    break;

                default:
                    InsertCharacter(key.KeyChar);
                    break;
            }
        }

        private static void ExitIDE()
        {
            klavaypr.On = true;
            lol.op = "";
        }

        private static void ClearCode()
        {
            PragmaInterpritator.variables.Clear();
            Code.Clear();
            CurrentLine = 1;
            CurrentLineText = "";
            CursorColumn = 0;
            ScrollOffset = 0;
        }

        private static void RunCode()
        {
            PragmaInterpritator.code = new List<string>(Code);
            PragmaInterpritator.variables.Clear();
            PragmaInterpritator.Execute();
            Kernel.op = "exec";
            klavaypr.On = true;
        }

        private static void SaveCode()
        {
            try
            {
                string partitionPath = GetPartitionPath();
                if (string.IsNullOrEmpty(partitionPath)) return;

                string totalCode = string.Join("#", Code);
                string fileName = FindAvailableFileName(partitionPath);

                Directory.CreateDirectory(partitionPath + "PragmaCode");
                File.WriteAllText(partitionPath + @"PragmaCode\" + fileName + ".pcf", totalCode);

                FpsShower.Msg("Successfully saved!", "PragmaCode", true);
            }
            catch (Exception e)
            {
                FpsShower.Msg("Saving error!", e.Message, false);
            }
        }

        private static string GetPartitionPath()
        {
            foreach (var partition in Kernel.fs.Disks[Kernel.globaldskcnt].Partitions)
            {
                if (!string.IsNullOrEmpty(partition.RootPath))
                    return partition.RootPath;
            }
            return null;
        }

        private static string FindAvailableFileName(string partitionPath)
        {
            for (int i = 0; ; i++)
            {
                string fileName = "MyProgram" + i;
                if (!File.Exists(partitionPath + @"PragmaCode\" + fileName + ".pcf"))
                    return fileName;
            }
        }

        private static void InsertNewLine()
        {
            if (CurrentLine <= Code.Count)
            {
                string currentLineText = Code[CurrentLine - 1];
                string beforeCursor = currentLineText.Substring(0, CursorColumn);
                string afterCursor = currentLineText.Substring(CursorColumn);

                Code[CurrentLine - 1] = beforeCursor;
                Code.Insert(CurrentLine, afterCursor);

                CurrentLine++;
                CursorColumn = 0;

                EnsureVisible();
            }
        }

        private static void HandleBackspace()
        {
            if (CursorColumn > 0 && CurrentLine <= Code.Count)
            {
                Code[CurrentLine - 1] = Code[CurrentLine - 1].Remove(CursorColumn - 1, 1);
                CursorColumn--;
            }
            else if (CurrentLine > 1 && CursorColumn == 0)
            {
                string currentLine = Code[CurrentLine - 1];
                Code.RemoveAt(CurrentLine - 1);
                CurrentLine--;
                CursorColumn = Code[CurrentLine - 1].Length;
                Code[CurrentLine - 1] += currentLine;
            }
        }

        private static void MoveCursorUp()
        {
            if (CurrentLine > 1)
            {
                CurrentLine--;
                CursorColumn = Math.Min(CursorColumn, Code[CurrentLine - 1].Length);
                EnsureVisible();
            }
        }

        private static void MoveCursorDown()
        {
            if (CurrentLine < Code.Count)
            {
                CurrentLine++;
                CursorColumn = Math.Min(CursorColumn, Code[CurrentLine - 1].Length);
                EnsureVisible();
            }
        }

        private static void MoveCursorLeft()
        {
            if (CursorColumn > 0)
            {
                CursorColumn--;
            }
            else if (CurrentLine > 1)
            {
                CurrentLine--;
                CursorColumn = Code[CurrentLine - 1].Length;
                EnsureVisible();
            }
        }

        private static void MoveCursorRight()
        {
            if (CurrentLine <= Code.Count && CursorColumn < Code[CurrentLine - 1].Length)
            {
                CursorColumn++;
            }
            else if (CurrentLine < Code.Count)
            {
                CurrentLine++;
                CursorColumn = 0;
                EnsureVisible();
            }
        }

        private static void ScrollUp()
        {
            ScrollOffset = Math.Max(0, ScrollOffset - 10);
        }

        private static void ScrollDown()
        {
            ScrollOffset = Math.Min(Math.Max(0, Code.Count - 25), ScrollOffset + 10);
        }

        private static void EnsureVisible()
        {
            if (CurrentLine - 1 < ScrollOffset)
            {
                ScrollOffset = Math.Max(0, CurrentLine - 1);
            }
            else if (CurrentLine - 1 >= ScrollOffset + 25)
            {
                ScrollOffset = CurrentLine - 25;
            }
        }

        private static void InsertCharacter(char c)
        {
            // Разрешаем все печатные символы ASCII включая пробел
            if (char.IsAscii(c) && !char.IsControl(c) && (char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c) || c == ' '))
            {
                if (CurrentLine > Code.Count)
                {
                    Code.Add("");
                }

                Code[CurrentLine - 1] = Code[CurrentLine - 1].Insert(CursorColumn, c.ToString());
                CursorColumn++;
            }
        }

        public static void Load(string text)
        {
            Code = text.Split('#').ToList();
            CurrentLine = 1;
            CurrentLineText = "";
            CursorColumn = 0;
            ScrollOffset = 0;
        }
    }
}