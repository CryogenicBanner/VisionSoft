using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [Range(1, 4)]
    public int life;         //Vida base
    [SerializeField]
    private int currentLife; //Vida en tiempo de ejecución
    public GameObject particleDestruction;

    private bool isQuitting;

    // Start is called before the first frame update
    void Awake()
    {
        isQuitting = false;
        ResetLife();
        if (currentLife <= 0) currentLife = 1;    
    }

    public void ResetLife()
    {
        currentLife = life;
        //Debug.Log("Reseteando vida: " + currentLife);
    }

    private void OnDestroy()
    {
        if (!isQuitting)
        {
            if(particleDestruction != null)
            {
                Instantiate(particleDestruction, transform.position, transform.rotation);
            }
        }
    }

    public void ReceiveDamage(int damage)
    {
        Debug.Log("Vida actual: " + currentLife);
        currentLife -= damage;
        Debug.Log("Vida tras resta: " + currentLife);
        if (currentLife <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public int GetCurrentLife()
    {
        return currentLife;
    }
    void OnApplicationQuit()
    {
        isQuitting = true;
    }
}
