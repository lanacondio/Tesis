using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTpMetaheuristica.Services
{
    public class LocalSearchWorker
    {
        public Matrix resultSeed;
        private static Random r = new Random();
        public LocalSearchWorker(Matrix seed)
        {
            this.resultSeed = seed;
        }

        public void Run()
        {
            var maxIterations = int.Parse(ConfigurationManager.AppSettings["MaxIterations"]);

            for (int j = 0; j < maxIterations; j++)
            {
                var localSolution = this.MakeLocalSolution(resultSeed);
                if (localSolution.IsBetter(resultSeed)) resultSeed = localSolution;                
            }
            
        }

        public Matrix MakeLocalSolution(Matrix matrix)
        {
            var result = matrix.Clone();
            int sourceIndex = r.Next(0, result.Rows.Count);
            var destIndex = r.Next(0, result.Rows.Count);
            result = result.Permute( sourceIndex, destIndex);
            return result;

        }

    }
}
