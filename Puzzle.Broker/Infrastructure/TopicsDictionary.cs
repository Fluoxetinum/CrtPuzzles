using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Puzzle.Broker.Interfaces;

namespace Puzzle.Broker.Infrastructure
{
	internal class TopicsDictionary : ISubscribersDictionary
	{
		private static readonly int DefaultLocksCount = Environment.ProcessorCount * 4;
		private const int DefaultExpandBoundary = 10;
		private const int DefaultMaxhHashTableLenght = 65_536;
		private volatile int _count;
		private readonly int _expandBoundary;
		private readonly object[] _locks;
		private LinkedList<TopicDictionaryEntry>[] _hashTable;


		public TopicsDictionary() : this(DefaultLocksCount, DefaultExpandBoundary)
		{

		}


		public TopicsDictionary(int locksCount, int expandBoundary)
		{
			_expandBoundary = expandBoundary;
			_count = 0;
			_locks = new object[locksCount];
			for (int i = 0; i < _locks.Length; i++)
			{
				_locks[i] = new object();
			}
			_hashTable = new LinkedList<TopicDictionaryEntry>[locksCount];
		}


		public int Count => _count;


		public IReadOnlyCollection<ISubscriber> this[string topic]
		{
			get
			{
				var subscribers = new List<ISubscriber>();
				if (topic == null)
				{
					return subscribers;
				}
				object l = GetLock(topic);
				lock (l)
				{
					int code = GetCode(topic);
					if (_hashTable[code] == null)
					{
						return subscribers;
					}
					foreach (TopicDictionaryEntry entry in _hashTable[code])
					{
						if (entry.Topic.Equals(topic))
						{
							subscribers.Add(entry.Subscriber);
						}
					}
				}
				return subscribers;
			}
		}


		public bool Remove(string topic, ISubscriber subscriber)
		{
			bool result = false;

			if (topic == null || subscriber == null)
			{
				return result;
			}

			object l = GetLock(topic);
			lock (l)
			{
				int code = GetCode(topic);
				if (_hashTable[code] == null)
				{
					return result;
				}
				result = _hashTable[code].Remove(new TopicDictionaryEntry(topic, subscriber));
				if (result)
				{
					Interlocked.Decrement(ref _count);
				}
			}
			return result;
		}


		public void Add(string topic, ISubscriber subscriber)
		{
			if (topic == null)
			{
				throw new ArgumentNullException(nameof(topic));
			}
			if (subscriber == null)
			{
				throw new ArgumentNullException(nameof(subscriber));
			}

			bool expandRequired = false;
			object l = GetLock(topic);
			lock (l)
			{
				int code = GetCode(topic);
				if (_hashTable[code] == null)
				{
					_hashTable[code] = new LinkedList<TopicDictionaryEntry>();
				}

				_hashTable[code].AddLast(new TopicDictionaryEntry(topic, subscriber));
				Interlocked.Increment(ref _count);

				if (_hashTable[code].Count > _expandBoundary &&
					_hashTable.Length < DefaultMaxhHashTableLenght)
				{
					expandRequired = true;
				}
			}

			if (expandRequired)
			{
				Expand(_hashTable);
			}
		}


		private void Expand(LinkedList<TopicDictionaryEntry>[] hashTable)
		{
			bool[] lockTakenArr = new bool[_locks.Length];

			for (int i = 0; i < _locks.Length; i++)
			{
				Monitor.Enter(_locks[i], ref lockTakenArr[i]);
			}

			if (hashTable == _hashTable)
			{
				_count = 0;

				LinkedList<TopicDictionaryEntry>[] tempHashTable = _hashTable;

				_hashTable = new LinkedList<TopicDictionaryEntry>[_hashTable.Length * 3];

				foreach (var subscribersList in tempHashTable)
				{
					if (subscribersList == null)
					{
						continue;
					}

					foreach (var entry in subscribersList)
					{
						Add(entry.Topic, entry.Subscriber);
					}
				}
			}

			for (int i = 0; i < _locks.Length; i++)
			{
				if (lockTakenArr[i])
				{
					Monitor.Exit(_locks[i]);
				}
			}
		}


		private object GetLock(string topic)
		{
			int code = Math.Abs(topic.GetHashCode());
			int lockI = code % _locks.Length;
			object l = _locks[lockI];
			return l;
		}

		private int GetCode(string topic)
		{
			return Math.Abs(topic.GetHashCode() % _hashTable.Length);
		}


		private readonly struct TopicDictionaryEntry : IEquatable<TopicDictionaryEntry>
		{
			public string Topic { get; }
			public ISubscriber Subscriber { get; }

			public TopicDictionaryEntry(string topic, ISubscriber subscriber)
			{
				Topic = topic;
				Subscriber = subscriber;
			}

			public bool Equals(TopicDictionaryEntry other)
			{
				return this.Subscriber.Equals(other.Subscriber) &&
					   this.Topic.Equals(other.Topic);
			}

			public override bool Equals(object obj)
			{
				if (obj == null) return false;
				if (obj.GetType() != this.GetType()) return false;
				return Equals((TopicDictionaryEntry)obj);
			}

			public override int GetHashCode()
			{
				return Subscriber.GetHashCode();
			}
		}
	}
}
