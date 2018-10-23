using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
	public float freq;
	public Vector2 velocity;
	public Particle particlePrefab;
	public ParticlePropertyType type;

	float timer;

	bool spawn;
	
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Space))
			spawn = true;

		if (!spawn) return;

		timer -= Time.deltaTime;
		if (timer > 0) return;
		timer = 1 / freq;

		var p = Instantiate(particlePrefab.gameObject).GetComponent<Particle>();
		p.transform.position = transform.position;
		p.type = type;
		p.velocity = velocity;
	}
}
