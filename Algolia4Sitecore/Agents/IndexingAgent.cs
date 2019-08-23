namespace Algolia4Sitecore.Agents
{
    using System;
    using System.Collections.Generic;
    using Indexing;
    using IndexingQueue;
    using Services;

    using Sitecore.Diagnostics;

    public class IndexingAgent
    {
        private readonly IIndexingService indexingService;

        public IndexingAgent()
        {
            this.indexingService = new IndexingService(new SimpleItemCrawler(new BaseItemParser()));
        }

        public void Run()
        {
            if (IndexingQueue.Items.IsEmpty)
            {
                return;
            }

            TimeSpan timeSinceLastAdd = DateTime.UtcNow - IndexingQueue.LastChange;

            if (timeSinceLastAdd.TotalMinutes < 1 && IndexingQueue.Items.Count < 1000)
            {
                return; // wait, probably publish is running and not too many items in queue
            }

            List<IndexingQueueItem> items = new List<IndexingQueueItem>();

            while (items.Count < 1000)
            {
                var queueItem = IndexingQueue.Dequeue();

                if (queueItem.Equals(IndexingQueueItem.Empty))
                {
                    break;
                }

                items.Add(queueItem);
            }

            Log.Info("Start indexing: " + items.Count + " items", this);

            this.indexingService.IndexPageItems(items);

            Log.Info("Indexing finished", this);
        }
    }
}