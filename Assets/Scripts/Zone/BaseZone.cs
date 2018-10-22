using UnityEngine;

public class BaseZone : MonoBehaviour
{
	public Particle particle;

	public float strength;
	public float range;

	protected CircleCollider2D collider;

	private void OnDrawGizmos()
	{
		if (collider == null)
			collider = GetComponent<CircleCollider2D>();
		collider.radius = range;

		Gizmos.DrawWireSphere(transform.position, range);
	}

	public virtual Vector2 Affect(Vector2 velocity, Vector2 point, ParticlePropertyType type, float deltatime)
	{
		return velocity;
	}
}
