using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cog.Modules.Resources
{
    public class ResourceManager
    {
        private Dictionary<string, ResourceContainer> loadedContainers = new Dictionary<string, ResourceContainer>();
        private Dictionary<Type, ResourceCollection> collections = new Dictionary<Type, ResourceCollection>();

        internal ResourceManager()
        {
        }

        public ResourceContainer GetContainer(string name)
        {
            return loadedContainers[name.ToLower()];
        }

        public ResourceContainer Load(string name, string filename)
        {
            name = name.ToLower();
            var container = SQLiteContainer.LoadFile(name, filename);
            Debug.Info("Loaded Resource Container {0}@{1}!", name, filename);

            loadedContainers.Add(name, container);
            return container;
        }

        public void LoadDictionary(string name, string dictionary)
        {
            throw new NotImplementedException();
        }

        internal ResourceCollection GetResourceCollection(Type type)
        {
            ResourceCollection c;
            if (!collections.TryGetValue(type, out c))
            {
                c = new ResourceCollection();

                foreach (var attribute in type.GetCustomAttributes<ResourceAttribute>())
                {
                    var container = GetContainer(attribute.ContainerName);
                    c.AddResource(attribute.Key, container.Load(attribute.Filename));
                }

                collections.Add(type, c);
            }
            return c;
        }
    }
}
