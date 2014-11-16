using UnityEngine;
using System.Collections;

public class FallingExit : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "RunMan"){
			other.gameObject.GetComponent<Controller>().DroppingExit();
		}
	}
}
