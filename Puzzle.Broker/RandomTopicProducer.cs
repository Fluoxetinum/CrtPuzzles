using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Timers;
using Puzzle.Broker.Infrastructure;
using Puzzle.Broker.Interfaces;
using Puzzle.Common;

namespace Puzzle.Broker
{
	public class RandomTopicProducer : IProducer
	{
		private readonly Guid _id;
		private readonly ISimpleLogger _logger;
		private readonly IPostable _postable;
		private readonly List<string> _topicsList;
		private readonly System.Timers.Timer _producerTimer;
		private readonly ThreadLocal<Random> _random;
		

		public RandomTopicProducer(IPostable postable, ISimpleLogger logger, IReadOnlyCollection<string> topics)
		{
			if (postable == null)
			{
				throw new ArgumentNullException(nameof(postable));
			}
			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}
			if (topics == null)
			{
				throw new ArgumentNullException(nameof(topics));
			}

			_id = Guid.NewGuid();
			_logger = logger;
			_postable = postable;
			_topicsList = new List<string>();
			foreach (string topic in topics)
			{
				_topicsList.Add(topic);
			}
			_producerTimer = new System.Timers.Timer();
			_random = new ThreadLocal<Random>(() => new Random());
		}


		Guid IProducer.Id => _id;


		void IProducer.StartProducing(TimeSpan interval)
		{
			_producerTimer.Interval = interval.TotalMilliseconds;
			_producerTimer.Elapsed += ProducerTimerOnElapsed;
			_producerTimer.Start();
		}


		void IProducer.StopProducing()
		{
			_producerTimer.Stop();
		}


		void IDisposable.Dispose()
		{
			_producerTimer?.Dispose();
		}


		private void ProducerTimerOnElapsed(object sender, ElapsedEventArgs e)
		{
			Guid messageId = Guid.NewGuid();
			int topicI = _random.Value.Next(0, _topicsList.Count);
			string topic = _topicsList[topicI];
			string body = $"Body of the message#{messageId}.";
			_postable.Post(new BrokerMessage(messageId, topic, body));

			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append($"[{DateTime.Now:HH:mm:ss.fff}]:");
			stringBuilder.Append($"Producer#{_id.ToShortString()} in Thread#{Thread.CurrentThread.ManagedThreadId}" +
			                     $" posted a message#{messageId.ToShortString()} (Topic : {topic}).");
			_logger.Info(stringBuilder.ToString());
		}
	}
}
