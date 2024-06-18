using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BoatController : MonoBehaviour
{
    public PlayableDirector boatLeaveTimeline;

    public void Interact()
    {
        boatLeaveTimeline.Play();
    }
}
