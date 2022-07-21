using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollEnabler : MonoBehaviour
{
    public GameObject ragdollRig;
    public Rigidbody2D rb;
    public Animator animator;
    public PlayerMovement playerMovement;
    public BoxCollider2D boxCollider1, boxCollider2;
    public CircleCollider2D circleCollider;
    private BoxCollider2D[] ragdollColliders;
    private Rigidbody2D[] ragdollRBs;
    private HingeJoint2D[] ragdollHingJoints;
    private FixedJoint2D[] ragdollFixedJoints;


    //unity animator does not reset bone positions so I have to do it manually
    public List<GameObject> bones;
    public List<float> xPositions;
    public List<float> yPositions;

    // to reset bone 1's rotation & position
    private Quaternion b1quat;

    public GameObject GetRagdoll()
    {
        return ragdollRig;
    }

    public void GetRagdollComponents()
    {
        ragdollColliders = ragdollRig.GetComponentsInChildren<BoxCollider2D>();
        ragdollRBs = ragdollRig.GetComponentsInChildren<Rigidbody2D>();
        ragdollHingJoints = ragdollRig.GetComponentsInChildren<HingeJoint2D>();
        ragdollFixedJoints = ragdollRig.GetComponentsInChildren<FixedJoint2D>();

        foreach(GameObject bone in bones)
        {
            xPositions.Add(bone.transform.localPosition.x);
            yPositions.Add(bone.transform.localPosition.y);
        }
        b1quat = bones[0].transform.localRotation;
    }

    public void EnableRagdoll()
    {
        animator.SetBool("isDead", true);
        Invoke("DelayEnable", 0.1f);
    }

    private void DelayEnable()
    {
        animator.enabled = false;
        for (int i = 0; i < bones.Count; i++)
        {
            bones[i].transform.localPosition = new Vector3(xPositions[i], yPositions[i], 0);
        }
        foreach (BoxCollider2D col in ragdollColliders)
        {
            col.enabled = true;
        }
        foreach (Rigidbody2D rb in ragdollRBs)
        {
            rb.isKinematic = false;
        }
        foreach (HingeJoint2D hj in ragdollHingJoints)
        {
            hj.enabled = true;
        }
        foreach (FixedJoint2D fj in ragdollFixedJoints)
        {
            fj.enabled = true;
        }

        bones[0].transform.localRotation = b1quat;
        bones[1].transform.localRotation = Quaternion.identity;
        rb.freezeRotation = false;
        rb.gravityScale = 1;
        rb.mass = 1;
        boxCollider1.enabled = false;
        boxCollider2.enabled = false;
        circleCollider.enabled = false;
        playerMovement.enabled = false;
    }

    public void DisableRagdoll()
    {
        foreach (BoxCollider2D col in ragdollColliders)
        {
            col.enabled = false;
        }
        foreach (Rigidbody2D rb in ragdollRBs)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
        foreach (HingeJoint2D hj in ragdollHingJoints)
        {
            hj.enabled = false;
        }
        foreach (FixedJoint2D fj in ragdollFixedJoints)
        {
            fj.enabled = false;
        }
        for(int i=0; i<bones.Count; i++)
        {
            bones[i].transform.localPosition = new Vector3(xPositions[i], yPositions[i], 0);
        }
        rb.rotation = 0f;
        rb.freezeRotation = true;
        boxCollider1.enabled = true;
        boxCollider2.enabled = true;
        circleCollider.enabled = true;
        playerMovement.enabled = true;
        animator.enabled = true;
        animator.SetBool("isDead", false);
        rb.gravityScale = 5;
        rb.mass = 10;
        /*ragdollRig.transform.SetParent(gameObject.transform);
        ragdollRig.transform.localPosition = new Vector3(0, -0.454f, 0);*/

        /*Destroy(ragdollRig);
        GameObject newRig = Instantiate(ragdollRig, Vector3.zero, Quaternion.identity);
        newRig.transform.parent = gameObject.transform;*/
    }

    private void Awake()
    {
        GetRagdollComponents();
    }


}
