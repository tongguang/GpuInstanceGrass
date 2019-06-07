using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GupInstanceGrass.Demo
{
    public class GpuGrassTest : MonoBehaviour
    {
        public GrassData GrassDataInfo;
        public GrassInstance GrassInstanceObj;

        private Transform _Transform;

        void Start()
        {
            _Transform = transform;

            GrassInstanceObj.SetGrassLevel(2);
            GrassInstanceObj.SetMapGrassData(GrassDataInfo);
        }

        // Update is called once per frame
        void Update()
        {
            GrassInstanceObj.SetRolePosition(_Transform.position.x, _Transform.position.y, _Transform.position.z);
        }
    }
}

