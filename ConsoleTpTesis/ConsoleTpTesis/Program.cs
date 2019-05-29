using ConsoleTpMetaheuristica.Services;
using ConsoleTpTesis.Services;
using System;
using System.Configuration;

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

                FloydWarshallShortestRouteService.CalculateShortestRoute(environment.Graph);
                
                GraspService GraspService = new GraspService();

                var result = GraspService.GetResult(environment);
                Console.WriteLine("Result Environment:");
                Console.WriteLine("--------------------\n");
                Console.WriteLine("Total de semillas generadas:"+ result.SeedsCount.ToString());
                Console.WriteLine("Total de busquedas locales:"+ result.LocalIterationsCount.ToString());                
                Console.WriteLine("Promedio de busquedas locales:"+ result.LocalIterationsAverage.ToString());                
                Console.WriteLine("Acumulated Profit:"+ result.AccumulatedProfit.ToString());

                var waitenter = bool.Parse(ConfigurationManager.AppSettings["WaitEnter"]);

                result.PrintResume();

                if (waitenter)
                {
                    Console.ReadLine();
                }
                

                
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            
        }
    }
}
