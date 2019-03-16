using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellcastingTextFeedback
{
    private const string hex1 = "slightly more";
    private const string hex2 = "more";
    private const string hex3 = "significantly more";

    private const string Hex = "Success. You HEX {targetName} dealing {damage} damage. {targetName} is now {keyword} vulnerable to critical attacks.";
    private const string Suneater = "Success. Your SUN EATER dealt {damage} damage to {targetName}";
    private const string Bind = "Success. The {targetColor} witch {targetName} is now BOUND. Unless dispelled, they will be unable to fly for {bindCountDown}.";
    private const string Resurrection = "Success. You have revived {targetName}, granting them {energyGiven} energy.";
    private const string Bless = "Success. You BLESS {targetName}, granting them {energyGiven}. Their RESILIENCE is also increased by {amount}.";
    private const string Silence = "Success. You have SILENCED {targetName}. They are unable to cast as long as they are SILENCED.";
    private const string WhiteFlame = "Success. Your WHITE FLAME deals {damage} damage to {targetName}.";
    private const string Grace = "Success. You have revived {targetName}, granting them {energyGiven} energy.";
    private const string Seal = "Success. Your SEAL has reduced the POWER and RESILIENCE of {targetName} by {amount}.";
    private const string Invisibility = "Success. {targetName} is now invisible to all but those with the power of Truesight.";
    private const string Dispel = "Success. You removed {condition} from {targetName}.";
    private const string Clarity = "Success. {targetName} is now gifted with CLARITY. They have a {amount}% greater chance of success with their spells.";
    private const string SealOfBalance = "Success. {targetName}'s RESILIENCE is now {amount} and their POWER is now {power}.";
    private const string SealofLight = "Success. {targetName}'s POWER is now {power}.";
    private const string SealOfShadow = "Success. {targetName}'s RESILIENCE has increased by {amount}.";
    private const string ReflectiveWard = "Success. {targetName} is now gifted with REFLECTIVE WARD, whenever they take damage, half of the damage will be reflected back on the attacker.";
    private const string RageWard = "Success. {targetName} is gifted with RAGE WARD. At low energy, their POWER will be doubled.";
    private const string GreaterSeal = "Success. {targetName} is gifted with GREATER SEAL. Their POWER is now {amount} and RESILIENCE is {newRes}.";
    private const string GreaterBless = "Success. {targetName} is gifted with GREATER BLESS, granting them {energyGiven} energy and + {amount} RESILIENCE.";
    private const string GreaterHex = "Success. {targetName} is cursed with GREATER HEX, inflicting {damage} damage and making them significantly more vulnerable to critical attacks.";
    private const string GreaterDispel = "Success. You removed all negative conditions from {targetName}.";
    private const string Banish = "Success. You have banished {targetName} to a random location in the world. Not nice at all!";
    private const string Wither = "Success. {targetName} is WITHERING. They are BOUND and will suffer {damage} damage now more when the condition expires.";
    private const string Leech = "Success. Your LEECH inflicts {amount} damage. You are healed for {energyGiven} energy.";
    private const string Burst = "Success. Your BURST inflicts {damage} damage.";
    private const string Lazurus = "Success. You have revived {targetName}, granting them {energyGiven} energy. Their RESILIENCE is set to {amount} for 1 minute.";
    private const string ShadowFeet = "Success. You have revived {targetName} granting them {energyGiven}, but you lose {amount} energy.";
    private const string Wail = "Success. You have banished {targetName}. Somewhere. They also suffer {amount} damage.";
    private const string TrueSight = "Success. You have granted the gift of Truesight to {targetName}. They can now see what is not meant to be seen within 3 kilometers.";
    private const string CrowsEye = "Success. You have granted the gift of Truesight to {targetName}. They can now see what is not meant to be seen worldwide.";
    //private const string MarysKiss = "Success. {targetName} wears the hidden mark of Mary's Kiss. If dispelled, both {targetName} and the witch that dispels the mark will suffer significant damage.";

    private const string mirrorMulti = "perfect mirrors of you have been created nearby";
    private const string mirrorSingle = "perfect mirror of you has been created nearby.";

    private const string HexTarget = "The {casterDegree} witch {casterWitch} cast HEX on you. You lose {amount} energy. You will be vulnerable to critical attacks while afflicted with HEX.";
    private const string SunEaterTarget = "The {casterDegree} witch {casterWitch} cast SUN EATER on you. You lose {amount} energy.";
    private const string BindTarget = "The {casterDegree} witch {casterWitch} cast BIND on you. You are unable to fly until BIND wears off or it is dispelled.";
    private const string ResurrectionTarget = "The {casterDegree} witch {casterWitch} revived you, granting you {energyGiven} energy.";
    private const string BlessTarget = "The {casterDegree} witch {casterWitch} has BLESSED you, granting you {energyGiven} energy. Your RESILIENCE has increased to {amount}.";
    private const string SilenceTarget = "The {casterDegree} witch {casterWitch} has SILENCED you. You are unable to cast while SILENCED.";
    private const string WhiteFlameTarget = "The {casterDegree} witch {casterWitch} cast WHITE FLAME on you. You lose {damage} energy.";
    private const string GraceTarget = "The {casterDegree} witch {casterWitch} revived you, granting you {energyGiven} energy.";
    private const string SealTarget = "The {casterDegree} witch {casterWitch} cast SEAL on you. Your POWER and RESILIENCE have been reduced by {amount}.";
    private const string InvisibilityTarget = "The {casterDegree} witch {casterWitch} has granted you Invisiblity. Only those with Truesight will be able to see you. Casting any spell will remove this condition.";
    private const string DispelTarget = "The {casterDegree} witch {casterWitch} has dispelled {conditionRemoved} from you.";
    private const string ClarityTarget = "The {casterDegree} witch {casterWitch} has granted you CLARITY. Your chance of success with spells is increased by {amount}%.";
    private const string SealOfBalanceTarget = "The {casterDegree} witch {casterWitch} has cast SEAL OF BALANCE on you. Your RESILIENCE is now {newRes} and POWER is {newPwr}.";
    private const string SealOfLightTarget = "The {casterDegree} witch {casterWitch} has cast SEAL OF LIGHT on you. Your POWER is now {amount}.";
    private const string SealOfShadowTarget = "The {casterDegree} witch {casterWitch} has gifted you with SEAL OF SHADOW. You gain {amount} RESILIENCE.";
    private const string ReflectiveWardTarget = "The {casterDegree} witch {casterWitch} has gifted you with REFLECTIVE WARD. Half of all incoming damage will be reflected back to the attacker.";
    private const string RageWardTarget = "The {casterDegree} witch {casterWitch} has gifted you with RAGE WARD. At low energy, your POWER will be doubled.";
    private const string GreaterSealTarget = "The {casterDegree} witch {casterWitch} has gifted you with GREATER SEAL. Your POWER is now {newPwr} and RESILIENCE is {newRes}.";
    private const string GreaterBlessTarget = "The {casterDegree} witch {casterWitch} has gifted you with GREATER BLESS, granting you {energyGiven} energy and + {amount} RESILIENCE.";
    private const string GreaterHexTarget = "The {casterDegree} witch {casterWitch} has cursed you with GREATER HEX. You lose {damage} energy and are now significantly more vulnerable to critical attacks.";
    private const string GreaterDispelTarget = "The {casterDegree} witch {casterWitch} has removed all negative conditions from you.";
    private const string BanishTarget = "The {casterDegree} witch {casterWitch} has banished you! That wasn't very nice at all.";
    private const string WitherTarget = "The {casterDegree} witch {casterWitch} cast WITHER on you. You are BOUND to this location. You suffer {damage} damage and will suffer more when WITHER expires.";
    private const string LeechTarget = "The {casterDegree} witch {casterWitch} cast LEECH on you. You suffer {damage} damage and {casterWitch} gains {energyGiven} energy.";
    //private const string MirrorsTarget = "The {casterDegree} witch {casterWitch} cast MIRRORS on you. {amount} {mirrorAmount}";
    private const string BurstTarget = "The {casterDegree} witch {casterWitch} cast BURST on you dealing {damage} damage.";
    private const string LazurusTarget = "The {casterDegree} witch {casterWitch} revived you, granting you {energyGiven} energy. Your RESILIENCE is set to {amount} for 1 minute.";
    private const string ShadowFeetTarget = "The {casterDegree} witch {casterWitch} revived you, granting you {energyGiven}. They suffer {damage} damage to bring you back.";
    private const string WailTarget = "The {casterDegree} witch {casterWitch} has banished you! You suffer {damage} damage.";
    private const string TrueSightTarget = "The {casterDegree} witch {casterWitch} has granted you Truesight. You can now see what is not meant to be seen within 3 kilometers.";
    private const string CrowsEyeTarget = "The {casterDegree} witch {casterWitch} has granted you Truesight. You can now see what is not meant to be seen worldwide.";


    public static string CreateSpellDescription_Caster(WSData data)
    {

        string msg = "";
        string colorText = "";
        if (data.degree < 0)
        {
            colorText = "Shadow";
        }
        else if (data.degree > 0)
        {
            colorText = "White";
        }
        else
        {
            colorText = "Grey";
        }

        if (data.spell == "spell_hex")
        {
            msg = Hex;
            if (data.hexCount == 1)
            {
                msg = msg.Replace("{keyword}", hex1);
            }
            else if (data.hexCount == 2)
            {
                msg = msg.Replace("{keyword}", hex2);
            }
            else
            {
                msg = msg.Replace("{keyword}", hex3);
            }

            msg = msg.Replace("{targetName}", data.target);
            msg = msg.Replace("{damage}", data.result.total.ToString());
        }

        else
       if (data.spell == "spell_sunEater")
        {
            msg = Suneater;
            msg = msg.Replace("{targetName}", data.target);
            msg = msg.Replace("{damage}", data.result.total.ToString());
        }
        else
       if (data.spell == "spell_bind")
        {
            msg = Bind;
            msg = msg.Replace("{targetColor}", colorText);
            msg = msg.Replace("{targetName}", data.target);
            msg = msg.Replace("{bindCountDown}", data.bindCountDown.ToString() + " seconds.");
        }
        else
       if (data.spell == "spell_resurrection")
        {
            msg = Resurrection;
            msg = msg.Replace("{targetName}", data.target);
            msg = msg.Replace("{energyGiven}", data.result.total.ToString());
        }
        else
       if (data.spell == "spell_bless")
        {
            msg = Bless;
            msg = msg.Replace("{targetName}", data.target);
            msg = msg.Replace("{energyGiven}", data.result.total.ToString());
            msg = msg.Replace("{amount}", data.resChange.ToString());
        }
        else
       if (data.spell == "spell_silence")
        {
            msg = Silence;
            msg = msg.Replace("{targetName}", data.target);
        }
        else
       if (data.spell == "spell_whiteFlame")
        {
            msg = WhiteFlame;
            msg = msg.Replace("{targetName}", data.target);
            msg = msg.Replace("{damage}", data.result.total.ToString());
        }
        else
       if (data.spell == "spell_grace")
        {
            msg = Grace;
            msg = msg.Replace("{targetName}", data.target);
            msg = msg.Replace("{energyGiven}", data.result.total.ToString());
        }
        else
       if (data.spell == "spell_seal")
        {
            msg = Seal;
            msg = msg.Replace("{targetName}", data.target);
            msg = msg.Replace("{amount}", data.sealChange.ToString());
        }
        else
       if (data.spell == "spell_invisibility")
        {
            msg = Invisibility;
            msg = msg.Replace("{targetName}", data.target);
        }
        else
       if (data.spell == "spell_dispel")
        {
            msg = Dispel;
            msg = msg.Replace("{targetName}", data.target);
            msg = msg.Replace("{condition}", DownloadedAssets.spellDictData[data.condition.baseSpell].spellName);
        }
        else
       if (data.spell == "spell_clarity")
        {
            msg = Clarity;
            msg = msg.Replace("{targetName}", data.target);
            msg = msg.Replace("{amount}", data.clarityChange.ToString());
        }
        else
       if (data.spell == "spell_sealBalance")
        {
            msg = SealOfBalance;
            msg = msg.Replace("{targetName}", data.target);
            msg = msg.Replace("{amount}", data.resChange.ToString());
            msg = msg.Replace("{power}", data.pwrChange.ToString());
        }
        else
       if (data.spell == "spell_sealLight")
        {
            msg = SealofLight;
            msg = msg.Replace("{targetName}", data.target);
            msg.Replace("{power}", data.pwrChange.ToString());
        }
        else
       if (data.spell == "spell_sealShadow")
        {
            msg = SealOfShadow;
            msg = msg.Replace("{targetName}", data.target);
            msg = msg.Replace("{amount}", data.resChange.ToString());
        }
        else
       if (data.spell == "spell_reflectiveWard")
        {
            msg = ReflectiveWard;
            msg = msg.Replace("{targetName}", data.target);
        }
        else
       if (data.spell == "spell_rageWard")
        {
            msg = RageWard;
            msg = msg.Replace("{targetName}", data.target);
        }
        else
       if (data.spell == "spell_greaterSeal")
        {
            msg = GreaterSeal;
            msg = msg.Replace("{targetName}", data.target);
            msg = msg.Replace("{amount}", data.pwrChange.ToString());
            msg = msg.Replace("{newRes}", data.resChange.ToString());
        }
        else
       if (data.spell == "spell_greaterBless")
        {
            msg = GreaterBless;
            msg = msg.Replace("{targetName}", data.target);
            msg = msg.Replace("{energyGiven}", data.result.total.ToString());
            msg = msg.Replace("{amount}", data.resChange.ToString());
        }
        else
       if (data.spell == "spell_greaterHex")
        {
            msg = GreaterHex;
            msg = msg.Replace("{targetName}", data.target);
            msg = msg.Replace("{damage}", data.result.total.ToString());
        }
        else
       if (data.spell == "spell_greaterDispel")
        {
            msg = GreaterDispel;
            msg = msg.Replace("{targetName}", data.target);
        }
        else
       if (data.spell == "spell_banish")
        {
            msg = Banish;
            msg = msg.Replace("{targetName}", data.target);
        }
        else
       if (data.spell == "spell_wither")
        {
            msg = Wither;
            msg = msg.Replace("{targetName}", data.target);
            msg = msg.Replace("{damage}", data.result.total.ToString());
        }
        else
       if (data.spell == "spell_leech")
        {
            msg = Leech;
            msg = msg.Replace("{amount}", data.result.total.ToString());
            msg = msg.Replace("{energyGiven}", data.leechEnergy.ToString());
        }
        else
       if (data.spell == "spell_burst")
        {
            msg = Burst;
            msg = msg.Replace("{damage}", data.result.total.ToString());
        }
        else
       if (data.spell == "spell_lazurus")
        {
            msg = Lazurus;
            msg = msg.Replace("{targetName}", data.target);
            msg = msg.Replace("{energyGiven}", data.result.total.ToString());
            msg = msg.Replace("{amount}", data.resChange.ToString());
        }
        else
       if (data.spell == "spell_shadowfeet")
        {
            msg = ShadowFeet;
            msg = msg.Replace("{targetName}", data.target);
            msg = msg.Replace("{energyGiven}", data.result.total.ToString());
            msg = msg.Replace("{amount}", data.shadowFeetEnergy.ToString());
        }
        else
       if (data.spell == "spell_wail")
        {
            msg = Wail;
            msg = msg.Replace("{targetName}", data.target);
            msg = msg.Replace("{damage}", data.result.total.ToString());
        }
        else
       if (data.spell == "spell_trueSight")
        {
            msg = TrueSight;
            msg = msg.Replace("{targetName}", data.target);
        }
        else
       if (data.spell == "spell_crowsEye")
        {
            msg = CrowsEye;
            msg = msg.Replace("{targetName}", data.target);
        }
        /*
        if (data.spell == "spell_marysKiss")
        {
            msg = MarysKiss;
            msg = msg.Replace("{targetName}", data.target);
        }
        if (data.spell == "spell_whiteRain")
        {
            msg = msg.Replace("{dropAmount}", data.whiteRainOrbs.ToString());
        }
        */
        // might need to add Fool's Bargain


        if (string.IsNullOrEmpty(msg))
        {
            Debug.LogError("failed building cast spell message for " + data.spell);
#if UNITY_EDITOR
            Debug.LogError("WSData:\n" + Newtonsoft.Json.JsonConvert.SerializeObject(data));
#endif
        }

        return msg;
    }

    public static string CreateSpellDescription_Target(WSData data)
    {

        string msg = "";
        if (data.spell == "attack")
        {
            return "Spirit " + DownloadedAssets.spiritDictData[data.caster].spiritName + " attacked you. You lose <color=red>" + data.result.total.ToString() + "</color> Energy.";
        }
        string colorText = "";
        if (data.degree < 0)
        {
            colorText = "Shadow";
        }
        else if (data.degree > 0)
        {
            colorText = "White";
        }
        else
        {
            colorText = "Grey";
        }

        if (data.spell == "spell_hex")
        {
            msg = HexTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{amount}", data.result.total.ToString());
        }
        else
        if (data.spell == "spell_sunEater")
        {
            msg = SunEaterTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{amount}", data.result.total.ToString());
        }
        else
        if (data.spell == "spell_bind")
        {
            msg = BindTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
        }
        else
        if (data.spell == "spell_resurrection")
        {
            msg = ResurrectionTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{energyGiven}", data.result.total.ToString());
        }
        else
        if (data.spell == "spell_bless")
        {
            msg = BlessTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{energyGiven}", data.result.total.ToString());
            msg = msg.Replace("{amount}", data.resChange.ToString());
        }
        else
        if (data.spell == "spell_silence")
        {
            msg = SilenceTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
        }
        else
        if (data.spell == "spell_whiteFlame")
        {
            msg = WhiteFlameTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{energyGiven}", data.result.total.ToString());
        }
        else
        if (data.spell == "spell_grace")
        {
            msg = GraceTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{energyGiven}", data.result.total.ToString());
        }
        else
        if (data.spell == "spell_seal")
        {
            msg = SealTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{amount}", data.sealChange.ToString());
        }
        else
        if (data.spell == "spell_invisibility")
        {
            msg = InvisibilityTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
        }
        else
        if (data.spell == "spell_dispel")
        {
            msg = DispelTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{conditionRemoved}", DownloadedAssets.spellDictData[data.condition.baseSpell].spellName);
        }
        else
        if (data.spell == "spell_clarity")
        {
            msg = ClarityTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{amount}", data.clarityChange.ToString());
        }
        else
        if (data.spell == "spell_sealBalance")
        {
            msg = SealOfBalanceTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{newRes}", data.resChange.ToString());
            msg = msg.Replace("{newPwr}", data.pwrChange.ToString());
        }
        else
        if (data.spell == "spell_sealLight")
        {
            msg = SealOfLightTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{amount}", data.pwrChange.ToString());
        }
        else
        if (data.spell == "spell_sealShadow")
        {
            msg = SealOfShadowTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{amount}", data.resChange.ToString());
        }
        else
        if (data.spell == "spell_reflectiveWard")
        {
            msg = ReflectiveWardTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
        }
        else
        if (data.spell == "spell_rageWard")
        {
            msg = RageWardTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
        }
        else
        if (data.spell == "spell_greaterSeal")
        {
            msg = GreaterSealTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{newRes}", data.resChange.ToString());
            msg = msg.Replace("{newPwr}", data.pwrChange.ToString());
        }
        else
        if (data.spell == "spell_greaterBless")
        {
            msg = GreaterBlessTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{amount}", data.resChange.ToString());
            msg = msg.Replace("{energyGiven}", data.result.total.ToString());
        }
        else
        if (data.spell == "spell_greaterHex")
        {
            msg = GreaterHexTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{damage}", data.result.total.ToString());
        }
        else
        if (data.spell == "spell_greaterDispel")
        {
            msg = GreaterDispelTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
        }
        else
        if (data.spell == "spell_banish")
        {
            msg = BanishTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
        }
        else
        if (data.spell == "spell_wither")
        {
            msg = WitherTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{damage}", data.result.total.ToString());
        }
        else
        if (data.spell == "spell_leech")
        {
            msg = LeechTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{damage}", data.result.total.ToString());
            msg = msg.Replace("{energyGiven}", data.leechEnergy.ToString());
        }
        else
        if (data.spell == "spell_burst")
        {
            msg = BurstTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{damage}", data.result.total.ToString());
        }
        else
        if (data.spell == "spell_lazurus")
        {
            msg = LazurusTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{energyGiven}", data.result.total.ToString());
            msg = msg.Replace("{amount}", data.resChange.ToString());
        }
        else
        if (data.spell == "spell_shadowfeet")
        {
            msg = ShadowFeetTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{energyGiven}", data.result.total.ToString());
            msg = msg.Replace("{damage}", data.shadowFeetEnergy.ToString());
        }
        else
        if (data.spell == "spell_wail")
        {
            msg = WailTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
            msg = msg.Replace("{damage}", data.result.total.ToString());
        }
        else
        if (data.spell == "spell_trueSight")
        {
            msg = TrueSightTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
        }
        else
        if (data.spell == "spell_crowsEye")
        {
            msg = CrowsEyeTarget;
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
        }
        /* 
        if (data.spell == "spell_mirrors")
        {
            msg = MirrorsTarget;
            if (data.mirrorAmount == 1)
            {
                msg = msg.Replace("{mirrorAmount}", mirrorSingle);
            } else
            {
                msg = msg.Replace("{mirrorAmount}", mirrorMulti);
            }
            msg = msg.Replace("{casterDegree}", colorText);
            msg = msg.Replace("{casterWitch}", data.caster);
        }
        */
        // might need to add Fool's Bargain

        if (string.IsNullOrEmpty(msg))
        {
            if (data.result.total > 0)
            {
                msg = $"{data.caster} cast {DownloadedAssets.spellDictData[data.spell].spellID} on you. You gain <color=yellow>{data.result.total.ToString()}</color> Energy. ";
            }
            else if (data.result.total < 0)
            {
                msg = $"{data.caster} cast {DownloadedAssets.spellDictData[data.spell].spellID} on you. You lose <color=red>{data.result.total.ToString()}</color> Energy. ";
            }
            else
            {
                msg = $"{data.caster} cast {DownloadedAssets.spellDictData[data.spell].spellID} on you.";
            }
            Debug.LogError("failed building target spell message for " + data.spell);
#if UNITY_EDITOR
            Debug.LogError("WSData:\n" + Newtonsoft.Json.JsonConvert.SerializeObject(data));
#endif
        }

        return msg;
    }
}
