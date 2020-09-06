using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Broker.Interfaces
{
	public interface ISubscriber : IEquatable<ISubscriber>
	{
		public Guid Id { get; }

		public IReadOnlyCollection<string> Topics { get; }

		public bool Contains(string topic);

		public void Notify(IMessage message);
	}
}
