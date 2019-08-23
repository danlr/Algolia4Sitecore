namespace Algolia4Sitecore.Models.JsonDto
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    [JsonObject]
    public class IndexSettings
    {
        [JsonProperty("minWordSizefor1Typo")]
        public int MinWordSizefor1Typo { get; set; }

        [JsonProperty("minWordSizefor2Typos")]
        public int MinWordSizefor2Typos { get; set; }

        [JsonProperty("hitsPerPage")]
        public int HitsPerPage { get; set; }

        [JsonProperty("maxValuesPerFacet")]
        public int MaxValuesPerFacet { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("attributesToIndex")]
        public List<string> AttributesToIndex { get; set; }

        [JsonProperty("numericAttributesToIndex")]
        public List<string> NumericAttributesToIndex { get; set; }

        [JsonProperty("attributesToRetrieve")]
        public List<string> AttributesToRetrieve { get; set; }

        [JsonProperty("unretrievableAttributes")]
        public List<string> UnretrievableAttributes { get; set; }

        [JsonProperty("optionalWords")]
        public List<string> OptionalWords { get; set; }

        [JsonProperty("attributesForFaceting")]
        public List<string> AttributesForFaceting { get; set; }

        [JsonProperty("attributesToSnippet")]
        public List<string> AttributesToSnippet { get; set; }

        [JsonProperty("attributesToHighlight")]
        public List<string> AttributesToHighlight { get; set; }

        [JsonProperty("attributeForDistinct")]
        public List<string> AttributeForDistinct { get; set; }

        [JsonProperty("ranking")]
        public List<string> Ranking { get; set; }

        [JsonProperty("paginationLimitedTo")]
        public int PaginationLimitedTo { get; set; }

        [JsonProperty("exactOnSingleWordQuery")]
        public string ExactOnSingleWordQuery { get; set; }

        [JsonProperty("customRanking")]
        public List<string> CustomRanking { get; set; }

        [JsonProperty("separatorsToIndex")]
        public string SeparatorsToIndex { get; set; }

        [JsonProperty("removeWordsIfNoResults")]
        public string RemoveWordsIfNoResults { get; set; }

        [JsonProperty("queryType")]
        public string QueryType { get; set; }

        [JsonProperty("highlightPreTag")]
        public string HighlightPreTag { get; set; }

        [JsonProperty("highlightPostTag")]
        public string HighlightPostTag { get; set; }

        [JsonProperty("snippetEllipsisText")]
        public string SnippetEllipsisText { get; set; }

        [JsonProperty("alternativesAsExact")]
        public List<string> AlternativesAsExact { get; set; }
    }
}