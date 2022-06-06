### What is MapperSegregatorCore?

It's more an architectural solution library.

### How do I setup?

First, Install MapperSegregatorCoreDependencyInjection and apply configuration on startup.

```
PM> Install-Package MapperSegregatorCoreDependencyInjection
```

And then you let Mapper know in what assemblies are those profiles defined by calling the IServiceCollection extension method `RegisterMapperServices` at startup 
and a helper extension `UseMapperServices` that is used to find mapping from linq reference:
```csharp
// services is IServiceCollection
services.RegisterMapperServices(assembly1, assembly2...);

// app is WebApplicationBuilder
app.UseMapperServices();
```

### How do I create mapping?

First, install the `MapperSegregatorCore` to the class library that you have bussiness logic.

```
PM> Install-Package MapperSegregatorCore

```
Add Interface `IProfile` to DTO that you want to create mapping, and Interface will implement a method called <br /> `MapData(IProfileMapper profileMapper)`.

MapperEnum simplifies the code as shown on example below, the original way without using MapperEnum 
<br />
`profileMapper.BuildAsync<IList<User>, IList<GetUsersModel>>((user) => new GetUsersModel ...`

And what if we want to have two mappings one for
<br />
`<IList<User>, IList<GetUsersModel>>`
<br />
and other one 
<br />
`<User, GetUsersModel>`

<br /> 
Without MapperEnum we should do two mappings but with MapperEnum we can simply do 

`profileMapper.BuildAsync<User, GetUsersModel>(new MapperEnum[] { MapperEnum.List, MapperEnum.Single }, (user) => `

```csharp
    public class GetUsersModel : IProfile
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            await profileMapper.BuildAsync<User, GetUsersModel>(MapperEnum.List, (user) => new GetUsersModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
            });
        }
    }
```

What If you have nested mapping?

There is a parameter `options` that contains Mapper and some other methodes ex:

```csharp
    public class GetUsersModel : IProfile
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public RoleModel Role { get; set; } // other DTO
        
        public async Task MapData(IProfileMapper profileMapper)
        {
            await profileMapper.BuildAsync<User, GetUsersModel>(MapperEnum.List, (user, options) => new GetUsersModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = options.Mapper.Map<Role, RoleModel>(user.UserRoles.Select(x => x.Role).FirstOrDefault())
            });
        }
    }
    
    public class RoleModel : IProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            await profileMapper.BuildAsync<Role, RoleModel>(MapperEnum.Single, (role) => new RoleModel
            {
                Id = role.Id,
                Name = role.Name
            });
        }
    }
```


Call the mapping Ex:
```csharp
        private readonly IMapperSegregator _mapper;
        
        //constructor
        public UserService(IDbContext context, IMapperSegregator mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        // finds refrence when MapperEnum.List is included to mapping
        public async Task<IList<GetUsersModel>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
          
            return await _mapper.MapAsync<IList<User>, IList<GetUsersModel>>(users);
        }
        
         // finds refrence when MapperEnum.Single is included to mapping
        public async Task<GetUsersModel> GetSingleUser()
        {
            var user = await _context.Users.FirstOrDefaultAsync();
          
            return await _mapper.MapAsync<User, GetUsersModel>(users);
        }
        
        // Using Mapper Extensiton Helper that works only with MapperEnum.Queryable
        // finds refrence when MapperEnum.Queryable is included to mapping
        public async Task<IList<GetUsersModel>> GetUsers()
        {
            return await _context.Users.MapToListAsync<User, GetUsersModel>();
        }
```
