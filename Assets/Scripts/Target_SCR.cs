using UnityEngine;
using System.Collections;

public class Target_SCR : MonoBehaviour {

    [SerializeField]
    private Material _untargeted;
    [SerializeField]
    private Material _targeted;

    private bool _isTargeted;
    private Renderer _rend;

	// Use this for initialization
	void Start ()
    {
        _rend = GetComponent<Renderer>();
        _rend.enabled = true;

        _isTargeted = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (_isTargeted)
            _rend.sharedMaterial = _targeted;
        else
            _rend.sharedMaterial = _untargeted;

    }

    public void Targeted(bool isTargeted)
    {
        _isTargeted = isTargeted;
    }
}
