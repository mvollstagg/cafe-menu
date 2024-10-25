namespace IAndOthers.Core.IoC
{
    public interface IIODependencySingleton { }
    public interface IIODependencySingleton<T> : IIODependencySingleton where T : class { }
}
