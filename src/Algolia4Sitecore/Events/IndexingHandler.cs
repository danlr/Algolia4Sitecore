using Sitecore.Diagnostics;
using System;
using Algolia4Sitecore.Services;
using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecore.Publishing;
using Sitecore.Publishing.Pipelines.PublishItem;

namespace Algolia4Sitecore.Events
{
    public class IndexingHandler
    {
        private readonly IIndexingService indexingService;

        public IndexingHandler(IIndexingService indexingService)
        {
            this.indexingService =indexingService;
        }

        public void OnItemDeleted(object sender, EventArgs args)
        {
            Item item = Event.ExtractParameter(args, 0) as Item;
            if (item != null)
            {
                indexingService.DeleteItem(item);
                Log.Warn("Deleting item from algolia " + item.Paths.Path, this);
            }
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

            // if (!this.indexingService.ItemShouldBeIndexed(item))
            // {
            //     return;
            // }

            if (context.Action == PublishAction.DeleteTargetItem)
            {
                IndexingQueue.IndexingQueue.Delete(item);
            }
            else if (context.Action != PublishAction.Skip)
            {
                IndexingQueue.IndexingQueue.Add(item);
            }
        }
    }
}