using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsMago : MonoBehaviour
{
    public MenuController mc;

    [Header("Difficulty")]
    [Tooltip("Duration of the session")]
    public float sessionTime;

    // Variable para el tiempo actual de partida
    private float currentSessionTime;

    // Para controlar cuando la sesion de juego ha iniciado
    private bool gameInProgress;


    // Start is called before the first frame update
    void Start()
    {
        gameInProgress = false;
    }

    // Update is called once per frame
    void Update()
    {
        if( gameInProgress)
        {
            //mc.UpdateSessionTimer(sessionTime - currentSessionTime);
        }
        
    }

    public void StartGame()
    {
        Debug.Log("Game start");
        gameInProgress = true;
    }
}
