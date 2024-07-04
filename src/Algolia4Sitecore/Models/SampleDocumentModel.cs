using System.Collections.Generic;
using System.Linq;
using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Sites;

namespace Algolia4Sitecore.Models
{
    public class SampleDocumentModel : ItemRecord
    {
        public static ID TemplateId = new ID("d627d43c-f764-4447-8951-08c27200210d");

        public SampleDocumentModel(Item item, BaseLinkManager linkManager, BaseMediaManager mediaManager)
        {
            if (item == null)
            {
                return;
            }

            ObjectID = GetObjectId(item);

            Title = item["title"];
            Description = item["shortDescription"];
            Keywords = item["MetaKeywords"];
            Languages = GetTaxonomiesFromField(item.Fields["languages"]).ToList();
            Regions = GetTaxonomiesFromField(item.Fields["regions"]).ToList();
            Type = GetTaxonomyFromField(item.Fields["Type"]);
            Url = linkManager.GetItemUrl(item);
            Date = item[Sitecore.FieldIDs.Updated];

            var image = ((ImageField)item.Fields["image"])?.MediaItem;
            if (image != null)
            {
                Image = mediaManager.GetMediaUrl(image);
            }
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public List<string> Languages { get; set; } = new List<string>();
        public List<string> Regions { get; set; } = new List<string>();
        public string Date { get; set; }
    }
}