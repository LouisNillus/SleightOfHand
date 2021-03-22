using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CursorLockMode cursorMode;
    public bool cursorVisibility;

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
}
