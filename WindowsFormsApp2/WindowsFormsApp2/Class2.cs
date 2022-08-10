using AutoIt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    class Class2
    {
        private string str = "";
        Control control;
        TextBox textBox1;
        Rectangle windowRectangle;
        public Class2(string str, Control control, TextBox textBox, Rectangle windowRectangle)
        {
            this.str = str;
            this.control = control;
            this.textBox1 = textBox;
            this.windowRectangle = windowRectangle;
        }

        public string getAction()
        {
            return str;
        }

        public void function1()
        {
            if (str == "Start Fish")
            {
                AutoItX.Send("{F7}");
                Thread.Sleep(100);
                AutoItX.Send("{Enter}");
                Thread.Sleep(1000);
                if (selectFish() == true)
                {
                    AutoItX.Send("{Enter}");
                }
                else
                {
                    AutoItX.Send("{ESC}");
                }
            }
            else if (str == "Fish Game")
            {
                Thread.Sleep(500);
                string caughtHookImage = "";
                Rectangle caughtHookRectangle = new Rectangle();
                if (windowRectangle.Width == 1382)
                {
                    caughtHookImage = @".\images\caught_hook.png";
                    caughtHookRectangle = new Rectangle(0, 665, 650, 715);
                }
                else if (windowRectangle.Width == 1936)
                {
                    caughtHookImage = @".\images\caught_hook_1920_1080.png";
                    caughtHookRectangle = new Rectangle(0, 990, 260, 1015);
                }
                else
                {
                    return;
                }
                Rectangle rectangle = getWindowRectangle();
                if (isFishingGaugeOnScreen(rectangle) == true)
                {
                    string[] hookResults = UseImageSearch(caughtHookRectangle.X, caughtHookRectangle.Y, caughtHookRectangle.Width, caughtHookRectangle.Height, "30", caughtHookImage);
                    if (hookResults == null)
                    {
                        AutoItX.Send("{ESC}");
                    }
                    else
                    {

                    }

                    while (isFishingGaugeOnScreen(rectangle) == true)
                    {
                        string[] arrowResults = UseImageSearch((int)(rectangle.Width * .21), (int)(rectangle.Height * .29), (int)(rectangle.Width * .37), (int)(rectangle.Height * .53), "3", @".\images\gold-arrow-pixel.png");
                        if (arrowResults == null)
                        {
                            // Nothing to process
                        }
                        else
                        {
                            AutoItX.Send("{a}");
                        }

                        arrowResults = UseImageSearch((int)(rectangle.Width * .6), (int)(rectangle.Height * .29), (int)(rectangle.Width * .8), (int)(rectangle.Height * .53), "3", @".\images\gold-arrow-pixel.png");
                        if (arrowResults == null)
                        {
                            // Nothing to process
                        }
                        else
                        {
                            AutoItX.Send("{d}");
                        }

                        arrowResults = UseImageSearch((int)(rectangle.Width * .21), (int)(rectangle.Height * .29), (int)(rectangle.Width * .37), (int)(rectangle.Height * .53), "3", @".\images\silver-arrow-pixel.png");
                        if (arrowResults == null)
                        {
                            // Nothing to process
                        }
                        else
                        {
                            AutoItX.Send("{a}");
                        }

                        arrowResults = UseImageSearch((int)(rectangle.Width * .6), (int)(rectangle.Height * .29), (int)(rectangle.Width * .8), (int)(rectangle.Height * .53), "3", @".\images\silver-arrow-pixel.png");
                        if (arrowResults == null)
                        {
                            // Nothing to process
                        }
                        else
                        {
                            AutoItX.Send("{d}");
                        }
                    }
                    AutoItX.Send("{Enter}");
                }
            }
        }

        private bool selectFish()
        {
            string fishImage = "";
            Rectangle windowRectangle = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            Rectangle rectangle = new Rectangle();
            if (windowRectangle.Width == 1382)
            {
                rectangle = new Rectangle(0, 450, 200, 470);
                fishImage = @".\images\fish.png";
            }
            else if (windowRectangle.Width == 1936)
            {
                rectangle = new Rectangle(0, 750, 200, 780);
                fishImage = @".\images\fish_1920_1080.png";
            }
            else
            {
                appendText("Could not assign rectangle for isFishingReady()");
                return false;
            }

            string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "30", fishImage);
            if (results == null)
            {
                int i = 0;
                bool found = false;
                while (results == null && i < 7)
                {
                    AutoItX.Send("{down}");
                    Thread.Sleep(250);
                    results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "30", fishImage);
                    if (results == null)
                    {

                    }
                    else
                    {
                        found = true;
                    }
                    i++;
                }
                if (found == true)
                {
                    appendText("Found fish in menu");
                }
                else
                {
                    appendText("Did not find fish in menu");
                }
                return found;
            }
            else
            {
                // Nothing to process, the action menu is in the state I want it in
                appendText("Found fish in menu");
                return true;
            }
        }

        Rectangle getWindowRectangle()
        {
            Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            return windowDimensionInfo;
        }

        bool isFishingGaugeOnScreen(Rectangle rectangle)
        {
            int width = rectangle.Width;
            int height = rectangle.Height;
            string[] fishGaugeResults = UseImageSearch((int)(width / 3), 0, width - (int)(width / 3), (int)(height / 2), "10", @".\images\fish-gauge-pixel.png");
            if (fishGaugeResults == null)
            {
                appendText("Did not see fishing gauge");
                return false;
            }
            else
            {
                appendText("Found fishing gauge");
                return true;
            }
        }

        [DllImport(@".\lib\ImageSearchDLL.dll")]
        public static extern IntPtr ImageSearch(int x, int y, int right, int bottom, [MarshalAs(UnmanagedType.LPStr)] string imagePath);

        public static string[] UseImageSearch(int left, int top, int right, int bottom, string tolerance, string imgPath)
        {
            imgPath = "*" + tolerance + " " + imgPath;

            IntPtr result = ImageSearch(left, top, right, bottom, imgPath);
            string res = Marshal.PtrToStringAnsi(result);

            if (res[0] == '0') return null;

            string[] data = res.Split('|');

            int x; int y;
            int.TryParse(data[1], out x);
            int.TryParse(data[2], out y);

            return data;
        }

        private void appendText(string s)
        {
            control.BeginInvoke((MethodInvoker)delegate ()
            {
                DateTime timestamp = DateTime.Now;
                textBox1.AppendText(timestamp.ToString() + " " + s);
                textBox1.AppendText(Environment.NewLine);
            });
        }
    }
}
