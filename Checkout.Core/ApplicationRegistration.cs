using System;
using System.Reflection;
using Checkout.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CodingTest.Core
{
	public static class ApplicationRegistration
	{
		public static IServiceCollection AddApplicationService(this IServiceCollection services)
		{
			services.AddScoped<ICheckoutService, CheckoutService>();
			return services;
		}
	}
}

