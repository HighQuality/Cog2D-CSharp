using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
            typeToId.Add(type, nextSceneId);
            idToType.Add(type);
            nextSceneId++;
        }

        internal static Scene CreateFromId(ushort typeId, long id)
        {
            if (typeId < 1 || typeId >= idToType.Count)
                throw new ArgumentOutOfRangeException("Scene ID " + id.ToString() + " is not cached!");

            var type = idToType[(int)typeId];
            var scene = (Scene)FormatterServices.GetUninitializedObject(type);
            Engine.AssignId(scene, id);
            type.GetConstructor(new Type[0]).Invoke(scene, new object[0]);
            return scene;
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
