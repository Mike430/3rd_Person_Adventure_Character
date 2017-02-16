using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPlayerMovement : MonoBehaviour {

    Rigidbody _mRigidBody;
    [SerializeField]
    float _mMoveSpeed = 3f;

	// Use this for initialization
	void Start () {
        _mRigidBody = this.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.I))
        {
            _mRigidBody.MovePosition(new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z + (_mMoveSpeed * Time.deltaTime)));
        }
        if (Input.GetKey(KeyCode.K))
        {
            _mRigidBody.MovePosition(new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z + (-_mMoveSpeed * Time.deltaTime)));
        }
        if (Input.GetKey(KeyCode.J))
        {
            _mRigidBody.MovePosition(new Vector3(transform.position.x + (_mMoveSpeed * Time.deltaTime), transform.position.y + 0.01f, transform.position.z));
        }
        if (Input.GetKey(KeyCode.L))
        {
            _mRigidBody.MovePosition(new Vector3(transform.position.x + (-_mMoveSpeed * Time.deltaTime), transform.position.y + 0.01f, transform.position.z));
        }
    }
}
