using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common.Caching
{
    public interface ICachingService<TItem, TConfiguration>
    {
        /// <summary>
        /// Add single item to cache
        /// </summary>
        Task AddItem(TItem item, TConfiguration configuration);

        /// <summary>
        /// Get all items from cache
        /// </summary>
        Task<IQueryable<KeyValuePair<TItem, TConfiguration>>> GetItems();

        /// <summary>
        /// Return current number of items in cache
        /// </summary>        
        Task<int> Count();

        /// <summary>
        /// Clear all items from cache
        /// </summary>
        Task Clear();

        /// <summary>
        /// Clear items from cache related to target configuration
        /// </summary>        
        Task Clear(TConfiguration configuration);

        /// <summary>
        /// Provides synchronization object to use for locking critical sections
        /// </summary>        
        object Locker { get; }
    }
}
