using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Invector.vCharacterController;

public class Player : MonoBehaviour
{
    public static Player instance;

    Animator animator;

    bool canSlide = true;

    public int HP;
    public float delayThrow;
    public float attackCD;
    float attackCDProgress;

    [HideInInspector] public UnityEvent OnDead;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C) && canSlide == true)
        {
            StartCoroutine(Slide());
        }

        if(Input.GetMouseButtonDown(0))
        {
            if(attackCDProgress == 0f)
            {
                StartCoroutine(ThrowCardCoroutine());
            }
        }

        OnDeadTrigger();
    }

    public IEnumerator ThrowCardCoroutine()
    {
        while(attackCDProgress < attackCD)
        {
            attackCDProgress += Time.deltaTime;
            yield return null;
        }

        animator.SetTrigger("ThrowCards");
        attackCDProgress = 0f;
    }

    public void ThrowCard()
    {
        CardThrowing.instance.ThrowCard();
    }

    public IEnumerator Slide()
    {
        canSlide = false;
        float cameraSmooth = 0f;

        float currentHeight = Camera.main.GetComponent<vThirdPersonCamera>().height;
        animator.SetTrigger("Slide");
        while (cameraSmooth < 0.25f)
        {
            Camera.main.GetComponent<vThirdPersonCamera>().height = Mathf.Lerp(currentHeight, 1.1f, cameraSmooth/0.25f);
            cameraSmooth += Time.deltaTime;
            yield return null;
        }


        yield return new WaitForSeconds(1f);

        cameraSmooth = 0f;

        while (cameraSmooth < 0.25f)
        {
            Camera.main.GetComponent<vThirdPersonCamera>().height = Mathf.Lerp(1.1f, 1.8f, cameraSmooth / 0.25f);
            cameraSmooth += Time.deltaTime;
            yield return null;
        }
        canSlide = true;
    }

    public void TakeDamages(int value)
    {
        HP -= value;
    }

    public void OnDeadTrigger()
    {
        if (HP <= 0) OnDead.Invoke();
    }

}
