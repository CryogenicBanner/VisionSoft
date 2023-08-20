using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IllusionManager : MonoBehaviour
{
    // --------- Variables P�blicas

    [Header("Elementos para 3D")]
    [Tooltip("Holder que contiene las c�maras 3D")]
    public Transform m_StrabismCamera;

    [Tooltip("Prefab que usaremos como objetivo para el videojuego")]
    public GameObject m_IndicatorPrefab;

    [Tooltip("Prefab que usaremos como efecto de humo")]
    public GameObject humoPrefab;

    [Tooltip("GO que sirve para mostrar el objetivo que estamos seleccionando")]
    public Transform m_IndicatorSelectorPlane;
    public MenuController mc;

    [Header("Elementos de Configuraci�n")]
    [Tooltip("Indica qu� tanto se separan los objetivos al acertar el minijuego")]
    public int separationRatio;

    [Tooltip("�El movimiento de los ojos debe hacerse hacia afuera?")]
    public bool endotrophic = true;
    [Tooltip("Duraci�n de la Sesi�n")]
    public float sessionTime;

    [Tooltip("Camara de la ciudad")]
    public GameObject camaraCiudad;
    public Transform posCamaraCiudad;
    [Tooltip("Camara de la mesa de juego")]
    public GameObject camaraJuego;
    public Transform posCamaraJuego;


    // ---------- Variables Privadas

    //Lista de objetivos instanciados en escena
    private List<GameObject> m_IndicatorList = null;
    private List<GameObject> m_SmokeList = null;

    //Control de cantidad de separaci�n y m�ximo de separaci�n
    [SerializeField] private float maxSeparation = 16, currentSeparation = 0;
    //AllowUserInputs nos permite controlar si el usuario puede realizar interacciones (taps, button press o swipes) con el juego
    public bool gameRunning;

    private float currentSessionTime;
    private Points pointsManager;
    private Vector3 auxV3CenterOfCamera;

    void Start() {
        /*GameRestart();*/ 
        gameRunning = false;
    }

    public void GameRestart()
    {
        //Funcion para borrar los objetos creados antes de cada ronda
        BorrarObjetosInstanciados();

        //El juego a�n no debe iniciarse, por lo que lo detenemos en lo que instanciamos todo.
        pointsManager = GetComponent<Points>();
        gameRunning = false;
        currentSessionTime = sessionTime;
        if (ResetSceneCross())
        { 
            ResetCurrentSeparation();
        }

        gameRunning = true;
        ResetMainHUD();
        ModifyDificulty(true, RandomGenerator(m_IndicatorList));
        ActivarCamara(2);
        //AdjustCameraOnMenu(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameRunning)
        {
            ControllerInputs();

            currentSessionTime -= Time.deltaTime;
            //Debug.Log("CST = " + currentSessionTime);
            
            mc.UpdateSessionTimer(currentSessionTime);

            if(currentSessionTime < 0)
            {
                Debug.Log("timer -> "+currentSessionTime);
                currentSessionTime = 0;
                gameRunning = false;
                Debug.Log("Has terminado el juego! ");
                ActivarCamara(1);
                if(pointsManager != null)
                    mc.EndGameTimeAndShowPoints(pointsManager.GetFloatCurrentPoints());
            }
            
        }
    }
    private void ControllerInputs()
    {
        //Eligiendo el objetivo de arriba
        if (Input.GetKeyDown(KeyCode.UpArrow)) { CompareWithObjective(m_IndicatorList[0]); }
        //Eligiendo el objetivo de la derecha.
        if (Input.GetKeyDown(KeyCode.RightArrow)) { CompareWithObjective(m_IndicatorList[1]); }
        //Eligiendo el objetivo de abajo.
        if (Input.GetKeyDown(KeyCode.DownArrow)) { CompareWithObjective(m_IndicatorList[2]);}
        //"Eligiendo el objetivo de la izquierda.";
        if (Input.GetKeyDown(KeyCode.LeftArrow)){  CompareWithObjective(m_IndicatorList[3]); }
        
        //Si presionamos el 1, la dificultad baja
        if (Input.GetKeyDown(KeyCode.Alpha1)) { ModifyDificulty(false, RandomGenerator(m_IndicatorList)); }
        //Si presionamos el 2, las bolitas cambian de lugar
        if (Input.GetKeyDown(KeyCode.Alpha2)) { SeparateAndMisplace(RandomGenerator(m_IndicatorList)); }
        //Si presionamos el 3, la dificultad sube.
        if (Input.GetKeyDown(KeyCode.Alpha3)) { ModifyDificulty(true, RandomGenerator(m_IndicatorList)); }

    }

    private void CompareWithObjective(GameObject g)
    {
        if (gameRunning)
        {
            GenerateSmoke(0, 1.5f);
            GenerateSmoke(1.5f, 0);
            GenerateSmoke(0, -1.5f);
            GenerateSmoke(-1.5f, 0);
            if (g.GetComponent<Card>().getObjectiveStatus())
            {
                Debug.Log("Este es el indicado");
                ModifyDificulty(true, RandomGenerator(m_IndicatorList));
                mc.txt_combo.text = pointsManager.GetFloatCurrentCombo().ToString();
                mc.txt_points.text = pointsManager.GetFloatCurrentPoints().ToString();
            }
            else
            {
                Debug.Log("Este no es el indicado");
                ModifyDificulty(false, RandomGenerator(m_IndicatorList));
                mc.txt_combo.text = pointsManager.GetFloatCurrentCombo().ToString();
            }
        }
        
    }

    private void GenerateSmoke(float x, float y){
        GameObject g = Instantiate(humoPrefab, transform);
        g.transform.localPosition = auxV3CenterOfCamera + new Vector3(x, y, -1f);
        m_SmokeList.Add(g);
    }

    private void BorrarObjetosInstanciados(){
        if(m_IndicatorList != null){
            Debug.Log("Lista esferas -> "+m_IndicatorList.Count);
            int auxC = m_IndicatorList.Count;
            for(int i=0; i<auxC; i++){
                Destroy(m_IndicatorList[i]);
            }
        }
        if(m_SmokeList != null){
            Debug.Log("Lista humos -> "+m_SmokeList.Count);
            int auxD = m_SmokeList.Count;
            for(int i=0; i<auxD; i++){
                Destroy(m_SmokeList[i]);
            }
        }
    }

    //Funci�n para aparecer los indicators en forma de cruz

    private bool ResetSceneCross()
    {
        if (m_StrabismCamera == null || m_IndicatorPrefab == null)
        {
            Debug.LogError("No se han asignado las variables publicas del script IndicatorHolder");
            return false;
        }

        //Primero instanciamos nuestra lista de indicadores, dej�ndola lista para instanciarlos en X posici�n.
        m_IndicatorList = new List<GameObject>();
        m_SmokeList = new List<GameObject>();
        //Luego indicamos cu�nto espacio debemos dejar entre cada elemento a instanciar.
        float separation = 1.5f;

        //Ahora procedemos con los c�lculos para la distancia de los objetos que instanciaremos.
        //Primero obtenemos el tama�o de la c�mara (en Camera ortographic Size)
        float cameraSize = 0;
        Camera mainCamera = m_StrabismCamera.GetChild(0).GetComponent<Camera>();
        if (mainCamera != null)
        {
            cameraSize = mainCamera.orthographicSize;
            Debug.Log("Camera Size = " + cameraSize);
            //Agregamos un peque�o Offset para extender un poco el ancho, pues el uso de 2 c�maras 3D afecta los c�lculos iniciales
            cameraSize += 1.5f;
        }
        else { Debug.LogError("No se encontró correctamente una c�mara en el prefab de c�mara 3D"); return false; }

        //Ahora multiplicamos por 2 la cameraSize, y esto es el ancho de nuestra pantalla en unidades Unity.
        float horizontalRegion = 2 * (cameraSize);

        //Preparamos los datos para la divisi�n de nuestros objetos por regi�n
        float halfHorizontalRegion = horizontalRegion / 2;
        Vector3 Vec3_CenterOfCamera = new Vector3(halfHorizontalRegion - (cameraSize), 0, 0);

        auxV3CenterOfCamera = Vec3_CenterOfCamera;

        //Instanciamos primero los objetos, los acomodamos en su lugar y luego los agregamos a la lista
        InstantiateObjective(Vec3_CenterOfCamera, 0, separation);
        InstantiateObjective(Vec3_CenterOfCamera, separation, 0);
        InstantiateObjective(Vec3_CenterOfCamera, 0, -separation);
        InstantiateObjective(Vec3_CenterOfCamera, -separation, 0);
        //Terminamos de instanciar y posicionar los 4 objetos.

        //ubicamos el plano selector en uno de los 4 objetivos.
        m_IndicatorSelectorPlane.position = m_IndicatorList[1].transform.position;

        return true;
    }

    private void InstantiateObjective(Vector3 centercam, float x, float y)
    {
        GameObject g = Instantiate(m_IndicatorPrefab, transform);
        g.transform.localPosition = centercam + new Vector3(x, y);
        m_IndicatorList.Add(g);
    }

    private void ModifyDificulty(bool increase, int misplacedObstacle)
    {
        //Si debemos aumentar la dificultad...
        if (increase)
        {
            //Preguntamos si debemos aumentar la separación del filtro o disminuirlo
            if(endotrophic)
                currentSeparation = (currentSeparation+1 > maxSeparation) ? maxSeparation : currentSeparation+1;
            else 
                currentSeparation = (currentSeparation -1 < 0) ? 0 : currentSeparation - 1;
            
            pointsManager.ScorePoints(5);
            pointsManager.ModifyCombo(true);
        }
        //... en cambio, si queremos bajar la dificultad...
        else if (!increase)
        {
            if (endotrophic)
                currentSeparation = (currentSeparation - 1 < 0) ? 0 : currentSeparation - 1;
            else
                currentSeparation = (currentSeparation + 1 > maxSeparation) ? maxSeparation : currentSeparation + 1;

            pointsManager.ResetCombo();
            
        }


        SeparateAndMisplace(misplacedObstacle);
    }

    private void SeparateAndMisplace(int misplacedObstacle)
    {
        // Crear una serie de Vectores que contengan las posiciones futuras de los indicadores.
        List<Vector3> myVectors = new List<Vector3>();

        //El foreach recoje todas las posiciones y las settea a la separaci�n que deben tener por igual.
        //Debug.Log("Igualando indicadores");

        foreach (GameObject g in m_IndicatorList)
        {
            Vector3 savedPos = g.transform.localPosition;
            savedPos.z = currentSeparation;
            myVectors.Add(savedPos);
            g.GetComponent<Card>().setObjetiveStatus(false);
        }

        //Debug.Log("Separando Indicadores");
        int rand = misplacedObstacle;
        Vector3 misplacedVector = myVectors[rand];
        misplacedVector.z = currentSeparation + separationRatio;
        myVectors[rand] = misplacedVector;
        m_IndicatorList[rand].GetComponent<Card>().setObjetiveStatus(true);

        //Asignamos las posiciones calculadas de los indicadores
        for (int i = 0; i < m_IndicatorList.Count; i++)
        {
            m_IndicatorList[i].transform.localPosition = myVectors[i];
        }

        //Debug.Log("Posición " + i + ": " + myVectors[i]);
        Vector3 planeIndicator = m_IndicatorSelectorPlane.transform.localPosition;
        planeIndicator.z = currentSeparation + 2 * separationRatio;
        m_IndicatorSelectorPlane.transform.localPosition = planeIndicator;
    }
    private void ResetCurrentSeparation()
    {
        if (endotrophic) { currentSeparation = 0f; } 
        else { currentSeparation = maxSeparation; }
    }


    public void QuitApp() { Application.Quit(); }
    public void PauseGame() { Time.timeScale = 0; }
    public void ResumeGame() { Time.timeScale = 1; }


    // ----------------------------------------------------
    private int RandomGenerator(int bottom, int top) { return Random.Range(bottom, top + 1); }
    private int RandomGenerator(List<GameObject> myList) { return Random.Range(0, myList.Count); }
    // ----------------------------------------------------

    // funciones para activar la camara de juego y de ciudad
    // 1 -> camara de la ciudad
    // 2 -> camara de la mesa de juego
    public void ActivarCamara(int camara)
    {
        if (camara == 1)
        {
            camaraCiudad.SetActive(true);
            camaraJuego.SetActive(false);
        }
        else
        {
            camaraCiudad.SetActive(false);
            camaraJuego.SetActive(true);
        }
    }
    //Función que utiliza una misma cámara en vez de activar o desactivarlas.
    public void AdjustCameraOnMenu(bool b)
    {
        if (b)
        {
            camaraCiudad.GetComponent<Camera>().orthographic = false;
            camaraCiudad.transform.position = posCamaraCiudad.position;
            Camera.main.transform.rotation = posCamaraCiudad.rotation;
        }
        else
        {
            camaraCiudad.GetComponent<Camera>().orthographic = true;
            //Camera.main.orthographicSize = 40f;
            Camera.main.transform.position = posCamaraJuego.position;
            Camera.main.transform.rotation = posCamaraJuego.rotation;
        }
    }

    // Funciones para los botones del MainHUD Canvas
    public void BtnUp(){ CompareWithObjective(m_IndicatorList[0]); }
    public void BtnRight() { CompareWithObjective(m_IndicatorList[1]); }
    public void BtnDown(){ CompareWithObjective(m_IndicatorList[2]); }
    public void BtnLeft(){ CompareWithObjective(m_IndicatorList[3]); }

    private void ResetMainHUD()
    {
        mc.txt_HUD_Timer.text = sessionTime.ToString();
        mc.txt_points.text = "0";
        mc.txt_combo.text = "0";
    }

    public void StopGame(){
        gameRunning = false;
    }
}
