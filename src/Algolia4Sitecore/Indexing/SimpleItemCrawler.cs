namespace Algolia4Sitecore.Indexing
{
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    using Sitecore.Data.Items;
    using Sitecore.Links;
    using Sitecore.Sites;

    public class SimpleItemCrawler : BaseAlgoliaCrawler
    {
        public SimpleItemCrawler(IItemParser itemParser)
            : base(itemParser)
        {
        }

        public override JObject GetJsonForItem(Item item)
        {
            UrlOptions urlOptions = UrlOptions.DefaultOptions;
            urlOptions.AlwaysIncludeServerUrl = true;
            urlOptions.SiteResolving = true;
            urlOptions.Language = item.Language;
            urlOptions.Site = SiteContextFactory.GetSiteContext("website");

            JObject j = new JObject();
            j.Add("objectID", base.GetObjectId(item));
            j.Add("url", LinkManager.GetItemUrl(item, urlOptions));

            Dictionary<string, object> indexable = this.ItemParser.GetIndexable(item);
            foreach (var pair in indexable)
            {
                j.Add(pair.Key, JToken.FromObject(pair.Value));
            }

            return j;
        }
    }
}