using System.Collections.Generic;
using UnityEngine;

public class OverlaySystem : MonoBehaviour
{
    public GameObject nearOverlay;
    public GameObject farOverlay;

    private Queue<GameObject> nearOverlayQueue = new ();
    private Queue<GameObject> farOverlayQueue = new ();

    public void CreateOverlayObjects()
    {

    }
}