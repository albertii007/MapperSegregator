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
    public class MapperDelegateCreator : IProfileMapper, IDisposable
    {
        private IList<Delegate> Delegates { get; set; } = new List<Delegate>();

        private readonly Assembly[] _assemblies;

        private Func<Task> SealedFunc;

        private MapperEnum[] AcceptEnums;
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
                AcceptEnums = Array.Empty<MapperEnum>();

                await item.MapData(this);
            }

            return Delegates;
        }

        public async Task ToTypeAsync(params MapperEnum[] enums)
        {
            AcceptEnums = enums;
            
            if(SealedFunc != null) await SealedFunc();
        }

        public IProfileMapper Build<TOrigin, TDestination>(MapperDelegate<TOrigin, MapperOptionHandler, TDestination> func)
        {
            SealedFunc = new Func<Task>(() =>
            {
                if (!AcceptEnums.Any()) Delegates.Add(func);
                else
                {
                    if (AcceptEnums.Contains(MapperEnum.Single))
                    {
                        Delegates.Add(new MapperDelegate<TOrigin, MapperOptionHandler, TDestination>((origin, options) =>
                        {
                            options.MapperEnum = MapperEnum.Single;

                            if (origin == null) return default;

                            return func(origin, options);
                        }));
                    }
                    if (AcceptEnums.Contains(MapperEnum.List))
                    {
                        Delegates.Add(new MapperDelegate<IList<TOrigin>, MapperOptionHandler, IList<TDestination>>((origins, options) =>
                        {
                            options.MapperEnum = MapperEnum.List;

                            return origins.Select(x => func(x, options)).ToList();
                        }));
                    }
                    if (AcceptEnums.Contains(MapperEnum.Queryable))
                    {
                        Delegates.Add(new MapperDelegate<IQueryable<TOrigin>, MapperOptionHandler, Task<IList<TDestination>>>(async (origins, options) =>
                        {
                            options.MapperEnum = MapperEnum.Queryable;

                            return await origins.Select(x => func(x, options)).ToListAsync();
                        }));
                    }
                    if (AcceptEnums.Contains(MapperEnum.Array))
                    {
                        Delegates.Add(new MapperDelegate<TOrigin[], MapperOptionHandler, TDestination[]>((origins, options) =>
                        {
                            options.MapperEnum = MapperEnum.Array;

                            return origins.Select(x => func(x, options)).ToArray();
                        }));
                    }
                }
                return Task.CompletedTask;
            });
            return this;
        }

        public IProfileMapper Build<TOrigin, TDestination>(MapperDelegate<TOrigin, TDestination> func)
        {
            SealedFunc = new Func<Task>(() =>
            {
                if (!AcceptEnums.Any()) Delegates.Add(new MapperDelegate<TOrigin, MapperOptionHandler, TDestination>((origin, options) => func(origin)));
                else
                {
                    if (AcceptEnums.Contains(MapperEnum.Single))
                    {
                        Delegates.Add(new MapperDelegate<TOrigin, MapperOptionHandler, TDestination>((origin, options) =>
                        {
                            options.MapperEnum = MapperEnum.Single;

                            if (origin == null) return default;

                            return func(origin);
                        }));
                    }
                    if (AcceptEnums.Contains(MapperEnum.List))
                    {
                        Delegates.Add(new MapperDelegate<IList<TOrigin>, MapperOptionHandler, IList<TDestination>>((origins, options) =>
                        {
                            options.MapperEnum = MapperEnum.List;

                            return origins.Select(x => func(x)).ToList();
                        }));
                    }
                    if (AcceptEnums.Contains(MapperEnum.Queryable))
                    {
                        Delegates.Add(new MapperDelegate<IQueryable<TOrigin>, MapperOptionHandler, Task<IList<TDestination>>>(async (origins, options) =>
                        {
                            options.MapperEnum = MapperEnum.Queryable;

                            return await origins.Select(x => func(x)).ToListAsync();
                        }));
                    }
                    if (AcceptEnums.Contains(MapperEnum.Array))
                    {
                        Delegates.Add(new MapperDelegate<TOrigin[], MapperOptionHandler, TDestination[]>((origins, options) =>
                        {
                            options.MapperEnum = MapperEnum.Array;

                            return origins.Select(x => func(x)).ToArray();
                        }));
                    }
                }
                return Task.CompletedTask;
            });

            return this;
        }

        public IProfileMapper Build<TOrigin, TDestination>(MapperDelegate<TOrigin, MapperOptionHandler, Task<TDestination>> func)
        {
            SealedFunc = new Func<Task>(() =>
            {
                if (!AcceptEnums.Any()) Delegates.Add(func);
                else
                {
                    if (AcceptEnums.Contains(MapperEnum.Single))
                    {
                        Delegates.Add(new MapperDelegate<TOrigin, MapperOptionHandler, Task<TDestination>>(async (origin, options) =>
                        {
                            options.MapperEnum = MapperEnum.Single;

                            if (origin == null) return default;

                            return await func(origin, options);
                        }));
                    }
                    if (AcceptEnums.Contains(MapperEnum.List))
                    {
                        Delegates.Add(new MapperDelegate<IList<TOrigin>, MapperOptionHandler, Task<IList<TDestination>>>(async (origins, options) =>
                        {
                            options.MapperEnum = MapperEnum.List;

                            IList<TDestination> destinations = new List<TDestination>();

                            foreach (var item in origins)
                            {
                                destinations.Add(await func(item, options));
                            }

                            return destinations;
                        }));
                    }
                    if (AcceptEnums.Contains(MapperEnum.Queryable))
                    {
                        Delegates.Add(new MapperDelegate<IQueryable<TOrigin>, MapperOptionHandler, Task<IList<TDestination>>>(async (origins, options) =>
                        {
                            options.MapperEnum = MapperEnum.Queryable;

                            IList<TDestination> destinations = new List<TDestination>();

                            foreach (var item in origins)
                            {
                                destinations.Add(await func(item, options));
                            }

                            return destinations;
                        }));
                    }
                    if (AcceptEnums.Contains(MapperEnum.Array))
                    {
                        Delegates.Add(new MapperDelegate<TOrigin[], MapperOptionHandler, Task<TDestination[]>>(async (origins, options) =>
                        {
                            options.MapperEnum = MapperEnum.Array;

                            IList<TDestination> destinations = new List<TDestination>();

                            foreach (var item in origins)
                            {
                                destinations.Add(await func(item, options));
                            }

                            return destinations.ToArray();
                        }));
                    }
                }
                return Task.CompletedTask;
            });

            return this;
        }

        public IProfileMapper Build<TOrigin, TDestination>(MapperDelegate<TOrigin, Task<TDestination>> func)
        {
            SealedFunc = new Func<Task>(() =>
            {
                if (!AcceptEnums.Any()) Delegates.Add(new MapperDelegate<TOrigin, MapperOptionHandler, Task<TDestination>>(async (origin, options) => await func(origin)));
                else
                {
                    if (AcceptEnums.Contains(MapperEnum.Single))
                    {
                        Delegates.Add(new MapperDelegate<TOrigin, MapperOptionHandler, Task<TDestination>>(async (origin, options) =>
                        {
                            options.MapperEnum = MapperEnum.Single;

                            if (origin == null) return default;

                            return await func(origin);
                        }));
                    }
                    if (AcceptEnums.Contains(MapperEnum.List))
                    {
                        Delegates.Add(new MapperDelegate<IList<TOrigin>, MapperOptionHandler, Task<IList<TDestination>>>(async (origins, options) =>
                        {
                            options.MapperEnum = MapperEnum.List;

                            IList<TDestination> destinations = new List<TDestination>();

                            foreach (var item in origins)
                            {
                                destinations.Add(await func(item));
                            }

                            return destinations;
                        }));
                    }
                    if (AcceptEnums.Contains(MapperEnum.Queryable))
                    {
                        Delegates.Add(new MapperDelegate<IQueryable<TOrigin>, MapperOptionHandler, Task<IList<TDestination>>>(async (origins, options) =>
                        {
                            options.MapperEnum = MapperEnum.Queryable;

                            IList<TDestination> destinations = new List<TDestination>();

                            foreach (var item in origins)
                            {
                                destinations.Add(await func(item));
                            }

                            return destinations;
                        }));
                    }
                    if (AcceptEnums.Contains(MapperEnum.Array))
                    {
                        Delegates.Add(new MapperDelegate<TOrigin[], MapperOptionHandler, Task<TDestination[]>>(async (origins, options) =>
                        {
                            options.MapperEnum = MapperEnum.Array;

                            IList<TDestination> destinations = new List<TDestination>();

                            foreach (var item in origins)
                            {
                                destinations.Add(await func(item));
                            }

                            return destinations.ToArray();
                        }));
                    }
                }
                return Task.CompletedTask;
            });

            return this;
        }

        public void Dispose()
        {
            AcceptEnums = Array.Empty<MapperEnum>();

            GC.SuppressFinalize(this);
        }
    }
}
