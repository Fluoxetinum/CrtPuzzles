using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Broker.Interfaces
{
	public interface IProducer : IDisposable
	{
		public Guid Id { get; }

		public void StartProducing(TimeSpan interval);

		public void StopProducing();
	}
}
