using System;
using System.Collections.Generic;
using System.Text;
using Puzzle.Common;

namespace Puzzle.Broker.Tests.Infrastructure
{
	public class TestLogger : ISimpleLogger
	{
		public void Info(string message)
		{
			;
		}


		public void Error(Exception e, string message)
		{
			;
		}
	}
}
