﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Take.Elephant
{
    /// <summary>
    /// Defines a FIFO storage container.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQueue<T> : IReceiverQueue<T>, ISenderQueue<T>
    {
        /// <summary>
        /// Gets the number of items in the queue.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<long> GetLengthAsync(CancellationToken cancellationToken = default);
    }

    public interface IReceiverQueue<T>
    {
        /// <summary>
        /// Dequeues an item from the queue, if available.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<T> DequeueOrDefaultAsync(CancellationToken cancellationToken = default);
    }

    public interface ISenderQueue<T>
    {
        /// <summary>
        /// Enqueues an item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task EnqueueAsync(T item, CancellationToken cancellationToken = default);
    }

    public interface IPartitionSenderQueue<T>
    {
        /// <summary>
        /// Enqueues an item to a specific partition through the Key value.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task EnqueueAsync(T item, string key, CancellationToken cancellationToken = default);
    }

    public interface IBatchSenderQueue<T>
    {
        /// <summary>
        /// Enqueues a batch of items.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task EnqueueBatchAsync(IEnumerable<T> items, CancellationToken cancellationToken = default);
    }

    public interface IBatchReceiverQueue<T>
    {
        /// <summary>
        /// Enqueues a batch of items.
        /// </summary>
        /// <param name="maxBatchSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> DequeueBatchAsync(int maxBatchSize, CancellationToken cancellationToken);
    }    
}
