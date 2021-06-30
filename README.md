# MapperSegregatorCore

### How to install?

[![NuGet](https://img.shields.io/badge/Nuget-v5.0.11-blue)](https://www.nuget.org/packages/MapperSegregatorCore/5.0.11)

or on Package Manager

Install-Package MapperSegregatorCore -Version 5.0.11


### What is MapperSegregatorCore?

Mapper segregator core is simple tool that is created for segregating mappings from bussiness logic to DTO.

### How to start?

Startup.cs
```csharp
public void ConfigureServices(IServiceCollection services)
{
  /// Assembly should be where DTO-s are, also you can add more than one assembly.
  services.RegisterMapperServices(Assembly);
  ///or 
  services.RegisterMapperServices(Assembly1, Assembly2 etc...);
}
```

On Dto implement interface IProfile that gives you a method MapData;

```csharp
public class CityDto : IProfile 
{
  public Guid Id {get; set;}
  public string Name {get; set;}

  public async Task MapData(IProfileMapper profileMapper)
  {
    /// By default MapperEnum is Single.
     await profileMapper.BuildAsync<City, CityDto>((user) => new CityDto
         {
             Id = art.Id,
             Name = art.Name
         });
     });
  }
}
```

```csharp
public class BookDto : IProfile 
{
  public Guid Id {get; set;}
  public string Name {get; set;}

  public async Task MapData(IProfileMapper profileMapper)
  {
    /// You can map only if you send a list.
     await profileMapper.BuildAsync<Book, BookDto>(MapperEnum.List, (user) => new BookDto
         {
             Id = art.Id,
             Name = art.Name
         });
     });
  }
}
```

```csharp
public class UserDto : IProfile 
{
  public Guid Id {get; set;}
  public string Username {get; set;}
  public string ExtraString {get; set;}
  public int ExtraNumber {get; set;}
  public string PhoneNumber {get; set;}
  public CityDto City {get; set;}
  public IList<BookDto> Books {get; set;} = new List<BookDto>();
  
  public async Task MapData(IProfileMapper profileMapper)
  {
    /// Now you can map Sinle and List.
     await profileMapper.BuildAsync<User, UserDto>(new[] {MapperEnum.Single, MapperEnum.List, MapperEnum.Queryable},(user, options) => new UserDto
         {
             Id = art.Id,
             Name = art.Name,
             City = options.Mapper.Map<City, CityDto>(user.City),
             Books = options.Mapper.Map<Book, BookDto>(user.Books.ToList()),
             ExtraString = options.GetFromParams<string>(), /// result will be "test extra"
             ExtraNumber = options.GetFromParams<int>(), /// result will be "10"
         });
     });
  }
}
```

```csharp
public class UserService 
{
  /// IMapperSegregator _mapper; 
  
  public async Task<UserDto> ReturnUser(User user) // with extra parameters
  {
    /// Maps if MapperEnum.Single
    return await _mapper.MapAsync<User, UserDto>(user, "test extra", 10); /// test extra and 10 are extra optional params you can send anything.
  }
    
  public async Task<IList<UserDto>> ReturnUsers(IList<User> users)
  {
  /// Maps if MapperEnum.List
    return await _mapper.MapAsync<IList<User>, IList<UserDto>>(users);
  }
  
  public async Task<IList<UserDto>> ReturnUsers() /// with DbContext
  {
    /// Maps if MapperEnum.List
    return await _dbContext.Users.MapToListAsync<IList<User>, IList<UserDto>>(users);
    or
    /// Maps if MapperEnum.Queryable
    return await _dbContext.Users.QueryableMap<IList<User>, IList<UserDto>>(users);
  }
}
```




