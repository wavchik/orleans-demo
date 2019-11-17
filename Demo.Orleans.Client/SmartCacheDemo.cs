using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.SmartCache.GrainInterfaces;
using Demo.SmartCache.GrainInterfaces.State;
using Orleans;
using Patterns.SmartCache.Host;

namespace Demo.Orleans.Client
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
                    ShortDescription = $"This is the {index}th item",
                    Price = index
                };

            var grain = _grainFactory.GetGrain<ICatalogItemGrain>(grainId);
            await grain.SetItem(grainState);
        }

        private Task InitializeItems()
        {
            int i = 0;
            List<Task> createTasks = Constants.ItemIds.Select(item => CreateCatalogItem(item, i++)).ToList();

            return Task.WhenAll(createTasks.ToArray());
        }

        private async Task ReadItems()
        {
            foreach (var itemId in Constants.ItemIds)
            {
                var grain = _grainFactory.GetGrain<ICatalogItemGrain>(itemId);
                var state = await grain.GetItem();
            }

            Console.WriteLine("End Cache Demo");
        }

        public void Run()
        {
            Task.
                WhenAll(InitializeItems())
                .ContinueWith(_ => ReadItems())
                .Wait();
        }
    }
}