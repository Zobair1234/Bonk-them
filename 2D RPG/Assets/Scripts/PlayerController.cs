using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class PlayerController : MonoBehaviourPun
{
    // Each player will have unique ID
    [HideInInspector]
    public int id;

    [Header("info")]
    public float moveSpeed;
    public int gold;
    public int currentHp;
    public int maxHp;
    public bool dead;

    [Header("Attack")]
    public int damage;
    public float attackRange;
    public float attackDelay;
    public float LastAttackTime;

    [Header("Components")]
    public Rigidbody2D rig;
    public Player photonPlayer;
    public SpriteRenderer sr;
    public Animator weaponAnim;
    public HeaderInfo headerInfo;

    // local player
    public static PlayerController me;

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;

        //add ourselves to the array
        GameManager.instance.players[id - 1] = this;

        //initialize the health bar
        headerInfo.Initialized(player.NickName, maxHp);


        if (player.IsLocal)
            me = this;
        else
            rig.isKinematic = false;
    }
 
    private void Update()
    {
        Debug.Log("LEft");

        if (!photonView.IsMine)
            return;
        Move();

        if (Input.GetMouseButtonDown(0) && Time.time - LastAttackTime > attackDelay)
            Attack();

        float mouseX = (Screen.width / 2) - Input.mousePosition.x;

        
        if (mouseX < 0)
        {
            weaponAnim.transform.parent.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            weaponAnim.transform.parent.localScale = new Vector3(1, 1, 1);
        }
        
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        rig.velocity = new Vector2(x, y) * moveSpeed;
    }
     void Attack()
    {
        LastAttackTime = Time.time;
        Vector3 dir = (Input.mousePosition - Camera.main.ScreenToWorldPoint(transform.position).normalized);
         

        //shoot raycast
        RaycastHit2D Hit = Physics2D.Raycast(transform.position + dir, dir, attackRange);
      

        // enemy is hit if
        if(Hit.collider !=null && Hit.collider.gameObject.CompareTag("Enemy"))
        {

        }

        //player attack animation
        weaponAnim.SetTrigger("Attack");


    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        currentHp -= damage;

        //update the health bar
       
       headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, currentHp);

        

        if(currentHp <=0)
        {
            Die();
        }

        else
        {
            StartCoroutine(DamageFlash());
            IEnumerator DamageFlash()
            {
                sr.color = Color.red;
                yield return new WaitForSeconds(0.04f);
                sr.color = Color.white;
            }
        }
    }

    void Die()
    {
        dead = true;
        rig.isKinematic = true;

        //we want player to disappear from map
        transform.position = new Vector3(0, 99, 0);

        //player should spawn in the map

        Vector3 spawnPos = GameManager.instance.spawnPoint[Random.Range(0,GameManager.instance.spawnPoint.Length)].position;

        StartCoroutine(Spawn(spawnPos, GameManager.instance.respawnTime));

    }

    //spawn time
    IEnumerator Spawn(Vector3 spawnPos, float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);

        dead = false;
        transform.position = spawnPos;
        currentHp = maxHp;
        rig.isKinematic = false;

        //update health bar
    }

    // healing player

    [PunRPC]
    void Heal(int amountToHeal)
    {
        currentHp = Mathf.Clamp(currentHp + amountToHeal, 0, maxHp);

        //update health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, currentHp);
    }

    //gold function
    [PunRPC]
    void GetGold(int goldToGive)
    {
        gold += goldToGive;

        //update the UI

        GoldUI.instance.UpdateGoldText(gold);
    }

}
