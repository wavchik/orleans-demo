using System;
using System.Collections.Generic;
using System.Linq;

namespace Patterns.SmartCache.Host
{
    internal static class Constants
    {
        public static readonly Guid RegistryId = Guid.Parse("{d8c4eea3-ddd4-4d55-9287-a3201a698ae6}");
        public static readonly Guid AnotherRegistryId = Guid.Parse("{d77f552a-7b4f-48fd-9f0e-ef4394cee3d2}");

        public static readonly IList<Guid> ItemIds =
            Enumerable.Range(1, 100).ToList().Select(x => Guid.NewGuid()).ToList();

        public static readonly IList<Guid> SecondItemIds =
            Enumerable.Range(1, 100).ToList().Select(x => Guid.NewGuid()).ToList();
    }
}