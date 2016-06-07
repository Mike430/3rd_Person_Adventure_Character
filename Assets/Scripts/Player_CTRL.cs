using UnityEngine;
using System.Collections;

public class Player_CTRL : MonoBehaviour
{

    [SerializeField]
    private Targeting_System_SCR _crossHair;
    [SerializeField]
    private Line_SCR _ropeLine;
    private SpringJoint _rope = null;

    [SerializeField]
    private Transform _cameraPivot;
    [SerializeField]
    private GameObject _playerAvatar;

    private Vector3 _cameraForward;
    private Vector3 _cameraRight;
    private Vector3 _moveToXZ;

    private GameObject _target;
    private Rigidbody _rigidBD;
    private Animator _animCTRL;

    private bool _IsWalking = false;
    private bool _IsRunning = false;
    private bool _Ascending = false;
    private bool _Decending = false;
    private bool _IsJumping = false;
    private bool _IsAirborn = false;
    private bool _IsSwinging = false;

    public float _walkSpeed;
    public float _runSpeed;
    public float _jumpForce;
    public float _swingForce;

    // Use this for initialization
    void Start ()
    {
        _rigidBD = GetComponent<Rigidbody>();
        _animCTRL = GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        _cameraForward = _cameraPivot.forward;
        _cameraRight = _cameraPivot.right;

        TestForFalling();

        if (!_IsAirborn)
        {
            _IsRunning = false;
            _IsWalking = false;

            if ((Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) && !_IsAirborn)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    _IsRunning = true;
                    CalculateHorizontalMovement(_runSpeed);
                }
                else
                {
                    _IsWalking = true;
                    CalculateHorizontalMovement(_walkSpeed);
                }
            }

            if (!_IsJumping && Input.GetKey(KeyCode.Space))
            {
                _rigidBD.AddForce(Vector3.up * _jumpForce);
                _IsJumping = true;
            }
        }
        else
        {
            _IsRunning = false;
            _IsWalking = false;
        }

        UpdateRope();

        UpdateAnimCTRL();
    }

    void CalculateHorizontalMovement(float speed)
    {
        _moveToXZ += speed * (Input.GetAxis("Vertical") * _cameraForward);
        _moveToXZ += speed * (Input.GetAxis("Horizontal") * _cameraRight);

        _moveToXZ *= Time.deltaTime;
        //transform.Translate(moveTo);
        //_rigidBD.AddForce(moveTo);
        MovePlayerXZ();
    }

    void UpdateRope()
    {
        if (Input.GetAxis("Fire1") != 0)
        {
            if (_crossHair.FoundTarget() && _target == null)
            {
                _target = _crossHair.GetTarget();
                _ropeLine._posA = _target.transform;
            }

            if (_target != null && _Decending)
            {
                if (_rope == null)
                {
                    _IsSwinging = true;
                    _rigidBD.drag = 0.0f;
                    Debug.Log("Entered create rope");
                    this.gameObject.AddComponent<SpringJoint>();
                    _rope = this.GetComponent<SpringJoint>();

                    _rope.autoConfigureConnectedAnchor = false;

                    _rope.connectedBody = _target.GetComponent<Rigidbody>();
                    _rope.anchor = new Vector3(0.0f, 0.32f, 0.0f); //_rope.anchor.Set(0.0f, 0.32f, 0.0f);
                    _rope.connectedAnchor = new Vector3(0.0f, -0.5f, 0.0f); //_rope.connectedAnchor.Set(0.0f, -0.5f, 0.0f);

                    Vector3 difference = _target.transform.position - transform.position;
                    float distance = Vector3.Magnitude(difference);
                    _rope.maxDistance = distance;
                    _rope.minDistance = 0.75f;

                    _rope.spring = 1000.0f;
                    _rope.damper = 0.2f;
                    _rope.tolerance = 0.025f;
                    _rope.breakForce = Mathf.Infinity;
                    _rope.breakTorque = Mathf.Infinity;
                }
            }
        }
        else
        {
            _IsSwinging = false;
            _rigidBD.drag = 0.5f;
            _target = null;
            _ropeLine._posA = transform;
            Destroy(_rope);
            _rope = null;
        }

        if (_IsSwinging)
        {
            if (Input.GetKey(KeyCode.Q) && _rope.maxDistance > 0.75f)
                _rope.maxDistance -= 5.0f * Time.deltaTime;

            if (Input.GetKey(KeyCode.E) && _rope.maxDistance < 100.0f)
                _rope.maxDistance += 5.0f * Time.deltaTime;

            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
                CalculateHorizontalMovement(_swingForce);
        }
    }

    void UpdateAnimCTRL()
    {
        _animCTRL.SetBool("Ascending", _Ascending);
        _animCTRL.SetBool("Decending", _Decending);
        _animCTRL.SetBool("IsWalking", _IsWalking);
        _animCTRL.SetBool("IsRunning", _IsRunning);
    }

    void TurnPlayerAvatar(Vector3 heading)
    {
        // http://docs.unity3d.com/ScriptReference/Vector3.RotateTowards.html
        float step = 10 * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(_playerAvatar.transform.forward, heading, step, 0.0F);
        Debug.DrawRay(_playerAvatar.transform.position, newDir, Color.red);
        _playerAvatar.transform.rotation = Quaternion.LookRotation(newDir);

        /*_playerAvatar.transform.rotation = Quaternion.Lerp(_playerAvatar.transform.rotation,
            _cameraPivot.transform.rotation, Time.deltaTime * 10);*/
    }

    void TestForFalling()
    {
        _Ascending = false;
        _Decending = false;

        Vector3 startPos = transform.position;
        startPos.y += 0.25f; // When Player hits the ground, he can intersect it, pushing the origin through the floor, the ray cast is lifted up slightly so that it always intersects.
        _IsAirborn = !Physics.Raycast(startPos, Vector3.down, 0.35f);

        if (_IsAirborn)
        {
            if (_rigidBD.velocity.y > 0.001f)
                _Ascending = true;
            else if (_rigidBD.velocity.y < -0.001f)
                _Decending = true;

            MovePlayerXZ();
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (_IsJumping && other.gameObject.tag == "Floor")
        {
            _IsJumping = false;
            
        }
    }

    void MovePlayerXZ()
    {
        if (!_IsAirborn)
        {
            _rigidBD.MovePosition(transform.position + _moveToXZ);
            TurnPlayerAvatar(_moveToXZ);
        }
        else if (_IsAirborn && !_IsSwinging)
            _rigidBD.MovePosition(transform.position + _moveToXZ);
        else if (_IsSwinging)
        {
            _rigidBD.AddForce(_moveToXZ * 5);
            TurnPlayerAvatar(_moveToXZ);
        }
    }
}
