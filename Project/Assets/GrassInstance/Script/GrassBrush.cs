using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace GupInstanceGrass
{
    [ExecuteInEditMode]
    public class GrassBrush : MonoBehaviour
    {
        public GameObject GrassPrefab;

        public Texture GrassTexture = null;

        public Color GrassColor = Color.white;

        public float GrassMinScale = 1.0f;

        public float GrassMaxScale = 1.0f;

        public float BrushRadius = 1.0f;

        public int CreateGrassNum = 1;

        public string TerrainLayerName = "Terrain";

        public Vector2Int MapSize = new Vector2Int(100, 100);

        //    public Vector2Int BlockSize = new Vector2Int(30, 30);

        public string GrassDataDir = "Assets/3-GpuInstanceGrass/GrassData";

        public string GrassDataFileName = "grass_data";


        private void Awake()
        {
            //        GrassPrefab =
            //            AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_RawData/map_editor/map_models/grass/grass.prefab");
        }

        public void Save()
        {
            int xMax = Mathf.CeilToInt((1.0f * MapSize.x / GrassInstance.BlockSize.x));
            int yMax = Mathf.CeilToInt((1.0f * MapSize.y / GrassInstance.BlockSize.y));
            GrassData grassData = ScriptableObject.CreateInstance<GrassData>();
            for (int i = 0; i < xMax * yMax; i++)
            {
                grassData.BlockList.Add(new GrassData.GrassBlock());
            }

            grassData.BlockXMax = xMax;
            grassData.BlockYMax = yMax;
            var grassRoot = transform;
            var msehFilters = grassRoot.GetComponentsInChildren<MeshFilter>();
            HashSet<Texture2D> textures = new HashSet<Texture2D>();
            foreach (var msehFilter in msehFilters)
            {
                var blockX = Mathf.FloorToInt(1.0f * msehFilter.transform.position.x / GrassInstance.BlockSize.x);
                var blockY = Mathf.FloorToInt(1.0f * msehFilter.transform.position.z / GrassInstance.BlockSize.y);
                var index = blockY * xMax + blockX;
                var grassItem = new GrassData.GrassItem();
                grassData.BlockList[index].GrassItems.Add(grassItem);
                grassItem.TransformMatrix = msehFilter.transform.localToWorldMatrix;
                grassItem.UV0Offset = new Vector4(1f, 1f, 0f, 0f);
                foreach (var uvInfo in msehFilter.sharedMesh.uv)
                {
                    grassItem.UV0Offset.x = Mathf.Min(grassItem.UV0Offset.x, uvInfo.x);
                    grassItem.UV0Offset.y = Mathf.Min(grassItem.UV0Offset.y, uvInfo.y);
                    grassItem.UV0Offset.z = Mathf.Max(grassItem.UV0Offset.z, uvInfo.x);
                    grassItem.UV0Offset.w = Mathf.Max(grassItem.UV0Offset.w, uvInfo.y);
                }
                var grassEditorDataCache = msehFilter.GetComponent<GrassEditorDataCache>();
                grassItem.ColorInfo = grassEditorDataCache.GrassColor;
                var texture = grassEditorDataCache.GrassTexture;
                var assetName = texture.name;
                grassItem.TextureName = assetName;
                textures.Add(texture as Texture2D);
            }
            grassData.GrassTextures = textures.ToList();
            if (!Directory.Exists(GrassDataDir))
            {
                Directory.CreateDirectory(GrassDataDir);
                AssetDatabase.Refresh();
            }
            AssetDatabase.CreateAsset(grassData, GrassDataDir + "/" + GrassDataFileName + ".asset");
        }
    }


}
