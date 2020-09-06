using System;
using System.Collections.Generic;
using System.Text;
using Puzzle.Common;

namespace Puzzle.Broker.Interfaces
{
	public interface IProducerCreator
	{
		public IProducer Create(IPostable postable, ISimpleLogger logger, List<string> topics);
	}
}
