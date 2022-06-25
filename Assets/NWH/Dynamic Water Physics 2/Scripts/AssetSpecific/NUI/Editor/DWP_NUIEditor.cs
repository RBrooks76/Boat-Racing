#if UNITY_EDITOR
using NWH.NUI;

namespace NWH.DWP2.NUI
{
    /// <summary>
    ///     NWH Vehicle Physics specific NUI Editor.
    /// </summary>
    public class DWP_NUIEditor : NUIEditor
    {
        public override string GetDocumentationBaseURL()
        {
            return "http://dynamicwaterphysics.com/doku.php/";
        }
    }
}

#endif
