using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistantRay : MonoBehaviour
{
	[SerializeField] ParticleSpawner spawner;
	[SerializeField] int maxSimulationSteps = 3000;

	int currentSteps = 0;
	List<Collider2D> currentZoneColliders = new List<Collider2D>();
	List<Collider2D> currentParticleColliders = new List<Collider2D>();
	List<BaseZone> affectingZones = new List<BaseZone>();

	Dictionary<Collider2D, Particle> colliderToParticle = new Dictionary<Collider2D, Particle>();
	Dictionary<Particle, Vector2> particlePositions = new Dictionary<Particle, Vector2>();
	Dictionary<Particle, Vector2> particleVelocities = new Dictionary<Particle, Vector2>();

	List<Particle> destroyedParticles = new List<Particle>();

	List<Particle> allParticlesInScene = new List<Particle>();
	List<BaseZone> allZonesInScene = new List<BaseZone>();

	RaycastHit2D[] hits = new RaycastHit2D[10];
	Particle launchParticle;

	ParticleColor currentColor;
	ParticlePropertyType currentType;

	private void OnDrawGizmoss()
	{
		particlePositions.Clear();
		particleVelocities.Clear();
		allParticlesInScene = new List<Particle>(FindObjectsOfType<Particle>());
		for (int i = 0; i < allParticlesInScene.Count; i ++)
		{
			allParticlesInScene[i].ResetProperties();
			particlePositions.Add(allParticlesInScene[i], allParticlesInScene[i].transform.position);
			particleVelocities.Add(allParticlesInScene[i], allParticlesInScene[i].velocity);
			foreach (var z in allParticlesInScene[i].zones)
				allZonesInScene.Add(z);
		}

		launchParticle = Instantiate(spawner.particlePrefab.gameObject).GetComponent<Particle>();
		launchParticle.ResetProperties();

		Physics2D.queriesHitTriggers = true;
		Physics2D.queriesStartInColliders = true;
		Vector2 position = transform.position;
		Vector2 velocity = spawner.velocity;
		affectingZones.Clear();
		destroyedParticles.Clear();
		currentZoneColliders.Clear();
		currentParticleColliders.Clear();
		colliderToParticle.Clear();
		currentColor = launchParticle.color;
		currentType = spawner.type;
		launchParticle.type = spawner.type;

		currentSteps = 0;
		while (currentSteps < maxSimulationSteps)
		{
			var origPos = position;
			var origVel = velocity;
			SimMovement(ref position, ref velocity);
			currentSteps++;

			Gizmos.color = Color.white;
			Gizmos.DrawLine(origPos, position);

			//for (int i = 0; i < affectingZones.Count; i++)
			//{
			//	// Gizmos.color = Color.blue;
			//	// Gizmos.DrawLine(position, affectingZones[i].transform.position);

			//	if (IsInZone(position, affectingZones[i])) continue;

			//	affectingZones.RemoveAt(i);
			//	i--;
			//}

			affectingZones.Clear();
			//var hitsNumber = Physics2D.RaycastNonAlloc(origPos, origVel, hits, origVel.magnitude * Time.fixedDeltaTime);
			for (int i = 0; i < allZonesInScene.Count; i++)
			{
				if (!IsInZone(origPos, 
					particlePositions[allZonesInScene[i].particle],
					allZonesInScene[i])) continue;

				affectingZones.Add(allZonesInScene[i]);

				//if (currentZoneColliders.Contains(hits[i].collider)) continue;
				//var zone = hits[i].collider.GetComponent<BaseZone>();
				//if (zone != null && !affectingZones.Contains(zone))
				//{
				//	affectingZones.Add(zone);
				//	currentZoneColliders.Add(hits[i].collider);
				//}
			}

			launchParticle.transform.position = position;

			foreach (var p in allParticlesInScene)
			{
				var destroy = false;
				foreach (var z in p.zones)
				{
					if (IsInZone(position, particlePositions[p], z))
					{
						if ((z as ColorZone) != null)
						{
							print(p.color);
							launchParticle.ChangeColor(p.color);
							currentColor = p.color;
							destroy = true;
						}

						var property = z as PropertyZone;
						if (property != null)
						{
							print(property.type);
							launchParticle.ChangeProperty(property.type);
							currentType = launchParticle.type;
							destroy = true;
						}
					}
				}

				if (destroy)
				{
					destroyedParticles.Add(p);
				}
			}

			var zones = launchParticle.zones;
			var largestRange = 0f;

			foreach (var z in zones)
			{
				if (z.range > largestRange)
					largestRange = z.range;
			}

			//hitsNumber = Physics2D.CircleCastNonAlloc(origPos, largestRange, origVel, hits);

			foreach (var z in zones)
			{
				for (int i = 0; i < allParticlesInScene.Count; i++)
				{
					if (!IsInZone(particlePositions[allParticlesInScene[i]], position, z) || 
						allParticlesInScene[i].size >= launchParticle.size) continue;

					var particle = allParticlesInScene[i];

					if (destroyedParticles.Contains(particle)) continue;

					var origParticlePos = particlePositions[particle];
					particleVelocities[particle] = launchParticle.Affect(particlePositions[particle], particleVelocities[particle], particle.type, Time.fixedDeltaTime);
					particlePositions[particle] = particlePositions[particle] + particleVelocities[particle] * Time.fixedDeltaTime;
					Gizmos.color = Color.red;
					Gizmos.DrawLine(origParticlePos, particlePositions[particle]);
				}
			}
		}

		DestroyImmediate(launchParticle.gameObject);
	}

	private bool IsInZone(Vector2 point, Vector2 zonePos, BaseZone zone)
	{
		var dir = zonePos - point;
		return dir.sqrMagnitude < (zone.range * zone.range);
	}

	private void SimMovement(ref Vector2 position, ref Vector2 velocity)
	{
		foreach (var z in affectingZones)
		{
			velocity = z.Affect(velocity, position, currentType, Time.fixedDeltaTime);
		}

		position += velocity * Time.fixedDeltaTime;
	}
}
