using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SharpLimiter
{
    public class SharpLimiter : IDisposable
    {
        private bool _duty;
        private uint _initialCapacity;
        private static Thread _producer;
        private static Thread _consumer;
        private readonly uint _capacity;
        private readonly TimeSpan _period;
        private BlockingCollection<Task> _job_pool;
        private  BlockingCollection<bool> _permssion_pool = new BlockingCollection<bool>();

        public SharpLimiter(uint capacity, TimeSpan period, uint initialCapacity = 0)
        {
            _capacity = capacity;
            _period = period;
            _initialCapacity = initialCapacity;
            Warmup();
        }

        public void Dispose()
        {
            _duty = false;
            _permssion_pool.Add(true);
            var _tmp = new Task(delegate {  });
            var result = _job_pool.TryAdd(_tmp);
            _producer.Join();
            _consumer.Join();
            _job_pool.Dispose();
            _permssion_pool.Dispose();
        }

        private void Warmup()
        {
            _permssion_pool = new BlockingCollection<bool>();
            _job_pool = new BlockingCollection<Task>();
            for (var i = 0; i < _initialCapacity; i++) _permssion_pool.Add(true);
            _duty = true;
            _producer = new Thread(() =>
            {
                while (_duty)
                {
                    if (_permssion_pool.Count < _capacity) _permssion_pool.Add(true);
                    Thread.Sleep((int) (_period.TotalMilliseconds / _capacity));
                }
            })
            {
                IsBackground = true
            };
            _producer.Start();
            _consumer = new Thread(() =>
            {
                while (_duty)
                {
                    var job = _job_pool.Take();
                    _permssion_pool.Take();
                    try
                    {
                        job.Start();
                    }
                    catch(Exception e )
                    {
                        throw new Exception($"Limiter job execution caused exception: {e.Message}");
                    }
                }
            })
            {
                IsBackground = true
            };
            _consumer.Start();
        }
        public Task ExecuteOnLimiter(Action? action=null)
        {
            var tmp = action == null ? new Task(delegate {  }) : new Task(action.Invoke);
            _job_pool.Add(tmp);
            return tmp;
        }
    }
}