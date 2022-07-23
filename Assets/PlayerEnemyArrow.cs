using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnemyArrow : MonoBehaviour
{
    public bool player1 = true;
    private GameObject myGO, enemyGO;

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
        gameObject.transform.position = myGO.transform.position;
        Quaternion rotation = Quaternion.LookRotation(enemyGO.transform.position - transform.position, transform.TransformDirection(Vector3.up));
        transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
        //transform.LookAt(enemyGO.transform, Vector3.right);
    }
}
