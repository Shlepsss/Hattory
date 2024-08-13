﻿using Cosmos.Core;
using Cosmos.Core.Multiboot;
using Cosmos.HAL;
using Cosmos.HAL.Drivers.Video;
using Cosmos.System;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using System;
using System.Drawing;
using System.IO;
using ConsoleKeyy = Cosmos.System.ConsoleKeyEx;
using sus = Cosmos.System;

#pragma warning disable CA1416 // бля меня этот ебучий варнинг заебал

namespace Hattory
{
    public class Kernel : sus.Kernel
    {

        //=====CANVAS======

        public static Canvas canvas;

        //====BOOLEANS====

        public static bool ispysk = false; //knopka pysk
        public static bool ispcinfo = false; //pc information
        public static bool iscolors = false; //color changer
        public static bool iscalc = false; //calculator
        public static bool istaskm = false; //hardware monitor (old task manager)
        public static bool isdiskman = false; //disk manager
        public static bool oboi = true; //show the wallpaper or not?
        public static bool isalwaysshowfps; //always true
        public static bool isfilemanager = false; // file manager
        public static bool iscalcready = false; //idk lol, something for normal calculator work. I wrote this in 2021
        public static bool isformatsure = false; //Are you want to delete file?
        public static bool isfilerename = false; //Are you want to rename file?
        public static bool issettingmenu = false; // menu with PC management tools
        public static bool shutdown; // Do you want to shutdown the computer?
        public static bool isColorfull = false; //paint activated?
        public static bool isguessit = false;

        //====OTHER====

        public static string first; //for calculator
        public static string second; //for calculator
        public static float firstI; //for calculator
        public static float secondI; //for calculator
        public static float c; //for calculator
        public static string status = ""; //quo? for guess it yet
        public static string enterrednum = ""; //for guess it
        public static int randomnum; //for guess it
        public static int globaldskcnt = 0; //current disk number
        public static Color colora; //system background color
        public static sus.FileSystem.CosmosVFS fs = new CosmosVFS(); //FS
        [ManifestResourceStream(ResourceName = "Hattory.krab.bmp")] public static byte[] krabsburger;
        [ManifestResourceStream(ResourceName = "Hattory.cursor.bmp")] public static byte[] tallc;
        public static sus.Graphics.Bitmap image = new sus.Graphics.Bitmap(krabsburger);
        public static sus.Graphics.Bitmap tc = new sus.Graphics.Bitmap(tallc);

        //BEFORE RUN
        protected override void BeforeRun()
        {
            try
            {
                //Console.Beep(2000, 100);
                //canvas.Clear(1);
                //canvas.DrawImage(BIMG, 0, 0);
                //canvas.Display();
                //Console.Beep(2000, 1000);
                try
                {
                    VFSManager.RegisterVFS(fs);
                }
                catch (Exception) { } //FILE SYSTEM
                //GRAPHICS SELECT PHASE
                bool canvber()
                {
                    if (VBEDriver.ISAModeAvailable()) { return true; }
                    if (PCI.Exists(VendorID.VirtualBox, DeviceID.VBVGA)) { return true; }
                    if (PCI.Exists(VendorID.Bochs, DeviceID.BGA)) { return true; }
                    return Multiboot2.IsVBEAvailable;
                }
                colora = Color.Black;
                if (PCI.GetDevice(VendorID.VMWare, DeviceID.SVGAIIAdapter) != null && PCI.Exists(PCI.GetDevice(VendorID.VMWare, DeviceID.SVGAIIAdapter)))
                {
                    canvas = new SVGAIICanvas(new Mode(1024, 768, ColorDepth.ColorDepth32));
                    sus.MouseManager.ScreenWidth = 1024;
                    sus.MouseManager.ScreenHeight = 768;
                }
                else if (canvber())
                {
                    canvas = new VBECanvas(new Mode(1024, 768, ColorDepth.ColorDepth32));
                    sus.MouseManager.ScreenWidth = 1024;
                    sus.MouseManager.ScreenHeight = 768;
                }
                sus.MouseManager.X = sus.MouseManager.ScreenWidth / 2;
                sus.MouseManager.Y = sus.MouseManager.ScreenHeight / 2;
                FpsShower.ShouldRender = true;
                //Console.Beep(3000, 1000);
            }
            catch (Exception e)
            {
                canvas.Disable();
                System.Console.BackgroundColor = ConsoleColor.Blue;
                System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.WriteLine("Vse, kina ne bydet!");
                System.Console.Write("ERROR CODE: " + e.Message);
            }
        }

