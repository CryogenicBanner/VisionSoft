using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{

    public int life = 1;
    public int currentLife = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (currentLife <= 0) ResetLife();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetLife()
    {
        currentLife = life;
    }

    public void ReceiveDamage(int damage)
    {
        currentLife -= damage;
        if (currentLife <= 0)
        {
            //Destroy(this.gameObject);
            currentLife = 0;
            Debug.Log("Se murio el carrito :,v");
        }
    }

    public int GetCurrentLife()
    {
        return currentLife;
    }
}
