using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaCProject
{
    class Program
    {
        public static void Main(string[] args)
        {
            List<int> initialSet = new List<int>();
            int r;
            Console.WriteLine("Input weights:");
            string line = Console.ReadLine();
            string[] numbers = line.Split(' ');
            foreach(string number in numbers)
            {
                int n;
                if (int.TryParse(number, out n))
                    initialSet.Add(n);
            }
            do
            {
                Console.WriteLine("Input r:");
                line = Console.ReadLine();
            }
            while (!(int.TryParse(line, out r)));

            List<List<int>> resultBrute = BruteForce(initialSet, r);
            List<List<int>> resultHeuristic = Heuristic(initialSet, r);

            Console.WriteLine("Brute Force solution:");
            PrintSolution(resultBrute);
            Console.WriteLine("Initial fitness function value: {0}", Fitness(resultBrute));
            Console.WriteLine("Second fitness function value: {0}", MyFitness(resultBrute, r));
            Console.WriteLine();
            Console.WriteLine("Heuristic solution:");
            PrintSolution(resultHeuristic);
            Console.WriteLine("Initial fitness function value: {0}", Fitness(resultHeuristic));
            Console.WriteLine("Second fitness function value: {0}", MyFitness(resultHeuristic, r));

            Console.WriteLine();
            Console.WriteLine("Partitions:");
            List<List<List<int>>> testBrute = GeneratePartitions(initialSet, r);
            foreach(List<List<int>> l in testBrute)
            {
                PrintSolution(l);
                Console.WriteLine();
            }

        }

        public static List<List<int>> BruteForce(List<int> initialSet, int r)
        {
            List<int> initialCopy = new List<int>(initialSet);
            List<List<List<int>>> allSets = GeneratePartitions(initialCopy, r);
            int bestIndex = 0;
            int tmp = initialSet.Sum();
            foreach (List<List<int>> list in allSets)
            {
                if (Fitness(list) < tmp)
                {
                    bestIndex = allSets.IndexOf(list);
                    tmp = Fitness(list);
                }
            }
            return allSets[bestIndex];
        }

        public static List<List<List<int>>> GeneratePartitions(List<int> initialSet, int r)
        {
            List<List<List<int>>> allSets = new List<List<List<int>>>();
            if(r == 1)
            {
                List<List<List<int>>> singleSol = new List<List<List<int>>>();
                List<List<int>> l = new List<List<int>>();
                l.Add(initialSet);
                singleSol.Add(l);
                return singleSol;
            }
            else if(r == initialSet.Count())
            {
                List<List<List<int>>> singleSol = new List<List<List<int>>>();
                List<List<int>> Sp = new List<List<int>>();
                foreach(int elem in initialSet)
                {
                    List<int> p = new List<int>();
                    p.Add(elem);
                    Sp.Add(p);
                }
                singleSol.Add(Sp);
                return singleSol;
            }
            else
            {
                int s1 = initialSet.First();
                initialSet.RemoveAt(0);
                List<List<List<int>>> Sp = GeneratePartitions(initialSet, r);
                foreach(List<List<int>> p in Sp)
                {
                    foreach(List<int> sub in p)
                    {
                        List<List<int>> pp = new List<List<int>>();
                        foreach (List<int> subp in p)
                        {
                            List<int> copy = new List<int>(subp);
                            pp.Add(copy);
                        }
                        pp.ElementAt(p.IndexOf(sub)).Add(s1);
                        allSets.Add(pp);
                    }
                }
                List<List<List<int>>> Spp = GeneratePartitions(initialSet, r-1);
                foreach(List<List<int>> p in Spp)
                {
                    List<int> pp = new List<int>();
                    pp.Add(s1);
                    p.Add(pp);
                    allSets.Add(p);
                }
            }
            return allSets;
        }

        public static List<List<int>> Heuristic(List<int> initialSet, int r)
        {
            List<int> sorted = new List<int>(initialSet);
            List<List<int>> result = new List<List<int>>();
            for(int i=0; i< r; i++)
            {
                List<int> set = new List<int>();
                result.Add(set);
            }
            sorted.Sort((a, b) => b.CompareTo(a));
            foreach(int elem in sorted)
            {
                result[IndexOfMin(result)].Add(elem);
            }
            return result;
        }

        public static int IndexOfMin(List<List<int>> list)
        {
            int tmp = list.Sum(item => item.Sum());
            int index = 0;
            foreach(List<int> set in list)
            {
                if (set.Sum() < tmp)
                {
                    tmp = set.Sum();
                    index = list.IndexOf(set);
                }
            }
            return index;
        }

        public static int Fitness(List<List<int>> list)
        {
            List<int> weights = new List<int>();
            foreach(List<int> set in list)
            {
                weights.Add(set.Sum());
            }
            return weights.Max() - weights.Min();
        }

        public static int MyFitness(List<List<int>> list, int r)
        {
            int totalSum = list.Sum(item => item.Sum());
            int result = 0;
            foreach (List<int> set in list)
            {
                result += Math.Abs(set.Sum() - totalSum / r);
            }
            return result;
        }

        public static void PrintSolution(List<List<int>> solution)
        {
            Console.Write("{");
            foreach(List<int> set in solution)
            {
                Console.Write("{");
                for(int i=0; i < set.Count(); i++)
                {
                    Console.Write(set[i]);
                    if (!(set.Count() == i + 1))
                        Console.Write(", ");
                }
                Console.Write("}");
                if (!(solution.Count() == solution.IndexOf(set) + 1))
                    Console.Write(", ");
            }
            Console.Write("}");
            Console.WriteLine();
        }
    }
}
