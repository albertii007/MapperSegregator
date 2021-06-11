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

        private readonly IList<Assembly> _assembly;

        public ProfileMapper(IList<Assembly> assembly)
        {
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }
        private async Task InvokeBuildersAsync(IList<Assembly> assemblies)
        {
            if (_funcs.Any()) return;

            IList<IProfile> types = assemblies.SelectMany(x => x.GetTypes().Where(x => typeof(IProfile).IsAssignableFrom(x))).Select(x => (IProfile)Activator.CreateInstance(x)).ToList();

            foreach (var item in types)
            {
                await item.MapData(this);
            }
        }

        public async Task LoadBuildersAsync()
        {
            await InvokeBuildersAsync(_assembly);
        }

        public (object, bool) GetFunc(Type type1, Type type2)
        {
            var result = _funcs.Where(x => x.GetType().GetGenericArguments()[0] == type1 && x.GetType().GetGenericArguments()[2] == type2).FirstOrDefault();

            if (result != null) return (result, false);

            return (_funcs.Where(x => x.GetType().GetGenericArguments()[0] == type1 && x.GetType().GetGenericArguments()[2].GetGenericArguments()[0] == type2).FirstOrDefault(), true);
        }

        public Task Builder<TOrigin, TDestination>(Func<TOrigin, MapperOptionHandler, TDestination> func)
        {
            _funcs.Add(func);

            return Task.CompletedTask;
        }

        public Task Builder<TOrigin, TDestination>(Func<TOrigin, MapperOptionHandler, Task<TDestination>> func)
        {
            _funcs.Add(func);

            return Task.CompletedTask;
        }
    }
}
