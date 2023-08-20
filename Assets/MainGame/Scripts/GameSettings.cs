using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class GameSettings : MonoBehaviour
{
    [Header("Ship")]
    public GameObject shipPrefab;
    public Transform shipStartingPos;
    public Transform CameraMenuPos;

    [Tooltip("Defines the border size of all elements in scene")]
    [Range(5, 100)]
    public int borderSize;

    [Header("Meteorites")]
    public List<GameObject> meteoritePrefab;
    public float maxDistanceFromShip;

    [Header("Stereoscopic Colors")]
    public Color leftEye, RightEye;

    [Header("Difficulty")]
    [Tooltip("Duration of the session")]
    public float sessionTime;
    [Tooltip("Should we allow mixing colors in the asteroids?")]
    public bool mixedColors;
    [Tooltip("How much time between respawning of asteroids")]
    public float timeForRespawn;
    [Tooltip("Quantity of respawning asteroids each timeForRespawn")]
    public int respawnXQuantity;
    [Header("UI")]
    public MenuController mc;

    //[Header("Sounds")]
    private StudioEventEmitter backgroundMusic;
    private bool missionIsActive;            //Indica si existe una misión en curso,
    private float currentRespawnTime;
    private float currentSessionTime;

    private GameObject instancedShip;
    private Destructible shipDestructibleData;

    private List<GameObject> currentAliveObjectives;
    private bool endGame;            //Marca cuando se te acaban las vidas (Derrota)
    private bool endOfSessionTime;   //Marca que CurrentSessionTime ha terminado (Victoria)
    private bool showedResults;


    // Start is called before the first frame update
    void Start()
    {
        backgroundMusic = GetComponent<StudioEventEmitter>();
        currentAliveObjectives = new List<GameObject>();
        MenuRestart();
        Sound_Restart();
        backgroundMusic.Play();
        showedResults = false;
    }

    private void Sound_Restart()
    {
        backgroundMusic.SetParameter("MissionIsActive", 0f);
        backgroundMusic.SetParameter("EndGame", 0f);
        backgroundMusic.SetParameter("EndOfSessionTime", 0f);
    }

    private void Sound_ReceiveParameters(bool missionIsActive, bool endGame, bool endOfSessionTime)
    {
        backgroundMusic.SetParameter("MissionIsActive", missionIsActive ? 1f : 0f) ;
        backgroundMusic.SetParameter("EndGame", endGame ? 1f : 0f);
        backgroundMusic.SetParameter("EndOfSessionTime", endOfSessionTime ? 1f : 0f);
    }

    public void MenuRestart()
    {
        CheckShipInstance();

        instancedShip.GetComponent<Ship>().SetInDemoMode(true);
        shipDestructibleData.ResetLife();
        DeleteAllCurrentObjectives();
        missionIsActive = false;
        endGame = false;
        endOfSessionTime = false;
        AdjustMenuCamera(true);
        Sound_Restart();

    }

    // Checks if the ship is already instanced
    private void CheckShipInstance()
    {
        if (shipPrefab != null && shipStartingPos != null && borderSize != 0)
        {
            if (instancedShip != null)
            {
                instancedShip.transform.rotation = shipStartingPos.rotation;
                instancedShip.transform.position = shipStartingPos.position;
            }
            else
            {
                instancedShip = Instantiate(shipPrefab, shipStartingPos.position, shipStartingPos.rotation);
                shipDestructibleData = instancedShip.GetComponent<Destructible>();
            }
        }
        else { Debug.LogWarning("Alguno de los elementos en GameSettings no está declarado"); }
    }

    private void DeleteAllCurrentObjectives()
    {
        foreach (GameObject g in currentAliveObjectives)
        {
            Destroy(g);
        }
    }

    public void StartGame()
    {
        DeleteAllCurrentObjectives();
        CheckShipInstance();
        currentRespawnTime = currentSessionTime = 0;
        shipDestructibleData.ResetLife();
        AdjustMenuCamera(false);
        missionIsActive = true;
        endGame = false;
        endOfSessionTime = false;
        showedResults = false;
        instancedShip.GetComponent<Ship>().SetInDemoMode(false);
        ResumeGame();
        Sound_ReceiveParameters(missionIsActive, endGame, endOfSessionTime);
    }

    private void Update()
    {
        //Si la misión no está activa, significa que ganamos o perdimos
        if (!missionIsActive && !showedResults)
        {
            if (shipDestructibleData.GetCurrentLife() <= 0 && !endGame)
            {
                Debug.Log("Defeat");

                DeleteAllCurrentObjectives();
                endGame = true;
                endOfSessionTime = false;
                mc.EndGame();
                showedResults = true;
                
            }
            else if (endOfSessionTime && !endGame)
            {
                Debug.Log("Victory!");
                
                DeleteAllCurrentObjectives();
                endGame = false;
                endOfSessionTime = true;
                mc.EndGameTime();
                showedResults = true;
            }
            Sound_ReceiveParameters(missionIsActive, endGame, endOfSessionTime);
        }
        else if (missionIsActive) {
            // INICIO respawnear objetivos
            currentRespawnTime += Time.deltaTime;
            if (currentRespawnTime >= timeForRespawn && instancedShip != null)
            {
                for (int i = 0; i < respawnXQuantity; i++)
                {
                    SpawnTarget();
                }
                currentRespawnTime = 0;
            }
            // FIN respawnear objetivos

            currentSessionTime += Time.deltaTime;
            if (currentSessionTime >= sessionTime || instancedShip == null)
            {
                currentSessionTime = sessionTime;
                missionIsActive = false;
                endOfSessionTime = true;

            }
            mc.UpdateSessionTimer(sessionTime - currentSessionTime);
            Sound_ReceiveParameters(missionIsActive, endGame, endOfSessionTime);
        }
    }

    private void SpawnTarget()
    {
        //Instanciamos el meteorito
        Vector3 randomizedPosition = GetRandomizedPosAroundPoint(shipStartingPos.position, maxDistanceFromShip);
        int randIndex = Random.Range(0, meteoritePrefab.Count);
        GameObject newMeteorite = Instantiate(meteoritePrefab[randIndex], shipStartingPos.position + randomizedPosition, shipStartingPos.rotation);
        //Modificamos los colores de instanciación del meteorito
        Meteorite meteorite = newMeteorite.GetComponent<Meteorite>();
        if (meteorite != null)
        {
            bool colorToFill = (Random.value < 0.5);
            Meteorite meteorScript = newMeteorite.GetComponent<Meteorite>();
            meteorScript.ChangeOutlineColor(colorToFill ? leftEye : RightEye);

            if (mixedColors) { meteorScript.ChangeArrowOutlineColor(colorToFill ? RightEye : leftEye); }
            else { meteorScript.ChangeArrowOutlineColor(colorToFill ? leftEye : RightEye); }
        }
        currentAliveObjectives.Add(newMeteorite);
        //Destructible meteorite_d = newMeteorite.GetComponent<Destructible>();
        //meteorite_d.ResetLife();
    }

    private Vector3 GetRandomizedPosAroundPoint(Vector3 pos, float maxDistance)
    {
        return pos + (RandomPointOnCircleEdge(maxDistance));
    }

    private Vector3 RandomPointOnCircleEdge(float radius)
    {
        var vector2 = Random.insideUnitCircle.normalized * radius;
        return new Vector3(vector2.x, 0, vector2.y);
    }


    public void QuitApp() { Application.Quit(); }
    public void PauseGame() { Time.timeScale = 0; }
    public void ResumeGame() { Time.timeScale = 1; }

    public void AdjustMenuCamera(bool b)
    {
        if (b)
        {
            Camera.main.orthographic = false;
            Camera.main.transform.position = CameraMenuPos.position;
            Camera.main.transform.rotation = CameraMenuPos.rotation;
        }
        else
        {
            Camera.main.orthographic = true;
            Camera.main.orthographicSize = 40f;
            Camera.main.transform.position = shipStartingPos.position + (Vector3.up * borderSize);
            Camera.main.transform.LookAt(shipStartingPos);
        }
    }
}
