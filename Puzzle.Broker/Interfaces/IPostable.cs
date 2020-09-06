using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Broker.Interfaces
{
	public interface IPostable
	{
		void Post(IMessage message);
	}
}
