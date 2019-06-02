using UnityEditor;
using UnityEngine;

namespace GupInstanceGrass.Editor
{
    [CustomEditor(typeof(GrassBrush))]
    public class GrassBrushEditor : UnityEditor.Editor
    {
        SerializedProperty _GrassPrefab;
        SerializedProperty _GrassTexture;
        SerializedProperty _GrassColor;
        SerializedProperty _GrassMinScale;
        SerializedProperty _GrassMaxScale;
        SerializedProperty _BrushRadius;
        SerializedProperty _CreateGrassNum;
        SerializedProperty _TerrainLayerName;

        RaycastHit _RaycastHit;
        private bool _IsRayHit = false;

        void OnEnable()
        {
            _GrassPrefab = serializedObject.FindProperty("GrassPrefab");
            _GrassTexture = serializedObject.FindProperty("GrassTexture");
            _GrassColor = serializedObject.FindProperty("GrassColor");
            _GrassMinScale = serializedObject.FindProperty("GrassMinScale");
            _GrassMaxScale = serializedObject.FindProperty("GrassMaxScale");
            _BrushRadius = serializedObject.FindProperty("BrushRadius");
            _CreateGrassNum = serializedObject.FindProperty("CreateGrassNum");
            _TerrainLayerName = serializedObject.FindProperty("TerrainLayerName");
        }

        void OnSceneGUI()
        {
            Event e = Event.current;
            if (e == null)
            {
                return;
            }
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            var mono = target as GrassBrush;
            if (e.type == EventType.MouseDown && !e.alt && e.button == 0)
            {
                if (_GrassTexture.objectReferenceValue == null)
                {
                    Debug.LogError("请选择贴图");
                    return;
                }
                if (_GrassPrefab.objectReferenceValue != null && _IsRayHit)
                {
                    var go = new GameObject("grass_group");
                    go.transform.SetParent(mono.transform);
                    var props = new MaterialPropertyBlock();
                    props.SetColor("_Color", _GrassColor.colorValue);
                    props.SetTexture("_MainTex", _GrassTexture.objectReferenceValue as Texture);
                    if (_CreateGrassNum.intValue == 1)
                    {
                        var grass = PrefabUtility.InstantiatePrefab(_GrassPrefab.objectReferenceValue) as GameObject;
                        if (grass != null)
                        {
                            grass.transform.SetParent(go.transform);
                            grass.transform.localPosition = _RaycastHit.point;
                            grass.transform.localScale =
                                (_GrassMinScale.floatValue + (_GrassMaxScale.floatValue - _GrassMinScale.floatValue) * GetRandom()) * Vector3.one;
                            var render = grass.GetComponent<Renderer>();
                            var grassCache = grass.AddComponent<GrassEditorDataCache>();
                            grassCache.RendererObj = render;
                            grassCache.GrassColor = _GrassColor.colorValue;
                            grassCache.GrassTexture = _GrassTexture.objectReferenceValue as Texture;
                            render.SetPropertyBlock(props);
                        }
                    }
                    else
                    {
                        if (_GrassPrefab.objectReferenceValue != null)
                        {
                            int createNum = 0;
                            for (int i = 0; i < _CreateGrassNum.intValue * 10; i++)
                            {
                                float x, z;
                                GetRandomPos(_RaycastHit.point.x, _RaycastHit.point.z, _BrushRadius.floatValue, out x,
                                    out z);
                                var startPos = new Vector3(x, _RaycastHit.point.y + 500, z);
                                RaycastHit tempRaycastHit;
                                if (Physics.Raycast(startPos, Vector3.down, out tempRaycastHit, 1000f,
                                    1 << LayerMask.NameToLayer(_TerrainLayerName.stringValue)))
                                {
                                    if (Vector3.Distance(_RaycastHit.point, tempRaycastHit.point) > _BrushRadius.floatValue)
                                    {
                                        continue;
                                    }
                                    var pos = tempRaycastHit.point;
                                    var grass = PrefabUtility.InstantiatePrefab(_GrassPrefab.objectReferenceValue) as GameObject;
                                    if (grass != null)
                                    {
                                        grass.transform.SetParent(go.transform);
                                        grass.transform.localPosition = pos;
                                        grass.transform.localScale =
                                            (_GrassMinScale.floatValue + (_GrassMaxScale.floatValue - _GrassMinScale.floatValue) * GetRandom()) * Vector3.one;
                                        var render = grass.GetComponent<Renderer>();
                                        var grassCache = grass.AddComponent<GrassEditorDataCache>();
                                        grassCache.RendererObj = render;
                                        grassCache.GrassColor = _GrassColor.colorValue;
                                        grassCache.GrassTexture = _GrassTexture.objectReferenceValue as Texture;
                                        render.SetPropertyBlock(props);
                                        createNum += 1;
                                    }

                                    if (createNum >= _CreateGrassNum.intValue)
                                    {
                                        break;
                                    }
                                }

                            }
                        }
                    }
                }
            }

            if (e.type == EventType.MouseDrag && !e.alt && e.button == 0)
            {
            }

            _IsRayHit = false;
            Ray worldRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(worldRay, out _RaycastHit, 500f, 1 << LayerMask.NameToLayer(_TerrainLayerName.stringValue)))
            {
                _IsRayHit = true;
                Handles.color = new Color(1f, 1f, 1f, 0.5f);
                Handles.DrawSolidDisc(_RaycastHit.point, _RaycastHit.normal, _BrushRadius.floatValue);
                Handles.color = Color.red;
                Handles.DrawWireDisc(_RaycastHit.point, _RaycastHit.normal, _BrushRadius.floatValue);
                SceneView.RepaintAll();
            }
        }

        void GetRandomPos(float centerX, float centerY, float radius, out float x, out float y)
        {
            float u = Mathf.Sqrt(GetRandom()) * radius;
            float v = GetRandom() * 2 * Mathf.PI;

            x = centerX + u * Mathf.Cos(v);
            y = centerY + u * Mathf.Sin(v);
        }

        float GetRandom()
        {
            var x = UnityEngine.Random.value;
            return x;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Save"))
            {
                var mono = target as GrassBrush;
                mono.Save();
            }
        }
    }
}
