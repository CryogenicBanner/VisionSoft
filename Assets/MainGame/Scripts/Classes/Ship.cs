using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Ship : MonoBehaviour
{
    [Header("GameObject Components")]
    public Transform model;
    public Transform lasersContainer;

    [Header("Prefabs")]
    public GameObject speedUpParticles;

    [Header("Sound Emitters")]
    public StudioEventEmitter sound_lazerShot;
    public StudioEventEmitter sound_targetSelection;

    [Header("Settings")]
    public float laserDuration;

    private GameObject objective;
    private GameObject currentSpeedParticles;
    private float currentLaserDuration;

    private bool isInTurbulence;
    private float timeTurbulence;
    private SwipeDetection swipeDetection;

    // Start is called before the first frame update
    void Start()
    {
        objective = null;
        timeTurbulence = 0;
        swipeDetection = GetComponent<SwipeDetection>();
        //if (swipeDetection != null) Debug.Log("Si hay Swipe");
    }

    // Update is called once per frame
    void Update()
    {
        if (objective != null)
        {
            FollowObjective(objective.transform);
        }

        UpdateInputs();
        ShipTurbulence(isInTurbulence);
    }

    private void UpdateInputs()
    {
        //#if UNITY_ANDROID && !UNITY_EDITOR
        if(Input.touchCount > 0) {
            foreach (Touch t in Input.touches) {
                if(Input.GetTouch(t.fingerId).phase == TouchPhase.Began) {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(t.fingerId).position);
                    if(Physics.Raycast(ray, out hit)) {
                        Destructible d = hit.transform.GetComponent<Destructible>();
                        if (d != null)
                        {
                        Debug.Log("Enfocando :v");
                            SetObjective(hit.transform.gameObject);
                            sound_targetSelection.Play();
                        }
                    }
                }
            }
        }
        //#endif
        else if (Input.GetButtonDown("Player1_Shoot"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))  //Si el rayo le pegó a algo a una distancia menor igual que 100 metros
            {
                Destructible d = hit.transform.GetComponent<Destructible>();
                if (d != null)
                {
                    SetObjective(hit.transform.gameObject);
                    sound_targetSelection.Play();
                }

            }
            else
            {
                Debug.Log("No detectamos objetivo");
            }
        }
        if(objective != null)
        {
            UpdateArrows();
            //#if UNITY_ANDROID && !UNITY_EDITOR
                if (swipeDetection.activeDirection)
                {
                    Debug.Log("Disparando hacia la " + swipeDetection.currentDirection.ToString());
                    if (objective.GetComponent<Meteorite>().ConfirmSwipe(swipeDetection.currentDirection))
                    {
                        Debug.Log("DISPARANDO! :v");
                        StartCoroutine(Shoot(objective.transform.position));
                        
                        swipeDetection.activeDirection = false;
                    }
                }
            //#endif
        }
    }

    private void UpdateArrows()
    {
        bool confirmedswipe = false;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) confirmedswipe = objective.GetComponent<Meteorite>().ConfirmSwipe(Direction.Left);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) confirmedswipe = objective.GetComponent<Meteorite>().ConfirmSwipe(Direction.Right);
        else if (Input.GetKeyDown(KeyCode.UpArrow)) confirmedswipe = objective.GetComponent<Meteorite>().ConfirmSwipe(Direction.Up);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) confirmedswipe = objective.GetComponent<Meteorite>().ConfirmSwipe(Direction.Down);
        
        if (confirmedswipe)
        {
            StartCoroutine(Shoot(objective.transform.position));
        }
        
    }
    private void DealDamage(Destructible d)
    {
        d.ReceiveDamage(1);
    }
    private void FollowObjective(Transform obj)
    {
        transform.LookAt(obj);
    }

    public GameObject GetObjective() { return objective; }
    public void SetObjective(GameObject objective) { this.objective = objective; }

    public IEnumerator Shoot(Vector3 targetPos)
    {
        //Debug.Log("Nave Disparando :v");
        Vector3 target = new Vector3(0, 0, DistanceBetween2Vector3(transform.position, targetPos));
        if(lasersContainer != null)
        {
            LineRenderer[] lines = lasersContainer.GetComponentsInChildren<LineRenderer>();
            foreach(LineRenderer line in lines)
            {
                line.SetPosition(1, target);
            }
        }
        lasersContainer.transform.gameObject.SetActive(true);
        lasersContainer.gameObject.SetActive(true);
        sound_lazerShot.Play();
        //Resaltamos el objetivo al que le estamos disparando
        Outline o = objective.transform.GetComponentInChildren<Outline>();
        if (!o.OutlineIsAlternating()) StartCoroutine(o.AlternateColors());
        //Esperamos una cantidad de tiempo a que se vea la animación de los láseres.
        yield return new WaitForSeconds(laserDuration);
        //Al terminar de inflingir daño, quitamos una cantidad de la vida al objetivo y desactivamos los láseres.
        if(objective != null)
        {
            DealDamage(objective.transform.GetComponent<Destructible>());
        }
        lasersContainer.gameObject.SetActive(false);
        yield return null;
    }

    private float DistanceBetween2Vector3(Vector3 a, Vector3 b)
    {
        float value = 0;
        value = Mathf.Sqrt(Mathf.Pow(b.x - a.x, 2)+Mathf.Pow(b.z - a.z, 2));
        return value;
    }



    private void InstantiateSpeedParticles(bool instantiate)
    {
        if (instantiate && currentSpeedParticles == null)
        {
            Vector3 pos = transform.position + (Vector3.forward * 10);
            currentSpeedParticles = Instantiate(speedUpParticles);
            currentSpeedParticles.transform.position = pos;
            currentSpeedParticles.transform.LookAt(transform);
        }
        else if(!instantiate)
        {
            if(currentSpeedParticles != null) Destroy(currentSpeedParticles);
        }
    }

    // ------------------------------ Funciones aún en desarrollo

    public void SetInDemoMode(bool set)
    {
        InstantiateSpeedParticles(set);
    }

    public IEnumerator SpeedUpAnimation()
    {
        InstantiateSpeedParticles(true);
        yield return null;
    }

    private void ShipTurbulence (bool turbulence)
    {
        if (turbulence)
        {
            timeTurbulence += Time.deltaTime;
            Vector3 vectorTurbulence = new Vector3(
                Mathf.Lerp(-0.1f, 0.1f, Mathf.PingPong(timeTurbulence, 0.2f) - 0.1f), 
                Mathf.Lerp(-0.1f, 0.1f, Mathf.PingPong(timeTurbulence, 0.2f) -0.1f), 
                Mathf.Lerp(-0.1f, 0.1f, Mathf.PingPong(timeTurbulence, 0.2f) -0.1f)
                );
            model.position = vectorTurbulence;
        }
        else
        {
            model.position = Vector3.zero;
            timeTurbulence = 0;
        }
    }
    public void SetShipTurbulence(bool turbulence) { isInTurbulence = turbulence; }
}
