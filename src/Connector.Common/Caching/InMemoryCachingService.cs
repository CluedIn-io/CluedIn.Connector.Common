using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common.Caching
{
    public class InMemoryCachingService<TItem, TConfiguration> : ICachingService<TItem, TConfiguration>
        where TConfiguration : class
    {
        public object Locker { get; }
        private List<KeyValuePair<TItem, TConfiguration>> _storage;

        public InMemoryCachingService()
        {
            Locker = new object();
            _storage = new List<KeyValuePair<TItem, TConfiguration>>();
        }

        public Task AddItem(TItem item, TConfiguration configuration)
        {
            _storage.Add(new KeyValuePair<TItem, TConfiguration>(item, configuration));

            return Task.CompletedTask;
        }

        public Task Clear()
        {
            _storage.Clear();

            return Task.CompletedTask;
        }

        public Task Clear(TConfiguration configuration)
        {
            _storage = _storage.Where(x => !x.Value.Equals(configuration)).ToList();

            return Task.CompletedTask;
        }

        public Task<int> Count()
        {
            return Task.FromResult(_storage.Count);
        }

        public Task<IQueryable<KeyValuePair<TItem, TConfiguration>>> GetItems()
        {
            return Task.FromResult(_storage.AsQueryable());
        }
    }
}
