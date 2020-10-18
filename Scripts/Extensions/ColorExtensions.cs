// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/HierarchyHeader               --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using UnityEngine;

namespace  instance.id.HierarchyHeader
{
    public static class ColorExtensions
    {
        public static Color GetColor(this string color)
        {
            ColorUtility.TryParseHtmlString(color, out var outColor);
            return outColor;
        }
    }
}
