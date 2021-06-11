using MapperSegregator.Base;
using MapperSegregator.Helpers;
using MapperSegregator.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MapperSegregator.Extensions
{
    public static class MapperInjectExtension
    {
        public static void RegisterMapperServices(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddSingleton(x => new ProfileMapper(assemblies));

            services.AddTransient<IMapperSegregator, MapperSegregatorHandler>();
        }

        public static void UseMapperServices(this IApplicationBuilder app)
        {
            MapperSegregatorHelper.Configure(app.ApplicationServices.GetRequiredService<IMapperSegregator>());
        }
    }
}
