using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnUI_Script_Mago_v01_26_03_22 : MonoBehaviour
{

    public Button btnUp, btnDown, btnLeft, btnRight, btnAux;

    // Start is called before the first frame update
    void Start()
    {
        if (btnUp)
        {

            btnUp.GetComponent<Button>().onClick.AddListener(() => Debug.Log("Se presiono la tecla Up"));
        }
        if (btnDown)
        {
            btnDown.GetComponent<Button>().onClick.AddListener(() => Debug.Log("Se presiono la tecla Down"));
        }
        if (btnLeft)
        {
            btnLeft.GetComponent<Button>().onClick.AddListener(() => Debug.Log("Se presiono la tecla Left"));
        }
        if (btnRight)
        {
            btnRight.GetComponent<Button>().onClick.AddListener(() => Debug.Log("Se presiono la tecla Right"));
        }
        if (btnAux)
        {
            btnAux.GetComponent<Button>().onClick.AddListener(() => Debug.Log("Se presiono la tecla Aux"));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
