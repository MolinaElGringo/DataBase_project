using Npgsql;

public class ConnexionDbService {
    protected readonly string _configuration;

    public ConnexionDbService(IConfiguration config)
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
                throw new Exception("Error doing the changes in the database!");
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
}