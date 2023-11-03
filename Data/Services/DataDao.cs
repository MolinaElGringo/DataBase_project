
namespace BlazinRoleGame.Data
{

    public class UserService : ServiceBase<User>
    {
        public UserService(IConfiguration configuration) : base(configuration)
        {}
    }
    public class PersonneService : ServiceBase<Personne>
    {
        public PersonneService(IConfiguration configuration) : base(configuration)
        {}
    }
}