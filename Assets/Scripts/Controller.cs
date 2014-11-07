using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
	public Transform[] controlPath;
	public Transform character;
	public Transform camera;

	private float pathPosition=0;
	private RaycastHit hit;
	private float rayLength = 100;
	private Vector3 floorPosition;	
	private float lookAheadAmount = .01f;
	private float ySpeed=0;
	private float gravity=.1f;
	private float jumpForce=2.15f;
	private uint jumpState=0; //0=grounded 1=jumping
	private Vector3 previousNormal;
	private float velocity = 0;
	private float velocityDecrement = 0.01f;
	private float velocityIncrement = 0.03f;
	private float cameraOffset = 0.01f;
	private Vector3 previousPosition;
	void OnDrawGizmos(){
		iTween.DrawPath(controlPath,Color.blue);	
	}	
	
	
	void Start(){
		previousNormal = Vector3.up;
		//plop the character pieces in the "Ignore Raycast" layer so we don't have false raycast data:	
		foreach (Transform child in character) {
			child.gameObject.layer=2;
		}
	}
	
	
	void Update(){
		DetectKeys();
		FindFloorAndRotation();
		MoveCharacter();
	}
	
	
	void DetectKeys(){
		//forward path movement:

		if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
			velocity += velocityIncrement;
		}

		velocity = Mathf.Clamp(velocity - velocityDecrement, 0, 0.5f);
		pathPosition += Time.deltaTime * velocity;

		//jump:
		if (Input.GetKeyDown("space") && jumpState==0) {
			ySpeed+=jumpForce;
			jumpState=1;
		}
	}
	
	
	void FindFloorAndRotation(){
		float pathPercent = pathPosition%1;
		Vector3 coordinateOnPath = iTween.PointOnPath(controlPath,pathPercent);
		Vector3 lookTarget;
		Vector3 diretion;
		lookTarget = iTween.PointOnPath(controlPath,pathPercent+lookAheadAmount);
		diretion = lookTarget - coordinateOnPath;
		
		int layerOfPath = 1 << 8;

		if (Physics.Raycast(coordinateOnPath,-previousNormal,out hit, rayLength, layerOfPath)){
			previousNormal = hit.normal;
			Debug.DrawRay(coordinateOnPath, -previousNormal * hit.distance);
			if(Vector3.Distance(previousPosition, hit.point) > 0.1f){
				floorPosition=hit.point;
				character.LookAt(transform.position + diretion.normalized, previousNormal);
			}
		}
	}
	
	
	void MoveCharacter(){
		character.position = floorPosition + previousNormal.normalized * ySpeed;
		previousPosition = character.position;
		ySpeed -=gravity;
		ySpeed = Mathf.Clamp(ySpeed, 0, jumpForce+1);
		jumpState=0;
	
	}

}