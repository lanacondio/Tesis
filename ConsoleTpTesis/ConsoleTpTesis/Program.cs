using ConsoleTpMetaheuristica.Services;
using ConsoleTpTesis.Services;
using System;

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
                Console.WriteLine("Total de semillas generadas:");
                Console.WriteLine(result.SeedsCount);
                Console.WriteLine("Total de busquedas locales:");
                Console.WriteLine(result.LocalIterationsCount);
                Console.WriteLine("Promedio de busquedas locales:");
                Console.WriteLine(result.LocalIterationsAverage);
                Console.WriteLine("Acumulated Profit:");
                Console.WriteLine(result.AccumulatedProfit);


                result.PrintResume();

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            
        }
    }
}
