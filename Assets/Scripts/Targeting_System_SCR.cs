using UnityEngine;
using System.Collections;

public class Targeting_System_SCR : MonoBehaviour {

    private bool _targetFound = false;
    private GameObject _currentTarget = null;

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        RaycastHit hitInfo;
        Ray crossHair = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(crossHair, out hitInfo, 50.0f))
        {
            if (hitInfo.collider.gameObject.tag == "Target")
            {
                _targetFound = true;
                _currentTarget = hitInfo.collider.gameObject;
                _currentTarget.GetComponent<Target_SCR>().Targeted(true);
            }
            else
            {
                TargetNotFound();
            }
        }
        else
            TargetNotFound();
	}

    private void TargetNotFound()
    {
        if (_currentTarget != null)
            _currentTarget.GetComponent<Target_SCR>().Targeted(false);

        _targetFound = false;
        _currentTarget = null;
    }

    public bool FoundTarget()
    {
        return _targetFound;
    }

    public GameObject GetTarget()
    {
        if (_currentTarget != null)
            return _currentTarget;
        else
            return null;
    }
}
