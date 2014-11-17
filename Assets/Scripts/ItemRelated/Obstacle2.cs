using UnityEngine;
using System.Collections;

public class Obstacle2 : MonoBehaviour {
	
	public float obstaclePosition;
	public int modeOfCharacter;

	private ItemGenerator2 itemGenerator;
	private CharacterLeftFeedback leftFeedback;
	private Controller controller;
	private SoundManager sm;
	// Use this for initialization
	void Start () {
		itemGenerator = GameObject.Find ("ItemsGenerator2").GetComponent<ItemGenerator2> ();
		controller = GameObject.Find ("Character2").GetComponent<Controller> ();
		leftFeedback = GameObject.Find ("LeftFeedback2").GetComponent<CharacterLeftFeedback> ();
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

			sm.PlayVoiceEffect(modeOfCharacter, 1, false);
		}
	}
}
