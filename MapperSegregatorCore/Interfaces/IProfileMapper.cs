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
        Task ToTypeAsync(params MapperEnum[] enums);
        IProfileMapper Build<TOrigin, TDestination>(MapperDelegate<TOrigin, MapperOptionHandler, TDestination> func);
        IProfileMapper Build<TOrigin, TDestination>(MapperDelegate<TOrigin, TDestination> func);
        IProfileMapper Build<TOrigin, TDestination>(MapperDelegate<TOrigin, MapperOptionHandler, Task<TDestination>> func);
        IProfileMapper Build<TOrigin, TDestination>(MapperDelegate<TOrigin, Task<TDestination>> func);
    }
}
