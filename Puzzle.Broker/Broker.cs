using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Puzzle.Broker.Interfaces;

namespace Puzzle.Broker
{
	public class Broker : IPostable, ISubscribable, IDisposable
	{
		private static readonly int DefaultNotifierThreadsCount = Environment.ProcessorCount;
		private volatile bool _stopFlag;
		private readonly Thread[] _notifierThreads;
		private readonly IBrokerQueue<IMessage> _queue;
		private readonly ISubscribersDictionary _dictionary;


		public Broker(IBrokerQueue<IMessage> queue, ISubscribersDictionary dictionary)
			: this(queue, dictionary, DefaultNotifierThreadsCount)
		{

		}


		public Broker(IBrokerQueue<IMessage> queue, ISubscribersDictionary dictionary, int notifierThreadsCount)
		{
			if (queue == null)
			{
				throw new ArgumentNullException(nameof(queue));
			}
			if (dictionary == null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			_stopFlag = false;
			_queue = queue;
			_dictionary = dictionary;
			_notifierThreads = new Thread[notifierThreadsCount];

			for (int i = 0; i < notifierThreadsCount; i++)
			{
				_notifierThreads[i] = new Thread(NotifySubscribers);
				_notifierThreads[i].Start();
			}
		}


		void IPostable.Post(IMessage message)
		{
			_queue.Enqueue(message);
		}


		void ISubscribable.Subscribe(ISubscriber subscriber)
		{
			foreach (string subscriberTopic in subscriber.Topics)
			{
				_dictionary.Add(subscriberTopic, subscriber);
			}
		}


		void ISubscribable.Unsubscribe(ISubscriber subscriber)
		{
			foreach (string subscriberTopic in subscriber.Topics)
			{
				_dictionary.Remove(subscriberTopic, subscriber);
			}
		}


		void IDisposable.Dispose()
		{
			_queue?.Dispose();
			_stopFlag = true;

			foreach (var notifierThread in _notifierThreads)
			{
				notifierThread.Join();
			}
		}


		private void NotifySubscribers()
		{
			while (!_stopFlag)
			{
				IMessage message;
				bool result = _queue.TryDequeue(TimeSpan.FromSeconds(1), out message);

				if (!result)
				{
					continue;
				}

				IReadOnlyCollection<ISubscriber> subscribers = _dictionary[message.Topic];
				foreach (ISubscriber subscriber in subscribers)
				{
					subscriber.Notify(message);
				}
			}
		}
	}
}
