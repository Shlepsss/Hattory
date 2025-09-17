using COROS;
using Cosmares;
using Cosmos.Core;
using Cosmos.Core.Multiboot;
using Cosmos.HAL;
using Cosmos.HAL.BlockDevice;
using Cosmos.HAL.Drivers.Audio;
using Cosmos.HAL.Drivers.Video;
using Cosmos.System;
using Cosmos.System.Audio.IO;
using Cosmos.System.Audio;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ConsoleKeyy = Cosmos.System.ConsoleKeyEx;
using sus = Cosmos.System;
using Cosmos.HAL.Audio;
using Cosmos.Debug.Kernel.Plugs.Asm;
using System.Linq;
using Cosmos.System.Audio.DSP.Processing;
using System.Threading;

#pragma warning disable CA1416 // бля меня этот ебучий варнинг заебал

namespace Hattory
{
    public class Kernel : sus.Kernel
    {

        //=====CANVAS======

        public static Canvas canvas;

        //====BOOLEANS====

        public static bool ispysk = false; //knopka pysk
        public static bool istaskmgr = false; //knopka pysk
        public static bool oboi = true; //show the wallpaper or not?
        public static bool isalwaysshowfps; //always true
        public static bool Render = true;
        public static bool NoAC97msg = false;
        public static bool NoAC97 = false;
        public static string op = "";

        public static string FileManagerCurrentPath = @"Partitions on disk:";
        public static string SelectedFileName = "";
        public static bool IsFileOperationLocked = false;

        //====OTHER====

        public static int att = 0;
        public static Color colorOfPanel = Color.Indigo;
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
        public static string soundStr = "===-------"; //sound
        public static sus.FileSystem.CosmosVFS fs = new CosmosVFS(); //FS
        [ManifestResourceStream(ResourceName = "Hattory.krab.bmp")] public static byte[] krabsburger;
        [ManifestResourceStream(ResourceName = "Hattory.cursor.bmp")] public static byte[] tallc;
        [ManifestResourceStream(ResourceName = "Hattory.chord.wav")] public static byte[] chor;
        [ManifestResourceStream(ResourceName = "Hattory.startup.wav")] public static byte[] startu;
        #region icons
        [ManifestResourceStream(ResourceName = "Hattory.icons.calc.bmp")] public static byte[] calcI;
        [ManifestResourceStream(ResourceName = "Hattory.icons.colorfull.bmp")] public static byte[] colorfullI;
        [ManifestResourceStream(ResourceName = "Hattory.icons.fm.bmp")] public static byte[] fmI;
        [ManifestResourceStream(ResourceName = "Hattory.icons.guess.bmp")] public static byte[] guessI;
        [ManifestResourceStream(ResourceName = "Hattory.icons.info.bmp")] public static byte[] infoI;
        [ManifestResourceStream(ResourceName = "Hattory.icons.sett.bmp")] public static byte[] settI;
        [ManifestResourceStream(ResourceName = "Hattory.icons.vs.bmp")] public static byte[] vsI;
        [ManifestResourceStream(ResourceName = "Hattory.icons.warn.bmp")] public static byte[] warnI;
        [ManifestResourceStream(ResourceName = "Hattory.icons.sound.bmp")] public static byte[] soundI;
        public static sus.Graphics.Bitmap calcIcon = new sus.Graphics.Bitmap(calcI);
        public static sus.Graphics.Bitmap colorfullIcon = new sus.Graphics.Bitmap(colorfullI);
        public static sus.Graphics.Bitmap fmIcon = new sus.Graphics.Bitmap(fmI);
        public static sus.Graphics.Bitmap guessIcon = new sus.Graphics.Bitmap(guessI);
        public static sus.Graphics.Bitmap infoIcon = new sus.Graphics.Bitmap(infoI);
        public static sus.Graphics.Bitmap settIcon = new sus.Graphics.Bitmap(settI);
        public static sus.Graphics.Bitmap vsIcon = new sus.Graphics.Bitmap(vsI);
        public static sus.Graphics.Bitmap warnIcon = new sus.Graphics.Bitmap(warnI);
        public static sus.Graphics.Bitmap soundIcon = new sus.Graphics.Bitmap(soundI);
        #endregion
        public static sus.Graphics.Bitmap image = new sus.Graphics.Bitmap(krabsburger);
        public static sus.Graphics.Bitmap tc = new sus.Graphics.Bitmap(tallc);

        public static AudioMixer mixer;
        public static AC97 driver;
        public static SeekableAudioStream stream;
        public static AudioManager audioManager;
        public static GainPostProcessor audioVolume = new GainPostProcessor(0.3f);

