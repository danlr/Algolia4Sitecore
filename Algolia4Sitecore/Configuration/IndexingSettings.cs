namespace Algolia4Sitecore.Configuration
{
    using System;
    using System.Collections.Generic;

    public class IndexingSettings
    {
        public IndexingSettings()
        {
            this.IncludedFields = new List<IndexedFieldDefinition>();
            this.ContentFields = new List<Guid>();
            this.ExcludedTemplates = new List<Guid>();
        }

        public List<IndexedFieldDefinition> IncludedFields { get; set; }

        public List<Guid> ContentFields { get; set; }

        public List<Guid> ExcludedTemplates { get; set; }

        public void AddIncludedField(System.Xml.XmlNode node)
        {
            string guidString = Sitecore.Xml.XmlUtil.GetValue(node);
            if (Guid.TryParse(guidString, out var guid))
            {
                IndexedFieldDefinition field = new IndexedFieldDefinition();
                field.Id = guid;
                field.FieldName = node.Attributes["fieldName"]?.Value;

                if (node.Attributes["indexed"] != null)
                {
                    field.Indexed = node.Attributes["indexed"].Value.Equals("true", StringComparison.OrdinalIgnoreCase);
                }

                if (node.Attributes["type"] != null)
                {
                    Enum.TryParse(node.Attributes["type"].Value, true, out FieldType fieldType);

                    field.FieldType = fieldType;
                }
                else
                {
                    field.FieldType = FieldType.Text;
                }

                if (field.FieldType == FieldType.GeoLoc &&
                    node.Attributes["latitude"] != null &&
                    node.Attributes["longitude"] != null)
                {
                    Guid.TryParse(node.Attributes["latitude"].Value, out var latGuid);
                    Guid.TryParse(node.Attributes["longitude"].Value, out var lonGuid);

                    var geoField = new GeoLocationFieldDefinition()
                    {
                        Id = field.Id,
                        FieldName = field.FieldName,
                        LatitudeId = latGuid,
                        LongitudeId = lonGuid
                    };

                    field = geoField;
                }

                this.IncludedFields.Add(field);
            }
        }

        public void AddContentField(System.Xml.XmlNode node)
        {
            string guidString = Sitecore.Xml.XmlUtil.GetValue(node);

            if (Guid.TryParse(guidString, out var guid))
            {
                this.ContentFields.Add(guid);
            }
        }

        public void AddExcludedTemplate(System.Xml.XmlNode node)
        {
            string guidString = Sitecore.Xml.XmlUtil.GetValue(node);

            if (Guid.TryParse(guidString, out var guid))
            {
                this.ExcludedTemplates.Add(guid);
            }
        }
    }
}