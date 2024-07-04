# Another version of code that should help you to index Sitecore data into Algolia

This code shows different approach - simplify indexing process, updated Algolia packages, strongly typed data objects that incapsulate all calculations requred for them.

When an item is processed by the publishing pipeline our handler tryes to send it into a configured Algolia index, document model class gets the item and required dependencies and populates its fields. The documents are sent into Algolia indexes.

Important points:
- this is just a code sample, not a production solution! Any search solution must be carefully ajusted for a specific project needs. But once you understand this sample it will be easy for you to build a proper indexing for your project.
- there is no Helix here, it is up to you how to split code to features.
- this approach will work for solutions with small/medium amounts of items, when items are not changed and published frequeently. Batching is required for very dynamic content. Carefull deletion and regular full rebuild required if items are often deleted.
- there are options for multi-language content. Here '{itemId}_{itemLang}' algolia object ID is used to separate language version. Different indexes are required per language if you have a lot of data and marketers want to manage indexes per language/market/
- you do not want to manage index settings manually! Here we have 'PowershellHelper' class that will allow to set all indexes configuration as per config file with SPE.