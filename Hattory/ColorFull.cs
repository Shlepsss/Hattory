using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using sus = Cosmos.System;
using lol = Hattory.Kernel;
using Cosmos.System.Graphics;
using System.Drawing;
using Cosmos.System.Graphics.Fonts;
using Cosmos.Debug.Kernel.Plugs.Asm;

namespace Hattory
{
    public class Colorfull
    {
        public static int Collor = 0;
        public static List<string> coords = new List<string>();
        public static void Start()
        {
            lol.canvas.DrawFilledRectangle(Color.FromArgb(128, 0, 128), 30, 60, 300, 320);
            lol.canvas.DrawFilledRectangle(Color.FromArgb(45, 0, 45), 32, 62, 296, 316);
            lol.canvas.DrawFilledRectangle(Color.FromArgb(75, 0, 130), 32, 62, 296, 25);
            Otrisovka.Write("Colorfull Draw", 40, 67, Color.Lavender);

            // Кнопка закрытия
            lol.canvas.DrawFilledRectangle(Color.FromArgb(220, 20, 60), 30 + 300 - 22, 60 + 4, 18, 18);
            Otrisovka.Write("X", 30 + 300 - 17, 60 + 7, Color.White);

            lol.canvas.DrawFilledRectangle(Color.SkyBlue, 40, 310, 60, 25);
            lol.canvas.DrawFilledRectangle(Color.White, 110, 310, 60, 25);
            Otrisovka.Write("SAVE", 45, 317, Color.Black);
            Otrisovka.Write("CLEAR", 115, 317, Color.Black);
            int colorY = 345;
            lol.canvas.DrawFilledRectangle(Color.White, 40, 95, 280, 200);
            lol.canvas.DrawFilledRectangle(Color.DarkGray, 40, colorY, 20, 20);
            lol.canvas.DrawFilledRectangle(Color.Blue, 70, colorY, 20, 20);
            lol.canvas.DrawFilledRectangle(Color.Green, 100, colorY, 20, 20);
            lol.canvas.DrawFilledRectangle(Color.SkyBlue, 130, colorY, 20, 20);
            lol.canvas.DrawFilledRectangle(Color.Red, 160, colorY, 20, 20);
            lol.canvas.DrawFilledRectangle(Color.DeepPink, 190, colorY, 20, 20);
            lol.canvas.DrawFilledRectangle(Color.Yellow, 220, colorY, 20, 20);

            if (Collor == 0) { Otrisovka.Write("Black", 250, colorY + 2, Color.DarkGray); }
            if (Collor == 1) { Otrisovka.Write("Blue", 250, colorY + 2, Color.DarkBlue); }
            if (Collor == 2) { Otrisovka.Write("Green", 250, colorY + 2, Color.DarkGreen); }
            if (Collor == 3) { Otrisovka.Write("SkyBlue", 250, colorY + 2, Color.SkyBlue); }
            if (Collor == 4) { Otrisovka.Write("Red", 250, colorY + 2, Color.DarkRed); }
            if (Collor == 5) { Otrisovka.Write("Pink", 250, colorY + 2, Color.DeepPink); }
            if (Collor == 6) { Otrisovka.Write("Yellow", 250, colorY + 2, Color.YellowGreen); }

            // COLORS
            #region colors
            if (lol.Click(40, colorY, 20, 20)) { Collor = 0; }
            if (lol.Click(70, colorY, 20, 20)) { Collor = 1; }
            if (lol.Click(100, colorY, 20, 20)) { Collor = 2; }
            if (lol.Click(130, colorY, 20, 20)) { Collor = 3; }
            if (lol.Click(160, colorY, 20, 20)) { Collor = 4; }
            if (lol.Click(190, colorY, 20, 20)) { Collor = 5; }
            if (lol.Click(220, colorY, 20, 20)) { Collor = 6; }
            #endregion

            if (lol.Click(40, 95, 280, 200))
            {
                if (!coords.Contains((int)sus.MouseManager.X + ";" + (int)sus.MouseManager.Y))
                {
                    coords.Add((int)sus.MouseManager.X + ";" + (int)sus.MouseManager.Y + ";" + Collor);
                }
            }
            if (lol.ClickRight(40, 95, 280, 200))
            {
                if (coords.Count != 0)
                {
                    foreach (string s in coords)
                    {
                        string[] coordinates = s.Split(';');
                        int x = Convert.ToInt32(coordinates[0]);
                        int y = Convert.ToInt32(coordinates[1]);
                        float d = (float)Math.Sqrt((int)Math.Pow((int)sus.MouseManager.X - x, 2) + (int)Math.Pow((int)sus.MouseManager.Y - y, 2));
                        if (d <= 3.0f)
                        {
                            coords.Remove(s);
                        }
                    }
                }
            }
            if (coords.Count != 0)
            {
                foreach (string s in coords)
                {
                    string[] coordinates = s.Split(';');
                    if (coordinates[2] == "0") { lol.canvas.DrawFilledCircle(Color.Black, Convert.ToInt32(coordinates[0]), Convert.ToInt32(coordinates[1]), 2); }
                    if (coordinates[2] == "1") { lol.canvas.DrawFilledCircle(Color.Blue, Convert.ToInt32(coordinates[0]), Convert.ToInt32(coordinates[1]), 2); }
                    if (coordinates[2] == "2") { lol.canvas.DrawFilledCircle(Color.Green, Convert.ToInt32(coordinates[0]), Convert.ToInt32(coordinates[1]), 2); }
                    if (coordinates[2] == "3") { lol.canvas.DrawFilledCircle(Color.SkyBlue, Convert.ToInt32(coordinates[0]), Convert.ToInt32(coordinates[1]), 2); }
                    if (coordinates[2] == "4") { lol.canvas.DrawFilledCircle(Color.Red, Convert.ToInt32(coordinates[0]), Convert.ToInt32(coordinates[1]), 2); }
                    if (coordinates[2] == "5") { lol.canvas.DrawFilledCircle(Color.DeepPink, Convert.ToInt32(coordinates[0]), Convert.ToInt32(coordinates[1]), 2); }
                    if (coordinates[2] == "6") { lol.canvas.DrawFilledCircle(Color.Yellow, Convert.ToInt32(coordinates[0]), Convert.ToInt32(coordinates[1]), 2); }
                }
            }
            if (lol.Click(40, 310, 60, 25))
            {
                try 
                {
                    string goodpart = "";
                    string ftext = "";
                    foreach (string s in coords)
                    {
                        ftext += s + "&";
                    }
                    string filenam;
                    foreach (var partition in Kernel.fs.Disks[Kernel.globaldskcnt].Partitions)
                    {
                        if (partition.RootPath == "" || partition.RootPath == null) { }
                        else
                        {
                            goodpart = partition.RootPath;
                        }
                    }
                    if (goodpart != "")
                    {
                        for (int i = 0; ; i++)
                        {
                            filenam = "MyImage" + i.ToString();
                            if (!Directory.Exists(goodpart + "ColorFull")) { Directory.CreateDirectory(goodpart + "ColorFull"); }
                            if (!File.Exists(goodpart + @"ColorFull\" + filenam + ".cfi"))
                            {
                                File.Create(goodpart + @"ColorFull\" + filenam + ".cfi");
                                File.WriteAllText(goodpart + @"ColorFull\" + filenam + ".cfi", ftext);
                                FpsShower.Msg("Successfully saved!");
                                FpsShower.playSound = true;
                                break;
                            }
                        }
                    }
                }
                catch (Exception e) {
                    FpsShower.Msg("Saving error!", e.ToString(), false);
                    FpsShower.playSound = true;
                }
            }
            /*if (lol.Click(30, 640, 50, 25) && File.Exists(@"0:\MyImage.CFI"))
            {
                string data = File.ReadAllText(@"0:\MyImage.CFI");
                string[] pixelarr = data.Split("&");
                List<string> pixellist = pixelarr.ToList();
                pixellist.Remove(pixellist.Last());
                foreach (string s in pixellist)
                {
                    if (!coords.Contains(s)) { coords.Add(s); }
                }
            }*/
            if (lol.Click(110, 310, 60, 25))
            {
                coords.Clear();
            }
            // Обработка закрытия
            if (lol.Click(30 + 300 - 22, 60 + 4, 18, 18))
            {
                Kernel.op = "";
            }
        }
        public static void Loadd(string info)
        {
            string[] pixelarr = info.Split("&");
            List<string> pixellist = pixelarr.ToList();
            pixellist.Remove(pixellist.Last());
            coords = pixellist;
        }
    }
}