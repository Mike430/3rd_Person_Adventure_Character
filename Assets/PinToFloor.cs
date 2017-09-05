using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinToFloor : MonoBehaviour
{
    public float _mRayLength = Mathf.Infinity;
    [SerializeField]
    private CapsuleCollider _mCapsuleCollider;

	// Use this for initialization
	void Start ()
    {
        PinMeToFloor();
	}
	
	public bool PinMeToFloor ()
    {
        bool success = true;
        Vector3 localUp = this.transform.up;
        Ray floorSearchRay = new Ray(transform.position, -1 * localUp);
        RaycastHit floorDetection = new RaycastHit();
        if (!Physics.Raycast(floorSearchRay, out floorDetection, _mRayLength))
        {
            success = false;
        }
        else
        {
            //float sloap = floorDetection.normal.y; // 0 = 90d, 1 = 0d
            float radius = _mCapsuleCollider.radius;

            //Vector3 heightMod = localUp * Mathf.Tan(Mathf.Deg2Rad * (90 + (Mathf.Rad2Deg * Vector3.Dot(-localUp, floorDetection.normal))));
            Vector3 heightMod = CalculateArtificialDistanceFromFloor(floorDetection.normal, localUp, radius);
            Vector3 newPos = floorDetection.point + (heightMod * radius);
            //Vector3 newPos = floorDetection.point + heightMod;
            newPos.y -= 0.15f;

            Debug.Log(this.name + " y = " + heightMod + "\n");

            this.transform.position = newPos;
        }

        return success;
    }

    Vector3 CalculateArtificialDistanceFromFloor(Vector3 normal, Vector3 playerUp, float radius)
    {
        float scaler = Vector3.Dot(-playerUp, normal);
        scaler *= Mathf.Rad2Deg;
        scaler += 90;
        scaler *= Mathf.Deg2Rad;
        scaler = Mathf.Tan(scaler);
        Vector3 returnVec = playerUp * scaler;
        
        Debug.Log(this.name + " - " + returnVec + "\nup: " + -playerUp + "\tn: " + normal + "\tscaler: " + scaler);
        
        return returnVec;
    }
}


/*
 * fire a ray to hit the floor
 * get the radius
 * make the height modification equal to local up vector * f(the floor's normal, the local up, the radius) !!CHECK THESE THREE ARE CORRECT!!
 * 
 * f(fN, lU, r)
 * {
 * calculate the difference in angle between these the fN and lU vectors
 * the height modification will now be equal to either a sin or tan function of that angle
 * then we shall multiply the final height modification by the radius.
 * }
 * 
 * Then we make the player's new position equal to where the inital ray hit plus the height mod vector.
 */
