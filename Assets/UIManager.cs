using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI armAce;

    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void WriteAce(CardType ct)
    {
        switch(ct)
        {
            case CardType.Spades:
                armAce.color = Color.black;
                armAce.text = "a";
                break;
            case CardType.Heart:
                armAce.color = Color.red;
                armAce.text = "N";
                break;
            case CardType.Diamond:
                armAce.color = Color.red;
                armAce.text = "A";
                break;
            case CardType.Clubs:
                armAce.color = Color.black;
                armAce.text = "n";
                break;
        }
    }
}
