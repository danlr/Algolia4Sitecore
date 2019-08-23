namespace Algolia4Sitecore.Pipelines.Initialize
{
    using System.Collections.Generic;
    using System.Linq;
    using Algolia.Search;
    using Configuration;
    using Models.JsonDto;
    using Newtonsoft.Json.Linq;
    using Sitecore.Diagnostics;
    using Sitecore.Pipelines;

    public class InitIndexes
    {
        public void Process(PipelineArgs args)
        {
            Log.Info("InitIndexes", this);
            var client = new AlgoliaClient(Settings.ApiApplicationId, Settings.ApiAdminKey);

            var indexes = client.ListIndexes().GetValue("items").ToObject<List<IndexInfo>>();

            indexes = indexes.Where(i => i.Name.StartsWith(Settings.PageIndexesPrefix)).ToList();

            Log.Info("Number of indexes:" + indexes.Count, this);

            foreach (IndexInfo indexInfo in indexes)
            {
                var index = client.InitIndex(indexInfo.Name);

                Log.Info("Index:" + indexInfo.Name, this);

                var settings = index.GetSettings().ToObject<IndexSettings>();

                if (settings.AttributesToIndex == null)
                {
                    // settings.AttributesToIndex = new List<string> { "title", "content", "name" };
                    settings.AttributesToIndex = Settings.IndexingSettings.IncludedFields
                                                                            .Where(f => f.Indexed)
                                                                            .Select(f => f.FieldName)
                                                                            .ToList();

                    settings.AttributesForFaceting = Settings.IndexingSettings.IncludedFields
                                                                            .Where(f => f.FieldType == FieldType.Facet)
                                                                            .Select(f => f.FieldName)
                                                                            .ToList();
                    var idsFields = settings.AttributesForFaceting.Select(a => a + "_ids").ToList();
                    settings.AttributesForFaceting.AddRange(idsFields);

                    index.SetSettings(JObject.FromObject(settings), true);
                }
            }
        }
    }
}