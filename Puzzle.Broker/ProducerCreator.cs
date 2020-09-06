using System;
using System.Collections.Generic;
using System.Text;
using Puzzle.Broker.Interfaces;
using Puzzle.Common;

namespace Puzzle.Broker
{
	public class ProducerCreator : IProducerCreator
	{
		IProducer IProducerCreator.Create(IPostable postable, ISimpleLogger logger, List<string> topics)
		{
			return new RandomTopicProducer(postable, logger, topics);
		}
	}
}
