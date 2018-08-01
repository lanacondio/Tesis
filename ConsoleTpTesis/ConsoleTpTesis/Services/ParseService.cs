using ConsoleTpTesis.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTpTesis.Services
{
    public class ParseService
    {

        public static GraphEnvironment ParseInput(string path)
        {
            //load
            var dataName = string.Empty;
            int nodesQuantity;
            int edgesQuantity;
            int capacity;
            int timeLimit;
            var trucksQuantity = int.Parse(ConfigurationManager.AppSettings["TruckQuantity"]);
            
            var graph = new Graph() { Arcs = new List<Arc>(), Nodes = new List<Node>() };
            var trucks = new List<Truck>();

            for (int i = 0; i < trucksQuantity; i++)
            {
                var truck = new Truck()
                {
                    Id = i
                };
                trucks.Add(truck);
            }

            Console.Write("loading file from: " + path);
            using (var reader = new StreamReader(path))
            {
                int lineNumber = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line != string.Empty)
                    {
                        var data = line.Split(' ');
                        if (lineNumber > 5)
                        {
                            var arc = ParseArc(data[0], graph);
                            arc.Cost = int.Parse(data[2]);
                            arc.Demand = int.Parse(data[4]);                          
                            arc.Profit = (int)Math.Round(double.Parse(data[6]));
                            graph.Arcs.Add(arc);

                        }
                        else
                        {
                            switch (lineNumber)
                            {
                                case 0:
                                    dataName = data[1];
                                    break;
                                case 1:
                                    nodesQuantity = int.Parse(data[3]);
                                    for (int i = 1; i <= nodesQuantity; i++)
                                    {
                                        graph.Nodes.Add(new Node()
                                        {
                                            Id = i
                                        });
                                    }
                                    break;
                                case 2:
                                    edgesQuantity = int.Parse(data[3]);
                                    break;
                                case 3:
                                    capacity = int.Parse(data[1]);
                                    trucks.ForEach(x => x.Capacity = capacity);
                                    break;
                                case 4:
                                    timeLimit = int.Parse(data[2]);
                                    trucks.ForEach(x => x.TimeLimit = timeLimit);
                                    break;
                                default:
                                    break;
                            }
                        }

                    }
                    
                    lineNumber++;
                }
                
            }
            return new GraphEnvironment()
            {
                Graph = graph,
                Trucks = trucks
            };

        }


        public static Arc ParseArc(string tuple, Graph graph)
        {
            var end = tuple.Remove(0, 1);
            var numbers = end.Remove(end.Length - 1, 1).Split(',');
            var firstNumber = int.Parse(numbers[0]);
            var secondNumber = int.Parse(numbers[1]);
            var firstNode = graph.Nodes.Where(x => x.Id == firstNumber).FirstOrDefault();
            var secondNode = graph.Nodes.Where(x => x.Id == secondNumber).FirstOrDefault();
            var arc = new Arc() { first = firstNode, second = secondNode };
            return arc;
        }

    }

}
