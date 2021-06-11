using MapperSegregator.Base;
using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace MapperSegregator
{
    public class MapperSegregatorHandler : IMapperSegregator
    {
        private readonly ProfileMapper _profileMapper;

        public MapperSegregatorHandler(ProfileMapper profile)
        {
            _profileMapper = profile ?? throw new ArgumentNullException(nameof(profile));
        }

        public TDestination Map<TOrigin, TDestination>(TOrigin origin, params object[] objects) where TDestination : class
        {
            return MapAsync<TOrigin, TDestination>(origin, objects).GetAwaiter().GetResult();
        }

        public async Task<TDestination> MapAsync<TOrigin, TDestination>(TOrigin origin, params object[] objects) where TDestination : class
        {
            await _profileMapper.LoadBuildersAsync();

            var (func, isTask) = _profileMapper.GetFunc(typeof(TOrigin), typeof(TDestination));

            if (func == null) throw new Exception($"{typeof(TOrigin).FullName} && {typeof(TDestination).FullName} are not implemented");

            if (isTask) return await (func.GetType().GetMethod("Invoke").Invoke(func, new object[] { origin, new MapperOptionHandler(objects) }) as Task<TDestination>);

            return func.GetType().GetMethod("Invoke").Invoke(func, new object[] { origin, new MapperOptionHandler(objects) }) as TDestination;
        }
    }
}
