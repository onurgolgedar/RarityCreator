using System;
using System.Collections.Generic;
using System.Linq;

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
      //const int CATEGORY_COUNT = 5;
      //const int NFT_COUNT = 80;
      //static int[] SUBCATEGORY_COUNTS = new int[CATEGORY_COUNT] { 4, 5, 6, 7, 4 };
      //static decimal[][] SUBCATEGORY_RATES = new decimal[CATEGORY_COUNT][] {
      //   new decimal[4] { 1/10m, 6/10m, 2/10m, 1/10m },
      //   new decimal[5] { 1/5m, 1/5m, 1/5m, 1/5m, 1/5m },
      //   new decimal[6] { 4/30m, 6/30m, 10/30m, 4/30m, 4/30m, 2/30m },
      //   new decimal[7] { 3/70m, 7/70m, 15/70m, 25/70m, 5/70m, 5/70m, 10/7m },
      //   new decimal[4] { 30/70m, 7/70m, 3/70m, 30/70m }
      //};

      const int CROSSOVER_SIZE = NFT_COUNT / 5;

      static void Main(string[] args)
      {
         //Pool of All Possible Combinations
         Population pool = new Population();
         /*for (byte i = 0; i < SUBCATEGORY_COUNTS[0]; i++)
            for (byte j = 0; j < SUBCATEGORY_COUNTS[1]; j++)
               for (byte k = 0; k < SUBCATEGORY_COUNTS[2]; k++)
                  for (byte t = 0; t < SUBCATEGORY_COUNTS[3]; t++)
                     for (byte y = 0; y < SUBCATEGORY_COUNTS[4]; y++)
                        for (byte z = 0; z < SUBCATEGORY_COUNTS[5]; z++)
                           for (byte x = 0; x < SUBCATEGORY_COUNTS[6]; x++)
                              for (byte o = 0; o < SUBCATEGORY_COUNTS[7]; o++)
                                 for (byte n = 0; n < SUBCATEGORY_COUNTS[8]; n++)
                                    pool.Add(new Gene(new byte[] { i, j, k, t, y, z, x, o, n }));*/

         //Population pool = new Population();
         //for (byte i = 0; i < SUBCATEGORY_COUNTS[0]; i++)
         //   for (byte j = 0; j < SUBCATEGORY_COUNTS[1]; j++)
         //      for (byte k = 0; k < SUBCATEGORY_COUNTS[2]; k++)
         //         for (byte t = 0; t < SUBCATEGORY_COUNTS[3]; t++)
         //            for (byte z = 0; z < SUBCATEGORY_COUNTS[4]; z++)
         //               pool.Add(new Gene(new byte[] { i, j, k, t, z }));

         Population parent1 = pool.SelectFromPool(NFT_COUNT);
         Population parent2 = pool.SelectFromPool(NFT_COUNT);

         Console.WriteLine(Math.Round(parent1.Fitness() / parent1.MaxFitness * 10000) / 100 + "%");
         Console.WriteLine(Math.Round(parent2.Fitness() / parent2.MaxFitness * 10000) / 100 + "%");

         Population bestParent;
         Population otherParent;
         decimal bestFitness;
         decimal otherFitness;
         while (true)
         {
            decimal fitness1 = parent1.Fitness();
            decimal fitness2 = parent2.Fitness();
            if (fitness1 > fitness2)
            {
               bestFitness = fitness1;
               otherFitness = fitness2;
               bestParent = parent1;
               otherParent = parent2;
            }
            else
            {
               bestFitness = fitness2;
               otherFitness = fitness1;
               bestParent = parent2;
               otherParent = parent1;
            }

            Population child = bestParent.Crossover(otherParent, pool);
            decimal childFitness = child.Fitness();
            if (childFitness > bestFitness)
            {
               otherParent = bestParent;
               otherFitness = bestFitness;
               bestParent = child;
               bestFitness = childFitness;
               Console.WriteLine(Math.Round(childFitness / child.MaxFitness * 10000) / 100 + "%");
            }
            else if (childFitness > otherFitness)
            {
               otherParent = child;
               otherFitness = childFitness;
            }

            parent1 = bestParent;
            parent2 = otherParent;

            if (bestFitness / bestParent.MaxFitness > 0.62m)
            {
               Console.WriteLine(bestParent);
               break;
            }
         }
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
            SetRates();
         }

         public decimal Fitness()
         {
            MaxFitness = 0;

            for (int i = 1; i < CATEGORY_COUNT; i++)
               MaxFitness += SUBCATEGORY_COUNTS[i];

            decimal fitness = MaxFitness;

            for (int i = 1; i < CATEGORY_COUNT; i++)
               for (int j = 0; j < Subcategory_rates[i].Length; j++)
               {
                  decimal fitnessLoss = Math.Abs(Subcategory_rates[i][j] - SUBCATEGORY_RATES[i][j]) / Math.Max(Subcategory_rates[i][j], SUBCATEGORY_RATES[i][j]);
                  fitness -= fitnessLoss;
               }

            return fitness;
         }
         public bool Add(Gene gene)
         {
            if (!Genes.Contains(gene))
               Genes.Add(gene);
            else
               return false;

            return true;
         }
         public void Feed(Population pop, int count)
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
                     Genes.Add(gene);
                     break;
                  }
               }

               count--;
            }
         }
         public void DeleteLast(int count)
         {
            for (int i = 0; i < count; i++)
               Genes.RemoveAt(Genes.Count - 1);
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
         public Population SelectFromPool(int count)
         {
            List<Gene> selectedList = new List<Gene>();
            while (count > 0)
            {
               while (true)
               {
                  Gene gene = new Gene(new byte[] { (byte)randomGenerator.Next(SUBCATEGORY_COUNTS[0]), (byte)randomGenerator.Next(SUBCATEGORY_COUNTS[1]), (byte)randomGenerator.Next(SUBCATEGORY_COUNTS[2]),
                                                    (byte)randomGenerator.Next(SUBCATEGORY_COUNTS[3]), (byte)randomGenerator.Next(SUBCATEGORY_COUNTS[4]), (byte)randomGenerator.Next(SUBCATEGORY_COUNTS[5]),
                                                    (byte)randomGenerator.Next(SUBCATEGORY_COUNTS[6]), (byte)randomGenerator.Next(SUBCATEGORY_COUNTS[7]), (byte)randomGenerator.Next(SUBCATEGORY_COUNTS[8]) });
                  if (!selectedList.Contains(gene))
                  {
                     selectedList.Add(gene);
                     break;
                  }
               }

               count--;
            }

            return new Population(selectedList);
         }
         public Population Crossover(Population pop, Population pool)
         {
            Population child = new Population();
            foreach (Gene gene in Genes)
               child.Add(new Gene(gene.Array));

            Shuffle(child.Genes);

            child.DeleteLast(CROSSOVER_SIZE);
            child.Feed(pop, CROSSOVER_SIZE);

            while (true)
            {
               bool done = true;

               List<Gene> range1 = child.Genes.GetRange(0, CROSSOVER_SIZE);
               List<Gene> range2 = child.Genes.GetRange(child.Genes.Count - CROSSOVER_SIZE, CROSSOVER_SIZE);
               for (int i = 0; i < range1.Count; i++)
               {
                  Gene gene1 = range1[i];

                  for (int j = 0; j < range2.Count; j++)
                  {
                     Gene gene2 = range2[j];

                     if (gene2.Equals(gene1))
                     {
                        child.Genes.RemoveAt(i);
                        child.Genes.Insert(i, pool.SelectFromPool(1).Genes[0]);
                        done = false;
                     }
                  }

                  if (done == false)
                     break;
               }

               if (done)
                  break;
            }

            child.SetRates();
            return child;
         }

         public void SetRates()
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
         public override string ToString()
         {
            string str = "Population: ";

            foreach (Gene gene in Genes)
               str += gene.ToString();

            str += "_________";
            for (int i = 0; i < Subcategory_rates.Length; i++)
               for (int j = 0; j < Subcategory_rates[i].Length; j++)
                  str += (char)('A' + i) + "" + j + ": " + Math.Round(Subcategory_rates[i][j] * 1000) / 10 + "%\n";

            return "Fitness: " + Fitness() + "/" + MaxFitness + "\n";
         }

         public List<Gene> Genes { get; set; }
         public decimal[][] Subcategory_rates { get; set; }
         public decimal MaxFitness { get; set; }
      }
      public class Gene
      {
         public Gene(byte[] array)
         {
            Array = array;
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

            foreach (byte _byte in Array)
               str += _byte + " ";

            return str + "\n";
         }

         public byte[] Array { get; set; }
      }

      static public void Shuffle<T>(this IList<T> list)
      {
         int n = list.Count;
         while (n > 1)
         {
            n--;
            int k = randomGenerator.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
         }
      }
   }
}