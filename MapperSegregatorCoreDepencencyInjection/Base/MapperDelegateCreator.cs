using MapperSegregator.Base;
using MapperSegregator.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MapperSegregator.Extensions.DependencyInjection.Base
{
    public class MapperDelegateCreator : IProfileMapper , IDisposable
    {
        private IList<Delegate> Delegates { get; set; } = new List<Delegate>();

        private readonly Assembly[] _assemblies;

        public MapperDelegateCreator(Assembly[] assemblies)
        {
            _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        }

        public async Task<IList<Delegate>> InvokeBuildersAsync()
        {
            IList<IProfile> types = _assemblies.SelectMany(x => x.GetTypes())
                                               .Where(x => (!x.IsInterface && x.GetInterfaces().Any(x => x == typeof(IProfile))))
                                               .Distinct()
                                               .Select(x => (IProfile)Activator.CreateInstance(x)).ToList();

            foreach (var item in types)
            {
                await item.MapData(this);
            }

            return Delegates;
        }

        public Task BuildAsync<TOrigin, TDestination>(MapperEnum[] enums, MapperDelegate<TOrigin, MapperOptionHandler, TDestination> func)
        {
            if (enums.Contains(MapperEnum.Single))
            {
                Delegates.Add(new MapperDelegate<TOrigin, MapperOptionHandler, TDestination>((origin, options) =>
                {
                    options.MapperEnum = MapperEnum.Single;

                    if (origin == null) return default;

                    return func(origin, options);
                }));
            }
            if (enums.Contains(MapperEnum.List))
            {
                Delegates.Add(new MapperDelegate<IList<TOrigin>, MapperOptionHandler, IList<TDestination>>((origins, options) =>
                {
                    options.MapperEnum = MapperEnum.List;

                    return origins.Select(x => func(x, options)).ToList();
                }));
            }
            if (enums.Contains(MapperEnum.Queryable))
            {
                Delegates.Add(new MapperDelegate<IQueryable<TOrigin>, MapperOptionHandler, Task<IList<TDestination>>>(async (origins, options) =>
                {
                    options.MapperEnum = MapperEnum.Queryable;

                    return await origins.Select(x => func(x, options)).ToListAsync();
                }));
            }
            if (enums.Contains(MapperEnum.Array))
            {
                Delegates.Add(new MapperDelegate<TOrigin[], MapperOptionHandler, TDestination[]>((origins, options) =>
                {
                    options.MapperEnum = MapperEnum.Array;

                    return origins.Select(x => func(x, options)).ToArray();
                }));
            }

            return Task.CompletedTask;
        }

        public Task BuildAsync<TOrigin, TDestination>(MapperEnum[] enums, MapperDelegate<TOrigin, TDestination> func)
        {
            if (enums.Contains(MapperEnum.Single))
            {
                Delegates.Add(new MapperDelegate<TOrigin, MapperOptionHandler, TDestination>((origin, options) =>
                {
                    options.MapperEnum = MapperEnum.Single;

                    if (origin == null) return default;

                    return func(origin);
                }));
            }
            if (enums.Contains(MapperEnum.List))
            {
                Delegates.Add(new MapperDelegate<IList<TOrigin>, MapperOptionHandler, IList<TDestination>>((origins, options) =>
                {
                    options.MapperEnum = MapperEnum.List;

                    return origins.Select(x => func(x)).ToList();
                }));
            }
            if (enums.Contains(MapperEnum.Queryable))
            {
                Delegates.Add(new MapperDelegate<IQueryable<TOrigin>, MapperOptionHandler, Task<IList<TDestination>>>(async (origins, options) =>
                {
                    options.MapperEnum = MapperEnum.Queryable;

                    return await origins.Select(x => func(x)).ToListAsync();
                }));
            }
            if (enums.Contains(MapperEnum.Array))
            {
                Delegates.Add(new MapperDelegate<TOrigin[], MapperOptionHandler, TDestination[]>((origins, options) =>
                {
                    options.MapperEnum = MapperEnum.Array;

                    return origins.Select(x => func(x)).ToArray();
                }));
            }

            return Task.CompletedTask;
        }

        public Task BuildAsync<TOrigin, TDestination>(MapperEnum enumVal, MapperDelegate<TOrigin, TDestination> func)
        {
            if (enumVal == MapperEnum.Single)
            {
                Delegates.Add(new MapperDelegate<TOrigin, MapperOptionHandler, TDestination>((origin, options) =>
                {
                    options.MapperEnum = MapperEnum.Single;

                    if (origin == null) return default;

                    return func(origin);
                }));
            }
            else if (enumVal == MapperEnum.List)
            {
                Delegates.Add(new MapperDelegate<IList<TOrigin>, MapperOptionHandler, IList<TDestination>>((origins, options) =>
                {
                    options.MapperEnum = MapperEnum.List;

                    return origins.Select(x => func(x)).ToList();
                }));
            }
            else if (enumVal == MapperEnum.Queryable)
            {
                Delegates.Add(new MapperDelegate<IQueryable<TOrigin>, MapperOptionHandler, Task<IList<TDestination>>>(async (origins, options) =>
                {
                    options.MapperEnum = MapperEnum.Queryable;

                    return await origins.Select(x => func(x)).ToListAsync();
                }));
            }
            else if (enumVal == MapperEnum.Array)
            {
                Delegates.Add(new MapperDelegate<TOrigin[], MapperOptionHandler, TDestination[]>((origins, options) =>
                {
                    options.MapperEnum = MapperEnum.Array;

                    return origins.Select(x => func(x)).ToArray();
                }));
            }

            return Task.CompletedTask;
        }

        public Task BuildAsync<TOrigin, TDestination>(MapperEnum enumVal, MapperDelegate<TOrigin, MapperOptionHandler, TDestination> func)
        {
            if (enumVal == MapperEnum.Single)
            {
                Delegates.Add(new MapperDelegate<TOrigin, MapperOptionHandler, TDestination>((origin, options) =>
                {
                    options.MapperEnum = MapperEnum.Single;

                    if (origin == null) return default;

                    return func(origin, options);
                }));
            }
            else if (enumVal == MapperEnum.List)
            {
                Delegates.Add(new MapperDelegate<IList<TOrigin>, MapperOptionHandler, IList<TDestination>>((origins, options) =>
                {
                    options.MapperEnum = MapperEnum.List;

                    return origins.Select(x => func(x, options)).ToList();
                }));
            }
            else if (enumVal == MapperEnum.Queryable)
            {
                Delegates.Add(new MapperDelegate<IQueryable<TOrigin>, MapperOptionHandler, Task<IList<TDestination>>>(async (origins, options) =>
                {
                    options.MapperEnum = MapperEnum.Queryable;

                    return await origins.Select(x => func(x, options)).ToListAsync();
                }));
            }
            else if (enumVal == MapperEnum.Array)
            {
                Delegates.Add(new MapperDelegate<TOrigin[], MapperOptionHandler, TDestination[]>((origins, options) =>
                {
                    options.MapperEnum = MapperEnum.Array;

                    return origins.Select(x => func(x, options)).ToArray();
                }));
            }

            return Task.CompletedTask;
        }

        public Task BuildAsync<TOrigin, TDestination>(MapperDelegate<TOrigin, MapperOptionHandler, TDestination> func)
        {
            Delegates.Add(func);

            return Task.CompletedTask;
        }

        public Task BuildAsync<TOrigin, TDestination>(MapperDelegate<TOrigin, TDestination> func)
        {
            Delegates.Add(new MapperDelegate<TOrigin, MapperOptionHandler, TDestination>((origin, options) =>
            {
                return func(origin);
            }));

            return Task.CompletedTask;
        }

        public Task BuildAsync<TOrigin, TDestination>(MapperDelegate<TOrigin, MapperOptionHandler, Task<TDestination>> func)
        {
            Delegates.Add(func);

            return Task.CompletedTask;
        }

        public Task BuildAsync<TOrigin, TDestination>(MapperDelegate<TOrigin, Task<TDestination>> func)
        {
            Delegates.Add(new MapperDelegate<TOrigin, MapperOptionHandler, Task<TDestination>>(async (origin, options) =>
            {
                return await func(origin);
            }));

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
