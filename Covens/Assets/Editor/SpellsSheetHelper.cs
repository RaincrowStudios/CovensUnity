using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

[System.Serializable]
public class SpellsSheetHelper
{
    public const string SPELLS_FOLDER = "/Editor Default Resources/Spells";

    [System.Serializable]
    private struct ServerSpellData
    {
        [JsonProperty("_id")]           public string id;
        [JsonProperty("name")]          public string name;
        [JsonProperty("school")]        public int school;
        [JsonProperty("cost")]          public int cost;
        [JsonProperty("alignment")]     public int alignment;
        [JsonProperty("xp")]            public int xp;
        [JsonProperty("level")]         public int level;
        [JsonProperty("requiredLevel")] public int requiredLevel;
        [JsonProperty("successChance")] public float successChance;
        [JsonProperty("cooldown")]      public float cooldown;
        [JsonProperty("self")]          public bool self;
        [JsonProperty("target")]        public bool target;
        [JsonProperty("canCrit")]       public bool canCrit;

        [JsonProperty("ingredients")]   public IngredientData[] ingredients;
        [JsonProperty("restrictions")]  public RestrictionData restriction;
    }

    [System.Serializable]
    private struct EffectData
    {
        [JsonProperty("buff")]          public bool buff;
        [JsonProperty("baseSpell")]     public BaseSpellData baseSpell;
    }

    [System.Serializable]
    private struct BaseSpellData
    {
    }

    [System.Serializable]
    private struct RestrictionData
    {
        [JsonProperty("state")]         public string state;
        [JsonProperty("status")]        public string[] status;
        [JsonProperty("placeOfPower")]  public bool placeOfPower;
        [JsonProperty("targetSchool")]  public int? targetSchool;
        [JsonProperty("selfSchool")]    public int? casterSchool;
        [JsonProperty("type")]          public string targetType;
    }

    [System.Serializable]
    private struct IngredientData
    {
        [JsonProperty("collectible")]   public string id;
        [JsonProperty("count")]         public int count;
        [JsonProperty("type")]          public string type;
    }

    [SerializeField] private List<ServerSpellData> m_Spells;
    [SerializeField] private string m_SheetString = "";

    private Vector2 m_SheetTextScroll;

    public void LoadSpells()
    {
        string path = Application.dataPath + SPELLS_FOLDER;
        if (System.IO.Directory.Exists(path) == false)
        {
            Debug.LogError("Path not found: \"" + path + "\"");
            return;
        }

        List<string> files = new List<string>(System.IO.Directory.EnumerateFiles(path));
        m_Spells = new List<ServerSpellData>();

        for(int i = 0; i < files.Count; i++)
        {
            if (files[i].EndsWith(".meta"))
                continue;

            string json = System.IO.File.ReadAllText(files[i]);
            m_Spells.Add(JsonConvert.DeserializeObject<ServerSpellData>(json));
        }

        m_SheetString = "";
        foreach (ServerSpellData spell in m_Spells)
        {
            m_SheetString += spell.id;
            m_SheetString += "\t" + spell.requiredLevel;
            m_SheetString += "\t"; //glyph icon
            m_SheetString += "\t" + spell.school;
            m_SheetString += "\t"; //base spell
            m_SheetString += "\t" + spell.cost;

            m_SheetString += "\t";
            if (spell.ingredients != null && spell.ingredients.Length > 0)
            {
                for (int i = 0; i < spell.ingredients.Length - 1; i++)
                    m_SheetString += spell.ingredients[i].id + ",";
                m_SheetString += spell.ingredients[spell.ingredients.Length - 1].id;
            }

            m_SheetString += "\t" + spell.alignment;
            m_SheetString += "\t" + spell.xp;
            m_SheetString += "\t" + spell.cooldown;
            m_SheetString += "\t" + spell.restriction.placeOfPower;
            m_SheetString += "\t" + GetValidSchoolArray(spell.restriction.casterSchool);
            m_SheetString += "\t" + (int)GetTarget(spell.self, spell.target);
            m_SheetString += "\t" + GetValidSchoolArray(spell.restriction.targetSchool);
            m_SheetString += "\t" + GetTargetType(spell.restriction.targetType);
            m_SheetString += "\t" + GetValidState(spell.restriction.state);
            
            m_SheetString += "\t";
            if (spell.restriction.status != null && spell.restriction.status.Length > 0)
            {
                for (int i = 0; i < spell.restriction.status.Length - 1; i++)
                    m_SheetString += spell.restriction.status[i] + ",";
                m_SheetString += spell.restriction.status[spell.restriction.status.Length - 1];
            }
            m_SheetString += "\n";
        }
    }

    public SpellData.TargetType GetTargetType(string type)
    {
        if (type == "character")
            return SpellData.TargetType.WITCH;
        if (type == "spirit")
            return SpellData.TargetType.SPIRIT;
        return SpellData.TargetType.ANY;
    }

    public SpellData.Target GetTarget(bool self, bool target)
    {
        if (self && target)
            return SpellData.Target.ANY;
        if (self)
            return SpellData.Target.SELF;
        return SpellData.Target.OTHER;
    }

    public string GetValidSchoolArray(int? school)
    {
        if (school == null)
            return "-1,0,1";
        return school.ToString();
    }

    public string GetValidState(string state)
    {
        if (string.IsNullOrEmpty(state))
            return $"vulnerable,\"\"";
        return state;
    }

    public void DrawGUI()
    {
        if (GUILayout.Button("Load spells"))
        {
            LoadSpells();
            return;
        }

        using (new BoxScope())
        {
            int columnCount = 4;
            for (int i = 0; i < m_Spells.Count; i += columnCount)
            {
                using (new GUILayout.HorizontalScope())
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        if (i + j >= m_Spells.Count)
                            break;

                        ServerSpellData spell = m_Spells[i + j];
                        GUILayout.Label(spell.name, GUILayout.Width(150));
                    }
                }
            }
        }

        using (new BoxScope())
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("Formatted spreadsheet string");
                GUILayout.FlexibleSpace();
            }

            using (var scroll = new EditorGUILayout.ScrollViewScope(m_SheetTextScroll, GUILayout.Height(200)))
            {
                m_SheetTextScroll = scroll.scrollPosition;
                EditorGUILayout.TextArea(m_SheetString, GUILayout.ExpandHeight(true));
            }
        }
    }
}
