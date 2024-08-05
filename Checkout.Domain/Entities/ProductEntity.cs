using System;
using Checkout.Domain.Common;

namespace Checkout.Domain.Entities
{
  public class ProductEntity : AuditableEntity
  {
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public string SKU { get; set; }
    public decimal UnitPrice { get; set; }
  }
}

