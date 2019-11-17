using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Demo.SiloHost.Client.Extensions
{
    public static class ApplicationPartManagerExtension
    {
        public static void IgnoreOrleansDashboard(this ApplicationPartManager t)
        {
            const string appPartName = "OrleansDashboard";
            var appPart = t.ApplicationParts.SingleOrDefault(i => i.Name == appPartName);
            if (appPart == null)
            {
                return;
            }

            t.ApplicationParts.Remove(appPart);
        }
    }
}
