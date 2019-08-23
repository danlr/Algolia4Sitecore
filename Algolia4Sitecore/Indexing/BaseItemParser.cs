namespace Algolia4Sitecore.Indexing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Configuration;

    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Links;
    using Sitecore.Resources.Media;

    using FieldType = Configuration.FieldType;

    public class BaseItemParser : IItemParser
    {
        public Dictionary<string, object> GetIndexable(Item item)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (item == null)
            {
                return result;
            }

            result = this.GetIndexableFieldsData(item, true);

            ItemLink[] itemLinks = Globals.LinkDatabase.GetReferences(item);

            foreach (ItemLink itemLink in itemLinks)
            {
                Item relatedItem = itemLink.GetTargetItem();

                if (relatedItem != null && relatedItem.Paths.IsContentItem)
                {
                    Dictionary<string, object> fields = this.GetIndexableFieldsData(relatedItem, false);

                    foreach (var field in fields)
                    {
                        if (result.ContainsKey(field.Key))
                        {
                            if (field.Value is string)
                            {
                                result[field.Key] += Environment.NewLine + field.Value;
                            }
                        }
                        else
                        {
                            result.Add(field.Key, field.Value);
                        }
                    }
                }
            }

            return result;
        }

        protected virtual Dictionary<string, object> GetIndexableFieldsData(Item item, bool includeSystemFields)
        {
            var fields = new Dictionary<string, object>();
            if (includeSystemFields)
            {
                fields.Add("name", item.Name);
                fields.Add("id", item.ID.Guid);
                fields.Add("path", item.Paths.FullPath);
            }

            foreach (var includedField in Settings.IndexingSettings.IncludedFields)
            {
                ID fieldId = ID.Parse(includedField.Id);

                if (item.Fields[fieldId] != null && !string.IsNullOrWhiteSpace(item.Fields[fieldId].Value))
                {
                    var indexFieldName = includedField.FieldName ?? item.Fields[fieldId].Name.ToLower();

                    switch (includedField.FieldType)
                    {
                        case FieldType.Facet:
                            this.ExtractFacetValues(item, fieldId, fields, indexFieldName);
                            break;
                        case FieldType.Media:
                            this.ExtractMediaValues(item, fieldId, fields, indexFieldName);
                            break;
                        case FieldType.Text:
                            string value = item[fieldId];
                            fields.Add(indexFieldName, value);
                            break;
                        case FieldType.Boolean:
                            string boolValue = item[fieldId];
                            fields.Add(indexFieldName, boolValue == "1");
                            break;
                        case FieldType.GeoLoc:
                            this.ExtractGeolocationValue(item, includedField, fields, indexFieldName);
                            break;
                    }
                }
            }

            this.ExtractContentField(item, ref fields);

            return fields;
        }

        private void ExtractGeolocationValue(Item item, IndexedFieldDefinition fieldDefinition, Dictionary<string, object> fields, string indexFieldName)
        {
            if (!(fieldDefinition is GeoLocationFieldDefinition geoLocationField))
            {
                return;
            }

            float.TryParse(item.Fields[ID.Parse(geoLocationField.LatitudeId)].Value, out var latitude);
            float.TryParse(item.Fields[ID.Parse(geoLocationField.LongitudeId)].Value, out var longitude);

            fields.Add(indexFieldName, new { lat = latitude, lon = longitude });
        }

        private void ExtractMediaValues(Item item, ID fieldId, Dictionary<string, object> fields, string indexFieldName)
        {
            Field field = item.Fields[fieldId];

            if (field.TypeKey == "image")
            {
                ImageField imageField = field;

                if (imageField.MediaItem != null)
                {
                    string path = MediaManager.GetMediaUrl(imageField.MediaItem);
                    fields.Add(indexFieldName, path);
                }
            }
            else
            {
                var ids = ID.ParseArray(field.Value, true);
                if (ids.Length <= 0)
                {
                    return;
                }

                List<string> urlList = new List<string>();

                foreach (ID id in ids)
                {
                    Item referencedItem = item.Database.GetItem(id);
                    if (referencedItem != null && referencedItem.Paths.IsMediaItem)
                    {
                        MediaItem mediaItem = new MediaItem(referencedItem);
                        urlList.Add(MediaManager.GetMediaUrl(mediaItem));
                    }
                }

                fields.Add(indexFieldName, urlList);
            }
        }

        private void ExtractFacetValues(Item item, ID fieldId, Dictionary<string, object> fields, string indexFieldName)
        {
            var ids = ID.ParseArray(item.Fields[fieldId].Value, true);
            if (ids.Length <= 0)
            {
                return;
            }

            var idsList = ids.Select(i => i.Guid.ToString("D")).ToArray();
            fields.Add(indexFieldName + "_ids", idsList);

            List<string> keysList = new List<string>();

            foreach (ID id in ids)
            {
                Item referencedItem = item.Database.GetItem(id);

                if (referencedItem != null)
                {
                    keysList.Add(referencedItem.Name);
                }
            }

            fields.Add(indexFieldName, keysList);
        }

        private void ExtractContentField(Item item, ref Dictionary<string, object> fields)
        {
            string content = string.Empty;

            foreach (Guid contentField in Settings.IndexingSettings.ContentFields)
            {
                ID fieldId = ID.Parse(contentField);
                if (item.Fields[fieldId] != null)
                {
                    content += item.Fields[fieldId].Value + Environment.NewLine;
                }
            }

            fields.Add("content", content);
        }
    }
}