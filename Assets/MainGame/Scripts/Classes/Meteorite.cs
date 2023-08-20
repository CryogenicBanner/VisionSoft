using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class Meteorite : Destructible
{
    public Transform arrowModel;
    public GameObject arrowOutline;
    public GameObject meteoriteModel;
    
    [Range(0, 10)] public float speed;
    public bool rotating = true;
    public Direction arrowDirection;

    [Header("Sound Sources")]
    public StudioEventEmitter spawningSound;

    private Transform target;
    private Vector3 asteroidRotation;

    private Rigidbody rb;
    private BoxCollider col;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        //Obtenemos el transform del objeto que perseguiremos.
        target = GameObject.FindGameObjectWithTag("Player").transform;
        //Rotamos el modelo del meteorito y le damos una rotación a seguir en el transcurso de su vida.
        //meteoriteModel.transform.Rotate(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180), Space.Self);
        asteroidRotation.x = Random.Range(-0.6f, 0.6f);
        asteroidRotation.y = Random.Range(-0.6f, 0.6f);
        asteroidRotation.z = Random.Range(-0.6f, 0.6f);
        arrowDirection = (Direction) Random.Range(0, 3);

        switch (arrowDirection)
        {
            case Direction.Up:
                arrowModel.Rotate(0, 270, 0);
                break;
            case Direction.Down:
                arrowModel.Rotate(0, 90, 0);
                break;
            case Direction.Left:
                arrowModel.Rotate(0, 180, 0);
                break;
            case Direction.Right:
                break;
            default:
                break;
        }
        spawningSound.Play();
    }

    //En FixedUpdate realizamos todo aquello que tenga que ver con físicas (en este caso, movimiento del objeto
    void FixedUpdate()
    {
        if(transform != null)
        {
            //transform.position = Vector3.MoveTowards(transform.position, target, speed);
            if(target != null)
            {
                Vector3 move = transform.InverseTransformPoint(target.position).normalized * speed * Time.deltaTime;
                rb.MovePosition(transform.position + move);
            }
            rb.velocity = Vector3.zero;
            //if (rotating) meteoriteModel.transform.Rotate(asteroidRotation.x, asteroidRotation.y, asteroidRotation.z, Space.Self);
        }
    }

    public void ChangeOutlineColor(Color color)
    {
        meteoriteModel.GetComponent<Outline>().OutlineColor = color;
    }

    public void ChangeArrowOutlineColor(Color color)
    {
        arrowOutline.GetComponent<MeshRenderer>().material.color = color;
    }

    public bool ConfirmSwipe(Direction newDir)
    {
        if (arrowDirection == newDir) return true;
        else return false;
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1);
        Gizmos.DrawCube(transform.position + col.center, col.size);
    }
    */
}
