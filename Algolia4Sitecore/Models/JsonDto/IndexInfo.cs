namespace Algolia4Sitecore.Models.JsonDto
{
    using System;

    using Newtonsoft.Json;

    [JsonObject]
    public class IndexInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("entries")]
        public int Entries { get; set; }

        [JsonProperty("dataSize")]
        public int DataSize { get; set; }

        [JsonProperty("fileSize")]
        public int FileSize { get; set; }

        [JsonProperty("lastBuildTimeS")]
        public int LastBuildTimeS { get; set; }

        [JsonProperty("numberOfPendingTasks")]
        public int NumberOfPendingTasks { get; set; }

        [JsonProperty("pendingTask")]
        public bool PendingTask { get; set; }
    }
}