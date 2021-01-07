using System.Threading;

namespace GZipTest.DataStructure
{
    public class ThreadLock
    {
        private int _waiter = 0;
        private AutoResetEvent _waiterLock;

        public ThreadLock()
        {
            _waiter = 0;
            _waiterLock = new AutoResetEvent(false);
        }

        public void Enter()
        {
            if (Interlocked.Increment(ref _waiter) == 1)
                return;
            _waiterLock.WaitOne();
        }

        public void Wait()
        {
            Leave();
            Thread.Sleep(1);
            Enter();
        }

        public void Leave()
        {
            if (Interlocked.Decrement(ref _waiter) == 0)
                return;
            _waiterLock.Set();
        }
    }
}
