using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerSettings : MonoBehaviour
{
    [Header("Difficulty")]
    [Tooltip("Duration of the session")]
    public float sessionTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PauseGame() { Time.timeScale = 0; }
    public void ResumeGame() { Time.timeScale = 1; }
}
