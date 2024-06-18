using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float healthRestored = 30f;
    public float hydrationRestored = -15f;
    public int uses = 1;
    public bool destoryAfterUsing = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact(GameObject caller)
    {
        if (uses != 0)
        {
            caller.GetComponent<PlayerController>().UpdateHealthAndHydration(healthRestored, hydrationRestored);
            uses -= 1;
        }
        if (uses == 0)
        {
            if (destoryAfterUsing)
            {
                Destroy(gameObject);
            }
        }
    }
}