        //--------------------------\
        // Clicks / Clicks / Clicks |
        //--------------------------/
        #region govno

        public static bool Click(int x, int y, int w, int h)
        {
            if (sus.MouseManager.X <= x + w && sus.MouseManager.X >= x)
            {
                if (sus.MouseManager.Y <= y + h && sus.MouseManager.Y >= y)
                {
                    if (sus.MouseManager.MouseState == sus.MouseState.Left)
                    {
                        sus.MouseManager.MouseState = sus.MouseState.None;
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool ClickMiddle(int x, int y, int w, int h)
        {
            if (sus.MouseManager.X <= x + w && sus.MouseManager.X >= x)
            {
                if (sus.MouseManager.Y <= y + h && sus.MouseManager.Y >= y)
                {
                    if (sus.MouseManager.MouseState == sus.MouseState.Middle)
                    {
                        sus.MouseManager.MouseState = sus.MouseState.None;
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool ClickInfinity(int x, int y, int w, int h)
        {
            if (sus.MouseManager.X <= x + w && sus.MouseManager.X >= x)
            {
                if (sus.MouseManager.Y <= y + h && sus.MouseManager.Y >= y)
                {
                    if (sus.MouseManager.MouseState == sus.MouseState.Left)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool CheckPos(int x, int y, int w, int h)
        {
            if (sus.MouseManager.X <= x + w && sus.MouseManager.X >= x)
            {
                if (sus.MouseManager.Y <= y + h && sus.MouseManager.Y >= y)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool ClickRight(int x, int y, int w, int h)
        {
            if (sus.MouseManager.X <= x + w && sus.MouseManager.X >= x)
            {
                if (sus.MouseManager.Y <= y + h && sus.MouseManager.Y >= y)
                {
                    if (sus.MouseManager.MouseState == sus.MouseState.Right)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        protected override void Run()
        {
            try
            {
                #region start
                canvas.Clear(colora);
                //oboi
                if (oboi == true)
                {
                    canvas.DrawImage(image, 0, 0);
                }
                //pysk
                canvas.DrawFilledRectangle(Color.Indigo, 0, 0, 1024, 50);
                canvas.DrawFilledCircle(Color.SlateBlue, 989, 25, 20);
                //time
                int mont;
                mont = DateTime.Now.Month;
                string montt = "";
                switch (mont)
                {
                    case 01:
                        montt = "January";
                        break;
                    case 02:
                        montt = "February";
                        break;
                    case 03:
                        montt = "March";
                        break;
                    case 04:
                        montt = "April";
                        break;
                    case 05:
                        montt = "May";
                        break;
                    case 06:
                        montt = "June";
                        break;
                    case 07:
                        montt = "July";
                        break;
                    case 08:
                        montt = "August";
                        break;
                    case 09:
                        montt = "September";
                        break;
                    case 10:
                        montt = "October";
                        break;
                    case 11:
                        montt = "November";
                        break;
                    case 12:
                        montt = "December";
                        break;
                }
                //pysk
                if (CheckPos(964, 0, 45, 50))
                {
                    if (ispysk == false)
                    {
                        ispysk = true;
                    }
                }
                #endregion

                // ----------------------------------------------\
                // CLOSE WINDOWS / CLOSE WINDOWS / CLOSE WINDOWS |
                //-----------------------------------------------/

                #region close

                //PC info
                if (ClickRight(30, 60, 380, 102))
                {
                    if (ispcinfo == true)
                    {
                        ispcinfo = false;
                    }
                }
                //colors
                if (ClickRight(30, 60, 270, 75))
                {
                    if (iscolors == true)
                    {
                        iscolors = false;
                    }
                }
                //calc
                if (ClickRight(25, 60, 225, 325))
                {
                    if (iscalc == true)
                    {
                        iscalc = false;
                        Calc.c = c;
                        Calc.first = first;
                        Calc.second = second;
                        Calc.firstI = firstI;
                        Calc.secondI = secondI;
                        Calc.iscalcready = false;
                    }
                }
                //taskman
                if (ClickRight(645, 510, 150, 85))
                {
                    if (istaskm == true)
                    {
                        istaskm = false;
                    }
                }
                //FileManager
                if (ClickRight(10, 60, 270 + (Notepad.path.Length * 2), 250))
                {
                    if (isfilemanager == true && isformatsure == false)
                    {
                        isfilemanager = false;
                    }
                }
                //DiskManager
                if (ClickRight(10, 60, 300, 125))
                {
                    if (isdiskman == true)
                    {
                        isdiskman = false;
                    }
                }
                //setting menu
                if (ClickRight(100, 100, 200, 150))
                {
                    if (issettingmenu == true)
                    {
                        issettingmenu = false;
                    }
                }
                //Guess It!
                if (ClickRight(150, 100, 375, 200))
                {
                    klavaypr.On = true;
                    isguessit = false;
                }

                #endregion

                // ------------------------------------------\
                // CLICKS CLICKS CLICKS CLICKS CLICKS CLICKS |
                //-------------------------------------------/


                #region clicks

                //====SettingMenu
                if (Click(889, 52, 96, 15))
                {
                    if (ispysk)
                    {
                        if (!issettingmenu)
                        {
                            isfilemanager = false;
                            shutdown = false;
                            iscalcready = false;
                            issettingmenu = true;
                            ispysk = false; //knopka pysk
                            ispcinfo = false; //pc information
                            iscolors = false; //color changer
                            iscalc = false; //calculator
                            isdiskman = false; //disk manager
                            isfilerename = false; //Are you want to rename file?
                            isguessit = false;
                            isColorfull = false;
                            if (isformatsure)
                            {
                                Notepad.islock = false;
                                isformatsure = false; //Are you want to delete file?
                            }
                            else if (isfilerename)
                            {
                                Notepad.islock = false;
                                Notepad.filename = "";
                                klavaypr.On = true;
                                isfilerename = false;
                            }
                        }
                    }
                }
                //====FILE MANAGER
                if (Click(889, 67, 112, 15))
                {
                    if (ispysk)
                    {
                        if (!isfilemanager)
                        {
                            isfilemanager = true;
                            shutdown = false;
                            iscalcready = false;
                            issettingmenu = false;
                            ispysk = false; //knopka pysk
                            ispcinfo = false; //pc information
                            iscolors = false; //color changer
                            iscalc = false; //calculator
                            isdiskman = false; //disk manager
                            isfilerename = false; //Are you want to rename file?
                            isguessit = false;
                            isColorfull = false;
                            if (isformatsure)
                            {
                                Notepad.islock = false;
                                isformatsure = false; //Are you want to delete file?
                            }
                            else if (isfilerename)
                            {
                                Notepad.islock = false;
                                Notepad.filename = "";
                                klavaypr.On = true;
                                isfilerename = false;
                            }
                        }
                    }
                }
                //====CALCULATOR
                if (Click(889, 82, 96, 15))
                {
                    if (ispysk == true)
                    {
                        if (iscalc == false)
                        {
                            isfilemanager = false;
                            c = 0;
                            firstI = 0;
                            secondI = 0;
                            first = "";
                            second = "";
                            shutdown = false;
                            iscalcready = false;
                            issettingmenu = false;
                            ispysk = false; //knopka pysk
                            ispcinfo = false; //pc information
                            iscolors = false; //color changer
                            iscalc = true; //calculator
                            isdiskman = false; //disk manager
                            isfilerename = false; //Are you want to rename file?
                            isguessit = false;
                            isColorfull = false;
                            if (isformatsure)
                            {
                                Notepad.islock = false;
                                isformatsure = false; //Are you want to delete file?
                            }
                            else if (isfilerename)
                            {
                                Notepad.islock = false;
                                Notepad.filename = "";
                                klavaypr.On = true;
                                isfilerename = false;
                            }
                        }
                    }
                }
                //====Guess It!
                if (Click(889, 98, 72, 15))
                {
                    if (ispysk)
                    {
                        if (!issettingmenu)
                        {
                            isfilemanager = false;
                            shutdown = false;
                            iscalcready = false;
                            issettingmenu = false;
                            ispysk = false; //knopka pysk
                            ispcinfo = false; //pc information
                            iscolors = false; //color changer
                            iscalc = false; //calculator
                            isdiskman = false; //disk manager
                            isfilerename = false; //Are you want to rename file?
                            isguessit = true;
                            klavaypr.On = false;
                            randomnum = new Random().Next(0, 101);
                            status = "New number has made";
                            enterrednum = "";
                            isColorfull = false;
                            if (isformatsure)
                            {
                                Notepad.islock = false;
                                isformatsure = false; //Are you want to delete file?
                            }
                            else if (isfilerename)
                            {
                                Notepad.islock = false;
                                Notepad.filename = "";
                                klavaypr.On = true;
                                isfilerename = false;
                            }
                        }
                    }
                }
                //====colorfull
                if (Click(889, 114, 72, 15))
                {
                    if (ispysk)
                    {
                        if (!isColorfull)
                        {
                            isfilemanager = false;
                            shutdown = false;
                            iscalcready = false;
                            issettingmenu = false;
                            ispysk = false; //knopka pysk
                            ispcinfo = false; //pc information
                            iscolors = false; //color changer
                            iscalc = false; //calculator
                            isdiskman = false; //disk manager
                            isfilerename = false; //Are you want to rename file?
                            isguessit = false;
                            isColorfull = true;
                            if (isformatsure)
                            {
                                Notepad.islock = false;
                                isformatsure = false; //Are you want to delete file?
                            }
                            else if (isfilerename)
                            {
                                Notepad.islock = false;
                                Notepad.filename = "";
                                klavaypr.On = true;
                                isfilerename = false;
                            }
                        }
                    }
                }
                //====disable pysk
                if (sus.MouseManager.MouseState == sus.MouseState.Right)
                {
                    if (ispysk == true)
                    {
                        ispysk = false;
                    }
                }
                //====shutdown
                if (Click(889, 150, 30, 30))
                {
                    if (ispysk)
                    {
                        if (!shutdown)
                        {
                            isfilemanager = false;
                            shutdown = true;
                            iscalcready = false;
                            issettingmenu = false;
                            ispysk = false; //knopka pysk
                            ispcinfo = false; //pc information
                            iscolors = false; //color changer
                            iscalc = false; //calculator
                            isdiskman = false; //disk manager
                            isfilerename = false; //Are you want to rename file?
                            isguessit = false;
                            isColorfull = false;
                            if (isformatsure)
                            {
                                isformatsure = false; //Are you want to delete file?
                                Notepad.islock = false;
                            }
                            else if (isfilerename)
                            {
                                isfilerename = false;
                                Notepad.islock = false;
                                Notepad.filename = "";
                                klavaypr.On = true;
                            }
                        }
                    }
                }
                //=====rebbot
                if (Click(929, 150, 30, 30))
                {
                    if (ispysk == true)
                    {
                        sus.Power.Reboot();
                    }
                }

                #endregion

                // ---------------------------------------------\
                // BOOLEANS BOOLEANS BOOLEANS BOOLEANS BOOLEANS |
                //----------------------------------------------/

                #region boolean

                //====shutdown MENU
                if (shutdown)
                {
                    //Code generated by HattoryWindowCreator

                    canvas.DrawFilledRectangle(Color.DarkGray, 250, 200, 250, 100); //window background
                    canvas.DrawFilledRectangle(Color.Gray, 390, 240, 100, 50); //button
                    canvas.DrawFilledRectangle(Color.Gray, 260, 240, 100, 50); //button
                    Otrisovka.Write("Are you want to shutdown PC?", 260, 210, Color.Black); //CWR=224
                    Otrisovka.Write("Yes", 298, 257, Color.Black); //CWR=24
                    Otrisovka.Write("No", 432, 257, Color.Black); //CWR=16

                    if (Click(390, 240, 100, 50))
                    {
                        shutdown = false;
                    }

                    if (Click(260, 240, 100, 50))
                    {
                        sus.Power.Shutdown();
                    }
                }
                //====FPS
                if (isalwaysshowfps == true)
                {
                    canvas.DrawFilledRectangle(Color.Black, 0, 743, 70, 25);
                    Otrisovka.Write("FPS: " + (FpsShower.FPS / 2).ToString(), 5, 748, Color.White);
                }
                //====PCInformation
                if (ispcinfo == true)
                {
                    canvas.DrawFilledRectangle(Color.LightGray, 30, 60, 380, 102);
                    Otrisovka.Write("Computer Information", 35, 62, Color.Black);
                    Otrisovka.Write("RAM: " + CPU.GetAmountOfRAM().ToString() + "MB", 35, 78, Color.Black);
                    try
                    {
                        Otrisovka.Write("CPU: " + CPU.GetCPUBrandString(), 35, 94, Color.Black);
                    }
                    catch (Exception) { }
                    Otrisovka.Write("Frames Per Second: " + (FpsShower.FPS / 2).ToString(), 35, 110, Color.Black);
                    Otrisovka.Write("Video Driver: " + canvas.Name(), 35, 126, Color.Black);
                    Otrisovka.Write("Resolution: " + canvas.Mode.Width + "x" + canvas.Mode.Height + " 32 bit color", 35, 142, Color.Black);
                    if (Click(35, 110, 176, 16))
                    {
                        if (isalwaysshowfps == false)
                        {
                            isalwaysshowfps = true;
                        }
                        else if (isalwaysshowfps == true)
                        {
                            isalwaysshowfps = false;
                        }
                    }
                }
                //====SETTING MENU
                if (issettingmenu == true)
                {
                    //Code generated by HattoryWindowCreator

                    canvas.DrawFilledRectangle(Color.DarkGray, 100, 100, 200, 150);
                    Otrisovka.Write("Disk Manager", 110, 170, Color.Black);
                    Otrisovka.Write("Hardware Monitor", 110, 150, Color.Black);
                    Otrisovka.Write("PC Information", 110, 130, Color.Black);
                    Otrisovka.Write("Colors", 110, 110, Color.Black);
                    //colors
                    if (Click(110, 110, 48, 16))
                    {
                        if (iscolors == false)
                        {
                            ispcinfo = false;
                            ispysk = false;
                            iscalc = false;
                            isfilemanager = false;
                            isdiskman = false;
                            iscolors = true;
                            issettingmenu = false;
                        }
                    }
                    //PCInf
                    if (Click(110, 130, 112, 16))
                    {
                        if (ispcinfo == false)
                        {
                            iscolors = false;
                            ispysk = false;
                            isdiskman = false;
                            iscalc = false;
                            isfilemanager = false;
                            ispcinfo = true;
                            issettingmenu = false;
                        }
                    }
                    //HM
                    if (Click(110, 150, 128, 16))
                    {
                        if (istaskm == false)
                        {
                            istaskm = true;
                        }
                    }
                    //DM
                    if (Click(110, 170, 96, 16))
                    {
                        if (isdiskman == false)
                        {
                            iscalcready = false;
                            isfilemanager = false;
                            ispcinfo = false;
                            ispysk = false;
                            iscolors = false;
                            iscalc = false;
                            isdiskman = true;
                            issettingmenu = false;
                        }
                    }
                }
                //====FILE MANAGER
                if (isfilemanager == true)
                {
                    //Notepad.path = globaldskcnt.ToString() + @":\";
                    Notepad.enternote();
                }
                //===COLOR CHANGER
                if (iscolors == true)
                {
                    canvas.DrawFilledRectangle(Color.LightGray, 30, 60, 270, 75);
                    canvas.DrawFilledRectangle(Color.MidnightBlue, 35, 65, 75, 30);
                    canvas.DrawFilledRectangle(Color.Black, 115, 65, 75, 30);
                    Otrisovka.Write("Wallpaper", 117, 80, Color.White);
                    canvas.DrawFilledRectangle(Color.Navy, 195, 65, 75, 30);
                    canvas.DrawFilledRectangle(Color.DarkSlateGray, 35, 100, 75, 30);
                    canvas.DrawFilledRectangle(Color.Black, 115, 100, 75, 30);
                    canvas.DrawFilledRectangle(Color.Teal, 195, 100, 75, 30);
                    if (Click(35, 65, 75, 30))
                    {
                        colora = Color.MidnightBlue;
                    }
                    if (Click(115, 65, 75, 30))
                    {
                        if (oboi == false)
                        {
                            oboi = true;
                        }
                        else if (oboi == true)
                        {
                            oboi = false;
                        }
                    }
                    if (Click(195, 65, 75, 30))
                    {
                        colora = Color.Navy;
                    }
                    if (Click(35, 100, 75, 30))
                    {
                        colora = Color.DarkSlateGray;
                    }
                    if (Click(115, 100, 75, 30))
                    {
                        colora = Color.Black;
                    }
                    if (Click(195, 100, 75, 30))
                    {
                        colora = Color.Teal;
                    }
                }
                //=====KNOPKA PUSK
                if (ispysk == true)
                {
                    canvas.DrawFilledRectangle(Color.SlateBlue, 884, 50, 140, 140);
                    Otrisovka.Write("Settigs Menu", 889, 52, Color.White);
                    Otrisovka.Write("File Inspector", 889, 68, Color.White);
                    Otrisovka.Write("Calculator", 889, 84, Color.White);
                    Otrisovka.Write("Guess It!", 889, 100, Color.White);
                    Otrisovka.Write("ColorFull", 889, 116, Color.White);
                    canvas.DrawFilledRectangle(Color.Crimson, 889, 150, 30, 30);
                    canvas.DrawFilledRectangle(Color.OrangeRed, 929, 150, 30, 30);
                    //------------------------------
                }
                //=====HARDWARE MONITOR
                if (istaskm == true)
                {
                    float URIM = GCImplementation.GetUsedRAM() / 1024 / 1024;
                    int URpercentage = (int)(URIM / CPU.GetAmountOfRAM() * 100); //get percentage
                    canvas.DrawFilledRectangle(Color.LightGray, 869, 678, 150, 85);
                    Otrisovka.Write("Hardware Monitor", 874, 683, Color.Black);
                    Otrisovka.Write("RAM Usage in MBs:", 874, 698, Color.Black);
                    canvas.DrawFilledRectangle(Color.DarkGreen, 874, 713, 100, 20);
                    canvas.DrawFilledRectangle(Color.Lime, 874, 713, URpercentage, 20);
                    Otrisovka.Write(URIM.ToString() + " MBs / " + URpercentage.ToString() + " %", 876, 716, Color.Black);
                    Otrisovka.Write("FPS: " + (FpsShower.FPS / 2).ToString(), 874, 738, Color.Black);
                }
                //===CALCULATOR
                if (iscalc == true)
                {
                    Calc.Calculator();
                }
                //=====DISK MAMAGER
                if (isdiskman == true)
                {
                    try
                    {
                        canvas.DrawFilledRectangle(Color.Purple, 10, 60, 300, 125);
                        Otrisovka.Write("Disks: " + "current - " + globaldskcnt, 15, 65, Color.Black);
                        var diskslist = VFSManager.GetDisks();
                        int iznai = 80;
                        int dskcnt = 0;
                        foreach (var disk in diskslist)
                        {
                            Otrisovka.Write("Disk " + dskcnt + " - " + (disk.Size / 1024 / 1024) + "MB, " + "MBR: " + disk.IsMBR, 15, iznai, Color.Black);
                            if (Click(15, iznai, 36, 16))
                            {
                                globaldskcnt = dskcnt;
                                Notepad.path = dskcnt + @":\";
                                isfilemanager = true;
                                isdiskman = false;
                            }
                            /*try
                            {
                                disk.Mount();
                            }
                            catch (Exception) { }
                            if (ClickRight(15, iznai, 36, 9))
                            {
                                isformatsure = true;
                            }*/
                            dskcnt += 1;
                            iznai += 16;
                        }
                    }
                    catch (Exception) { }
                }
                //Guess It!
                if (isguessit == true)
                {
                    //Code generated by HattoryWindowCreator
                    canvas.DrawFilledRectangle(Color.DarkGray, 150, 100, 375, 130); //window background
                    canvas.DrawFilledRectangle(Color.Gray, 415, 140, 100, 75); //button
                    canvas.DrawFilledRectangle(Color.Gray, 153, 181, 200, 15);
                    Otrisovka.Write("New Number", 427, 174, Color.Black); //CWR=48
                    Otrisovka.Write("I made random number from 0 to 100, Guess It!", 155, 105, Color.Black); //CWR=360
                    sus.KeyEvent k;
                    bool IsKeyPressed = sus.KeyboardManager.TryReadKey(out k);
                    Otrisovka.Write(enterrednum, 155, 180, Color.Black);
                    Otrisovka.Write(status, 155, 210, Color.Black);
                    klavaypr.YPRklava(k);
                    if (k.Key == ConsoleKeyy.Spacebar)
                    {
                        enterrednum += " ";
                    }
                    else if (k.Key == ConsoleKeyy.Backspace)
                    {
                        if (enterrednum.Length >= 1)
                        {
                            enterrednum = enterrednum.Remove(enterrednum.Length - 1);
                        }
                    }
                    else if (k.Key == ConsoleKeyy.Escape)
                    {
                        klavaypr.On = true;
                        isguessit = false;
                    }
                    else if (k.Key == ConsoleKeyy.Enter)
                    {
                        try
                        {
                            int num = Convert.ToInt32(enterrednum);
                            if (num > randomnum)
                            {
                                status = "Guessed number less than " + num.ToString();
                            }
                            else if (num < randomnum)
                            {
                                status = "Guessed number greater than " + num.ToString();
                            }
                            else if (num == randomnum)
                            {
                                status = "You guess it! Congratulations!";
                                randomnum = new System.Random().Next(0, 101);
                            }
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        if (char.IsAscii(k.KeyChar) && k.KeyChar != ' ' && char.IsLetterOrDigit(k.KeyChar))
                        {
                            enterrednum += k.KeyChar;
                        }
                    }
                    if (Click(415, 150, 100, 75))
                    {
                        randomnum = new System.Random().Next(0, 101);
                        status = "New number has made";
                        enterrednum = "";
                    }
                }
                //======SureDelete
                if (isformatsure == true)
                {
                    canvas.DrawFilledRectangle(Color.LightGray, 400, 240, 100, 50); // 270, 200
                    Otrisovka.Write("Delete File?", 405, 245, Color.Black);
                    canvas.DrawFilledRectangle(Color.DarkGray, 415, 260, 35, 25);
                    Otrisovka.Write("Yes", 420, 270, Color.Black);
                    canvas.DrawFilledRectangle(Color.DarkGray, 455, 260, 35, 25);
                    Otrisovka.Write("No", 465, 270, Color.Black);
                    if (Click(415, 260, 35, 25))
                    {
                        File.Delete(Notepad.path + Notepad.filenameeee);
                        Notepad.islock = false;
                        isformatsure = false;
                    }
                    if (Click(455, 260, 35, 25))
                    {
                        //Directory.CreateDirectory(@"0:\");
                        Notepad.islock = false;
                        isformatsure = false;
                    }
                }
                //=====RenameFile
                if (isfilerename == true)
                {
                    canvas.DrawFilledRectangle(Color.Gray, 305, 240, 180, 50); // 270, 200       //420
                    Otrisovka.Write("Create file with name:", 310, 245, Color.Black); //775
                    try
                    {
                        sus.KeyEvent k;
                        bool IsKeyPressed = sus.KeyboardManager.TryReadKey(out k);
                        Otrisovka.Write(Notepad.filename, 310, 260, Color.Black);
                        klavaypr.YPRklava(k);
                        if (k.Key == ConsoleKeyy.Spacebar)
                        {
                            Notepad.filename += " ";
                        }
                        else if (k.Key == ConsoleKeyy.Backspace)
                        {
                            Notepad.filename = Notepad.filename.Remove(Notepad.filename.Length - 1);
                        }
                        else if (k.Key == ConsoleKeyy.Escape)
                        {
                            Notepad.filename = "";
                            Notepad.islock = false;
                            klavaypr.On = true;
                            isfilerename = false;
                        }
                        else if (k.Key == ConsoleKeyy.Enter)
                        {
                            try
                            {
                                File.Create(Notepad.path + @"\" + Notepad.filename);
                                Notepad.filename = "test.txt";
                                Notepad.islock = false;
                                klavaypr.On = true;
                                isfilerename = false;
                            }
                            catch (Exception)
                            {
                                Notepad.filename = "";
                                Notepad.islock = false;
                                klavaypr.On = true;
                                isfilerename = false;
                            }
                        }
                        else
                        {
                            if (char.IsAscii(k.KeyChar) && k.KeyChar != ' ')
                            {
                                Notepad.filename += k.KeyChar;
                            }
                        }
                    }
                    catch (Exception) { }
                }

                if (isColorfull)
                {
                    Colorfull.Start();
                }

                #endregion

                // ------------------------------------------\
                // OTHER OTHER OTHER OTHER OTHER OTHER OTHER |
                //-------------------------------------------/

                #region other

                //Time
                Otrisovka.Write(DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString(), 5, 1, Color.White);
                Otrisovka.Write(montt, 5, 17, Color.White);
                Otrisovka.Write(DateTime.Now.Day.ToString() + "th " + DateTime.Now.Year.ToString(), 5, 33, Color.White);
                //mouse
                try
                {
                    canvas.DrawImageAlpha(tc, (int)sus.MouseManager.X, (int)sus.MouseManager.Y);
                }
                catch (Exception) { }
                klavaypr.control();
                canvas.Display();
                Cosmos.Core.Memory.Heap.Collect();

                #endregion
            }
            catch (Exception e)
            {
                sus.PCSpeaker.Beep();
                //Only for debug
                //canvas.Disable();
                //System.Console.WriteLine(e.ToString());
            }
        }
    }
}