using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunPlayer : MonoBehaviour
{
    public float stunTime = 2f;
    public GameObject stunFX;

    public void Stun(GameObject _player)
    {
        PlayerMovement PM = _player.GetComponent<PlayerMovement>();
        if (PM)
        {
            PM.enabled = false;
            _player.GetComponent<CharacterController2D>().ResetPlayerMovement(stunTime);
            GameObject fx = Instantiate(stunFX, _player.transform.position, Quaternion.identity);
            fx.transform.SetParent(_player.transform);
            Destroy(fx, stunTime);
            _player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}
