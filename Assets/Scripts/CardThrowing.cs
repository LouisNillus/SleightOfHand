using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardThrowing : MonoBehaviour
{

    public Transform cardOrigin;
    public Transform B;
    public Transform C;
    public GameObject target;


    public float interpolateAmount;

    public float duration;
    public float noise;

    public GameObject cardPrefab;



    public List<GameObject> enemies = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            target = MostAlignedEnemy();        
            GameObject go = Instantiate(cardPrefab, cardOrigin.transform.position, Quaternion.identity);

            B.position = Vector3.Lerp(cardOrigin.position, target.transform.position, 0.33f);
            C.position = Vector3.Lerp(cardOrigin.position, target.transform.position, 0.66f);

            StartCoroutine(Interpolate(go, duration));
        }

        if(Input.GetKey(KeyCode.A))
        {
            Time.timeScale = 0.1f;
        }
        else
        {
            Time.timeScale = 1f;
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

    public IEnumerator Interpolate(GameObject _go, float duration)
    {
        float time = 0f;

        Vector3 b = NoisyVector(B.position, noise);
        Vector3 c = NoisyVector(C.position, noise);

        Vector3 oldPos = _go.transform.position;
        Vector3 newPos = _go.transform.position;

        while (time < duration)
        {
            oldPos = newPos;
            _go.transform.position = CubicInterpolation(cardOrigin.position, b, c, target.transform.position, (time/duration));
            newPos = _go.transform.position;
            Vector3 v3 = newPos - oldPos;
            _go.transform.rotation = Quaternion.LookRotation(v3);
            _go.transform.localEulerAngles += Vector3.right * 90f;
            time += Time.deltaTime;
            yield return null;
        }
        
        Methods.SetMaterialColor(target, Random.ColorHSV());

        Destroy(_go);
    }

    public Vector3 NoisyVector(Vector3 vec, float range)
    {
        return new Vector3(vec.x + Random.Range(-range, range), vec.y + Random.Range(-range, range), vec.z + Random.Range(-range, range));
    }

    public GameObject MostAlignedEnemy()
    {
        GameObject currentBest = target;
        float bestScore = -1000f;

        foreach(GameObject en in enemies)
        {
            if (bestScore < Vector3.Dot((en.transform.position - Camera.main.transform.position).normalized, Camera.main.transform.forward.normalized))
            {
                bestScore = Vector3.Dot((en.transform.position - Camera.main.transform.position).normalized, Camera.main.transform.forward.normalized);
                if (target != null && en != target) Methods.SetMaterialColor(target, Color.white);
                currentBest = en;
            }
        }

        Methods.SetMaterialColor(currentBest, Color.red);
        return currentBest;
    }
}
