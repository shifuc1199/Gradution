using UnityEngine;
using System.Collections;

namespace Ferr.Example {
	public class Toss : MonoBehaviour {
		Vector3  start;
		Animator animator;
	
		public GameObject visual;
		public bool       swipeToJump = false;
	
		Rigidbody2D body;

		void Start () {
			body = GetComponent<Rigidbody2D>();
			body.gravityScale = 1;
		
			animator = visual.GetComponent<Animator>();
		}
	
		void Update () {
		
			if (body.gravityScale > 0) {
				visual.transform.rotation = Quaternion.FromToRotation(Vector3.right, body.velocity);
				visual.transform.eulerAngles -= new Vector3(0, 0, 90);
			}
		
			if (Input.GetButtonDown("Fire1")) {
				start = Input.mousePosition;
			} else if (Input.GetButtonUp("Fire1")) {
				Ray   ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
				float dist = 0;
				new Plane(-Vector3.forward, transform.position.z).Raycast(ray, out dist);
			
				if (swipeToJump)
					body.AddForce( (Input.mousePosition - start) * 2.1f );
				else
					body.AddForce( (ray.GetPoint(dist) - transform.position) * 100f );
			
				animator.SetTrigger("Jump");
				body.gravityScale = 1;
			}
		}
	
		void OnCollisionEnter2D(Collision2D collision) {
			animator.SetTrigger("Land");
			body.gravityScale = 0;
			body.velocity     = Vector3.zero;
			body.Sleep();
		
			visual.transform.eulerAngles = new Vector3(0,0, 360 - Ferr.PathUtil.ClockwiseAngle(Vector2.right, collision.contacts[0].normal) + 270);
		}
	}
}