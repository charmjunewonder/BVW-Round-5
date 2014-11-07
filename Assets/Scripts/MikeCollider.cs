using UnityEngine;
using System.Collections;

public class MikeCollider : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "RunMan"){
			other.gameObject.GetComponent<Controller>().collectedItemCount++;
			Debug.Log(other.gameObject.GetComponent<Controller>().collectedItemCount);
		}
	}
}
