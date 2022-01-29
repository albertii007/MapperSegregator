using MapperSegregator.Base;
using System.Threading.Tasks;

namespace MapperSegregator.Interfaces
{
    public delegate TDestination MapperDelegate<in TOrigin, TMapperOptions, out TDestination>(TOrigin origin, MapperOptionHandler mapperOption);

    public delegate TDestination MapperDelegate<in TOrigin, out TDestination>(TOrigin origin);

    public enum MapperEnum
    {
        Single,
        List,
        Array,
        Queryable
    }

    public interface IProfileMapper
    {
        Task BuildAsync<TOrigin, TDestination>(MapperDelegate<TOrigin, MapperOptionHandler, TDestination> func);
        Task BuildAsync<TOrigin, TDestination>(MapperDelegate<TOrigin, TDestination> func);
        Task BuildAsync<TOrigin, TDestination>(MapperDelegate<TOrigin, MapperOptionHandler, Task<TDestination>> func);
        Task BuildAsync<TOrigin, TDestination>(MapperDelegate<TOrigin, Task<TDestination>> func);
        Task BuildAsync<TOrigin, TDestination>(MapperEnum[] enums, MapperDelegate<TOrigin, MapperOptionHandler, TDestination> func);
        Task BuildAsync<TOrigin, TDestination>(MapperEnum[] enums, MapperDelegate<TOrigin, TDestination> func);
        Task BuildAsync<TOrigin, TDestination>(MapperEnum enumVal, MapperDelegate<TOrigin, MapperOptionHandler, TDestination> func);
        Task BuildAsync<TOrigin, TDestination>(MapperEnum enumVal, MapperDelegate<TOrigin, TDestination> func);
    }
}
