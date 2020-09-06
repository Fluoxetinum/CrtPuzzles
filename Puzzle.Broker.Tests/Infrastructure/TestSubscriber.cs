using System;
using System.Collections.Generic;
using System.Text;
using Puzzle.Broker.Interfaces;
using Xunit;

namespace Puzzle.Broker.Tests.Infrastructure
{
	public class TestSubscriber : ISubscriber
	{
		private readonly HashSet<string> _topicsHashSet;


		public TestSubscriber()
		{
			Id = Guid.NewGuid();
			_topicsHashSet = new HashSet<string>();
		}


		public TestSubscriber(IEnumerable<string> topics)
		{
			_topicsHashSet = new HashSet<string>(topics);
		}

		public Guid Id { get; }


		public IReadOnlyCollection<string> Topics => _topicsHashSet;


		public bool Equals(ISubscriber other)
		{
			if (other == null) return false;
			return Id == other.Id;
		}


		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ISubscriber)obj);
		}


		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}


		public bool Contains(string topic)
		{
			return _topicsHashSet.Contains(topic);
		}


		public void Notify(IMessage message)
		{
			Assert.Contains(message.Topic, _topicsHashSet);
		}
	}
}
