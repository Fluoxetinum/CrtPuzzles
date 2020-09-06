using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Broker.Interfaces
{
	public interface ISubscribersDictionary
	{
		public int Count { get; }

		public IReadOnlyCollection<ISubscriber> this[string topic] { get; }

		public bool Remove(string topic, ISubscriber subscriber);

		public void Add(string topic, ISubscriber subscriber);
	}
}
