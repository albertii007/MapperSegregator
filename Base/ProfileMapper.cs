using MapperSegregator.Interfaces;
using Microsoft.EntityFrameworkCore;
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

            LoadBuildersAsync();
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

        public async void LoadBuildersAsync()
        {
            await InvokeBuildersAsync(_assembly);
        }

        public (object, bool) GetFunc(Type type1, Type type2)
        {
            var result = _funcs.Where(x => x.GetType().GetGenericArguments()[0] == type1 && x.GetType().GetGenericArguments()[2] == type2).FirstOrDefault();

            if (result != null) return (result, false);

            return (_funcs.Where(x => x.GetType().GetGenericArguments()[0] == type1 && x.GetType().GetGenericArguments()[2].GetGenericArguments()[0] == type2).FirstOrDefault(), true);
        }

        public Task BuildAsync<TOrigin, TDestination>(MapperEnum[] enums,Func<TOrigin, MapperOptionHandler, TDestination> func)
        {
            if (enums.Contains(MapperEnum.Single))
            {
                _funcs.Add(func);
            }
            if (enums.Contains(MapperEnum.List))
            {
                _funcs.Add(new Func<IList<TOrigin>, MapperOptionHandler, IList<TDestination>>((origins, options) =>
                {
                    options.MapperEnum = MapperEnum.List;

                    return origins.Select(x => func(x, options)).ToList();
                }));
            }
            if (enums.Contains(MapperEnum.Queryable))
            {
                _funcs.Add(new Func<IQueryable<TOrigin>, MapperOptionHandler, Task<IList<TDestination>>>(async (origins, options) =>
                {
                    options.MapperEnum = MapperEnum.Queryable;

                    return await origins.Select(x => func(x, options)).ToListAsync();
                }));
            }
            if (enums.Contains(MapperEnum.Array))
            {
                _funcs.Add(new Func<TOrigin[], MapperOptionHandler, TDestination[]>((origins, options) =>
                {
                    options.MapperEnum = MapperEnum.Array;

                    return origins.Select(x => func(x, options)).ToArray();
                }));
            }

            return Task.CompletedTask;
        }

        public Task BuildAsync<TOrigin, TDestination>(MapperEnum[] enums, Func<TOrigin, TDestination> func)
        {
            if (enums.Contains(MapperEnum.Single))
            {
                _funcs.Add(func);
            }
            if (enums.Contains(MapperEnum.List))
            {
                _funcs.Add(new Func<IList<TOrigin>, MapperOptionHandler, IList<TDestination>>((origins, options) =>
                {
                    options.MapperEnum = MapperEnum.List;

                    return origins.Select(x => func(x)).ToList();
                }));
            }
            if (enums.Contains(MapperEnum.Queryable))
            {
                _funcs.Add(new Func<IQueryable<TOrigin>, MapperOptionHandler, Task<IList<TDestination>>>(async (origins, options) =>
                {
                    options.MapperEnum = MapperEnum.Queryable;

                    return await origins.Select(x => func(x)).ToListAsync();
                }));
            }
            if (enums.Contains(MapperEnum.Array))
            {
                _funcs.Add(new Func<TOrigin[], MapperOptionHandler, TDestination[]>((origins, options) =>
                {
                    options.MapperEnum = MapperEnum.Array;

                    return origins.Select(x => func(x)).ToArray();
                }));
            }

            return Task.CompletedTask;
        }

        public Task BuildAsync<TOrigin, TDestination>(MapperEnum enumVal, Func<TOrigin, TDestination> func)
        {
            if (enumVal == MapperEnum.Single)
            {
                _funcs.Add(func);
            }
            else if (enumVal == MapperEnum.List)
            {
                _funcs.Add(new Func<IList<TOrigin>, MapperOptionHandler, IList<TDestination>>((origins, options) =>
                {
                    options.MapperEnum = MapperEnum.List;

                    return origins.Select(x => func(x)).ToList();
                }));
            }
            else if (enumVal == MapperEnum.Queryable)
            {
                _funcs.Add(new Func<IQueryable<TOrigin>, MapperOptionHandler, Task<IList<TDestination>>>(async (origins, options) =>
                {
                    options.MapperEnum = MapperEnum.Queryable;

                    return await origins.Select(x => func(x)).ToListAsync();
                }));
            }
            else if (enumVal == MapperEnum.Array)
            {
                _funcs.Add(new Func<TOrigin[], MapperOptionHandler, TDestination[]>((origins, options) =>
                {
                    options.MapperEnum = MapperEnum.Array;

                    return origins.Select(x => func(x)).ToArray();
                }));
            }

            return Task.CompletedTask;
        }

        public Task BuildAsync<TOrigin, TDestination>(Func<TOrigin, MapperOptionHandler, TDestination> func)
        {
            _funcs.Add(func);

            return Task.CompletedTask;
        }

        public Task BuildAsync<TOrigin, TDestination>(Func<TOrigin, TDestination> func)
        {
            _funcs.Add(new Func<TOrigin, MapperOptionHandler, TDestination>((origin, options) =>
            {
                return func(origin);
            }));

            return Task.CompletedTask;
        }

        public Task BuildAsync<TOrigin, TDestination>(Func<TOrigin, MapperOptionHandler, Task<TDestination>> func)
        {
            _funcs.Add(func);

            return Task.CompletedTask;
        }

        public Task BuildAsync<TOrigin, TDestination>(Func<TOrigin, Task<TDestination>> func)
        {
            _funcs.Add(new Func<TOrigin, MapperOptionHandler, Task<TDestination>>(async (origin, options) =>
            {
                return await func(origin);
            }));

            return Task.CompletedTask;
        }
    }
}
