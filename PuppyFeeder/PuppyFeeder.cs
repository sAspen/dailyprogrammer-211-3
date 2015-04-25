using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppyFeeder
{
    public class TreatDistribution : IEquatable<TreatDistribution>
    {
        internal string Distribution;
        internal int Happiness;
        internal uint PuppiesFed;
        private uint[] Treats;

        public TreatDistribution(uint[] treats)
        {
            Distribution = "";
            Happiness = 0;
            PuppiesFed = 0;

            Treats = (uint[])treats.Clone();
        }

        public TreatDistribution(string distribution, uint[] treats, uint newest)
        {
            Distribution = distribution + newest;
            PuppiesFed = (uint)Distribution.Length;

            Treats = (uint[])treats.Clone();
            
            Treats[newest]--;

            CalculateHappiness();
        }

        private void CalculateHappiness()
        {
            Happiness = CalculateHappiness(Distribution);
        }

        public static int CalculateHappiness(string distribution)
        {
            int ret = 0;

            for (int i = 0; i < distribution.Length; i++)
            {
                uint cur = (uint)Char.GetNumericValue(distribution[i]);
                int mood = 0;
                int neighbors = 0;
                if (i - 1 >= 0)
                {
                    neighbors++;

                    uint left = (uint)Char.GetNumericValue(distribution[i - 1]);
                    if (cur > left)
                    {
                        mood++;
                    }
                    else if (cur < left)
                    {
                        mood--;
                    }
                }
                if (i + 1 < distribution.Length)
                {
                    neighbors++;

                    uint right = (uint)Char.GetNumericValue(distribution[i + 1]);
                    if (cur > right)
                    {
                        mood++;
                    }
                    else if (cur < right)
                    {
                        mood--;
                    }
                }

                if (Math.Abs(mood) == neighbors)
                {
                    if (mood < 0)
                    {
                        ret--;
                    }
                    else if (mood > 0)
                    {
                        ret++;
                    }
                }
            }

            return ret;
        }

        public Stack<TreatDistribution> GenerateSuccessors()
        {
            Stack<TreatDistribution> successors = new Stack<TreatDistribution>();

            for (uint i = 0; i < Treats.Length; i++)
            {
                if (Treats[i] == 0)
                {
                    continue;
                }

                successors.Push(new TreatDistribution(Distribution, Treats, i));
            }

            return successors;
        }
        
        public override string ToString()
        {
            string s = "";

            s += Happiness + "\n";
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
    }

    public class TreatDistributionFinder
    {
        private string ParameterDistribution;
        private uint NumberOfPuppies;
        private uint[] Treats;

        public TreatDistributionFinder(string parameterDistribution)
        {
            ParameterDistribution = parameterDistribution;
            
            string[] tokens = parameterDistribution.Split();
            NumberOfPuppies = (uint)tokens.Length;

            uint max = 0;
            foreach (string s in tokens)
            {
                max = Math.Max(max, Convert.ToUInt32(s));
            }

            Treats = new uint[max + 1];


            foreach (string s in tokens)
            {
                Treats[Convert.ToUInt32(s)]++;
            }

            
        }

        public TreatDistribution findOptimalDistribution()
        {
            HashSet<TreatDistribution> closed = new HashSet<TreatDistribution>();
            Stack<TreatDistribution> candidates = new Stack<TreatDistribution>();
            TreatDistribution distribution = new TreatDistribution(Treats);
            candidates.Push(distribution);

            TreatDistribution best = distribution;
            while (candidates.Count > 0)
            {
                //distribution = candidates.OrderByDescending(c => c.Happiness).ToList().First();
                distribution = candidates.Pop();

                if (distribution.PuppiesFed == NumberOfPuppies && distribution.Happiness > best.Happiness)
                {
                    best = distribution;
                }

                closed.Add(distribution);

                Stack<TreatDistribution> successors = distribution.GenerateSuccessors();
                while (successors.Count > 0)
                {
                    TreatDistribution d = successors.Pop();
                    if (!closed.Contains(d))
                    {
                        candidates.Push(d);
                    }
                }

                //candidates.AddRange(distribution.GenerateSuccessors().Except(closed));
            }

            return best;
        }
    }
}
