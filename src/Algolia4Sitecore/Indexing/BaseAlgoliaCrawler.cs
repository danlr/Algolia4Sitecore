namespace Algolia4Sitecore.Indexing
{
    using System;
    using Newtonsoft.Json.Linq;
    using Sitecore.Data;
    using Sitecore.Data.Items;

    public abstract class BaseAlgoliaCrawler : IAlgoliaCrawler
    {
        protected BaseAlgoliaCrawler(IItemParser itemParser)
        {
            this.ItemParser = itemParser;
        }

        protected IItemParser ItemParser { get; set; }

        public abstract JObject GetJsonForItem(Item item);

        public string GetObjectId(Item item)
        {
            return this.GetObjectId(item.ID);
        }

        public string GetObjectId(ID id)
        {
            return this.GetObjectId(id.Guid);
        }

        public string GetObjectId(Guid id)
        {
            return id.ToString("D");
        }
    }
}