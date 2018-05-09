﻿using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour {

	public float maxHealth;

	[Header("Rotation")]
	[Range(0f, 1f)]
	public float rotationFactor;

	[Header("Movement")]
	public float speed;

	[Header("Aesthetic (Optionnal)")]
	public MeshRenderer[] meshRenderers;
	public Collider[] colliders;
	public TrailRenderer[] trails;

	protected new Transform transform;
	protected Rigidbody rb;
	protected Health hp;

	protected Transform target;

	protected virtual void Awake() {
		transform = GetComponent<Transform>();
		rb = GetComponent<Rigidbody>();
		hp = GetComponent<Health>();
	}

	protected virtual void Start() {
		target = GameManager.Instance.CurrentPlayer.transform;
	}

	protected virtual void Update() {
		var modifiedTarget = new Vector3(target.position.x, transform.position.y, target.position.z);
		var targetRotation = Quaternion.LookRotation(modifiedTarget - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationFactor);
	}

	protected virtual void OnEnable() {
		hp.Value = maxHealth;
		foreach (var item in trails) {
			item.Clear();
		}
		if (meshRenderers.Any() && colliders.Any()) {
			StartCoroutine(SpawnAnim());
		}
	}

	protected virtual void OnDisable() {
		if (meshRenderers.Any() && colliders.Any()) {
			StopCoroutine(SpawnAnim());
		}
	}

	protected virtual IEnumerator SpawnAnim() {
		rb.isKinematic = true;
		foreach (var item in colliders) {
			item.enabled = false;
		}
		for (int i = 0; i < 2; i++) {
			foreach (var item in meshRenderers) {
				item.enabled = true;
			}
			yield return new WaitForSeconds(.25f);
			foreach (var item in meshRenderers) {
				item.enabled = false;
			}
			yield return new WaitForSeconds(.25f);
		}
		foreach (var item in meshRenderers) {
			item.enabled = true;
		}
		foreach (var item in colliders) {
			item.enabled = true;
		}
		rb.isKinematic = false;
	}
}