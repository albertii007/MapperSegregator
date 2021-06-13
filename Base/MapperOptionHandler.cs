using MapperSegregator.Helpers;
using MapperSegregator.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapperSegregator.Base
{
    public sealed class MapperOptionHandler
    {
        private readonly object[] _objs;
        public MapperEnum MapperEnum { get; set; } = MapperEnum.Single;
        public MapperOptionHandler(object[] objs)
        {
            _objs = objs;
        }

        public Task<TClass> GetFromParamsAsync<TClass>()
        {
            return Task.FromResult(GetFromParams<TClass>());
        }

        public bool IsEnum(MapperEnum mapperEnum)
        {
            return MapperEnum == mapperEnum;
        }

        public bool ContainsEnum(params MapperEnum[] mapperEnums)
        {
            return mapperEnums.Contains(MapperEnum);
        }

        public IMapperSegregator Mapper => MapperSegregatorHelper.Mapper;

        public TClass GetFromParams<TClass>()
        {
            var typeClass = typeof(TClass);

            if (typeClass.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>)))

                return (TClass)_objs.Where(x => x.GetType().GetGenericArguments()[0] == typeClass.GetGenericArguments()[0]).FirstOrDefault();
            else
                return (TClass)_objs.Where(x => x.GetType() == typeClass).FirstOrDefault();
        }
    }
}
