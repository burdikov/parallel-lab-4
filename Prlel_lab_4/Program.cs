using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Prlel_lab_4
{
    class Program
    {
        /// <summary>
        /// Общее число потоков
        /// </summary>
        const int M = 10;

        /// <summary>
        /// Количество обрабатываемых файлов
        /// </summary>
        const int O = 20;

        static string s = "c:\\users\\леонид\\desktop\\parallel-lab-four\\";

        static List<string> paths = new List<string>();

        static void Main(string[] args)
        {
            for (int i = 0; i < O; i++)
               paths.Add(s + (i + 1) + ".txt");
            
            v11();
            v12();
            //v2();

            Console.ReadKey();
        }

        static char[] vowels = new char[] { 'а', 'я', 'о', 'у', 'ы', 'е', 'ё', 'э', 'ю', 'и' };
        static char[] consonants = new char[] { 'й', 'ц', 'к', 'н', 'г', 'ш', 'щ', 'з', 'х', 'ф', 'в', 'п', 'р', 'л', 'д', 'ж', 'ч', 'с', 'м', 'т', 'б' };

        static bool? isVowel(char ch)
        {
            ch = char.ToLower(ch);
            if (vowels.Contains(ch)) return true;
            if (consonants.Contains(ch)) return false;
            return null;
        }

        #region 1.1

        static Dictionary<string, long> dict11 = new Dictionary<string, long>();
        static Dictionary<char, long> dict11b = new Dictionary<char, long>();
        static Dictionary<string, long> dict11c = new Dictionary<string, long>();

        static void v11()
        {
            Thread[] workers = new Thread[M];
            Stopwatch timer = new Stopwatch();

            Console.WriteLine("== Реализация 1.1. ==");
            timer.Start();
            for (int i = 0; i < M; i++)
            {
                workers[i] = new Thread(dowork);
                workers[i].Start(i);
            }
            for (int i = 0; i < M; i++)
                workers[i].Join();
            timer.Stop();

            Console.WriteLine("Время: " + timer.ElapsedMilliseconds + "\n");

            dict11 = dict11.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            dict11b = dict11b.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

            int z = 0;
            foreach (var item in dict11)
            {
                Console.WriteLine(item.Key + " " + item.Value);
                if (++z == 20) break;
            }
            z = 0; Console.WriteLine("========================");
            foreach (var item in dict11b)
            {
                Console.WriteLine(item.Key + " " + item.Value);
                if (++z == 20) break;
            }
            z = 0; Console.WriteLine("========================");
            foreach (var item in dict11c)
            {
                Console.WriteLine(item.Key + " " + item.Value);
            }
            Console.WriteLine("========================");
        }

        static void dowork(object o)
        {
            int w = (int)o;

            StreamReader f;
            List<string> buf;

            Dictionary<string, long> dict = new Dictionary<string, long>();
            Dictionary<char, long> dict2 = new Dictionary<char, long>();
            Dictionary<string, long> dict3 = new Dictionary<string, long>();

            string[] s;
            bool? isVowel;

            dict3.Add("Согласные", 0);
            dict3.Add("Гласные", 0);

            for (int i = w * (O / M); i < (w + 1) * (O / M); i++)
            { 
                f = File.OpenText(paths[i]);
                buf = new List<string>();
                while (!f.EndOfStream)
                {
                    buf.Add(f.ReadLine().ToUpper());
                }
                foreach (string line in buf)
                {
                    s = line.Split(new char[] { ',','.','!','?',';',':','"','\t','\n','[',']',' ','-' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var word in s)
                    {
                        if (dict.ContainsKey(word))
                            dict[word]++;
                        else
                            dict.Add(word, 1);
                    }
                    foreach (char ch in line)
                    {
                        if (dict2.ContainsKey(ch))
                            dict2[ch]++;
                        else
                            dict2.Add(ch, 1);
                    }
                    foreach (char ch in line)
                    {
                        isVowel = Program.isVowel(ch);
                        if (isVowel != null)
                            if ((bool)isVowel)
                                dict3["Гласные"]++;
                            else
                                dict3["Согласные"]++;
                    }
                }
                f.Close();
            }

            lock ("UPLOAD")
            {
                foreach (var pair in dict)
                {
                    if (dict11.ContainsKey(pair.Key))
                        dict11[pair.Key] += pair.Value;
                    else
                        dict11.Add(pair.Key, pair.Value);
                }
                foreach (var pair in dict2)
                {
                    if (dict11b.ContainsKey(pair.Key))
                        dict11b[pair.Key] += pair.Value;
                    else
                        dict11b.Add(pair.Key, pair.Value);
                }
                foreach (var pair in dict3)
                {
                    if (dict11c.ContainsKey(pair.Key))
                        dict11c[pair.Key] += pair.Value;
                    else
                        dict11c.Add(pair.Key, pair.Value);
                }
            }
        }

        #endregion 1.1

        #region 1.2

        static ConcurrentDictionary<string, long> dict12 = new ConcurrentDictionary<string, long>();
        static ConcurrentDictionary<char, long> dict12b = new ConcurrentDictionary<char, long>();
        static ConcurrentDictionary<string, long> dict12c = new ConcurrentDictionary<string, long>();
        static Dictionary<string, long> result12;

        static void v12()
        {
            Thread[] workers = new Thread[M];
            Stopwatch timer = new Stopwatch();

            Console.WriteLine("== Реализация 1.2. ==");
            timer.Start();
            for (int i = 0; i < M; i++)
            {
                workers[i] = new Thread(dowork12);
                workers[i].Start(i);
            }
            for (int i = 0; i < M; i++)
                workers[i].Join();
            timer.Stop();

            Console.WriteLine("Время: " + timer.ElapsedMilliseconds + "\n");

            result12 = dict12.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            var result12b = dict12b.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            var result12c = dict12c.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

            int k = 0;
            foreach (var pair in result12)
            {
                Console.WriteLine(pair.Key + " " + pair.Value);
                if (++k == 20) break;
            }
            k = 0;
            Console.WriteLine("========================");
            foreach (var pair in result12b)
            {
                Console.WriteLine(pair.Key + " " + pair.Value);
                if (++k == 20) break;
            }
            k = 0;
            Console.WriteLine("========================");
            foreach (var pair in result12c)
            {
                Console.WriteLine(pair.Key + " " + pair.Value);
                if (++k == 20) break;
            }
            Console.WriteLine("========================");
        }

        static void dowork12(object o)
        {
            int w = (int)o;
            StreamReader f;
            List<string> buf;
            string[] s;

            for (int i = w * (O / M); i < (w + 1) * (O / M); i++)
            {
                f = File.OpenText(paths[i]);

                buf = new List<string>();

                bool? isVowel;

                while (!f.EndOfStream)
                {
                    buf.Add(f.ReadLine().ToUpper());
                }
                foreach (string line in buf)
                {
                    s = line.Split(new char[] { ',', '.', '!', '?', ';', ':', '"', '\t', '\n', '[', ']', ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var word in s)
                    {
                        dict12.AddOrUpdate(word, 1, (x, y) => y + 1);
                    }
                    foreach (char ch in line)
                    {
                        dict12b.AddOrUpdate(ch, 1, (x, y) => y + 1);
                    }
                    foreach (char ch in line)
                    {
                        isVowel = Program.isVowel(ch);
                        if (isVowel != null)
                            if ((bool)isVowel)
                                dict12c.AddOrUpdate("Гласные", 1, (x, y) => y + 1);
                            else
                                dict12c.AddOrUpdate("Согласные", 1, (x, y) => y + 1);
                    }
                }
                f.Close();
            }
        }
        #endregion 1.2

        #region 2

        /// <summary>
        /// Потоки-читатели файла
        /// </summary>
        const int N = 5;
        
        /// <summary>
        /// Потоки-обработчики буфера
        /// </summary>
        const int K = 5;

        static ConcurrentBag<string> sharedBuffer = new ConcurrentBag<string>();
        static ConcurrentDictionary<string, long> sharedDict = new ConcurrentDictionary<string, long>();
        static Dictionary<string, long> result2;

        static bool isFinished = false;

        static void v2()
        {
            Thread[] fileReaders = new Thread[N];
            Thread[] analitics = new Thread[K];

            Stopwatch timer = new Stopwatch();

            Console.WriteLine("== Реализация 2. ==");

            timer.Start();
            for (int i = 0; i < N; i++)
            {
                fileReaders[i] = new Thread(ReadFromFile);
                fileReaders[i].Start(i);
            }
            for (int i = 0; i < K; i++)
            {
                analitics[i] = new Thread(ReadFromBuffer);
                analitics[i].Start();
            }
            for (int i = 0; i < N; i++)
            {
                fileReaders[i].Join();
            }
            isFinished = true;
            for (int i = 0; i < K; i++)
            {
                analitics[i].Join();
            }
            timer.Stop();

            Console.WriteLine("Время: " + timer.ElapsedMilliseconds + "\n");

            result2 = sharedDict.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            int q = 0;
        }

        static void ReadFromFile(object o)
        {
            int w = (int)o;
            StreamReader f;

            for (int i = w * (O / N); i < (w + 1) * (O / N); i++)
            {
                f = File.OpenText(paths[i]);
                while (!f.EndOfStream)
                {
                    sharedBuffer.Add(f.ReadLine().ToUpper());
                }
                f.Close();
            }
        }
        static void ReadFromBuffer()
        {
            string s;

            while (!(isFinished & sharedBuffer.IsEmpty))
            {
                if (sharedBuffer.TryTake(out s))
                {
                    foreach (var word in s.Split())
                    {
                        if (word.All(char.IsLetter) & word != "")
                            sharedDict.AddOrUpdate(word, 1, (x, y) => y + 1);
                    }
                }
            }
        }
        #endregion 2

        #region unneeded
        /*
        static Dictionary<string, long> etalonne = new Dictionary<string, long>();

        
        static void sXe()
        {
            List<string> buf;
            StreamReader f;
            Stopwatch timer = new Stopwatch();

            Console.WriteLine("== Последовательный алгоритм. ==");
            timer.Start();
            for (int i = 0; i < O; i++)
            {
                f = File.OpenText(paths[i]);
                buf = new List<string>();
                while (!f.EndOfStream)
                {
                    buf.Add(f.ReadLine().ToUpper());
                }
                f.Close();
                foreach (var line in buf)
                {
                    foreach (var word in line.Split())
                    {
                        if (word.All(char.IsLetter) & word != "")
                            if (etalonne.ContainsKey(word))
                                etalonne[word]++;
                            else
                                etalonne.Add(word, 1);
                    }
                }
            }
            timer.Stop();
            Console.WriteLine("Время: " + timer.ElapsedMilliseconds + "\n");

            etalonne = etalonne.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        static bool checkOnEtalonne(Dictionary<string, long> executee)
        {
            bool ok = true;
            foreach (var pair in executee)
            {
                if (!etalonne.ContainsKey(pair.Key) | !(etalonne[pair.Key] == pair.Value))
                    ok = false;
            }
            return ok;
        }
        */
        #endregion unneeded
    }
}
