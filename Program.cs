using System;
using System.Collections.Generic;
using System.Linq;

namespace RarityCreator
{
   class Program
   {
      static Random randomGenerator = new Random();

      // Configuration
      const int CATEGORY_COUNT = 7;
      const int NFT_COUNT = 1111;
      static int[] SUBCATEGORY_COUNTS = new int[CATEGORY_COUNT] { 10, 14, 5, 8, 4, 17, 3 };
      static decimal[][] SUBCATEGORY_RATES = new decimal[CATEGORY_COUNT][] {
         new decimal[10] { 12/100m, 34/100m, 10/100m, 4/100m, 20/100m, 5/100m, 5/100m, 2/100m, 3/100m, 5/100m },
         new decimal[14] { 1/28m, 1/28m, 10/28m, 2/28m, 1/28m, 1/28m, 1/28m, 1/28m, 1/28m, 1/28m, 1/28m, 1/28m, 1/28m, 5/28m },
         new decimal[5] { 10/100m, 1/100m, 5/100m, 40/100m, 44/100m },
         new decimal[8] { 8/80m, 12/80m, 20/80m, 30/80m, 1/80m, 4/80m, 2/80m, 3/80m },
         new decimal[4] { 1/8m, 1/8m, 3/8m, 3/8m },
         new decimal[17] { 5/37m, 8/37m, 5/37m, 4/37m, 1/37m, 2/37m, 1/37m, 1/37m, 1/37m, 1/37m, 1/37m, 1/37m, 1/37m, 1/37m, 1/37m, 1/37m, 2/37m },
         new decimal[3] { 2/7m, 4/7m, 1/7m }
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

      const int CROSSOVER_SIZE = NFT_COUNT/4;

      static void Main(string[] args)
      {
         //Pool of All Possible Combinations
         Population pool = new Population();
         for (byte i = 0; i < SUBCATEGORY_COUNTS[0]; i++)
            for (byte j = 0; j < SUBCATEGORY_COUNTS[1]; j++)
               for (byte k = 0; k < SUBCATEGORY_COUNTS[2]; k++)
                  for (byte t = 0; t < SUBCATEGORY_COUNTS[3]; t++)
                     for (byte y = 0; y < SUBCATEGORY_COUNTS[4]; y++)
                        for (byte z = 0; z < SUBCATEGORY_COUNTS[5]; z++)
                           for (byte x = 0; x < SUBCATEGORY_COUNTS[6]; x++)
                              pool.Add(new Gene(new byte[] { i, j, k, t, y, z, x }));

         //Population pool = new Population();
         //for (byte i = 0; i < SUBCATEGORY_COUNTS[0]; i++)
         //   for (byte j = 0; j < SUBCATEGORY_COUNTS[1]; j++)
         //      for (byte k = 0; k < SUBCATEGORY_COUNTS[2]; k++)
         //         for (byte t = 0; t < SUBCATEGORY_COUNTS[3]; t++)
         //            for (byte z = 0; z < SUBCATEGORY_COUNTS[4]; z++)
         //               pool.Add(new Gene(new byte[] { i, j, k, t, z }));

         Population parent1 = pool.Select(NFT_COUNT);
         Population parent2 = pool.Select(NFT_COUNT);
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

            Population child = bestParent.Crossover(parent2, pool);
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

            if (bestFitness / bestParent.MaxFitness > 0.75m)
            {
               bestParent.Print();
               break;
            }
         }

         Console.ReadLine();
         // Random Search
         decimal bestRatio = 0m;
         Population bestSelection = null;
         do
         {
            Population selection = pool.Select(NFT_COUNT);
            decimal fitness = selection.Fitness();
            if (fitness / selection.MaxFitness > bestRatio)
            {
               bestRatio = fitness / selection.MaxFitness;
               bestSelection = selection;
               Console.WriteLine(Math.Round(bestRatio*1000)/10 + "%");
            }
         }
         while (bestRatio < 0.95m);

         // Print, if bestRatio is enough
         bestSelection?.Print();
      }

