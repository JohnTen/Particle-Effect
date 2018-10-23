using System.Collections;
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
	public float decayTime;
	public List<BaseZone> zones = new List<BaseZone>();

	float decayTimer;
	Material material;

	private void Awake()
	{
		ResetProperties();

		if (size == ParticleSize.Medium)
			StartCoroutine(Decay());
	}

	private void FixedUpdate()
	{
		speed = velocity.magnitude;
		transform.Translate(velocity * Time.deltaTime);
	}

	public void ResetProperties()
	{
		if (material == null)
			material = GetComponent<Renderer>().sharedMaterial;
		ChangeColor(color);

		if (zones.Count <= 0)
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
				if (this.color == ParticleColor.Red)
					color = ParticleColor.Green;
				else if (this.color == ParticleColor.Green)
					color = ParticleColor.Blue;
				else if (this.color == ParticleColor.Blue)
					color = ParticleColor.Red;

				decayTimer = decayTime;
				break;
		}

		this.color = color;
		switch (color)
		{
			case ParticleColor.Blue:
				material.color = Color.blue;
				break;

			case ParticleColor.Green:
				material.color = Color.green;
				break;

			case ParticleColor.Red:
				material.color = Color.red;
				break;
		}
	}

	public void ChangeProperty(ParticlePropertyType type)
	{
		ForceZone zone;
		for (int i = 0; i < zones.Count; i++)
		{
			zone = zones[i] as ForceZone;
			if (zone == null) continue;

			if (type == ParticlePropertyType.Gravity)
			{
				if (zone.gravity)
				{
					Destroy(zone.gameObject);
					zones.RemoveAt(i);
					return;
				}
			}
			else if (type == ParticlePropertyType.Negative)
			{
				if (zone.negative)
					return;
				if (zone.positive)
				{
					Destroy(zone.gameObject);
					zones.RemoveAt(i);
					this.type = ParticlePropertyType.None;
					return;
				}
			}
			else if (type == ParticlePropertyType.Positive)
			{
				if (zone.positive)
					return;
				if (zone.negative)
				{
					Destroy(zone.gameObject);
					zones.RemoveAt(i);
					this.type = ParticlePropertyType.None;
					return;
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

	public void AddProperty(ParticlePropertyType type)
	{
		ForceZone zone;
		for (int i = 0; i < zones.Count; i ++)
		{
			zone = zones[i] as ForceZone;
			if (zone == null) continue;

			if (type == ParticlePropertyType.Gravity)
			{
				if (zone.gravity)
				{
					Destroy(zone.gameObject);
					zones.RemoveAt(i);
					return;
				}
			}
			else if (type == ParticlePropertyType.Negative)
			{
				if (zone.negative)
				{
					Destroy(zone.gameObject);
					zones.RemoveAt(i);
					this.type = ParticlePropertyType.None;
					return;
				}

				if (zone.positive)
				{
					Destroy(zone.gameObject);
					zones.RemoveAt(i);
					break;
				}
			}
			else if (type == ParticlePropertyType.Positive)
			{
				if (zone.positive)
				{
					Destroy(zone.gameObject);
					zones.RemoveAt(i);
					this.type = ParticlePropertyType.None;
					return;
				}

				if (zone.negative)
				{
					Destroy(zone.gameObject);
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
		zones.Add(zone);
	}

	IEnumerator Decay()
	{
		decayTimer = decayTime;
		while (true)
		{
			while (decayTimer > 0)
			{
				decayTimer -= Time.deltaTime;
				yield return null;
			}

			decayTimer = decayTime;
			if (color == ParticleColor.Red)
				color = ParticleColor.Blue;
			else if (color == ParticleColor.Green)
				color = ParticleColor.Red;
			else if (color == ParticleColor.Blue)
				color = ParticleColor.Green;
			yield return null;
			print(color);

			ChangeColor(color);
		}
	}
}
