using UnityEngine;
using System.Collections;

public class JumpPadCollider : MonoBehaviour {

	public int modeOfCharacter;
	public Animator jumpPadAnimator;

	void Start(){
	}

	void Update()
	{

	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "RunMan"){
			Debug.Log("fsdlk");
			jumpPadAnimator.SetTrigger("Jump");
			other.gameObject.GetComponent<Controller>().SetIsJumpingTrue();
		}
	}
}
