using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using Cog.Extensions;

namespace Cog.Modules.Resources
{
    class DictionaryContainer : ResourceContainer
    {
        public DictionaryContainer(string name, string path)
            : base(name, path)
        {
        }

        public override byte[] ReadData(string file)
        {
            return File.ReadAllBytes(System.IO.Path.Combine(Path, file));
        }

        private void UpdateData(string file, byte[] data)
        {
            File.WriteAllBytes(System.IO.Path.Combine(Path, file), data);
        }
        
        public override void Preload(string file)
        {
            throw new NotImplementedException();
        }

        public override void PreloadAll()
        {
            throw new NotImplementedException();
        }

        public override void Import(string file, byte[] data)
        {
            File.WriteAllBytes(System.IO.Path.Combine(Path, file), data);
        }

        public override void Update(string file, byte[] data)
        {
            File.WriteAllBytes(System.IO.Path.Combine(Path, file), data);
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        ~DictionaryContainer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Loads a Resource Container from a Cog2D Resource Container file (.crc)
        /// </summary>
        /// <param name="containerName">The name used to identify this resource container during runtime</param>
        /// <param name="filename">The path to the file to load</param>
        internal static ResourceContainer LoadDictionary(string containerName, string dictionary)
        {
            var container = new DictionaryContainer(containerName, dictionary);
            return container;
        }
    }
}
