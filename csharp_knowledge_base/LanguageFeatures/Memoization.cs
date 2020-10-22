using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageFeatures
{
    /// <summary>
    /// https://www.aleksandar.io/post/memoization/
    /// </summary>
    public static class Memoization
    {
        public static async Task Example()
        {
            SlowSquareMemoization();
            MultiTreadMemoization();
            await AsyncMemoization();
            
            // TODO: think about
            // Everything so far has been useful, but in most real world scenarios, at least when I/O is involved, you need a expiration for your cache entries.
            //
            // (Un)fortunately there are many ways this could be done:
            //
            // Using a timer to evict stale dictionary entries
            // Sorting a dictionary and evicting first/last entries
            // Removing entries the moment they are requested
        }

        private static async Task AsyncMemoization()
        {
            var client = new HttpClient();
            
            Func<string, Task<HttpResponseMessage>> get = location => client.GetAsync(location);
            var memoizedGet = get.Memoize();

            var (elapsedFirst, _) = await MeasureAsync(() => memoizedGet("https://www.google.com"));
            Console.WriteLine(elapsedFirst);

            // returns memoized result
            var (elapsedSecond, _) = await MeasureAsync(() => memoizedGet("https://www.google.com"));
            Console.WriteLine(elapsedSecond);
        }

        private static void MultiTreadMemoization()
        {
            int calls = 0;
            Func<int, int> identity = n =>
            {
                Interlocked.Increment(ref calls);
                return n;
            };
            var memoized = identity.LazyMemoize();
            Parallel.For(0, 1000, i => memoized(0));
            Console.WriteLine(calls); // 1 - called just once
        }

        private static void SlowSquareMemoization()
        {
            Func<int, int> slowSquare = n =>
            {
                Thread.Sleep(100);
                return n * n;
            };
            
            Measure(() => slowSquare(2));
            Measure(() => slowSquare(2));
            Measure(() => slowSquare(2));
            var memoizedSlow = slowSquare.Memoize();
            Measure(() => memoizedSlow(2));
            Measure(() => memoizedSlow(2));
            Measure(() => memoizedSlow(2));
            Measure(() => memoizedSlow(4));
            Measure(() => memoizedSlow(4));
        }

        private static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> f)
        {
            var cache = new ConcurrentDictionary<T, TResult>();
            return a => cache.GetOrAdd(a, f);
        }
        
        private static Func<T, TResult> LazyMemoize<T, TResult>(this Func<T, TResult> f)
        {
            var cache = new ConcurrentDictionary<T, Lazy<TResult>>();
            return a => cache.GetOrAdd(a, new Lazy<TResult>(() => f(a))).Value;
        }

        #region Measure

        // TODO: extract this to separate class or think about how do it better

        private static async Task<(TimeSpan, HttpResponseMessage)> MeasureAsync(Func<Task<HttpResponseMessage>> p0)
        {
            var sw = new Stopwatch();
            sw.Start();
            var result = await p0();
            sw.Stop();
            return (sw.Elapsed, result);
        }

        private static TimeSpan Measure(Action action)
        {
            var sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();
            return sw.Elapsed;
        }

        #endregion
    }
}