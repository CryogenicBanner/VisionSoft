using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeerMouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("posX -> " + Input.mousePosition.x + " / ## / " + "posY -> " + Input.mousePosition.y);
    }
}
