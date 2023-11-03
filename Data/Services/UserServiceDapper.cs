using Npgsql;
using Dapper;

namespace BlazinRoleGame.Data;

public class UserServiceDapper : IUserService
{
    private readonly string _configuration;

    public UserServiceDapper(IConfiguration config)
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

    #region Methode Dapper
    /*Dapper*/
    /*get the liste of users in the database*/
    public Task<User[]> GetUsersAsync()
    {
        return Task.FromResult(ExecuteDbOperation(conn =>
        {
            var result = conn.Query<User>("select * from public.\"User\"");
            return result.ToArray();
        }));
    }

    /*Dapper*/
    /*add a user to the database*/
    public Task<User> AddUserAsync(User user)
    {

        if (user == null)
            return Task.FromCanceled<User>(new CancellationToken());

        return Task.FromResult(ExecuteDbOperation(conn =>
        {
            const string InsertSQL = "insert into public.\"User\" (\"FirstName\", \"LastName\") values (@FirstName, @LastName)";
            var add = conn.Query<User>(InsertSQL, new { FirstName = user.FirstName, LastName = user.LastName });
            return user;
        }));


    }

    /*Dapper*/
    /*delete a user from the database*/
    public Task<bool> DeleteUserAsync(User user)
    {
        if (user == null)
            return Task<bool>.FromResult(false);

        return Task.FromResult(ExecuteDbOperation(conn =>
        {
            const string DeleteSQL = "Delete from public.\"User\" where \"UserID\" = @UserID";
            var delete = conn.Query<User>(DeleteSQL, new { UserID = user.UserID });
            return true;
        }));
    }

    /*Dapper*/
    /*update a user in the database*/
    public Task<User> UpdateUserAsync(User user)
    {
        if (user == null)
            return Task.FromCanceled<User>(new CancellationToken());

        return Task.FromResult(ExecuteDbOperation(conn =>
        {
            string UpdateSQL = string.Format("Update public.\"User\" set \"FirstName\" = '{0}', \"LastName\"='{1}' where \"UserID\" = {2}", user.FirstName, user.LastName, user.UserID);
            var update = conn.Query<User>(UpdateSQL);
            return user;
        }));
    }

    #endregion


}