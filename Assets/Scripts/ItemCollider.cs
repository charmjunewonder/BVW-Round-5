using UnityEngine;
using System.Collections;

public class ItemCollider : MonoBehaviour {
	public Controller controller;
	public float pathPosition;
	void Start(){
		StartCoroutine(CheckValid());
	}

	IEnumerator CheckValid(){
		while(true){
			float pathPositionOfCharacter = controller.pathPosition;
			if(pathPositionOfCharacter - pathPosition > 0.5f){
				Destroy(gameObject);
			}
			yield return new WaitForSeconds(1.0f);
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "RunMan"){
			other.gameObject.GetComponent<Controller>().collectedItemCount++;
			Destroy(gameObject);
			Debug.Log(other.gameObject.GetComponent<Controller>().collectedItemCount);
		}
	}
}
