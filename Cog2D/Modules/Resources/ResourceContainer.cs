using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Resources
{
    public class ResourceContainer : IDisposable
    {
        private FileStream stream;

        private ResourceContainer()
        {
            throw new NotImplementedException();
            stream = new FileStream("", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }

        public void PreloadAll()
        {
            throw new NotImplementedException();
        }

        public void Preload(string file)
        {
            throw new NotImplementedException();
        }

        public IResource Load(string file)
        {
            throw new NotImplementedException();
        }
        
        public void Import(string internalFile, string externalFilename)
        {
            throw new NotImplementedException();
        }
        
        public void SetFileData(string file, byte[] data)
        {

        }

        /// <summary>
        /// Loads a Resource Container from a dictionary
        /// </summary>
        /// <param name="containerName">The name used to identify this resource container during runtime</param>
        /// <param name="dictionary">The path dictionary to load from</param>
        public static ResourceContainer LoadDictionary(string containerName, string dictionary)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads a Resource Container from a Cog2D Resource Container file (.crc)
        /// </summary>
        /// <param name="containerName">The name used to identify this resource container during runtime</param>
        /// <param name="filename">The path to the file to load</param>
        public static ResourceContainer LoadFile(string containerName, string filename)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
