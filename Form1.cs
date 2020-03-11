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
                // Read city connection file and create connection Dictionary
                Dictionary<string, Dictionary<string, float>> cityConnectedList = PlagueIncAlgorithm.PlagueInc.readInputFile(Path.Combine(Environment.CurrentDirectory, "..\\..\\txt\\InputFile.txt"));
                // Get a list of strings that contains which city infects which
                List<string> cityInfectsOthers = PlagueIncAlgorithm.PlagueInc.PlagueIncResult();
             
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
                    if (countInfectionRoute % 4 == 0)
                    {
                        resultText = resultText + "\n";
                    } else
                    {
                        resultText = resultText + " | ";
                    }
                    
                    if (cityConnectedList[cityInfectsSplit[0]].ContainsKey(cityInfectsSplit[1]))
                    {
                        // Add connection to Graph and give it color red as a sign for infected
                        graph.AddEdge(cityInfectsSplit[0], cityInfectsSplit[1]).Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                        graph.FindNode(cityInfectsSplit[0]).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
                        graph.FindNode(cityInfectsSplit[1]).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;

                        // Delete connection from connection dictionary
                        cityConnectedList[cityInfectsSplit[0]].Remove(cityInfectsSplit[1]);
                    }
                    
                }

                foreach (KeyValuePair<string, Dictionary<string, float>> connectionDict in cityConnectedList)
                    // Add uninfected connection and cities to the graph
                {
                    
                        foreach (KeyValuePair<string, float> targetCity in connectionDict.Value)
                        {
                            graph.AddEdge(connectionDict.Key, targetCity.Key);
                        }
                    
                    graph.FindNode(connectionDict.Key).Attr.Shape = Microsoft.Msagl.Drawing.Shape.Circle;
                    // Change all nodes to circle shape
                }

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

            // Menampilkan jalur persebaran di GUI
            LabelTest.Text = resultText;
        }
    }
}
