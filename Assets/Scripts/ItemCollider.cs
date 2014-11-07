using UnityEngine;
using System.Collections;

public class ItemCollider : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "RunMan"){
			other.gameObject.GetComponent<Controller>().collectedItemCount++;
			Destroy(gameObject);
			Debug.Log(other.gameObject.GetComponent<Controller>().collectedItemCount);
		}
	}
}
