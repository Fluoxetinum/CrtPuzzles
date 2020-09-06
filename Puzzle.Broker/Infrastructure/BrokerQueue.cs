using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Puzzle.Broker.Interfaces;

namespace Puzzle.Broker.Infrastructure
{
	internal class BrokerQueue<T> : IBrokerQueue<T>
	{
		private const int DefaultCapacity = 10;
		private int _count;
		private readonly Queue<T> _queue;
		private readonly object _lock;
		private readonly SemaphoreSlim _producerSemaphore;
		private readonly SemaphoreSlim _consumerSemaphore;


		public BrokerQueue() : this(DefaultCapacity)
		{

		}


		public BrokerQueue(int capacity)
		{
			_count = 0;
			_queue = new Queue<T>();
			_lock = new object();
			_consumerSemaphore = new SemaphoreSlim(initialCount: capacity, maxCount: capacity);
			_producerSemaphore = new SemaphoreSlim(initialCount: 0, maxCount: capacity);
		}


		public int Count => _count;


		public void Dispose()
		{
			_consumerSemaphore.Dispose();
			_producerSemaphore.Dispose();
		}


		public void Enqueue(T message)
		{
			_consumerSemaphore.Wait();

			lock (_lock)
			{
				_queue.Enqueue(message);
			}

			Interlocked.Increment(ref _count);

			_producerSemaphore.Release();
		}


		public bool TryDequeue(TimeSpan interval, out T value)
		{
			bool result = _producerSemaphore.Wait(interval);
			value = default(T);

			if (!result)
			{
				return false;
			}

			lock (_lock)
			{
				value = _queue.Dequeue();
			}

			Interlocked.Decrement(ref _count);

			_consumerSemaphore.Release();

			return true;
		}
	}
}
