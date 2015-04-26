using System;
using System.Collections.Generic;
using System.Linq;

namespace PuppyFeeder
{
    static class Extensions
    {
        private static Random RNG;

        static Extensions()
        {
            RNG = new Random();
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static void Shuffle<T>(this T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = RNG.Next(n + 1);
                T value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }

        public static T[] CopyShuffle<T>(this T[] array)
        {
            T[] shuffled = (T[])array.Clone();
            int n = shuffled.Length;
            while (n > 1)
            {
                n--;
                int k = RNG.Next(n + 1);
                T value = shuffled[k];
                shuffled[k] = shuffled[n];
                shuffled[n] = value;
            }
            return shuffled;
        }
    }

    public class TreatDistribution : IEquatable<TreatDistribution>, ICloneable
    {
        internal char[] Distribution;
        internal int Happiness;
        internal int Fitness;
        private bool GoodLeftEdge, GoodRightEdge;

        public TreatDistribution(char[] distribution)
        {
            Distribution = distribution.Clone() as char[];

            GoodLeftEdge = GoodRightEdge = false;

            Recalculate();
        }

        internal void Recalculate()
        {
            Happiness = CalculateHappiness();


            int goodEdgeAdvantage = 0;
            Fitness = Happiness + Distribution.Length + 1;
            if (GoodLeftEdge)
            {
                Fitness += goodEdgeAdvantage;
            }
            if (GoodRightEdge)
            {
                Fitness += goodEdgeAdvantage;
            }
        }

        private int CalculateHappiness()
        {
            int ret = 0;

            for (int i = 0; i < Distribution.Length; i++)
            {
                uint cur = (uint)Char.GetNumericValue(Distribution[i]);

                uint? left = ((i - 1 >= 0) ? (uint)Char.GetNumericValue(Distribution[i - 1]) : (uint?)null);
                uint? right = ((i + 1 < Distribution.Length) ? (uint)Char.GetNumericValue(Distribution[i + 1]) : (uint?)null);

                if (left != null && right != null)
                {
                    if (cur > left && cur > right)
                    {
                        ret++;
                    }
                    else if (cur < left && cur < right)
                    {
                        ret--;
                    }
                }
                else if (left != null)
                {
                    if (cur > left)
                    {
                        ret++;
                        GoodRightEdge = true;
                    }
                    else if (cur < left)
                    {
                        ret--;
                    }
                }
                else if (right != null)
                {
                    if (cur > right)
                    {
                        ret++;
                        GoodLeftEdge = true;
                    }
                    else if (cur < right)
                    {
                        ret--;
                    }
                }
            }

            return ret;
        }

        public override string ToString()
        {
            string s = "";

            s += Happiness + ", \n";
            foreach (char c in Distribution)
            {
                s += c + " ";
            }

            s = s.Substring(0, s.Length - 1);

            return s;
        }

        public override int GetHashCode()
        {
            return Distribution.GetHashCode();
        }


        public bool Equals(TreatDistribution other)
        {
            // Check whether the compared object is null.
            if (Object.ReferenceEquals(other, null)) return false;

            // Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, other)) return true;

            // Check whether the objects’ properties are equal.
            return Distribution.Equals(other.Distribution);
        }

        public object Clone()
        {
            return new TreatDistribution(Distribution) as object;
        }
    }

    public class TreatDistributionFinder
    {
        private char[] ParameterDistribution;
        private uint[] TreatCounts;
        private Random RNG;

        public TreatDistributionFinder(string parameterDistribution)
        {
            string[] tokens = parameterDistribution.Split();

            ParameterDistribution = new char[tokens.Length];

            uint max = 0;
            int i = 0;
            foreach (string s in tokens)
            {
                ParameterDistribution[i++] = s[0];
                uint j = (uint)Char.GetNumericValue(s[0]);
                max = Math.Max(max, j);
            }

            TreatCounts = CountTreats(ParameterDistribution, max + 1);

            RNG = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
        }

        private static uint[] CountTreats(char[] distribution, uint max)
        {
            uint[] treatCounts = new uint[max];
            foreach (char c in distribution)
            {
                int i = (int)Char.GetNumericValue(c);
                treatCounts[i]++;
            }

            treatCounts[0] = 0;
            return treatCounts;
        }

        private TreatDistribution SelectParent(List<TreatDistribution> population)
        {
            int fitnessSum = population.Sum(d => d.Fitness);
            int target = RNG.Next(fitnessSum);

            int sum = 0;
            foreach (TreatDistribution d in population)
            {
                sum += d.Fitness;
                if (sum > target)
                {
                    return d;
                }
            }

            return population.First();
        }

        private char[] createOffspring(TreatDistribution parent1, TreatDistribution parent2)
        {
            TreatDistribution[] parents = { parent1, parent2 };
            char[] chromosomes = new char[parent1.Distribution.Length];
            uint[] remainingTreats = TreatCounts.Clone() as uint[];



            for (int i = 0; i < chromosomes.Length; i++)
            {
                int p = RNG.Next(2);
                uint t = (uint)Char.GetNumericValue(parents[p].Distribution[i]);

                if (remainingTreats[t] > 0)
                {
                    chromosomes[i] = parents[p].Distribution[i];
                    remainingTreats[t]--;
                }
                else
                {
                    while (true)
                    {
                        int j = RNG.Next(remainingTreats.Length);
                        if (remainingTreats[j] > 0)
                        {
                            chromosomes[i] = j.ToString()[0];
                            remainingTreats[j]--;
                            break;
                        }
                    }
                }
            }

            for (int i = RNG.Next(-5, 5); i > 0; i--)
            {
                char c1, c2, temp;
                c1 = chromosomes[RNG.Next(chromosomes.Length)];
                c2 = chromosomes[RNG.Next(chromosomes.Length)];

                temp = c1;
                c1 = c2;
                c2 = c1;
            }

            return chromosomes;
        }

        public TreatDistribution RunGeneticAlgorithm()
        {
            const uint populationSize = 160, lastGeneration = 1000;

            List<TreatDistribution> population = new List<TreatDistribution>((int)populationSize);
            for (uint i = 0; i < populationSize; i++)
            {
                TreatDistribution d = new TreatDistribution(ParameterDistribution);
                population.Add(d);
            }

            foreach (TreatDistribution d in population)
            {
                d.Distribution.Shuffle();
                d.Recalculate();
            }

            TreatDistribution elite = null;
            uint generation = 0;
            List<TreatDistribution> newPopulation = new List<TreatDistribution>((int)populationSize - 1);
            while (true)
            {
                population = population.OrderByDescending(d => d.Fitness).ToList();
                newPopulation.Clear();

                if (elite == null || elite.Fitness < population.First().Fitness)
                {
                    elite = population.First();
                }
                newPopulation.Insert(0, elite.Clone() as TreatDistribution);

                if (generation++ == lastGeneration)
                {
                    return elite;
                }

                newPopulation.AddRange(population.GetRange(0, (int)(populationSize / 2 - 1)));

                for (uint i = 0; i < populationSize / 2; i++)
                {
                    TreatDistribution parent1, parent2;
                    parent1 = SelectParent(population);
                    do
                    {
                        parent2 = SelectParent(population);
                    } while (Object.ReferenceEquals(parent1, parent2));


                    newPopulation.Add(new TreatDistribution(createOffspring(parent1, parent2)));
                }

                population = newPopulation;
            }
        }


        public TreatDistribution findOptimalDistribution()
        {
            return RunGeneticAlgorithm();
        }
    }
}
