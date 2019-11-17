using System;
using System.Linq;
using System.Threading.Tasks;
using Demo.SmartCache.GrainInterfaces;
using Demo.SmartCache.GrainInterfaces.State;
using Orleans;

namespace Patterns.SmartCache.Host
{
    internal class RegistryDemo
    {
        private readonly IGrainFactory _grainFactory;
        public RegistryDemo(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        // Partial Application FTW!
        private Func<Guid, int, Task> CreateAndRegisterCatalogItem(ICatalogItemRegistryGrain registryGrain)
            => async (id, index) =>
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
                await registryGrain.RegisterGrain(grain);
            };

        private Task SetupCatalog()
        {
            var catalogRegistry = _grainFactory.GetGrain<ICatalogItemRegistryGrain>(Constants.RegistryId);
            var tasks = Constants.ItemIds.Select(CreateAndRegisterCatalogItem(catalogRegistry));

            return Task.WhenAll(tasks.ToArray());
        }

        private async Task ReadCatalog()
        {
            var catalogRegistry = _grainFactory.GetGrain<ICatalogItemRegistryGrain>(Constants.RegistryId);
            var items = await catalogRegistry.GetRegisteredGrains();

            foreach (var item in items)
            {
                
                var state = await item.GetItem();
                Console.WriteLine(state);
            }
        }

        public void Run()
        {
            SetupCatalog()
                .ContinueWith(_ => ReadCatalog())
                .ContinueWith(_ => {
                    Console.WriteLine("Orleans Silo is running.\nPress Enter to continue...");
                    Console.ReadLine();
                })
                .Wait();
        }
    }
}