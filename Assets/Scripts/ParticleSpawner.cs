using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
	public float freq;
	public Vector2 velocity;
	public Particle particlePrefab;
	public ParticleProperties properties;

	float timer;
	
	void Update ()
	{
		timer -= Time.deltaTime;
		if (timer > 0) return;
		timer = 1 / freq;

		var p = Instantiate(particlePrefab.gameObject).GetComponent<Particle>();
		p.transform.position = transform.position;
		p.properties.positive = properties.positive;
		p.properties.negative = properties.negative;
		p.velocity = velocity;
	}
}
