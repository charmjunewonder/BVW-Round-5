using UnityEngine;
using System.Collections;

public class FallingTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "RunMan"){
			Debug.Log("DroppingTrigger");
			other.gameObject.GetComponent<Controller>().DropWithGravity();
		}
	}
}
