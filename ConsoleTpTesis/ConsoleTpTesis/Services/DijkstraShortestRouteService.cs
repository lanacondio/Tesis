using ConsoleTpTesis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTpTesis.Services
{
    public static class DijkstraShortestRouteService
    {
        public static void CalculateShortestRoute(Graph graph)
        {
            var dijkstra = new Dijkstra();
            dijkstra.Run(1, graph);
        }
        
        
    }
}
