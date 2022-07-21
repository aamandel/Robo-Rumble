using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
	public bool pickedUp = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
    	//if(collision.gameObject.CompareTag("Player"))
    	//{
    		PickupManager manager = collision.GetComponent<PickupManager>();
    		if(manager)
    		{
    			pickedUp = manager.pickupAbility(gameObject);

    			if(pickedUp)
    			{
    			    Destroy(gameObject);
    			}
    		}
    	//}
    }
}
