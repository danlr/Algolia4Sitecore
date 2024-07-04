using System.Collections.Generic;
using Sitecore.Diagnostics;

namespace Algolia4Sitecore.Configuration
{
    public class Configuration
    {
        public Dictionary<string, IndexConfiguration> Indexes { get; protected set; } = new Dictionary<string, IndexConfiguration>();

        public virtual void AddIndex(IndexConfiguration index)
        {
            Assert.ArgumentNotNull(index, nameof(index));
            Assert.IsFalse(this.Indexes.ContainsKey(index.Name) , "An index with the name \"{0}\" has already been added.", index.Name);

            this.Indexes[index.Name] = index;
        }
    }
}