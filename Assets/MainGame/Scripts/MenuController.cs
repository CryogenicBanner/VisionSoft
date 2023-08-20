using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject HUD;
    public TMP_Text txt_HUD_Timer;
    public TMP_Text txt_combo;
    public TMP_Text txt_points;
    public TMP_Text txt_Results_Title;
    public TMP_Text txt_Results_Text;
    public GameObject Results;

    public GameObject WinnerMenu;
    public GameObject restartMenu;

    public Transform CameraMenuPos;


    public void showEndMenu()
    {
        Debug.Log("fin de la partida");

        //canvasEnd = GameObject.Find("CanvasEnd").GetComponent<Canvas>();
        //Camera.main.transform.LookAt(canvasEnd.transform);
        //canvasEnd.gameObject.SetActive(true);
        //canvasEnd.enabled = true;
    }

    public void UpdateSessionTimer(float time)
    {
        double newText = System.Math.Round(time, 1);
        if (newText % 1 == 0) txt_HUD_Timer.text = newText.ToString() + ".0";
        else txt_HUD_Timer.text = newText.ToString();
    }

    public void ActivateHUD(bool b)
    {
        if (b)
        {
            mainMenu.SetActive(false);
            HUD.SetActive(true);
        }
    }

    public void EndGame()
    {
        Debug.Log("Valio maaaaaaaaaaaaaais");
        FillResultsMenu("Defeat!");
        Results.SetActive(true);
        HUD.SetActive(false);
    }

    public void EndGameTime()
    {
        Debug.Log("Se acabo el tiempo");
        FillResultsMenu("Victory!");
        Results.SetActive(true);
        HUD.SetActive(false);
    }

    public void EndGameTimeAndShowPoints(float results)
    {
        Debug.Log("Se acabó el tiempo");
        FillResultsMenu("Victory", results);
        Results.SetActive(true);
        HUD.SetActive(false);

    }

    // Function to fill the results menu
    public void FillResultsMenu(string title)
    {
        txt_Results_Title.text = title;
    }

    public void FillResultsMenu(string title, float results)
    {
        txt_Results_Title.text = title;
        txt_Results_Text.text = "Puntuación: " + results;
    }
}
