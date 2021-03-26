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
    public float attackCD;
    float attackCDProgress;

    [Range(0,1)]
    public float animationCalibration;

    [HideInInspector] public UnityEvent OnDead;

    private void Awake()
    {
        instance = this;
    }

    // Start
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update
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
                StartCoroutine(ThrowCooldown());
                StartCoroutine(ThrowDelayed(animationCalibration));
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (CardThrowing.instance.canCombo)
            {
                CardThrowing.instance.forceAceLeftClick = CardThrowing.instance.forceAceRightClick;
                StartCoroutine(ThrowCooldown());           
                StartCoroutine(ThrowDelayed(animationCalibration));
            }
        }

        OnDeadTrigger();
    }

    public IEnumerator ThrowCooldown()
    {       
        while(attackCDProgress < attackCD)
        {
            attackCDProgress += Time.deltaTime;
            yield return null;
        }
        attackCDProgress = 0f;
    }

    public IEnumerator ThrowDelayed(float delay)
    {
        animator.SetTrigger("ThrowCards");
        yield return new WaitForSeconds(delay);
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


        yield return new WaitForSeconds(1f); //slide CD + 0.25f

        cameraSmooth = 0f;

        while (cameraSmooth < 0.25f) // <-- 0.25f
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
