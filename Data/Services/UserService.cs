using Npgsql;
using Dapper;

namespace BlazinRoleGame.Data;

public class UserService : IUserService
{
    private readonly string _configuration;

    public UserService(IConfiguration config)
    {
        _configuration = config["DefaultConectionString"] ?? throw new ArgumentNullException(nameof(config), "Configuration cannot be null");
    }

    private T ExecuteDbOperation<T>(Func<NpgsqlConnection, T> operation)
    {
        using (var conn = new NpgsqlConnection(_configuration))
        {
            conn.Open();
            try
            {
                return operation(conn);
            }
            catch
            {
                // Handle or log exceptions here as per your requirements.
                throw;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
    }

    #region Methode ADO.NET

    /*ADO.NET*/
    /*get the liste of users in the database*/
    public Task<User[]> GetUsersAsync()
    {
        return Task.FromResult(ExecuteDbOperation(conn =>
      {
          var cmder = new NpgsqlCommand("select * from public.\"User\"", conn);
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

          return result.ToArray();
      }));
    }

    /*ADO.NET*/
    /*add a user to the database*/
    public Task<User> AddUserAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return Task.FromResult(ExecuteDbOperation(conn =>
        {
            var cmder = new NpgsqlCommand("insert into public.\"User\" (\"FirstName\", \"LastName\") values (@FirstName, @LastName)", conn);
            cmder.Parameters.AddWithValue("FirstName", user.FirstName);
            cmder.Parameters.AddWithValue("LastName", user.LastName ?? "");
            cmder.ExecuteNonQuery();

            return user; // Here you might want to return the created user with its new ID (if it's auto-generated in the DB)
        }));
    }

    /*ADO.NET*/
    /*delete a user from the database*/
    public Task<bool> DeleteUserAsync(User user)
    {
        if (user == null)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(ExecuteDbOperation(conn =>
        {
            var cmder = new NpgsqlCommand("Delete from public.\"User\" where \"UserID\" = @UserID", conn);
            cmder.Parameters.AddWithValue("UserID", user.UserID);
            cmder.ExecuteNonQuery();

            return true;
        }));
    }

    /*ADO.NET*/
    /*update a user in the database*/
    public Task<User> UpdateUserAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return Task.FromResult(ExecuteDbOperation(conn =>
        {
            var cmder = new NpgsqlCommand("Update public.\"User\" set \"FirstName\"=@FirstName, \"LastName\"=@LastName where \"UserID\" = @UserID", conn);
            cmder.Parameters.AddWithValue("FirstName", user.FirstName);
            cmder.Parameters.AddWithValue("LastName", user.LastName ?? "");
            cmder.Parameters.AddWithValue("UserID", user.UserID);
            cmder.ExecuteNonQuery();

            return user;
        }));
    }
    #endregion
}

