namespace Algolia4Sitecore
{
    using System.Linq;

    using Configuration;

    using Models.JsonDto;

    public class Settings
    {
        public static string[] IndexingRoot => Sitecore.Configuration.Settings.GetSetting("Algolia4Sitecore.IndexingRoot").Split('|');

        public static string IndexingDatabase => Sitecore.Configuration.Settings.GetSetting("Algolia4Sitecore.IndexingDatabase");

        public static string PageIndexesPrefix => Sitecore.Configuration.Settings.GetSetting("Algolia4Sitecore.PageIndexesPrefix");

        public static bool IndexOnlyPages => Sitecore.Configuration.Settings.GetBoolSetting("Algolia4Sitecore.IndexOnlyPages", true);

        public static string ApiAdminKey => Sitecore.Configuration.Settings.GetSetting("Algolia4Sitecore.ApiAdminKey");

        public static string ApiApplicationId => Sitecore.Configuration.Settings.GetSetting("Algolia4Sitecore.ApiApplicationId");

        public static IndexingSettings IndexingSettings => Sitecore.Configuration.Factory.CreateObject("algoliaSearch/indexingSettings", true) as IndexingSettings;

        public static IndexSettings GetDefaultIndexSettings()
        {
            var settings = new IndexSettings();

            settings.AttributesToIndex = IndexingSettings.IncludedFields
                                                                        .Where(f => f.Indexed)
                                                                        .Select(f => f.FieldName)
                                                                        .ToList();

            settings.AttributesForFaceting = IndexingSettings.IncludedFields
                                                                        .Where(f => f.FieldType == FieldType.Facet || f.FieldType == FieldType.Boolean)
                                                                        .Select(f => f.FieldName)
                                                                        .ToList();
            settings.AttributesForFaceting.AddRange(IndexingSettings.IncludedFields
                                                                        .Where(f => f.FieldType == FieldType.Facet)
                                                                        .Select(f => f.FieldName + "_ids"));

            return settings;
        }
    }
}