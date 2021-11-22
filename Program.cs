using System;
using System.Collections.Generic;

namespace RarityCreator
{
   static class Program
   {
      static Random randomGenerator = new Random();

      // Configuration
      const int CATEGORY_COUNT = 9;
      const int NFT_COUNT = 1111;
      static byte[] SUBCATEGORY_COUNTS = new byte[CATEGORY_COUNT] { 10, 10, 10, 10, 2, 12, 14, 21, 15 };
      static decimal[][] SUBCATEGORY_RATES = new decimal[CATEGORY_COUNT][] {
         new decimal[10] { 12/100m, 34/100m, 10/100m, 4/100m, 20/100m, 5/100m, 5/100m, 2/100m, 3/100m, 5/100m },
         new decimal[10] { 12/100m, 34/100m, 10/100m, 4/100m, 20/100m, 5/100m, 5/100m, 2/100m, 3/100m, 5/100m },
         new decimal[10] { 40/100m, 20/100m, 3/100m, 3/100m, 3/100m, 6/100m, 3/100m, 17/100m, 3/100m, 2/100m },
         new decimal[10] { 12/100m, 24/100m, 10/100m, 4/100m, 20/100m, 5/100m, 15/100m, 2/100m, 3/100m, 5/100m },
         new decimal[2] { 30/100m, 70/100m },
         new decimal[12] { 2/24m, 2/24m, 2/24m, 2/24m, 2/24m, 2/24m, 2/24m, 2/24m, 2/24m, 2/24m, 2/24m, 2/24m },
         new decimal[14] { 5/28m, 1/28m, 3/28m, 1/28m, 2/28m, 2/28m, 3/28m, 3/28m, 1/28m, 1/28m, 2/28m, 2/28m, 1/28m, 1/28m },
         new decimal[21] { 8/42m, 2/42m, 1/42m, 1/42m, 5/42m, 2/42m, 1/42m, 4/42m, 1/42m, 1/42m, 2/42m, 1/42m, 3/42m, 2/42m, 2/42m, 1/42m, 1/42m, 1/42m, 1/42m, 1/42m, 1/42m },
         new decimal[15] { 3/30m, 3/30m, 3/30m, 1/30m, 4/30m, 2/30m, 2/30m, 1/30m, 3/30m, 1/30m, 1/30m, 2/30m, 2/30m, 2/30m, 1/30m }
      };

      // Genetic Algorithm Configuration
      const int CROSSOVER_SIZE = NFT_COUNT / 20;
      const int MUTATION_SIZE = NFT_COUNT / 17;

      static void Main(string[] args)
      {
         SortedList<decimal, Population> habitat = new SortedList<decimal, Population>();
         for (int i = 0; i < 30; i++)
         {
            Population pop = new Population();
            pop.Feed(NFT_COUNT);
            pop.CalculateRates();
            habitat.Add(pop.Fitness() + (decimal)randomGenerator.Next(1000000) / 1000000000000, pop);
         }

         int tryCount = 0;
         while (tryCount < 250000)
         {
            tryCount++;

            decimal pop1_fitness = habitat.Keys[habitat.Keys.Count - 1];
            Population pop1 = habitat[pop1_fitness];

            decimal pop2_fitness = habitat.Keys[habitat.Keys.Count - 1 - randomGenerator.Next(3)];
            Population pop2 = habitat[pop2_fitness];

            Population child;
            if (pop1_fitness > pop2_fitness)
               child = pop1.Crossover(pop2);
            else
               child = pop2.Crossover(pop1);

            decimal child_fitness = child.Fitness() + (decimal)randomGenerator.Next(1000000) / 1000000000000;
            if (child_fitness > habitat.Keys[habitat.Count - 1])
            {
               Console.WriteLine(Math.Round(habitat.Keys[habitat.Count - 1] * 100) / 100 + " (at " + tryCount + ")");
               tryCount = 0;
            }

            habitat.Add(child_fitness, child);
            habitat.RemoveAt(0);
         }

         Console.WriteLine(habitat.Values[habitat.Count - 1]);
      }

