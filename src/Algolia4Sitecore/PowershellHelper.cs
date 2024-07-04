using Algolia4Sitecore.Services;

namespace Algolia4Sitecore
{
    public class PowershellHelper
    {
        public static void InitIndexes()
        {
            var indexingService = new IndexingService(null, null);

            indexingService.InitAllIndexes(true);
        }
    }
}