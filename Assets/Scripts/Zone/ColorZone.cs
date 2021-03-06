﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorZone : BaseZone
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		var particle = collision.GetComponent<Particle>();
		if (particle == null) return;

		if (particle == this.particle) return;

		if (this.particle.color == ParticleColor.None || particle.color == ParticleColor.None)
			return;

		if (particle.size < this.particle.size)
		{
			if (this.particle.color != particle.color)
				return;

			StartCoroutine(DelayDestroy(particle.gameObject));
		}

		if (particle.size >= this.particle.size)
		{
			if (this.particle.color != particle.color)
				particle.ChangeColor(this.particle.color);

			StartCoroutine(DelayDestroy(this.particle.gameObject));
		}
	}

	IEnumerator DelayDestroy(GameObject go)
	{
		yield return null;
		print("Color");
		Destroy(go);
	}
}