      public class Population
      {
         public Population()
         {
            Genes = new List<Gene>();
         }
         public Population(List<Gene> genes)
         {
            Genes = genes;
            CalculateRates();
         }

         public bool Add(Gene gene)
         {
            if (!Genes.Contains(gene))
               Genes.Add(gene);
            else
               return false;

            return true;
         }
         public void Cut(int count)
         {
            for (int i = 0; i < count; i++)
               Genes.RemoveAt(Genes.Count - 1);
         }
         public Population Copy()
         {
            Population copy = new Population();

            foreach (Gene gene in Genes)
               copy.Add(gene.Copy());

            return copy;
         }
         public Population Crossover(Population pop)
         {
            Population child = Copy();
            child.Shuffle();

            child.Cut(CROSSOVER_SIZE);
            child.Feed(CROSSOVER_SIZE);

            // Mutation
            for (int i = 0; i < MUTATION_SIZE; i++)
            {
               child.Genes.RemoveAt(i);
               child.Feed(1, i);
            }

            child.CalculateRates();

            return child;
         }
         public void CalculateRates()
         {
            Subcategory_rates = new decimal[CATEGORY_COUNT][] {
               new decimal[10],
               new decimal[10],
               new decimal[10],
               new decimal[10],
               new decimal[2],
               new decimal[12],
               new decimal[14],
               new decimal[21],
               new decimal[15]
            };
            //Subcategory_rates = new decimal[CATEGORY_COUNT][] {
            //   new decimal[4],
            //   new decimal[5],
            //   new decimal[6],
            //   new decimal[7],
            //   new decimal[4]
            //};

            for (int i = 0; i < Genes.Count; i++)
            {
               Gene gene = Genes[i];

               for (int j = 0; j < gene.Array.Length; j++)
                  Subcategory_rates[j][gene.Array[j]] += 1m / Genes.Count;
            }
         }
         public void Feed(int count, int startIndex = -1)
         {
            while (count > 0)
            {
               while (true)
               {
                  Gene gene = new Gene(new byte[] { (byte)randomGenerator.Next(SUBCATEGORY_COUNTS[0]), (byte)randomGenerator.Next(SUBCATEGORY_COUNTS[1]), (byte)randomGenerator.Next(SUBCATEGORY_COUNTS[2]),
                                                    (byte)randomGenerator.Next(SUBCATEGORY_COUNTS[3]), (byte)randomGenerator.Next(SUBCATEGORY_COUNTS[4]), (byte)randomGenerator.Next(SUBCATEGORY_COUNTS[5]),
                                                    (byte)randomGenerator.Next(SUBCATEGORY_COUNTS[6]), (byte)randomGenerator.Next(SUBCATEGORY_COUNTS[7]), (byte)randomGenerator.Next(SUBCATEGORY_COUNTS[8]) });
                  if (!Genes.Contains(gene))
                  {
                     if (startIndex == -1)
                        Genes.Add(gene);
                     else
                        Genes.Insert(startIndex, gene);
                     break;
                  }
               }

               count--;
            }
         }
         public decimal Fitness()
         {
            int rare1 = 0, rare2 = 0, rare3 = 0, rare4 = 0, rare5 = 0, rare6 = 0, rare7 = 0, rare8 = 0, rare9 = 0, rare10 = 0;
            decimal averagePossibility = 0;
            decimal fitness;

            SortedList<decimal, string> possibilities = new SortedList<decimal, string>();
            foreach (Gene gene in Genes)
            {
               decimal possibility = 1;
               for (int i = 0; i < CATEGORY_COUNT; i++)
                  possibility *= (decimal)Math.Pow((double)Subcategory_rates[i][gene.Array[i]], 1.4);
               averagePossibility += possibility / Genes.Count;

               possibilities.Add(possibility + (decimal)randomGenerator.Next(100000) / 1000000000000000000, "");
            }

            foreach (decimal possibility in possibilities.Keys)
            {
               if (Math.Abs(possibility - averagePossibility) > averagePossibility * 99 / 100)
                  rare10++;
               else if (Math.Abs(possibility - averagePossibility) > averagePossibility * 97 / 100)
                  rare9++;
               else if (Math.Abs(possibility - averagePossibility) > averagePossibility * 93 / 100)
                  rare8++;
               else if (Math.Abs(possibility - averagePossibility) > averagePossibility * 85 / 100)
                  rare7++;
               else if (Math.Abs(possibility - averagePossibility) > averagePossibility * 67 / 100)
                  rare6++;
               else if (Math.Abs(possibility - averagePossibility) > averagePossibility * 40 / 100)
                  rare5++;
               else if (Math.Abs(possibility - averagePossibility) > averagePossibility * 25 / 100)
                  rare4++;
               else if (Math.Abs(possibility - averagePossibility) > averagePossibility * 12 / 100)
                  rare3++;
               else if (Math.Abs(possibility - averagePossibility) > averagePossibility * 7 / 100)
                  rare2++;
               else
                  rare1++;
            }

            fitness = rare10 * 1600 + rare9 * 800 + rare8 * 550 + rare7 * 300 + rare6 * 140 + rare5 * 55 + rare4 * 15 + rare3 * 4 + rare2 * 2 + rare1;

            return fitness;
         }
         public Population Select(int count)
         {
            List<Gene> clonedContent = new List<Gene>();
            foreach (Gene gene in Genes)
               clonedContent.Add(new Gene(gene.Array));

            List<Gene> selectedList = new List<Gene>();
            while (count > 0)
            {
               int index = randomGenerator.Next(clonedContent.Count);

               selectedList.Add(clonedContent[index]);
               clonedContent.RemoveAt(index);

               count--;
            }

            return new Population(selectedList);
         }
         public void Shuffle()
         {
            int n = Genes.Count;
            while (n > 1)
            {
               n--;
               int k = randomGenerator.Next(n + 1);
               Gene value = Genes[k];
               Genes[k] = Genes[n];
               Genes[n] = value;
            }
         }

