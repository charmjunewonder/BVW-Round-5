//#define DEBUG
using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class Controller : MonoBehaviour {

	public static SerialPort spsp;

	public KeyCode key1;
	public KeyCode key2;
	public KeyCode jumpKey;

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
	public GameObject wheelChair;

	public SoundManager sm;

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
	private float velocityDecrement = 0.00004f;
	private float velocityIncrement = 0.0006f;

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

	private bool isDropping;

	private int[] speedAngles = {30, 90, 190, 360};
	private int[] speedNums = {10, 30, 100, 200};

	private SerialPort spUnity;
	private bool isOnRollerCoaster;
	private bool isPadJumping;

	void OnDrawGizmos(){
		iTween.DrawPath(controlPath,Color.blue);	
	}	
	
	
	void Start(){
		spUnity = Controller.spsp;

		//set the model of the character
		characterMode = 0;
		models[0].SetActive(true);
		animator = models[0].GetComponent<Animator>();

		velocityUpperBounds = new float[4];
		velocityUpperBounds [0] = 0.00003f;
		velocityUpperBounds [1] = 0.00008f; 
		velocityUpperBounds [2] = 0.00025f;
		velocityUpperBounds [3] = 0.00025f;

		previousNormal = Vector3.up;
		//plop the character pieces in the "Ignore Raycast" layer so we don't have false raycast data:	
		foreach (Transform child in character) {
			child.gameObject.layer=2;
		}

		isOnRollerCoaster = false;

		normals = new Queue ();
		StartCoroutine(look());
		StartCoroutine("CheckCollectedItemCount");
	}

	IEnumerator look(){
		animator.SetTrigger("Look");
		yield return new WaitForSeconds(1.5f);
		walkable = true;
	}
	
	
	void Update(){
		//--------------------------x----------------------------
		if (spUnity != null) {
			if (spUnity.IsOpen) {
				try {
					DetectKeysArduino ();
				} catch (System.Exception) {

				}
			}
			//DetectKeys();
		}
		//------------------------------------------------------
		Debug.Log(isDropping);
		//characterMode = 3;
		animator = models[characterMode].GetComponent<Animator>();
		DetectKeys();
		FindFloorAndRotation();
		MoveCharacter();
		//CheckCollectedItemCount();
	}

	IEnumerator seniorAutoWalk(){
		walkable = false;
		GameObject wheelChairClone = Instantiate(wheelChair) as GameObject;
		wheelChairClone.transform.localScale = new Vector3(100, 100, 100);
		wheelChairClone.AddComponent<WheelChairCollider>();
		BoxCollider bc = wheelChairClone.AddComponent<BoxCollider>();
		bc.size = new Vector3(0.05f, 0.05f, 0.05f);
		bc.isTrigger = true;
		wheelChairClone.SetActive(true);

		ModifyLookAtDirection(wheelChairClone, pathPosition+0.01f);
		while(true){
			velocity = Mathf.Clamp(velocity + velocityIncrement * Time.deltaTime, 0, 0.00003f);
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	public void seniorSit(){
		StartCoroutine(seniorSitIEnumerator());
	}

	IEnumerator seniorSitIEnumerator(){
		wheelChair.SetActive(true);
		StopCoroutine("seniorAutoWalk");
		animator.SetTrigger("Sit");
		yield return new WaitForSeconds(7.75f);
		walkable = true;
		models[3].collider.enabled = false;
		StartCoroutine("wheelChairAutoRun");
	}

	IEnumerator wheelChairAutoRun(){
		while(true){
			velocity = 0.0005f;
			yield return new WaitForSeconds(0.1f);
		}
	}

	void DetectKeysArduino(){

		int arduinoValue = spUnity.ReadByte ();

		if(walkable){
			if(arduinoValue == 1) {
				velocity = Mathf.Clamp(velocity + velocityIncrement * Time.deltaTime, 0, 0.00005f);
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
				if(arduinoValue == 1) {
					waitingCount = 0;
					lookingBack = false;
					StartCoroutine(LookForward());
				}
			}
		} else if(characterMode == 3){

		} else{
			if(waitingCount > 5){
				waitingCount = 0;
				walkable = false;
				StartCoroutine(Idle());
			}
			//jump:
			if (walkable && arduinoValue == 2 && jumpState==0) {
				animator.SetTrigger("Jump");
				StartCoroutine(Jump());
				jumpState=1;
			}
		}
		velocity = Mathf.Clamp(velocity - velocityDecrement * Time.deltaTime, 0, 1f);
		pathPosition += velocity;
		animator.SetFloat("Speed", velocity);
		
	}

	void DetectKeys(){
		if (!isOnRollerCoaster) {
			if(walkable){
				if(Input.GetKey(key1) || Input.GetKeyDown(key2)) {
					if(velocity < velocityUpperBounds[characterMode % 4])
					{
						velocity += velocityIncrement * Time.deltaTime;
					}
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
					if(Input.GetKey(key1) || Input.GetKey(key2)) {
						waitingCount = 0;
						lookingBack = false;
						StartCoroutine(LookForward());
					}
				}
			} else if(characterMode == 3){

			} else{
				if(waitingCount > 5){
					waitingCount = 0;
					walkable = false;
					StartCoroutine(Idle());
				}
				//jump:
				if (walkable && Input.GetKeyDown(jumpKey) && jumpState==0) {
					animator.SetTrigger("Jump");
					StartCoroutine(Jump());
					jumpState=1;
				}
			}
			if(velocity > velocityUpperBounds[characterMode % 4])
			{
				velocity = Mathf.Clamp(velocity - 5 * velocityDecrement * Time.deltaTime, 0, 1f);
			}
			else
			{
				velocity = Mathf.Clamp(velocity - velocityDecrement * Time.deltaTime, 0, 1f);
			}

		}
		else
		{
			velocity = 0.0005f;
		}
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
		for(int i = 0; i < 18; i++){
			ySpeed += jumpIncrement / 1.8f;
			yield return new WaitForSeconds(0.01f);
		}
		//yield return new WaitForSeconds(0.01f);
		for(int i = 0; i < 15; i++){
			ySpeed -= jumpIncrement / 1.5f;
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
				//Debug.DrawRay(coordinateOnPath+ offsetVector.normalized * pathOffset, previousNormal.normalized * 10, Color.red, 10); 
				if(isDropping){
					//Debug.Log("fssfsdfsdfsdfsd " + pathPosition);
					character.transform.up = previousNormal.normalized;
				} else{
					character.transform.LookAt(transform.position + diretion.normalized, previousNormal);
					//Debug.Log("fjlas " + pathPosition);
				}
				offsetVector = Vector3.Cross(previousNormal, diretion);
			}
		}
	}
	
	
	void MoveCharacter(){
		prevPosition = character.position;
		// set offset for each player
		//character.position = floorPosition + previousNormal.normalized * ySpeed;
		if(isPadJumping){
			pathPosition += 0.001f;
			float pathPercent = pathPosition%1f;
			Vector3 coordinateOnPath = iTween.PointOnPath(controlPath,pathPercent);
			character.position = coordinateOnPath + offsetVector.normalized * pathOffset;
			return;
		}

		Vector3 nextPosition = calculateNextPosition();
		if(character.position.y - nextPosition.y > 40 && !isDropping){
			Debug.Log("drop");
			StartCoroutine(DropWithGravityIEnumerator());
		} 
		else{
			Debug.Log("move");
			character.position = nextPosition;
		}
	}

	Vector3 calculateNextPosition(){
		return 	floorPosition + previousNormal.normalized * ySpeed + offsetVector.normalized * pathOffset;
	}

	public void DropWithGravity(){
		//isDropping = true;
	}

	public void DroppingExit(){
		isDropping = false;
	}

	IEnumerator DropWithGravityIEnumerator(){
		walkable = false;
		isDropping = true;
		Vector3 destination = calculateNextPosition();
		Debug.Log(destination);
		int count = 0;
		while(character.position.y - destination.y > 0.1f){
			Debug.Log(++count + " " + character.position + " " + destination);
			Vector3 position = character.position;
			position.y -= 3f;
			character.position = position;
			yield return new WaitForSeconds(0.01f);
		}
		character.position = destination;
		walkable = true;
	}

	IEnumerator CheckCollectedItemCount(){
		while(true){
			if(collectedItemCount >= 4 && characterMode <= 3){

				collectedItemCount = 0;
				models[characterMode].SetActive(false);

				TransitionEffect.gameObject.SetActive(true);

				characterMode = ++characterMode % 4;
				animator = models[characterMode].GetComponent<Animator>();

				sm.PlaySoundEffect(2, false);
				sm.PlayBGM(characterMode);

				Invoke("SetNextModelActive", 1);
				Invoke("SetWalkableTrue", 1.7f);
			}
			if(characterMode == 3){
				StartCoroutine("seniorAutoWalk");
				break;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	private void SetNextModelActive()
	{
		models[characterMode].SetActive(true);
		animator = models[characterMode].GetComponent<Animator>();
	}

	public void SetWalkableTrue()
	{
		sm.PlaySoundEffect (1, true);
		TransitionEffect.gameObject.SetActive(false);
		walkable = true;
	}

	public void SetWalkableFalse()
	{
		walkable = false;
	}

	public void SetVelocity(float vol)
	{
		velocity = vol;
	}
	public float GetVelocity()
	{
		return velocity;
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
		Vector3 offsetVector1 = Vector3.Cross(vectorWithMinDistance, direction);
		other.transform.LookAt(other.transform.position + direction.normalized, vectorWithMinDistance);
		other.transform.position = positionVector + 1f * vectorWithMinDistance.normalized
				+ offsetVector1.normalized * pathOffset;
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
		float ratio = velocity / (velocityUpperBounds[characterMode]- velocityDecrement * Time.deltaTime);
		rotAngle = -90 + ratio * speedAngles[characterMode];
		GUI.DrawTexture(new Rect(Screen.width * 0.665f, Screen.height * 0.625f, 498*0.7f, 320*0.7f), speed[0]);
		int speedNum =  Mathf.CeilToInt(speedNums[characterMode] * ratio);
		speedNum = Mathf.Clamp (speedNum, 0, 300);
		int num1 = speedNum / 100;
		int remainder = speedNum - num1 * 100;
		int num2 = remainder / 10;
		remainder = remainder - num2 * 10;

		if(num1 != 0){
			GUI.DrawTexture(new Rect(Screen.width * 0.665f + 160, Screen.height * 0.625f + 137, 76*0.75f, 92*0.75f), numbers[num1]);
		}
		if(num1 != 0 || num2 != 0){
			GUI.DrawTexture(new Rect(Screen.width * 0.665f + 193, Screen.height * 0.625f + 137, 76*0.75f, 92*0.75f), numbers[num2]);
		}

		GUI.DrawTexture(new Rect(Screen.width * 0.665f + 226, Screen.height * 0.625f + 137, 76*0.75f, 92*0.75f), numbers[remainder]);
		GUI.DrawTexture(new Rect(Screen.width * 0.665f + 260, Screen.height * 0.625f + 160, 84*0.9f, 46*0.9f), speed[2]);

        Vector3 pivotPoint = new Vector2(Screen.width * 0.665f+159*0.7f, Screen.height * 0.625f+166*0.7f);
        GUIUtility.RotateAroundPivot(rotAngle, pivotPoint);
        //GUI.DrawTexture(new Rect(Screen.width * 0.665f, Screen.width * 0.665f, speed[1].width*0.65f, speed[1].height*0.65f), speed[1]);
        GUI.DrawTexture(new Rect(Screen.width * 0.665f+46*0.7f, Screen.height * 0.625f+160*0.7f, speed[1].width*0.65f, speed[1].height*0.65f), speed[1]);
        
    }

	public Animator GetAnimator()
	{
		return animator;
	}

	public void SetOnRollerCoaster(bool t)
	{
		isOnRollerCoaster = t;
		if (t) {
			velocity = 0.0005f;
		}
	}
	public void SetIsJumpingTrue(){
		Debug.Log("Jump");
		isPadJumping = true;
		walkable = false;
	}
	public void SetIsJumpingFalse(){
		isPadJumping = false;
		walkable = true;
	}
}