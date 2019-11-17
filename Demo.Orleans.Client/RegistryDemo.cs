using System;
using System.Linq;
using System.Threading.Tasks;
using Demo.SmartCache.GrainInterfaces;
using Demo.SmartCache.GrainInterfaces.State;
using Orleans;
using Patterns.SmartCache.Host;

namespace Demo.Orleans.Client
{
    internal class RegistryDemo
    {
        private readonly IGrainFactory _grainFactory;
        private readonly Random _random = new Random();
        public RegistryDemo(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        private Func<Guid, int, Task> CreateAndRegisterCatalogItem(ICatalogItemRegistryGrain registryGrain)
            => async (id, index) =>
            {
                var grainId = id;
                var grainState =
                    new CatalogItem
                    {
                        DisplayName = $"Item {index}",
                        SKU = id.ToString(),
                        ShortDescription = $"This is the {index}th item",
                        Price = _random.NextDouble() * 10
                    };

                var grain = _grainFactory.GetGrain<ICatalogItemGrain>(grainId);
                await grain.SetItem(grainState);
                await registryGrain.RegisterGrain(grain);
            };

        private Task SetupCatalog()
        {
            var catalogRegistry = _grainFactory.GetGrain<ICatalogItemRegistryGrain>(Constants.RegistryId);
            var tasks = Constants.ItemIds.Select(CreateAndRegisterCatalogItem(catalogRegistry));

            var catalogRegistrySecond = _grainFactory.GetGrain<ICatalogItemRegistryGrain>(Constants.AnotherRegistryId);

            return Task.WhenAll(tasks.Union(Constants.SecondItemIds.Select(CreateAndRegisterCatalogItem(catalogRegistrySecond))).ToArray());
        }

        private async Task ReadCatalog()
        {
            var catalogRegistry = _grainFactory.GetGrain<ICatalogItemRegistryGrain>(Constants.RegistryId);
            var items = await catalogRegistry.GetRegisteredGrains();

            var foundItems =
                items
                    .Select(async item => await item.GetItem())
                    .Select(x => x.Result)
                    .Where(x => x.Price > 9.8)
                    .ToList();

            Console.WriteLine("Items in Registry that greater than 8");
            foundItems.ForEach(Console.WriteLine);

            Console.WriteLine("End Registry Demo");
        }

        public void Run()
        {
            SetupCatalog()
                .ContinueWith(_ => ReadCatalog())
                .Wait();
        }
    }
}