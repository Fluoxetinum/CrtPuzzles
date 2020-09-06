using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Broker.Infrastructure
{
	public static class GuidLogging
	{
		public static string ToShortString(this Guid guid)
		{
			return $"{guid.ToString().Substring(0, 4)}...";
		}
	}
}
