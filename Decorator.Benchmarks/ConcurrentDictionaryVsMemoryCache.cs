using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Decorator.Benchmarks
{
	public class ConcurrentDictionaryVsMemoryCache
	{
		private MemoryCache _mem;
		private ConcurrentDictionary<int, MethodInfo> _dic;
		private ConcurrentHashcodeDictionary<MethodInfo> _home;

		private int _lastAccessor;

		[GlobalSetup]
		public void Setup()
		{
			_mem = new MemoryCache(new MemoryCacheOptions());
			_dic = new ConcurrentDictionary<int, MethodInfo>();
			_home = new ConcurrentHashcodeDictionary<MethodInfo>();

			var methods = typeof(ConcurrentDictionaryVsMemoryCache).GetMethods();

			_mem.Set<MethodInfo>(typeof(string).GetHashCode(), methods[0]);
			_mem.Set<MethodInfo>(typeof(int).GetHashCode(), methods[1]);
			_mem.Set<MethodInfo>(typeof(long).GetHashCode(), methods[2]);

			_dic.TryAdd(typeof(string).GetHashCode(), methods[0]);
			_dic.TryAdd(typeof(int).GetHashCode(), methods[1]);
			_dic.TryAdd(typeof(long).GetHashCode(), methods[2]);

			_home.TryAdd(typeof(string).GetHashCode(), methods[0]);
			_home.TryAdd(typeof(int).GetHashCode(), methods[1]);
			_home.TryAdd(typeof(long).GetHashCode(), methods[2]);

			_lastAccessor = typeof(long).GetHashCode();

			MemRead();
			DicRead();
		}

		[Benchmark]
		public MethodInfo MemRead()
		{
			if(_mem.TryGetValue<MethodInfo>(_lastAccessor, out var method)) return method;
			else throw new Exception("lol nop");
		}

		[Benchmark]
		public MethodInfo DicRead()
		{
			if (_dic.TryGetValue(_lastAccessor, out var method)) return method;
			else throw new Exception("lol nop");
		}

		[Benchmark]
		public MethodInfo HomemadeRead()
		{
			if (_home.TryGetValue(_lastAccessor, out var method)) return method;
			else throw new Exception("lol nop");
		}
	}
}
