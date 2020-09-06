using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Puzzle.Broker.Infrastructure;
using Puzzle.Broker.Interfaces;
using Puzzle.Common;

namespace Puzzle.Broker
{
	public class LoggingSubscriber : ISubscriber
	{
		private readonly Guid _id;
		private readonly ISimpleLogger _logger;
		private readonly HashSet<string> _topicsSet;

		public LoggingSubscriber(ISimpleLogger logger, IReadOnlyCollection<string> topics)
		{
			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}
			if (topics == null)
			{
				throw new ArgumentNullException(nameof(topics));
			}

			_logger = logger;
			_id = Guid.NewGuid();
			_topicsSet = new HashSet<string>(topics);
		}


		Guid ISubscriber.Id => _id;


		IReadOnlyCollection<string> ISubscriber.Topics => _topicsSet;


		bool ISubscriber.Contains(string topic)
		{
			return _topicsSet.Contains(topic);
		}


		void ISubscriber.Notify(IMessage message)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append($"[{DateTime.Now:HH:mm:ss.fff}]:");
			stringBuilder.AppendLine($"Subscriber#{_id.ToShortString()} in Thread#{Thread.CurrentThread.ManagedThreadId}" +
			                         $" received a message#{message.Id.ToShortString()}.");
			if (message != null)
			{
				stringBuilder.AppendLine($"Topic : {message.Topic}");
				stringBuilder.Append($"Body : {message.Body}");
			}
			_logger.Info(stringBuilder.ToString());
		}


		bool IEquatable<ISubscriber>.Equals(ISubscriber other)
		{
			if (other == null) return false;
			return _id == other.Id;
		}


		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (obj.GetType() != this.GetType()) return false;
			return ((IEquatable<ISubscriber>)this).Equals((ISubscriber)obj);
		}


		public override int GetHashCode()
		{
			return _id.GetHashCode();
		}
	}
}
