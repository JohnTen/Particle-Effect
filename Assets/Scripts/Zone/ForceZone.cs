using System.Collections.Generic;
using UnityEngine;

public class ForceZone : BaseZone
{
	public bool gravity;
	public bool positive;
	public bool negative;

	List<Collider2D> manipulatingParticles = new List<Collider2D>();
	List<Particle> particles = new List<Particle>();

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
				manipulatingParticles.RemoveAt(i);
				i--;
				continue;
			}

			if (p.size >= particle.size) continue;

			p.velocity = Affect(p.velocity, p.transform.position, p.type, Time.fixedDeltaTime);
		}
	}

	public override Vector2 Affect(Vector2 velocity, Vector2 point, ParticlePropertyType type, float deltatime)
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
			if (type == ParticlePropertyType.Positive)
				velocity -= Mathf.Lerp(strength, 0, dist / range) * dir;
			if (type == ParticlePropertyType.Negative)
				velocity += Mathf.Lerp(strength, 0, dist / range) * dir;
		}

		if (negative)
		{
			if (type == ParticlePropertyType.Negative)
				velocity -= Mathf.Lerp(strength, 0, dist / range) * dir;
			if (type == ParticlePropertyType.Positive)
				velocity += Mathf.Lerp(strength, 0, dist / range) * dir;
		}

		return velocity;
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		foreach (var c in manipulatingParticles)
			if (c == collision) return;

		var particle = collision.GetComponent<Particle>();
		if (particle != null && !particles.Contains(particle))
		{
			particles.Add(particle);
			manipulatingParticles.Add(collision);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		var same = false;
		foreach (var c in manipulatingParticles)
			if (c == collision) same = true;
		if (!same) return;

		var particle = collision.GetComponent<Particle>();
		if (particle != null)
		{
			particles.Remove(particle);
			manipulatingParticles.Remove(collision);
		}
	}
}
