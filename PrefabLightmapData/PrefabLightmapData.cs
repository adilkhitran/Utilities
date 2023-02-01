using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace KHiTrAN
{

	public class PrefabLightmapData : MonoBehaviour
	{
		[System.Serializable]
		public struct RendererInfo
		{
			public Renderer renderer;

			public int lightmapIndex;
			public Vector4 lightmapOffsetScale;
		}

		public RendererInfo[] m_RendererInfo;
		private Texture2D[] m_Lightmaps;


		private void OnValidate()
		{
			if (m_RendererInfo == null)
				m_RendererInfo = new RendererInfo[0];

		}

		void Awake()
		{
			ApplyLightData();
		}

		public void ApplyLightData()
		{
			if (m_RendererInfo == null || m_RendererInfo.Length == 0)
				return;

			var data = LightmapSettings.lightmaps;
			m_Lightmaps = new Texture2D[data.Length];
			for (int i = 0; i < data.Length; i++)
			{
				m_Lightmaps[i] = data[i].lightmapColor;
			}
			ApplyRendererInfo(m_RendererInfo, 0);
		}

		static void ApplyRendererInfo(RendererInfo[] infos, int lightmapOffsetIndex)
		{
			for (int i = 0; i < infos.Length; i++)
			{
				var info = infos[i];
				info.renderer.lightmapIndex = info.lightmapIndex + lightmapOffsetIndex;
				info.renderer.lightmapScaleOffset = info.lightmapOffsetScale;
			}
		}

#if UNITY_EDITOR
		[UnityEditor.MenuItem("Assets/Bake Prefab Lightmaps")]
		public static void GenerateLightmapInfo()
		{
			if (UnityEditor.Lightmapping.giWorkflowMode != UnityEditor.Lightmapping.GIWorkflowMode.OnDemand)
			{
				Debug.LogError("ExtractLightmapData requires that you have baked you lightmaps and Auto mode is disabled.");
				return;
			}
			UnityEditor.Lightmapping.Bake();

			PrefabLightmapData[] prefabs = FindObjectsOfType<PrefabLightmapData>();

			foreach (var instance in prefabs)
			{
				var gameObject = instance.gameObject;
				var rendererInfos = new List<RendererInfo>();
				var lightmaps = new List<Texture2D>();

				instance.m_RendererInfo = GenerateLightmapInfo(gameObject, rendererInfos, out lightmaps).ToArray();

				instance.m_Lightmaps = lightmaps.ToArray();

				var targetPrefab = UnityEditor.PrefabUtility.GetPrefabParent(gameObject) as GameObject;
				if (targetPrefab != null)
				{
					//UnityEditor.Prefab
					UnityEditor.PrefabUtility.ReplacePrefab(gameObject, targetPrefab);
				}
			}
		}

		public static void LoadLightmapInfo()
		{

			PrefabLightmapData[] prefabs = FindObjectsOfType<PrefabLightmapData>();

			foreach (var instance in prefabs)
			{
				var gameObject = instance.gameObject;
				var rendererInfos = new List<RendererInfo>();
				var lightmaps = new List<Texture2D>();

				instance.m_RendererInfo = GenerateLightmapInfo(gameObject, rendererInfos, out lightmaps).ToArray();

				instance.m_Lightmaps = lightmaps.ToArray();
			}
		}

		static List<RendererInfo> GenerateLightmapInfo(GameObject root, List<RendererInfo> rendererInfos, out List<Texture2D> lightmaps)
		{
			lightmaps = new List<Texture2D>();
			var renderers = root.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer renderer in renderers)
			{
				if (renderer.lightmapIndex != -1)
				{
					RendererInfo info = new RendererInfo();
					info.renderer = renderer;
					info.lightmapOffsetScale = renderer.lightmapScaleOffset;

					Texture2D lightmap = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapColor;

					info.lightmapIndex = lightmaps.IndexOf(lightmap);
					if (info.lightmapIndex == -1)
					{
						info.lightmapIndex = lightmaps.Count;
						lightmaps.Add(lightmap);
					}

					rendererInfos.Add(info);
				}
			}
			return rendererInfos;
		}
#endif

	}
}