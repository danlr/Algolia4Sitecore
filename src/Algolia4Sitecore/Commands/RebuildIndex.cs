namespace Algolia4Sitecore.Commands
{
    using Indexing;
    using Services;
    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Managers;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Shell.Applications.Dialogs.ProgressBoxes;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Web.UI.Sheer;

    public class RebuildIndex : Command
    {
        private readonly IIndexingService indexingService;

        public RebuildIndex()
        {
            this.indexingService = new IndexingService(new SimpleItemCrawler(new BaseItemParser()));
        }

        protected Handle JobHandle { get; set; }

        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            Context.ClientPage.Start(this, "Run");
        }

        protected void Run(ClientPipelineArgs args)
        {
            ProgressBox.ExecuteSync("Rebuild indexes", "Completely rebuild search indexes.", "Applications/16x16/replace2.png", this.Rebuild, this.Done);
        }

        private void Rebuild(ClientPipelineArgs args)
        {
            var languages = LanguageManager.GetLanguages(Database.GetDatabase(Settings.IndexingDatabase));

            foreach (Language language in languages)
            {
                string indexName = this.indexingService.GetPagesIndexName(language.Name);

                Context.Job.Status.Messages.Add("Rebuilding index: " + indexName);

                var messages = this.indexingService.RebuildIndex(language.Name);

                foreach (string message in messages)
                {
                    Context.Job.Status.Messages.Add(message);
                }
            }
        }

        private void Done(ClientPipelineArgs args)
        {
            SheerResponse.Alert("Indexes have been rebuilt successfully.");
        }
    }
}