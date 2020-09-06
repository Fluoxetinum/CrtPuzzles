using System;
using System.Collections.Generic;
using System.Text;
using Puzzle.Broker.Interfaces;
using Puzzle.Common;

namespace Puzzle.Broker
{
	public class SubscriberCreator : ISubscriberCreator
	{
		ISubscriber ISubscriberCreator.Create(ISimpleLogger logger, List<string> topics)
		{
			return new LoggingSubscriber(logger, topics);
		}
	}
}
