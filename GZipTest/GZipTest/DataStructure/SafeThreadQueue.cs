using System.Collections.Generic;
using GZipTest.DataTypes;

namespace GZipTest.DataStructure
{
    public class SafeThreadQueue<T> : IWriteQueue<T>, IReadQueue<T> where T : ByteBlock
    {
        private readonly Queue<T> _queue;
        private int _counter;
        private readonly ThreadLock _lock;

        public SafeThreadQueue()
        {
            _queue = new Queue<T>();
            _lock = new ThreadLock();
        }

        public int Count { get; private set; }

        public bool IsEnqueueStoped { get; private set; }

        public void Enqueue(T block)
        {
            long serialNumber = block.SerialNumber;
            _lock.Enter();
            while (serialNumber != _counter)
            {
                _lock.Wait();
            }

            _queue.Enqueue(block);
            Count = _queue.Count;
            _counter++;
            _lock.Leave();
        }

        public T Dequeue()
        {
            T result = default(T);
            _lock.Enter();
            while (_queue.Count == 0 && !IsEnqueueStoped)
            {
                _lock.Wait();
            }

            if (_queue.Count != 0)
                result = _queue.Dequeue();
            Count = _queue.Count;
            _lock.Leave();
            return result;
        }

        public void StopEnqueue()
        {
            _lock.Enter();
            IsEnqueueStoped = true;
            Count = _queue.Count;
            _counter = 0;
            _lock.Leave();
        }
    }

    public interface IWriteQueue<in T>
    {
        void Enqueue(T block);
    }

    public interface IReadQueue<out T>
    {
        T Dequeue();

        int Count { get; }

        bool IsEnqueueStoped { get; }
    }
}
