using ConsoleTpMetaheuristica.Services;
using ConsoleTpTesis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTpMetaheuristica
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var path = args[0];
            var lastBestResult = 0;
            if (args.Length > 1)
            {
                lastBestResult = int.Parse(args[1]);
            }
            try
            {
                var environment = ParseService.ParseInput(path);
                DijkstraShortestRouteService.CalculateShortestRoute(environment.Graph);
                GraspService GraspService = new GraspService();
                GraspService.GetResult(environment);
                Console.WriteLine("\n");
                Console.WriteLine("Accumulated Profit: " + environment.AccumulatedProfit.ToString());
                
                foreach(var truck in environment.Trucks)
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("Truck" + truck.Id+" :");
                    truck.Travel.ForEach(x => Console.WriteLine(x.Id.ToString()));

                }
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            
        }
    }
}
