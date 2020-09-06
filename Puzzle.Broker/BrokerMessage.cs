using System;
using System.Collections.Generic;
using System.Text;
using Puzzle.Broker.Interfaces;

namespace Puzzle.Broker
{
	public class BrokerMessage : IMessage
	{
		private readonly Guid _id;
		private readonly string _topic;
		private readonly string _body;

		public BrokerMessage(Guid id, string topic, string body)
		{
			_id = id;
			_topic = topic;
			_body = body;
		}


		Guid IMessage.Id => _id;


		string IMessage.Topic => _topic;


		string IMessage.Body => _body;
	}
}
