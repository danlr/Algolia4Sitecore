namespace Algolia4Sitecore.Configuration
{
    using System;

    public class GeoLocationFieldDefinition : IndexedFieldDefinition
    {
        public GeoLocationFieldDefinition()
        {
            this.FieldType = FieldType.GeoLoc;
        }

        public Guid LatitudeId { get; set; }

        public Guid LongitudeId { get; set; }
    }
}