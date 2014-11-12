using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {
	public Controller controller;
	public float itemPosition;

	public int modeOfCharacter;
	private LifeProgressBar lifeBar;

	void Start(){
		controller = GameObject.Find ("Character1").GetComponent<Controller> ();
		lifeBar = GameObject.Find ("LifeProgressBar").GetComponent<LifeProgressBar> ();
	}

	void Update()
	{
		if (modeOfCharacter > 0) {
			transform.Rotate (0, 0, -3, Space.Self);
		}
		else
		{
			transform.Rotate (0, 3, 0, Space.Self);
		}
		CheckValid ();
	}

	void CheckValid(){

		float pathPositionOfCharacter = controller.pathPosition;
		if(pathPositionOfCharacter - itemPosition > 0.005f){

			Destroy(gameObject);
			ItemGenerator.itemCount--;

		}
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "RunMan"){
			other.gameObject.GetComponent<Controller>().collectedItemCount++;
			lifeBar.changeToNextState();
			Destroy(gameObject);
			ItemGenerator.itemCount--;
			if(modeOfCharacter == 0)
			{
				controller.soundEffectPlayer.clip = controller.soundEffects[0];
				controller.soundEffectPlayer.Play();
			}
		}
	}


}
