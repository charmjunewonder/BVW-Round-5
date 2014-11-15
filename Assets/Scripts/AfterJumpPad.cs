using UnityEngine;
using System.Collections;

public class AfterJumpPad : MonoBehaviour {

	public int modeOfCharacter;

	void Start(){
	}

	void Update()
	{

	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "RunMan"){
			Debug.Log("fsdlk");
			other.gameObject.GetComponent<Controller>().SetIsJumpingFalse();
		}
	}
}
