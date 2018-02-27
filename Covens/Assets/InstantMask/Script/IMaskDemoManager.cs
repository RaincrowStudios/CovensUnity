using UnityEngine;
using System.Collections;
#if (UNITY_EDITOR)
using UnityEditor;
#endif

[ExecuteInEditMode()]
public class IMaskDemoManager : MonoBehaviour
{
	Material DemoCubeTexMaterial;
	InstantShaderMask IMShaderScript;
	InstantShaderMask IMShader2Script;
	InstantShaderMask IMShaderGUIScript;
	InstantParticleMask IMParticleScript;
	float Deg1Val;
	bool Deg1Check;
	float Deg2Val;
	bool Deg2Check;
	float tempX = 0.005f;
	float tempY = 0.005f;
	float tempX2 = -0.003f;
	float tempY2 = 0.003f;

	void Update ()
	{

		if (this.gameObject.name == "DemoQuadTex") {
			#if (UNITY_EDITOR)
			if (!EditorApplication.isPlaying && Application.isEditor)
				DemoCubeTexMaterial = GetComponent<Renderer> ().sharedMaterial;
			else
			#endif
				DemoCubeTexMaterial = GetComponent<Renderer> ().materials [0];
		}

		if (this.gameObject.name == "DemoCubeShader")
			IMShaderScript = GetComponent<InstantShaderMask> ();

		if (this.gameObject.name == "DemoSpriteShader")
			IMShader2Script = GetComponent<InstantShaderMask> ();

		if (this.gameObject.name == "DemoParticleSystem") {
			IMParticleScript = GetComponent<InstantParticleMask> ();
			#if (UNITY_EDITOR)
			if (!EditorApplication.isPlaying && !EditorApplication.isPaused) {
				if (GetComponent<ParticleSystem> () != null)
					GetComponent<ParticleSystem> ().Pause ();
			}
			#endif
		}

		// for DemoCanvas/Image2 (UnityGUI with shader)
		if (this.gameObject.name == "Image2") {
			IMShaderGUIScript = GetComponent<InstantShaderMask> ();
		}
		// for DemoCanvas/Image2 end

		#if (UNITY_EDITOR)
		if (EditorApplication.isPlaying) {
			#endif
			IMaskDemoExe ();
			#if (UNITY_EDITOR)
		}
		#endif

	}
	
	void LateUpdate ()
	{
		//The reason for using LateUpdate(), it is necessary to perform this process after InstantTextureMask.cs script in same frame when not Play UnityEditor.
		//It is necessary to apply the image after changed in same frame in order to check that on UnityEditor when not Play the scene. 
		//It apply the changes in one flame immediately when you change some value in demo without Play the scene.
		//When in Play the UnityEditor(or Build game), there is no need to worry about this order because frame is performed in continuous.
		if (this.gameObject.name == "DemoQuadTex") {
			if (DemoCubeTexMaterial != null)
				DemoCubeTexMaterial.mainTexture = GetComponent<InstantTextureMask> ().Result;
		}

		if (this.gameObject.name == "DemoSpriteTex") {
			SpriteRenderer DemoSpriteRndr = GetComponent<SpriteRenderer>();
			Texture2D thisResult = null;
			if (GetComponent<InstantTextureMask> () && DemoSpriteRndr != null)
				thisResult = GetComponent<InstantTextureMask> ().Result;
			if (thisResult != null) {
				DemoSpriteRndr.sprite = Sprite.Create (thisResult, new Rect (0, 0, thisResult.width, thisResult.height), new Vector2 (0.5f, 0.5f));
				DemoSpriteRndr.sprite.hideFlags = HideFlags.DontSave;
			} else {
				DemoSpriteRndr.sprite = null;
			}
		}

	}
	
	void IMaskDemoExe ()
	{
		if (IMShaderScript != null) {
			IMShaderScript.Mask1Angle += 0.5f;
			IMShaderScript.Mask2Angle -= 0.8f;
		}

		if(IMShader2Script != null) {
			IMShader2Script.Mask2ImageOffset.x += 0.01f;
		}

		if (IMParticleScript != null) {
			if (IMParticleScript.Mask1Degree < 5.0f)
				Deg1Check = true;
			else if (IMParticleScript.Mask1Degree > 99.0f)
				Deg1Check = false;
			if (Deg1Check) {
				if (IMParticleScript.Mask1Degree > 95.0f)
					Deg1Val = 0.055f;
				else
					Deg1Val = 1.0f;
			} else {
				if (IMParticleScript.Mask1Degree > 95.0f)
					Deg1Val = -0.055f;
				else
					Deg1Val = -4.5f;
			}
			IMParticleScript.Mask1Degree += Deg1Val;

			if (IMParticleScript.Mask2Degree < 5.0f)
				Deg2Check = true;
			else if (IMParticleScript.Mask2Degree > 99.0f)
				Deg2Check = false;
			if (Deg2Check) {
				if (IMParticleScript.Mask2Degree > 95.0f)
					Deg2Val = 0.055f;
				else
					Deg2Val = 4.0f;
			} else {
				if (IMParticleScript.Mask2Degree > 95.0f)
					Deg2Val = -0.055f;
				else
					Deg2Val = -2.0f;
			}
			IMParticleScript.Mask2Degree += Deg2Val;
		}

		if (IMShaderGUIScript != null) {
			if(0.2f < IMShaderGUIScript.Mask1Position.x)
				tempX = -0.005f;
			else if(IMShaderGUIScript.Mask1Position.x < -0.23f)
				tempX = 0.005f;
			if(0.24f < IMShaderGUIScript.Mask1Position.y)
				tempY = -0.005f;
			else if(IMShaderGUIScript.Mask1Position.y < -0.2f)
				tempY = 0.005f;
			IMShaderGUIScript.Mask1Position += new Vector2(tempX, tempY);
			if(0.3 < IMShaderGUIScript.Mask2Position.x)
				tempX2 = -0.005f;
			else if(IMShaderGUIScript.Mask2Position.x < -0.43)
				tempX2 = 0.005f;
			if(0.32 < IMShaderGUIScript.Mask2Position.y)
				tempY2 = -0.005f;
			else if(IMShaderGUIScript.Mask2Position.y < -0.4)
				tempY2 = 0.005f;
			IMShaderGUIScript.Mask2Position += new Vector2(tempX2, tempY2);
		}

	}

}
