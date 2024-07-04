using Sitecore.Data.Managers;
using Sitecore.Data;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;

namespace Algolia4Sitecore
{
    public static class ItemExtensions
    {
        public static bool HasBaseTemplate(this Item item, ID templateId)
        {
            return HasBaseTemplates(item, new ID[] { templateId }, false);
        }

        public static bool HasBaseTemplates(this Item item, IEnumerable<ID> templateIds, bool requireAll = false)
        {
            if (item == null)
            {
                return false;
            }

            var set = item.GetBaseTemplatesIds();

            return requireAll
                ? !templateIds.Except(set).Any()
                : set.Any(_ => templateIds.Any(i => i.Equals(_)));
        }

        public static IEnumerable<ID> GetBaseTemplatesIds(this Item item)
        {
            var allTemplates = new List<ID>();
            if (item == null)
            {
                return allTemplates;
            }

            var t = TemplateManager.IsTemplate(item) ? TemplateManager.GetTemplate(item.ID, item.Database) : TemplateManager.GetTemplate(item);
            var templates = t?.GetBaseTemplates()?.Where(x => x != null).Select(x => x.ID).Distinct().ToList();
            var baseTemplates = t?.GetBaseTemplates()?.Where(x => x != null).SelectMany(x => x.BaseIDs).Distinct();
            if (templates != null)
            {
                allTemplates.AddRange(templates);
            }

            if (baseTemplates != null)
            {
                allTemplates.AddRange(baseTemplates);
            }

            allTemplates.Add(item.TemplateID);
            if (t != null && TemplateManager.IsTemplate(item))
            {
                allTemplates.Add(t.ID);
            }
            return allTemplates;
        }
    }
}