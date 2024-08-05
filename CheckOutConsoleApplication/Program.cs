using Checkout.RuleEngine;
using Checkout.RuleEngine.Entities;
using Checkout.RuleEngine.Persistence;

namespace CheckOutConsoleApplication;

class Program
{
    static async Task Main(string[] args)
    {
        IRuleLoader ruleLoader = new RuleJsonFileLoader();

        var root = await ruleLoader.LoadAsync("block-tree.json");
        printTree(root.First().Condition);


        Console.WriteLine("Done");
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
