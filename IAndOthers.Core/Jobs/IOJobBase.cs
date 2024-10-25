using IAndOthers.Core.IoC;

namespace IAndOthers.Core.Jobs
{
    public abstract class IOJobBase
    {
        public abstract Task Execute();
    }
}
