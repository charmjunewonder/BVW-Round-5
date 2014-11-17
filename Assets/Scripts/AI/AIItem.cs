using UnityEngine;
using System.Collections;

public class AIItem : MonoBehaviour {

	public AICharacter controller;
	public float itemPosition;
	public GameObject[] models;

	private LifeProgressBar lifeBar;

	void Start(){
		//lifeBar = GameObject.Find ("LifeProgressBar").GetComponent<LifeProgressBar> ();
		controller = GameObject.Find ("AICharacter1").GetComponent<AICharacter> ();
	}

	void Update()
	{
		transform.Rotate (0, 3f, 0, Space.Self);
		CheckValid ();
	}

	void CheckValid(){

		if(true){
			float pathPositionOfCharacter = controller.pathPosition;
			if(pathPositionOfCharacter - itemPosition > 0.001f){
				Destroy(gameObject);
				//ItemGenerator.itemCount--;
			}
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "RunMan"){
			other.gameObject.GetComponent<AICharacter>().collectedItemCount++;
			Destroy(gameObject);
			//AIItemGenerator.itemCount--;
		}
	}
}
