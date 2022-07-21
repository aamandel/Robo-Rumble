using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    public CharacterController2D controller;
    public PlayerMovement playermove;
    public Transform fireEmissionPoint;
    public ParticleSystem fireVFX;
    public Animator animator;
    public float FlightBoost = 50f;
    public string currentAbility;                          //key for which ability the player has
    private Rigidbody2D RB2D;                       //Rigid body
    private bool facingRight;

    public GameObject magicWand;
    public GameObject timeTrap;
    public Transform wandPoint;
    public Transform TrapPoint;
    private GameObject wand;

    [SerializeField] private Image FireBoostImg;
    [SerializeField] private Image TimeTrapImg;


    // Start is called before the first frame update
    void Start()
    {
    	currentAbility = "Time Trap";
    	RB2D = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
    	facingRight = controller.m_FacingRight;

    	if(wand){
    		wand.transform.position = new Vector3(wandPoint.position.x, wandPoint.position.y, wandPoint.position.z);
    		wand.transform.rotation = wandPoint.rotation;
    	}

    	if(string.Equals(currentAbility, "Fire Boost")){
    		FireBoostImg.enabled = true;
    		TimeTrapImg.enabled = false;
    		if(Input.GetAxisRaw("Vertical") == 1f){
                animator.SetBool("IsBoosting", true); 
                playermove.doneBoosting = false;
                currentAbility = "Empty";
    		}
    	}else if(string.Equals(currentAbility, "Time Trap")){
            TimeTrapImg.enabled = true;
            FireBoostImg.enabled = false;
            if(Input.GetAxisRaw("Vertical") == 1f){
                animator.SetBool("IsCasting", true);
                currentAbility = "Empty";
    		}
    	}else if(string.Equals(currentAbility, "Empty")){
    		FireBoostImg.enabled = false;
            TimeTrapImg.enabled = false;
    	}
        
    }

    void SpawnFireEffect()
    {
        //Instantiate(fireVFX, fireEmissionPoint.position, fireEmissionPoint.rotation);
        if(facingRight)
        {
        	RB2D.velocity = new Vector2(FlightBoost, FlightBoost);
        }else{
            RB2D.velocity = new Vector2(-FlightBoost, FlightBoost);
        }
        fireVFX.Play();
        
    }

    void FinishFireEffect()
    {
    	playermove.doneBoosting = true;
    	if(facingRight)
        {
        	RB2D.velocity = new Vector2(FlightBoost, FlightBoost);
        }else{
            RB2D.velocity = new Vector2(-FlightBoost, FlightBoost);
        }
    }

    void SpawnWand()
    {
    	if(!wand)
    	{
    		wand = Instantiate(magicWand, wandPoint.position, wandPoint.rotation);
    	}
        
    }

    void RemoveWand(){
    	if(wand){
    		Destroy(wand);
    		animator.SetBool("IsCasting", false);
    	}
    }

    void SpawnTimeTrap(){
        Instantiate(timeTrap, TrapPoint.position, TrapPoint.rotation);
    }
}
