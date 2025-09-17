using Cosmos.HAL.BlockDevice;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ConsoleKeyy = Cosmos.System.ConsoleKeyEx;
using sus = Cosmos.System;

namespace Hattory
{
    internal class Notepad
    {
        public static Canvas canvas = Kernel.canvas;
        public static string CurrentPath = "";
        public static string CurrentFilePath = "";
        public static string TextContent = "";
        public static byte[] FileBytes;
        public static sus.Graphics.Bitmap LoadedImage;
        public static string SelectedFileName;
        public static bool IsTextEditorActive = true;
        public static bool IsImageOpened = true;
        public static bool IsOperationLocked = false;

        // Переменные для редактора
        public static int EditorCursorLine = 0;
        public static int EditorCursorColumn = 0;
        public static int EditorScrollOffset = 0;
        public static List<string> EditorLines = new List<string>();
        public static bool IsEditorActive = false;
        public static string EditedFileName = "";

        // Переменные для переключателя дисков
        public static bool IsDiskSelectionActive = false;
        public static int SelectedDiskIndex = 0;

        // Цвета интерфейса (фиолетовая темная тема)
        public static Color WindowBg = Color.FromArgb(45, 0, 45);
        public static Color WindowBorder = Color.FromArgb(128, 0, 128);
        public static Color TitleBar = Color.FromArgb(75, 0, 130);
        public static Color TextColor = Color.Lavender;
        public static Color HighlightColor = Color.FromArgb(138, 43, 226);
        public static Color ButtonColor = Color.FromArgb(106, 90, 205);
        public static Color CloseButtonColor = Color.FromArgb(220, 20, 60);

        // Координаты окон
        public static int FileManagerX = 10;
        public static int FileManagerY = 60;
        public static int FileManagerWidth = 1004;
        public static int FileManagerHeight = 500;

        public static int TextEditorX = 10;
        public static int TextEditorY = 60;
        public static int TextEditorWidth = 1004;
        public static int TextEditorHeight = 500;

        public static void EnterNote()
        {
            try
            {
                if (IsDiskSelectionActive)
                {
                    ShowDiskSelection();
                    return;
                }

                if (IsEditorActive)
                {
                    ShowTextEditor();
                }
                else
                {
                    RenderFileManagerUI();
                    HandleFileOperations();
                }
            }
            catch (Exception ex)
            {
                HandleError("File Manager Error", ex.Message);
            }
            finally
            {
                Cosmos.Core.Memory.Heap.Collect();
            }
        }

        private static string GetCurrentPartitionPath()
        {
            try
            {
                var disks = VFSManager.GetDisks();
                if (disks == null || disks.Count == 0)
                    return @"0:\";

                int diskIndex = Math.Min(Kernel.globaldskcnt, disks.Count - 1);
                var disk = disks[diskIndex];

                // Простой и надежный способ получения пути
                if (disk.Partitions.Count > 0 && !string.IsNullOrEmpty(disk.Partitions[0].RootPath))
                {
                    return disk.Partitions[0].RootPath;
                }
            }
            catch (Exception)
            {
                // Игнорируем ошибки и используем fallback
            }

            return @"0:\";
        }

