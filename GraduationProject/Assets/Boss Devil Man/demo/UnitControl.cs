using UnityEngine;
using System;
using System.Collections;

//Just for demonstration, you can replace it with your own code logic.
public class UnitControl : MonoBehaviour {

	private Animator animator;
	private float walkStartTime = 0;
	private bool isEvade = false;

	void Start () {
		animator = this.GetComponent<Animator> ();
	}

	void Update(){

		int horizontal = 0;  
		int vertical = 0;		

		horizontal = (int)(Input.GetAxisRaw ("Horizontal"));
		vertical = (int)(Input.GetAxisRaw ("Vertical"));

		if (horizontal != 0) {
			vertical = 0;
		}

		Vector3 localScale = this.transform.localScale;
		Vector3 velocity = Vector3.zero;
		Vector3 newPosition = Vector3.zero;

		if (horizontal != 0) {
			if (walkStartTime == 0) {
				walkStartTime = Time.time;
			}
			float speed = 0.05f;
			float dis = 0.1f;
			if (Time.time - walkStartTime > 2.0f) {
				speed = 0.03f;
				animator.SetTrigger ("run");
			} else {
				animator.SetTrigger ("walk");
			}
			if (isEvade) {
				speed = 0.01f;
				dis = 0.2f;
			}
			if (horizontal < 0) {
				localScale.x = -Math.Abs (localScale.x);
				newPosition = this.transform.position + new	Vector3 (-dis, 0, 0);
			} else if (horizontal > 0) {
				localScale.x = Math.Abs (localScale.x);
				newPosition = this.transform.position + new	Vector3 (dis, 0, 0);
			}

			this.transform.localScale = localScale;
			this.transform.position = Vector3.SmoothDamp (this.transform.position, newPosition, ref velocity, speed);


		}


		if (Input.GetKeyUp (KeyCode.A) || Input.GetKeyUp (KeyCode.D) || Input.GetKeyUp (KeyCode.LeftArrow) || Input.GetKeyUp (KeyCode.RightArrow)) {
			walkStartTime = 0;
			animator.ResetTrigger ("idle_1");
			animator.ResetTrigger ("walk");
			animator.ResetTrigger ("run");
			animator.SetTrigger ("idle_1");
		}

		if (Input.anyKeyDown) {
			foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode))) {  
				if (Input.GetKeyDown (keyCode)) {  
					if (keyCode == KeyCode.H) {
						animator.SetTrigger ("skill_1");
					} else if (keyCode == KeyCode.J) {
						animator.SetTrigger ("skill_2");
					} else if (keyCode == KeyCode.K) {
						animator.SetTrigger ("hit_1");
					} else if (keyCode == KeyCode.L) {
						animator.SetTrigger ("hit_2");
					} else if (keyCode == KeyCode.Y) {
						animator.SetTrigger ("hit_2");
						animator.SetTrigger ("death");
					} else if (keyCode == KeyCode.Space) {
						animator.SetTrigger ("idle_2");
						StartCoroutine (Evade ());
					} 
				}  
			}  
		}
	}

	public IEnumerator Evade(){
		yield return new WaitForSeconds (0.2f);
		isEvade = true;
		yield return new WaitForSeconds (0.2f);
		isEvade = false;
	}

}
