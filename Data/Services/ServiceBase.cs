using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks.Dataflow;
using Dapper;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Npgsql;

namespace BlazinRoleGame.Datads
{
    public class ServiceBase<Titem> : ISupportCRUD<Titem> where Titem : class
    {
        private readonly string _connectionString;
        public ServiceBase(IConfiguration configuration)
        {
            _connectionString = configuration["DefaultConectionString"] ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null");
        }

        /// <summary>
        /// Add a new entity to the database
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<Titem> AddAsync(Titem entity)
        {

            var type = entity.GetType();
            var className = type.Name;
            var primaryKeyProperty = type.GetProperties().FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());
            
            var propNameValue = type.GetProperties()
                .Where(prop => prop != primaryKeyProperty)
                .ToDictionary(prop => prop.Name, p => p.GetValue(entity));

            var properties = string.Join(", ", propNameValue.Keys.Select(propName => $"\"{propName}\""));
            var values = string.Join(", ", propNameValue.Keys.Select(propName => $"@{propName}"));


            if (primaryKeyProperty == null)
            {
                throw new Exception("No primary key found");
            }

            var query = $"INSERT INTO public.\"{className}\" ({properties}) values ({values}) returning \"{primaryKeyProperty.Name}\";";

            return Task.FromResult(ExecuteDbOperationForOne(conn =>
            {
                int result = conn.ExecuteScalar<int>(query, param: propNameValue);
                entity.GetType().GetProperty(primaryKeyProperty.Name).SetValue(entity, result);
                return entity;
            }));
        }

        /// <summary>
        /// Delete an entity from the database
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(Titem entity)
        {
            var type = entity.GetType();
            var className = type.Name;
            var primaryKeyProperty = type.GetProperties().FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());

            if (primaryKeyProperty == null)
                throw new Exception("No primary key found");

            var query = $"DELETE FROM public.\"{className}\" WHERE \"{primaryKeyProperty.Name}\" = {primaryKeyProperty.GetValue(entity)};";

            var result = ExecuteDbOperationForOne(conn =>
            {
                var deletedEntity = conn.QuerySingleOrDefault<Titem>(query, param: primaryKeyProperty);
                return deletedEntity;
            });

            if (result == null)
                return Task.FromResult(true);
            else
                return Task.FromResult(false);
        }

        /// <summary>
        /// Get all entities from the database
        /// </summary>
        /// <returns></returns>
        public Task<List<Titem>> GetAllAsync()
        {
            var query = $"SELECT * FROM public.\"{typeof(Titem).Name}\";";
            return Task.FromResult(ExecuteDbOperationForM(conn =>
            {
                return conn.Query<Titem>(query).ToList();
            }));
        }

        /// <summary>
        /// Get an entity by its primary key
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<Titem> GetById(Titem entity)
        {
            var type = entity.GetType();
            var className = type.Name;

            var primaryKeyProperty = type.GetProperties().FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());

            if (primaryKeyProperty == null)
                throw new Exception("No primary key found");

            var query = "SELECT * FROM public.\"{className}\" WHERE \"{primaryKeyProperty.Name}\" = @{primaryKeyProperty.Name};";

            return Task.FromResult(ExecuteDbOperationForOne(conn =>
            {
                var getEntityId = conn.QuerySingleAsync<int>(query, param: primaryKeyProperty);
                return entity;
            }));
        }

        /// <summary>
        /// Update an entity in the database
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<Titem> UpdateAsync(Titem entity)
        {
            var type = entity.GetType();
            var className = type.Name;
            var primaryKeyProperty = type.GetProperties().FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());

            if (primaryKeyProperty == null)
            {
                throw new Exception("No primary key found");
            }

            var propNameValue = type.GetProperties()
                                .ToDictionary(prop => prop.Name, p => p.GetValue(entity));


            var query = $"UPDATE public.\"{className}\" SET ";

            foreach (var prop in propNameValue)
            {
                if (prop.Key == primaryKeyProperty.Name)
                    continue;

                if (prop.Key != propNameValue.Last().Key)
                    query += $"\"{prop.Key}\" = @{prop.Key}, ";
                else
                    query += $"\"{prop.Key}\" = @{prop.Key} ";
            }

            query += $"WHERE \"{primaryKeyProperty.Name}\" = @{primaryKeyProperty.Name};";

            return Task.FromResult(ExecuteDbOperationForOne(conn =>
            {
                var changedEntity = conn.QuerySingleAsync(query, param: propNameValue);
                return entity;
            }));
        }

        /// <summary>
        /// Execute a database operation for a single item
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        protected virtual Titem ExecuteDbOperationForOne(Func<NpgsqlConnection, Titem> operation)
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

        /// <summary>
        /// Execute a database operation for a list of items
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        protected virtual List<Titem> ExecuteDbOperationForM(Func<NpgsqlConnection, List<Titem>> operation)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                try
                {
                    return operation(conn);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
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