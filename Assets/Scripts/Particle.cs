using System.Collections.Generic;
using UnityEngine;

public enum ParticlePropertyType
{
	None,
	Gravity,
	Positive,
	Negative,
}

public enum ParticleSize
{
	Small = 0,
	Medium = 1,
	Large = 2,
}

public enum ParticleColor
{
	None,
	Red,
	Green,
	Blue,
	Beta,
}

public class Particle : MonoBehaviour
{
	public ParticleSize size;
	public ParticlePropertyType type;
	public ParticleColor color;
	public Vector2 velocity;
	public float speed;
	public List<BaseZone> zones = new List<BaseZone>();

	Material material;

	private void Awake()
	{
		GetComponentsInChildren(zones);
		foreach (var z in zones)
		{
			z.particle = this;
			var fz = z as ForceZone;
			if (fz == null)
				continue;

			if (fz.positive)
				type = ParticlePropertyType.Positive;
			if (fz.negative)
				type = ParticlePropertyType.Negative;
		}

		material = GetComponent<Renderer>().material;
		ChangeColor(color);
	}

	private void FixedUpdate()
	{
		speed = velocity.magnitude;
		transform.Translate(velocity * Time.deltaTime);
	}

	public void Affect(Particle particle, float deltaTime)
	{
		particle.velocity = Affect(particle.transform.position, particle.velocity, particle.type, deltaTime);
	}

	public Vector3 Affect(Vector3 point, Vector3 velocity, ParticlePropertyType type, float deltaTime)
	{
		foreach (var z in zones)
		{
			velocity = z.Affect(velocity, point, type, deltaTime);
		}

		return velocity;
	}

	public bool HasProperty(ParticlePropertyType type)
	{
		foreach (var z in zones)
		{
			var fz = z as ForceZone;
			if (fz == null)
				continue;

			if (type == ParticlePropertyType.Gravity && fz.gravity)
				return true;

			if (type == ParticlePropertyType.Positive && fz.positive)
				return true;

			if (type == ParticlePropertyType.Negative && fz.negative)
				return true;
		}

		return false;
	}

	public void ChangeColor(ParticleColor color)
	{
		ColorZone zone = null;
		for (int i = 0; i < zones.Count; i ++)
		{
			zone = zones[i] as ColorZone;
			if (zone != null)
				break;
		}

		if (zone == null)
		{
			if (color == ParticleColor.None) return;

			zone = Instantiate(PrefabLibrary.Instance.ColorZone.gameObject).GetComponent<ColorZone>();
			zone.particle = this;
			zone.transform.SetParent(transform);
			zone.transform.localPosition = Vector3.zero;
		}

		switch (color)
		{
			case ParticleColor.None:
				color = ParticleColor.None;
				Destroy(zone.gameObject);
				break;

			case ParticleColor.Beta:
				if (color == ParticleColor.Red)
					color = ParticleColor.Green;
				else if (color == ParticleColor.Green)
					color = ParticleColor.Blue;
				else if (color == ParticleColor.Blue)
					color = ParticleColor.Red;
				break;

			case ParticleColor.Blue:
				color = ParticleColor.Blue;
				break;

			case ParticleColor.Green:
				color = ParticleColor.Green;
				break;

			case ParticleColor.Red:
				color = ParticleColor.Red;
				break;
		}

		this.color = color;
		switch (color)
		{
			case ParticleColor.Blue:
				material.color = Color.blue; ;
				break;

			case ParticleColor.Green:
				material.color = Color.green;
				break;

			case ParticleColor.Red:
				material.color = Color.red;
				break;
		}
	}

	public void AddProperty(ParticlePropertyType type)
	{
		ForceZone zone;
		for (int i = 0; i < zones.Count; i ++)
		{
			zone = zones[i] as ForceZone;
			if (zone == null) return;

			if (type == ParticlePropertyType.Gravity)
			{
				if (zone.gravity)
				{
					zones.RemoveAt(i);
					return;
				}
			}
			else if (type == ParticlePropertyType.Negative)
			{
				if (zone.negative)
				{
					zones.RemoveAt(i);
					this.type = ParticlePropertyType.None;
					return;
				}

				if (zone.positive)
				{
					zones.RemoveAt(i);
					break;
				}
			}
			else if (type == ParticlePropertyType.Positive)
			{
				if (zone.positive)
				{
					zones.RemoveAt(i);
					this.type = ParticlePropertyType.None;
					return;
				}

				if (zone.negative)
				{
					zones.RemoveAt(i);
					break;
				}
			}
		}

		zone = null;
		switch (type)
		{
			case ParticlePropertyType.Gravity:
				zone = Instantiate(PrefabLibrary.Instance.GravityZone.gameObject).GetComponent<ForceZone>();
				break;
			case ParticlePropertyType.Negative:
				zone = Instantiate(PrefabLibrary.Instance.NegativeZone.gameObject).GetComponent<ForceZone>();
				this.type = ParticlePropertyType.Negative;
				break;
			case ParticlePropertyType.Positive:
				zone = Instantiate(PrefabLibrary.Instance.PositiveZone.gameObject).GetComponent<ForceZone>();
				this.type = ParticlePropertyType.Positive;
				break;
		}

		zone.particle = this;
		zone.transform.SetParent(transform);
		zone.transform.localPosition = Vector3.zero;
	}
}
