using System;

namespace Demo.SmartCache.GrainInterfaces.State
{
    [Serializable]
    public class CatalogItem
    {
        public string DisplayName { get; set; }
        public string SKU { get; set; }
        public string ShortDescription { get; set; }

        public double Price { get; set; }

        public override string ToString()
        {
            return $@"[SKU : {SKU}] [DisplayName : {DisplayName}] [Price: {Price}] [ShortDescription: {ShortDescription}]";
        }
    }
}