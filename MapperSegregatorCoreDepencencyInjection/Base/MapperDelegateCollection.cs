using MapperSegregator.Base;
using MapperSegregator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapperSegregator.Extensions.DependencyInjection.Base
{
    public class MapperDelegateCollection
    {
        private IList<Delegate> Delegates { get; set; } = new List<Delegate>();
        public Delegate LastUsedDelegate { get; set; }
        public MapperDelegateCollection(IList<Delegate> delegates)
        {
            Delegates = delegates;
        }

        public (MapperDelegate<TOrigin, MapperOptionHandler, TDestination>, MapperDelegate<TOrigin, MapperOptionHandler, Task<TDestination>>) GetDelegate<TOrigin, TDestination>() where TDestination : class
        {
            var parameterType = typeof(TOrigin);

            if (LastUsedDelegate != null)
            {
                bool isCurrent = (LastUsedDelegate.Method.ReturnType == typeof(TDestination) && LastUsedDelegate.Method.GetParameters()[0].ParameterType == parameterType);

                if (isCurrent) return (LastUsedDelegate as MapperDelegate<TOrigin, MapperOptionHandler, TDestination>, default);

                bool isCurrentAsTask = (LastUsedDelegate.Method.ReturnType == typeof(Task<TDestination>) && LastUsedDelegate.Method.GetParameters()[0].ParameterType == parameterType);

                if (isCurrentAsTask) return (default, LastUsedDelegate as MapperDelegate<TOrigin, MapperOptionHandler, Task<TDestination>>);
            }

            var result = Delegates.Where(x => (x.Method.ReturnType == typeof(TDestination) && x.Method.GetParameters()[0].ParameterType == parameterType)).FirstOrDefault();

            if (result != null) return (result as MapperDelegate<TOrigin, MapperOptionHandler, TDestination>, default);

            return (default, Delegates.Where(x => (x.Method.ReturnType == typeof(Task<TDestination>) && x.Method.GetParameters()[0].ParameterType == parameterType)).FirstOrDefault() as MapperDelegate<TOrigin, MapperOptionHandler, Task<TDestination>>);
        }
    }
}
