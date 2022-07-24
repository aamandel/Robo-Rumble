using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnemyArrow : MonoBehaviour
{
    public bool player1 = true;
    private GameObject myGO, enemyGO;
    public float minDisplayDist = 10f;
    public SpriteRenderer arrowSprite;

    private void Start()
    {
        if (player1)
        {
            myGO = StaticData.p1GO;
            enemyGO = StaticData.p2GO;
        }
        else
        {
            myGO = StaticData.p2GO;
            enemyGO = StaticData.p1GO;
        }
    }

    private void Update()
    {
        if(!myGO || !enemyGO)
        {
            return;
        }
        gameObject.transform.position = myGO.transform.position;
        Vector3 distVector = enemyGO.transform.position - transform.position;
        if(distVector.magnitude < minDisplayDist)
        {
            arrowSprite.enabled = false;
            return;
        }
        arrowSprite.enabled = true;
        transform.right = distVector;
    }
}
