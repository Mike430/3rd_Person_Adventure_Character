using UnityEngine;
using System.Collections;

public class Line_SCR : MonoBehaviour {

    public LineRenderer _lnRenderer;
    public Transform _posA;
    public Transform _posB;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        _lnRenderer.SetPosition(0, _posA.position);
        _lnRenderer.SetPosition(1, _posB.position);
    }
}
