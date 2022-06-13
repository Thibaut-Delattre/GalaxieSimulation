using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attracteur : MonoBehaviour
{
	public float G = 0F;

	public static List<Attracteur> Attracteurs;

	public Rigidbody rb;

	private void Start()
	{
		G = 0.007456F;
	}

	void FixedUpdate()
	{
		foreach (Attracteur attracteur in Attracteurs)
		{
			if (attracteur != this)
				Attirer(attracteur);
		}
	}

	void OnEnable()
	{
		if (Attracteurs == null)
			Attracteurs = new List<Attracteur>();

		Attracteurs.Add(this);
	}

	void OnDisable()
	{
		Attracteurs.Remove(this);
	}




	void Attirer(Attracteur objetaAttirer)
	{
		Rigidbody rbaAttirer = objetaAttirer.rb;

		Vector3 direction = rb.position - rbaAttirer.position;
		float distance = direction.magnitude;

		if (distance == 0f)
			return;

		float forceMagnitude = G * (rb.mass * rbaAttirer.mass) / Mathf.Pow(distance, 2);
		Vector3 force = direction.normalized * forceMagnitude;

		rbaAttirer.AddForce(force);
	}
}
