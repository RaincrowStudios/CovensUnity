using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellGlyphs : MonoBehaviour
{ 
	public Sprite hex; 
	public Sprite attack; 
	public Sprite ward; 
	public Sprite sunEater;
	public Sprite bind;
	public Sprite sealShadow;
	public Sprite leech;
	public Sprite ressurection;
	public Sprite greaterHex;
	public Sprite shadowTablet;
	public Sprite bless;
	public Sprite whiteFlame;
	public Sprite silence;
	public Sprite sealOfLight;
	public Sprite lightJudgement;
	public Sprite grace;
	public Sprite slowBurn;
	public Sprite blindingAura;
	public Sprite radiance;
	public Sprite dispel;
	public Sprite invisibility;
	public Sprite aradiaFavor;
	public Sprite abremelinOil;
	public Sprite abremelinBalm;
	public Sprite foolBargain;
	public Sprite mortalCoil;
	public Sprite deeSeal;
	public Sprite trueSight;
	public Sprite mirrors;

	public static Dictionary<string,Sprite> glyphs = new Dictionary<string, Sprite>();

	void Awake()
	{
		glyphs.Add ("spell_hex", hex);
		glyphs.Add ("spell_sunEater", sunEater);
		glyphs.Add ("spell_bind", bind);
		glyphs.Add ("spell_sealShadow", sealShadow);
		glyphs.Add ("spell_leech", leech);
		glyphs.Add ("spell_resurrection",ressurection);
		glyphs.Add ("spell_greaterHex", greaterHex);
		glyphs.Add ("spell_shadowTablet", shadowTablet);
		glyphs.Add ("spell_bless", bless);
		glyphs.Add ("spell_whiteFlame", whiteFlame);
		glyphs.Add ("spell_silence", silence);
		glyphs.Add ("spell_sealLight", sealOfLight);
		glyphs.Add ("spell_lightJudgement", lightJudgement);
		glyphs.Add ("spell_grace", grace);
		glyphs.Add ("spell_slowBurn", slowBurn);
		glyphs.Add ("spell_blindingAura", blindingAura);
		glyphs.Add ("spell_radiance", radiance);
		glyphs.Add ("spell_dispel", dispel);
		glyphs.Add ("spell_invisibility", invisibility);
		glyphs.Add ("spell_aradiaFavor", aradiaFavor);
		glyphs.Add ("spell_abremelinOil", abremelinOil);
		glyphs.Add ("spell_abremelinBalm", abremelinBalm);
		glyphs.Add ("spell_foolBargain", foolBargain);
		glyphs.Add ("spell_mortalCoil", mortalCoil);
		glyphs.Add ("spell_deeSeal", deeSeal);
		glyphs.Add ("spell_trueSight", trueSight);
		glyphs.Add ("spell_mirrors", mirrors);
		glyphs.Add ("spell_attack", attack);
		glyphs.Add ("spell_ward", ward);
	}

	void Add (string id, string name, int cost , string desc, int degree) {
		SpellData sd = new SpellData ();
		sd.id = id;
		sd.displayName= name;
		sd.cost = cost;
		sd.description= desc;
		sd.school = degree;
		SpellCastAPI.spells.Add (id, sd);
	}
}

