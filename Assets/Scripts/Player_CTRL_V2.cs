using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;

enum PLAYER_STATES{
    IDLE,
    WALK,
    RUN,
    AIRBOURNE
}

public class Player_CTRL_V2 : MonoBehaviour {
    // GamePlay Variables
    [SerializeField]
    private Transform _mCameraOrientation;
    [SerializeField]
    private float _mWalkingSpeed;
    [SerializeField]
    private float _mRunningSpeed;
    [SerializeField]
    private float _mJumpStreangth;

    [SerializeField]
    private float _mArtificalVelocityMagDegredation = 0.5f;
    private Vector3 _mArtificialVelocity = Vector3.zero;

    private PLAYER_STATES _mState = new PLAYER_STATES();

    // ParentChild Relationships
    private GameObject _mParentGameObject = null;
    [SerializeField]
    Text _mUiText;

    // Components
    private Rigidbody _mRigidBD;
    private bool _mStillTouchingFloor;

    // Debug
    public Renderer _mRend;
    public Material _mIdleStateMat;
    public Material _mWalkStateMat;
    public Material _mRunStateMat;
    public Material _mAirbourneStateMat;

    
    void Start ()
    {
        _mRigidBD = GetComponent<Rigidbody>();
        _mUiText.text = "No Parent";
        _mState = PLAYER_STATES.IDLE;
        _mStillTouchingFloor = false;
    }

    
    void FixedUpdate()
    {
        if (!IsTouchingFloor())
            _mStillTouchingFloor = false;

        _mArtificialVelocity *= (_mArtificalVelocityMagDegredation * Time.deltaTime);

        if (!_mStillTouchingFloor /* && DistanceToFloor() > 0.25f*/)
        {
            Debug.Log("FallingA\n"); // falling
            _mState = PLAYER_STATES.AIRBOURNE;
            _mRigidBD.AddForce(_mArtificialVelocity);
        }
        else if (Input.GetKey(KeyCode.Space) && IsTouchingFloor())
        {
            Debug.Log("JumpingB\n"); // jumping
            _mState = PLAYER_STATES.AIRBOURNE;
            _mRigidBD.AddForce(Vector3.up * _mJumpStreangth);
            _mRigidBD.AddForce(_mArtificialVelocity);
        }
        else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Debug.Log("Running\n"); // running
                _mState = PLAYER_STATES.RUN;
                _mArtificialVelocity = FlattenMovementVectorAgainstFloor(CalculateHorizontalMovementVecotor(_mRunningSpeed));
            }
            else
            {
                Debug.Log("Walking\n"); // walking
                _mState = PLAYER_STATES.WALK;
                _mArtificialVelocity = FlattenMovementVectorAgainstFloor(CalculateHorizontalMovementVecotor(_mWalkingSpeed));
            }
        }
        else
        {
            Debug.Log("Idle\n"); // idleing
            _mState = PLAYER_STATES.IDLE;
            _mArtificialVelocity *= ((_mArtificalVelocityMagDegredation * 0.9f) * Time.deltaTime);
        }

        if (_mState.Equals(PLAYER_STATES.AIRBOURNE))
        {
            Debug.Log("lol\n");
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

        UpdateStateMaterial();

        _mRigidBD.MovePosition(transform.position + _mArtificialVelocity);
    }


    Vector3 CalculateHorizontalMovementVecotor(float speed)
    {
        Vector3 direction = new Vector3();
        direction += (_mCameraOrientation.forward * (speed * Input.GetAxis("Vertical"))) * Time.deltaTime;
        direction += (_mCameraOrientation.right * (speed * Input.GetAxis("Horizontal"))) * Time.deltaTime;
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
        return Physics.Raycast(startPos, Vector3.down, 0.35f);
    }

    RaycastHit GetFloor()
    {
        RaycastHit returnInfo;
        Vector3 startPos = transform.position;
        startPos.y += 0.25f;
        Ray ray = new Ray(startPos, Vector3.down);

        Physics.Raycast(ray, out returnInfo, 0.45f);
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


    private void UpdateStateMaterial()
    {
        switch (_mState)
        {
            case PLAYER_STATES.IDLE:
                {
                    _mRend.material = _mIdleStateMat;
                    break;
                }
            case PLAYER_STATES.WALK:
                {
                    _mRend.material = _mWalkStateMat;
                    break;
                }
            case PLAYER_STATES.RUN:
                {
                    _mRend.material = _mRunStateMat;
                    break;
                }
            case PLAYER_STATES.AIRBOURNE:
                {
                    _mRend.material = _mAirbourneStateMat;
                    break;
                }
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        if (_mState != PLAYER_STATES.AIRBOURNE && GetFloor().distance > 0.28f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - (GetFloor().distance - 0.28f), transform.position.z);
        }
        _mStillTouchingFloor = true;
    }
}