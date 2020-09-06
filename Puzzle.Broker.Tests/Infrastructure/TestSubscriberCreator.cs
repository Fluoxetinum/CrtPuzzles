using System;
using System.Collections.Generic;
using System.Text;
using Puzzle.Broker.Interfaces;
using Puzzle.Common;

namespace Puzzle.Broker.Tests.Infrastructure
{
	public class TestSubscriberCreator : ISubscriberCreator
	{
		public ISubscriber Create(ISimpleLogger logger, List<string> topics)
		{
			return new TestSubscriber(topics);
		}
	}
}
