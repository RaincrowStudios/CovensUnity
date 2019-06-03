using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingPopCast : MonoBehaviour
{

    public Transform caster;
   // public Transform casterCore;
    public Transform target;
    public GameObject magicCharge;
    public GameObject magic;
    public GameObject magicHit;
    public Button Cast;
    // Start is called before the first frame update
    void Start()
    {
        //caster.position = new Vector3(caster.transform.position.x, caster.transform.position.y + 200f, caster.transform.position.z);
        Cast.onClick.AddListener(castingMagics);
        //casterCore.position = new Vector3 (caster.transform.position.x, caster.transform.position.y, caster.transform.position.z);
        
    }

    // Update is called once per frame
    public void castingMagics()
    {
        SpellcastingTrailFX.SpawnTrail(
            Random.Range(-2, 2),
            caster,
            target,
            null
        );
        //var l = Utilities.InstantiateObject(magicCharge, caster, 5f);
        ////casterCore.position = new Vector3 (caster.transform.position.x, caster.transform.position.y, caster.transform.position.z);
        //var p = Utilities.InstantiateObject(magic, caster, 4f);
        //p.transform.LookAt(target);
        //LeanTween.move(p, target, 1f).setEase(LeanTweenType.easeInExpo).setOnComplete(() => {
        //    var h = Utilities.InstantiateObject(magicHit, target, 4f); 
        //    //var o = caster.position;// new Transform (caster.transform.rotation.x, -caster.transform.rotation.y, caster.transform.rotation.z
        //    h.transform.LookAt(caster);
        //   // LeanTween.moveLocalX(p, 1f, 0.2f);
        //    LeanTween.value(0f, 1f, 1.5f).setOnComplete(() => {
        //        Destroy(p);
        //        Destroy(h);
        //    });
        //});
    }
}
