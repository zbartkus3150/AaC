using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

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
                    Console.WriteLine("Input 1 for text file input or 2 for console input:");
                    line = Console.ReadLine();
                }
                while (!(int.TryParse(line, out option)));
            }
            while (option != 1 && option != 2);

            if(option == 1)
            {
                FileInput();
            }
            else
            {
                ConsoleInput();
            }
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }

        public static void ConsoleInput()
        {
            Stopwatch sw = new Stopwatch();
            List<int> initialSet = new List<int>();
            int r;
            Console.WriteLine("Input weights:");
            string line = Console.ReadLine();
            string[] numbers = line.Split(',');
            if (numbers.Length == 1)
            {
                Random rnd = new Random();
                int n;
                if (int.TryParse(numbers[0], out n))
                {
                    for (int i = 0; i < n; i++)
                    {
                        initialSet.Add(rnd.Next(n * 4));
                    }
                }
            }
            else
            {
                foreach (string number in numbers)
                {
                    int n;
                    if (int.TryParse(number, out n))
                        initialSet.Add(n);
                }
            }
            do
            {
                Console.WriteLine("Input number of partitions:");
                line = Console.ReadLine();
            }
            while (!(int.TryParse(line, out r)));
            Console.WriteLine("Type 1 for optimal solution, 2 for heurisctic solution or 3 for both:");
            string o = Console.ReadLine();

            double timeBrute;
            double timeHeuristic;
            List<List<int>> resultBrute = new List<List<int>>();
            List<List<int>> resultHeuristic = new List<List<int>>();
            if (o == "1" || o == "3")
            {
                Console.WriteLine("Type 1 to use first fitness function of 2 for the second one:");
                string f = Console.ReadLine();
                if(f == "1")
                {
                    sw.Restart();
                    resultBrute = BruteForce(initialSet, r, Fitness);
                    timeBrute = sw.Elapsed.TotalMilliseconds;
                    sw.Stop();
                }
                else
                {
                    sw.Restart();
                    resultBrute = BruteForce(initialSet, r, MyFitness);
                    timeBrute = sw.Elapsed.TotalMilliseconds;
                    sw.Stop();
                }
                Console.WriteLine("Brute Force solution:");
                PrintSolution(resultBrute);
                Console.WriteLine("Initial fitness function value: {0}", Fitness(resultBrute, r));
                Console.WriteLine("Second fitness function value: {0}", MyFitness(resultBrute, r));
                Console.WriteLine("Time needed: " + timeBrute + " miliseconds");
            }
            if (o == "2" || o == "3")
            {
                sw.Restart();
                resultHeuristic = Heuristic(initialSet, r);
                timeHeuristic = sw.Elapsed.TotalMilliseconds;
                sw.Stop();
                Console.WriteLine("Heuristic solution:");
                PrintSolution(resultHeuristic);
                Console.WriteLine("Initial fitness function value: {0}", Fitness(resultHeuristic, r));
                Console.WriteLine("Second fitness function value: {0}", MyFitness(resultHeuristic, r));
                Console.WriteLine("Time needed: " + timeHeuristic + " miliseconds");
            }
            Console.WriteLine();


            if (o == "1" || o == "3")
            {
                Console.WriteLine();
                Console.WriteLine("Print partitions? (y/n)");
                string response = Console.ReadLine();
                if (response == "y")
                {
                    Console.WriteLine("All partitions:");
                    List<List<List<int>>> testBrute = GeneratePartitions(initialSet, r);
                    foreach (List<List<int>> l in testBrute)
                    {
                        PrintSolution(l);
                        Console.WriteLine();
                    }
                }
            }
        } 

        public static void FileInput()
        {
            List<int> initialSet = new List<int>();
            Console.WriteLine("Input file name:");
            string fileName = Console.ReadLine();
            List<string> alllines = new List<string>();
            try
            {
                alllines = System.IO.File.ReadAllLines(fileName).ToList();
            }
            catch (Exception e) {
                Console.WriteLine("Invalid file name");
                return;
            }
            string[] lines;
            while (alllines.Count() >= 4)
            {
                lines = alllines.GetRange(0, 4).ToArray();
                FileInputParse(lines, initialSet);
                alllines.RemoveRange(0, 4);
                initialSet.Clear();
            }
        }

        public static void FileInputParse(string[] lines, List<int> initialSet)
        {
            Stopwatch sw = new Stopwatch();
            int r;
            if (lines.Length != 4)
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
                    foreach (string w in weights)
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
                        for (int i = 0; i < size; i++)
                        {
                            initialSet.Add(rnd.Next(size * 4));
                        }
                    }
                }
                Console.WriteLine();
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine();
                double time;
                if (lines[1] == "o")
                {
                    Console.WriteLine("Brute force solution:");
                    List<List<int>> resultBrute = new List<List<int>>();
                    if (lines[2] == "1")
                    {
                        sw.Restart();
                        resultBrute = BruteForce(initialSet, r, Fitness);
                        time = sw.Elapsed.TotalMilliseconds;
                        PrintSolution(resultBrute);
                        Console.WriteLine("Initial fitness function value: {0}", Fitness(resultBrute, r));
                    }
                    else
                    {
                        sw.Restart();
                        resultBrute = BruteForce(initialSet, r, MyFitness);
                        time = sw.Elapsed.TotalMilliseconds;
                        PrintSolution(resultBrute);
                        Console.WriteLine("Second fitness function value: {0}", MyFitness(resultBrute, r));
                    }
                    Console.WriteLine("Time needed: " + time + " miliseconds");
                    sw.Stop();
                }
                else if (lines[1] == "h")
                {
                    Console.WriteLine("Heuristic solution:");
                    sw.Restart();
                    List<List<int>> resultHeuristic = Heuristic(initialSet, r);
                    time = sw.Elapsed.TotalMilliseconds;
                    PrintSolution(resultHeuristic);
                    if (lines[2] == "1")
                        Console.WriteLine("Initial fitness function value: {0}", Fitness(resultHeuristic, r));
                    else
                        Console.WriteLine("Second fitness function value: {0}", MyFitness(resultHeuristic, r));
                    Console.WriteLine("Time needed: " + time + " miliseconds");
                    sw.Stop();
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