        private static void RenderFileManagerUI()
        {
            // Окно файлового менеджера
            canvas.DrawFilledRectangle(WindowBorder, FileManagerX, FileManagerY, FileManagerWidth, FileManagerHeight);
            canvas.DrawFilledRectangle(WindowBg, FileManagerX + 2, FileManagerY + 2, FileManagerWidth - 4, FileManagerHeight - 4);

            // Заголовок окна
            canvas.DrawFilledRectangle(TitleBar, FileManagerX + 2, FileManagerY + 2, FileManagerWidth - 4, 25);
            Otrisovka.Write($"File Manager - Disk {Kernel.globaldskcnt}: {CurrentPath}", FileManagerX + 20, FileManagerY + 7, TextColor);

            // Кнопка закрытия
            int closeButtonX = FileManagerX + FileManagerWidth - 22;
            int closeButtonY = FileManagerY + 4;
            canvas.DrawFilledRectangle(CloseButtonColor, closeButtonX, closeButtonY, 18, 18);
            Otrisovka.Write("X", closeButtonX + 5, closeButtonY + 3, Color.White);

            // Разделители
            canvas.DrawLine(HighlightColor, FileManagerX + 2, FileManagerY + 27, FileManagerX + FileManagerWidth - 2, FileManagerY + 27);
            canvas.DrawLine(HighlightColor, FileManagerX + 200, FileManagerY + 2, FileManagerX + 200, FileManagerY + FileManagerHeight - 2);

            // Заголовки колонок
            Otrisovka.Write("DIRECTORIES", FileManagerX + 20, FileManagerY + 35, HighlightColor);
            Otrisovka.Write("FILES", FileManagerX + 210, FileManagerY + 35, HighlightColor);

            // Информационная панель
            int statusY = FileManagerY + FileManagerHeight - 32;
            canvas.DrawFilledRectangle(TitleBar, FileManagerX + 2, statusY, FileManagerWidth - 4, 30);
            Otrisovka.Write("Left: Open | Middle: Delete/Create | Right: Set BG", FileManagerX + 20, statusY + 5, TextColor);
        }

        private static void HandleFileOperations()
        {
            if (string.IsNullOrEmpty(CurrentPath))
            {
                CurrentPath = GetCurrentPartitionPath();
            }

            ShowFilesAndDirectories();
            HandleSpecialCases();
        }

