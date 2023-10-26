using Npgsql;
using Dapper;

namespace BlazinRoleGame.Data;

public class UserService : IUserService
{

    IConfiguration configuration;

    public UserService(IConfiguration config)
    {
        configuration = config;
    }

    #region Methode ADO.NET

    /*ADO.NET*/
    /*get the liste of users in the database*/
    public Task<User[]> GetUsersAsync()
    {
        var conn = new NpgsqlConnection(connectionString: configuration["DefaultConectionString"]);
        conn.Open();

        var cmder = new NpgsqlCommand();
        cmder.Connection = conn;
        cmder.CommandText = "select * from public.\"User\"";
        var reader = cmder.ExecuteReader();

        var result = new List<User>();

        while (reader.Read())
        {

            result.Add(new User()
            {
                UserID = (int)reader["UserID"],
                FirstName = (string)reader["FirstName"],
                LastName = (string)reader["LastName"],
            });


        }


        conn.Close();
        return Task.FromResult(result.ToArray());
    }

    /*ADO.NET*/
    /*add a user to the database*/
    public Task<User> AddUserAsync(User user)
    {
        if (user == null)
            return Task.FromCanceled<User>(new CancellationToken());

        var conn = new NpgsqlConnection(connectionString: configuration["DefaultConectionString"]);
        conn.Open();
        var cmder = new NpgsqlCommand();
        cmder.Connection = conn;
        const string InsertSQL = "insert into public.\"User\" (\"FirstName\", \"LastName\") values (@FirstName, @LastName)";
        cmder.Parameters.AddWithValue("FirstName", user.FirstName);
        cmder.Parameters.AddWithValue("LastName", user.LastName??"");
        cmder.CommandText = InsertSQL;
        cmder.ExecuteNonQuery();

        conn.Close();

        return Task.FromResult(user);
    }

    /*ADO.NET*/
    /*delete a user from the database*/
    public Task<bool> DeleteUserAsync(User user)
    {
         if (user == null)
            return Task<bool>.FromResult(false);

        var conn = new NpgsqlConnection(connectionString: configuration["DefaultConectionString"]);
        conn.Open();
        var cmder = new NpgsqlCommand();
        cmder.Connection = conn;
        const string DeleteSQL = "Delete from public.\"User\" where \"UserID\" = @UserID";
        cmder.Parameters.AddWithValue("UserID", user.UserID);
        cmder.CommandText = DeleteSQL;
        cmder.ExecuteNonQuery();

        conn.Close();

        return Task<bool>.FromResult(true);
    }

    /*ADO.NET*/
    /*update a user in the database*/
    public Task<User> UpdateUserAsync(User user)
    {
        if (user == null)
            return Task.FromCanceled<User>(new CancellationToken());

        var conn = new NpgsqlConnection(connectionString: configuration["DefaultConectionString"]);
        conn.Open();
        var cmder = new NpgsqlCommand();
        cmder.Connection = conn;
        const string UpdateSQL = "Update public.\"User\" set \"FirstName\"=@FirstName, \"LastName\"=@LastName where \"UserID\" = @UserID";
        cmder.Parameters.AddWithValue("FirstName", user.FirstName);
        cmder.Parameters.AddWithValue("LastName", user.LastName??"");
        cmder.Parameters.AddWithValue("UserID", user.UserID);
        cmder.CommandText = UpdateSQL;
        cmder.ExecuteNonQuery();
        return Task.FromResult(user);
    }
    #endregion
}