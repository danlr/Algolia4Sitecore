namespace Algolia4Sitecore.Commands
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using Indexing;
    using IndexingQueue;
    using Services;
    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Applications.Dialogs.ProgressBoxes;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Web.UI.Sheer;

    public class ReindexTree : Command
    {
        private readonly IIndexingService indexingService;

        public ReindexTree()
        {
            this.indexingService = new IndexingService(new SimpleItemCrawler(new BaseItemParser()));
        }

        protected Handle JobHandle { get; set; }

        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            Item contextItem = context.Items[0];
            Assert.IsNotNull(contextItem, "context item cannot be null");
            Context.ClientPage.Start(this, "Run",
                new NameValueCollection
                {
                    { "itemUri", contextItem.Uri.ToString() },
                    { "itemPath", contextItem.Paths.ContentPath }
                });
        }

        protected void Run(ClientPipelineArgs args)
        {
            string parameter = args.Parameters["itemPath"];
            if (string.IsNullOrEmpty(parameter))
            {
                return;
            }

            ProgressBox.ExecuteSync($"Re-Index Tree. ({parameter})", "Re-indexing the current item with subitems.", "Applications/16x16/replace2.png", this.Refresh, this.RefreshDone);
        }

        private static Item GetItemByUri(string uri)
        {
            ItemUri itemUri = ItemUri.Parse(uri);
            return Sitecore.Configuration.Factory.GetDatabase(itemUri.DatabaseName).GetItem(itemUri.ToDataUri());
        }

        private void Refresh(ClientPipelineArgs args)
        {
            Item rootItem = GetItemByUri(args.Parameters["itemUri"]);
            if (rootItem == null)
            {
                return;
            }

            List<IndexingQueueItem> itemsToIndex = this.GetIndexingList(rootItem);

            this.indexingService.IndexPageItemsAsync(itemsToIndex).Wait();

            if (Context.Job != null)
            {
                Context.Job.Status.Messages.Add("Item indexed");
            }
        }

        private List<IndexingQueueItem> GetIndexingList(Item rootItem)
        {
            List<IndexingQueueItem> result = new List<IndexingQueueItem>();

            if (Context.Job != null)
            {
                Context.Job.Status.Messages.Add(rootItem.Paths.FullPath);
            }

            if (this.indexingService.ItemShouldBeIndexed(rootItem))
            {
                result.Add(new IndexingQueueItem(rootItem));
            }
            else
            {
                if (Context.Job != null)
                {
                    Context.Job.Status.Messages.Add("skip");
                }
            }

            foreach (Item item in rootItem.GetChildren())
            {
                result.AddRange(this.GetIndexingList(item));
            }

            return result;
        }

        private void RefreshDone(ClientPipelineArgs args)
        {
            SheerResponse.Alert("The current tree has been re-indexed successfully.");
        }
    }
}