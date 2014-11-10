using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Resources
{
    public class ResourceCollection
    {
        private Dictionary<string, Resource> resources = new Dictionary<string, Resource>();

        internal ResourceCollection()
        {
        }

        public Texture GetTexture(string key)
        {
            return resources[key] as Texture;
        }

        internal void AddResource(string key, Resource resource)
        {
            resources.Add(key, resource);
        }
    }
}
