namespace Algolia4Sitecore.Events
{
    using System;
    using Indexing;
    using IndexingQueue;
    using Services;
    using Sitecore.Data.Items;
    using Sitecore.Events;
    using Sitecore.Publishing;
    using Sitecore.Publishing.Pipelines.PublishItem;

    public class IndexingHandler
    {
        private readonly IIndexingService indexingService;

        public IndexingHandler()
        {
            this.indexingService = new IndexingService(new SimpleItemCrawler(new BaseItemParser()));
        }

        public void OnItemDeleted(object sender, EventArgs args)
        {
            Item item = Event.ExtractParameter(args, 0) as Item;

            if (item == null)
            {
                return;
            }

            if (!this.indexingService.ItemShouldBeIndexed(item))
            {
                return;
            }

            IndexingQueue.Delete(item);
        }

        public void OnItemProcessed(object sender, EventArgs args)
        {
            var arguments = args as ItemProcessedEventArgs;

            var context = arguments?.Context;

            Item item = context?.VersionToPublish;

            if (item == null)
            {
                return;
            }

            if (!this.indexingService.ItemShouldBeIndexed(item))
            {
                return;
            }

            if (context.Action == PublishAction.DeleteTargetItem)
            {
                IndexingQueue.Delete(item);
            }
            else if (context.Action != PublishAction.Skip)
            {
                IndexingQueue.Add(item);
            }
        }
    }
}