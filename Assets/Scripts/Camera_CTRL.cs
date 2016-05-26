using UnityEngine;
using System.Collections;

public class Camera_CTRL : MonoBehaviour
{
    public float sensitivity;
    public Transform cameraOrbit;
    public Transform cameraPitch;

    float maxCameraPitch = 90.0f;
    float minCameraPitch = -90.0f;

    float cameraOrientationPitch = 0.0f;
    float cameraOrientationOrbit = 0.0f;

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        /*cameraOrbit.Rotate(0.0f, Input.GetAxis("Mouse X"), 0.0f);
        cameraPitch.Rotate(Input.GetAxis("Mouse Y"), 0.0f, 0.0f);*/

        cameraOrientationPitch -= Input.GetAxis("Mouse Y") * sensitivity;

        if (cameraOrientationPitch > maxCameraPitch)
            cameraOrientationPitch = maxCameraPitch;

        if (cameraOrientationPitch < minCameraPitch)
            cameraOrientationPitch = minCameraPitch;

        cameraPitch.localEulerAngles = new Vector3(cameraOrientationPitch, 0.0f, 0.0f);

        cameraOrientationOrbit += Input.GetAxis("Mouse X") * sensitivity;

        if (cameraOrientationOrbit > 360.0f)
            cameraOrientationOrbit -= 360.0f;

        if (cameraOrientationOrbit < 0.0f)
            cameraOrientationOrbit += 360.0f;

        cameraOrbit.localEulerAngles = new Vector3(0.0f, cameraOrientationOrbit, 0.0f);
    }
}
