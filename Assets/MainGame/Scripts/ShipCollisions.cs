using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class ShipCollisions : MonoBehaviour
{
    public StudioEventEmitter soundMeteorImpactsShip;
    public StudioEventEmitter soundDamagedShip;

    private Destructible ship_destructible;
    private Ship s;
    private Outline outline_ship;


    private void Start()
    {
        ship_destructible = GetComponent<Destructible>();
        if (ship_destructible == null) Debug.Log("No existe un Destructible en el objeto " + transform.name);
        s = GetComponent<Ship>();
        if (s == null) Debug.Log("No existe un Ship en el objeto " + transform.name);
        outline_ship = GetComponent<Outline>();
        if (outline_ship == null) Debug.Log("No existe un outline_ship en el objeto " + transform.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Projectile" && ship_destructible != null)
        {
            DamageSettings ds = other.GetComponent<DamageSettings>();
            Destructible d_projectile = other.GetComponent<Destructible>();
            if(ds != null && d_projectile != null)
            {
                ship_destructible.ReceiveDamage(ds.damageGiven);
                soundMeteorImpactsShip.Play();
                d_projectile.ReceiveDamage(d_projectile.GetCurrentLife());
                if (ship_destructible.GetCurrentLife() == 1)
                {
                    soundDamagedShip.Play();

                }
                if (!outline_ship.OutlineIsAlternating())
                {
                    StartCoroutine(outline_ship.AlternateColors());
                }
            }
            else
            {
                Debug.Log("No existe un DamageSettings en el proyectil " + other.transform.name);
            }
        }
    }

    private void OnDestroy()
    {
        soundDamagedShip.Stop();
    }
}
