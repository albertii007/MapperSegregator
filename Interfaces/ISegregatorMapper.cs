using System.Threading.Tasks;

namespace MapperSegregator.Interfaces
{
    public interface ISegregatorMapper
    {
        Task<TDestination> Map<TOrigin, TDestination>(TOrigin origin, params object[] objects) where TDestination : class;
    }
}
