using System;
namespace Checkout.Domain.Common
{
	public class AuditableEntity
	{
		public DateTime CreatedDate { get; set; }
		public DateTime? UpdatedDate { get; set; }
	}
}

