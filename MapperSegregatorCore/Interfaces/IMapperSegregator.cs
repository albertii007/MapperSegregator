using System.Threading.Tasks;

namespace MapperSegregator.Interfaces
{
    public interface IMapperSegregator
    {
        Task<TDestination> MapAsync<TOrigin, TDestination>(TOrigin origin, params object[] objects) where TDestination : class;
        TDestination Map<TOrigin, TDestination>(TOrigin origin, params object[] objects) where TDestination : class;
    }
}
