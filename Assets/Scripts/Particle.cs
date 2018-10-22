using System.Collections.Generic;
using UnityEngine;

public enum ParticlePropertyType
{
	Gravity,
	Positive,
	Negative,
}

public class Particle : MonoBehaviour
{
	public ParticleProperties properties;
	public ParticleColor color;
	public Vector2 velocity;
	public float speed;
	public List<Zone> zones;

	private void FixedUpdate()
	{
		speed = velocity.magnitude;
		transform.Translate(velocity * Time.deltaTime);
	}

	public void AddProperty(ParticlePropertyType type)
	{
		for (int i = 0; i < zones.Count; i ++)
		{
			if (type == ParticlePropertyType.Gravity)
			{
				if (zones[i].gravity)
				{
					zones.RemoveAt(i);
					return;
				}
			}
			else if (type == ParticlePropertyType.Negative)
			{
				if (zones[i].negative)
				{
					zones.RemoveAt(i);
					return;
				}

				if (zones[i].positive)
				{
					zones.RemoveAt(i);
					break;
				}
			}
			else if (type == ParticlePropertyType.Positive)
			{
				if (zones[i].positive)
				{
					zones.RemoveAt(i);
					return;
				}

				if (zones[i].negative)
				{
					zones.RemoveAt(i);
					break;
				}
			}
		}

		Zone zone = null;
		switch (type)
		{
			case ParticlePropertyType.Gravity:
				zone = Instantiate(PrefabLibrary.Instance.GravityZone.gameObject).GetComponent<Zone>();
				break;
			case ParticlePropertyType.Negative:
				zone = Instantiate(PrefabLibrary.Instance.NegativeZone.gameObject).GetComponent<Zone>();
				break;
			case ParticlePropertyType.Positive:
				zone = Instantiate(PrefabLibrary.Instance.PositiveZone.gameObject).GetComponent<Zone>();
				break;
		}

		zone.transform.SetParent(transform);
		zone.transform.localPosition = Vector3.zero;
	}
}
