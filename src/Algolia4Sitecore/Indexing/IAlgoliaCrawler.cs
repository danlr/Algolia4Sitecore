namespace Algolia4Sitecore.Indexing
{
    using System;

    using Newtonsoft.Json.Linq;

    using Sitecore.Data;
    using Sitecore.Data.Items;

    public interface IAlgoliaCrawler
    {
        JObject GetJsonForItem(Item item);

        string GetObjectId(Item item);

        string GetObjectId(ID id);

        string GetObjectId(Guid id);
    }
}