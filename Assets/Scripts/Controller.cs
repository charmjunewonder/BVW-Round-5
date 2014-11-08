//#define DEBUG
using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {

	public Transform[] controlPath;
	public Transform character;
	public Transform camera;
	public int collectedItemCount = 0;
	public GameObject[] models;
	public int characterMode = 0;
	public string[] tagsForItems;// = {"MilkItem", "BookItem", "MoneyItem", "PillItem"};

	public float pathPosition=0.001f;
	
	private RaycastHit hit;
	private float rayLength = 100;
	private Vector3 floorPosition;	
	private float lookAheadAmount = .001f;
	private float ySpeed=0;
	private float gravity=.1f;
	private float jumpForce=2.15f;
	private uint jumpState=0; //0=grounded 1=jumping
	private Vector3 previousNormal;
	private float velocity = 0;
	private float velocityDecrement = 0.0001f;
	private float velocityIncrement = 0.003f;

	private float[] velocityUpperBounds;
	//private float cameraOffset = 0.01f;
	private Vector3 previousPosition;

	private Vector3 prevPosition;

	private Queue normals;

	void OnDrawGizmos(){
		iTween.DrawPath(controlPath,Color.blue);	
	}	
	
	
	void Start(){
		//set the model of the character
		characterMode = 0;
		models[0].SetActive(true);
		tagsForItems = new string[4];
		tagsForItems[0] = "MilkItem";
		tagsForItems[1] = "BookItem";
		tagsForItems[2] = "MoneyItem";
		tagsForItems[3] = "PillItem";

		velocityUpperBounds = new float[4];
		velocityUpperBounds [0] = 0.0001f;

		previousNormal = Vector3.up;
		//plop the character pieces in the "Ignore Raycast" layer so we don't have false raycast data:	
		foreach (Transform child in character) {
			child.gameObject.layer=2;
		}

		normals = new Queue ();

	}
	
	
	void Update(){
		rigidbody.WakeUp ();
		DetectKeys();
		FindFloorAndRotation();
		MoveCharacter();
		CheckCollectedItemCount();
	}
	
	
	void DetectKeys(){

		if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
			if(velocity <= velocityUpperBounds[0])
				velocity += velocityIncrement * Time.deltaTime;
		}
		velocity = Mathf.Clamp(velocity - velocityDecrement * Time.deltaTime, 0, 1f);
		pathPosition += velocity;

		//jump:
		if (Input.GetKeyDown("space") && jumpState==0) {
			ySpeed+=jumpForce;
			jumpState=1;
		}
	}
	
	
	void FindFloorAndRotation(){
		float pathPercent = pathPosition%1f;
		Vector3 coordinateOnPath = iTween.PointOnPath(controlPath,pathPercent);
		Vector3 lookTarget;
		Vector3 diretion;
		lookTarget = iTween.PointOnPath(controlPath,pathPercent+lookAheadAmount);
		diretion = lookTarget - coordinateOnPath;

		int layerOfPath = 1 << 8;
//
		if (Physics.Raycast(coordinateOnPath,-previousNormal,out hit, rayLength, layerOfPath)){
			previousNormal = GetNormal(hit.normal);

			if(Vector3.Distance(previousPosition, hit.point) > 0.5f){
				floorPosition=hit.point;
				character.transform.LookAt(transform.position + diretion.normalized, previousNormal);

			}
		}
	}
	
	
	void MoveCharacter(){
		prevPosition = character.position;
		character.position = (floorPosition + previousNormal.normalized * ySpeed);// * 0.2f + prevPosition * 0.8f;

		//Debug.Log (character.transform.position);

		ySpeed -=gravity;
		ySpeed = Mathf.Clamp(ySpeed, 0, jumpForce+1);
		jumpState=0;
	
	}

	void CheckCollectedItemCount(){
		if(collectedItemCount >= 5){
			collectedItemCount = 0;
			models[characterMode].SetActive(false);

			//destroy previous items
			GameObject[] items = GameObject.FindGameObjectsWithTag(tagsForItems[characterMode]);
			int num = items.Length;
			for(int i = 0; i < num; i++){
				Destroy(items[i]);
			}
			characterMode = ++characterMode % 4;
			models[characterMode].SetActive(true);


		}
	}

	public Vector3 GetPositionAheadWithOffset(float offset){
		float pathPercent = (pathPosition+offset)%1;
		return iTween.PointOnPath(controlPath,pathPercent);
	}

	private Vector3 GetNormal(Vector3 v)
	{
		normals.Enqueue(v);
		if(normals.Count >= 20)
		{
			normals.Dequeue();
		}

		Vector3 average = new Vector3(0, 0, 0);
		foreach(Vector3 temp in normals)
		{
			average += temp;
		}
		average /= normals.Count;
		return average;
	}

}