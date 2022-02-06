using System.Collections.Concurrent;

namespace SharpLimiter
{
    public class SharpLimiter
    {
        private BlockingCollection<bool> _permssion_pool = new BlockingCollection<bool>();
    }
}
