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
    class SQLiteContainer : ResourceContainer
    {
        private SQLiteConnection database;
        public int Revision { get { return BitConverter.ToInt32(ReadMetaData("Revision"), 0); } private set { UpdateMetaData("Revision", BitConverter.GetBytes((Int32)value)); } }

        public SQLiteContainer(string name, string path)
            : base(name, path)
        {
            if (!File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);
                database = new SQLiteConnection(string.Format("Data Source={0};Version=3;", path));
                database.Open();

                using (var cmd = new SQLiteCommand(database))
                {
                    cmd.CommandText = "CREATE TABLE Files (File TEXT PRIMARY KEY NOT NULL UNIQUE, Data BLOB NOT NULL)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE TABLE MetaData (Key TEXT PRIMARY KEY UNIQUE NOT NULL, Data BLOB)";
                    cmd.ExecuteNonQuery();
                }

                InsertMetaData("Version", BitConverter.GetBytes((Int32)1));
                InsertMetaData("Revision", BitConverter.GetBytes((Int32)1));
            }
            else
            {
                database = new SQLiteConnection(string.Format("Data Source={0};Version=3;", path));
                database.Open();

                var version = BitConverter.ToInt32(ReadMetaData("Version"), 0);
                if (version < 1 || version > 1)
                    throw new Exception(string.Format("SQLiteContainer version {0} not supported!", version));

                Debug.Event("Loaded resource container \"{0}\" (Container Version {1})", Name, version);
            }
        }

        private byte[] ReadMetaData(string key)
        {
            using (var cmd = new SQLiteCommand(database))
            {
                cmd.CommandText = "SELECT Data FROM MetaData WHERE Key = @Key";
                cmd.Parameters.AddWithValue("@Key", key);
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                        return (byte[])rdr[0];
                    throw new Exception(string.Format("MetaData key {0} not found!", key));
                }
            }
        }

        private void InsertMetaData(string key, byte[] data)
        {
            using (var cmd = new SQLiteCommand(database))
            {
                cmd.CommandText = "INSERT INTO MetaData(Key, Data) VALUES(@Key, @Data)";
                cmd.Parameters.AddWithValue("@Key", key);
                cmd.Parameters.AddWithValue("@Data", data);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateMetaData(string key, byte[] data)
        {
            using (var cmd = new SQLiteCommand(database))
            {
                cmd.CommandText = "INSERT INTO MetaData(Key, Data) VALUES(@Key, @Data)";
                cmd.Parameters.AddWithValue("@Key", key);
                cmd.Parameters.AddWithValue("@Data", data);
                cmd.ExecuteNonQuery();
            }
        }

        public override byte[] ReadData(string file)
        {
            using (var cmd = new SQLiteCommand(database))
            {
                cmd.CommandText = "SELECT Data FROM Files WHERE File = @File LIMIT 1";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@File", file);

                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                        return (byte[])rdr[0];
                }
            }
            
            throw new Exception(string.Format("File {0} in container {1} was not found!", file, Name));
        }

        private void InsertData(string file, byte[] data)
        {
            using (var cmd = new SQLiteCommand(database))
            {
                cmd.CommandText = "INSERT INTO Data(File, Data) VALUES(@File, @Data)";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@File", file);
                cmd.Parameters.AddWithValue("@Data", data);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateData(string file, byte[] data)
        {

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
            using (var cmd = new SQLiteCommand(database))
            {
                cmd.CommandText = "INSERT INTO Files(File, Data) VALUES(@File, @Data)";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@File", file);
                cmd.Parameters.AddWithValue("@Data", data);
                cmd.ExecuteNonQuery();
            }
        }

        public override void Update(string file, byte[] data)
        {
            using (var cmd = new SQLiteCommand(database))
            {
                cmd.CommandText = "UPDATE Files SET Data = @Data, WHERE File = @File";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@File", file);
                cmd.Parameters.AddWithValue("@Data", data);
                cmd.ExecuteNonQuery();
            }
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
                database.Close();
                database.Dispose();
            }
        }

        ~SQLiteContainer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Loads a Resource Container from a Cog2D Resource Container file (.crc)
        /// </summary>
        /// <param name="containerName">The name used to identify this resource container during runtime</param>
        /// <param name="filename">The path to the file to load</param>
        internal static ResourceContainer LoadFile(string containerName, string filename)
        {
            var container = new SQLiteContainer(containerName, filename);
            return container;
        }
    }
}
