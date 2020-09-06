using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Broker.Interfaces
{
	public interface IBrokerQueue<T> : IDisposable
	{
		public int Count { get; }

		public void Enqueue(T message);

		public bool TryDequeue(TimeSpan interval, out T value);
	}
}
