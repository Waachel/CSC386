using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaRace.Data_Queues
{
    public class MessagePool<T> where T : DataMessage, new()
    {
        private readonly Stack<T> pool = new Stack<T>();
        private readonly int maxPoolSize;

        public MessagePool(int maxPoolSize)
        {
            this.maxPoolSize = maxPoolSize;
        }

        public T GetMessage()
        {
            if (pool.Count > 0)
            {
                return pool.Pop();
            }
            else
            {
                return new T();
            }
        }

        public void ReleaseMessage(T message)
        {
            if (pool.Count < maxPoolSize)
            {
                pool.Push(message);
            }
        }
    }
}
