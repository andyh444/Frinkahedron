using BenchmarkDotNet.Running;
using Frinkahedron.Benchmarks;

internal class Program
{
    private static void Main(string[] args)
    {
        BenchmarkRunner.Run<Benchmarks>();
    }
}