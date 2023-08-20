using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorHolder : MonoBehaviour
{
    public Transform m_StrabismCamera;
    public GameObject m_IndicatorPrefab;
    public Transform m_IndicatorSelectorPlane;

    public int separationRatio;

    [Range(2, 5)]
    private int m_NumIndicator;

    private List<GameObject> m_IndicatorList;

    [SerializeField]private float maxSeparation, currentSeparation;
    private bool allowUserInputs, gameSet;

    private CarController carController;

    [SerializeField] private int test_misplacedObj = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        // -------------------------- Encontrar CarController
        carController = FindObjectOfType<CarController>();
        /*
        if (carController == null) { Debug.Log("ya valio, no encontramos a CarController en el script IndicatorHolder"); } else
        {
            Debug.Log("Encontramos al TileManager desde el IndicatorHolder");
        }
        */
        gameSet = false;
        ResetSceneCross();
        ResetCurrentSeparation();
        allowUserInputs = true;
    }

    public void ResetScene()
    {
        if (m_StrabismCamera == null || m_IndicatorPrefab == null)
        {
            Debug.LogError("No se han asignado las variables publicas del script IndicatorHolder");
            return;
        }
        //Primero instanciamos nuestra lista de indicadores, dejándola lista para instanciarlos en X posición.
        m_IndicatorList = new List<GameObject>();

        //El número de caminos es igual al número de indicadores (CarController tiene referencia al numero de caminos).
        if (carController != null) m_NumIndicator = carController.tm.numberOfRoads;
        else m_NumIndicator = 3;

        //Ahora procedemos con los cálculos para la distancia de los objetos que instanciaremos.
        //Primero obtenemos el tamaño de la cámara (en Camera ortographic Size)
        float cameraSize = 0;
        Camera mainCamera = m_StrabismCamera.GetChild(0).GetComponent<Camera>();
        if (mainCamera != null)
        {
            cameraSize = mainCamera.orthographicSize;
            Debug.Log("Camera Size = " + cameraSize);
            //Agregamos un pequeño Offset para extender un poco el ancho, pues el uso de 2 cámaras 3D afecta los cálculos iniciales
            cameraSize += 1.5f;
        }
        else { Debug.LogError("No se encontró correctamente una cámara en el prefab de cámara 3D"); }
        //Ahora multiplicamos por 2 la cameraSize, y esto es el ancho de nuestra pantalla en unidades Unity.
        float horizontalRegion = 2 * (cameraSize);
        //Preparamos los datos para la división de nuestros objetos por región
        int numDivision = m_NumIndicator + 1;
        float separation = horizontalRegion / numDivision;
        //Por cada objeto que instanciaremos, ajusta su separación
        for (int i = 0; i < m_NumIndicator; i++)
        {
            GameObject g = Instantiate(m_IndicatorPrefab, transform);
            g.transform.localPosition = new Vector3(separation * (i + 1) - (cameraSize), 0, 0);
            m_IndicatorList.Add(g);
        }
        m_IndicatorSelectorPlane.position = m_IndicatorList[1].transform.position;

        switch (m_NumIndicator)
        {
            case 2:
                maxSeparation = 18;
                break;
            case 3:
                maxSeparation = 15;
                break;
            case 4:
                maxSeparation = 13;
                break;
            case 5:
                maxSeparation = 10;
                break;
            default:
                break;
        }
        gameSet = true;
        
    }

    //Función para aparecer los indicators en forma de cruz
    private void ResetSceneCross()
    {
        if (m_StrabismCamera == null || m_IndicatorPrefab == null)
        {
            Debug.LogError("No se han asignado las variables publicas del script IndicatorHolder");
            return;
        }

        //Primero instanciamos nuestra lista de indicadores, dejándola lista para instanciarlos en X posición.
        m_IndicatorList = new List<GameObject>();
        m_NumIndicator = 4;
        //Luego indicamos cuánto espacio debemos dejar entre cada elemento a instanciar.
        float separation = 1.5f;

        //Ahora procedemos con los cálculos para la distancia de los objetos que instanciaremos.
        //Primero obtenemos el tamaño de la cámara (en Camera ortographic Size)
        float cameraSize = 0;
        Camera mainCamera = m_StrabismCamera.GetChild(0).GetComponent<Camera>();
        if (mainCamera != null)
        {
            cameraSize = mainCamera.orthographicSize;
            Debug.Log("Camera Size = " + cameraSize);
            //Agregamos un pequeño Offset para extender un poco el ancho, pues el uso de 2 cámaras 3D afecta los cálculos iniciales
            cameraSize += 1.5f;
        }
        else { Debug.LogError("No se encontró correctamente una cámara en el prefab de cámara 3D"); }
        
        //Ahora multiplicamos por 2 la cameraSize, y esto es el ancho de nuestra pantalla en unidades Unity.
        float horizontalRegion = 2 * (cameraSize);

        //Preparamos los datos para la división de nuestros objetos por región
        float halfHorizontalRegion = horizontalRegion / 2;
        Vector3 Vec3_CenterOfCamera = new Vector3 (halfHorizontalRegion - (cameraSize), 0, 0);

        //Instanciamos primero los objetos, los acomodamos en su lugar y luego los agregamos a la lista
        GameObject g = Instantiate(m_IndicatorPrefab, transform);
        g.transform.localPosition = Vec3_CenterOfCamera + new Vector3(0,separation);
        m_IndicatorList.Add(g);

        g = Instantiate(m_IndicatorPrefab, transform);
        g.transform.localPosition = Vec3_CenterOfCamera + new Vector3(separation, 0);
        m_IndicatorList.Add(g);

        g = Instantiate(m_IndicatorPrefab, transform);
        g.transform.localPosition = Vec3_CenterOfCamera + new Vector3(0, -separation);
        m_IndicatorList.Add(g);

        g = Instantiate(m_IndicatorPrefab, transform);
        g.transform.localPosition = Vec3_CenterOfCamera + new Vector3(-separation, 0);
        m_IndicatorList.Add(g);
        //Terminamos de instanciar y posicionar los 4 objetos.

        //ubicamos el plano selector en uno de los 4 objetivos.
        m_IndicatorSelectorPlane.position = m_IndicatorList[1].transform.position;
        
        //Otorgamos un máximo de separación para el efecto 3D
        maxSeparation = 16;
        gameSet = true;
    }

    private void Update()
    {
        if (allowUserInputs && gameSet)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) //Si presionamos el 1, la dificultad baja
            {
                ModifyDificulty(false, test_misplacedObj);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2)) //Si presionamos el 2, las bolitas cambian de lugar
            {
                SeparateAndMisplace(test_misplacedObj);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3)) //Si presionamos el 3, la dificultad sube.
            {
                ModifyDificulty(true, test_misplacedObj);
            }
        }
    }

    public void SetIndicatorSelectorPlane(int desiredLine)
    {
        m_IndicatorSelectorPlane.transform.position = m_IndicatorList[desiredLine].transform.position + new Vector3(0, -separationRatio, 0);
    }


    public void ModifyDificulty(bool increase, int misplacedObstacle)
    {
        if (increase)
        {
            if (currentSeparation + 1 > maxSeparation) currentSeparation = maxSeparation;
            else currentSeparation += 1;
        }
        else if(!increase)
        {
            if (currentSeparation - 1 < 0) currentSeparation = 0;
            else currentSeparation -= 1;
        }
        SeparateAndMisplace(misplacedObstacle);
    }

    private void SeparateAndMisplace(int misplacedObstacle)
    {
        // Crear una serie de Vectores que contengan las posiciones futuras de los indicadores.
        List<Vector3> myVectors = new List<Vector3>();

        //El foreach recoje todas las posiciones y las settea a la separación que deben tener por igual.
        Debug.Log("Igualando indicadores");
        foreach (GameObject g in m_IndicatorList)
        {
            Vector3 savedPos = g.transform.localPosition;
            savedPos.z = currentSeparation;
            myVectors.Add(savedPos);
        }

        Debug.Log("Separando Indicadores");
        int rand = misplacedObstacle;
        Vector3 misplacedVector = myVectors[rand];
        misplacedVector.z = currentSeparation +separationRatio;
        myVectors[rand] = misplacedVector;

        //Asignamos las posiciones calculadas de los indicadores
        for(int i=0; i< m_IndicatorList.Count; i++)
        {
            m_IndicatorList[i].transform.localPosition = myVectors[i];
        }

        //Debug.Log("Posición " + i + ": " + myVectors[i]);
        Vector3 planeIndicator = m_IndicatorSelectorPlane.transform.localPosition;
        planeIndicator.z = currentSeparation + 2*separationRatio;
        m_IndicatorSelectorPlane.transform.localPosition = planeIndicator;
    }

    public void ResetCurrentSeparation()
    {
        currentSeparation = 0f;
    }
}
