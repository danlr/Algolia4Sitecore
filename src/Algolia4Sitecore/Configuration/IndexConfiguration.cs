namespace Algolia4Sitecore.Configuration
{
    using System;
    using System.Collections.Generic;

    public class IndexConfiguration
    {
        public IndexConfiguration(string name, string algoliaName)
        {
            (Name, AlgoliaName) = (name, algoliaName);
        }

        public string Name { get; protected set; }

        public string AlgoliaName { get; protected set; }

        public int HitsPerPage { get; set; }

        public int MaxValuesPerFacet { get; set; }

        public int PaginationLimitedTo { get; set; }

        public int MinWordSizefor1Typo { get; set; }

        public int MinWordSizefor2Typos { get; set; }

        public string RemoveWordsIfNoResults { get; set; }

        public bool AllowTyposOnNumericTokens { get; set; }

        public bool RemoveStopWords { get; set; }

        public string Ranking { get; set; }

        public string CustomRanking { get; set; }

        public HashSet<string> RootPaths { get; protected set; } = new HashSet<string>();

        public HashSet<Guid> Templates { get; protected set; } = new HashSet<Guid>();

        public HashSet<string> ReplicaSorting { get; protected set; } = new HashSet<string>();

        public List<string> UnretrievableAttributes { get; protected set; } = new List<string>();

        public List<string> FacetFields { get; protected set; } = new List<string>();

        public List<string> SearchableFields { get; protected set; } = new List<string>();


        public void AddReplicaIndex(System.Xml.XmlNode node)
        {
            string replicaSorting = Sitecore.Xml.XmlUtil.GetValue(node);
            this.ReplicaSorting.Add(replicaSorting);
        }

        public void AddFacetField(System.Xml.XmlNode node)
        {
            string field = Sitecore.Xml.XmlUtil.GetValue(node);
            this.FacetFields.Add(field);
        }

        public void AddSearchableField(System.Xml.XmlNode node)
        {
            string field = Sitecore.Xml.XmlUtil.GetValue(node);
            this.SearchableFields.Add(field);
        }

        public void AddRootPath(System.Xml.XmlNode node)
        {
            string path = Sitecore.Xml.XmlUtil.GetValue(node);

            this.RootPaths.Add(path.Trim());
        }

        public void AddTemplate(System.Xml.XmlNode node)
        {
            string id = Sitecore.Xml.XmlUtil.GetValue(node);

            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            if (Guid.TryParse(id.Trim(), out var guid))
            {
                this.Templates.Add(guid);
            }
        }

        public void AddUnretrievableAttribute(System.Xml.XmlNode node)
        {
            string attribute = Sitecore.Xml.XmlUtil.GetValue(node);

            this.UnretrievableAttributes.Add(attribute.Trim());
        }

        public override string ToString()
        {
            return $"id={Name}, algoliaName={AlgoliaName}";
        }
    }
}