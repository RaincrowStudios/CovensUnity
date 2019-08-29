using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class FTFBase : MonoBehaviour
{


    protected List<string> dialogues = new List<string>(){
      "Savannah Grey: “You’re all alone witch. Let’s teach you how to protect yourself.”",
      "Savannah Grey: “The Barghest is a spectral hound that can act as a familiar to witches, but before you can summon it, you must defeat it in battle.”",
      "Savannah Grey: “Tap on the Barghest to select it.”",
      "Savannah Grey: “I will weaken the Barghest for you.”",
      "Savannah Grey: “Let's use a spell from the White School to capture this Barghest.”",
      "Savannah Grey: “This is your spellbook. You can tap these glyphs to sort your spells by the three schools of magic; White, Grey, and Shadow.”",
      "Savannah Grey: “You can tap on the Spell card to select it, and add ingredients to make your spells even more incredible.”",
      "Savannah Grey: “Tap on the White Flame’s Glyph to cast it upon the selected target.”",
      "Savannah Grey: “Well done! You have gained knowledge of your first spirit.”",
      "Savannah Grey: “Spirits can be summoned to help you in many ways, but most importantly as companions inside Places of Power.”",
      "Savannah Grey: “This is the Place of Power {locationName}. Witches congregate here to practice their spellcraft and compete for amazing rewards.”",
      "Savannah Grey: “Let’s spectate the battle from a distance, witch.”",
      "Savannah Grey: “Witches come from far and wide with exotic ingredients and spirits to claim victory over Places of Power.”",
      "Savannah Grey: “This Place of Power is a Last Witch Standing battle, they must defeat the Guardian and each other to be victorious.”",
      "Savannah Grey: “Places of Power just like the one you saw are spread out around the world.”",
      "Savannah Grey: “Here you can see when upcoming Places of Power battles will take place. But beware, the farther you travel from your physical form, the weaker you become.”",
      "Savannah Grey: “You can tap here to fly, and travel around the world.”",
      "Savannah Grey: “I think it’s time we introduce you to Madame Fortuna. She can provide you sufficient ingredients to get started…”",
      "Madame Fortuna: “Sastipe, Dea Grey! It is always a pleasure my dear lady…”",
      "Savannah Grey: “The pleasure is mine, dear friend. {witch name} is our newest witch; {he/she} needs a collection of ingredients for {his/her} spellcraft.”",
      "Savannah Grey: “Abondia's Best collection added 940 items to your inventory! You can collect a variety of items to empower your spellcraft from all over the world.”",
      "Savannah Grey: “Well, witch, that's quite enough hand-holding. I officially welcome you to the {season} tournament of witchcraft. {number} days remain.”",
      "Savannah Grey: “This is just the beginning of your adventure. I strongly suggest you visit Witch School, but if you're the adventurous type, feel free to explore on your own.”",
      "Savannah Grey: “As long as you abide by my rules, I'll continue to grant you a daily blessing of energy.”",
    };

    protected enum Act
    {
        ACT_INTRO,
        ACT_SPIRIT,
        ACT_POP,
        ACT_AFX_VIDEO,
        ACT_POP_UI,
        ACT_STORE,
        ACT_END
    }

    protected static int CurrentDialogue;
    protected static Act CurrentAct;

    [SerializeField] private Act startingAct = Act.ACT_INTRO;
    [SerializeField] private GameObject m_dialogueBox;
    [SerializeField] private GameObject m_dialogueText;
    [SerializeField] private Transform m_Avatar;
    [SerializeField] private Transform m_Hand;
    [SerializeField] private Image m_AvatarImg;
    [SerializeField] private Sprite savannahSprite;
    [SerializeField] private Sprite fortunaSprite;
    [SerializeField] private Button m_OnClickBtn;

    private bool isDialogueShowing
    {
        get
        {
            return m_dialogueBox.activeInHierarchy;
        }
    }

    private static FTFBase Instance { get; set; }

    private void Awake()
    {
        Instance = this;
        CurrentAct = startingAct;
    }

    protected void ShowDialogue()
    {

    }

    protected void ChangeDialogue(string text, System.Action onComplete)
    {
        if (!isDialogueShowing) ShowDialogue();
        m_OnClickBtn.onClick.RemoveAllListeners();
        m_OnClickBtn.onClick.AddListener(() => onComplete());
    }

    protected void ShowDialogueFortuna(string text, System.Action oOnComplete, bool right = true)
    {

    }

    protected void ShowHand(Transform t)
    {
        m_Hand.SetParent(t);
        m_Hand.localPosition.Zero();
    }

}