        private static void ShowFilesAndDirectories()
        {
            try
            {
                if (!Directory.Exists(CurrentPath))
                {
                    CurrentPath = GetCurrentPartitionPath();
                    if (!Directory.Exists(CurrentPath))
                    {
                        HandleError("Directory not found", "Cannot access file system");
                        return;
                    }
                }

                int dirY = FileManagerY + 50;
                int fileY = FileManagerY + 50;

                // Кнопка возврата (только если не в корне)
                string rootPath = GetCurrentPartitionPath();
                if (CurrentPath != rootPath)
                {
                    Otrisovka.Write("[..]", FileManagerX + 20, dirY, HighlightColor);

                    if (Kernel.Click(FileManagerX + 20, dirY, 40, 16))
                    {
                        string parentPath = Path.GetDirectoryName(CurrentPath.TrimEnd('\\'));
                        if (!string.IsNullOrEmpty(parentPath))
                        {
                            CurrentPath = parentPath;
                        }
                        else
                        {
                            CurrentPath = rootPath;
                        }
                        if (!CurrentPath.EndsWith(@"\")) CurrentPath += @"\";
                        return;
                    }
                    dirY += 18;
                }

                // Кнопка переключения дисков
                Otrisovka.Write("[Switch Disk]", FileManagerX + 20, dirY, HighlightColor);
                if (Kernel.Click(FileManagerX + 20, dirY, 120, 16))
                {
                    IsDiskSelectionActive = true;
                    return;
                }
                dirY += 18;

                var directories = Directory.GetDirectories(CurrentPath);
                var files = Directory.GetFiles(CurrentPath);

                foreach (string dir in directories)
                {
                    string dirName = Path.GetFileName(dir);
                    Otrisovka.Write(dirName, FileManagerX + 20, dirY, HighlightColor);

                    if (Kernel.Click(FileManagerX + 20, dirY, dirName.Length * 8, 16))
                    {
                        CurrentPath = Path.Combine(CurrentPath, dirName);
                        if (!CurrentPath.EndsWith(@"\")) CurrentPath += @"\";
                        return;
                    }
                    dirY += 18;
                }

                foreach (string filePath in files)
                {
                    string fileName = Path.GetFileName(filePath);
                    Otrisovka.Write(fileName, FileManagerX + 210, fileY, TextColor);

                    HandleFileClicks(filePath, fileName, fileName.Length * 8, fileY);
                    fileY += 18;
                }
            }
            catch (Exception ex)
            {
                HandleError("Directory Access Error", ex.Message);
            }
        }

        private static void ShowDiskSelection()
        {
            canvas.DrawFilledRectangle(WindowBorder, 300, 200, 424, 250);
            canvas.DrawFilledRectangle(WindowBg, 302, 202, 420, 246);
            canvas.DrawFilledRectangle(TitleBar, 302, 202, 420, 25);
            Otrisovka.Write("Select Disk", 320, 207, TextColor);

            try
            {
                var disks = VFSManager.GetDisks();
                int yPos = 230;

                for (int i = 0; i < disks.Count; i++)
                {
                    var disk = disks[i];
                    string diskInfo = $"Disk {i}: {disk.Size / 1024 / 1024}MB";

                    // Подсвечиваем текущий выбранный диск
                    Color diskColor = (i == Kernel.globaldskcnt) ? HighlightColor : TextColor;

                    Otrisovka.Write(diskInfo, 320, yPos, diskColor);

                    if (Kernel.Click(320, yPos, 200, 16))
                    {
                        Kernel.globaldskcnt = i;
                        CurrentPath = GetCurrentPartitionPath();
                        IsDiskSelectionActive = false;
                        return;
                    }
                    yPos += 18;
                }

                // Кнопка отмены
                canvas.DrawFilledRectangle(ButtonColor, 420, 400, 80, 25);
                Otrisovka.Write("Cancel", 430, 405, TextColor);
                if (Kernel.Click(420, 400, 80, 25))
                {
                    IsDiskSelectionActive = false;
                }
            }
            catch (Exception ex)
            {
                HandleError("Disk Error", ex.Message);
                IsDiskSelectionActive = false;
            }
        }

        private static void HandleFileClicks(string filePath, string fileName, int textWidth, int currentY)
        {
            string fullPath = Path.Combine(CurrentPath, fileName);

            if (Kernel.Click(FileManagerX + 210, currentY, textWidth, 16))
            {
                OpenFile(fullPath, fileName);
            }
            else if (Kernel.ClickMiddle(FileManagerX + 210, currentY, textWidth, 16) && !IsOperationLocked)
            {
                PrepareForDeletion(fileName);
            }
            else if (Kernel.ClickRight(FileManagerX + 210, currentY, textWidth, 16))
            {
                SetAsBackground(fullPath, fileName);
            }
        }

        private static void OpenFile(string fullPath, string fileName)
        {
            try
            {
                if (!File.Exists(fullPath))
                {
                    HandleError("File not found", fullPath);
                    return;
                }

                string extension = Path.GetExtension(fileName).ToLower();

                switch (extension)
                {
                    case ".txt":
                        OpenTextFile(fullPath, fileName);
                        break;
                    case ".cfi":
                        OpenColorfullFile(fullPath);
                        break;
                    case ".pcf":
                        OpenPragmaFile(fullPath);
                        break;
                    case ".bmp":
                        OpenImageFile(fullPath);
                        break;
                    default:
                        HandleError("Unsupported format", $"Cannot open {extension} files");
                        break;
                }
            }
            catch (Exception ex)
            {
                HandleError("File Open Error", ex.Message);
            }
        }

        private static void OpenTextFile(string fullPath, string fileName)
        {
            try
            {
                string fileContent = File.ReadAllText(fullPath);
                EditedFileName = fileName;

                EditorLines.Clear();

                // Используем тот же разделитель, что и при сохранении
                string[] lines = fileContent.Split('#');
                EditorLines.AddRange(lines);

                EditorCursorLine = 0;
                EditorCursorColumn = 0;
                EditorScrollOffset = 0;
                IsEditorActive = true;
                IsTextEditorActive = false;

                // Отключаем управление курсором клавиатурой в редакторе
                klavaypr.On = false;
            }
            catch (Exception ex)
            {
                HandleError("File Read Error", ex.Message);
            }
        }

        private static void ShowTextEditor()
        {
            // Окно редактора
            canvas.DrawFilledRectangle(WindowBorder, TextEditorX, TextEditorY, TextEditorWidth, TextEditorHeight);
            canvas.DrawFilledRectangle(WindowBg, TextEditorX + 2, TextEditorY + 2, TextEditorWidth - 4, TextEditorHeight - 4);

            // Заголовок окна
            canvas.DrawFilledRectangle(TitleBar, TextEditorX + 2, TextEditorY + 2, TextEditorWidth - 4, 25);
            Otrisovka.Write($"Text Editor - {EditedFileName}", TextEditorX + 20, TextEditorY + 7, TextColor);

            // Кнопка закрытия
            int closeButtonX = TextEditorX + TextEditorWidth - 22;
            int closeButtonY = TextEditorY + 4;
            canvas.DrawFilledRectangle(CloseButtonColor, closeButtonX, closeButtonY, 18, 18);
            Otrisovka.Write("X", closeButtonX + 5, closeButtonY + 3, Color.White);

            // Область номеров строк
            canvas.DrawFilledRectangle(Color.FromArgb(60, 0, 60), TextEditorX + 2, TextEditorY + 27, 40, TextEditorHeight - 57);

            // Номера строк
            int lineY = TextEditorY + 32;
            int visibleLines = Math.Min(28, EditorLines.Count - EditorScrollOffset);

            for (int i = 0; i < visibleLines; i++)
            {
                int lineNumber = EditorScrollOffset + i + 1;
                Otrisovka.Write($"{lineNumber:000}", TextEditorX + 5, lineY, HighlightColor);
                lineY += 16;
            }

            // Текст
            lineY = TextEditorY + 32;
            for (int i = 0; i < visibleLines; i++)
            {
                int lineIndex = EditorScrollOffset + i;
                if (lineIndex < EditorLines.Count)
                {
                    string lineText = EditorLines[lineIndex];
                    if (lineText.Length > 120)
                    {
                        lineText = lineText.Substring(0, 117) + "...";
                    }
                    Otrisovka.Write(lineText, TextEditorX + 45, lineY, TextColor);
                }
                lineY += 16;
            }

            // Курсор
            if (EditorCursorLine >= EditorScrollOffset && EditorCursorLine < EditorScrollOffset + 28)
            {
                int cursorX = TextEditorX + 45 + Math.Min(EditorCursorColumn, 120) * 8;
                int cursorY = TextEditorY + 32 + (EditorCursorLine - EditorScrollOffset) * 16;
                canvas.DrawLine(HighlightColor, cursorX, cursorY, cursorX, cursorY + 14);
            }

            // Панель статуса
            int statusY = TextEditorY + TextEditorHeight - 32;
            canvas.DrawFilledRectangle(TitleBar, TextEditorX + 2, statusY, TextEditorWidth - 4, 30);
            Otrisovka.Write($"Line: {EditorCursorLine + 1}, Col: {EditorCursorColumn + 1} | ESC: Exit | F1: Save | F2: Save & Exit",
                           TextEditorX + 20, statusY + 5, TextColor);

            HandleEditorInput();
        }

        private static void HandleEditorInput()
        {
            // Обработка клика по кнопке закрытия
            int closeButtonX = TextEditorX + TextEditorWidth - 22;
            int closeButtonY = TextEditorY + 4;
            if (Kernel.Click(closeButtonX, closeButtonY, 18, 18))
            {
                CloseTextEditor(false);
                return;
            }

            if (sus.KeyboardManager.TryReadKey(out var key))
            {
                // Отключаем стандартное управление курсором в редакторе
                bool controlEnabled = klavaypr.On;
                klavaypr.On = false;

                switch (key.Key)
                {
                    case ConsoleKeyy.Escape:
                        CloseTextEditor(false);
                        break;

                    case ConsoleKeyy.F1:
                        SaveTextFile();
                        break;

                    case ConsoleKeyy.F2:
                        if (SaveTextFile())
                        {
                            CloseTextEditor(true);
                        }
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
                        EditorScrollOffset = Math.Max(0, EditorScrollOffset - 10);
                        break;

                    case ConsoleKeyy.PageDown:
                        EditorScrollOffset = Math.Min(Math.Max(0, EditorLines.Count - 28), EditorScrollOffset + 10);
                        break;

                    default:
                        InsertCharacter(key.KeyChar);
                        break;
                }

                // Восстанавливаем состояние управления курсором
                klavaypr.On = controlEnabled;
            }
        }

        private static void InsertNewLine()
        {
            if (EditorCursorLine < EditorLines.Count)
            {
                string currentLine = EditorLines[EditorCursorLine];
                string beforeCursor = currentLine.Substring(0, EditorCursorColumn);
                string afterCursor = currentLine.Substring(EditorCursorColumn);

                EditorLines[EditorCursorLine] = beforeCursor;
                EditorLines.Insert(EditorCursorLine + 1, afterCursor);

                EditorCursorLine++;
                EditorCursorColumn = 0;

                EnsureEditorVisible();
            }
            else
            {
                EditorLines.Add("");
                EditorCursorLine = EditorLines.Count - 1;
                EditorCursorColumn = 0;
            }
        }

        private static void HandleBackspace()
        {
            if (EditorCursorColumn > 0 && EditorCursorLine < EditorLines.Count)
            {
                EditorLines[EditorCursorLine] = EditorLines[EditorCursorLine].Remove(EditorCursorColumn - 1, 1);
                EditorCursorColumn--;
            }
            else if (EditorCursorLine > 0 && EditorCursorColumn == 0)
            {
                string currentLine = EditorLines[EditorCursorLine];
                EditorLines.RemoveAt(EditorCursorLine);
                EditorCursorLine--;
                EditorCursorColumn = EditorLines[EditorCursorLine].Length;
                EditorLines[EditorCursorLine] += currentLine;
                EnsureEditorVisible();
            }
        }

        private static void MoveCursorUp()
        {
            if (EditorCursorLine > 0)
            {
                EditorCursorLine--;
                EditorCursorColumn = Math.Min(EditorCursorColumn, EditorLines[EditorCursorLine].Length);
                EnsureEditorVisible();
            }
        }

        private static void MoveCursorDown()
        {
            if (EditorCursorLine < EditorLines.Count - 1)
            {
                EditorCursorLine++;
                EditorCursorColumn = Math.Min(EditorCursorColumn, EditorLines[EditorCursorLine].Length);
                EnsureEditorVisible();
            }
        }

        private static void MoveCursorLeft()
        {
            if (EditorCursorColumn > 0)
            {
                EditorCursorColumn--;
            }
            else if (EditorCursorLine > 0)
            {
                EditorCursorLine--;
                EditorCursorColumn = EditorLines[EditorCursorLine].Length;
                EnsureEditorVisible();
            }
        }

        private static void MoveCursorRight()
        {
            if (EditorCursorLine < EditorLines.Count && EditorCursorColumn < EditorLines[EditorCursorLine].Length)
            {
                EditorCursorColumn++;
            }
            else if (EditorCursorLine < EditorLines.Count - 1)
            {
                EditorCursorLine++;
                EditorCursorColumn = 0;
                EnsureEditorVisible();
            }
        }

        private static void InsertCharacter(char c)
        {
            if (!char.IsControl(c) && char.IsAscii(c) && c != '\0')
            {
                if (EditorCursorLine >= EditorLines.Count)
                {
                    EditorLines.Add("");
                }

                EditorLines[EditorCursorLine] = EditorLines[EditorCursorLine].Insert(EditorCursorColumn, c.ToString());
                EditorCursorColumn++;
            }
        }

        private static void EnsureEditorVisible()
        {
            if (EditorCursorLine < EditorScrollOffset)
            {
                EditorScrollOffset = Math.Max(0, EditorCursorLine);
            }
            else if (EditorCursorLine >= EditorScrollOffset + 28)
            {
                EditorScrollOffset = EditorCursorLine - 27;
            }
        }

        private static bool SaveTextFile()
        {
            try
            {
                string fullPath = Path.Combine(CurrentPath, EditedFileName);

                // Используем тот же подход, что и в Visual Stupidio
                string totalCode = "";
                foreach (string line in EditorLines)
                {
                    totalCode += line + "#"; // Используем # как разделитель строк
                }

                // Убираем последний лишний разделитель
                if (totalCode.EndsWith("#"))
                {
                    totalCode = totalCode.Substring(0, totalCode.Length - 1);
                }

                File.WriteAllText(fullPath, totalCode);

                FpsShower.Msg("Text Editor", "File saved successfully!", true);
                FpsShower.playSound = true;
                return true;
            }
            catch (Exception ex)
            {
                HandleError("Save Error", $"Failed to save: {ex.Message}");
                return false;
            }
        }

        private static void CloseTextEditor(bool saveSuccess)
        {
            IsEditorActive = false;
            IsTextEditorActive = true;
            EditorLines.Clear();
            TextContent = "";
            EditedFileName = "";

            // Восстанавливаем управление курсором
            klavaypr.On = true;

            if (saveSuccess)
            {
                FpsShower.Msg("Text Editor", "File saved and closed", true);
                FpsShower.playSound = true;
            }
        }

        private static void OpenColorfullFile(string fullPath)
        {
            try
            {
                string content = File.ReadAllText(fullPath);
                Colorfull.Loadd(content);
                Kernel.op = "colorfull";
            }
            catch (Exception ex)
            {
                HandleError("Colorfull Error", ex.Message);
            }
        }

        private static void OpenPragmaFile(string fullPath)
        {
            try
            {
                string content = File.ReadAllText(fullPath);
                PragmaCode.Load(content);
                Kernel.op = "pragmaC";
            }
            catch (Exception ex)
            {
                HandleError("Pragma Error", ex.Message);
            }
        }

        private static void OpenImageFile(string fullPath)
        {
            if (!IsImageOpened) return;

            try
            {
                FileBytes = File.ReadAllBytes(fullPath);
                LoadedImage = new sus.Graphics.Bitmap(FileBytes);

                if (LoadedImage.Width <= 512 && LoadedImage.Height <= 512)
                {
                    if (LoadedImage.Depth == ColorDepth.ColorDepth32)
                    {
                        IsImageOpened = false;
                    }
                    else
                    {
                        ShowMessage("Not 32 bit depth!", "Only 32 bit depth supported!", false);
                    }
                }
                else
                {
                    ShowMessage("Image too large", "512x512 max supported!", false);
                }
            }
            catch (Exception ex)
            {
                HandleError("Image Load Error", ex.Message);
            }
        }

        private static void PrepareForDeletion(string fileName)
        {
            try
            {
                IsOperationLocked = true;
                SelectedFileName = fileName;
                Kernel.op = "delete";
            }
            catch (Exception ex)
            {
                HandleError("Delete Error", ex.Message);
            }
        }

        private static void SetAsBackground(string fullPath, string fileName)
        {
            if (fileName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    FileBytes = File.ReadAllBytes(fullPath);
                    LoadedImage = new sus.Graphics.Bitmap(FileBytes);

                    if (LoadedImage.Height <= 768 && LoadedImage.Width <= 1024 &&
                        LoadedImage.Depth == ColorDepth.ColorDepth32)
                    {
                        Kernel.image = LoadedImage;
                    }
                    else
                    {
                        ShowMessage("Invalid image", "Max 1024x768, 32-bit depth required", false);
                    }
                }
                catch (Exception ex)
                {
                    HandleError("Background Set Error", ex.Message);
                }
            }
        }

        private static void HandleSpecialCases()
        {
            // Обработка клика по кнопке закрытия файлового менеджера
            int closeButtonX = FileManagerX + FileManagerWidth - 22;
            int closeButtonY = FileManagerY + 4;
            if (Kernel.Click(closeButtonX, closeButtonY, 18, 18))
            {
                Kernel.op = "";
                return;
            }

            // Создание нового файла (средняя кнопка мыши)
            if (Kernel.ClickMiddle(FileManagerX + 210, FileManagerY + 50, 200, 400) && !IsOperationLocked)
            {
                IsOperationLocked = true;
                SelectedFileName = "newfile.txt";
                Kernel.op = "rename";
            }
        }

        private static void HandleError(string title, string message)
        {
            FpsShower.Msg(title,message,false);
            FpsShower.playSound = true;
        }

        private static void ShowMessage(string title, string message, bool isInfo)
        {
            FpsShower.Msg(title, message, true);
            FpsShower.playSound = true;
        }
    }
}