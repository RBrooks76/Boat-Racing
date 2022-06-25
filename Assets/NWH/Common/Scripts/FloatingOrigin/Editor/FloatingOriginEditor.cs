#if UNITY_EDITOR
using NWH.Common;
#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace NWH.Common.FloatingOrigin
{
	#if UNITY_EDITOR
	[CustomEditor(typeof(FloatingOrigin))]
	public class FloatingOriginEditor : NUIEditor {
		public override bool OnInspectorNUI() {
			if (!base.OnInspectorNUI())
			{
				return false;
			}
			
			drawer.Field("distanceThreshold");
			drawer.Field("OnBeforeJump");
			drawer.Field("OnAfterJump");
		
			drawer.EndEditor(this);
			return true;
		}
	}
	#endif
}



#endif
