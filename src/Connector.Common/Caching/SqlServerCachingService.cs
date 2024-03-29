﻿using CluedIn.Core.Configuration;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common.Caching
{
    public class SqlServerCachingService<TItem, TConfiguration> : ICachingService<TItem, TConfiguration>
    {
        public object Locker { get; }

        private readonly string _connectionString;
        private readonly string _primaryConnectionStringKeyName = "Streams.Common.SqlCacheConnectionString";
        private readonly string _fallbackConnectionStringKeyName = Core.Constants.Configuration.ConnectionStrings.CluedInEntities;

        private readonly string _schemaName = "Streams";
        private readonly string _tableNamePrefix = "SqlCaching";
        private readonly string _noSchemaTableName;
        private readonly string _tableName;

        private readonly string _configurationColumn = "Configuration";
        private readonly string _dataColumn = "Data";

        private SqlServerCachingService(string id)
        {
            Locker = new object();
            var connectionStringSettings = ConfigurationManagerEx.ConnectionStrings[_primaryConnectionStringKeyName] ??
                ConfigurationManagerEx.ConnectionStrings[_fallbackConnectionStringKeyName];

            _connectionString = connectionStringSettings.ConnectionString;            
            _noSchemaTableName = $"{_tableNamePrefix}{id}";
            _tableName = $"{_schemaName}.{_tableNamePrefix}{id}";
        }

        /// <summary>
        /// Create and set up service SqlServerCachingService instance.
        /// </summary>
        /// <param name="id">unique connector identifier</param>        
        public static async Task<SqlServerCachingService<TItem, TConfiguration>> CreateCachingService(string id)
        {
            var service = new SqlServerCachingService<TItem, TConfiguration>(id);
            await service.EnsureTableCreated();

            return service;
        }

        public async Task AddItem(TItem item, TConfiguration configuration)
        {
            var serializedData = JsonConvert.SerializeObject(item);
            var serializedConfiguration = JsonConvert.SerializeObject(configuration);
            var query = $@"INSERT INTO {_tableName} ({_dataColumn}, {_configurationColumn})
                        VALUES (@{_dataColumn}, @{_configurationColumn});";

            var parameters = new[]
            {
                new SqlParameter($"@{_dataColumn}", serializedData),
                new SqlParameter($"@{_configurationColumn}", serializedConfiguration)
            };

            try
            {
                await ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Can't store item:\n {serializedData} \n with configurations:\n {serializedConfiguration}");
                throw;
            }
        }

        public async Task Clear()
        {
            var query = $"TRUNCATE TABLE {_tableName}";
            await ExecuteNonQuery(query);
        }

        public async Task Clear(TConfiguration configuration)
        {
            var serializedConfiguration = JsonConvert.SerializeObject(configuration);
            var query = $"DELETE FROM {_tableName} WHERE {_configurationColumn}='{serializedConfiguration}'";

            await ExecuteNonQuery(query);
        }

        public async Task<int> Count()
        {
            var query = $@"SELECT COUNT({_dataColumn}) FROM {_tableName}";
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(query, connection);
            await command.Connection.OpenAsync();

            return (int)await command.ExecuteScalarAsync();
        }

        public async Task<IQueryable<KeyValuePair<TItem, TConfiguration>>> GetItems()
        {
            var result = new List<KeyValuePair<TItem, TConfiguration>>();
            var query = $"SELECT * FROM {_tableName}";
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(query, connection);
            await command.Connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                var dataString = reader[_dataColumn].ToString();
                var configString = reader[_configurationColumn].ToString();

                var data = JsonConvert.DeserializeObject<TItem>(dataString);
                var config = JsonConvert.DeserializeObject<TConfiguration>(configString);
                result.Add(new KeyValuePair<TItem, TConfiguration>(data, config));
            }

            return result.AsQueryable();
        }

        private async Task<int> ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(query, connection);
            if (parameters != null && parameters.Any())
                command.Parameters.AddRange(parameters);

            await command.Connection.OpenAsync();

            return await command.ExecuteNonQueryAsync();
        }

        private async Task EnsureTableCreated()
        {
            var createSchema = @$"IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name='{_schemaName}')
                                    EXEC('CREATE SCHEMA [{_schemaName}]')";

            await ExecuteNonQuery(createSchema);

            var createTable = @$"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{_noSchemaTableName}' and xtype='U')
                                    CREATE TABLE {_tableName} (
                                    {_dataColumn} varchar(max) not null,
                                    {_configurationColumn} varchar(max) not null
                                    )";

            await ExecuteNonQuery(createTable);
        }
    }
}
