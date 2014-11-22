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
        public abstract byte[] ReadData(string file);

        public Resource Load(string file)
        {
            var data = ReadData(file);
            var extension = System.IO.Path.GetExtension(file).ToLower();
            Resource resource = null;
            string resourceType;

            if (extension == ".png" || extension == ".bmp")
            {
                resource = Engine.Renderer.LoadTexture(data);
                resourceType = "Texture";
            }
            else if (extension == ".wav" || extension == ".ogg" || extension == ".flac")
            {
                resource = Engine.Audio.Load(data);
                resourceType = "Sound";
            }
            else
                throw new NotImplementedException("Resource Type \"" + extension + "\" not implemented!");

            Console.WriteLine("Resource {0} ({1}) in container {2} loaded!", file, resourceType, Name);

            if (resource != null)
            {
                resource.Container = this;
                LoadedResources.Add(resource);
            }

            return resource;
        }

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
                
        public abstract void Dispose();
    }
}
