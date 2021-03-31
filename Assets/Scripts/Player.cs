using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
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
    bool isDying;
    vThirdPersonController controller;

    [Range(0,1)]
    public float animationCalibration;

    [HideInInspector] public UnityEvent OnDead;

    IEnumerator damagesFeedback;

    private void Awake()
    {
        instance = this;
    }

    // Start
    void Start()
    {
        damagesFeedback = DamagesFeedback();
        controller = this.GetComponent<vThirdPersonController>();
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

    public void Shoot(CardType cardType = CardType.Any)
    {
        StartCoroutine(ThrowCooldown());
        StartCoroutine(ThrowDelayed(animationCalibration, cardType));
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

    public IEnumerator ThrowDelayed(float delay, CardType cardType = CardType.Any)
    {
        animator.SetTrigger("ThrowCards");
        yield return new WaitForSeconds(delay);
        CardThrowing.instance.ThrowCard(cardType);
    }

    public Volume m_Volume;
    Vignette vig;

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

        StopCoroutine(damagesFeedback);
        StartCoroutine(DamagesFeedback());
    }

    public IEnumerator DamagesFeedback()
    {
        VolumeProfile profile = m_Volume.sharedProfile;
        Vignette vignette;

        if (profile.TryGet<Vignette>(out vignette))
        {
            vig = vignette;
        }

        vig.color.value = Color.red;
        vig.intensity.value = ((-0.008f * HP + 0.8f));

        float intensity = vig.intensity.value;

        float t = 0f;
        while(t < 0.75f)
        {
            vig.color.value = Color.Lerp(Color.red, Color.black, t/0.75f);
            vig.intensity.value = Mathf.Lerp(intensity, 0.18f, t/0.75f);
            t += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator CanComboFeedback()
    {
        VolumeProfile profile = m_Volume.sharedProfile;
        Vignette vignette;

        if (profile.TryGet<Vignette>(out vignette))
        {
            vig = vignette;
        }

        vig.color.value = Color.yellow;
        vig.intensity.value = (0.3f);

        float intensity = vig.intensity.value;

        float t = 0f;
        while (t < 0.75f)
        {
            vig.color.value = Color.Lerp(Color.yellow, Color.black, t / 0.75f);
            vig.intensity.value = Mathf.Lerp(intensity, 0.18f, t / 0.75f);
            t += Time.deltaTime;
            yield return null;
        }
    }

    public void OnDeadTrigger()
    {
        if (HP <= 0 && isDying == false)
        {
            foreach(GameObject en in CardThrowing.instance.enemies)
            {
                en.GetComponent<Enemy>().canAttack = false;
            }

            isDying = true;
            animator.SetTrigger("Dead");
            controller.lockMovement = true;
            controller.lockRotation = true;
            OnDead.Invoke();
        }
    }

}
