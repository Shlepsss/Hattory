using Cosmos.HAL.Audio;
using Cosmos.System.Audio.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hattory
{
    internal class FpsShower
    {
        private static DateTime lastTime;
        private static int fps = 0;
        private static int frames = 0;
        private static bool showmsg = false;
        private static string messag = "";
        private static string messag2 = "";
        private static bool typ = true;
        public static bool playSound = false;
        /// <summary>
        /// Должен ли показываться счётчик ФПС?
        /// </summary>
        public static bool ShouldRender { get; set; } = true;

        public static int FPS
        {
            get
            {
                Update();
                return fps;
            }
        }
        private static void Update()
        {
            if ((DateTime.Now - lastTime).TotalMilliseconds >= 500)
            {
                lastTime = DateTime.Now;
                fps = frames * 2;
                frames = 0;
            }
            frames++;
        }
        /// <summary>
        /// Beep with: freq - Frequency in Hz, duration in msec.
        /// </summary>
        public static void Beep(uint freq, uint duration)
        {
            Cosmos.System.PCSpeaker.Beep(freq, duration);
        }
        public static void msgEngine()
        {
            if (showmsg)
            {
                Msg(messag, messag2, typ);
            }
        }
        /// <summary>
        /// переменная type: true - сообщение, false - предупреждение
        /// </summary>
        public static void Msg(string message, string message2 = "" , bool IsInfo = true)
        {
            messag = message;
            messag2 = message2;
            typ = IsInfo;
            showmsg = true;
            if (IsInfo) { Kernel.canvas.DrawImageAlpha(Kernel.infoIcon, 398, 390);
                Kernel.canvas.DrawFilledRectangle(System.Drawing.Color.DeepSkyBlue, 390, 332, 244, 104);
                Kernel.canvas.DrawFilledRectangle(System.Drawing.Color.MidnightBlue, 392, 334, 240, 100);
                Kernel.canvas.DrawImageAlpha(Kernel.infoIcon, 398, 390);
            }
            else { Kernel.canvas.DrawImageAlpha(Kernel.warnIcon, 417, 380);
                Kernel.canvas.DrawFilledRectangle(System.Drawing.Color.Red, 390, 332, 244, 104);
                Kernel.canvas.DrawFilledRectangle(System.Drawing.Color.FromArgb(75, 0, 0), 392, 334, 240, 100);
                Kernel.canvas.DrawImageAlpha(Kernel.warnIcon, 417, 380);
            }
            Otrisovka.Write(message, 400, 345, System.Drawing.Color.White);
            Otrisovka.Write(message2, 400, 365, System.Drawing.Color.White);
            Kernel.canvas.DrawFilledRectangle(System.Drawing.Color.MediumPurple, 482, 394, 60, 30);
            Otrisovka.Write("OK", 504, 399, System.Drawing.Color.Indigo);
            if (playSound)
            {
                if (Kernel.NoAC97)
                {
                    if (IsInfo) { Beep(1150, 50); }
                    else {
                        Beep(600, 60);
                        Beep(850, 40);
                    }
                }
                else {
                    Kernel.mixer.Streams.Add(new MemoryAudioStream(new SampleFormat(AudioBitDepth.Bits16, 2, true), 44100, Kernel.chor));
                }
                playSound = false;
                
            }
            if(Kernel.Click(482, 394, 60, 30))
            {
                showmsg = false;
            }
        }
    }
}
