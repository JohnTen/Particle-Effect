using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyZone : BaseZone
{
	public ParticlePropertyType type;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		var particle = collision.GetComponent<Particle>();
		if (particle == null) return;

		if (particle == this.particle) return;

		if (particle.size < this.particle.size)
		{
			print(particle);
			Destroy(this.particle.gameObject);
			Destroy(particle.gameObject);
		}

		if (particle.size >= this.particle.size)
		{
			print(particle);
			particle.AddProperty(type);
			StartCoroutine(DelayDestroy());
		}
	}

	IEnumerator DelayDestroy()
	{
		yield return null;
		print("Property");
		Destroy(this.particle.gameObject);
	}
}
