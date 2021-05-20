// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/HierarchyHeader               --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using System;
using UnityEngine;

namespace instance.id.HierarchyHeader
{
    [Serializable]
    public class HierarchyHeaderSettingsData
    {
        [SerializeField] private string m_Name;
        public string name => m_Name;

        [SerializeField] public string headerPrefix = "---";
        [SerializeField] public string characterStrip = "-";
        [SerializeField] public int FontSize = 12;
        [SerializeField] public FontStyle FontStyle = FontStyle.Bold;
        [SerializeField] public TextAnchor Alignment = TextAnchor.MiddleCenter;
        [SerializeField] public Color TextColor = new Color(0.94f, 0.94f, 0.94f);
        [SerializeField] public Color BackgroundColor = new Color(0.14f, 0.14f, 0.14f);

        
        public HierarchyHeaderSettingsData()
        {
            m_Name = "Default";
        }
    }
}
