using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
	public bool gravity;
	public bool positive;
	public bool negative;

	public float strength;
	public float range;

	CircleCollider2D collider;

	[SerializeField] List<Particle> particles = new List<Particle>();

	private void OnDrawGizmos()
	{
		if (collider == null)
			collider = GetComponent<CircleCollider2D>();
		collider.radius = range;

		Gizmos.DrawWireSphere(transform.position, range);
	}

	private void Start()
	{
		collider = GetComponent<CircleCollider2D>();
	}

	private void FixedUpdate()
	{
		for (int i = 0; i < particles.Count; i ++)
		{
			var p = particles[i];
			if (p == null)
			{
				particles.RemoveAt(i);
				i--;
				continue;
			}

			p.velocity = Affect(p.velocity, p.transform.position, p.properties, Time.fixedDeltaTime);
		}
	}

	public Vector2 Affect(Vector2 velocity, Vector2 point, ParticleProperties properties, float deltatime)
	{
		Vector2 dir = (Vector2)transform.position - point;
		var dist = dir.magnitude;
		dir = dir.normalized * deltatime;

		if (gravity)
		{
			velocity += Mathf.Lerp(strength, 0, dist / range) * dir;
		}

		if (positive)
		{
			if (properties.positive)
				velocity -= Mathf.Lerp(strength, 0, dist / range) * dir;
			if (properties.negative)
				velocity += Mathf.Lerp(strength, 0, dist / range) * dir;
		}

		if (negative)
		{
			if (properties.negative)
				velocity -= Mathf.Lerp(strength, 0, dist / range) * dir;
			if (properties.positive)
				velocity += Mathf.Lerp(strength, 0, dist / range) * dir;
		}

		return velocity;
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		var particle = collision.GetComponent<Particle>();
		if (particle != null && !particles.Contains(particle))
			particles.Add(particle);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		var particle = collision.GetComponent<Particle>();
		if (particle != null)
			particles.Remove(particle);
	}
}
