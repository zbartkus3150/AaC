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
            string line;
            int option;
            do
            {
                do
                {
                    Console.WriteLine("Input 0 for text file input or 1 for console input:");
                    line = Console.ReadLine();
                }
                while (!(int.TryParse(line, out option)));
            }
            while (option != 0 && option != 1);

            if(option == 0)
            {
                FileInput();
            }
            else
            {
                ConsoleInput();
            }

        }

        public static void ConsoleInput()
        {
            List<int> initialSet = new List<int>();
            int r;
            Console.WriteLine("Input weights:");
            string line = Console.ReadLine();
            string[] numbers = line.Split(' ');
            foreach (string number in numbers)
            {
                int n;
                if (int.TryParse(number, out n))
                    initialSet.Add(n);
            }
            do
            {
                Console.WriteLine("Input number of partitions:");
                line = Console.ReadLine();
            }
            while (!(int.TryParse(line, out r)));

            List<List<int>> resultBrute = BruteForce(initialSet, r, Fitness);
            List<List<int>> resultHeuristic = Heuristic(initialSet, r);

            Console.WriteLine("Brute Force solution:");
            PrintSolution(resultBrute);
            Console.WriteLine("Initial fitness function value: {0}", Fitness(resultBrute, r));
            Console.WriteLine("Second fitness function value: {0}", MyFitness(resultBrute, r));
            Console.WriteLine();
            Console.WriteLine("Heuristic solution:");
            PrintSolution(resultHeuristic);
            Console.WriteLine("Initial fitness function value: {0}", Fitness(resultHeuristic, r));
            Console.WriteLine("Second fitness function value: {0}", MyFitness(resultHeuristic, r));

            Console.WriteLine();
            Console.WriteLine("Partitions:");
            List<List<List<int>>> testBrute = GeneratePartitions(initialSet, r);
            foreach (List<List<int>> l in testBrute)
            {
                PrintSolution(l);
                Console.WriteLine();
            }
        } 

        public static void FileInput()
        {
            List<int> initialSet = new List<int>();
            int r;
            Console.WriteLine("Input file name:");
            string fileName = Console.ReadLine();
            string[] lines = System.IO.File.ReadAllLines(fileName);
            if(lines.Length != 4)
            {
                Console.WriteLine("Invalid input!");
            }
            else
            {
                if (!(int.TryParse(lines[0], out r)))
                {
                    Console.WriteLine("Invalid input!");
                    return;
                }
                if (lines[3].Contains(','))
                {
                    string[] weights = lines[3].Split(',');
                    foreach(string w in weights)
                    {
                        int number;
                        if (int.TryParse(w, out number))
                            initialSet.Add(number);
                    }
                }
                else
                {
                    int size;
                    if (int.TryParse(lines[3], out size))
                    {
                        Random rnd = new Random();
                        for (int i=0; i< size; i++)
                        {
                            initialSet.Add(rnd.Next(size * 4));
                        }
                    }
                }
                if(lines[1] == "o")
                {
                    List<List<int>> resultBrute = new List<List<int>>();
                    if (lines[2] == "1")
                    {
                        resultBrute = BruteForce(initialSet, r, Fitness);
                        PrintSolution(resultBrute);
                        Console.WriteLine("Initial fitness function value: {0}", Fitness(resultBrute, r));
                    }
                    else
                    {
                        resultBrute = BruteForce(initialSet, r, MyFitness);
                        PrintSolution(resultBrute);
                        Console.WriteLine("Second fitness function value: {0}", MyFitness(resultBrute, r));
                    }
                }
                else if(lines[1] == "h")
                {
                    List<List<int>> resultHeuristic = Heuristic(initialSet, r);
                    PrintSolution(resultHeuristic);
                    if(lines[2] == "1")
                        Console.WriteLine("Initial fitness function value: {0}", Fitness(resultHeuristic, r));
                    else
                        Console.WriteLine("Second fitness function value: {0}", MyFitness(resultHeuristic, r));
                }
                else
                {
                    Console.WriteLine("Invalid input!");
                }
            }
        }

        public static List<List<int>> BruteForce(List<int> initialSet, int r, Func<List<List<int>>, int, int> fitness)
        {
            List<int> initialCopy = new List<int>(initialSet);
            List<List<List<int>>> allSets = GeneratePartitions(initialCopy, r);
            int bestIndex = 0;
            int tmp = initialSet.Sum();
            foreach (List<List<int>> list in allSets)
            {
                if (fitness(list, r) < tmp)
                {
                    bestIndex = allSets.IndexOf(list);
                    tmp = fitness(list, r);
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
                List<int> initialCopy = new List<int>(initialSet);
                List<List<List<int>>> Sp = GeneratePartitions(initialCopy, r);
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
                List<int> initialCopy2 = new List<int>(initialSet);
                List<List<List<int>>> Spp = GeneratePartitions(initialCopy2, r-1);
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

        public static int Fitness(List<List<int>> list, int r)
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
