using MapperSegregator.Base;
using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace MapperSegregator
{
    public class MapperSegregatorHandler : ISegregatorMapper
    {
        private readonly ProfileMapper _profileMapper;
        public MapperSegregatorHandler(ProfileMapper profile)
        {
            _profileMapper = profile ?? throw new ArgumentNullException(nameof(profile));
        }

        public async Task<TDestination> Map<TOrigin, TDestination>(TOrigin origin, params object[] objects) where TDestination : class
        {
            await _profileMapper.LoadBuilders();

            var func = _profileMapper.GetFunc(typeof(TOrigin), typeof(TDestination), out bool isTask);

            if (func == null) throw new Exception($"{typeof(TOrigin).FullName} && {typeof(TDestination).FullName} are not implemented");

            if (isTask)
            {
                var result = func.GetType().GetMethod("Invoke").Invoke(func, new object[] { origin, objects }) as Task<TDestination>;

                return await result;
            }

            return func.GetType().GetMethod("Invoke").Invoke(func, new object[] { origin, objects }) as TDestination;
        }
    }
}
