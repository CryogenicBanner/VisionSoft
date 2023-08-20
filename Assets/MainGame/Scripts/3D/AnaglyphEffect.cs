using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    Michael's 3-D Anaglyph effect.
    Originally written 16.10.2018
*/

[ExecuteInEditMode]
public class AnaglyphEffect : MonoBehaviour {

    public Camera cam2;
    public float stereoWidth = 1.0f;
    public float eyeWidth = 0.2f; // Used for experimental purposes.


    private void Update()
    {
        // Adjust camera y-angles based on stereo width.
        transform.localEulerAngles = Vector3.up * -stereoWidth;
        cam2.transform.localEulerAngles = Vector3.up * stereoWidth;

        // Distance between eyes
        transform.localPosition = Vector3.right * eyeWidth;
        cam2.transform.localPosition = Vector3.right * -eyeWidth;

    }
}
