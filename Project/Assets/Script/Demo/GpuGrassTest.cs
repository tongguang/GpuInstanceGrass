using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GpuGrassTest : MonoBehaviour
{
    public GrassData GrassDataInfo;
    public GrassInstance GrassInstanceObj;

	void Start ()
    {
        GrassInstanceObj.SetMapGrassData(GrassDataInfo);
        GrassInstanceObj.SetGrassLevel(1);
    }
	
	// Update is called once per frame
	void Update () {
        GrassInstanceObj.SetRolePosition(transform.position.x, transform.position.z);
    }
}
