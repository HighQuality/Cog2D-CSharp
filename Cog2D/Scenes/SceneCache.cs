using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Scenes
{
    static class SceneCache
    {
        private static Dictionary<Type, int> typeToId;
        private static List<Type> idToType;
        private static int nextSceneId;

        internal static void InitializeCache()
        {
            typeToId = new Dictionary<Type, int>();
            idToType = new List<Type>();
            // ID 0 is not used
            idToType.Add(null);
            nextSceneId = 1;
        }

        internal static void CreateCache(Type type)
        {
            Debug.Info("Scene ID {0} = {1}", nextSceneId, type.FullName);

            typeToId.Add(type, nextSceneId);
            idToType.Add(type);
            nextSceneId++;
        }

        internal static Scene CreateFromId(int id)
        {
            if (id < 1 || id >= idToType.Count)
                throw new ArgumentOutOfRangeException("Scene ID " + id.ToString() + " is not cached!");
            return (Scene)Activator.CreateInstance(idToType[(int)id]);
        }

        internal static int IdFromType(Type type)
        {
            int id;
            if (!typeToId.TryGetValue(type, out id))
                throw new Exception("Scene " + type.FullName + " is not cached!");
            return id;
        }
    }
}
