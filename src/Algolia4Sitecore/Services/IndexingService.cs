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

        protected SearchClient Client
        {
            get
            {
                var searchClient = this.client;
                if (searchClient != null)
                {
                    return searchClient;
                }

                var config = new SearchConfig(
                    Settings.AppName,
                    Settings.AdminApiKey
                );

                config.UserAgent.AddSegment("Brimit_Sitecore_Integration", "2.0");

                return (this.client = new SearchClient(config));
            }
        }

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
            if (item.HasBaseTemplate(SampleDocumentModel.TemplateId))
            {
                var asset = new SampleDocumentModel(item, linkManager, mediaManager);
                Client.SaveObject(indexName: indexConfiguration.AlgoliaName, asset);
            }
        }

        private void DeleteItemFromIndex(Item item, IndexConfiguration indexConfiguration)
        {
            if (item.HasBaseTemplate(SampleDocumentModel.TemplateId))
            {
                Client.DeleteObject(indexName: indexConfiguration.AlgoliaName, ItemRecord.GetObjectId(item));
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
                    Client.GetSettings(indexName: indexName);
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
                Client.SetSettings(indexName: indexName, settings);
            }
        }
    }
}