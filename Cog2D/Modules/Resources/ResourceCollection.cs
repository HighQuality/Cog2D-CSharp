using Cog.Modules.Audio;
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

        public BitmapFont GetBitmapFont(string key)
        {
            return resources[key] as BitmapFont;
        }

        public SoundEffect GetSound(string key)
        {
            return resources[key] as SoundEffect;
        }

        public T Get<T>(string key)
            where T : class
        {
            return resources[key] as T;
        }

        internal void AddResource(string key, Resource resource)
        {
            resources.Add(key, resource);
        }
    }
}
