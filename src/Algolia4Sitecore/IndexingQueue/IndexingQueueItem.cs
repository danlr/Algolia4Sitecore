namespace Algolia4Sitecore.IndexingQueue
{
    using System;

    using Sitecore.Data.Items;

    public struct IndexingQueueItem
    {
        public readonly Guid Id;
        public readonly string Language;
        public DateTime DateTime;
        public bool Deleted;

        public IndexingQueueItem(Item item, bool deleteFromIndex = false)
        {
            this.Id = item.ID.Guid;
            this.Language = item.Language.Name;
            this.DateTime = DateTime.UtcNow;
            this.Deleted = deleteFromIndex;
        }

        public static IndexingQueueItem Empty { get; } = default(IndexingQueueItem);

        public bool Equals(IndexingQueueItem other)
        {
            return this.Language != null && (this.Id.Equals(other.Id) && string.Equals(this.Language, other.Language));
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return obj is IndexingQueueItem item && this.Equals(item);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Id.GetHashCode() * 397) ^ (this.Language != null ? this.Language.GetHashCode() : 0);
            }
        }
    }
}