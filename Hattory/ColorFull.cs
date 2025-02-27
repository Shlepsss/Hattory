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
            lol.canvas.DrawFilledRectangle(Color.Red, 30, 610, 50, 25);
            Otrisovka.Write("SAVE", 35, 619, Color.Black);
            lol.canvas.DrawFilledRectangle(Color.White, 90, 610, 50, 25);
            Otrisovka.Write("CLEAR", 95, 619, Color.Black);
            lol.canvas.DrawFilledRectangle(Color.DarkGray, 30, 640, 20, 20);
            lol.canvas.DrawFilledRectangle(Color.Blue, 60, 640, 20, 20);
            lol.canvas.DrawFilledRectangle(Color.Green, 90, 640, 20, 20);
            lol.canvas.DrawFilledRectangle(Color.SkyBlue, 120, 640, 20, 20);
            lol.canvas.DrawFilledRectangle(Color.Red, 30, 670, 20, 20);
            lol.canvas.DrawFilledRectangle(Color.DeepPink, 60, 670, 20, 20);
            lol.canvas.DrawFilledRectangle(Color.Yellow, 90, 670, 20, 20);
            lol.canvas.DrawFilledRectangle(Color.White, 120, 670, 20, 20);
            lol.canvas.DrawFilledRectangle(Color.White, 30, 300, 300, 300);

            if (Collor == 0) { Otrisovka.Write("Black", 75, 700, Color.DarkGray); }
            if (Collor == 1) { Otrisovka.Write("Blue", 75, 700, Color.DarkBlue); }
            if (Collor == 2) { Otrisovka.Write("Green", 75, 700, Color.DarkGreen); }
            if (Collor == 3) { Otrisovka.Write("SkyBlue", 75, 700, Color.SkyBlue); }
            if (Collor == 4) { Otrisovka.Write("Red", 75, 700, Color.DarkRed); }
            if (Collor == 5) { Otrisovka.Write("Pink", 75, 700, Color.DeepPink); }
            if (Collor == 6) { Otrisovka.Write("Yellow", 75, 700, Color.YellowGreen); }
            if (Collor == 7) { Otrisovka.Write("White..? useless :D", 75, 700, Color.White); }

            // COLORS
            #region colors
            if (lol.Click(30, 640, 20, 20)) { Collor = 0; }
            if (lol.Click(60, 640, 20, 20)) { Collor = 1; }
            if (lol.Click(90, 640, 20, 20)) { Collor = 2; }
            if (lol.Click(120, 640, 20, 20)) { Collor = 3; }
            if (lol.Click(30, 670, 20, 20)) { Collor = 4; }
            if (lol.Click(60, 670, 20, 20)) { Collor = 5; }
            if (lol.Click(90, 670, 20, 20)) { Collor = 6; }
            if (lol.Click(120, 670, 20, 20)) { Collor = 7; }
            #endregion

            if (lol.Click(30, 300, 300, 300))
            {
                if (!coords.Contains((int)sus.MouseManager.X + ";" + (int)sus.MouseManager.Y))
                {
                    coords.Add((int)sus.MouseManager.X + ";" + (int)sus.MouseManager.Y + ";" + Collor);
                }
            }
            if (lol.ClickRight(30, 300, 300, 300))
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
                    if (coordinates[2] == "7") { lol.canvas.DrawFilledCircle(Color.White, Convert.ToInt32(coordinates[0]), Convert.ToInt32(coordinates[1]), 2); }
                }
            }
            if (lol.Click(30, 610, 50, 25))
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
            if (lol.Click(90, 610, 50, 25))
            {
                coords.Clear();
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