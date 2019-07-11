using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellbookFullScreenManager : MonoBehaviour
{
    public GameObject[] whiteSpells;
    public GameObject[] shadowSpells;
    public GameObject[] greySpells;
    public Button[] SchoolButton;
    public Button[] spellButton;
    public GameObject[] selectedGlow;
    public Button[] castButton;
    public GameObject InventoryButton;
    public GameObject InventoryUI;
    public GameObject UISpellbook_MoreSpells;
    public int num2;
    public int num;
    float inventoryPosInit = -3128f;
    float inventoryPosOpen = -1570f;
    float SpellbookPosInit = 1067f;
    float SpellbookPosOpen = 64f;
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.moveLocalX(UISpellbook_MoreSpells, SpellbookPosOpen, 1f);
        InventoryButton.GetComponent<Button>().onClick.AddListener(() => OnClickInventory());
        for (int i = 0; i < spellButton.Length; i++)
        {
            spellButton[i].onClick.AddListener(() => OnSelectSpell(i));
        }
        for (int i = 0; i < castButton.Length; i++)
        {
            castButton[i].onClick.AddListener(() => OnClickCast());
        }
        for (int i = 0; i < SchoolButton.Length; i++)
        {
            SchoolButton[i].onClick.AddListener(() => OnSortSpells(i));
        }
    }

    public void OnSelectSpell(int num)
    {
        for (int i = 0; i < selectedGlow.Length; i++)
        {
            if (num == i)
            {
                selectedGlow[i].SetActive(true);
            }
            else
            {
                selectedGlow[i].SetActive(false);
            }
            InventoryButton.SetActive(true);

        }

    }
    public void OnClickInventory()
    {
        LeanTween.moveLocalX(InventoryUI, inventoryPosOpen, 1f);
    }
    public void OnCloseInventory()
    {
        LeanTween.moveLocalX(InventoryUI, inventoryPosInit, 0.5f);
    }
    public void OnClickCast()
    {
        if (InventoryUI.transform.localPosition.x != inventoryPosInit)
        {
            OnCloseInventory();
        }
        LeanTween.moveLocalX(UISpellbook_MoreSpells, SpellbookPosInit, 0.5f);
    }
    public void OnSortSpells(int num2)
    {
        if (num2 == 1 || num2 == 2) //white flame and greater bless
        {
            for (int i = 0; i < whiteSpells.Length; i++)
            {
                whiteSpells[i].SetActive(true);
            }
            for (int i = 0; i < shadowSpells.Length; i++)
            {
                shadowSpells[i].SetActive(false);
            }
            for (int i = 0; i < greySpells.Length; i++)
            {
                greySpells[i].SetActive(false);
            }


        }
        else if (num2 == 3 || num2 == 4) //greater seal and seal
        {
            for (int i = 0; i < whiteSpells.Length; i++)
            {
                whiteSpells[i].SetActive(false);
            }
            for (int i = 0; i < shadowSpells.Length; i++)
            {
                shadowSpells[i].SetActive(false);
            }
            for (int i = 0; i < greySpells.Length; i++)
            {
                greySpells[i].SetActive(true);
            }
        }
        else //twilight dusk
        {
            for (int i = 0; i < whiteSpells.Length; i++)
            {
                whiteSpells[i].SetActive(false);
            }
            for (int i = 0; i < shadowSpells.Length; i++)
            {
                shadowSpells[i].SetActive(true);
            }
            for (int i = 0; i < greySpells.Length; i++)
            {
                greySpells[i].SetActive(false);
            }
        }
        for (int i = 0; i < SchoolButton.Length; i++)
        {
            SchoolButton[i].onClick.AddListener(() => OnEndSort());
        }
    }
    public void OnEndSort()
    {
        for (int i = 0; i < whiteSpells.Length; i++)
        {
            whiteSpells[i].SetActive(true);
        }
        for (int i = 0; i < shadowSpells.Length; i++)
        {
            shadowSpells[i].SetActive(true);
        }
        for (int i = 0; i < greySpells.Length; i++)
        {
            greySpells[i].SetActive(true);
        }
        for (int i = 0; i < SchoolButton.Length; i++)
        {
            SchoolButton[i].onClick.AddListener(() => OnSortSpells(i));
        }
    }
}
