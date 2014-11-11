using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Scenes
{
    static class SceneCache
    {
        private static Dictionary<Type, ushort> typeToId;
        private static List<Type> idToType;
        private static ushort nextSceneId;

        internal static void InitializeCache()
        {
            typeToId = new Dictionary<Type, ushort>();
            idToType = new List<Type>();
            nextSceneId = 1;
        }

        internal static void CreateCache(Type type)
        {
            Debug.Info("Scene ID {0} = {1}", nextSceneId, type.FullName);

            typeToId.Add(type, nextSceneId);
            idToType.Add(type);
            nextSceneId++;
        }
    }
}
