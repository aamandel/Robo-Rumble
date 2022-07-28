using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonController : MonoBehaviour
{
    public GameObject rootBone;
    public float flySpeed = 0.5f;
    public float verticalFlySpeed = 1f;
    public float magnitude = 0.1f;
    Vector3 prevPos;
    public float delayMove = 0.1f;
    public Weapon Flame;

    public GameObject jawBone, headBone;
    private bool jawsAreOpen = false;
    private float jawOpen = -45f;
    private float jawClosed = 45f;
    private float headOpen = 20f;
    private float headClosed = -20f;

    public GameObject wingBoneFront1, wingBoneFront2, wingBoneBack1, wingBoneBack2;
    public float wingFlapSpeed = 1f;
    public float wingAmplitude = 30f;
    private bool wingsUp = true;

    public List<GameObject> bodyParts = new List<GameObject>();

    private void Start()
    {
        prevPos = rootBone.transform.position;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        
        rootBone.transform.position += transform.up * Mathf.Sin(Time.time * verticalFlySpeed) * magnitude + (transform.right * flySpeed);
        rootBone.transform.right = (prevPos - rootBone.transform.position).normalized;
        if(rootBone.transform.eulerAngles.z == 0)
        {
            rootBone.transform.eulerAngles = new Vector3(0, 0, -180);
        }

        if((prevPos - rootBone.transform.position).y > 0)
        {
            if (!jawsAreOpen)
            {
                StartCoroutine(SetBoneAngle(jawBone, jawOpen, 0.1f));
                StartCoroutine(SetBoneAngle(headBone, headOpen, 0.1f));
                jawsAreOpen = true;
            }
            Flame.Shoot();
        }
        else if (jawsAreOpen)
        {
            StartCoroutine(SetBoneAngle(jawBone, jawClosed, 0.1f));
            StartCoroutine(SetBoneAngle(headBone, headClosed, 0.1f));
            jawsAreOpen = false;
        }

        // TODO: turn around
        if (rootBone.transform.localPosition.x > 40f)
        {
            rootBone.transform.localPosition = new Vector3(-40f, rootBone.transform.localPosition.y, rootBone.transform.localPosition.z);
            
            //transform.Rotate(0, 180f, 0f);
        }

        prevPos = rootBone.transform.position;
        if(delayMove < 0)
        {
            SnakeMovement();
        }
        else
        {
            delayMove -= Time.fixedDeltaTime;
        }

        

        //flap wings
        if (wingsUp)
        {
            StartCoroutine(SetBoneAngle(wingBoneFront1, wingAmplitude, wingFlapSpeed));
            StartCoroutine(SetBoneAngle(wingBoneBack1, wingAmplitude, wingFlapSpeed));
            StartCoroutine(SetBoneAngle(wingBoneFront2, wingAmplitude/2, wingFlapSpeed));
            StartCoroutine(SetBoneAngle(wingBoneBack2, wingAmplitude/2, wingFlapSpeed));
            wingsUp = false;
            Invoke("WingSwitch", wingFlapSpeed);
        }
        
    }

    void SnakeMovement()
    {
        for(int i=1; i<bodyParts.Count; i++)
        {
            MarkerManager markM = bodyParts[i - 1].GetComponent<MarkerManager>();
            if (markM)
            {
                bodyParts[i].transform.rotation = markM.markerList[0].rotation;
                markM.markerList.RemoveAt(0);
            }
        }
    }

    IEnumerator SetBoneAngle(GameObject bone, float newAngle, float duration)
    {
        float timePassed = 0f;
        while( timePassed < duration)
        {
            float angle = (newAngle/duration) * Time.deltaTime;
            bone.transform.rotation *= Quaternion.AngleAxis(angle, Vector3.forward);

            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    void WingSwitch()
    {
        wingsUp = !wingsUp;
        wingAmplitude = -wingAmplitude;
    }
}
