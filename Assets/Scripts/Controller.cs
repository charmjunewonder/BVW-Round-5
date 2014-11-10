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
	public float pathPosition=0.001f;
	public float pathOffset = 0;
	public ParticleSystem TransitionEffect;
	public Texture[] speed;
	public Texture[] numbers;
	
	private RaycastHit hit;
	private float rayLength = 100;
	private Vector3 floorPosition;	
	private float lookAheadAmount = .001f;
	private float ySpeed=0;
	private float gravity=.1f;
	private float jumpForce=10f;
	private uint jumpState=0; //0=grounded 1=jumping
	private Vector3 previousNormal;
	private float velocity = 0;
	private float velocityDecrement = 0.0001f;
	private float velocityIncrement = 0.0002f;

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

	private float rotAngle = -90;

	void OnDrawGizmos(){
		iTween.DrawPath(controlPath,Color.blue);	
	}	
	
	
	void Start(){
		//set the model of the character
		characterMode = 0;
		models[0].SetActive(true);
		animator = models[0].GetComponent<Animator>();

		velocityUpperBounds = new float[4];
		velocityUpperBounds [0] = 0.00001f;
		velocityUpperBounds [1] = 0.00008f; 
		velocityUpperBounds [2] = 0.0003f;

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
		FindFloorAndRotation();
		MoveCharacter();
		CheckCollectedItemCount();
	}
	
	
	void DetectKeys(){
		if(walkable){
			if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
				velocity = Mathf.Clamp(velocity + velocityIncrement * Time.deltaTime, 0, velocityUpperBounds[characterMode % 4]);
				waitingCount = 0;
			} else{
				waitingCount += Time.deltaTime;
			}
		}
		if(characterMode == 0){
			if(waitingCount > 5){
				waitingCount = 0;
				walkable = false;
				StartCoroutine(LookBack());
			}
			if(lookingBack){
				if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
					waitingCount = 0;
					lookingBack = false;
					StartCoroutine(LookForward());
				}
			}
		} else{
			if(waitingCount > 5){
				waitingCount = 0;
				walkable = false;
				StartCoroutine(Idle());
			}
			//jump:
			if (Input.GetKeyDown("space") && jumpState==0) {
				animator.SetTrigger("Jump");
				StartCoroutine(Jump());
				jumpState=1;
			}
		}
		velocity = Mathf.Clamp(velocity - velocityDecrement * Time.deltaTime, 0, 1f);
		pathPosition += velocity;
		animator.SetFloat("Speed", velocity);

	}

	IEnumerator Idle(){
		animator.SetTrigger("Idle");
		yield return new WaitForSeconds(0.1f);
		walkable = true;
	}

	IEnumerator LookBack(){
		animator.SetTrigger("LookBack");
		yield return new WaitForSeconds(0.6f);
		lookingBack = true;
	}

	IEnumerator LookForward(){
		animator.SetTrigger("LookForward");
		yield return new WaitForSeconds(0.95f);
		walkable = true;
	}

	IEnumerator Jump(){
		float jumpIncrement = jumpForce/5;
		for(int i = 0; i < 10; i++){
			ySpeed += jumpIncrement;
			yield return new WaitForSeconds(0.01f);
		}
		yield return new WaitForSeconds(0.05f);
		for(int i = 0; i < 10; i++){
			ySpeed -= jumpIncrement;
			yield return new WaitForSeconds(0.01f);
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

			TransitionEffect.gameObject.SetActive(true);

			characterMode = ++characterMode % 4;
			animator = models[characterMode].GetComponent<Animator>();
			animator.SetTrigger("Idle");
			walkable = false;
			Invoke("SetNextModelActive", 1);
			Invoke("SetWalkableTrue", 1.7f);
		}
	}

	private void SetNextModelActive()
	{
		models[characterMode].SetActive(true);
	}

	private void SetWalkableTrue()
	{
		walkable = true;
	}

	public Vector3 GetPositionWithPercent(float percent){
		return iTween.PointOnPath(controlPath,percent);
	}

	public void ModifyLookAtDirection(GameObject other, float percent){
		Vector3 coordinateOnPath = iTween.PointOnPath(controlPath,percent);
		Vector3 lookTarget;
		Vector3 direction;
		lookTarget = iTween.PointOnPath(controlPath,percent+lookAheadAmount);
		direction = lookTarget - coordinateOnPath;

		int layerOfPath = 1 << 8;

		Vector3 directionA = new Vector3(-direction.y, direction.x, 0);
		float minDistance = Mathf.Infinity;
		Vector3 vectorWithMinDistance = directionA;
		Vector3 positionVector = other.transform.position;
		for(int i = 0; i < 8; i++){
			directionA = Quaternion.AngleAxis(-45, direction) * directionA;
			if (Physics.Raycast(coordinateOnPath,-directionA,out hit, 10.0f, layerOfPath)){

				if(hit.distance < minDistance){
					minDistance = hit.distance;
					vectorWithMinDistance = directionA;
					positionVector = hit.point;
					
				}
			}
		}
		for(int i = 0; i < 9; i++){
			directionA = Quaternion.AngleAxis(-45+10*i, direction) * vectorWithMinDistance;

			if (Physics.Raycast(coordinateOnPath,-directionA,out hit, 10.0f, layerOfPath)){

				if(hit.distance < minDistance){
					minDistance = hit.distance;
					vectorWithMinDistance = directionA;
					positionVector = hit.point;
				}
			}
		}
		other.transform.LookAt(other.transform.position + direction.normalized, vectorWithMinDistance);
		other.transform.position = positionVector;
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

	void OnGUI() {
		// GUI.DrawTexture(new Rect(Screen.width * 0.665f, Screen.height * 0.625f, 498*0.7f, 320*0.7f), speed[0]);

  //       Vector3 pivotPoint = new Vector2(Screen.width * 0.6912f+speed[1].width*0.65f, Screen.height * 0.76f+speed[1].height*0.65f/2);
  //       GUIUtility.RotateAroundPivot(rotAngle, pivotPoint);
  //       GUI.DrawTexture(new Rect(Screen.width * 0.6912f, Screen.height * 0.76f, speed[1].width*0.65f, speed[1].height*0.65f), speed[1]);
		rotAngle = -90 + velocity / (velocityUpperBounds[characterMode]- velocityDecrement * Time.deltaTime) * 90 * (characterMode+1);
		GUI.DrawTexture(new Rect(Screen.width * 0.665f, Screen.height * 0.625f, 498*0.7f, 320*0.7f), speed[0]);

		GUI.DrawTexture(new Rect(Screen.width * 0.665f, Screen.height * 0.625f, 498*0.7f, 320*0.7f), numbers[0]);
        Vector3 pivotPoint = new Vector2(Screen.width * 0.665f+159*0.7f, Screen.height * 0.625f+166*0.7f);
        GUIUtility.RotateAroundPivot(rotAngle, pivotPoint);
        //GUI.DrawTexture(new Rect(Screen.width * 0.665f, Screen.width * 0.665f, speed[1].width*0.65f, speed[1].height*0.65f), speed[1]);
        GUI.DrawTexture(new Rect(Screen.width * 0.665f+46*0.7f, Screen.height * 0.625f+160*0.7f, speed[1].width*0.65f, speed[1].height*0.65f), speed[1]);
        
    }

}