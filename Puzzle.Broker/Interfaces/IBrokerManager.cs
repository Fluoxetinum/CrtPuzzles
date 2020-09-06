using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Broker.Interfaces
{
	public interface IBrokerManager
	{
		public ISubscribable GetSubscribable();

		public IPostable GetPostable();
	}
}
