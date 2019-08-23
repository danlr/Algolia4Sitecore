namespace Algolia4Sitecore.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Algolia.Search;

    using Indexing;

    using IndexingQueue;

    using Newtonsoft.Json.Linq;

    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Globalization;
    using Sitecore.Mvc.Extensions;

    public class IndexingService : IIndexingService
    {
        private const int BatchSize = 1000;
        private readonly IAlgoliaCrawler crawler;
        private readonly Database database;
        private AlgoliaClient client;

        public IndexingService(IAlgoliaCrawler crawler)
        {
            this.crawler = crawler;
            this.database = Database.GetDatabase(Settings.IndexingDatabase);
        }

        public AlgoliaClient Client => this.client ?? (this.client = new AlgoliaClient(Settings.ApiApplicationId, Settings.ApiAdminKey));

        public void IndexPageItem(IndexingQueueItem queueItem)
        {
            Index index = this.Client.InitIndex(this.GetPagesIndexName(queueItem.Language));

            if (!queueItem.Deleted)
            {
                Item item = this.database.GetItem(ID.Parse(queueItem.Id), Language.Parse(queueItem.Language));

                index.AddObject(this.crawler.GetJsonForItem(item));
            }
            else
            {
                index.DeleteObject(this.crawler.GetObjectId(queueItem.Id));
            }
        }

        public async Task IndexPageItemAsync(IndexingQueueItem queueItem)
        {
            Index index = this.Client.InitIndex(this.GetPagesIndexName(queueItem.Language));

            if (!queueItem.Deleted)
            {
                Item item = this.database.GetItem(ID.Parse(queueItem.Id), Language.Parse(queueItem.Language));

                await index.AddObjectAsync(this.crawler.GetJsonForItem(item));
            }
            else
            {
                await index.DeleteObjectAsync(this.crawler.GetObjectId(queueItem.Id));
            }
        }

        public void IndexPageItems(IEnumerable<IndexingQueueItem> queueItems)
        {
            var groupedByLanguage = queueItems
                .Where(i => !i.Equals(IndexingQueueItem.Empty))
                .GroupBy(i => i.Language)
                .Where(grouping => !string.IsNullOrWhiteSpace(grouping.Key));

            foreach (IGrouping<string, IndexingQueueItem> grouping in groupedByLanguage)
            {
                Index index = this.Client.InitIndex(this.GetPagesIndexName(grouping.Key));
                Language language = Language.Parse(grouping.Key);
                List<JObject> objectsToUpdate = new List<JObject>();
                List<string> objectsToDelete = new List<string>();

                foreach (IndexingQueueItem indexingQueueItem in grouping)
                {
                    if (indexingQueueItem.Deleted)
                    {
                        objectsToDelete.Add(this.crawler.GetObjectId(indexingQueueItem.Id));
                    }
                    else
                    {
                        Item item = this.database.GetItem(ID.Parse(indexingQueueItem.Id), language);

                        if (item != null && item.Versions.Count > 0)
                        {
                            objectsToUpdate.Add(this.crawler.GetJsonForItem(item));
                        }
                    }
                }

                index.AddObjects(objectsToUpdate);
                index.DeleteObjects(objectsToDelete);
            }
        }

        public async Task IndexPageItemsAsync(IEnumerable<IndexingQueueItem> queueItems)
        {
            var groupedByLanguage = queueItems
                .Where(i => !i.Equals(IndexingQueueItem.Empty))
                .GroupBy(i => i.Language)
                .Where(grouping => !string.IsNullOrWhiteSpace(grouping.Key));

            foreach (IGrouping<string, IndexingQueueItem> grouping in groupedByLanguage)
            {
                Index index = this.Client.InitIndex(this.GetPagesIndexName(grouping.Key));
                Language language = Language.Parse(grouping.Key);
                List<JObject> objectsToUpdate = new List<JObject>();
                List<string> objectsToDelete = new List<string>();

                foreach (IndexingQueueItem indexingQueueItem in grouping)
                {
                    if (indexingQueueItem.Deleted)
                    {
                        objectsToDelete.Add(this.crawler.GetObjectId(indexingQueueItem.Id));
                    }
                    else
                    {
                        Item item = this.database.GetItem(ID.Parse(indexingQueueItem.Id), language);

                        if (item != null && item.Versions.Count > 0)
                        {
                            objectsToUpdate.Add(this.crawler.GetJsonForItem(item));
                        }
                    }
                }

                await index.AddObjectsAsync(objectsToUpdate);
                await index.DeleteObjectsAsync(objectsToDelete);
            }
        }

        public bool ItemShouldBeIndexed(Item item)
        {
            if (item == null)
            {
                return false;
            }

            if (!Settings.IndexingRoot.Any(r => item.Paths.Path.StartsWith(r.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            if (Settings.IndexingSettings.ExcludedTemplates.Any(t => item.TemplateID.Guid.Equals(t)))
            {
                return false; // todo: may be check base templates as well?
            }

            if (Settings.IndexOnlyPages)
            {
                if (item[FieldIDs.LayoutField].IsEmptyOrNull() &&
                    item[FieldIDs.FinalLayoutField].IsEmptyOrNull())
                {
                    return false;
                }
            }

            return true;
        }

        public string GetPagesIndexName(string language)
        {
            return Settings.PageIndexesPrefix + language;
        }

        public IEnumerable<string> RebuildIndex(string language)
        {
            var indexName = this.GetPagesIndexName(language);
            var tempIndexName = "temp_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + "_" + this.GetPagesIndexName(language); // TODO: integrate IClock
            Index tempIndex = this.Client.InitIndex(tempIndexName);

            //this.logger.Info("Rebuild algolia index " + indexName, this);

            tempIndex.SetSettings(JObject.FromObject(Settings.GetDefaultIndexSettings()), true);

            //this.logger.Info("Temporary algolia index: " + tempIndexName, this);

            yield return "Prepare temporary index " + tempIndexName;

            foreach (List<IndexingQueueItem> list in this.GetAllIndexItems(language))
            {
                //this.logger.Info("Add items: " + list.Count, this);

                yield return "Add items: " + list.Count;

                this.IndexPageItems(list, tempIndex, language);
            }

            //this.logger.Info("Switch indexes: " + tempIndexName + " to " + indexName, this);

            this.client.MoveIndex(tempIndexName, indexName);

            //this.logger.Info("done.", this);
        }

        private IEnumerable<List<IndexingQueueItem>> GetAllIndexItems(string language)
        {
            using (new LanguageSwitcher(language))
            {
                List<IndexingQueueItem> batch = new List<IndexingQueueItem>();

                foreach (string path in Settings.IndexingRoot)
                {
                    Item rootItem = this.database.GetItem(path);

                    foreach (var queueItem in this.GetIndexingList(rootItem))
                    {
                        batch.Add(queueItem);

                        if (batch.Count >= BatchSize)
                        {
                            yield return batch;
                            batch = new List<IndexingQueueItem>();
                        }
                    }
                }

                yield return batch;
            }
        }

        private void IndexPageItems(IEnumerable<IndexingQueueItem> queueItems, Index index, string languageName)
        {
            Language language = Language.Parse(languageName);
            List<JObject> objectsToUpdate = new List<JObject>();
            List<string> objectsToDelete = new List<string>();

            foreach (IndexingQueueItem indexingQueueItem in queueItems)
            {
                if (indexingQueueItem.Deleted)
                {
                    objectsToDelete.Add(this.crawler.GetObjectId(indexingQueueItem.Id));
                }
                else
                {
                    Item item = this.database.GetItem(ID.Parse(indexingQueueItem.Id), language);

                    if (item != null && item.Versions.Count > 0)
                    {
                        objectsToUpdate.Add(this.crawler.GetJsonForItem(item));
                    }
                }
            }

            index.AddObjects(objectsToUpdate);
            index.DeleteObjects(objectsToDelete);
        }

        private IEnumerable<IndexingQueueItem> GetIndexingList(Item rootItem)
        {
            if (this.ItemShouldBeIndexed(rootItem))
            {
                yield return new IndexingQueueItem(rootItem);
            }

            foreach (Item item in rootItem.GetChildren())
            {
                foreach (var indexable in this.GetIndexingList(item))
                {
                    yield return indexable;
                }
            }
        }
    }
}