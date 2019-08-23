namespace Algolia4Sitecore.Indexing
{
    using System.Collections.Generic;

    using Sitecore.Data.Items;

    public interface IItemParser
    {
        Dictionary<string, object> GetIndexable(Item item);
    }
}