using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SetColor : MonoBehaviour
{
    public Color color;

    // Start
    void Start()
    {
        Methods.SetMaterialColor(gameObject, color);
    }

    // Update
    void Update()
    {

#if UNITY_EDITOR
        Methods.SetMaterialColor(gameObject, color);
#endif      

    }
}
