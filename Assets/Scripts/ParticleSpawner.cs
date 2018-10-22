using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
	public float freq;
	public bool positive;
	public bool negative;
	public Vector2 velocity;
	public Particle particlePrefab;

	float timer;

	// Update is called once per frame
	void Update ()
	{
		timer -= Time.deltaTime;
		if (timer > 0) return;
		timer = 1 / freq;

		var p = Instantiate(particlePrefab.gameObject).GetComponent<Particle>();
		p.transform.position = transform.position;
		p.positive = positive;
		p.negative = negative;
		p.velocity = velocity;
	}
}
