namespace Algolia4Sitecore.IndexingQueue
{
    using System;
    using System.Collections.Concurrent;

    using Sitecore.Data.Items;

    public static class IndexingQueue
    {
        public static ConcurrentQueue<IndexingQueueItem> Items { get; } = new ConcurrentQueue<IndexingQueueItem>();

        public static DateTime LastChange { get; private set; }

        public static void Add(Item item)
        {
            Items.Enqueue(new IndexingQueueItem(item));
            LastChange = DateTime.UtcNow;
        }

        public static void Delete(Item item)
        {
            Items.Enqueue(new IndexingQueueItem(item, deleteFromIndex: true));
            LastChange = DateTime.UtcNow;
        }

        public static IndexingQueueItem Dequeue()
        {
            IndexingQueueItem item = IndexingQueueItem.Empty;

            if (!Items.IsEmpty)
            {
                Items.TryDequeue(out item);
            }

            return item;
        }
    }
}