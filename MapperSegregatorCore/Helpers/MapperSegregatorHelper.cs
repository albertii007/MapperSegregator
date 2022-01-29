using MapperSegregator.Interfaces;
using System;

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
    }
}
