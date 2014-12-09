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

        public T DynamicLoad<T>(string key, string container, string filename)
            where T : Resource
        {
            var alreadyLoaded = TryGetResource(key);
            var resourceContainer = Engine.ResourceHost.GetContainer(container);

            // If a resource of the same type is already loaded, verify it's the same
            if (alreadyLoaded != null)
            {
                if (alreadyLoaded.Container == resourceContainer &&
                alreadyLoaded.Filename == filename &&
                alreadyLoaded is T)
                    return (T)alreadyLoaded;
                else
                    throw new Exception("A resource with the same key is already loaded that's not identical!");
            }

            var resource = resourceContainer.Load(filename);
            if (!(resource is T))
                throw new Exception("Tried to load a " + resource.GetType().Name + " as a " + typeof(T).Name);
            resources.Add(key, resource);
            return (T)resource;
        }

        public Resource TryGetResource(string key)
        {
            Resource res;
            resources.TryGetValue(key, out res);
            return res;
        }

        public Texture GetTexture(string key)
        {
            return Get<Texture>(key);
        }

        public BitmapFont GetBitmapFont(string key)
        {
            return Get<BitmapFont>(key);
        }

        public SoundEffect GetSound(string key)
        {
            return Get<SoundEffect>(key);
        }

        public T Get<T>(string key)
            where T : Resource
        {
            Resource resource = TryGetResource(key);
            if (resource != null && !(resource is T))
                throw new Exception("Tried to get a " + resource.GetType().Name + " as a " + resource.GetType().Name);
            return (T)resource;
        }

        internal void AddResource(string key, Resource resource)
        {
            resources.Add(key, resource);
        }
    }
}
