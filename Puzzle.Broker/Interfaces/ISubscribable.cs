using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Broker.Interfaces
{
	public interface ISubscribable
	{
		void Subscribe(ISubscriber subscriber);

		void Unsubscribe(ISubscriber subscriber);
	}
}
