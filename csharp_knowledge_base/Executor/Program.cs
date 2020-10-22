using System;
using LanguageFeatures;

namespace Executor
{
    class Program
    {
        static void Main(string[] args)
        {
            Memoization.Example().Wait();

            Console.WriteLine("Hello World!");
        }
    }
}