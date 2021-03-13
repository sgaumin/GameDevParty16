using UnityEngine;

public class FollowCameraRotation : MonoBehaviour
{
	void Update()
	{
		transform.rotation = Camera.main.transform.rotation;
	}
}
