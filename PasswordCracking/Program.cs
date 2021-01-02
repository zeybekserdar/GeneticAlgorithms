using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GAPassword
{
    class Program
    {

        const string Genes = "abcçdefgğhıijklmnoöpqrsştuüvwxyzABCÇDEFGĞHIİJKLMNOÖPQRSŞTUÜVWXYZ";
        public static int total = 0;
        static readonly Random random = new Random();

        public static ArrayList Shuffle(ArrayList list)
        {
            ArrayList newlist = new ArrayList();
            int k = list.Count;
            for (int i = 0; i < list.Count; i++)
            {
                int index = random.Next(k);
                newlist.Add(list[index]);
                k--;
            }

            return newlist;
        }

        /* Kelime karşılaştırması */
        public static int Fitness(String word, String test_word)
        {
            int score = 0;
            if (word.Length != test_word.Length)
            {
                Console.WriteLine("Uzunluklar eşleşmiyor.");
                return -1;
            }
            else
            {
                for (int i = 0; i < word.Length; i++)
                {
                    if (word[i] == test_word[i]) { score += 1; }
                }
                return score * 100 / word.Length;
            }

        }

        public static string newWord(int length)
        {
            char[] word = new char[length];
            for (int i = 0; i < length; i++)
            {
                word[i] = Genes[random.Next(Genes.Length - 1)];
            }
            total += 1;
            return String.Join(null, word);

        }

        public static ArrayList firstGeneration(int pop_size, int length)
        {
            ArrayList pop = new ArrayList();
            for (int i = 0; i < pop_size; i++)
            {
                pop.Add(newWord(length));
            }
            return pop;
        }

        public static Dictionary<String, int> popFitness(ArrayList pop, String word)
        {
            IDictionary<String, int> pop_fit = new Dictionary<String, int>();
            foreach (String item in pop)
            {
                if (!pop_fit.Keys.Contains(item))
                {
                    pop_fit.Add(item, Fitness(word, item));
                }
            }
            var items = from pair in pop_fit
                        orderby pair.Value descending
                        select pair;
            var sorted = new Dictionary<String, int>();
            foreach (KeyValuePair<String, int> pair in items)
            {
                sorted.Add(pair.Key, pair.Value);
            }
            return sorted;
        }


        public static ArrayList selectPopulation(Dictionary<String, int> pop_sort, int best, int lucky)
        {
            ArrayList next_gen = new ArrayList();
            for (int i = 0; i < best - 1; i++)
            {
                int index = random.Next(pop_sort.Count);
                if (i >= pop_sort.Count)
                {
                    KeyValuePair<String, int> pair = pop_sort.ElementAt(index);
                    next_gen.Add(pair.Key);
                }
                else
                {
                    KeyValuePair<String, int> pair = pop_sort.ElementAt(i);
                    next_gen.Add(pair.Key);
                }
            }

            for (int i = 0; i < lucky; i++)
            {
                int index = random.Next(pop_sort.Count);
                KeyValuePair<String, int> pair = pop_sort.ElementAt(index);
                next_gen.Add(pair.Key);
            }
            ArrayList shuffle = Shuffle(next_gen);
            return shuffle;
        }

        public static String createChild(String parent1, String parent2)
        {
            total += 1;
            String child = String.Empty;
            for (int i = 0; i < parent1.Length; i++)
            {
                if (random.Next(1, 11) > 5)
                {
                    child += parent1[i];
                }
                else
                {
                    child += parent2[i];
                }
            }
            return child;

        }

        public static ArrayList createChildren(ArrayList parents)
        {
            ArrayList next_pop = new ArrayList();
            for (int i = 0; i <parents.Count / 2; i++)
            {
                for (int j = 0; j < random.Next(1,5); j++)
                {
                    next_pop.Add(createChild(parents[i].ToString(),parents[parents.Count - 1 - i].ToString()));
                    
                }
            }
            return next_pop;

        }

        public static ArrayList Mutation(ArrayList pop, int chance)
        {
            for (int i = 0; i < pop.Count; i++)
            {
                if (random.Next(0, 101) < chance)
                {
                    int r = random.Next(0, pop[0].ToString().Length - 1);
                    String word = String.Empty;
                    if (r != 0)
                    {
                        word += pop[i].ToString().Substring(0, r);
                    }
                    word += Genes[random.Next(Genes.Length - 1)] + pop[i].ToString().Substring(r + 1);
                    pop[i] = word;
                }
            }
            return pop;
        }


        public
        static void Main(string[] args)
        {
            Stopwatch sw = Stopwatch.StartNew();
            String password = "MerhabaMedium";
            int population_size = 3000;
            int best_candidates = 1000;
            int lucky_candidates = 250;
            int mutation_percent = 10;

            ArrayList population = firstGeneration(population_size, password.Length);
            int i = 0;
            while (true)
            {
                
                Dictionary<String, int> pop_sort = popFitness(population, password);
                for (int p = 0; p < pop_sort.Count; p++)
                {
                    KeyValuePair<String, int> pair = pop_sort.ElementAt(p);

                        Console.WriteLine(p + " " + pair.Value + " " + pair.Key + " " + pop_sort.Count);
                    
                }
                KeyValuePair<String, int> pair_first = pop_sort.First();
                if (pair_first.Value == 100.0)
                {
                    Console.WriteLine("Şifre Bulundu : " + pair_first.Key);
                    Console.WriteLine(i + ". Nesil");
                    Console.WriteLine(total + " birey oluşturuldu");
                    Console.WriteLine("Geçen Süre : " + sw.ElapsedMilliseconds + " milisaniye");
                    break;
                }

                ArrayList next_parents = selectPopulation(pop_sort, best_candidates, lucky_candidates);

                ArrayList next_pop = createChildren(next_parents);

                next_pop = Mutation(next_pop, mutation_percent);
                population = next_pop;
                i += 1;
            }

            Console.WriteLine("Çıkış yapmak için herhangi bir tuşa basınız.");
            Console.ReadKey();
        }
    }
}
