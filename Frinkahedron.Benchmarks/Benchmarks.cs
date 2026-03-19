using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Benchmarks
{
    [MemoryDiagnoser]
    public class Benchmarks
    {
        [GlobalSetup]
        public void GlobalSetup()
        {
        }

        [Benchmark]
        public void Benchmark1()
        {
            // TODO: Replace this with code to benchmark
            Thread.Sleep(50);
        }

        [Benchmark]
        public void Benchmark2()
        {
            // TODO: Replace this with code to benchmark
            Thread.Sleep(25);
        }
    }
}
