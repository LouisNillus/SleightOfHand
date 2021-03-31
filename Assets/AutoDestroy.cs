using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float delay;

    // Start
    void Start()
    {
        Invoke("Kill", delay);
    }

    public void Kill()
    {
        Destroy(this.gameObject);
    }
}