        //BEFORE RUN
        protected override void BeforeRun()
        {
            try
            {
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
                    canvas.Clear(0);
                    canvas.Display();
                    Otrisovka.Write("VM detected... Selected SVGAII...   OK", 5, 5, Color.White);
                    canvas.Display();
                }
                else if (canvber())
                {
                    canvas = new VBECanvas(new Mode(1024, 768, ColorDepth.ColorDepth32));
                    sus.MouseManager.ScreenWidth = 1024;
                    sus.MouseManager.ScreenHeight = 768;
                    canvas.Clear(0);
                    canvas.Display();
                    Otrisovka.Write("VBE Screen initializing... OK", 5, 5, Color.White);
                    canvas.Display();
                }

                Thread.Sleep(200);

                //ACHI
                Otrisovka.Write("ACHI Initialize...", 5, 20, Color.White);
                canvas.Display();
                AHCI_DISK ahci_load = new();
                ahci_load.Init();
                Otrisovka.Write("OK", 170, 20, Color.White);
                canvas.Display();
                Otrisovka.Write("File System initializing...", 5, 35, Color.White);
                canvas.Display();

                Thread.Sleep(200);

                //FILE SYSTEM
                try
                {
                    VFSManager.RegisterVFS(fs);
                    Otrisovka.Write("OK", 270, 35, Color.White);
                    canvas.Display();
                }
                catch (Exception e) {
                    Otrisovka.Write("Failed " + e.Message, 300, 35, Color.White);
                    canvas.Display();
                }

                Thread.Sleep(200);

                sus.MouseManager.X = sus.MouseManager.ScreenWidth / 2;
                sus.MouseManager.Y = sus.MouseManager.ScreenHeight / 2;
                FpsShower.ShouldRender = true;

                Otrisovka.Write("Sound initializing...", 5, 50, Color.White);
                canvas.Display();
                try
                {
                    driver = AC97.Initialize(4096);
                    mixer = new AudioMixer()
                    {
                        SampleRate = 44100,
                    };
                    mixer.PostProcessors.Add(audioVolume);
                    mixer.Streams.Add(new MemoryAudioStream(new SampleFormat(AudioBitDepth.Bits16, 2, true), 44100, Kernel.startu));
                    audioManager = new AudioManager()
                    {
                        Stream = mixer,
                        Output = driver
                    };
                    audioManager.Enable();

                    Otrisovka.Write("OK", 200, 50, Color.White);
                    canvas.Display();
                }
                catch (Exception e)
                {
                    NoAC97msg = true;
                    NoAC97 = true;
                    Otrisovka.Write("Failed " + e.Message, 200, 50, Color.White);
                    canvas.Display();
                }

                Thread.Sleep(200);

                Otrisovka.Write("Starting system...", 5, 65, Color.White);
                canvas.Display();

                Thread.Sleep(1000);

                canvas.Clear(Color.Black);
                canvas.Display();

                Thread.Sleep(200);
                //АНИМАЦИЯ

                canvas.Clear(Color.Black);
                canvas.DrawImage(image, 0, 0);
                Otrisovka.Write("Made by Shlepsss. 2025. V1.6.1709", 380, 220, Color.White);
                canvas.DrawFilledRectangle(Color.Indigo, 0, 0, 1024, 50);
                canvas.DrawFilledCircle(Color.SlateBlue, 989, 25, 20);
                canvas.Display();

                Thread.Sleep(1500);
            }
            catch (Exception e)
            {
                canvas.Clear(Color.Red);
                canvas.Display();
                Otrisovka.Write("CRITICAL ERROR: " + e.Message + "... SYSTEM HALTED.", 50, 50, Color.Black);
                canvas.Display();
            }
        }

