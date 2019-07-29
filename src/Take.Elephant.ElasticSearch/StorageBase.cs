﻿using Nest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Take.Elephant.Elasticsearch.Mapping;

namespace Take.Elephant.Elasticsearch
{
    public class StorageBase<T> : IQueryableStorage<T> where T : class
    {
        protected readonly IElasticClient ElasticClient;
        private readonly IElasticsearchConfiguration _configuration;
        private readonly ConcurrentDictionary<string, Func<T, object>> _propertiesDictionary;

        protected IMapping Mapping { get; }

        private string _index => Mapping.Index ?? _configuration.DefaultIndex;

        public StorageBase(IElasticsearchConfiguration configuration, IMapping mapping)
        {
            _configuration = configuration;

            var settings = new ConnectionSettings(new Uri(_configuration.Hostname))
                .BasicAuthentication(_configuration.Username, _configuration.Password)
                .DefaultIndex(_configuration.DefaultIndex);

            Mapping = mapping;
            ElasticClient = new ElasticClient(settings);
            _propertiesDictionary = new ConcurrentDictionary<string, Func<T, object>>();
        }

        public StorageBase(IElasticClient elasticClient, IMapping mapping)
        {
            Mapping = mapping;
            ElasticClient = elasticClient;
            _propertiesDictionary = new ConcurrentDictionary<string, Func<T, object>>();
        }

        public async Task<QueryResult<T>> QueryAsync<TResult>(
            Expression<Func<T, bool>> where,
            Expression<Func<T, TResult>> select, int skip, int take,
            CancellationToken cancellationToken = default)
        {
            var queryDescriptor = where.ParseToQueryContainer<T>();

            var result = await ElasticClient.SearchAsync<T>(s => s
                .Index(_index)
                .Query(_ => queryDescriptor)
                .From(skip).Size(take), cancellationToken);

            return new QueryResult<T>(new AsyncEnumerableWrapper<T>(result.Documents), (int)result.Total);
        }

        public async Task<bool> ContainsKeyAsync(string key, CancellationToken cancellationToken = default)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var response = await ElasticClient.DocumentExistsAsync<T>(key, d => d
                .Index(_index), cancellationToken);

            return response.Exists;
        }

        public async Task<T> GetValueOrDefaultAsync(string key, CancellationToken cancellationToken = default)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var result = await ElasticClient.SearchAsync<T>(s => s
                .Index(_index)
                .Query(q => q.Ids(a => a.Values(key.ToString()))), cancellationToken);

            return result.Documents.FirstOrDefault();
        }

        public async Task<bool> TryAddAsync(string key, T value, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (overwrite || !await ContainsKeyAsync(key, cancellationToken))
            {
                var result = await ElasticClient.IndexAsync(new IndexRequest<T>(value, _index,
                    key.ToString()), cancellationToken);

                return result.IsValid;
            }

            return false;
        }

        public async Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var result = await ElasticClient.DeleteAsync<T>(key,
                d => d.Index(_index), cancellationToken);

            return result.IsValid;
        }

        protected string GetPropertyValue(T entity, string property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(property);
            }

            var propertyAcessor = _propertiesDictionary
                .GetOrAdd(property, e => 
                    TypeUtil.BuildGetAccessor(
                        typeof(T).GetProperties()
                        .Single(p => p.Name == property)));

            return propertyAcessor(entity)?.ToString();
        }
    }
}
