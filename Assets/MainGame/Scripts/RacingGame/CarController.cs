using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Tooltip("Speed to accelerate car")]
    public float forwardSpeed;

    //Acceder a un box collider dentro de los tiles para saber la distancia entre 2 tiles.

    //Está público porque el script de IndicatorHolder usa este valor también.
    public TileManager tm;
    public IndicatorHolder ih;

    private CharacterController controller;
    public Car auto;

    private Vector3 direction;

    private int desiredLine = 1;

    // Start is called before the first frame update
    void Start()
    {
        tm = GameObject.FindGameObjectWithTag("GameController").GetComponent<TileManager>();
        if (tm.numberOfRoads == 0) tm.numberOfRoads = 3;
        controller = GetComponent<CharacterController>();
        tm.GenerateObstacle();
        ih.ModifyDificulty(true, tm.posCorrectObstacle);
    }

    // Update is called once per frame
    void Update()
    {
        direction.z = forwardSpeed;
        ControllerInputs();
        MoveCar();
    }

    private void MoveCar()
    {
        //Calcular dónde estaremos en el futuro

        //--Vector3 targetPosition = new Vector3();
        //--targetPosition = transform.position;

        //--targetPosition.z = tm.transform.forward.z * (tm.zSpawn - tm.tileLength);
        //targetPosition.x = tm.GetMapInstantiation().x - tm.tileLength;

        Vector3 targetPosition = (transform.forward * transform.position.z) + (transform.position.y * transform.up);

        targetPosition += new Vector3(desiredLine, 0, 0) * tm.tileLength;
        transform.position = Vector3.Lerp(transform.position, targetPosition, 10 * Time.deltaTime);
    }


    private void FixedUpdate()
    {
        Physics.SyncTransforms();
        //Mueve el carro hacia adelante
        controller.Move(direction * Time.fixedDeltaTime);
    }

    private void ControllerInputs()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            desiredLine++;
            //if (desiredLine > maxLines) desiredLine = maxLines;
            if (desiredLine > tm.numberOfRoads -1 ) desiredLine = tm.numberOfRoads -1;
            Debug.Log("Carril actual = " + desiredLine);
            SetIndicatorHolderPlaneSelector(desiredLine);
            
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            desiredLine--;
            if (desiredLine < 0) desiredLine = 0;
            Debug.Log("Carril actual = " + desiredLine);
            ih.SetIndicatorSelectorPlane(desiredLine);
        }
    }

    private void SetIndicatorHolderPlaneSelector(int desiredLine)
    {
        ih.SetIndicatorSelectorPlane(desiredLine);
    }

    public int GetDesiredLine()
    {
        return desiredLine;
    }

    // #Log 0611
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggger activado -> " + other.ToString());
        if (other.tag == "Obstacle")
        {
            //Restar vida y bajar dificultad
            auto.ReceiveDamage(1);
            Debug.Log("La vida del auto es -> " + auto.currentLife);
            tm.GenerateObstacle();
            ih.ModifyDificulty(false, tm.posCorrectObstacle);
        }
        else if (other.tag == "Untagged")
        {
            //Aumentar puntuación y dificultad.
            tm.GenerateObstacle();
            ih.ModifyDificulty(true, tm.posCorrectObstacle);
            Debug.Log("Aumentando dificultad!");
        }
    }

}
