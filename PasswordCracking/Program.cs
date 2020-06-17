using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordCracking
{
    class Program
    {

        const string LOWER_CASE = "abcdefghijklmnopqursşuvwxyz";
        const string UPPER_CAES = "ABCDEFGHIİJKLMNOPQRSTUVWXYZ";
        const string NUMBERS = "0123456789";


        static Random random = new Random();

        public static ArrayList Shuffle(ArrayList list)
        {
            ArrayList newlist = new ArrayList();
            int k = list.Count;
            for(int i=0; i<list.Count; i++)
            {
                int index = random.Next(k);
                newlist.Add(list[index]);
                k--;
            }

            return newlist;
        }

        /* Kelime karşılaştırması */
        public static int Fitness(String word,String test_word)
        {
            int score = 0;
            if(word.Length != test_word.Length)
            {
                Console.WriteLine("Uzunluklar eşleşmiyor.");
                return -1;
            }
            else
            {
                for(int i=0; i<word.Length; i++)
                {
                    if (word[i] == test_word[i]) { score += 1; }
                }
                return score * 100 / word.Length;
            }
            
        }

        public static string New_Word(int length)
        {
            char[] word = new char[length];
            string charSet = "";
            charSet += LOWER_CASE;
            charSet += UPPER_CAES;
            charSet += NUMBERS;
            for (int i =0; i<length; i++)
            {
                word[i] = charSet[random.Next(charSet.Length -1)];
            }

            return String.Join(null, word);

        }

        public static ArrayList Get_first_gen(int pop_size, int length)
        {
            ArrayList pop = new ArrayList();
            for(int i=0; i<pop_size; i++)
            {
                pop.Add(New_Word(length));
            }
            return pop;
        }

        public static Dictionary<String, int> Pop_fitness(ArrayList pop, String word)
        {
            IDictionary<String, int> pop_fit = new Dictionary<String, int>();
            foreach (String item in pop)
            {
                if(!pop_fit.Keys.Contains(item))
                {
                    pop_fit.Add(item, Fitness(word, item));
                }
                
            }
            var items = from pair in pop_fit
                        orderby pair.Value descending
                        select pair;
            var sorted = new Dictionary<String, int>();
            // Display results.
            foreach(KeyValuePair<String, int> pair in items)
            {
                sorted.Add(pair.Key, pair.Value);
            }

            return sorted;

        }


        public static ArrayList Select_pop(Dictionary<String, int> pop_sort,int best,int lucky,int pop_size)
        {
            ArrayList next_gen = new ArrayList();
            for(int i=0; i<best-1; i++)
            {
                int index = random.Next(pop_sort.Count);
                if (i>=pop_sort.Count)
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

            for(int i=0; i<lucky; i++)
            {
                int index = random.Next(pop_sort.Count);
                KeyValuePair<String, int> pair = pop_sort.ElementAt(index);
                next_gen.Add(pair.Key);
            }
            ArrayList shuffle = Shuffle(next_gen);
            return shuffle;
        }

        public static String Create_child(String parent1,String parent2)
        {
            String child = "";
            for(int i=0; i<parent1.Length; i++)
            {
                if(random.Next(0,10)>4)
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

        public static ArrayList Create_children(ArrayList parents)
        {
            ArrayList next_pop = new ArrayList();
            for(int i=0; i<(int)parents.Count/2; i++)
            {
                for(int j=0; j<4; j++)
                {
                    next_pop.Add(Create_child((String)parents[i], (String)parents[(int) parents.Count - 1 - i]));

                }
            }
            return next_pop;

        }

        public static ArrayList Mutate(ArrayList pop, int chance)
        {
            for(int i=0; i<pop.Count; i++)
            {
                if(random.Next(0,101)<chance)
                {
                    int r = random.Next(0, pop[0].ToString().Length - 1);
                    String word = "";
                    if(r!=0)
                    {
                        word += pop[i].ToString().Substring(0, r);
                    }
                    string charSet = "";
                    charSet += LOWER_CASE;
                    charSet += UPPER_CAES;
                    charSet += NUMBERS;
                    word += charSet[random.Next(charSet.Length - 1)] + pop[i].ToString().Substring(r+1);
                    pop[i] = word;
                }
            }
            return pop;
        }


        public 
        static void Main(string[] args)
        {
            Stopwatch sw = Stopwatch.StartNew();
            String password = "S0m3PAs5worD";
            int pop_size = 2000;
            int best_candidates = 750;
            int lucky_candidates = 250;
            int mutation_percent = 30;

            ArrayList pop = Get_first_gen(pop_size, password.Length);
            int i = 0;
            while (1==1)
            {
                Dictionary<String, int> pop_sort = Pop_fitness(pop, password);
                for(int p = 0; p<pop_sort.Count; p++)
                {
                    KeyValuePair<String, int> pair = pop_sort.ElementAt(p);
                    if (i % 5 == 4)
                    {
                        int value = pair.Value;
                        Console.WriteLine(p + " " + pair.Value + " " + pair.Key + " " + pop_sort.Count);
                    }
                }
                KeyValuePair<String, int> pair_first = pop_sort.ElementAt(0);
                if (pair_first.Value == 100.0)
                {
                    Console.WriteLine("Şifre Bulundu : "+pair_first.Key);
                    Console.WriteLine(i+". Nesil");
                    Console.WriteLine("Geçen Süre : "+sw.ElapsedMilliseconds+" milisaniye");
                    break;
                }

                ArrayList next_parents = Select_pop(pop_sort, best_candidates, lucky_candidates, pop_size);
                
                ArrayList next_pop = Create_children(next_parents);

                next_pop = Mutate(next_pop, mutation_percent);
                pop = next_pop;
                i = i + 1;
            }
            Console.WriteLine("Çıkış yapmak için herhangi bir tuşa basınız.");
            Console.ReadKey();
        }
    }
}
