using System.Threading.Tasks;

namespace MapperSegregator.Interfaces
{
    public interface IProfile
    {
        public Task MapData(IProfileMapper profileMapper);
    }
}
