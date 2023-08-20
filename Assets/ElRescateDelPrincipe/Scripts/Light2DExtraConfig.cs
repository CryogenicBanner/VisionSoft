using System;
using System.Collections.Generic;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.Experimental.SceneManagement;
#endif

namespace UnityEngine.Experimental.Rendering.Universal
{
    public class Light2DExtraConfig : MonoBehaviour
    {
        public Light2D light2D;

        public bool pingPongIntensity;
        public float speedPingPongIntensity;
        public float minPingPongIntensity;
        public float maxPingPongIntensity;



        // Start is called before the first frame update
        void Start()
        {
            light2D = this.GetComponent<Light2D>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (light2D != null)
            {
                if (pingPongIntensity)
                {
                    light2D.intensity = minPingPongIntensity + Mathf.PingPong(Time.time * speedPingPongIntensity, maxPingPongIntensity);
                }
            }
        }
    }
}


