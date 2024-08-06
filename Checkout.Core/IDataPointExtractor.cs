using System.Collections.Generic;
using System.Linq;
using Checkout.Domain;
using Checkout.RuleEngine.Entities;

namespace Checkout.Core;

public interface IDataPointExtractor
{
    dynamic Extract(DataPointDefinition def);
}

public class DataPointCartItemExtractor : IDataPointExtractor
{
    public IEnumerable<CartItem> Items { get; }

    public DataPointCartItemExtractor(IEnumerable<CartItem> items)
    {
        Items = items;
    }

    public dynamic Extract(DataPointDefinition def)
    {
        var skuParam = def.Params.Where(p => p.Key == "SKU")
            .FirstOrDefault();
        var sku = skuParam.Value;

        var item = Items.FirstOrDefault(t => t.SKU == sku);
        if (item == null) return null;
        return item;
    }
}