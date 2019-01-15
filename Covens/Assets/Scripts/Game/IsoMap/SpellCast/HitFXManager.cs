using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HitFXManager : UIAnimationManager
{
    public static HitFXManager Instance { get; set; }

    public GameObject hitShadow;
    public GameObject hitWhite;
    public GameObject hitGrey;
    public GameObject hitBanish;
    public GameObject WitchEscape;
    public GameObject backfireTarget;
    public Text backfireDamageTarget;

    public Text[] spellTitle;
    public Image[] spellGlyph;
    public Text Damage;
    public GameObject crit;
    public GameObject spellSuccess;
    public Text Failed;
    public Text XP;

    public GameObject hitShadowSelf;
    public GameObject hitBackfire;
    public GameObject hitWhiteSelf;
    public GameObject hitGreySelf;
    public Text backfireDamage;
    public Text DamageSelf;
    public Text critSelf;

    public Text[] spellTitleSelf;
    public Image[] spellGlyphSelf;

    public Transform DeathHead;

    public GameObject castingInfo;

    public GameObject Immune;

    public GameObject SpiritKilled;
    public Text spiritNameKilled;
    public Image spiritkilledSprite;

    public GameObject SpiritDiscovered;
    public Text titleSpirit;
    public Text titleDesc;
    public Image spiritDiscSprite;

    public bool isSpiritDiscovered = false;

    void Awake()
    {
        Instance = this;
        print(gameObject.name);
    }

    public void Attack(WSData data)
    {
        StartCoroutine(AttackHelper(data));
    }

    IEnumerator AttackHelper(WSData data)
    {
        //		print ("Got Attacked!");
        SoundManagerOneShot.Instance.PlayWhisperFX();

        //yield return new WaitForSeconds(2.2f);
        while (SpellManager.Instance.castingSpellAnim)
            yield return 1;

        int degree;
        if (data.spell != "")
        {
            degree = DownloadedAssets.spellDictData[data.spell].spellSchool;
        }
        else
        {
            if (data.result.total > 0)
            {
                degree = 1;
            }
            else
            {
                degree = -1;
            }
        }

        ShowCastingInfo(data, true);
        if (data.result.effect == "fail")
        {
            print("fail!!");
            Failed.text = "Spell Failed!";
            Reinit(Failed.gameObject);
            yield return 0;
        }
        else if (data.result.effect == "fizzle")
        {
            Reinit(Failed.gameObject);
            Failed.text = "Spell Fizzled!";
            yield return 0;
        }
        else if (data.result.effect == "backfire")
        {
            Reinit(Failed.gameObject);
            Failed.text = "Spell Backfired!";

            Reinit(hitBackfire);
            backfireDamage.text = data.result.total.ToString();
            //			XP.text = data.result.xpGain.ToString () + " XP";
            //			XP.gameObject.SetActive (true);
            //			SoundManagerOneShot.Instance.PlayWhisperFX ();
            yield return 0;
        }
        else
        {
            if (data.result.critical)
            {
                SoundManagerOneShot.Instance.PlayCrit();
                Reinit(crit);
            }
            else
            {
                crit.SetActive(false);
            }
            Damage.text = data.result.total.ToString();
            XP.text = data.result.xpGain.ToString() + " XP";
            Reinit(spellSuccess);
            Reinit(XP.gameObject);
            Reinit(Damage.gameObject);

            if (degree > 0)
            {
                Reinit(hitWhite);
            }
            else if (degree == 0)
            {
                Reinit(hitGrey);
            }
            else
            {
                Reinit(hitShadow);
            }
            foreach (var item in spellTitle)
            {
                item.text = DownloadedAssets.spellDictData[data.spell].spellName;
            }
            foreach (var item in spellGlyph)
            {
                DownloadedAssets.GetSprite(data.spell, item);

            }

        }
    }

    public void BackfireEnemy(WSData data)
    {
        SoundManagerOneShot.Instance.PlayWhisperFX();

        if (data.result.effect == "backfire")
        {
            ShowCastingInfo(data, false);
            Reinit(backfireTarget);
            backfireDamage.text = data.result.total.ToString();
            return;
        }
    }

    public void ShowCastingInfo(WSData data, bool isAttack)
    {
        if (castingInfo.activeInHierarchy)
        {
            castingInfo.GetComponent<DisableSelf>().CancelInvoke();
            castingInfo.SetActive(false);
        }

        castingInfo.SetActive(true);

        var t = castingInfo.GetComponent<Text>();

        if (data.command == "character_new_signature")
        {
            t.text = "You discovered a new signature " + DownloadedAssets.spellDictData[data.signature.id].spellName + " for the " + DownloadedAssets.spellDictData[data.signature.baseSpell].spellName + ".";
        }
        else
        {
            try
            {
                if (isAttack)
                {
                    if (data.result.effect == "success")
                    {
                        t.text = "You cast a successful " + DownloadedAssets.spellDictData[data.spell].spellName + " on " + (data.target.Contains("spirit_") ? DownloadedAssets.spiritDictData[data.target].spiritName : data.target) + ". You gain " + data.result.xpGain.ToString() + " XP.";
                    }
                    else if (data.result.effect == "fail")
                    {
                        t.text = "Your " + DownloadedAssets.spellDictData[data.spell].spellName + " on " + (data.target.Contains("spirit_") ? DownloadedAssets.spiritDictData[data.target].spiritName : data.target) + " failed, try again.";
                    }
                    else if (data.result.effect == "fizzle")
                    {
                        t.text = "Your " + DownloadedAssets.spellDictData[data.spell].spellName + " on " + (data.target.Contains("spirit_") ? DownloadedAssets.spiritDictData[data.target].spiritName : data.target) + " fizzled, try again.";
                    }
                    else
                    {
                        t.text = "You " + DownloadedAssets.spellDictData[data.spell].spellName + " backfired, you lose " + data.result.total.ToString() + " Energy.";
                    }
                }
                else
                {
                    if (data.result.effect == "success")
                    {
                        if (data.spell != "attack")
                            t.text = data.caster + " cast a " + DownloadedAssets.spellDictData[data.spell].spellName + " on you. You lose " + data.result.total + " Energy.";
                        else
                            t.text = data.caster + " attacked you. You lose " + data.result.total + " Energy.";

                    }
                    else if (data.result.effect == "backfire")
                    {
                        t.text = data.caster + "'s spell just backfired";
                    }
                }
            }
            catch
            {
            }
        }

    }

    public void Hit(WSData data)
    {
        ShowCastingInfo(data, false);
        if (data.result.effect == "success")
        {
            SoundManagerOneShot.Instance.PlayWhisperFX();

            if (data.spell != "attack")
            {
                int degree = DownloadedAssets.spellDictData[data.spell].spellSchool;
                if (degree > 0)
                {
                    Reinit(hitWhiteSelf);
                }
                else if (degree == 0)
                {
                    Reinit(hitGreySelf);
                }
                else
                {
                    Reinit(hitShadowSelf);
                }
                foreach (var item in spellTitleSelf)
                {
                    item.text = DownloadedAssets.spellDictData[data.spell].spellName;
                }
                foreach (var item in spellGlyphSelf)
                {
                    DownloadedAssets.GetSprite(data.spell, item);

                }
            }
            else
            {

                Reinit(hitShadowSelf);
                foreach (var item in spellTitleSelf)
                {
                    item.text = DownloadedAssets.localizedText[LocalizationManager.ftf_attack_button];
                }
                foreach (var item in spellGlyphSelf)
                {
                    DownloadedAssets.GetSprite("spell_hex", item);

                }
            }
        }
    }

    void Reinit(GameObject g)
    {
        if (g.activeInHierarchy)
        {
            g.GetComponent<DisableSelf>().CancelInvoke();
            g.SetActive(false);
        }
        g.SetActive(true);
    }

    public void TargetDead(bool isSpirit = false)
    {
        //		SpellCastUIManager.isDead = true;
        //		StartCoroutine (ScaleDeathHead ());
        if (!isSpirit)
        {
            Show(DeathHead.gameObject, false);
        }
        IsoTokenSetup.Instance.OnCharacterDead(true);
        if (isSpirit)
            StartCoroutine(ShowSpiritKill());
    }

    IEnumerator ShowSpiritKill()
    {
        yield return new WaitForSeconds(2.5f);
        SpellManager.Instance.Exit();
        yield return new WaitForSeconds(1.1f);
        if (isSpiritDiscovered)
        {
            SpiritDiscovered.SetActive(true);

            DownloadedAssets.GetSprite(MarkerSpawner.SelectedMarker.id, spiritDiscSprite);
        }
        else
        {
            //			print ("Fading In SPirit!!");
            SpiritKilled.SetActive(true);
            spiritNameKilled.text = DownloadedAssets.spiritDictData[MarkerSpawner.SelectedMarker.id].spiritName + " banished!";
            DownloadedAssets.GetSprite(MarkerSpawner.SelectedMarker.id, spiritkilledSprite);

        }
    }

    public void TargetRevive(bool isScaleDown = false)
    {
        //		SpellCastUIManager.isDead = false;
        Hide(DeathHead.gameObject, true, 2);
        if (!isScaleDown)
            IsoTokenSetup.Instance.OnCharacterDead(false);
        SoundManagerOneShot.Instance.PlayWhisperFX();

    }

    public void SetImmune(bool isImmune, bool isClose = false)
    {

        if (isImmune)
        {
            if (MapSelection.currentView == CurrentView.IsoView)
            {
                SoundManagerOneShot.Instance.WitchImmune();
                Immune.SetActive(false);
                Show(Immune, false);
                StartCoroutine(SetScaleFX(false, MapSelection.selectedItemTransform));
                SpellManager.Instance.IsImmune(true);
            }

        }
        else
        {
            Hide(Immune, true, 3f);
            StartCoroutine(SetScaleFX(true, MapSelection.selectedItemTransform));
            if (!isClose)
            {
                SpellManager.Instance.IsImmune(false);
            }
        }
    }

    public void HideFTFImmunity()
    {
        Hide(Immune, true, 3f);
    }

    IEnumerator SetScaleFX(bool isUp, Transform tr)
    {
        float t = 0;
        while (t > 0)
        {
            t += Time.deltaTime * 2.5f;
            if (!isUp)
                tr.localScale = Vector3.one * Mathf.SmoothStep(1, 0, t);
            else
                tr.localScale = Vector3.one * Mathf.SmoothStep(0, 1, t);
            yield return null;
        }
    }

    public void Escape()
    {
        if (!MapSelection.banishedCharacter)
        {
            WitchEscape.SetActive(true);
            StartCoroutine(ScaleDownWitch());
            Invoke("ReturnToMap", 2.5f);
            SoundManagerOneShot.Instance.PlayWhisperFX();
        }
    }

    public void Banish()
    {
        hitBanish.SetActive(true);
        MapSelection.banishedCharacter = true;
        StartCoroutine(ScaleDownWitch());
        Invoke("ReturnToMap", 2.5f);
        SoundManagerOneShot.Instance.PlayWhisperFX();
    }

    IEnumerator ScaleDownWitch()
    {
        float t = 0;
        if (MapSelection.selectedItemTransform == null)
            yield break;
        while (t <= 1)
        {
            t += Time.deltaTime * 2;
            MapSelection.selectedItemTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 41, Mathf.SmoothStep(1, 0, t));
            MapSelection.selectedItemTransform.localEulerAngles = new Vector3(0, Mathf.SmoothStep(243f, 0, t), 0);
            yield return 0;
        }
        MapSelection.IsSelf = true;
    }

    public void ReturnToMap()
    {
        WitchEscape.SetActive(false);
        if (MapSelection.currentView == CurrentView.IsoView)
        {
            if (MarkerSpawner.selectedType != MarkerSpawner.MarkerType.portal)
                SpellManager.Instance.Exit();
            else
                IsoPortalUI.instance.DisablePortalCasting();
            HitFXManager.Instance.WitchEscape.SetActive(false);
            HitFXManager.Instance.hitBanish.SetActive(false);

        }
    }


    public void EmpowerPortalFX(int newEnergy)
    {

    }
}

