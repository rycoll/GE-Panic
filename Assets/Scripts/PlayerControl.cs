﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour {

	public int playerID;
	public float speed;
	private Rigidbody body;
	private Vector3 startPos;
	public string horizInput, vertInput, boostInput;
	private float boostTimeout;
	private float maxBoostTimeout = 5f;
	private float touchTimeout, maxTouchTimeout = 3f;
	public Slider boostSlider;

	private GameObject lastPlayerTouched;

	private bool paused;
	private Vector3 pause_velocity;
	private Vector3 pause_angular_velocity;

	private void Start () {
		body = GetComponent<Rigidbody> ();
		startPos = transform.position;
		paused = false;
	}

	private void FixedUpdate () {
		//apparently FixedUpdate() should be used with rigidbodies, instead of Update()
		if (paused) return;

		float horiz_move = Input.GetAxis(horizInput);
		float vert_move = Input.GetAxis (vertInput);
		Vector3 move = new Vector3 (horiz_move, 0f, vert_move);

		float multiplier = 1f;
		if (Input.GetButton (boostInput) && boostTimeout <= 0f && !paused) {
			multiplier = 50f;
			boostTimeout = maxBoostTimeout;
		}
			
		body.AddForce (move * speed * multiplier);
		if (boostTimeout > 0) {
			boostTimeout -= Time.deltaTime;
		}

		boostSlider.value = 1 - (boostTimeout / maxBoostTimeout);

		if (lastPlayerTouched != null) {
			if (touchTimeout > 0f) {
				touchTimeout -= Time.deltaTime;
			} else {
				lastPlayerTouched = null;
			}
		}
	}

	private void OnCollisionEnter (Collision coll) {
		if (coll.gameObject.tag.Equals ("Player")) {
			lastPlayerTouched = coll.gameObject;
			touchTimeout = maxTouchTimeout;
		}
	}

	public void respawn () {
		this.transform.position = startPos;
		body.velocity = Vector3.zero;
		body.angularVelocity = Vector3.zero;
		boostTimeout = 0;
		lastPlayerTouched = null;
	}

	public float getBoostTimeout () {
		return boostTimeout;
	}

	public void pause () {
		Debug.Log ("Pause " + playerID);
		pause_velocity = body.velocity;
		pause_angular_velocity = body.angularVelocity;
		body.velocity = Vector3.zero;
		body.angularVelocity = Vector3.zero;
		body.useGravity = false;
		paused = true;
	}
	public void unpause () {
		body.velocity = pause_velocity;
		body.angularVelocity = pause_angular_velocity;
		body.useGravity = true;
		paused = false;
	}

	public GameObject getLastPlayerTouched () {
		return lastPlayerTouched;
	}
}
