using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {
	public Controller controller;
	public float itemPosition;

	private LifeProgressBar lifeBar;

	void Start(){
		controller = GameObject.Find ("Character1").GetComponent<Controller> ();
		lifeBar = GameObject.Find ("LifeProgressBar").GetComponent<LifeProgressBar> ();
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
				ItemGenerator.itemCount--;
			}
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "RunMan"){
			other.gameObject.GetComponent<Controller>().collectedItemCount++;
			lifeBar.changeToNextState();
			Destroy(gameObject);
			ItemGenerator.itemCount--;
		}
	}


}
