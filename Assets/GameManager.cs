using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CursorLockMode cursorMode;
    public bool cursorVisibility;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = cursorMode;
        Cursor.visible = cursorVisibility;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SlowMotion(bool state, float value = 0.2f)
    {
        if(state == true)
        {
            Time.timeScale = value;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public IEnumerator TimedSlowMotion(float duration, float value = 0.2f)
    {
        float t = 0f;
        while(t < (duration*value))
        {
            SlowMotion(true, value);
            t += Time.deltaTime;
            yield return null;
        }

        SlowMotion(false);
    }
}
