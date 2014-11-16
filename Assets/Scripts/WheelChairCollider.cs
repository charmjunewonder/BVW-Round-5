using UnityEngine;
using System.Collections;

public class WheelChairCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "Senior"){
			Debug.Log("sit");
			other.gameObject.transform.parent.gameObject.GetComponent<Controller>().seniorSit();
			//renderer.enabled = false;
			Destroy(gameObject);
		}
	}
}
