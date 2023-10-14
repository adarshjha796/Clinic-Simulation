using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogCollieFull_Controller : MonoBehaviour
{

    public int DamageDirection = 4;
    public int Health = 10;
    Animator anim;
    float horiz = 0f;
    float vertic = 0f;
    float H = 0f;
    float V = 0f;
    int AnimationID;
    public bool HurtLeg = false;
    bool hide = false;
    bool sleep = false;

    //buttoms
    public string Hide = "";
    public string Hite = "";
    public string Eat = "";





    private void Awake()
    {

    }
    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorBasicController();
    }
  
    void AnimatorBasicController()
    {
        
        anim.SetInteger("IDAnim", 0);
        anim.SetBool("attack", false);
        anim.SetBool("damage", false);
        anim.SetBool("death", false);
        anim.SetBool("hate", false);
        anim.SetBool("eat", false);
        anim.SetBool("drink", false);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            AttackStation(); 
        }
        if (Input.GetKeyDown(Hite))
        {
            ResetAnimation();
            HateStation();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            SetDamage(2);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W))
        {
            if (Input.GetKeyDown("t"))
            {
                JumpStation();
            }
            else BasicMovement();
        }
        else
        {
            IdleStation();
            V = 0f;
            H = 0f;
        } 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpStation();
        }
        if (Input.GetKeyDown(Hide))
        {
            HadeStation();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SleepStations();
        }
        if (Input.GetKeyDown(Eat))
        {
            EatDrinkStation();
        }

        if (horiz < H)
        {
            horiz = horiz + 0.04f;
            if (horiz >= H)
            {
                horiz = H;
            }
        }
        if (horiz > H)
        {
            horiz = horiz - 0.04f;
            if (horiz <= H)
            {
                horiz = H;
            }
        }
        if (vertic < V)
        {
            vertic = vertic + 0.02f;
            if (vertic >= V)
            {
                vertic = V;
            }
        }
        if (vertic > V)
        {
            vertic = vertic - 0.02f;
            if (vertic <= V)
            {
                vertic = V;
            }
        }
    }
    private void ResetAnimation()
    {
        anim.SetInteger("IDAnim", 0);
        anim.SetBool("attack", false);
        anim.SetBool("damage", false);
        anim.SetBool("death", false);
        anim.SetBool("hate", false);
        anim.SetBool("eat", false);
        anim.SetBool("drink", false);
        anim.SetBool("hide", false);
        anim.SetBool("walk", false);
        anim.SetBool("idle", false);
        anim.SetBool("jump", false);
        anim.SetFloat("Hor", horiz);
        anim.SetFloat("Vert", vertic);
        V = 0f;
    }
    void BasicMovement()
    {
        anim.SetBool("idle", false);
        anim.SetBool("sleep", false);
        // Move forward----------------
        if (Input.GetKeyDown(KeyCode.W))
        {
           
            V = 2f;
        } else if (Input.GetKeyUp(KeyCode.W))
        { 
            V = 0f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                anim.SetBool("hide", false);
            }
                if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                V = 3f; 
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                V = 2f;
            }
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                V = 1f;
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                V = 2f;
            }
            if(HurtLeg == true)
            {
                V = 0.5f;
            }
        }
        else V = 0f;
       //Move back----------------
        if (Input.GetKey(KeyCode.S))
        {
            V = -1f;
            vertic = vertic - 0.02f;
            if (vertic <= V)
            {
                vertic = V;
            }
        }
        //Move Left------------
        if (Input.GetKey(KeyCode.A))
        {
            H = -1f;
        } else if (Input.GetKeyUp(KeyCode.A))
        {
            H = 0f;
        }
        //Move Right------------
        if (Input.GetKey(KeyCode.D))
        {
            H = 1f;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            H = 0f;
        }

        anim.SetBool("walk", true);
        anim.SetBool("idle", false);
        anim.SetBool("jump", false);
        anim.SetFloat("Hor", horiz);
        anim.SetFloat("Vert", vertic);
    }
    void IdleStation()
    {
        int ID = Random.Range(0, 2);
        anim.SetInteger("IDAnim2", ID);
        
        anim.SetFloat("Hor", horiz);
        anim.SetFloat("Vert", vertic);
        anim.SetBool("idle", true);
        anim.SetBool("walk", false);
        anim.SetBool("jump", false);
    }
    void SleepStations()
    {
        if (sleep == false)
        {
            sleep = true;

            anim.SetBool("idle", false);
        }
        else sleep = false;
        if (sleep == true)
        {
            anim.SetBool("sleep", true);
        }
        else anim.SetBool("sleep", false);
    }
    void EatDrinkStation()
    {
        anim.SetBool("eat", true);
        anim.SetBool("idle", false);
    }
    void JumpStation()
    {
        anim.SetBool("jump", true);
        anim.SetBool("idle", false);
        anim.SetBool("hide", false);
    }
    void HateStation()
    {
        anim.SetBool("hate", true);
        anim.SetBool("idle", false);
        anim.SetBool("hide", false);
    }

    void HadeStation()
    {
        if (hide == false)
        {
            hide = true;
            anim.SetBool("idle", false);
        }
        else hide = false;
        if (hide == true)
        {
            anim.SetBool("hide", true);

        }
        else anim.SetBool("hide", false);

    }

    void AttackStation()
    {
        anim.SetBool("idle", false);
        int ID = Random.Range(7, 9); 
        anim.SetInteger("IDAnim", ID);
        anim.SetBool("attack", true);
        Debug.Log("Attack");

    }
   
    public void SetDamage(int damage)
    {
        Health = Health - damage;
        anim.SetInteger("IDAnim", DamageDirection);
        anim.SetBool("damage", true);
        if(Health <= 4)
        {
            HurtLeg = true;
        }
        if (Health <= 0)
        {
            Death();
        }
        Debug.Log("Damage");
    }
    void Death()
    {
        int ID = Random.Range(5, 7);
        anim.SetBool("death", true);
        anim.SetInteger("IDAnim", ID);
        Debug.Log("Die");
    }
    
}
