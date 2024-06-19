using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrown : MonoBehaviour
{
    public GameObject player;
    public float drownSpeed = 10;

    bool inWater = false;

    void Update()
    {
        if (inWater)
        {
            player.GetComponent<PlayerController>().UpdateHealthAndHydration(-Time.deltaTime * drownSpeed, 1);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            inWater = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            inWater = false;
        }
    }
}
