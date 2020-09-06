using System;
using System.Collections.Generic;
using System.Text;
using Puzzle.Broker.Infrastructure;
using Puzzle.Broker.Interfaces;

namespace Puzzle.Broker
{
    public class BrokerManager : IBrokerManager, IDisposable
    {
        private Broker _instance;
        private readonly object _lock;
        private readonly int _queueCapacity;


        public BrokerManager(int queueCapacity)
        {
            _queueCapacity = queueCapacity;
            _lock = new object();
        }


        private Broker GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Broker(  
                            new BrokerQueue<IMessage>(_queueCapacity), 
                            new TopicsDictionary()
                            );
                    }
                }
            }

            return _instance;
        }


        ISubscribable IBrokerManager.GetSubscribable()
        {
            return (ISubscribable) GetInstance();
        }


        IPostable IBrokerManager.GetPostable()
        {
            return (IPostable) GetInstance();
        }


        void IDisposable.Dispose()
        {
            ((IDisposable)_instance)?.Dispose();
        }
    }
}
