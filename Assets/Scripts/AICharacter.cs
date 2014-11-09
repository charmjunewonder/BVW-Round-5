using UnityEngine;
using System.Collections;

public class AICharacter : MonoBehaviour {

	public Transform[] controlPath;
	public Transform character;
	public Transform camera;
	public int collectedItemCount = 0;
	public GameObject[] models;
	public int characterMode = 0;

	public float pathPosition=0.001f;
	public float pathOffset = 0;

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

	private Animator animator;

	private bool walkable = false;
	private float waitingCount = 0;
	private bool lookingBack = false;
	private Vector3 offsetVector;
	void OnDrawGizmos(){
		iTween.DrawPath(controlPath,Color.blue);	
	}	
	
	
	void Start(){
		//set the model of the character
		characterMode = 0;
		models[0].SetActive(true);
		animator = models[0].GetComponent<Animator>();

		velocityUpperBounds = new float[4];
		velocityUpperBounds [0] = 0.0001f;

		previousNormal = Vector3.up;
		//plop the character pieces in the "Ignore Raycast" layer so we don't have false raycast data:	
		foreach (Transform child in character) {
			child.gameObject.layer=2;
		}

		normals = new Queue ();
		StartCoroutine(look());
	}

	IEnumerator look(){
		animator.SetTrigger("Look");
		yield return new WaitForSeconds(1.5f);
		walkable = true;
	}
	
	
	void Update(){
		DetectKeys();
		rigidbody.WakeUp ();
		FindFloorAndRotation();
		MoveCharacter();
		//CheckCollectedItemCount();
	}
	
	
	void DetectKeys(){

		if(velocity <= velocityUpperBounds[0])
		velocity += velocityIncrement * Time.deltaTime;
		waitingCount = 0;

		velocity = Mathf.Clamp(velocity - velocityDecrement * Time.deltaTime, 0, 1f);
		pathPosition += velocity;
		animator.SetFloat("Speed", velocity);

	}

	IEnumerator Jump(){
		float jumpIncrement = jumpForce/10;
		for(int i = 0; i < 10; i++){
			ySpeed += jumpIncrement;
			yield return new WaitForSeconds(0.02f);
		}
		for(int i = 0; i < 10; i++){
			ySpeed -= jumpIncrement;
			yield return new WaitForSeconds(0.02f);
		}
		ySpeed = Mathf.Clamp(ySpeed, 0, jumpForce+1);	
		jumpState=0;
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
				offsetVector = Vector3.Cross(previousNormal, diretion);
			}
		}
	}
	
	
	void MoveCharacter(){
		prevPosition = character.position;
		// set offset for each player
		character.position = floorPosition + previousNormal.normalized * ySpeed
			+ offsetVector.normalized * pathOffset;// * 0.2f + prevPosition * 0.8f;
		//Debug.Log (character.transform.position);

	}

	void CheckCollectedItemCount(){
		if(collectedItemCount >= 4){
			collectedItemCount = 0;
			models[characterMode].SetActive(false);

			//destroy previous items
			// GameObject[] items = GameObject.FindGameObjectsWithTag(tagsForItems[characterMode]);
			// int num = items.Length;
			// for(int i = 0; i < num; i++){
			// 	Destroy(items[i]);
			// }
			characterMode = ++characterMode % 4;
			models[characterMode].SetActive(true);
			animator = models[characterMode].GetComponent<Animator>();

		}
	}

	public Vector3 GetPositionWithPercent(float percent){
		return iTween.PointOnPath(controlPath,percent);
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
