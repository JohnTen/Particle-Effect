using UnityEngine;

public class Zone : MonoBehaviour
{
	[SerializeField] bool gravity;
	[SerializeField] bool positive;
	[SerializeField] bool negative;

	[SerializeField] float Strength;
	[SerializeField] float Range;

	CircleCollider2D collider;

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, Range);
	}

	private void Start()
	{
		collider = GetComponent<CircleCollider2D>();
	}

	private void Update()
	{
		collider.radius = Range;
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		var p = collision.GetComponent<Particle>();
		if (p == null) return;

		Vector2 dir = transform.position - collision.transform.position;
		var dist = dir.magnitude;
		dir = dir.normalized * Time.deltaTime;
		
		if (gravity)
		{
			p.velocity += Mathf.Lerp(Strength, 0, dist / Range) * dir;
		}

		if (positive)
		{
			if (p.positive)
				p.velocity -= Mathf.Lerp(Strength, 0, dist / Range) * dir;
			if (p.negative)
				p.velocity += Mathf.Lerp(Strength, 0, dist / Range) * dir;
		}

		if (negative)
		{
			if (p.negative)
				p.velocity -= Mathf.Lerp(Strength, 0, dist / Range) * dir;
			if (p.positive)
				p.velocity += Mathf.Lerp(Strength, 0, dist / Range) * dir;
		}
	}
}
