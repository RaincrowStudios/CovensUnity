using UnityEngine;
using System.Collections;

public class ShotSpeedSync : MonoBehaviour {

public int offset = 10;
private ParticleSystem parentParticle;

void Start (){
	parentParticle = transform.parent.GetComponent<ParticleSystem>();
	float parentSpeed = parentParticle.startSpeed;
	float thisSpeed = GetComponent<ParticleSystem>().startSpeed;
	
	//Debug.Log(parentSpeed);
	if( parentSpeed >= thisSpeed){
		float sum = parentSpeed - thisSpeed;
		if( sum - offset > 0){
			while( sum - offset > 0){
				thisSpeed++;
				sum = parentSpeed - thisSpeed;
			}
		}
		GetComponent<ParticleSystem>().startSpeed = thisSpeed;
	}
	
	//Debug.Log(particleSystem.startSpeed);
	//this.enabled = false;
}


}