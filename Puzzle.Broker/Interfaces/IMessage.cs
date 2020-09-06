using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Broker.Interfaces
{
	public interface IMessage
	{
		public Guid Id { get; }

		public string Topic { get; }

		public string Body { get; }
	}
}
