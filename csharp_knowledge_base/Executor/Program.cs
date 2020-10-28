using System;
using LanguageFeatures;
using Repositories;

namespace Executor
{
    class Program
    {
        static void Main(string[] args)
        {
            // Memoization.Example().Wait();
            
            CollectionOrientedRepositoryExample.Run();

            Console.WriteLine("Hello World!");
        }
    }
}