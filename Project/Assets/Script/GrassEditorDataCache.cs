using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GrassEditorDataCache : MonoBehaviour
{
    public Renderer RendererObj;

    public Color GrassColor = Color.white;
    public Texture GrassTexture = null;

    private void Start()
    {
        if (RendererObj)
        {
            var props = new MaterialPropertyBlock();
            props.SetColor("_Color", GrassColor);
            props.SetTexture("_MainTex", GrassTexture);
            RendererObj.SetPropertyBlock(props);
        }
    }

    private void OnEnable()
    {
        if (RendererObj)
        {
            var props = new MaterialPropertyBlock();
            props.SetColor("_Color", GrassColor);
            props.SetTexture("_MainTex", GrassTexture);
            RendererObj.SetPropertyBlock(props);
        }
    }
}

