﻿using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Weapon))]
public class Player : MonoBehaviour {

	[Header("Combat")]
	[SerializeField]
	private float maxRange;
	[SerializeField]
	private float attackPerSecond;
	private float FireInterval {
		get {
			return attackPerSecond == 0f ? 0f : 1 / attackPerSecond;
		}
	}

	[Header("Movement")]
	[SerializeField]
	private float speed;
	[SerializeField]
	private float decelerationFactor;

	[Header("Collision")]
	[SerializeField]
	private float bumpFactor;
	[SerializeField]
	private float disableDuration;
	private Tween collisionTween;

	private Transform target;
	private Boundary boundary;

	private Rigidbody rb;
	private new Transform transform;
	private Weapon weapon;

	private bool hasBeenTouched = false;

	private float nextFire;

	private void Awake() {
		rb = GetComponent<Rigidbody>();
		transform = GetComponent<Transform>();
		weapon = GetComponent<Weapon>();
	}

	private void Start() {
		target = null;
		nextFire = FireInterval;
		boundary = GameManager.Instance.boundary;
	}

	private void Update() {
		//if (hasBeenTouched) {
		//	return;
		//}

		AcquireTarget();
		nextFire += Time.deltaTime;
		if (target && nextFire >= FireInterval && weapon) {
			transform.LookAt(new Vector3(
				target.position.x,
				transform.position.y,
				target.position.z
			));
			weapon.Shoot();
			nextFire = 0;
		}
	}

	private void FixedUpdate() {
		var h = Input.GetAxis("Horizontal");
		var z = Input.GetAxis("Vertical");

		var direction = new Vector3(
			h,
			0f,
			z
		);

		rb.AddForce(direction * speed, ForceMode.Impulse);

		var clamped = Vector3.ClampMagnitude(new Vector3(rb.velocity.x, 0f, rb.velocity.z), speed);
		rb.velocity = new Vector3(clamped.x * decelerationFactor, rb.velocity.y, clamped.z * decelerationFactor);
	}

	private void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.CompareTag("Enemy")) {
			// changing deceleration factor to have a better impulse
			collisionTween.Complete(false);
			var df = decelerationFactor;
			decelerationFactor = decelerationFactor * 4;
			collisionTween = DOTween.To(() => decelerationFactor, x => decelerationFactor = x, df, disableDuration); //setting up the tween to reset the deceleration factor

			// enter slow motion and add impulse
			TimeManager.Instance.EnterSlowMotion();
			rb.velocity = Vector3.zero;
			rb.AddForce(collision.contacts[0].normal.normalized * bumpFactor, ForceMode.Impulse);
		}
	}

	private void AcquireTarget() {
		Transform nearest = null;
		var minDist = Mathf.Infinity;
		foreach (var item in GameManager.Instance.CurrentEnemies) {
			var dist = Vector3.Distance(item.transform.position, transform.position);
			if (dist < maxRange && dist < minDist) {
				nearest = item.transform;
				minDist = dist;
			}
		}
		target = nearest;
	}

	private void ResetHasBeenTouched() {
		hasBeenTouched = false;
	}

}
