using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerCTRL_Final : MonoBehaviour {
    // GamePlay Variables
    [SerializeField]
    private Transform _mCameraXZForwards;
    [SerializeField]
    private float _mWalkingSpeed;
    [SerializeField]
    private float _mRunningSpeed;
    [SerializeField]
    private float _mJumpStreangth;

    [SerializeField]
    private float _mArtificalVelocityMagDegredation;// = 0.9f;
    private Vector3 _mArtificialVelocity = Vector3.zero;

    private PLAYER_STATES _mState = new PLAYER_STATES();

    // ParentChild Relationships
    private GameObject _mParentGameObject = null;
    [SerializeField]
    Text _mUiText;

    // Components
    private Rigidbody _mRigidBD;
    private bool _mTouchingFloor;

    [SerializeField]
    private float _mSwingForce;
    [SerializeField]
    private Targeting_System_SCR _mCrossHair;
    [SerializeField]
    private Line_SCR _mRopeLine;
    [SerializeField]
    private GameObject _mPlayerAvatar;
    [SerializeField]
    private Transform _mDefaultRopEnd;

    private Animator _mAnimCTRL;
    private SpringJoint _mRopeJoint;
    private GameObject _mTargetedObject;
    private bool _mIsSwinging;


    void Start()
    {
        _mRigidBD = GetComponent<Rigidbody>();
        _mAnimCTRL = GetComponentInChildren<Animator>();
        
        _mUiText.text = "No Parent";
        _mState = PLAYER_STATES.IDLE;
        _mTouchingFloor = false;

        DetachRopeFromTarget();
    }


    void FixedUpdate()
    {
        _mTouchingFloor = IsTouchingFloor();
        UpdateRope();


        if (!_mTouchingFloor /* && DistanceToFloor() > 0.25f*/)
        {
            _mState = PLAYER_STATES.AIRBOURNE;
            _mRigidBD.AddForce(_mArtificialVelocity);
        }
        else if (Input.GetKey(KeyCode.Space) && _mTouchingFloor)
        {
            _mState = PLAYER_STATES.AIRBOURNE;
            _mRigidBD.AddForce(Vector3.up * _mJumpStreangth);
            _mRigidBD.AddForce(_mArtificialVelocity);
        }
        else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _mState = PLAYER_STATES.RUN;
                _mArtificialVelocity = FlattenMovementVectorAgainstFloor(CalculateHorizontalMovementVecotor(_mRunningSpeed));
            }
            else
            {
                _mState = PLAYER_STATES.WALK;
                _mArtificialVelocity = FlattenMovementVectorAgainstFloor(CalculateHorizontalMovementVecotor(_mWalkingSpeed));
            }
            Vector3 heading = _mArtificialVelocity;
            heading.y = 0;
            heading.Normalize();
            TurnPlayerAvatar(heading);
        }
        else
        {
            _mState = PLAYER_STATES.IDLE;

            if (_mArtificialVelocity.magnitude > 0.01f)
            {
                Vector3 subTraction = _mArtificialVelocity * _mArtificalVelocityMagDegredation;
                _mArtificialVelocity -= subTraction * Time.deltaTime;
            }
            else if (_mArtificialVelocity.magnitude != 0)
            {
                _mArtificialVelocity = Vector3.zero;
            }
        }

        if (_mState.Equals(PLAYER_STATES.AIRBOURNE))
        {
            _mRigidBD.drag = 0.5f;
            //AssignTansformParent(null);
        }
        else
        {
            _mRigidBD.drag = Mathf.Infinity;
            GameObject floor = GetFloor().collider.gameObject;
            if (floor != null)
                AssignTansformParent(floor);
        }

        _mRigidBD.MovePosition(transform.position + _mArtificialVelocity);
        UpdateAnimCTRL();
    }


    Vector3 CalculateHorizontalMovementVecotor(float speed)
    {
        Vector3 direction = new Vector3();
        direction += (_mCameraXZForwards.forward * (speed * Input.GetAxis("Vertical"))) * Time.deltaTime;
        direction += (_mCameraXZForwards.right * (speed * Input.GetAxis("Horizontal"))) * Time.deltaTime;
        return direction;
    }

    Vector3 FlattenMovementVectorAgainstFloor(Vector3 horizontalDirection)
    {
        RaycastHit hit;
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + 0.25f, transform.position.z);
        Ray ray = new Ray(origin, Vector3.down);
        if (Physics.Raycast(ray, out hit, 0.35f))
        {
            // http://answers.unity3d.com/questions/10323/calculating-a-movement-direction-that-is-a-tangent.html
            Vector3 temp = Vector3.Cross(hit.normal, horizontalDirection);
            Vector3 myDirection = Vector3.Cross(temp, hit.normal);
            horizontalDirection = myDirection;
            //Debug.Log("X: " + horizontalDirection.x + " Y: " + horizontalDirection.y + " Z: " + horizontalDirection.z);
        }
        // If the ray doesn't hit the floor, just return the vector anyway
        return horizontalDirection;
    }

    bool IsTouchingFloor()
    {
        Vector3 startPos = transform.position;
        startPos.y += 0.25f; // When Player hits the ground, he can intersect it, pushing the origin through the floor, the ray cast is lifted up slightly so that it always intersects.
        Ray ray = new Ray(startPos, Vector3.down);
        //return Physics.SphereCast(ray, 0.2f, 0.35f);

        return Physics.Raycast(startPos, Vector3.down, 0.35f);
    }

    RaycastHit GetFloor()
    {
        RaycastHit returnInfo;
        Vector3 startPos = transform.position;
        startPos.y += 0.25f;
        Ray ray = new Ray(startPos, Vector3.down);

        Physics.Raycast(ray, out returnInfo, 0.45f);
        //Physics.SphereCast(ray, 0.2f, out returnInfo);
        return returnInfo;
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
            _mUiText.text = "No Parent";
        }
        else
        {
            _mParentGameObject = newParent;
            transform.parent = newParent.transform;
            _mUiText.text = newParent.tag;
        }
    }

    private void TurnPlayerAvatar(Vector3 heading)
    {
        float step = 10 * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(_mPlayerAvatar.transform.forward, heading, step, 0.0F);
        _mPlayerAvatar.transform.rotation = Quaternion.LookRotation(newDir);
    }

    private void UpdateAnimCTRL()
    {
        _mAnimCTRL.SetBool("Ascending", _mState == PLAYER_STATES.AIRBOURNE && _mRigidBD.velocity.y > 0);
        _mAnimCTRL.SetBool("Decending", _mState == PLAYER_STATES.AIRBOURNE && _mRigidBD.velocity.y <= 0);
        _mAnimCTRL.SetBool("IsWalking", _mState == PLAYER_STATES.WALK);
        _mAnimCTRL.SetBool("IsRunning", _mState == PLAYER_STATES.RUN);
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        if (_mState != PLAYER_STATES.AIRBOURNE && GetFloor().distance > 0.28f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - (GetFloor().distance - 0.28f), transform.position.z);
        }
        _mTouchingFloor = true;
    }

    private void UpdateRope()
    {
        if (Input.GetAxis("Fire1") != 0)
        {
            if (_mCrossHair.FoundTarget() && _mTargetedObject == null)
            {
                _mTargetedObject = _mCrossHair.GetTarget();
                _mRopeLine._posA = _mTargetedObject.transform;
                AttachByRopeToTarget();
            }
        }
        else
        {
            DetachRopeFromTarget();
        }

        if (_mIsSwinging)
        {
            if (Input.GetKey(KeyCode.Q) && _mRopeJoint.maxDistance > 0.75f)
                _mRopeJoint.maxDistance -= 5.0f * Time.deltaTime;

            if (Input.GetKey(KeyCode.E) && _mRopeJoint.maxDistance < 100.0f)
                _mRopeJoint.maxDistance += 5.0f * Time.deltaTime;

            if ((Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) && _mState == PLAYER_STATES.AIRBOURNE)
                _mRigidBD.AddForce(CalculateHorizontalMovementVecotor(_mSwingForce));
        }
    }

    public void AttachByRopeToTarget()
    {
        if (_mRopeJoint == null)
        {
            _mIsSwinging = true;
            //_rigidBD.drag = 0.0f;
            Debug.Log("Entered create rope");
            this.gameObject.AddComponent<SpringJoint>();
            _mRopeJoint = this.GetComponent<SpringJoint>();

            _mRopeJoint.autoConfigureConnectedAnchor = false;

            _mRopeJoint.connectedBody = _mTargetedObject.GetComponent<Rigidbody>();
            _mRopeJoint.anchor = new Vector3(0.0f, 0.32f, 0.0f); //_rope.anchor.Set(0.0f, 0.32f, 0.0f);
            _mRopeJoint.connectedAnchor = new Vector3(0.0f, -0.5f, 0.0f); //_rope.connectedAnchor.Set(0.0f, -0.5f, 0.0f);

            Vector3 difference = _mTargetedObject.transform.position - transform.position;
            float distance = Vector3.Magnitude(difference);
            _mRopeJoint.maxDistance = distance;
            _mRopeJoint.minDistance = 0.75f;

            _mRopeJoint.spring = 1000.0f;
            _mRopeJoint.damper = 0.2f;
            _mRopeJoint.tolerance = 0.025f;
            _mRopeJoint.breakForce = Mathf.Infinity;
            _mRopeJoint.breakTorque = Mathf.Infinity;
        }
    }

    public void DetachRopeFromTarget()
    {
        _mIsSwinging = false;
        //_rigidBD.drag = 0.5f;
        _mTargetedObject = null;
        _mRopeLine._posA = _mDefaultRopEnd;
        Destroy(_mRopeJoint);
        _mRopeJoint = null;
    }
}
