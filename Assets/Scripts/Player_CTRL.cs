using UnityEngine;
using System.Collections;

public class Player_CTRL : MonoBehaviour
{
    //bello!
    [SerializeField]
    private Transform _cameraPivot;
    [SerializeField]
    private GameObject _playerAvatar;
    private Vector3 _cameraForward;
    private Vector3 _cameraRight;

    private Rigidbody _rigidBD;
    private Animator _animCTRL;

    private bool _IsWalking = false;
    private bool _IsRunning = false;
    public bool _Ascending = false;
    public bool _Decending = false;
    private bool _IsJumping = false;

    public float _walkSpeed;
    public float _runSpeed;
    public float _jumpForce;

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

        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _IsRunning = true;
                _IsWalking = false;
                CalculateHorizontalMovement(_runSpeed);
            }
            else
            {
                _IsRunning = false;
                _IsWalking = true;
                CalculateHorizontalMovement(_walkSpeed);
            }
        }
        else
        {
            _IsRunning = false;
            _IsWalking = false;
        }

        if (!_IsJumping && Input.GetKey(KeyCode.Space))
        {
            _rigidBD.AddForce(Vector3.up * _jumpForce);
            _IsJumping = true;
        }

        UpdateAnimCTRL();
    }

    void CalculateHorizontalMovement(float speed)
    {
        Vector3 moveTo = new Vector3();

        moveTo += speed * (Input.GetAxis("Vertical") * _cameraForward);
        moveTo += speed * (Input.GetAxis("Horizontal") * _cameraRight);

        moveTo *= Time.deltaTime;
        //transform.Translate(moveTo);
        //_rigidBD.AddForce(moveTo);
        _rigidBD.MovePosition(transform.position + moveTo);
        TurnPlayerAvatar(moveTo);
    }

    void UpdateAnimCTRL()
    {
        _Ascending = false;
        _Decending = false;

        Ray floorCheck = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (!Physics.Raycast(floorCheck, out hit, 0.5f))
        {
            if (_rigidBD.velocity.y > 0.001f)
                _Ascending = true;
            else if (_rigidBD.velocity.y < -0.001f)
                _Decending = true;
        }

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

    void OnCollisionEnter(Collision other)
    {
        if (_IsJumping && other.gameObject.tag == "Floor")
        {
            _IsJumping = false;
        }
    }
}
