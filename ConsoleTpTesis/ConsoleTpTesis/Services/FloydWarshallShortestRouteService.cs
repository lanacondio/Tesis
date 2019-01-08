using ConsoleTpTesis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTpTesis.Services
{
    public static class FloydWarshallShortestRouteService
    {
        public static void CalculateShortestRoute(Graph graph)
        {
            var floydWarshall = new FloydWarshall();
            floydWarshall.Run(graph, graph.Nodes.Count);
        }
    }
}
