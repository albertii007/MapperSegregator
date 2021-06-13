using MapperSegregator.Base;
using System;
using System.Threading.Tasks;

namespace MapperSegregator.Interfaces
{
    public interface IProfileMapper
    {
        Task BuildAsync<TOrigin, TDestination>(Func<TOrigin, MapperOptionHandler, TDestination> func);
        Task BuildAsync<TOrigin, TDestination>(Func<TOrigin, MapperOptionHandler, Task<TDestination>> func);
        Task BuildAsync<TOrigin, TDestination>(MapperEnum[] enums, Func<TOrigin, MapperOptionHandler, TDestination> func);
        Task BuildAsync<TOrigin, TDestination>(Func<TOrigin, TDestination> func);
        Task BuildAsync<TOrigin, TDestination>(Func<TOrigin, Task<TDestination>> func);
        Task BuildAsync<TOrigin, TDestination>(MapperEnum[] enums, Func<TOrigin, TDestination> func);
        Task BuildAsync<TOrigin, TDestination>(MapperEnum enumFunc, Func<TOrigin, TDestination> func);
    }
}
