using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float time = 300f;
    public GameObject boat;
    int minutes;
    int seconds;
    bool stopTimer = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopTimer)
        {
            time -= Time.deltaTime;
        }

        seconds = (int) (time % 60);
        minutes = (int) Mathf.Floor(time / 60);

        GetComponent<TMPro.TextMeshProUGUI>().text = minutes.ToString() + ":" + seconds.ToString();

        if (time <= 0)
        {
            boat.GetComponent<BoatController>().Interact();
        }
    }

    public void FreezeTimer()
    {
        stopTimer = true;
    }
}
