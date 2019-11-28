using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System;
using System.Reflection;
using FastMember;
using Sigil;
//using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.Windows;
using Magnum.Reflection;
using BenchmarkDotNet.Diagnosers;
using System.Runtime.CompilerServices;
using Fasterflect;

namespace Benchmark
{
	public class Program
	{
		static void Main(string[] args)
		{
			var summary = BenchmarkRunner.Run<Getters>();
			//Console.ReadLine();
			var summary2 = BenchmarkRunner.Run<Setters>();
			//Console.ReadLine();
		}
	}
}