

#####   Instant Mask   #####

Easy and smooth gradation Mask!
Instant Mask is mask the image such as MeshRenderer and SpriteRenderer, UnityGUI, Particle.
An image is cut or made hole shape of other one image.
It can mask with Alpha or Grayscale texture, it can smoothly semi-transparent.
And it is also possible states control, rotation mask, masking applicability, and so on.



========== How to use ==========


It is used by add the scripts to GameObject in accordance with the usage applications.


- InstantTextureMask
This is used to make an image that do not frequently changes of states.
It create a mask image dynamically, the resulting image will be generated to 'Result' item.
例)Texture2D MaskedTex = GetComponent<InstantTextureMask>().Result;
When it is attached in the GameObject with Image or RawImage component of UnityGUI,
can apply mask image in that component automatically.
As Unity of specification, it is necessary to turn ON the 'Read/Write Enabled' state
of texture to be used for processing.
(It will appear by selecing the texture and setting the Texture Type is Advanced in Inspector window)

In addition, you can export the result mask image file by pressing 'ExportResultImage' button.
The image file is generated in InstantMask/ExportImages folder in Project window,
and the file is named the numbers connect in order year, month, day, hour, minute, second and 4-digit number.


- InstantShaderMask
This is mask the texture of each Renderer and GUI using a shader.
It is used when often change the states of mask, size, rotate and so on.

It need a material that have 'InstantMaskShader.shader', the material is assigned
each renderer or Image or RawImage component of UnityGUI in same GameObject.
It drow a result mask image through that material.



- InstantParticleMask
This basic structure is the same as above InstantShaderMask,
this mask the particle texture and texture sheet animation of the particle.

It need a material that have 'InstantMaskShader', the material is assigned
the renderer of Particle(Shuriken) in same GameObject.
It drow a result mask image through that material too.




========== Each setting ==========

I think you will be able to roughly grasped the work of each setting items
by the setting names in Inspector window .
The setting name is same as Variable Name.(intact the capital letter and  fill the space)
For example, you want to change 'Mask Angle' by C# script,
Example) GetGomponent<InstantShaderMask>().MaskAngle += 2.0f;
It is written to Update(), the mask part rotate 2 degree in each update.
Please refer to the IMaskDemoManager.cs that are used in the asset of the Demo.
Or it is possible that variable is changed by the timeline of Animation window.

The drop-down list of change each WrapMode and SelectMaterial, these are int type internally,
so when you want to change it, you set an integer from 0 to order from the top of the list.
Example) GetGomponent<InstantShaderMask>().MaskImageWrapMode = 1;
It is set to Clamp the WrapMode of the Mask image.

InstantTextureMask use pixel unit to change size and position,
the pivot of change to size and position and angle is center of the mask image,
coordinate origin of the mask position(x:0,y:0) is the lower left of the base image.
InstantShaderMask and InstantParticleMask is compliant with the specifications of Unity shader
to change the Mask size, it is changed by Tiling.
And the pivot of rotation is center of the mask image.

If you want to know the detail of settings and behavior, please look at
the explanation site at the end of this text.



========== Notes from Unity specification ==========

* WrapMode is state of the root image, so can not be changed it from
  this asset during Edit mode(when Not scene Play in Unity).
  When Edit mode, WrapMode should be changed from the setting of root image in the usual way.
  In build game and when scene Play in Unity, You can change the WrapMode from this asset.
  However, mentioned above WrapMode is the setting of root image,
  so you should note that all GameObject using the same image receive a change of WrapMode all at once.


* InstantMaskShader.shader is added properties to a Unity builtin shader for UI.(UI-Default.shader)
  If you want to mask using other shader, you must add write property of InstantMask
  to the other shader script yourself.
  It is not possible rewrites appropriately shader script from asset,
  so it have no choice but to be done manually on the user side.


* It is material behavior in Unity specifications,
  in build game and when scene Play in Unity, the instance material is made automatically 
  if the same material is shared in some renderer and to be done individual rendering
  in each of the GameObject.
  In that case is not problem, but when Edit mode (Not scene Play in Unity) Unity specifications
  that do not make instance material, so all GameObject using the same material receive
  a change of state all at once.
  So when you want to see the individual masking some GameObjects that share the same material,
  need to confirm by scene Play.

  In addition, in Image and RawImage component of UnityGUI,
  Unity does not make the Instance material, so you should note that same material is
  same drawing results even in build game and when scene Play in Unity.

  For these behaviors, if this asset would solve such as replicate material or something,
  it is concern an error by discrepancy when referring to the material from such an other asset,
  so it leave the Unity standard behavior.
  There is nothing to worry this when you use the individual materials in each GameObject.



===============================================================


Detailed usage and further explanation of Instant Mask, please look at the site!
http://kakatte.webcrow.jp/imask/index_en.html

Producer: Myouji



