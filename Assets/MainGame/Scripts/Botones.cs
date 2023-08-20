using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Botones : MonoBehaviour
{

    private GameManager gameManager;
    public GameObject objeto;

    public Button btnJuegoMago, btnEscena1, btnEscena2, btnMenu, btnCrearObjeto;
    

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        if( btnJuegoMago)
        {
            btnJuegoMago.GetComponent<Button>().onClick.AddListener(() => gameManager.ChangeScene(""));
        }

        if(btnEscena1)
        {
            btnEscena1.GetComponent<Button>().onClick.AddListener(() => gameManager.ChangeScene("Prueba1"));
        }

        if(btnEscena2)
        {
            btnEscena2.GetComponent<Button>().onClick.AddListener(() => gameManager.ChangeScene("Prueba2"));
        }

        if( btnMenu)
        {
            btnMenu.GetComponent<Button>().onClick.AddListener(() => gameManager.ChangeScene("pruebaMenu"));
        }

        if (btnCrearObjeto)
        {
            btnCrearObjeto.GetComponent<Button>().onClick.AddListener(() => Instantiate(objeto, new Vector3(0, 0, 0), Quaternion.identity));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
