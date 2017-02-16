using UnityEngine;
using System.Collections;

public class MovingPlatforms : MonoBehaviour {
    // do what
    [SerializeField]
    private bool _move = true;
    //[SerializeField]
    //private bool _spin = false;

    // which axis for spinning
    [SerializeField]
    private bool _moveX = true;
    [SerializeField]
    private bool _moveY = false;
    [SerializeField]
    private bool _moveZ = false;
    // which axis for spinning
    /*
    [SerializeField]
    private bool _spinX = false;
    [SerializeField]
    private bool _spinY = false;
    [SerializeField]
    private bool _spinZ = false;
    */
    // flip motion
    [SerializeField]
    private bool _invertDir = false;


    [SerializeField]
    private float _distance = 20.0f;
    [SerializeField]
    private float _speed = 1.0f;// m/s

    private Vector3 _startPos;
    private Vector3 _endPos;

    [SerializeField]
    private float _sleepDuration;
    private float _currentSleepDuration;
    private bool _timeToSleep;
    private bool _towardsEndpos = true;
    private float _journeyStartTime;

    private Rigidbody _mRigidBody;

    // Use this for initialization
    void Start () {
        if (_move)
        {
            _mRigidBody = this.GetComponent<Rigidbody>();
            _journeyStartTime = Time.time;
            _startPos = new Vector3();
            _endPos = new Vector3();

            _endPos.x = 0.0f;

            _startPos = transform.position;
            _endPos = transform.position;
            if (_moveX)
            {
                _endPos.x += _invertDir == true ? _distance : -_distance;
            }
            if (_moveY)
            {
                _endPos.y += _invertDir == true ? _distance : -_distance;
            }
            if (_moveZ)
            {
                _endPos.z += _invertDir == true ? _distance : -_distance;
            }
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (_timeToSleep)
        {
            _currentSleepDuration -= Time.deltaTime;
            if (_currentSleepDuration < 0)
            {
                _journeyStartTime = Time.time;
                _timeToSleep = false;
            }
            else
                return;
        }

        if (_towardsEndpos)
        {
            _mRigidBody.MovePosition(Vector3.Lerp(_startPos, _endPos, (Time.time - _journeyStartTime) * _speed));

            if (transform.position == _endPos)
            {
                _timeToSleep = true;
                _currentSleepDuration = _sleepDuration;
                _towardsEndpos = false;
                _journeyStartTime = Time.time;
            }
        }
        else
        {
            _mRigidBody.MovePosition(Vector3.Lerp(_endPos, _startPos, (Time.time - _journeyStartTime) * _speed));

            if (transform.position == _startPos)
            {
                _timeToSleep = true;
                _currentSleepDuration = _sleepDuration;
                _towardsEndpos = true;
                _journeyStartTime = Time.time;
            }
        }
	}
}
