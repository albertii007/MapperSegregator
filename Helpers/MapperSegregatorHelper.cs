using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace MapperSegregator.Helpers
{
    public static class MapperSegregatorHelper
    {
        private static IMapperSegregator _mapperSegregator;

        public static void Configure(IMapperSegregator smartMapper)
        {
            _mapperSegregator = smartMapper ?? throw new ArgumentNullException(nameof(smartMapper));
        }

        public static IMapperSegregator Mapper => _mapperSegregator;

        public async static Task<TDestination> MapAsync<TOrigin, TDestination>(TOrigin origin, params object[] objects) where TDestination : class
        {
            return await _mapperSegregator.MapAsync<TOrigin, TDestination>(origin, objects);
        }
    }
}
