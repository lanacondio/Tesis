using ConsoleTpTesis.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleTpTesis.Services
{
    public static class GraphPrinterService
    {
        public static void Print(GraphEnvironment environment)
        {
            System.Windows.Forms.Form form = new System.Windows.Forms.Form();

            Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            //create a graph object 
            Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");

            foreach(var arc in environment.Graph.Arcs)
            {
                var label = GetTrucksWithArcInTravelLabel(arc, environment.Trucks);

                var edge = graph.AddEdge(arc.first.Id.ToString(), label, arc.second.Id.ToString());
                edge.Attr.ArrowheadAtSource = Microsoft.Msagl.Drawing.ArrowStyle.None;
                edge.Attr.ArrowheadAtTarget = Microsoft.Msagl.Drawing.ArrowStyle.None;
                edge.Attr.LineWidth = 3;                

                for (int i = 0; i< environment.Trucks.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            if(environment.Trucks[i].ArcsTravel.Where(x => x.Id == arc.Id).FirstOrDefault() != null)
                            {
                                edge.Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
                            }
                            break;
                        case 1:
                            if (environment.Trucks[i].ArcsTravel.Where(x => x.Id == arc.Id).FirstOrDefault() != null)
                            {
                                edge.Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                            }
                            break;
                        case 2:
                            if (environment.Trucks[i].ArcsTravel.Where(x => x.Id == arc.Id).FirstOrDefault() != null)
                            {
                                edge.Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;
                            }
                            break;
                        case 3:
                            if (environment.Trucks[i].ArcsTravel.Where(x => x.Id == arc.Id).FirstOrDefault() != null)
                            {
                                edge.Attr.Color = Microsoft.Msagl.Drawing.Color.Beige;
                            }
                            break;
                        case 4:
                            if (environment.Trucks[i].ArcsTravel.Where(x => x.Id == arc.Id).FirstOrDefault() != null)
                            {
                                edge.Attr.Color = Microsoft.Msagl.Drawing.Color.DarkOrange;                                
                            }
                            break;
                    }

                }
                
            }

            //var graphLabel = MakeGraphLabel(environment);
            //var glabel = new Microsoft.Msagl.Drawing.Label()
            //{
            //    Text = graphLabel,
            //    Width = 100,
            //    FontSize = 12,            
            //    FontColor = Microsoft.Msagl.Drawing.Color.Black
            //};
            //graph.Label = glabel;
            //create the graph content 
            //graph.AddEdge("A", "B");
            //graph.AddEdge("B", "C");
            //graph.AddEdge("A", "C").Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
            for(int i = 0; i< environment.Trucks.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        PrintNodes(graph, environment.Trucks[i].Travel, Microsoft.Msagl.Drawing.Color.Green, Microsoft.Msagl.Drawing.Color.White);
                        break;
                    case 1:
                        PrintNodes(graph, environment.Trucks[i].Travel, Microsoft.Msagl.Drawing.Color.Red, Microsoft.Msagl.Drawing.Color.White);
                        break;

                    case 2:
                        PrintNodes(graph, environment.Trucks[i].Travel, Microsoft.Msagl.Drawing.Color.Blue, Microsoft.Msagl.Drawing.Color.White);
                        break;

                    case 3:
                        PrintNodes(graph, environment.Trucks[i].Travel, Microsoft.Msagl.Drawing.Color.Beige, Microsoft.Msagl.Drawing.Color.PaleGreen);
                        break;
                    case 4:
                        PrintNodes(graph, environment.Trucks[i].Travel, Microsoft.Msagl.Drawing.Color.DarkOrange, Microsoft.Msagl.Drawing.Color.White);
                        break;

                }


            }


            //foreach(var node in environment.Trucks.Count)
            //{

            //    //graph.FindNode(node.Id.ToString()).Attr.FillColor = NewColor(Microsoft.Msagl.Drawing.Color.Xex(node.Id % 10));                
            //}


            //c.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
            //c.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Diamond;
            ////bind the graph to the viewer 
            viewer.Graph = graph;
            //associate the viewer with the form 
            form.SuspendLayout();
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(viewer);
            form.ResumeLayout();

            viewer.MouseUp += Viewer_MouseUp;

            //show the form 
            form.ShowDialog();
        }


        private static string MakeGraphLabel(GraphEnvironment environment)
        {
            var result = string.Empty;
            result += "Result Environment:";
            result += "--------------------\n";
            result += "Total de semillas generadas:";
            result += environment.SeedsCount.ToString();
            result += "Total de busquedas locales:";
            result += environment.LocalIterationsCount;
            result += "Promedio de busquedas locales:";
            result += environment.LocalIterationsAverage;

            return result;
        }

        private static string  GetTrucksWithArcInTravelLabel(Arc arc,IList<Truck> trucks)
        {            
            var result = string.Empty;
            var description = string.Format("Cost: {0} - Demand: {1}", arc.Cost.ToString(), arc.Demand.ToString()) + Environment.NewLine;

            foreach(var truck in trucks)
            {
                if (truck.ArcsTravel.Where(x => x.Id == arc.Id).FirstOrDefault() != null)
                {
                    if (string.IsNullOrEmpty(result))
                    {
                        result = truck.Id.ToString();
                    }
                    else
                    {
                        result += "," + truck.Id.ToString();
                    }
                }
            }


            return description +"- Trucks:"+result;
        }

        private static void PrintNodes(Microsoft.Msagl.Drawing.Graph graph, List<Node> nodes, Microsoft.Msagl.Drawing.Color color, Microsoft.Msagl.Drawing.Color fontcolor)
        {
            foreach(var node in nodes)
            {
                graph.FindNode(node.Id.ToString()).Attr.FillColor = color;
                graph.FindNode(node.Id.ToString()).Attr.Shape = Microsoft.Msagl.Drawing.Shape.Box;
                graph.FindNode(node.Id.ToString()).Attr.XRadius = 10;
                graph.FindNode(node.Id.ToString()).Attr.YRadius = 10;
                graph.FindNode(node.Id.ToString()).Attr.LineWidth = 1;
                graph.FindNode(node.Id.ToString()).Attr.LabelMargin = 10;
                //graph.FindNode(node.Id.ToString()).Attr.Padding = 10;
                graph.FindNode(node.Id.ToString()).Label.FontColor = fontcolor;
            }

        }

        private static Microsoft.Msagl.Drawing.Color NewColor(string color)
        {
            Color sysColor;
            ColorConverter cvtColor = new ColorConverter();
            sysColor = (Color) cvtColor.ConvertFromString(color);
            var thisColor = new Microsoft.Msagl.Drawing.Color(sysColor.R, sysColor.B, sysColor.G);            
            return thisColor;
        }

        
        private static void Viewer_MouseUp(object sender, MouseEventArgs e)
        {
            var gviewer = (Microsoft.Msagl.GraphViewerGdi.GViewer)sender;
            var dnode = gviewer.ObjectUnderMouseCursor as Microsoft.Msagl.GraphViewerGdi.DNode;
            if (dnode == null) return;
            if (dnode.Node.LabelText == "C")
                MessageBox.Show("C is clicked");
        }

    }
}
