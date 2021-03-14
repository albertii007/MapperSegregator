using System;
using System.Threading.Tasks;

namespace MapperSegregator.Interfaces
{
    public interface IProfileMapper
    {
        Task Builder<TOrigin, TDestination>(Func<TOrigin, object[], TDestination> func);
        Task Builder<TOrigin, TDestination>(Func<TOrigin, object[], Task<TDestination>> func);
        TClass GetFromParams<TClass>(object[] objList);
        Task<TClass> GetFromParamsAsync<TClass>(object[] objList);
    }
}
