using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    public int ability = 0;
    public AbilityManager abilityManager;

    void Start(){
    	abilityManager = GetComponent<AbilityManager>();
    }

    public bool pickupAbility(GameObject gameOBJ)
    {
    	switch(gameOBJ.tag)
    	{
    		case "Ability":
    		    ability = Random.Range(0,5); 
    		    if(string.Equals(abilityManager.currentAbility, "Empty"))
    		    {
                    if(ability == 1)
    		    	{
    		    	    abilityManager.currentAbility = "Fire Boost";
    		        }else if(ability == 2){
    		        	abilityManager.currentAbility = "Time Trap";

    		        }else {
    		        	abilityManager.currentAbility = "Empty"; 
    		        	
    		        }
                    return true;
    		    }else{
    		    	return false;
    		    }
            default:
                return false;
    	}
        
        

    	
    }
}
