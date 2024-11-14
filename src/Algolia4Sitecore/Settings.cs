using Algolia.Search.Models.Search;

namespace Algolia4Sitecore
{
    using System.Collections.Generic;
    using System;
    using System.Linq;

    public class Settings
    {
        public static string IndexingDatabase => Sitecore.Configuration.Settings.GetSetting("Algolia4Sitecore.IndexingDatabase");
        
        public static bool IndexOnlyPages => Sitecore.Configuration.Settings.GetBoolSetting("Algolia4Sitecore.IndexOnlyPages", true);

        public static string AdminApiKey => Sitecore.Configuration.Settings.GetSetting("Algolia4Sitecore.AdminApiKey");

        public static string SearchApiKey => Sitecore.Configuration.Settings.GetSetting("Algolia4Sitecore.SearchApiKey");

        public static string AppName => Sitecore.Configuration.Settings.GetSetting("Algolia4Sitecore.App");

        public static Configuration.Configuration IndexingConfiguration => Sitecore.Configuration.Factory.CreateObject("algolia.indexing/configuration", true) as Configuration.Configuration;

        public static IndexSettings GetDefaultIndexSettings(string indexName, string language)
        {
            var index = IndexingConfiguration.Indexes[indexName];

            var languages = new List<SupportedLanguage>();
            if (Enum.TryParse<SupportedLanguage>(language, out var lang))
            {
                languages.Add(lang);
            }

            var indexSettings = new IndexSettings
            {
                SearchableAttributes = index.SearchableFields,
                AttributesForFaceting = index.FacetFields,
                HitsPerPage = index.HitsPerPage,
                MaxValuesPerFacet = index.MaxValuesPerFacet,
                MinWordSizefor1Typo = index.MinWordSizefor1Typo,
                MinWordSizefor2Typos = index.MinWordSizefor2Typos,
                PaginationLimitedTo = index.PaginationLimitedTo,
                AllowTyposOnNumericTokens = index.AllowTyposOnNumericTokens,
                UnretrievableAttributes = index.UnretrievableAttributes,
                IndexLanguages = languages,
                CustomRanking = string.IsNullOrEmpty(index.CustomRanking) ? null : index.CustomRanking.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                Ranking = string.IsNullOrEmpty(index.Ranking) ? null : index.Ranking.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()
            };

            return indexSettings;
        }
    }
}