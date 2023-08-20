using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    //Objeto GameManager
    private GameObject gameManager;

    // Start is called before the first frame update
    void Start()
    {
        //Busco el objeto llamado GameManager
        GameObject gameManager = GameObject.Find("GameManager");

        //Le indico que no se destruya al cargar otra escena 
        DontDestroyOnLoad(gameManager);

        //Cargo la escena de inicio
        SceneManager.LoadScene("pruebaMenu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }

}
