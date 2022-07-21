using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Kinect/Kinect Clamp")]

public class KinectClamp : MonoBehaviour
{

    [System.Serializable]
    public class BoneClamp
    { //Class for bones with min and max XYZ rotation values
        public Transform bone;
        public Rigidbody2D rb;
        public float minZ = 0;
        public float maxZ = 360;
        public bool CrossesZero = false;
    }

    public BoneClamp[] boneClamps;
    private Vector3 newV3 = new Vector3(0f, 0f, 0f);

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    /*void Update()
    {
        foreach (BoneClamp clamp in boneClamps)
        {
            clamp.minZ = Mathf.Clamp(clamp.minZ, 0, 360);
            clamp.maxZ = Mathf.Clamp(clamp.maxZ, 0, 360);
        }
    }*/

    // We use LateUpdate to grab the rotation from the Transform after all Updates from
    // other scripts have occured
    void FixedUpdate()
    {
        foreach (BoneClamp clamp in boneClamps)
        {
            float rotationZ = clamp.rb.rotation;
            /*if(rotationZ > 350 || rotationZ < 10)
            {

            }
            else if(rotationZ < 180)
            {
                rotationZ = Mathf.Clamp(rotationZ, clamp.minZ, clamp.maxZ);
            }
            else
            {
                rotationZ = Mathf.Clamp(rotationZ, 360 + clamp.minZ, 360);
            }*/

            if (clamp.CrossesZero)
            {
                if(rotationZ > clamp.maxZ && rotationZ < 180)
                {
                    rotationZ = clamp.maxZ;
                }
                if(rotationZ < clamp.minZ && rotationZ > 180)
                {
                    rotationZ = clamp.minZ;
                }
            }
            else
            {
                rotationZ = Mathf.Clamp(rotationZ, clamp.minZ, clamp.maxZ);
            }
            
            

            newV3.z = rotationZ;

            //clamp.bone.localEulerAngles = newV3;
            //clamp.rb.rotation = rotationZ;
        }
    }

}
