using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_CTRL_3 : MonoBehaviour
{
    [SerializeField]
    private float _mSpeed;
    [SerializeField]
    private Transform _mCameraOrientation;
    [SerializeField]
    private float _mGravity;
    [SerializeField]
    private float _mJumpPower;

    private CharacterController _mCharacterCTRL;
    private GameObject _mParentGameObject;
    private float _mFallSpeed = 0;

    public Text output;


    void Start()
    {
        _mCharacterCTRL = this.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            dir = CalculateHorizontalMovementVecotor(_mSpeed);
        }
        else
        {
            dir = CalculateHorizontalMovementVecotor(_mSpeed * 5);
        }

        _mFallSpeed = ControlFall(_mFallSpeed);
        output.text = _mFallSpeed.ToString();
        _mCharacterCTRL.Move(new Vector3(dir.x, _mFallSpeed, dir.z));
        Debug.Log(_mCharacterCTRL.isGrounded + "\n");
    }

    Vector3 CalculateHorizontalMovementVecotor(float speed)
    {
        Vector3 direction = new Vector3();
        direction += (_mCameraOrientation.forward * (speed * Input.GetAxis("Vertical"))) * Time.deltaTime;
        direction += (_mCameraOrientation.right * (speed * Input.GetAxis("Horizontal"))) * Time.deltaTime;
        return direction;
    }

    float ControlFall(float fallSpeed)
    {
        if (!_mCharacterCTRL.isGrounded)
        {
            fallSpeed -= _mGravity * Time.deltaTime;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                fallSpeed = _mJumpPower;
            }
            else
            {
                fallSpeed = 0;
            }
        }
        return fallSpeed;
    }

    void AssignTansformParent(GameObject newParent)
    {
        if (newParent == null)
        {
            //detach
            if (_mParentGameObject != null)
            {
                //_mRigidBD.velocity += _mParentGameObject.GetComponent<Rigidbody>().velocity;
            }
            _mParentGameObject = null;
            transform.parent = null;
        }
        else
        {
            _mParentGameObject = newParent;
            transform.parent = newParent.transform;
        }
    }
}
