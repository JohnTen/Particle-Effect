using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
	[SerializeField] float time;

	private void Start()
	{
		Invoke("DestroySelf", time);
	}

	void DestroySelf()
	{
		Destroy(gameObject);
	}
}
