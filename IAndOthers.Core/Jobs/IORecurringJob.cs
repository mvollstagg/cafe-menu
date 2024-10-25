using IAndOthers.Core.IoC;

namespace IAndOthers.Core.Jobs
{
    public abstract class IORecurringJob : IOJobBase, IIODependencyScoped
    {
        public abstract string Code { get; }
        public abstract string Cron { get; }
    }
}
