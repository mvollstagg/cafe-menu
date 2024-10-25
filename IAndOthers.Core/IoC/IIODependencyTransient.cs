namespace IAndOthers.Core.IoC
{
    public interface IIODependencyTransient { }
    public interface IIODependencyTransient<T> : IIODependencyTransient where T : class { }
}
