using System;
using System.Collections.Generic;
using System.Text;
using Puzzle.Common;

namespace Puzzle.Broker.Interfaces
{
	public interface ISubscriberCreator
	{
		ISubscriber Create(ISimpleLogger logger, List<string> topics);
	}
}
