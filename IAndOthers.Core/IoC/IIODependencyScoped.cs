namespace IAndOthers.Core.IoC
{
    public interface IIODependencyScoped { }
    public interface IIODependencyScoped<T> : IIODependencyScoped where T : class { }
}
