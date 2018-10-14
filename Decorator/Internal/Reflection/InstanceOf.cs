using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Decorator
{
	internal static class InstanceOf<T>
	{
		public static T Create()
			=> Constructor();

		private static readonly Func<T> Constructor
			= Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
	}

	/*
	internal static class PreGenInstanceOf<T>
	{
		private const int MaxInQueue = 1_000_000;
		private const int QueueDelay = 100;

		private static ConcurrentQueue<T> _creationCache;

		static PreGenInstanceOf()
		{
			_creationCache = new ConcurrentQueue<T>();

			ConstructionQueueAsync();

			for (int i = 0; i < 100; i++)
			{
				_creationCache.Enqueue(Constructor());
			}
		}

		private static async void ConstructionQueueAsync()
		{
			while (_creationCache.Count < MaxInQueue)
			{
				for (int i = 0; i < MaxInQueue; i++)
				{
					_creationCache.Enqueue(Constructor());
				}

				await Task.Delay(QueueDelay);
			}
		}

		public static T Create()
		{
			if (_creationCache.TryDequeue(out var item))
			{
				return item;
			}
			else
			{
				ConstructionQueueAsync();
				return Constructor();
			}
		}

		private static readonly Func<T> Constructor
			= Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
	}
	*/
}