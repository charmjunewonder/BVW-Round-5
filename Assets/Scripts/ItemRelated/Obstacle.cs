using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

	public float obstaclePosition;
	public int modeOfCharacter;

	private CharacterLeftFeedback leftFeedback;
	private ItemGenerator itemGenerator;
	private Controller controller;
	private SoundManager sm;
	// Use this for initialization
	void Start () {
		itemGenerator = GameObject.Find ("ItemsGenerator").GetComponent<ItemGenerator> ();
		controller = GameObject.Find ("Character1").GetComponent<Controller> ();
		leftFeedback = GameObject.Find ("LeftFeedback1").GetComponent<CharacterLeftFeedback> ();
		sm = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		modeOfCharacter = controller.characterMode;
		CheckValid ();
	}


	void CheckValid(){

		float pathPositionOfCharacter = controller.pathPosition;
		if(pathPositionOfCharacter - obstaclePosition > 0.005f)
		{
			if(itemGenerator.obstacleQueue.Count > 0)
			{
				itemGenerator.obstacleQueue.Dequeue();
			}
			itemGenerator.obstacleCount--;
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "RunMan") {
			controller.SetVelocity(controller.GetVelocity() / 3);
			itemGenerator.obstacleCount--;
			leftFeedback.showBadFeedback();
			if(itemGenerator.obstacleQueue.Count > 0)
			{
				itemGenerator.obstacleQueue.Dequeue();
			}
			Destroy(gameObject);

			sm.PlayVoiceEffect(modeOfCharacter, 0, false);
		}
	}
}
