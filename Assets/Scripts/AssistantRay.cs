using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistantRay : MonoBehaviour
{
	[SerializeField] ParticleSpawner spawner;
	[SerializeField] int maxSimulationSteps = 900;
	[SerializeField] LineRenderer lineRenderer;

	Vector3[] linePoints;
	int currentSteps = 0;
	List<BaseZone> affectingZones = new List<BaseZone>();
	RaycastHit2D[] hits = new RaycastHit2D[10];

	public bool Fire { get; set; }

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
				//Gizmos.color = Color.blue;
				//Gizmos.DrawLine(position, affectingZones[i].transform.position);

				if (IsInZone(position, affectingZones[i])) continue;

				affectingZones.RemoveAt(i);
				i--;
			}

			var hitsNumber = Physics2D.RaycastNonAlloc(origPos, origVel, hits, origVel.magnitude * Time.fixedDeltaTime);
			for (int i = 0; i < hitsNumber; i++)
			{
				var zone = hits[i].collider.GetComponent<BaseZone>();
				if (zone != null && !affectingZones.Contains(zone))
					affectingZones.Add(zone);
			}
		}
	}

	private bool IsInZone(Vector3 point, BaseZone zone)
	{
		var dir = zone.transform.position - point;
		return dir.sqrMagnitude < (zone.range * zone.range);
	}

	private void SimMovement(ref Vector2 position, ref Vector2 velocity)
	{
		foreach (var z in affectingZones)
		{
			velocity = z.Affect(velocity, position, spawner.type, Time.fixedDeltaTime);
		}

		position += velocity * Time.fixedDeltaTime;
	}

	private int CalculatePoints()
	{
		if (linePoints == null)
			linePoints = new Vector3[maxSimulationSteps];

		Physics2D.queriesHitTriggers = true;
		Physics2D.queriesStartInColliders = true;
		Vector2 position = transform.position;
		Vector2 velocity = spawner.velocity;
		int nextPointIndex = 1;
		linePoints[0] = position;
		affectingZones.Clear();

		currentSteps = 0;
		while (currentSteps < maxSimulationSteps)
		{
			var origPos = position;
			var origVel = velocity;
			SimMovement(ref position, ref velocity);
			currentSteps++;

			if (origVel != velocity)
				nextPointIndex++;
			if (nextPointIndex < linePoints.Length)
				linePoints[nextPointIndex] = position;

			for (int i = 0; i < affectingZones.Count; i++)
			{

				if (IsInZone(position, affectingZones[i])) continue;

				affectingZones.RemoveAt(i);
				i--;
			}

			var hitsNumber = Physics2D.RaycastNonAlloc(origPos, origVel, hits, origVel.magnitude * Time.fixedDeltaTime);
			for (int i = 0; i < hitsNumber; i++)
			{
				var zone = hits[i].collider.GetComponent<BaseZone>();
				if (zone != null && !affectingZones.Contains(zone))
					affectingZones.Add(zone);
			}
		}
		nextPointIndex++;
		if (nextPointIndex < linePoints.Length)
			linePoints[nextPointIndex] = position;
		return nextPointIndex;
	}

	private void Awake()
	{
		if (lineRenderer == null)
			lineRenderer = GetComponent<LineRenderer>();
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.Space))
			Fire = true;
		if (Fire) return;
		var length = CalculatePoints();
		lineRenderer.positionCount = length;
		lineRenderer.SetPositions(linePoints);
	}
}
