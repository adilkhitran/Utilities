using UnityEditor;
using UnityEngine;

namespace KHiTrAN
{
	[CustomEditor(typeof(PrefabLightmapData))]
	public class PrefabLightmapDataEditor : Editor
	{
		private bool ShowList;

		public override void OnInspectorGUI()
		{
			var _target = (PrefabLightmapData)target;
			if (GUILayout.Button("Load\nLightmapInfo", GUILayout.Height(45)))
			{
				PrefabLightmapData.LoadLightmapInfo();
			}

			if (!(_target.m_RendererInfo == null || _target.m_RendererInfo.Length <= 0))
			{
				if (GUILayout.Button("Apply\nLightmapInfo", GUILayout.Height(45)))
				{
					_target.ApplyLightData();
				}
			}

			ShowList = EditorGUILayout.Foldout(ShowList, "Renders");
			if (ShowList)
			{
				if (_target.m_RendererInfo.Length <= 0)
				{
					EditorGUILayout.HelpBox("List is Empty.", MessageType.Info);
				}
				else
				{
					for (int i = 0; i < _target.m_RendererInfo.Length; i++)
					{
						var info = _target.m_RendererInfo[i];
						EditorGUILayout.ObjectField("Render" + i, info.renderer, typeof(Renderer));
					}
				}
			}
		}
	}
}