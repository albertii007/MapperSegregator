using MapperSegregator.Base;
using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace MapperSegregator.Extensions.DependencyInjection.Base
{
    public class MapperSegregatorHandler : IMapperSegregator
    {
        private readonly MapperDelegateCollection _mapperCollection;

        public MapperSegregatorHandler(MapperDelegateCollection mapperCollection)
        {
            _mapperCollection = mapperCollection ?? throw new ArgumentNullException(nameof(mapperCollection));
        }

        public TDestination Map<TOrigin, TDestination>(TOrigin origin, params object[] objects) where TDestination : class
        {
            return MapAsync<TOrigin, TDestination>(origin, objects).GetAwaiter().GetResult();
        }

        public async Task<TDestination> MapAsync<TOrigin, TDestination>(TOrigin origin, params object[] objects) where TDestination : class
        {
            var (func, taskFunc) = _mapperCollection.GetDelegate<TOrigin, TDestination>();

            if (func == null && taskFunc == null) throw new Exception($"{typeof(TOrigin).FullName} && {typeof(TDestination).FullName} are not implemented");

            if (taskFunc != null)
            {
                _mapperCollection.LastUsedDelegate = taskFunc;

                return await taskFunc(origin, new MapperOptionHandler(objects));
            }

            _mapperCollection.LastUsedDelegate = func;

            return func(origin, new MapperOptionHandler(objects));
        }
    }
}
