using BenchmarkDotNet.Running;
using Benchmarks;

var _ = BenchmarkRunner.Run<Perft>();
