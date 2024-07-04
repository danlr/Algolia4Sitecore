using Algolia.Search.Clients;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using Algolia.Search.Exceptions;
using Algolia4Sitecore.Configuration;
using Algolia4Sitecore.Models;
using Sitecore.Abstractions;
using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace Algolia4Sitecore.Services
{
    public class IndexingService : IIndexingService
    {
        private readonly BaseMediaManager mediaManager;
        private readonly BaseLinkManager linkManager;
        private SearchClient client;

        public IndexingService(BaseLinkManager linkManager, BaseMediaManager mediaManager)
        {
            this.mediaManager = mediaManager;
            this.linkManager = linkManager;
        }

        protected SearchClient Client => this.client ?? (this.client = new SearchClient(Settings.AppName, Settings.AdminApiKey));

        protected List<IndexConfiguration> Indexes => Settings.IndexingConfiguration.Indexes.Values.ToList();

        public void UpdateItem(Item item)
        {
            foreach (IndexConfiguration indexConfiguration in Indexes.Where(index => ItemBelongsToIndex(item, index)))
            {
                Log.Info($"Sending item {item.Paths.FullPath} to algolia", this);
                SendItemToIndex(item, indexConfiguration);
            }
        }

        public void DeleteItem(Item item)
        {
            foreach (IndexConfiguration indexConfiguration in Indexes.Where(index => ItemBelongsToIndex(item, index)))
            {
                Log.Info($"Sending item {item.Paths.FullPath} to algolia", this);
                DeleteItemFromIndex(item, indexConfiguration);
            }
        }

        private void SendItemToIndex(Item item, IndexConfiguration indexConfiguration)
        {
            var index = Client.InitIndex(indexConfiguration.AlgoliaName);

            if (item.HasBaseTemplate(SampleDocumentModel.TemplateId))
            {
                var asset = new SampleDocumentModel(item, linkManager, mediaManager);
                index.SaveObject(asset);
            }
        }

        private void DeleteItemFromIndex(Item item, IndexConfiguration indexConfiguration)
        {
            var index = Client.InitIndex(indexConfiguration.AlgoliaName);

            if (item.HasBaseTemplate(SampleDocumentModel.TemplateId))
            {
                index.DeleteObject(ItemRecord.GetObjectId(item));
            }
        }

        protected bool ItemBelongsToIndex(Item item, IndexConfiguration index)
        {
            return index.Templates.Contains(item.TemplateID.Guid) && (index.RootPaths.Any(path => item.Paths.FullPath.StartsWith(path, StringComparison.InvariantCultureIgnoreCase)));
        }

        public void InitAllIndexes(bool force = false)
        {
            foreach (var index in Indexes)
            {
                InitIndex(index, Language.Parse("en"), force);
            }
        }

        public void InitIndex(IndexConfiguration index, Language language, bool force = false)
        {
            string indexName = index.AlgoliaName;

            var algoliaIndex = Client.InitIndex(indexName);

            if (force)
            {
                Log.Info($"Algolia:: Force init index:{indexName}", this);
                SetSettings();
            }
            else
            {
                try
                {
                    // will fail if index just created with 'InitIndex' and has no settings
                    algoliaIndex.GetSettings();
                }
                catch (Exception ex) when ((ex is AlgoliaApiException) || (ex is AlgoliaException)) // index does not exist
                {
                    Log.Info($"Algolia:: Init index:{indexName}", this);
                    SetSettings();
                }
            }

            void SetSettings()
            {
                var settings = Settings.GetDefaultIndexSettings(index.Name, language.Name);
                algoliaIndex.SetSettings(settings, forwardToReplicas: false);
            }
        }
    }
}