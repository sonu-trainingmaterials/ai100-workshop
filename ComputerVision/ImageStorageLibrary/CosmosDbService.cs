﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;

namespace ImageStorageLibrary
{

    public interface ICosmosDbService<R> where R : class
    {
        Task<IEnumerable<R>> GetItemsAsync(string query, QueryRequestOptions options);
        Task<T> GetItemAsync<T>(string id);
        Task AddItemAsync<T>(T item);
        Task UpdateItemAsync<T>(string id, T item);
        Task DeleteItemAsync(string id);
    }

    public class CosmosDbService<R> : ICosmosDbService<R> where R : class
    {
        private Container _container;

        public CosmosDbService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddItemAsync<T>(T item)
        {
            await this._container.CreateItemAsync<T>(item, new PartitionKey((string)item.GetType().GetProperty("Id").GetValue(item, null)));
        }


        public async Task DeleteItemAsync(string id)
        {
            await this._container.DeleteItemAsync<R>(id, new PartitionKey(id));
        }

        public async Task<T> GetItemAsync<T>(string id)
        {
            try
            {
                ItemResponse<T> response = await this._container.ReadItemAsync<T>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default(T);
            }

        }

        public async Task<IEnumerable<R>> GetItemsAsync(string queryString, QueryRequestOptions options = null)
        {
            var query = this._container.GetItemQueryIterator<R>(queryString, null);
            List<R> results = new List<R>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateItemAsync<T>(string id, T item)
        {
            await this._container.UpsertItemAsync(item, new PartitionKey(id));
        }

    }
}

