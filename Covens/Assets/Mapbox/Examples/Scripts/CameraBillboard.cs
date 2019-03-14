namespace Mapbox.Examples
{
	using UnityEngine;

	public class CameraBillboard : MonoBehaviour
	{
		public Camera _camera;

        public bool m_StreetCamera = false;

		public void Start()
		{
            if (m_StreetCamera)
                _camera = MapController.Instance.m_StreetMap.camera;
            else
                _camera = Camera.main;
		}

		void Update()
		{
			transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward, _camera.transform.rotation * Vector3.up);
		}
	}
}