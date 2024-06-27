using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebServer
{
    public abstract class ContextHandler
    {
        public abstract bool CanHandleContext(Context context);
        public abstract Task<bool> HandleContext(Context context);
    }
}
