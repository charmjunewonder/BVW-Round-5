﻿using UnityEngine;
using System.Collections;

public class Item2 : MonoBehaviour {

	public float itemPosition;
	public int modeOfCharacter;

	private CharacterFeedback feedback;	
	private ItemGenerator2 itemGenerator;
	private Controller controller;
	private SoundManager sm;
	//private LifeProgressBar lifeBar;
	
	void Start(){
		itemGenerator = GameObject.Find ("ItemsGenerator2").GetComponent<ItemGenerator2> ();
		controller = GameObject.Find ("Character2").GetComponent<Controller> ();
		//lifeBar = GameObject.Find ("LifeProgressBar").GetComponent<LifeProgressBar> ();
		feedback = GameObject.Find ("Feedback2").GetComponent<CharacterFeedback> ();
		sm = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();
	}
	
	void Update()
	{
		modeOfCharacter = controller.characterMode;
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
			Destroy(gameObject);
			sm.PlayVoiceEffect(modeOfCharacter, 1, true);
		}
	}
}
