using System;
using System.Collections.Generic;

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
         new decimal[5] { 10/100m, 1/100m, 5/100m, 4/100m, 80/100m },
         new decimal[8] { 8/80m, 12/80m, 20/80m, 30/80m, 1/80m, 4/80m, 2/80m, 3/80m },
         new decimal[4] { 1/8m, 1/8m, 3/8m, 3/8m },
         new decimal[17] { 1/37m, 4/37m, 5/37m, 14/37m, 1/37m, 1/37m, 1/37m, 1/37m, 1/37m, 1/37m, 1/37m, 1/37m, 1/37m, 1/37m, 1/37m, 1/37m, 1/37m },
         new decimal[3] { 2/7m, 4/7m, 1/7m }
      };

      static void Main(string[] args)
      {
         // Pool of All Possible Combinations
         Population pool = new Population();
         for (byte i = 0; i < SUBCATEGORY_COUNTS[0]; i++)
            for (byte j = 0; j < SUBCATEGORY_COUNTS[1]; j++)
               for (byte k = 0; k < SUBCATEGORY_COUNTS[2]; k++)
                  for (byte t = 0; t < SUBCATEGORY_COUNTS[3]; t++)
                     for (byte y = 0; y < SUBCATEGORY_COUNTS[4]; y++)
                        for (byte z = 0; z < SUBCATEGORY_COUNTS[5]; z++)
                           for (byte x = 0; x < SUBCATEGORY_COUNTS[6]; x++)
                              pool.Add(new Gene(new byte[] { i, j, k, t, y, z, x }));

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
               Console.WriteLine(bestRatio + "%");
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

            for (int i = 0; i < CATEGORY_COUNT; i++)
               MaxFitness += SUBCATEGORY_COUNTS[i];

            decimal fitness = MaxFitness;

            for (int i = 0; i < CATEGORY_COUNT; i++)
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
         internal void Add(List<Gene> genes)
         {
            foreach (Gene gene in genes)
               Genes.Add(gene);
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

         internal byte[] Array { get; set; }
      }
   }
}