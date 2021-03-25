using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardThrowing : MonoBehaviour
{
    public static CardThrowing instance;

    public Transform cardOrigin;
    public Transform B;
    public Transform C;
    public GameObject target;

    public CardType forceAceLeftClick;
    public CardType forceAceRightClick;

    [Header("Cards Data")]
    [Range(0,100)]
    public int aceProbability;
    [Range(0,100)]
    public int normalCardDamages;

    [Header("Aiming Settings")]
    [Range(-1,1)]
    public float alignementThreshold = 0.5f;


    [Header("Cards Movement")]
    [Range(0, 1)]
    public float Bratio;
    [Range(0, 1)]
    public float Cratio;

    public float duration;
    public float noise;

    public (CardType, CardType) combo = (CardType.Any, CardType.Any);

    /*[HideInInspector]*/ public CardType lastCardType;

    public GameObject cardPrefab;

    [HideInInspector] public List<GameObject> enemies = new List<GameObject>();
    public bool canCombo;

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

        target = MostAlignedEnemy(); 
        
        /*if (Input.GetMouseButtonDown(0) && target != null)
        {
            GameObject go = Instantiate(cardPrefab, cardOrigin.transform.position, Quaternion.identity);

            B.position = Vector3.Lerp(cardOrigin.position, target.transform.position, Bratio);
            C.position = Vector3.Lerp(cardOrigin.position, target.transform.position, Cratio);

            StartCoroutine(Interpolate(go, target.transform.position, duration));

            target.GetComponent<Enemy>().TakeDamages(cardDamages);
        }

        if (Input.GetMouseButtonDown(1))
        {
            foreach(GameObject en in enemies)
            {
                GameObject go = Instantiate(cardPrefab, cardOrigin.transform.position, Quaternion.identity);

                target = en;

                B.position = Vector3.Lerp(cardOrigin.position, target.transform.position, 0.33f);
                C.position = Vector3.Lerp(cardOrigin.position, target.transform.position, 0.66f);


                StartCoroutine(Interpolate(go, target.transform.position, duration));
            }
        }*/

        if (Input.GetKey(KeyCode.C))
        {
            GameManager.instance.SlowMotion(true, 0.2f);
        }
        else
        {
            GameManager.instance.SlowMotion(false);
        }
        
    }

    public void ThrowCard()
    {
        if(target != null)
        {
            GameObject go = Instantiate(cardPrefab, cardOrigin.transform.position, Quaternion.identity);
            Card c = go.GetComponent<Card>();
            c.Initialize();

            if (canCombo)
            {
                Debug.Log("Combo !");
                combo.Item1 = lastCardType;
                combo.Item2 = c.typeOfCard;        
            }

            B.position = Vector3.Lerp(cardOrigin.position, target.transform.position, Bratio);
            C.position = Vector3.Lerp(cardOrigin.position, target.transform.position, Cratio);

            StartCoroutine(Interpolate(go, target, duration));

            target.GetComponent<Enemy>().TakeDamages(normalCardDamages);
        }
    }


    public Vector3 QuadraticInterpolation(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);

        return Vector3.Lerp(ab, bc, t);
    }

    public Vector3 CubicInterpolation(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 ab_bc = QuadraticInterpolation(a, b, c, t);
        Vector3 bc_cd = QuadraticInterpolation(b, c, d, t);

        return Vector3.Lerp(ab_bc, bc_cd, t);
    }

    public IEnumerator Interpolate(GameObject _go, GameObject _target, float duration)
    {
        float time = 0f;

        Vector3 b = B.position.NoisyVector(noise);
        Vector3 c = C.position.NoisyVector(noise);

        Vector3 oldPos = _go.transform.position;
        Vector3 firstPos = _go.transform.position;
        Vector3 newPos = _go.transform.position;

        Card card = _go.GetComponent<Card>();


        if (card.isAnAce)
        {
            GameManager.instance.StartCoroutine(GameManager.instance.TimedSlowMotion(0.75f));
        }

        lastCardType = card.typeOfCard;

        while (time < duration)
        {
            if (time < 0.75f)
            {
                canCombo = true;
            }
            else canCombo = false;

            oldPos = newPos;
            _go.transform.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * 360f, time / duration);
            _go.transform.position = CubicInterpolation(firstPos, b, c, _target.transform.position, (time/duration));
            newPos = _go.transform.position;
            Vector3 v3 = newPos - oldPos;
            _go.transform.rotation = Quaternion.LookRotation(v3);
            _go.transform.localEulerAngles += Vector3.right * 90f;
            time += Time.deltaTime;
            yield return null;
        }

        //Target reached :
        if(combo != (CardType.Any, CardType.Any))
        {
            card.Combo(combo, _target.GetComponent<Enemy>());
        }
        else
        {
            card.Play(_target.GetComponent<Enemy>());
        }



        Destroy(_go);
    }



    public GameObject MostAlignedEnemy()
    {
        GameObject currentBest = null;
        float bestScore = -1000f;

        foreach(GameObject en in enemies)
        {
            if(Vector3.Dot((en.transform.position - Camera.main.transform.position).normalized, Camera.main.transform.forward.normalized) > alignementThreshold)
            if (bestScore < Vector3.Dot((en.transform.position - Camera.main.transform.position).normalized, Camera.main.transform.forward.normalized))
            {
                bestScore = Vector3.Dot((en.transform.position - Camera.main.transform.position).normalized, Camera.main.transform.forward.normalized);
                if (target != null && en != target) Methods.SetMaterialColor(target, Color.white);
                currentBest = en;
            }
        }

        if (target != null && currentBest == null) Methods.SetMaterialColor(target, Color.white);

        Methods.SetMaterialColor(currentBest, Color.red);
        return currentBest;
    }
}
