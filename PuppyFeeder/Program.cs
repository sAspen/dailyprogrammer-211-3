using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppyFeeder
{
    struct ExampleInputs
    {
        internal static string[] Examples = { "1 1 1 1 1 2 2 3", 
                                                "1 2 2 3 3 3 4", "1 1 2 3 3 3 3 4 5 5", 
                                                "1 1 2 3 3 3 3 4 5 5", "1 1 2 2 3 4 4 5 5 5 6 6", 
                                                "1 1 2 2 2 2 2 2 3 4 4 4 5 5 5 6 6 6 7 7 8 8 9 9 9 9 9 9 9 9" };
    }

    class Program
    {
        static void Main(string[] args)
        {
            int i = 0;
            foreach (string input in ExampleInputs.Examples)
            {
                Console.WriteLine("Input " + i + ":\n" + input);
                TreatDistributionFinder finder = new TreatDistributionFinder(input);
                TreatDistribution distribution = finder.findOptimalDistribution();
                Console.WriteLine("Output " + i + ":\n" + distribution);

                i++;
            }
        }
    }
}
