using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingText : MonoBehaviour
{
    public string winText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Win()
    {
        GetComponent<TMPro.TextMeshProUGUI>().text = winText;
    }
}
