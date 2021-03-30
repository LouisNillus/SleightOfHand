using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleUp : Ability
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Double();
    }


    public void Double()
    {
        if (Input.GetKeyDown(KeyCode.F) && CardThrowing.instance.lastCardType != CardType.Any && CardThrowing.instance.canCombo)
        {
            Player.instance.Shoot(CardThrowing.instance.lastCardType);
        }
    }
}
