using Algolia4Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace Algolia4Sitecore.Services
{
    public interface IIndexingService
    {
        void InitIndex(IndexConfiguration index, Language language, bool force = false);

        void UpdateItem(Item item);

        void DeleteItem(Item item);
    }
}