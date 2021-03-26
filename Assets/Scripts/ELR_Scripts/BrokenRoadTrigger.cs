using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenRoadTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Player>() != null) 
        {
            this.GetComponentInParent<Animator>().SetTrigger("Go");
            Destroy(this.gameObject);
        }
    }
}
