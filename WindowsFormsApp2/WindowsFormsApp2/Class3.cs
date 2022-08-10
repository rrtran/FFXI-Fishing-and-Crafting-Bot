using AutoIt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    class Class3
    {
        private string str;
        private Control control;
        private TextBox textBox1;
        public Class3(string str, Control control, TextBox textBox)
        {
            this.str = str;
            this.control = control;
            this.textBox1 = textBox;
        }

        public string getAction()
        {
            return str;
        }

        public void function1()
        {
            if (str == "Start Craft")
            {
                AutoItX.Send("{Enter}");
                Thread.Sleep(1000);
                AutoItX.Send("{Enter}");
                Thread.Sleep(1000);
                AutoItX.Send("{Enter}");
                Thread.Sleep(1000);
                AutoItX.Send("{Enter}");
            }
            else if (str == "Dismiss Results")
            {
                AutoItX.Send("{ESC}");
            }
        }
    }
}
