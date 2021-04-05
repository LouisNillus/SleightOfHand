using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopVfx : MonoBehaviour
{
    int count;

    /*public float fadeDuration = 1;

    private float t = 0;*/
    public void Stop()
    {
        //gameObject.GetComponent<ParticleSystem>().Stop();
        foreach (ParticleSystem ps in transform.GetComponentsInChildren<ParticleSystem>())
        {
            count++;
            Debug.Log(count + ps.gameObject.name);
            ps.Stop();
            /*var currentCol = ps.colorOverLifetime.color.gradient.Evaluate(Time.deltaTime);
            var transparentCol = new Color32((byte)currentCol.r, (byte)currentCol.g, (byte)currentCol.b, 0);
            ps.startColor = Color32.Lerp(currentCol, transparentCol, t);*/
        }
    }
}
