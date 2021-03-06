using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using System.Linq;

public class AttackEvent : MonoBehaviour
{
    public float cooldownTime = 2;
    public float Damage = 100;
    private int hits = 0;

    protected Animator animationObject;

    protected bool cooldown = false;
    public GameObject playerObject;
    public List<GameObject> AttackRange = new List<GameObject>();
    // Start is called before the first frame update

    void Start()
    {
        animationObject = playerObject.GetComponent(typeof(Animator)) as Animator;
    }

    void FixedUpdate() {
        float isAttacking = Input.GetAxis("Fire1");
        bool hitting = false;
        if (isAttacking > .1 && hitting == false)
        {
            hitting = true;
            hits ++;
            if (hits%2==0)
            {
                animationObject.SetBool("isInward", true);
            }
            else
            {
                animationObject.SetBool("isInward", false);
            }
        }
        else
        {
            hitting = false;
        }
    }

    void Update()
    {
        //StartCoroutine(Cooldown());
        var playerController = playerObject.GetComponent<ThirdPersonController>();
        var playerScript = playerObject.GetComponent(typeof(ThirdPersonController)) as ThirdPersonController;
        float isAttacking = Input.GetAxis("Fire1");
        if (isAttacking > .1 && cooldown == false)
        {
            animationObject.SetTrigger("Attack");
            cooldown = true;
            playerScript.canMove = false;
            StartCoroutine(Cooldown());
            //Debug.Log("cooldown in effect");
            foreach (GameObject enemy in AttackRange.ToList())
            {
                if (enemy == null)
                {
                    AttackRange.Remove(enemy);
                }
                else
                {
                    enemy.GetComponent<enemyHealth>().TakeDamage(Damage);
                    //if the enemy health script attached to the enemy in question has a health value of 0 or less, trigger the death animation on the enemyCOPILOT animator object 
                    if (enemy.GetComponent<enemyHealth>().Health <= 0)
                    {
                        enemy.GetComponent<Animator>().SetTrigger("Death");
                        //set the isDead bool on the enemyCOPILOT script to true
                        enemy.GetComponent<EnemyCOPILOT>().isDead = true;
                    }
                }

            }
        }
        /*if (cooldown == true)
        {
            yield return new WaitForSeconds(cooldownTime);
            cooldown = false;
        }*/
    }

    IEnumerator Cooldown()
    {
        if (cooldown == true)
        {
            var playerScript = playerObject.GetComponent(typeof(ThirdPersonController)) as ThirdPersonController;
            /*while(playerScript.isGrounded == false){
                yield return null;
            }*/
            yield return new WaitForSeconds(cooldownTime);
            cooldown = false;
            playerScript.canMove = true;
            animationObject.ResetTrigger("Attack");
            //Debug.Log("cooldown ended");
        }
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(AttackRange);
        if (other.CompareTag("Enemy"))
        {
            if (AttackRange.Contains(other.gameObject))
            {
                Debug.Log("item already in list, wont add");
            }
            else
            {
                AttackRange.Add(other.gameObject);
            }
            //Debug.Log("added " + other.gameObject + " to Attack Range");
        }
    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log(AttackRange);
        if (other.CompareTag("Enemy"))
        {
            AttackRange.Remove(other.gameObject);
            //Debug.Log("removed " + other.gameObject + " from Attack Range");
        }
    }

}
