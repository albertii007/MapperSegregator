using MapperSegregator.Helpers;
using MapperSegregator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapperSegregator.Base
{
    public class MapperOptionHandler
    {
        private readonly object[] _objs;
        public MapperOptionHandler(object[] objs)
        {
            _objs = objs;
        }

        public Task<TClass> GetFromParamsAsync<TClass>()
        {
            return Task.FromResult(GetFromParams<TClass>());
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

        public (T1,T2) GetFromParams<T1, T2>()
        {
            var type1Class = typeof(T1);
            var type2Class = typeof(T2);


            if (type1Class.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>)) || type2Class.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>)))

                return ((T1)_objs.Where(x => x.GetType().GetGenericArguments()[0] == type1Class.GetGenericArguments()[0]).FirstOrDefault(), (T2)_objs.Where(x => x.GetType().GetGenericArguments()[0] == type2Class.GetGenericArguments()[0]).FirstOrDefault());
            else
                return (TClass)_objs.Where(x => x.GetType() == typeClass).FirstOrDefault();
        }
    }
}
