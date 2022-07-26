using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthHandler : MonoBehaviour
{
    //health
    public float maxHP = 100f;
    private float currHP;
    private bool enableRespawn = false;
    private bool invulnerable = false;
    public float invulnerableRespawnTime = 1.0f;
    public float respawnTime = 10f;
    public float healthRegenRate = 5f;

    //effects
    public GameObject deathFXPrefab;
    public Transform centerPoint;
    public RagdollEnabler ragdollEnabler;

    //player components
    public CircleCollider2D circleCollider2D;
    public BoxCollider2D boxCollider2D1, boxCollider2D2;
    public WeaponController weaponController;

    //spawnpoints
    private LevelGenerator levelGenerator;

    //Health bar
    public HealthBar healthBar;
    

    private void Start()
    {
        currHP = maxHP;
        GameObject lvlGen = GameObject.Find("LevelGenerator");
        if (lvlGen)
        {
            levelGenerator = lvlGen.GetComponent<LevelGenerator>();
        }
        healthBar.SetBar(maxHP);
    }

    private void FixedUpdate()
    {
        //healthSlider.transform.position = Camera.main.WorldToScreenPoint(centerPoint.position + healthBarOffset);
    }

    public void ApplyDamage(DamageParams DP)
    {
        if (invulnerable)
        {
            return;
        }

        if (DP.GetOrigin() != "Player")
        {
            this.currHP -= DP.GetDamage();
        }

        healthBar.SetBar(currHP);

        //Death Occured
        if (currHP <= 0)
        {
            ragdollEnabler.EnableRagdoll();

            invulnerable = true;
            Invoke("CanRespawn", respawnTime - 1f);
            Invoke("Respawn", respawnTime);
            if(StaticData.p1GO == gameObject)
            {
                StaticData.p1Lives--;
            }else if(StaticData.p2GO == gameObject)
            {
                StaticData.p2Lives--;
            }
            StaticData.playerUI.SetUI();
            weaponController.DropWeapon();
            //GameObject fx = Instantiate(deathFXPrefab, gameObject.transform.position, gameObject.transform.rotation);
            //Destroy(fx, 2f);
        }
    }

    public void CanRespawn()
    {
        enableRespawn = true;
    }

    //When player respawns
    public void Respawn()
    {
        if (enableRespawn)
        {
            //can no longer respawn and respawnPrompt is disabled
            enableRespawn = false;

            //disable ragdoll
            ragdollEnabler.DisableRagdoll();

            //reset health, position and velocity
            currHP = maxHP;
            healthBar.SetBar(maxHP);
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            Vector3 position = new Vector3(0, 0, 0);
            if (levelGenerator)
            {
                List<Vector3> spawns = levelGenerator.GetSpawnPoints();
                position = spawns[Random.Range(0, spawns.Count)];
            }
            gameObject.transform.position = position;

            circleCollider2D.enabled = true;
            boxCollider2D1.enabled = true;
            boxCollider2D2.enabled = true;

            //make player invulnerable for time
            invulnerable = true;
            Invoke("makeVulnerable", invulnerableRespawnTime);
        }
    }

    private void makeVulnerable()
    {
        invulnerable = false;
    }

    public float GetHealth()
    {
        return currHP;
    }
}
