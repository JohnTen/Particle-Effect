using System.Collections.Generic;
using UnityEngine;


public class Particle : MonoBehaviour
{
	public bool positive;
	public bool negative;
	public ParticleColor color;
	public Vector2 velocity;
	public float speed;

	private void Update()
	{
		speed = velocity.magnitude;
		transform.Translate(velocity * Time.deltaTime);
	}
}
