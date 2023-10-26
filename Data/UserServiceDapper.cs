using Npgsql;
using Dapper;

namespace BlazinRoleGame.Data;

public class UserServiceDapper : IUserService
{

    IConfiguration configuration;

    public UserServiceDapper(IConfiguration config)
    {
        configuration = config;
    }

    #region Methode Dapper
    /*Dapper*/
    /*get the liste of users in the database*/
    public Task<User[]> GetUsersAsync()
    {
        var conn = new NpgsqlConnection(connectionString: configuration["DefaultConectionString"]);
        conn.Open();

        var result = conn.Query<User>("select * from public.\"User\"");

        conn.Close();
        return Task.FromResult(result.ToArray());
    }

    /*Dapper*/
    /*add a user to the database*/
    public Task<User> AddUserAsync(User user)
    {

        if (user == null)
            return Task.FromCanceled<User>(new CancellationToken());

        var conn = new NpgsqlConnection(connectionString: configuration["DefaultConectionString"]);
        conn.Open();

        const string InsertSQL = "insert into public.\"User\" (\"FirstName\", \"LastName\") values (@FirstName, @LastName)";

        var add = conn.Query<User>(InsertSQL, new { FirstName = user.FirstName, LastName = user.LastName });

        conn.Close();
        return Task.FromResult(user);
    }

    /*Dapper*/
    /*delete a user from the database*/
    public Task<bool> DeleteUserAsync(User user)
    {
        if (user == null)
            return Task<bool>.FromResult(false);

        var conn = new NpgsqlConnection(connectionString: configuration["DefaultConectionString"]);
        conn.Open();

        string DeleteSQL = "Delete from public.\"User\" where \"UserID\" =" + user.UserID;

        var delete = conn.Query<User>(DeleteSQL);

        conn.Close();

        return Task<bool>.FromResult(true);
    }

    /*Dapper*/
    /*update a user in the database*/
    public Task<User> UpdateUserAsync(User user)
    {
        if (user == null)
            return Task.FromCanceled<User>(new CancellationToken());

        var conn = new NpgsqlConnection(connectionString: configuration["DefaultConectionString"]);
        conn.Open();

        string UpdateSQL = string.Format("Update public.\"User\" set \"FirstName\" = '{0}', \"LastName\"='{1}' where \"UserID\" = {2}", user.FirstName, user.LastName, user.UserID);

        var update = conn.Query<User>(UpdateSQL);

        conn.Close();

        return Task.FromResult(user);
    }

    #endregion


}