using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace MapperSegregator.Helpers
{
    public static class MapperSegregatorHelper
    {
        private static ISegregatorMapper _smartMapper;

        public static void Configure(ISegregatorMapper smartMapper)
        {
            _smartMapper = smartMapper ?? throw new ArgumentNullException(nameof(smartMapper));
        }

        public async static Task<TDestination> Map<TOrigin, TDestination>(TOrigin origin, params object[] objects) where TDestination : class
        {
            return await _smartMapper.Map<TOrigin, TDestination>(origin, objects);
        }
    }
}
