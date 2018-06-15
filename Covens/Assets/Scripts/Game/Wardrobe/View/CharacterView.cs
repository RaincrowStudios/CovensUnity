using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterView : MonoBehaviour
{
    [Serializable]
    public class ItemSlot
    {
        public string Name;
        public EnumEquipmentSlot m_Slot;
        public GameObject m_Root;
        public Image[] m_Images;
    }

    public EnumGender m_eGender;
    public ItemSlot[] m_ItemSlot;

    private void Reset()
    {
        List<ItemSlot> vSlots = new List<ItemSlot>();
        var values = Enum.GetValues(typeof(EnumEquipmentSlot));
        foreach (EnumEquipmentSlot slot in values)
        {
            ItemSlot pItem = new ItemSlot();
            pItem.Name = slot.ToString();
            pItem.m_Slot = slot;
            pItem.m_Root = GameObject.Find(slot.ToString());
            pItem.m_Images = pItem.m_Root.GetComponentsInChildren<Image>();
            vSlots.Add(pItem);
        }
        m_ItemSlot = vSlots.ToArray();
    }

    [ContextMenu("SetupTest")]
    public void TestSetup()
    {
        RandomItens(ItemDB.Instance.GetItens(m_eGender));
    }

    public void RandomItens(List<WardrobeItemModel> vItens)
    {
        List<WardrobeItemModel> vRandomItens = GetRandomItens(vItens);
        SetupChar(vRandomItens);
    }
    public List<WardrobeItemModel> GetRandomItens(List<WardrobeItemModel> vItens)
    {
        var values = Enum.GetValues(typeof(EnumEquipmentSlot));
        List<WardrobeItemModel> vRandomItens = new List<WardrobeItemModel>();
        foreach (EnumEquipmentSlot eSlot in values)
        {
            // ignore base
            if (eSlot == EnumEquipmentSlot.Base || eSlot == EnumEquipmentSlot.Hands)
                continue;
            
            List<WardrobeItemModel> vSlotItens = new List<WardrobeItemModel>();
            for(int i = 0; i < vItens.Count; i++)
            {
                if (vItens[i].EquipmentSlotEnum == eSlot)
                    vSlotItens.Add(vItens[i]);
            }

            bool bEquipped = UnityEngine.Random.Range(0, 100) <= 70;
            if (!bEquipped || vSlotItens.Count <= 0)
            {
                SetActivatedSlot(eSlot, false);
                continue;
            }
            
            int iIdx = UnityEngine.Random.Range(0, vSlotItens.Count);
            vRandomItens.Add(vSlotItens[iIdx]);
            //SetItem(vSlotItens[iIdx]);
        }
        return vRandomItens;
    }

    public void SetupChar(List<WardrobeItemModel> vItemList)
    {
        SetActivatedItens(false);
        // base must always be enabled
        SetActivatedSlot(EnumEquipmentSlot.Base, true);
        SetActivatedSlot(EnumEquipmentSlot.Hands, true);
        string sLog = "";
        for (int i = 0; i < vItemList.Count; i++)
        {
            List<Texture2D> vTexture = ItemDB.Instance.GetTextures(vItemList[i]);
            Sprite[] vSprites = new Sprite[vTexture.Count];
            for (int j = 0; j < vSprites.Length; j++)
            {
                Sprite pSprite = Sprite.Create(vTexture[j], new Rect(0, 0, vTexture[j].width, vTexture[j].height), new Vector2(.5f, .5f));
                vSprites[j] = pSprite;
            }
            SetItem(vItemList[i].EquipmentSlotEnum, vSprites);
            sLog += vItemList[i].Name + ", ";
        }
        Debug.Log("Set: " + sLog);
    }
    public void SetItem(WardrobeItemModel vItemList)
    {
        List<Texture2D> vTexture = ItemDB.Instance.GetTextures(vItemList);
        Sprite[] vSprites = new Sprite[vTexture.Count];
        for (int j = 0; j < vSprites.Length; j++)
        {
            Sprite pSprite = Sprite.Create(vTexture[j], new Rect(0, 0, vTexture[j].width, vTexture[j].height), new Vector2(.5f, .5f));
            vSprites[j] = pSprite;
        }
        SetItem(vItemList.EquipmentSlotEnum, vSprites);
        SetActivatedSlot(vItemList.EquipmentSlotEnum, true);
    }

    public void SetItem(EnumEquipmentSlot eSlot, Sprite[] vImages)
    {
        if (vImages == null || vImages.Length <= 0)
            return;
        for (int i = 0; i < m_ItemSlot.Length; i++)
        {
            if (m_ItemSlot[i].m_Root == null)
                continue;
            if (m_ItemSlot[i].m_Slot == eSlot)
            {
                m_ItemSlot[i].m_Root.SetActive(true);
                for(int j = 0; j < m_ItemSlot[i].m_Images.Length; j++)
                {
                    if (m_ItemSlot[i].m_Images == null || j >= m_ItemSlot[i].m_Images.Length - 1 || j >= vImages.Length -1 )
                        continue;
                    m_ItemSlot[i].m_Images[j].sprite = vImages[j];
                }
                break;
            }
        }
    }


    public void SetActivatedItens(bool bActivated)
    {
        for (int i = 0; i < m_ItemSlot.Length; i++)
        {
            if (m_ItemSlot[i].m_Root == null)
                continue;
            m_ItemSlot[i].m_Root.SetActive(bActivated);
        }
    }
    public void SetActivatedSlot(EnumEquipmentSlot eSlot, bool bActivated)
    {
        for (int i = 0; i < m_ItemSlot.Length; i++)
        {
            if (m_ItemSlot[i].m_Root == null)
                continue;
            if(m_ItemSlot[i].m_Slot == eSlot)
            {
                m_ItemSlot[i].m_Root.SetActive(bActivated);
            }
        }
    }
}