         public override string ToString()
         {
            string str = "Population: \n";

            SortedList<decimal, Gene> _genes = new SortedList<decimal, Gene>();
            foreach (Gene gene in Genes)
            {
               decimal possibility = 1;
               for (int i = 0; i < CATEGORY_COUNT; i++)
                  possibility *= Subcategory_rates[i][gene.Array[i]];

               _genes.Add(possibility + (decimal)randomGenerator.Next(100000) / 1000000000000000000, gene);
            }

            for (int i = _genes.Count - 1; i >= 0; i--)
            {
               decimal fitness = _genes.Keys[i];
               Gene gene = _genes[fitness];
               str += gene.ToString() + " (" + fitness * 1000000000 + ")\n";
            }

            str += "_________\n";
            for (int i = 0; i < Subcategory_rates.Length; i++)
            {
               SortedList<decimal, string> rateList = new SortedList<decimal, string>();
               for (int j = 0; j < Subcategory_rates[i].Length; j++)
                  rateList.Add(Subcategory_rates[i][j] + (decimal)randomGenerator.Next(100000) / 1000000000000000000, (char)('A' + i) + "" + j + ": " + Math.Round(Subcategory_rates[i][j] * 1000) / 10 + "\n");
               foreach (string _rateStr in rateList.Values)
                  str += _rateStr;
               str += "\n";
            }

            return str + "Fitness: " + Fitness();
         }

         public List<Gene> Genes { get; set; }
         public decimal[][] Subcategory_rates { get; set; }
      }
      public class Gene
      {
         public Gene(byte[] array)
         {
            Array = array;
         }

         public Gene Copy()
         {
            return new Gene(Array);
         }

         public override bool Equals(object obj)
         {
            Gene gene = (Gene)obj;

            for (int i = 0; i < gene.Array.Length; i++)
               if (Array[i] != gene.Array[i])
                  return false;

            return true;
         }
         public override int GetHashCode()
         {
            return Array.GetHashCode();
         }
         public override string ToString()
         {
            string str = "";

            for (int i = 0; i < Array.Length; i++)
               str += (char)('A' + i) + "" + Array[i] + " ";

            return str;
         }

         public byte[] Array { get; set; }
      }
   }
}