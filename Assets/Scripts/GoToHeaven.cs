using UnityEngine;
using System.Collections;

public class GoToHeaven : MonoBehaviour {

	public GameObject Destination;
	public bool Go;
	// Use this for initialization
	void Start () {
		Go = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Go) {
			Debug.Log ("In GOTOHEAVEN velocity" + transform.rigidbody.velocity);
			Vector3 v =  (Destination.transform.position - transform.position).normalized * 10;
			transform.position += v;
			if(transform.position.y >= Destination.transform.position.y)
			{
				Go = false;
			}
		}
	}
}
