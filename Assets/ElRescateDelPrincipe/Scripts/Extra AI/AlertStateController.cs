using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class AlertStateController : MonoBehaviour
{
    [SerializeField] private float alertValue;
    [SerializeField] private bool isRisingAlert;
    [Tooltip("Seconds to detect objective")]
    public float alertTolerance;
    


    // Start is called before the first frame update
    void Start()
    {
        alertValue = 0;
        isRisingAlert = false;
        if (alertTolerance == 0)
        {
            alertTolerance = 2;
        }

    }

    public void FixedUpdate()
    {
        if (isRisingAlert)
        {
            if (!IsReady())
            {
                alertValue += Time.deltaTime;
                if (alertValue > alertTolerance) alertValue = alertTolerance;
            }
        }
        else
        {
            if (!IsCalm())
            {
                alertValue -= Time.deltaTime;
                if (alertValue < 0) alertValue = 0;
            }
        }
    }

    public void IncreaseStatus()
    {
        if (!isRisingAlert) isRisingAlert = true;
    }

    public void DecreaseStatus()
    {
        if (isRisingAlert) isRisingAlert = false;
    }

    public bool IsReady()
    {
        return alertValue == alertTolerance;
    }

    public bool IsCalm()
    {
        return alertValue == 0;
    }
}
