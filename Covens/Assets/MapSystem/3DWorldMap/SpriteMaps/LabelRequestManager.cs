using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LabelRequestManager : MonoBehaviour
{
	SpriteMapsController spriteMaps;
	GetLabels getLabels;
	const double DEG2RAD = Math.PI / 180;
	const double R = 6371;
	Camera cam;
	Vector2 previousVec = Vector2.zero;
	int previousZoom = 0;
	public DynamicLabelManager dynamicLabelManager;
	// Use this for initialization
	void Start ()
	{
		spriteMaps = SpriteMapsController.instance;
		getLabels = GetLabels.instance;
		spriteMaps.OnMapUpdated += CheckRequest;
		cam = Camera.main;
	}
	
	void CheckRequest(){
		int zoom = 0;
		if (cam.orthographicSize < .6 && cam.orthographicSize > .3f) {
			zoom = 1;
		} else if (cam.orthographicSize <= .3f && cam.orthographicSize > .15f) {
			zoom = 2;
		} else if(cam.orthographicSize <= .15f){
			zoom = 3;
		}

		if (zoom == 1) {
			if (previousZoom != zoom || DistanceBetweenPointsD (SpriteMapsController.mapCenter, previousVec) > 100) {
				getLabels.RequestLabel (SpriteMapsController.mapCenter, zoom);
				previousZoom = zoom;
				previousVec = SpriteMapsController.mapCenter;
			}
		} else if (zoom == 2) {
			if (previousZoom != zoom || DistanceBetweenPointsD (SpriteMapsController.mapCenter, previousVec) > 40) {
				getLabels.RequestLabel (SpriteMapsController.mapCenter, zoom);
			}
			previousZoom = zoom;
			previousVec = SpriteMapsController.mapCenter;
		} else if (zoom == 3) {
			if (previousZoom != zoom || DistanceBetweenPointsD (SpriteMapsController.mapCenter, previousVec) > 5) {
				getLabels.RequestLabel (SpriteMapsController.mapCenter, zoom);
			}
			previousZoom = zoom;
			previousVec = SpriteMapsController.mapCenter;
		}
	}



	static double DistanceBetweenPointsD(Vector2 point1, Vector2 point2)
	{
		double scfY = Math.Sin(point1.y * DEG2RAD);
		double sctY = Math.Sin(point2.y * DEG2RAD);
		double ccfY = Math.Cos(point1.y * DEG2RAD);
		double cctY = Math.Cos(point2.y * DEG2RAD);
		double cX = Math.Cos((point1.x - point2.x) * DEG2RAD);
		double sizeX1 = Math.Abs(R * Math.Acos(scfY * scfY + ccfY * ccfY * cX));
		double sizeX2 = Math.Abs(R * Math.Acos(sctY * sctY + cctY * cctY * cX));
		double sizeX = (sizeX1 + sizeX2) / 2.0;
		double sizeY = R * Math.Acos(scfY * sctY + ccfY * cctY);
		if (double.IsNaN(sizeY)) sizeY = 0;
		return Math.Sqrt(sizeX * sizeX + sizeY * sizeY);
	}

}

