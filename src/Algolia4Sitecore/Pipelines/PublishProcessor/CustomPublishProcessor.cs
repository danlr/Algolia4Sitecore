using System.Linq;
using Algolia4Sitecore.Services;
using Sitecore.Diagnostics;
using Sitecore.Publishing.Pipelines.Publish;

namespace Algolia4Sitecore.Pipelines.PublishProcessor
{
    class CustomPublishProcessor : Sitecore.Publishing.Pipelines.Publish.PublishProcessor
    {
        private readonly IIndexingService indexingService;

        public CustomPublishProcessor(IIndexingService indexingService)
            : base()
        {
            this.indexingService = indexingService;
        }

        public override void Process(PublishContext context)
        {
            Assert.ArgumentNotNull(context, "context");

            if (context.Aborted)
                return;

            var processedItems = context.ProcessedPublishingCandidates.Keys
                .Select(i => context.PublishOptions.TargetDatabase.GetItem(i.ItemId)).Where(j => j != null);

            foreach (var processedItem in processedItems)
            {
                indexingService.UpdateItem(processedItem);
            }
        }
    }
}