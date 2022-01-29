using MapperSegregator.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapperSegregator.Extensions
{
    public static class MapperContextExtension
    {
        public async static Task<IList<TDestinition>> MapToListAsync<TOrigin, TDestinition>(this IQueryable<TOrigin> source, params object[] objects) where TDestinition : new() where TOrigin : new()
        {
            return await MapperSegregatorHelper.Mapper.MapAsync<IQueryable<TOrigin>, IList<TDestinition>>(source, objects);
        }
    }
}
