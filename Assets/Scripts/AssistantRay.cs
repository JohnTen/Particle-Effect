using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistantRay : MonoBehaviour
{
	[SerializeField] ParticleSpawner spawner;
	[SerializeField] int maxSimulationSteps = 3000;

	int currentSteps = 0;
	List<Zone> affectingZones = new List<Zone>();
	RaycastHit2D[] hits = new RaycastHit2D[10];

	private void OnDrawGizmos()
	{
		Physics2D.queriesHitTriggers = true;
		Physics2D.queriesStartInColliders = true;
		Vector2 position = transform.position;
		Vector2 velocity = spawner.velocity;
		affectingZones.Clear();

		currentSteps = 0;
		while (currentSteps < maxSimulationSteps)
		{
			var origPos = position;
			var origVel = velocity;
			SimMovement(ref position, ref velocity);
			currentSteps++;

			Gizmos.color = Color.white;
			Gizmos.DrawLine(origPos, position);

			for (int i = 0; i < affectingZones.Count; i++)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(position, affectingZones[i].transform.position);

				if (IsInZone(position, affectingZones[i])) continue;

				affectingZones.RemoveAt(i);
				i--;
			}

			var hitsNumber = Physics2D.RaycastNonAlloc(origPos, origVel, hits, origVel.magnitude * Time.fixedDeltaTime);
			for (int i = 0; i < hitsNumber; i++)
			{
				var zone = hits[i].collider.GetComponent<Zone>();
				if (zone != null && !affectingZones.Contains(zone))
					affectingZones.Add(zone);
			}
		}
	}

	private bool IsInZone(Vector3 point, Zone zone)
	{
		var dir = zone.transform.position - point;
		return dir.sqrMagnitude < (zone.range * zone.range);
	}

	private void SimMovement(ref Vector2 position, ref Vector2 velocity)
	{
		foreach (var z in affectingZones)
		{
			velocity = z.Affect(velocity, position, spawner.properties, Time.fixedDeltaTime);
		}

		position += velocity * Time.fixedDeltaTime;
	}
}
