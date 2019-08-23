namespace Algolia4Sitecore.Configuration
{
    using System;

    public class IndexedFieldDefinition
    {
        public Guid Id { get; set; }

        public string FieldName { get; set; }

        public bool Indexed { get; set; }

        public FieldType FieldType { get; set; }
    }
}