using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dragger : MonoBehaviour, IDragHandler
{
	[SerializeField] GameObject gameobject;
	[SerializeField] ParticlePropertyType type;
	[SerializeField] ParticleColor color;

	Vector3 pos;
	float zOffset = 50;

	GameObject controllingGo;

	public void OnDrag(PointerEventData eventData)
	{
		Vector3 position = eventData.position;
		position.z = zOffset;

		if (controllingGo == null)
		{
			controllingGo = Instantiate(gameobject);
			controllingGo.transform.localScale = Vector3.one * 0.5f;
		}
		controllingGo.transform.position = Camera.main.ScreenToWorldPoint(position);
	}

	void Update ()
	{
		if (!Input.GetMouseButtonUp(0) || controllingGo == null) return;


		Physics2D.queriesHitTriggers = true;
		Physics2D.queriesStartInColliders = true;
		var hit = Physics2D.Raycast(controllingGo.transform.position, Vector2.up * 0.01f);
		Debug.DrawRay(controllingGo.transform.position, Vector2.up, Color.white, 5);

		Destroy(controllingGo);
		if (hit.collider == null) return;

		var particle = hit.collider.GetComponentInParent<Particle>();
		if (particle == null) return;

		particle.ResetProperties();
		if (type != ParticlePropertyType.None)
		{
			particle.AddProperty(type);
		}
		if (color != ParticleColor.None)
		{
			particle.ChangeColor(color);
		}

	}
}