        //--------------------------\
        // Clicks / Clicks / Clicks |
        //--------------------------/
        #region Clicks

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
                    canvas.DrawImage(image, 512 - (int)(0.5 * image.Width), 384 - (int)(0.5 * image.Height));
                }
                //pysk
                canvas.DrawFilledRectangle(colorOfPanel, 0, 0, 1024, 50);
                canvas.DrawFilledCircle(Color.SlateBlue, 989, 25, 20);
                //icons
                canvas.DrawImageAlpha(settIcon, 150,5);
                canvas.DrawImageAlpha(calcIcon, 200,5);
                canvas.DrawImageAlpha(fmIcon, 250,5);
                canvas.DrawImageAlpha(colorfullIcon, 300,5);
                canvas.DrawImageAlpha(vsIcon, 350,5);
                canvas.DrawImageAlpha(guessIcon, 400,5);
                if(!NoAC97) { canvas.DrawImageAlpha(soundIcon, 846, 5); Otrisovka.Write(soundStr, 879, 13, Color.White); }
                
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
                //taskman
                if (ClickRight(869, 678, 150, 85))
                {
                    if (istaskmgr == true)
                    {
                        istaskmgr = false;
                    }
                }
                /*
                //PC info
                if (ClickRight(30, 60, 380, 102))
                {
                    if (op == "pcinfo")
                    {
                        op = "";
                    }
                }
                //colors
                if (ClickRight(30, 60, 270, 75))
                {
                    if (op == "colors")
                    {
                        op = "";
                    }
                }
                //calc
                if (ClickRight(25, 60, 225, 325))
                {
                    if (op == "calc")
                    {
                        op = "";
                        Calc.c = c;
                        Calc.first = first;
                        Calc.second = second;
                        Calc.firstI = firstI;
                        Calc.secondI = secondI;
                        Calc.iscalcready = false;
                    }
                }
                
                //FileManager
                if (ClickRight(10, 60, 140, 400))
                {
                    if (op == "filem" && op != "delete")
                    {
                        op = "";
                        Notepad.IsTextEditorActive = true;
                        Notepad.IsImageOpened = true;
                    }
                }
                //DiskManager
                if (ClickRight(10, 60, 300, 125))
                {
                    if (op == "diskman")
                    {
                        op = "";
                    }
                }
                //setting menu
                if (ClickRight(100, 100, 200, 150))
                {
                    if (op == "settings")
                    {
                        op = "";
                    }
                }
                //Guess It!
                if (ClickRight(150, 100, 375, 130))
                {
                    if(op == "guess")
                    {
                        klavaypr.On = true;
                        op = "";
                    }
                }
                //PragmaCode
                if (ClickRight(30, 60, 550, 500))
                {
                    if (op == "pragmaC")
                    {
                        klavaypr.On = true;
                        op = "";
                    }
                }
                */
                #endregion

                // ------------------------------------------\
                // CLICKS CLICKS CLICKS CLICKS CLICKS CLICKS |
                //-------------------------------------------/

                #region clicks
                if (op != "format" && op != "delete" && op != "rename")
                {
                    //====SettingMenu
                    if (Click(889, 52, 96, 15))
                    {
                        if (ispysk)
                        {
                            if (op != "settings")
                            {
                                op = "settings";
                                ispysk = false; //knopka pysk
                                if (op == "delete")
                                {
                                    IsFileOperationLocked = false;
                                    op = ""; ; //Are you want to delete file?
                                }
                                else if (op == "rename")
                                {
                                    IsFileOperationLocked = false;
                                    klavaypr.On = true;
                                    op = "";
                                }
                            }
                        }
                    }
                    //====FILE MANAGER
                    if (Click(889, 67, 96, 15))
                    {
                        if (ispysk)
                        {
                            if (op != "filem")
                            {
                                Notepad.CurrentPath = @"Partitions on disk:";
                                op = "filem";
                                ispysk = false;
                                if (op == "delete")
                                {
                                    IsFileOperationLocked = false;
                                    op = "";
                                }
                                else if (op == "rename")
                                {
                                    IsFileOperationLocked = false;
                                    klavaypr.On = true;
                                    op = "";
                                }
                            }
                        }
                    }
                    //====CALCULATOR
                    if (Click(889, 82, 96, 15))
                    {
                        if (ispysk == true)
                        {
                            if (op != "calc")
                            {
                                c = 0;
                                firstI = 0;
                                secondI = 0;
                                first = "";
                                second = "";
                                op = "calc";
                                ispysk = false; //knopka pysk
                                if (op == "delete")
                                {
                                    IsFileOperationLocked = false;
                                    op = ""; ; //Are you want to delete file?
                                }
                                else if (op == "rename")
                                {
                                    IsFileOperationLocked = false;
                                    klavaypr.On = true;
                                    op = "";
                                }
                            }
                        }
                    }
                    //====Guess It!
                    if (Click(889, 98, 72, 15))
                    {
                        if (ispysk)
                        {
                            if (op != "guess")
                            {
                                op = "guess";
                                att = 0;
                                klavaypr.On = false;
                                randomnum = new Random().Next(0, 101);
                                status = "New number has made";
                                enterrednum = "";
                                ispysk = false; //knopka pysk
                                if (op == "delete")
                                {
                                    IsFileOperationLocked = false;
                                    op = ""; ; //Are you want to delete file?
                                }
                                else if (op == "rename")
                                {
                                    IsFileOperationLocked = false;
                                    klavaypr.On = true;
                                    op = "";
                                }
                            }
                        }
                    }
                    //====colorfull
                    if (Click(889, 114, 72, 15))
                    {
                        if (ispysk)
                        {
                            if (op != "colorfull")
                            {
                                op = "colorfull";
                                ispysk = false; //knopka pysk
                                if (op == "delete")
                                {
                                    IsFileOperationLocked = false;
                                    op = ""; ; //Are you want to delete file?
                                }
                                else if (op == "rename")
                                {
                                    IsFileOperationLocked = false;
                                    klavaypr.On = true;
                                    op = "";
                                }
                            }
                        }
                    }
                    //====PragmaCode
                    if (Click(889, 130, 120, 15))
                    {
                        if (ispysk)
                        {
                            if (op != "pragmaC")
                            {
                                op = "pragmaC";
                                klavaypr.On = false;
                                ispysk = false; //knopka pysk
                                if (op == "delete")
                                {
                                    IsFileOperationLocked = false;
                                    op = ""; ; //Are you want to delete file?
                                }
                                else if (op == "rename")
                                {
                                    IsFileOperationLocked = false;
                                    klavaypr.On = true;
                                    op = "";
                                }
                            }
                        }
                    }

                    // Иконки
                    if (Click(150, 5, 32, 32))
                    {
                        if (op != "settings")
                            {
                                op = "settings";
                                ispysk = false; //knopka pysk
                                if (op == "delete")
                                {
                                    IsFileOperationLocked = false;
                                    op = ""; ; //Are you want to delete file?
                                }
                                else if (op == "rename")
                                {
                                    IsFileOperationLocked = false;
                                    klavaypr.On = true;
                                    op = "";
                                }
                            }
                    }
                    // Иконка файлового менеджера
                    if (Click(250, 5, 32, 32))
                    {
                        if (op != "filem")
                        {
                            Notepad.CurrentPath = @"Partitions on disk:";
                            op = "filem";
                            ispysk = false;
                            if (op == "delete")
                            {
                                IsFileOperationLocked = false;
                                op = "";
                            }
                            else if (op == "rename")
                            {
                                IsFileOperationLocked = false;
                                klavaypr.On = true;
                                op = "";
                            }
                        }
                    }
                    //====CALCULATOR
                    if (Click(200, 5, 32, 32))
                    {
                        if (op != "calc")
                        {
                            c = 0;
                            firstI = 0;
                            secondI = 0;
                            first = "";
                            second = "";
                            op = "calc";
                            ispysk = false; //knopka pysk
                            if (op == "delete")
                            {
                                IsFileOperationLocked = false;
                                op = ""; ; //Are you want to delete file?
                            }
                            else if (op == "rename")
                            {
                                IsFileOperationLocked = false;
                                klavaypr.On = true;
                                op = "";
                            }
                        }
                    }
                    //====Guess It!
                    if (Click(400, 5, 32, 32))
                    {
                        if (op != "guess")
                        {
                            op = "guess";
                            klavaypr.On = false;
                            randomnum = new Random().Next(0, 101);
                            status = "New number has made";
                            enterrednum = "";
                            ispysk = false; //knopka pysk
                            if (op == "delete")
                            {
                                IsFileOperationLocked = false;
                                op = ""; ; //Are you want to delete file?
                            }
                            else if (op == "rename")
                            {
                                IsFileOperationLocked = false;
                                klavaypr.On = true;
                                op = "";
                            }
                        }
                    }
                    //====colorfull
                    if (Click(300, 5, 32, 32))
                    {
                        if (op != "colorfull")
                        {
                            op = "colorfull";
                            ispysk = false; //knopka pysk
                            if (op == "delete")
                            {
                                IsFileOperationLocked = false;
                                op = ""; ; //Are you want to delete file?
                            }
                            else if (op == "rename")
                            {
                                IsFileOperationLocked = false;
                                klavaypr.On = true;
                                op = "";
                            }
                        }
                    }
                    //====PragmaCode
                    if (Click(350, 5, 32, 32))
                    {
                        if (op != "pragmaC")
                        {
                            op = "pragmaC";
                            klavaypr.On = false;
                            ispysk = false; //knopka pysk
                            if (op == "delete")
                            {
                                IsFileOperationLocked = false;
                                op = ""; ; //Are you want to delete file?
                            }
                            else if (op == "rename")
                            {
                                IsFileOperationLocked = false;
                                klavaypr.On = true;
                                op = "";
                            }
                        }
                    }
                    //====Volume
                    if (!NoAC97)
                    {
                        if (Click(846, 5, 33, 33))
                        {
                            soundStr = "----------";
                            audioVolume.Gain = 0.0f;
    }
                        if (Click(879, 13, 8, 16))
                        {
                            soundStr = "=---------";
                            audioVolume.Gain = 0.1f;
                        }
                        if (Click(887, 13, 8, 16))
                        {
                            soundStr = "==--------";
                            audioVolume.Gain = 0.2f;
                        }
                        if (Click(895, 13, 8, 16))
                        {
                            soundStr = "===-------";
                            audioVolume.Gain = 0.3f;
                        }
                        if (Click(903, 13, 8, 16))
                        {
                            soundStr = "====------";
                            audioVolume.Gain = 0.4f;
                        }
                        if (Click(911, 13, 8, 16))
                        {
                            soundStr = "=====-----";
                            audioVolume.Gain = 0.5f;
                        }
                        if (Click(919, 13, 8, 16))
                        {
                            soundStr = "======----";
                            audioVolume.Gain = 0.6f;
                        }
                        if (Click(927, 13, 8, 16))
                        {
                            soundStr = "=======---";
                            audioVolume.Gain = 0.7f;
                        }
                        if (Click(935, 13, 8, 16))
                        {
                            soundStr = "========--";
                            audioVolume.Gain = 0.8f;
                        }
                        if (Click(943, 13, 8, 16))
                        {
                            soundStr = "=========-";
                            audioVolume.Gain = 0.9f;
                        }
                        if (Click(951, 13, 8, 16))
                        {
                            soundStr = "==========";
                            audioVolume.Gain = 1.0f;
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
                        if (op != "shutd")
                        {
                            op = "shutd";
                            klavaypr.On = true;
                            ispysk = false; //knopka pysk
                            if (op == "delete")
                            {
                                IsFileOperationLocked = false;
                                op = ""; ; //Are you want to delete file?
                            }
                            else if (op == "rename")
                            {
                                IsFileOperationLocked = false;
                                klavaypr.On = true;
                                op = "";
                            }
                        }
                    }
                    //=====reboot
                    if (Click(929, 150, 30, 30))
                    {
                        if (op != "reboot")
                        {
                            op = "reboot";
                            klavaypr.On = true;
                            ispysk = false; //knopka pysk
                            if (op == "delete")
                            {
                                IsFileOperationLocked = false;
                                op = ""; ; //Are you want to delete file?
                            }
                            else if (op == "rename")
                            {
                                IsFileOperationLocked = false;
                                klavaypr.On = true;
                                op = "";
                            }
                        }
                    }
                }
                #endregion

                // ---------------------------------------------\
                // BOOLEANS BOOLEANS BOOLEANS BOOLEANS BOOLEANS |
                //----------------------------------------------/

                #region boolean
                KeyEvent k;
                switch (op)
                {
                    //====shutdown MENU
                    case "shutd":
                        // Единый стиль окна (250x100)
                        canvas.DrawFilledRectangle(Color.FromArgb(128, 0, 128), 250, 200, 250, 100);
                        canvas.DrawFilledRectangle(Color.FromArgb(45, 0, 45), 252, 202, 246, 96);
                        canvas.DrawFilledRectangle(Color.FromArgb(75, 0, 130), 252, 202, 246, 25);
                        Otrisovka.Write("Shutdown", 260, 207, Color.Lavender);

                        // Кнопка закрытия
                        canvas.DrawFilledRectangle(Color.FromArgb(220, 20, 60), 250 + 250 - 22, 200 + 4, 18, 18);
                        Otrisovka.Write("X", 250 + 250 - 17, 200 + 7, Color.White);

                        // Содержимое - центрируем по вертикали
                        canvas.DrawFilledRectangle(Color.Gray, 260, 255, 100, 30);
                        canvas.DrawFilledRectangle(Color.Gray, 390, 255, 100, 30);
                        Otrisovka.Write("Shutdown PC?", 265, 235, Color.Lavender);
                        Otrisovka.Write("Yes", 295, 262, Color.Black);
                        Otrisovka.Write("No", 425, 262, Color.Black);

                        if (Click(260, 255, 100, 30)) { sus.Power.Shutdown(); }
                        if (Click(390, 255, 100, 30)) { op = ""; }
                        if (Click(250 + 250 - 22, 200 + 4, 18, 18)) { op = ""; }
                        break;
                    //====reboot MENU
                    case "reboot":
                        // Единый стиль окна (250x100)
                        canvas.DrawFilledRectangle(Color.FromArgb(128, 0, 128), 250, 200, 250, 100);
                        canvas.DrawFilledRectangle(Color.FromArgb(45, 0, 45), 252, 202, 246, 96);
                        canvas.DrawFilledRectangle(Color.FromArgb(75, 0, 130), 252, 202, 246, 25);
                        Otrisovka.Write("Reboot", 270, 207, Color.Lavender);

                        // Кнопка закрытия
                        canvas.DrawFilledRectangle(Color.FromArgb(220, 20, 60), 250 + 250 - 22, 200 + 4, 18, 18);
                        Otrisovka.Write("X", 250 + 250 - 17, 200 + 7, Color.White);

                        // Содержимое
                        canvas.DrawFilledRectangle(Color.Gray, 260, 255, 100, 30);
                        canvas.DrawFilledRectangle(Color.Gray, 390, 255, 100, 30);
                        Otrisovka.Write("Reboot PC?", 270, 235, Color.Lavender);
                        Otrisovka.Write("Yes", 295, 263, Color.Black);
                        Otrisovka.Write("No", 425, 263, Color.Black);

                        if (Click(260, 255, 100, 30)) { sus.Power.Reboot(); }
                        if (Click(390, 255, 100, 30)) { op = ""; }
                        if (Click(250 + 250 - 22, 200 + 4, 18, 18)) { op = ""; }
                        break;
                    //====PCInformation
                    case "pcinfo":
                        // Единый стиль окна (380x140)
                        canvas.DrawFilledRectangle(Color.FromArgb(128, 0, 128), 30, 60, 380, 140);
                        canvas.DrawFilledRectangle(Color.FromArgb(45, 0, 45), 32, 62, 376, 136);
                        canvas.DrawFilledRectangle(Color.FromArgb(75, 0, 130), 32, 62, 376, 25);
                        Otrisovka.Write("Computer Information", 40, 67, Color.Lavender);

                        // Кнопка закрытия
                        canvas.DrawFilledRectangle(Color.FromArgb(220, 20, 60), 30 + 380 - 22, 60 + 4, 18, 18);
                        Otrisovka.Write("X", 30 + 380 - 17, 60 + 7, Color.White);

                        // Информация с правильными отступами
                        Otrisovka.Write("RAM: " + CPU.GetAmountOfRAM() + "MB", 40, 95, Color.Lavender);
                        try { Otrisovka.Write("CPU: " + CPU.GetCPUBrandString(), 40, 115, Color.Lavender); }
                        catch (Exception) { }
                        Otrisovka.Write("FPS: " + (FpsShower.FPS / 2), 40, 135, Color.Lavender);
                        Otrisovka.Write("Video: " + canvas.Name(), 40, 155, Color.Lavender);
                        Otrisovka.Write("Res: " + canvas.Mode.Width + "x" + canvas.Mode.Height, 40, 175, Color.Lavender);

                        if (Click(30 + 380 - 22, 60 + 4, 18, 18)) { op = ""; }
                        break;
                    //====SETTING MENU
                    case "settings":
                        // Единый стиль окна (200x160)
                        canvas.DrawFilledRectangle(Color.FromArgb(128, 0, 128), 100, 100, 200, 160);
                        canvas.DrawFilledRectangle(Color.FromArgb(45, 0, 45), 102, 102, 196, 156);
                        canvas.DrawFilledRectangle(Color.FromArgb(75, 0, 130), 102, 102, 196, 25);
                        Otrisovka.Write("Settings", 120, 107, Color.Lavender);

                        // Кнопка закрытия
                        canvas.DrawFilledRectangle(Color.FromArgb(220, 20, 60), 100 + 200 - 22, 100 + 4, 18, 18);
                        Otrisovka.Write("X", 100 + 200 - 17, 100 + 7, Color.White);

                        // Пункты меню с равными отступами
                        Otrisovka.Write("Colors", 110, 135, Color.Lavender);
                        Otrisovka.Write("PC Information", 110, 155, Color.Lavender);
                        Otrisovka.Write("Hardware Monitor", 110, 175, Color.Lavender);
                        Otrisovka.Write("Disk Manager", 110, 195, Color.Lavender);

                        if (Click(110, 135, 48, 16)) { op = "colors"; }
                        if (Click(110, 155, 112, 16)) { op = "pcinfo"; }
                        if (Click(110, 175, 128, 16)) { if (istaskmgr == true) { istaskmgr = false; } else { istaskmgr = true; } }
                        if (Click(110, 195, 96, 16)) { op = "diskman"; }
                        if (Click(100 + 200 - 22, 100 + 4, 18, 18)) { op = ""; }
                        break;
                    //====FILE MANAGER
                    case "filem":
                        Notepad.EnterNote();
                        //if (Notepad.IsEditorActive)
                        //{
                        //    klavaypr.On = false;
                        //}
                        break;
                    //===COLOR CHANGER
                    case "colors":
                        // Единый стиль окна (270x120)
                        canvas.DrawFilledRectangle(Color.FromArgb(128, 0, 128), 30, 60, 270, 120);
                        canvas.DrawFilledRectangle(Color.FromArgb(45, 0, 45), 32, 62, 266, 116);
                        canvas.DrawFilledRectangle(Color.FromArgb(75, 0, 130), 32, 62, 266, 25);
                        Otrisovka.Write("Color Settings", 40, 67, Color.Lavender);

                        // Кнопка закрытия
                        canvas.DrawFilledRectangle(Color.FromArgb(220, 20, 60), 30 + 270 - 22, 60 + 4, 18, 18);
                        Otrisovka.Write("X", 30 + 270 - 17, 60 + 7, Color.White);

                        // Цветовые кнопки в две строки
                        canvas.DrawFilledRectangle(Color.MidnightBlue, 40, 95, 50, 20);
                        canvas.DrawFilledRectangle(Color.Navy, 100, 95, 50, 20);
                        Otrisovka.Write("W/P", 100, 100, Color.SkyBlue);
                        canvas.DrawFilledRectangle(Color.DarkSlateGray, 160, 95, 50, 20);
                        canvas.DrawFilledRectangle(Color.Black, 220, 95, 50, 20);
                        canvas.DrawFilledRectangle(Color.Teal, 40, 125, 50, 20);
                        canvas.DrawFilledRectangle(Color.FromArgb(75,0,130), 100, 125, 50, 20);
                        canvas.DrawFilledRectangle(Color.DarkRed, 160, 125, 50, 20);
                        canvas.DrawFilledRectangle(Color.DarkGreen, 220, 125, 50, 20);

                        Otrisovka.Write("Click: Wallpaper", 40, 150, Color.White);
                        Otrisovka.Write("Middle: Panel", 180, 150, Color.White);

                        if (Click(40, 95, 50, 20)) { colora = Color.MidnightBlue; }
                        if (Click(100, 95, 50, 20)) { if (oboi == true) { oboi = false; } else { oboi = true; } }
                        if (Click(160, 95, 50, 20)) { colora = Color.DarkSlateGray; }
                        if (Click(220, 95, 50, 20)) { colora = Color.Black; }
                        if (Click(40, 125, 50, 20)) { colora = Color.Teal; }
                        if (Click(100, 125, 50, 20)) { colora = Color.FromArgb(75, 0, 130); }
                        if (Click(160, 125, 50, 20)) { colora = Color.DarkRed; }
                        if (Click(220, 125, 50, 20)) { colora = Color.DarkGreen; }

                        if (ClickMiddle(40, 95, 50, 20)) { colorOfPanel = Color.MidnightBlue; }
                        if (ClickMiddle(160, 95, 50, 20)) { colorOfPanel = Color.DarkSlateGray; }
                        if (ClickMiddle(220, 95, 50, 20)) { colorOfPanel = Color.Black; }
                        if (ClickMiddle(40, 125, 50, 20)) { colorOfPanel = Color.Teal; }
                        if (ClickMiddle(100, 125, 50, 20)) { colorOfPanel = Color.FromArgb(75, 0, 130); }
                        if (ClickMiddle(160, 125, 50, 20)) { colorOfPanel = Color.DarkRed; }
                        if (ClickMiddle(220, 125, 50, 20)) { colorOfPanel = Color.DarkGreen; }

                        if (Click(30 + 270 - 22, 60 + 4, 18, 18)) { op = ""; }
                        break;
                    //====CALCULATOR
                    case "calc":
                        Calc.Calculator();
                        break;
                    //====DISK MAMAGER
                    case "diskman":
                        try
                        {
                            canvas.DrawFilledRectangle(Color.Purple, 10, 60, 500, 300);
                            canvas.DrawFilledRectangle(Color.FromArgb(60, 0, 60), 12, 62, 496, 296);

                            Otrisovka.Write("Disk Information Manager", 20, 65, Color.White);

                            var disks = VFSManager.GetDisks();
                            int yPos = 85;

                            for (int i = 0; i < disks.Count; i++)
                            {
                                var disk = disks[i];
                                string diskStatus = $"Disk {i}: ";

                                if (disk.Partitions.Count > 0)
                                {
                                    var partition = disk.Partitions[0];
                                    diskStatus += $"{(partition.Host.BlockSize / 1024 / 1024)* partition.Host.BlockCount}MB, ";
                                    diskStatus += $"Has FS: {(partition.HasFileSystem ? "Yes" : "No")}, ";
                                    diskStatus += $"MBR: {disk.IsMBR}";

                                    // Подсвечиваем текущий выбранный диск
                                    Color diskColor = (i == globaldskcnt) ? Color.LightGreen : Color.White;
                                    Otrisovka.Write(diskStatus, 20, yPos, diskColor);
                                }
                                else
                                {
                                    diskStatus += "No partitions";
                                    Otrisovka.Write(diskStatus, 20, yPos, Color.Gray);
                                }

                                yPos += 16;
                            }

                            // Информация о текущем диске
                            if (globaldskcnt < disks.Count)
                            {
                                var currentDisk = disks[globaldskcnt];
                                yPos += 10;

                                Otrisovka.Write($"Current Disk: {globaldskcnt}", 20, yPos, Color.Yellow);
                                yPos += 16;
                                Otrisovka.Write($"Size: {currentDisk.Size / 1024 / 1024}MB", 20, yPos, Color.White);
                                yPos += 16;
                                Otrisovka.Write($"Partitions: {currentDisk.Partitions.Count}", 20, yPos, Color.White);
                                yPos += 16;
                                Otrisovka.Write($"MBR: {currentDisk.IsMBR}", 20, yPos, Color.White);
                            }

                            // Кнопка обновления
                            canvas.DrawFilledRectangle(Color.SteelBlue, 400, 320, 80, 25);
                            Otrisovka.Write("Refresh", 410, 325, Color.White);

                            if (Kernel.Click(400, 320, 80, 25))
                            {
                                // Просто перерисовываем окно
                            }
                        }
                        catch (Exception ex)
                        {
                            Otrisovka.Write($"Error: {ex.Message}", 20, 85, Color.Red);
                        }
                        break;
                    //====Guess It!
                    case "guess":
                        
                        canvas.DrawFilledRectangle(Color.FromArgb(128, 0, 128), 150, 100, 375, 160);
                        canvas.DrawFilledRectangle(Color.FromArgb(45, 0, 45), 152, 102, 371, 156);
                        canvas.DrawFilledRectangle(Color.FromArgb(75, 0, 130), 152, 102, 371, 25);
                        Otrisovka.Write("Guess It!", 170, 107, Color.Lavender);

                        // Кнопка закрытия
                        canvas.DrawFilledRectangle(Color.FromArgb(220, 20, 60), 150 + 375 - 22, 100 + 4, 18, 18);
                        Otrisovka.Write("X", 150 + 375 - 17, 100 + 7, Color.White);

                        klavaypr.On = false;
                        canvas.DrawFilledRectangle(Color.Gray, 415, 175, 100, 30);
                        canvas.DrawFilledRectangle(Color.Gray, 160, 175, 200, 20);
                        Otrisovka.Write("New Number", 420, 180, Color.Black);
                        Otrisovka.Write("I made random number from 0 to 100, Guess It!", 160, 155, Color.Lavender);
                        Otrisovka.Write("New Number", 420, 180, Color.Black);
                        Otrisovka.Write("Attempts: " + att, 420, 215, Color.White);
                        sus.KeyboardManager.TryReadKey(out k);
                        Otrisovka.Write(enterrednum, 160, 180, Color.Black);
                        Otrisovka.Write(status, 160, 215, Color.White);
                        klavaypr.YPRklava(k);
                        if (Click(415, 175, 100, 30))
                        {
                            randomnum = new System.Random().Next(0, 101);
                            status = "New number generated";
                            att = 0;
                            enterrednum = "";
                        }
                        if (Click(503, 104, 18, 18))
                        {
                            klavaypr.On = true;
                            att = 0;
                            op = "";
                        }
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
                            att=0;
                            op = "";
                        }
                        else if (k.Key == ConsoleKeyy.Enter)
                        {
                            try
                            {
                                int num = Convert.ToInt32(enterrednum);
                                if (num > randomnum)
                                {
                                    status = "Guessed number less than " + num.ToString();
                                    att++;
                                }
                                else if (num < randomnum)
                                {
                                    status = "Guessed number greater than " + num.ToString();
                                    att++;
                                }
                                else if (num == randomnum)
                                {
                                    status = "You guess it! Congratulations!";
                                    att=0;
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
                        break;
                    //====FORMAT DISK
                    case "format":
                        canvas.DrawFilledRectangle(Color.LightGray, 400, 240, 100, 50); // 270, 200
                        Otrisovka.Write("FORMAT DISK?", 405, 245, Color.Black);
                        canvas.DrawFilledRectangle(Color.DarkGray, 415, 260, 35, 25);
                        Otrisovka.Write("Yes", 420, 270, Color.Black);
                        canvas.DrawFilledRectangle(Color.DarkGray, 455, 260, 35, 25);
                        Otrisovka.Write("No", 465, 270, Color.Black);
                        if (Click(415, 260, 35, 25))
                        {
                            try
                            {
                                if (fs.Disks[globaldskcnt].IsMBR)
                                {
                                    fs.Disks[globaldskcnt].Clear();
                                    fs.Disks[globaldskcnt].CreatePartition((int)fs.Disks[globaldskcnt].Size - 10);
                                    fs.Disks[globaldskcnt].FormatPartition(0, "FAT32");
                                    if (!fs.Disks[globaldskcnt].Partitions[0].HasFileSystem) { throw new Exception("Unknown FS detected."); }
                                    FpsShower.Msg("Disk formated successfully!");
                                    FpsShower.playSound = true;
                                    op = "";
                                }
                                else
                                {
                                    Setup.GPT2MBR(fs.Disks[globaldskcnt]);
                                    fs.Disks[globaldskcnt].Clear();
                                    fs.Disks[globaldskcnt].CreatePartition((int)fs.Disks[globaldskcnt].Size - 10);
                                    fs.Disks[globaldskcnt].FormatPartition(0, "FAT32");
                                    FpsShower.Msg("Disk formated successfully!", "GPT -> MBR");
                                    FpsShower.playSound = true;
                                    op = "";
                                }
                            }
                            catch (Exception e)
                            {
                                FpsShower.Msg("Critical error happened!", e.ToString(), false);
                                FpsShower.playSound = true;
                                op = "";
                            }

                        }
                        if (Click(455, 260, 35, 25))
                        {
                            op = "";
                        }
                        break;
                    //====Delete
                    case "delete":
                        canvas.DrawFilledRectangle(Color.LightGray, 400, 240, 100, 50);
                        Otrisovka.Write("Delete File?", 405, 245, Color.Black);
                        canvas.DrawFilledRectangle(Color.DarkGray, 415, 260, 35, 25);
                        Otrisovka.Write("Yes", 420, 270, Color.Black);
                        canvas.DrawFilledRectangle(Color.DarkGray, 455, 260, 35, 25);
                        Otrisovka.Write("No", 465, 270, Color.Black);

                        if (Click(415, 260, 35, 25))
                        {
                            try
                            {
                                File.Delete(Path.Combine(Notepad.CurrentPath, SelectedFileName));
                                IsFileOperationLocked = false;
                                op = "";
                            }
                            catch (Exception ex)
                            {
                                FpsShower.Msg("Delete failed", ex.Message, false);
                                FpsShower.playSound = true;
                            }
                        }
                        if (Click(455, 260, 35, 25))
                        {
                            IsFileOperationLocked = false;
                            op = "";
                        }
                        break;
                    //=====RenameFile
                    case "rename":
                        canvas.DrawFilledRectangle(Color.Gray, 305, 240, 180, 50);
                        Otrisovka.Write("Create file with name:", 310, 245, Color.Black);

                        try
                        {
                            sus.KeyboardManager.TryReadKey(out k);
                            Otrisovka.Write(SelectedFileName, 310, 260, Color.Black);
                            klavaypr.YPRklava(k);

                            if (k.Key == ConsoleKeyy.Spacebar)
                            {
                                SelectedFileName += " ";
                            }
                            else if (k.Key == ConsoleKeyy.Backspace)
                            {
                                if (SelectedFileName.Length > 0)
                                    SelectedFileName = SelectedFileName.Remove(SelectedFileName.Length - 1);
                            }
                            else if (k.Key == ConsoleKeyy.Escape)
                            {
                                SelectedFileName = "";
                                IsFileOperationLocked = false;
                                klavaypr.On = true;
                                op = "";
                            }
                            else if (k.Key == ConsoleKeyy.Enter)
                            {
                                try
                                {
                                    File.Create(Path.Combine(Notepad.CurrentPath, SelectedFileName));
                                    SelectedFileName = "test.txt";
                                    IsFileOperationLocked = false;
                                    klavaypr.On = true;
                                    op = "";
                                }
                                catch (Exception ex)
                                {
                                    FpsShower.Msg("Create failed", ex.Message, false);
                                    FpsShower.playSound = true;
                                    SelectedFileName = "";
                                    IsFileOperationLocked = false;
                                    klavaypr.On = true;
                                    op = "";
                                }
                            }
                            else
                            {
                                if (char.IsAscii(k.KeyChar) && k.KeyChar != ' ')
                                {
                                    SelectedFileName += k.KeyChar;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            FpsShower.Msg("Rename error", ex.Message, false);
                            FpsShower.playSound = true;
                        }
                        break;
                    //=====Colorfull
                    case "colorfull":
                        Colorfull.Start();
                        break;
                    //=====PragmaCode
                    case "pragmaC":
                        klavaypr.On = false;
                        PragmaCode.Pragma();
                        break;
                    //=====PragmaExecute
                    case "exec":
                        PragmaInterpritator.Execute();
                        break;
                }
                //=====KNOPKA PUSK
                if (ispysk == true)
                {
                    canvas.DrawFilledRectangle(colorOfPanel, 884, 50, 140, 140);
                    Otrisovka.Write("Settigs Menu", 889, 52, Color.White);
                    Otrisovka.Write("File Manager", 889, 68, Color.White);
                    Otrisovka.Write("Calculator", 889, 84, Color.White);
                    Otrisovka.Write("Guess It!", 889, 100, Color.White);
                    Otrisovka.Write("ColorFull", 889, 116, Color.White);
                    Otrisovka.Write("Visual Stupidio", 889, 132, Color.White);
                    canvas.DrawFilledCircle(Color.Crimson, 904, 165, 15);
                    canvas.DrawFilledCircle(Color.OrangeRed, 944, 165, 15);
                    //------------------------------
                }
                //=====HARDWARE MONITOR
                if (istaskmgr)
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
                if (NoAC97msg)
                {
                    FpsShower.Msg("AC97 sound card not found", "Speaker will be used", false);
                    FpsShower.playSound = true;
                    NoAC97msg = false;
                }
                FpsShower.msgEngine();
                try
                {
                    canvas.DrawImageAlpha(tc, (int)sus.MouseManager.X, (int)sus.MouseManager.Y);
                }
                catch (Exception) { }
                klavaypr.control();
                if (Render) { canvas.Display(); }
                Cosmos.Core.Memory.Heap.Collect();
                #endregion
            }
            catch (Exception e)
            {
                //Only for debug
                //canvas.Disable();
                //System.Console.WriteLine(e.ToString());
            }
        }
    }
}