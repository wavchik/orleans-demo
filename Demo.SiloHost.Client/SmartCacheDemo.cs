using System;
using System.Linq;
using System.Threading.Tasks;
using Demo.SmartCache.GrainInterfaces;
using Demo.SmartCache.GrainInterfaces.State;
using Orleans;

namespace Patterns.SmartCache.Host
{
    internal class SmartCacheDemo
    {
        private readonly IGrainFactory _grainFactory;
        public SmartCacheDemo(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        private async Task CreateCatalogItem(Guid id, int index)
        {
            var grainId = id;
            var grainState =
                new CatalogItem
                {
                    DisplayName = $"Item {index}",
                    SKU = id.ToString(),
                    ShortDescription = $"This is the {index}th item"
                };

            var grain = _grainFactory.GetGrain<ICatalogItemGrain>(grainId);
            await grain.SetItem(grainState);
        }

        private Task InitializeItems()
        {
            var createTasks = Constants.ItemIds.Select(CreateCatalogItem);
            return Task.WhenAll(createTasks.ToArray());
        }

        private async Task ReadItems()
        {
            foreach (var itemId in Constants.ItemIds)
            {
                var grain = _grainFactory.GetGrain<ICatalogItemGrain>(itemId);
                var state = await grain.GetItem();
                Console.WriteLine(state);
            }
        }

        public void Run()
        {
            InitializeItems()
                .ContinueWith(_ => ReadItems())
                .ContinueWith(_ => {
                    Console.WriteLine("Orleans Silo is running.\nPress Enter to continue...");
                    Console.ReadLine();
                })
                .Wait();
        }
    }
}