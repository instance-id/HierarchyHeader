// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/HierarchyHeader               --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using UnityEngine;

namespace instance.id.HierarchyHeader
{
    public static class GameObjectExtensions
    {
        public static bool HasComponent<T>(this Component component) where T : Component
        {
            return component.GetComponent<T>() != null;
        }

        public static bool HasComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() != null;
        }
    }
}
