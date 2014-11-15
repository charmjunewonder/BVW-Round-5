using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {
	public Controller controller;
	public float itemPosition;

	private ItemGenerator itemGenerator;
	public int modeOfCharacter;
	private LifeProgressBar lifeBar;

	void Start(){
		itemGenerator = GameObject.Find ("ItemsGenerator").GetComponent<ItemGenerator> ();
		controller = GameObject.Find ("Character1").GetComponent<Controller> ();
		lifeBar = GameObject.Find ("LifeProgressBar").GetComponent<LifeProgressBar> ();
	}

	void Update()
	{
		if (modeOfCharacter > 0) {
			transform.Rotate (0, -3, 0, Space.World);
		}
		else
		{
			transform.Rotate (0, 3, 0, Space.World);
		}
		CheckValid ();
	}

	void CheckValid(){

		float pathPositionOfCharacter = controller.pathPosition;
		if(pathPositionOfCharacter - itemPosition > 0.005f){
			if(itemGenerator.itemQueue.Count > 0)
			{
				itemGenerator.itemQueue.Dequeue();
			}

			ItemGenerator.itemCount--;
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "RunMan"){
			other.gameObject.GetComponent<Controller>().collectedItemCount++;
			lifeBar.changeToNextState();
			if(itemGenerator.itemQueue.Count > 0)
			{
				itemGenerator.itemQueue.Dequeue();
			}

			ItemGenerator.itemCount--;
			if(modeOfCharacter == 0)
			{
				controller.soundEffectPlayer.clip = controller.soundEffects[0];
				controller.soundEffectPlayer.Play();
			}
			Destroy(gameObject);
		}
	}


}
