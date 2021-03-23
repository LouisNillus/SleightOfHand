using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardThrowing : MonoBehaviour
{

    public Transform cardOrigin;
    public Transform B;
    public Transform C;
    public GameObject target;

    [Range(-1,1)]
    public float alignementThreshold = 0.5f;

    public float duration;
    public float noise;

    public int cardDamages;

    public GameObject cardPrefab;

    public List<GameObject> enemies = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        target = MostAlignedEnemy(); 
        
        if (Input.GetMouseButtonDown(0) && target != null)
        {
            GameObject go = Instantiate(cardPrefab, cardOrigin.transform.position, Quaternion.identity);

            B.position = Vector3.Lerp(cardOrigin.position, target.transform.position, 0.33f);
            C.position = Vector3.Lerp(cardOrigin.position, target.transform.position, 0.66f);

            StartCoroutine(Interpolate(go, target.transform.position, duration));
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
        }

        if (Input.GetKey(KeyCode.C))
        {
            GameManager.instance.SlowMotion(true, 0.2f);
        }
        else
        {
            GameManager.instance.SlowMotion(false);
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

    public IEnumerator Interpolate(GameObject _go, Vector3 finalPos, float duration)
    {
        float time = 0f;

        Vector3 b = NoisyVector(B.position, noise);
        Vector3 c = NoisyVector(C.position, noise);

        Vector3 oldPos = _go.transform.position;
        Vector3 firstPos = _go.transform.position;
        Vector3 newPos = _go.transform.position;

        while (time < duration)
        {
            oldPos = newPos;
            _go.transform.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * 360f, time / duration);
            _go.transform.position = CubicInterpolation(firstPos, b, c, finalPos, (time/duration));
            newPos = _go.transform.position;
            Vector3 v3 = newPos - oldPos;
            _go.transform.rotation = Quaternion.LookRotation(v3);
            _go.transform.localEulerAngles += Vector3.right * 90f;
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(_go);
    }

    public Vector3 NoisyVector(Vector3 vec, float range)
    {
        return new Vector3(vec.x + Random.Range(-range, range), vec.y + Random.Range(-range, range), vec.z + Random.Range(-range, range));
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
