using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenRoadTriggerBox : MonoBehaviour
{
    [SerializeField] bool startTrigger;

    [SerializeField] bool slowMotion;
    [SerializeField] float slowMotionDuration;
    [SerializeField] float slowMotionScale;

    [SerializeField] bool changeCameraDistance;
    [SerializeField] float newCameraDistance;
    [SerializeField] Camera playerCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
        {
            if (startTrigger)
                StartTrigger();

            if (slowMotion)
                SlowMotion();

            if (changeCameraDistance)
                CameraDistance();
            
            Destroy(this.gameObject);
        }
    }

    void StartTrigger() 
    {
        this.gameObject.GetComponentInParent<Animator>().SetTrigger("Go");
    }
    void SlowMotion()
    {
        GameManager.instance.StartCoroutine(GameManager.instance.TimedSlowMotion(slowMotionDuration, slowMotionScale));
    }
    void CameraDistance()
    {
        playerCamera.GetComponent<vThirdPersonCamera>().defaultDistance = newCameraDistance;
    }
}
