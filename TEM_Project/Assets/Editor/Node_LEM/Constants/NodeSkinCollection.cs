using UnityEngine;
using System;
namespace LEM_Editor
{

    [Serializable]
    public struct NodeSkinCollection
    {
        public Texture2D m_TopBackground;
        public Texture2D m_SelectedTopOutline;

        //public Texture2D textureToRender;
        public Texture2D m_MidBackground;
        public Texture2D m_SelectedMidOutline;

        //Dark theme coming soon
        //public Texture2D dark_normal;
        //public Texture2D dark_selected;
    }


}