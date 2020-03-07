using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PlagueIncGUI
{
    public partial class Form1 : Form
    {

        public static int inputDays = 10;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
                int left,
                int top,
                int right,
                int bottom,
                int width,
                int height
        );

        public Form1()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 50, 50));
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 30, 30));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            InputDaysBox.Height = 100;
            InputDaysBox.Width = 100;
        }

        private void RoundTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void button_WOC1_Click(object sender, EventArgs e)
        {
            string resultText = "";
            bool tryParse = Int32.TryParse(InputDaysBox.Text, out inputDays);
            if (tryParse)
            {
                List<string> resultList = PlagueIncAlgorithm.PlagueInc.PlagueIncResult();
                foreach (string temp in resultList)
                {
                    resultText = resultText + " - " + temp;
                }
            }
            else
            {
                resultText = "Inputnya yang bener dong cok";
            }
            LabelTest.Text = resultText;
        }
    }
}
