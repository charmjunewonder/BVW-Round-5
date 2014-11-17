using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	public float itemPosition;
	public int modeOfCharacter;
	
	private CharacterFeedback feedback;
	private ItemGenerator itemGenerator;
	private Controller controller;
	//private LifeProgressBar lifeBar;

	void Start(){
		itemGenerator = GameObject.Find ("ItemsGenerator").GetComponent<ItemGenerator> ();
		controller = GameObject.Find ("Character1").GetComponent<Controller> ();
		//lifeBar = GameObject.Find ("LifeProgressBar").GetComponent<LifeProgressBar> ();
		feedback = GameObject.Find ("Feedback1").GetComponent<CharacterFeedback> ();
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

			itemGenerator.itemCount--;
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "RunMan"){
			other.gameObject.GetComponent<Controller>().collectedItemCount++;
			//lifeBar.changeToNextState();
			feedback.showFeedbackNumber(other.gameObject.GetComponent<Controller>().collectedItemCount);
			if(itemGenerator.itemQueue.Count > 0)
			{
				itemGenerator.itemQueue.Dequeue();
			}

			itemGenerator.itemCount--;
			if(modeOfCharacter == 0)
			{
				GameObject.Find ("SoundManager").GetComponent<SoundManager>().PlaySoundEffect(0, false);
			}
			Destroy(gameObject);
		}
	}


}