      class Population
      {
         internal Population()
         {
            Genes = new List<Gene>();
         }
         internal Population(List<Gene> genes)
         {
            Genes = genes;
            SetRates();
         }

         internal decimal Fitness()
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
         internal void Add(Gene gene)
         {
            Genes.Add(gene);
         }
         internal void AddReverse(Gene gene)
         {
            Genes.Insert(0, gene);
         }
         internal void Add(List<Gene> genes)
         {
            foreach (Gene gene in genes)
               Genes.Add(gene);
         }
         internal void DeleteLast(int count)
         {
            for (int i = 0; i < count; i++)
               Genes.RemoveAt(Genes.Count - 1);
         }
         internal Population Select(int count)
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
         internal Population Crossover(Population pop, Population pool)
         {
            Population child = new Population();
            foreach (Gene gene in Genes)
               child.Add(new Gene(gene.Array));

            child.Genes = child.Genes.Select(x => new { value = x, order = randomGenerator.Next() })
                                     .OrderBy(x => x.order).Select(x => x.value).ToList();

            child.DeleteLast(CROSSOVER_SIZE);
            child.Add(pop.Select(CROSSOVER_SIZE).Genes);

            for (int i = 0; i < 10; i++) {
               int index = randomGenerator.Next(NFT_COUNT);
               child.Genes.RemoveAt(index);
               child.Genes.Insert(index, pool.Select(1).Genes[0]);
            }

            //while (true)
            //{
            //   bool done = true;

            //   List<Gene> range1 = child.Genes.GetRange(0, CROSSOVER_SIZE);
            //   List<Gene> range2 = child.Genes.GetRange(child.Genes.Count - CROSSOVER_SIZE, CROSSOVER_SIZE);
            //   for (int i = 0; i < range1.Count; i++)
            //   {
            //      Gene gene1 = range1[i];

            //      for (int j = 0; j < range2.Count; j++)
            //      {
            //         Gene gene2 = range2[j];

            //         if (gene2.IsEqual(gene1))
            //         {
            //            child.Genes.RemoveAt(i);
            //            child.Genes.Insert(i, pool.Select(1).Genes[0]);
            //            done = false;
            //         }
            //      }

            //      if (done == false)
            //         break;
            //   }

            //   if (done)
            //      break;
            //}

            child.SetRates();
            return child;
         }

         internal void SetRates()
         {
            Subcategory_rates = new decimal[CATEGORY_COUNT][] {
               new decimal[10],
               new decimal[14],
               new decimal[5],
               new decimal[8],
               new decimal[4],
               new decimal[17],
               new decimal[3]
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
         internal void Print()
         {
            Console.WriteLine("Population: ");

            foreach (Gene gene in Genes)
               gene.Print();

            Console.WriteLine("_________");

            for (int i = 0; i < Subcategory_rates.Length; i++)
               for (int j = 0; j < Subcategory_rates[i].Length; j++)
                  Console.WriteLine((char)('A' + i) + "" + j + ": " + Math.Round(Subcategory_rates[i][j] * 1000) / 10 + "%");

            Console.WriteLine("Fitness: " + Fitness() + "/" + MaxFitness);
         }

         List<Gene> Genes { get; set; }
         decimal[][] Subcategory_rates { get; set; }
         public decimal MaxFitness { get; set; }
      }
      class Gene
      {
         public Gene(byte[] array)
         {
            Array = array;
         }

         internal void Print()
         {
            foreach (byte _byte in Array)
               Console.Write(_byte + " ");

            Console.WriteLine();
         }
         internal bool IsEqual(Gene gene)
         {
            if (gene.Array.Length != Array.Length)
               return false;

            for (int i = 0; i < gene.Array.Length; i++)
               if (Array[i] != gene.Array[i])
                  return false;

            return true;
         }

         internal byte[] Array { get; set; }
      }
   }
}