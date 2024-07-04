using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Algolia4Sitecore.Models
{
    public abstract class ItemRecord
    {
        public string ObjectID { get; set; }

        public string Keywords { get; set; }

        protected string GetTaxonomyFromField(ReferenceField field)
        {
            return field?.TargetItem?["title"];
        }

        protected IEnumerable<string> GetTaxonomiesFromField(MultilistField field)
        {
            var taxonomyItems = field?.GetItems();

            if (taxonomyItems != null && taxonomyItems.Length > 0)
            {
                return taxonomyItems.Select(t => t["title"]);
            }

            return Enumerable.Empty<string>();
        }

        public static string GetObjectId(Item item)
        {
            return $"{item.ID.Guid:N}_{item.Language.Name}";
        }
    }
}