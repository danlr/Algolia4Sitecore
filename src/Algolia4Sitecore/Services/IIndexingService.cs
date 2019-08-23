namespace Algolia4Sitecore.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using IndexingQueue;

    using Sitecore.Data.Items;

    public interface IIndexingService
    {
        void IndexPageItem(IndexingQueueItem queueItem);

        Task IndexPageItemAsync(IndexingQueueItem queueItem);

        void IndexPageItems(IEnumerable<IndexingQueueItem> queueItems);

        Task IndexPageItemsAsync(IEnumerable<IndexingQueueItem> queueItems);

        bool ItemShouldBeIndexed(Item item);

        string GetPagesIndexName(string language);

        IEnumerable<string> RebuildIndex(string language);
    }
}