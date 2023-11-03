


using System.ComponentModel.DataAnnotations;
using Dapper;
using Npgsql;

namespace BlazinRoleGame.Data
{

    public class ServiceBase<Titem> : ISupportCRUD<Titem> where Titem : class
    {
        private readonly string _connectionString;

        public ServiceBase(IConfiguration configuration)
        {
            _connectionString = configuration["DefaultConectionString"] ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null");
        }

        public Task<Titem> AddAsync(Titem entity)
        {

            var type = entity.GetType();
            var className = type.Name;

            var propNameValue = type.GetProperties()
                                .ToDictionary(prop => prop.Name, p => p.GetValue(entity));

            var properties = string.Join(", ", propNameValue.Keys.Select(propName => $"\"{propName}\""));
            var values = string.Join(", ", propNameValue.Keys.Select(propName => $"@{propName}"));
            var primaryKeyProperty = type.GetProperties().FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());

            var query = $"INSERT INTO public.\"{className}\" ({properties}) values ({values}) RETURNING {primaryKeyProperty.Name};";

            return Task.FromResult(ExecuteDbOperation(conn =>
            {
                var newID = conn.QuerySingleAsync<int>(query, param: propNameValue);
                primaryKeyProperty.SetValue(entity, newID);

                return entity;
            }));
        }

        public Task<bool> DeleteAsync(Titem entity)
        {
            return Task.FromResult(true);
        }

        public Task<List<Titem>> GetAllAsync()
        {
            var result = new List<Titem>();
            return Task.FromResult(result);
        }

        public Task<Titem> GetById(Titem entity)
        {
            var type = entity.GetType();
            var className = type.Name;


            var primaryKeyProperty = type.GetProperties().FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());

            return null;
        }

        public Task<Titem> UpdateAsync(Titem entity)
        {
            return null;
        }

        protected virtual Titem ExecuteDbOperation(Func<NpgsqlConnection, Titem> operation)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                try
                {
                    return operation(conn);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
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
    }

}