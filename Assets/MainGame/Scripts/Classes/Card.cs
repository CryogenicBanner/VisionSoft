using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Card : Destructible
{
    public bool objectiveStatus;

    // Start is called before the first frame update
    void Start()
    {
        objectiveStatus = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setObjetiveStatus(bool objective) { objectiveStatus = objective; }
    public bool getObjectiveStatus() { return objectiveStatus; }
}
