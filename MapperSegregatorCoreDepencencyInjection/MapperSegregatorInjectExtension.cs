using MapperSegregator.Extensions.DependencyInjection.Base;
using MapperSegregator.Helpers;
using MapperSegregator.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MapperSegregator.Extensions.DependencyInjection
{
    public static class MapperSegregatorInjectExtension
    {
        public static void RegisterMapperServices(this IServiceCollection services, params Assembly[] assemblies)
        {

            using (var delegateCreator = new MapperDelegateCreator(assemblies))
            {
                IList<Delegate> delegates = delegateCreator.InvokeBuildersAsync().GetAwaiter().GetResult();

                services.AddSingleton(x => new MapperDelegateCollection(delegates));
            }

            services.AddTransient<IMapperSegregator, MapperSegregatorHandler>();
        }

        public static void UseMapperServices(this IApplicationBuilder app)
        {
            MapperSegregatorHelper.Configure(app.ApplicationServices.GetRequiredService<IMapperSegregator>());
        }
    }
}
