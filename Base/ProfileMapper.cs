using MapperSegregator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MapperSegregator.Base
{
    public sealed class ProfileMapper : IProfileMapper
    {
        private readonly IList<object> _funcs = new List<object>();

        private readonly Assembly _assembly;
        public ProfileMapper(Assembly assembly)
        {
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }
        private async Task InvokeBuilders(Assembly assembly)
        {
            if (_funcs.Any()) return;

            IList<IProfile> types = assembly.GetTypes().Where(x => typeof(IProfile).IsAssignableFrom(x)).Select(x => (IProfile)Activator.CreateInstance(x)).ToList();

            foreach (var item in types)
            {
                await item.MapData(this);
            }
        }

        public async Task LoadBuilders()
        {
            await InvokeBuilders(_assembly);
        }

        public object GetFunc(Type type1, Type type2, out bool isTask)
        {
            var result = _funcs.Where(x => x.GetType().GetGenericArguments()[0] == type1 && x.GetType().GetGenericArguments()[2] == type2).FirstOrDefault();

            if (result != null)
            {
                isTask = false;
                return result;
            }
            else
            {
                isTask = true;
                return _funcs.Where(x => x.GetType().GetGenericArguments()[0] == type1 && x.GetType().GetGenericArguments()[2].GetGenericArguments()[0] == type2).FirstOrDefault();
            }
        }

        public Task Builder<TOrigin, TDestination>(Func<TOrigin, object[], TDestination> func)
        {
            _funcs.Add(func);

            return Task.CompletedTask;
        }

        public TClass GetFromParams<TClass>(object[] objList)
        {
            var typeClass = typeof(TClass);

            if (typeClass.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>)))

                return (TClass)objList.Where(x => x.GetType().GetGenericArguments()[0] == typeClass.GetGenericArguments()[0]).FirstOrDefault();
            else
                return (TClass)objList.Where(x => x.GetType() == typeClass).FirstOrDefault();
        }

        public Task<TClass> GetFromParamsAsync<TClass>(object[] objList)
        {
            return Task.FromResult(GetFromParams<TClass>(objList));
        }

        public Task Builder<TOrigin, TDestination>(Func<TOrigin, object[], Task<TDestination>> func)
        {
            _funcs.Add(func);

            return Task.CompletedTask;
        }
    }
}
