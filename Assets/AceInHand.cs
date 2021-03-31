using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AceInHand : Ability
{

    CardType loadedAce;
    bool isReloaded = true;

    [Range(0,20)]
    public float cooldownDuration;

    // Start is called before the first frame update
    void Start()
    {
        NextAce();
    }

    // Update is called once per frame
    void Update()
    {
        //loadedAce = CardType.Diamond;

        if (Input.GetKeyDown(KeyCode.E) && isReloaded == true)
        {
            Player.instance.Shoot(loadedAce);
            StartCoroutine(Cooldown());
        }
    }

    public void NextAce()
    {
        loadedAce = (CardType)Random.Range(0, 4);
        UIManager.instance.WriteAce(loadedAce);
    }
    
    public IEnumerator Cooldown()
    {
        isReloaded = false;
        float t = 0f;

        while(t < cooldownDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        NextAce();

        isReloaded = true;
    }

}
