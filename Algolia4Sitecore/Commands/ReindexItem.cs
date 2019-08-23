namespace Algolia4Sitecore.Commands
{
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

    public class ReindexItem : Command
    {
        private readonly IIndexingService indexingService;

        public ReindexItem()
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

            ProgressBox.ExecuteSync($"Re-Index Item. ({parameter})", "Re-indexing the current item.", "Applications/16x16/replace2.png", this.Refresh, this.RefreshDone);
        }

        private static Item GetItemByUri(string uri)
        {
            ItemUri itemUri = ItemUri.Parse(uri);
            return Sitecore.Configuration.Factory.GetDatabase(itemUri.DatabaseName).GetItem(itemUri.ToDataUri());
        }

        private void Refresh(ClientPipelineArgs args)
        {
            // this.JobHandle = Context.Job.Handle;
            Item itemByUri = GetItemByUri(args.Parameters["itemUri"]);
            if (itemByUri == null)
            {
                return;
            }

            if (!this.indexingService.ItemShouldBeIndexed(itemByUri))
            {
                return;
            }

            var queueItem = new IndexingQueueItem(itemByUri);

            this.indexingService.IndexPageItemAsync(queueItem).Wait();

            // Job job = JobManager.GetJob(this.JobHandle);
            if (Context.Job != null)
            {
                Context.Job.Status.Messages.Add("Item indexed");
            }
        }

        private void RefreshDone(ClientPipelineArgs args)
        {
            SheerResponse.Alert("The current item has been re-indexed successfully.");
        }
    }
}