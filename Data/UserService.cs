using Npgsql;
using Dapper;

namespace BlazinRoleGame.Data;

public class UserService
{

    IConfiguration configuration;

    public UserService(IConfiguration config)
    {
        configuration = config;
    }

    #region Methode Dapper
    /*Dapper*/
    /*get the liste of users in the database*/
    public Task<User[]> GetUsersAsyncDapper()
    {
        var conn = new NpgsqlConnection(connectionString: configuration["DefaultConectionString"]);
        conn.Open();

        var result = conn.Query<User>("select * from public.\"User\"");

        conn.Close();
        return Task.FromResult(result.ToArray());
    }

    /*Dapper*/
    /*add a user to the database*/
    public Task<User[]> AddUserDapper(User user)
    {

        if (user == null)
            return GetUsersAsync();

        var conn = new NpgsqlConnection(connectionString: configuration["DefaultConectionString"]);
        conn.Open();

        const string InsertSQL = "insert into public.\"User\" (\"FirstName\", \"LastName\") values (@FirstName, @LastName)";

        var add = conn.Query<User>(InsertSQL, new { FirstName = user.FirstName, LastName = user.LastName });

        conn.Close();
        return GetUsersAsyncDapper();
    }

    /*Dapper*/
    /*delete a user from the database*/
    public Task<User[]> DeleteUserDapper(User userToDelete)
    {
        if (userToDelete == null)
            return GetUsersAsync();

        var conn = new NpgsqlConnection(connectionString: configuration["DefaultConectionString"]);
        conn.Open();

        string DeleteSQL = "Delete from public.\"User\" where \"UserID\" =" + userToDelete.UserID;

        var delete = conn.Query<User>(DeleteSQL);

        conn.Close(); 

        return GetUsersAsyncDapper();
    }

    /*Dapper*/
    /*update a user in the database*/
    public Task<User[]> UpdateUserDapper(User userToUpdate)
    {
         if (userToUpdate == null)
            return GetUsersAsync();

        var conn = new NpgsqlConnection(connectionString: configuration["DefaultConectionString"]);
        conn.Open();

        string UpdateSQL = "Update public.\"User\" set \"FirstName\"="+userToUpdate.FirstName+", \"LastName\"="+userToUpdate.LastName+"where \"UserID\" ="+userToUpdate.UserID;

        var update = conn.Query<User>(UpdateSQL);

        conn.Close(); 

        return GetUsersAsyncDapper();
    }

    #endregion

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
    public Task<User[]> AddUser(User user)
    {
        if (user == null)
            return GetUsersAsync();

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

        return GetUsersAsync();
    }

    /*ADO.NET*/
    /*delete a user from the database*/
    public Task<User[]> DeleteUser(User userToDelete)
    {
         if (userToDelete == null)
            return GetUsersAsync();

        var conn = new NpgsqlConnection(connectionString: configuration["DefaultConectionString"]);
        conn.Open();
        var cmder = new NpgsqlCommand();
        cmder.Connection = conn;
        const string DeleteSQL = "Delete from public.\"User\" where \"UserID\" = @UserID";
        cmder.Parameters.AddWithValue("UserID", userToDelete.UserID);
        cmder.CommandText = DeleteSQL;
        cmder.ExecuteNonQuery();

        conn.Close();

        return GetUsersAsync();
    }

    /*ADO.NET*/
    /*update a user in the database*/
    public Task<User[]> UpdateUser(User userToUpdate)
    {
        if (userToUpdate == null)
            return GetUsersAsync();

        var conn = new NpgsqlConnection(connectionString: configuration["DefaultConectionString"]);
        conn.Open();
        var cmder = new NpgsqlCommand();
        cmder.Connection = conn;
        const string UpdateSQL = "Update public.\"User\" set \"FirstName\"=@FirstName, \"LastName\"=@LastName where \"UserID\" = @UserID";
        cmder.Parameters.AddWithValue("FirstName", userToUpdate.FirstName);
        cmder.Parameters.AddWithValue("LastName", userToUpdate.LastName??"");
        cmder.Parameters.AddWithValue("UserID", userToUpdate.UserID);
        cmder.CommandText = UpdateSQL;
        cmder.ExecuteNonQuery();

        return GetUsersAsync();
    }
    #endregion
}