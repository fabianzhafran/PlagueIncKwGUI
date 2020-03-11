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

            InputDaysBox.Height = 200;
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
                
                Dictionary<string, Dictionary<string, float>> cityConnectedList = PlagueIncAlgorithm.PlagueInc.readInputFile(Path.Combine(Environment.CurrentDirectory, "..\\..\\txt\\InputFile.txt"));
                List<string> cityInfectsOthers = PlagueIncAlgorithm.PlagueInc.PlagueIncResult();
             
                //foreach (KeyValuePair<string, List<string>> infectionDict in cityInfectsOthers)
                //{
                  //  resultText = resultText + " - " + infectionDict.Key;
                //}
                Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
                //create a graph object 
                Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");
                //create the graph content 

                int countInfectionRoute = 0;
                foreach (string cityInfects in cityInfectsOthers)
                {
                    countInfectionRoute++;
                    string[] cityInfectsSplit;
                    cityInfectsSplit = cityInfects.Split(' ');
                    resultText = resultText + cityInfectsSplit[0] + "=>" + cityInfectsSplit[1];
                    if (countInfectionRoute % 2 == 0)
                    {
                        resultText = resultText + "\n";
                    } else
                    {
                        resultText = resultText + " | ";
                    }
                    

                    graph.AddEdge(cityInfectsSplit[0], cityInfectsSplit[1]).Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                    graph.FindNode(cityInfectsSplit[0]).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
                    graph.FindNode(cityInfectsSplit[1]).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;

                    cityConnectedList[cityInfectsSplit[0]].Remove(cityInfectsSplit[1]);
                }

                //foreach(KeyValuePair<string, List<string>> infectionDict in cityInfectsOthers)
                //{
                //foreach(string victimCity in infectionDict.Value)
                //{
                //graph.AddEdge(infectionDict.Key, victimCity).Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                //graph.FindNode(infectionDict.Key).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
                //graph.FindNode(victimCity).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
                //}
                //}

                foreach (KeyValuePair<string, Dictionary<string, float>> connectionDict in cityConnectedList)
                {
                    {
                        foreach (KeyValuePair<string, float> targetCity in connectionDict.Value)
                        {

                            graph.AddEdge(connectionDict.Key, targetCity.Key);
                        }
                    }
                    graph.FindNode(connectionDict.Key).Attr.Shape = Microsoft.Msagl.Drawing.Shape.Circle;
                }
                // graph.AddEdge("A", "B");
                // graph.AddEdge("B", "C");
                // graph.AddEdge("A", "C").Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
                // graph.FindNode("A").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;
                // graph.FindNode("B").Attr.FillColor = Microsoft.Msagl.Drawing.Color.MistyRose;
                // Microsoft.Msagl.Drawing.Node c = graph.FindNode("C");
                // c.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
                // c.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Diamond;
                //bind the graph to the viewer 
                viewer.Graph = graph;
                //associate the viewer with the form 
                this.SuspendLayout();
                viewer.Dock = System.Windows.Forms.DockStyle.Fill;
                this.Controls.Add(viewer);
                this.ResumeLayout();

                
                
            }
            else
            {
                resultText = "Inputnya yang bener dong cok";
            }
            LabelTest.Text = resultText;
        }
    }
}
