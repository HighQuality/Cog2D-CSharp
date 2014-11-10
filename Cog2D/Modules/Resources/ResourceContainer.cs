using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Cog.Modules.Resources
{
    public abstract class ResourceContainer : IDisposable
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        internal List<Resource> LoadedResources = new List<Resource>();

        internal ResourceContainer(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }

        public abstract void PreloadAll();
        public abstract void Preload(string file);
        public abstract Resource Load(string file);
        public abstract byte[] ReadData(string file);

        public void Import(string file, string externalFile)
        {
            Import(file, File.ReadAllBytes(externalFile));
        }
        public abstract void Import(string file, byte[] data);

        public void Update(string file, string externalFile)
        {
            Update(file, File.ReadAllBytes(externalFile));
        }
        public abstract void Update(string file, byte[] data);

        /// <summary>
        /// Loads a Resource Container from a dictionary
        /// </summary>
        /// <param name="containerName">The name used to identify this resource container during runtime</param>
        /// <param name="dictionary">The path dictionary to load from</param>
        public static ResourceContainer LoadDictionary(string containerName, string dictionary)
        {
            throw new NotImplementedException();
        }
        
        public abstract void Dispose();
    }
}
