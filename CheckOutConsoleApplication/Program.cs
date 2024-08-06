using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Checkout.Core;
using Checkout.RuleEngine;
using Checkout.RuleEngine.Entities;
using Checkout.RuleEngine.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Checkout.Core.Contracts.Persistences;
using Checkout.Persistence;
using CodingTest.Core;

namespace CheckOutConsoleApplication;

class Program
{
    static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddApplicationService();
        builder.Services.AddPersistenceService();
        builder.Services.AddRuleEnginePersistenceService();

        using IHost host = builder.Build();

        // ------------------------------------------------
        // Application code should start here.
        await RunAsync(host.Services, "Scope 1");


        // ------------------------------------------------
        await host.RunAsync();
    }

    static async Task RunAsync(IServiceProvider services, string scope)
    {
        Console.WriteLine($"{scope}...");

        using IServiceScope serviceScope = services.CreateScope();
        IServiceProvider provider = serviceScope.ServiceProvider;

        var ruleLoader = provider.GetRequiredService<IRuleLoader>();

        var root = await ruleLoader.LoadAsync("block-tree.json");
        printTree(root.First().Condition);

        if (root == null)
        {
            Console.WriteLine("No rule");
        }

        var checkoutService = provider.GetRequiredService<ICheckoutService>();
        checkoutService.Rules = root;

        checkoutService.Scan("A");
        checkoutService.Scan("B");
        checkoutService.Scan("C");
        checkoutService.Scan("D");

        checkoutService.Scan("A");
        checkoutService.Scan("A");

        var total = checkoutService.Total();
        Console.WriteLine($"Total = {total}");
    }

    static void printTree(BlockBase root, int level = 0)
    {
        for (int i = 0; i < level; i++)
        {
            Console.Write("\t");
        }
        Console.WriteLine($"{root.GetType().Name}");
        if (root is BlockNode)
        {
            foreach (var item in (root as BlockNode)?.Children)
            {
                printTree(item, level + 1);
            }
        }
    }
}