using MapperSegregator.Base;
using System;
using System.Threading.Tasks;

namespace MapperSegregator.Interfaces
{
    public interface IProfileMapper
    {
        Task Builder<TOrigin, TDestination>(Func<TOrigin, MapperOptionHandler, TDestination> func);
        Task Builder<TOrigin, TDestination>(Func<TOrigin, MapperOptionHandler, Task<TDestination>> func);
    }
}
