using UnityEngine;
using System.Collections;

public class mover : MonoBehaviour {
	public float _speed;
	// Update is called once per frame
	void Update ()
    {
        // forward and backwards
        if (Input.GetKey(KeyCode.I))
            transform.Translate(0.0f, _speed * Time.deltaTime, 0.0f);
        if (Input.GetKey(KeyCode.K))
            transform.Translate(0.0f, -_speed * Time.deltaTime, 0.0f);

        // right and left
        if (Input.GetKey(KeyCode.L))
            transform.Translate(_speed * Time.deltaTime, 0.0f, 0.0f);
        if (Input.GetKey(KeyCode.J))
            transform.Translate(-_speed * Time.deltaTime, 0.0f, 0.0f);

        // up and down
        if (Input.GetKey(KeyCode.U))
            transform.Translate(0.0f, 0.0f, _speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.O))
            transform.Translate(0.0f, 0.0f, -_speed * Time.deltaTime);
    }
}
