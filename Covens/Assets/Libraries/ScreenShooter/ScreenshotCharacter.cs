using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotCharacter : MonoBehaviour
{
    public Camera m_Camera;
    public EnumEquipmentSlot[] m_Slots;
    public CharacterView m_CharView;
    public int Width = 800;
    public int Height = 1200;
    public string m_SavePath = "Builds/Screenshots/";
    public bool Clear = true;
    public bool EquipmentSplit = true;
    public bool SetRandomForEachStep = true;
    public int ExtraRandomItems = 0;

    public CharacterView[] Views;



    private void Reset()
    {
        m_Slots = Enum.GetValues(typeof(EnumEquipmentSlot)) as EnumEquipmentSlot[];
    }




    // Use this for initialization
    void Start()
    {
        StartCoroutine(Export());
    }
    IEnumerator Export()
    {
        yield return null;
        foreach(var view in Views)
        {
            foreach(var view2 in Views)
            {
                view2.gameObject.SetActive(false);
            }
            m_CharView = view;
            view.gameObject.SetActive(true);
            yield return StartCoroutine(Run());
        }

        UnityEditor.EditorApplication.isPlaying = false;
    }
    IEnumerator Run()
    {
        yield return new WaitForSeconds(1);

        if (Clear)
        {
            try
            {
                if (Directory.Exists(m_SavePath))
                    Directory.Delete(m_SavePath);

            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        // loop through slots
        foreach(EnumEquipmentSlot eSlot in m_Slots)
        {
            List<WardrobeItemModel> vItems = ItemDB.Instance.GetItens(eSlot, m_CharView.m_Controller.m_eGender);
            foreach(WardrobeItemModel pItem in vItems)
            {
                if(SetRandomForEachStep)
                    m_CharView.SetRandomItems();
                m_CharView.Equip(pItem);
                yield return null;
                // take screenshot
                yield return StartCoroutine(TakeScreenshot(eSlot, pItem.ID));
                yield return null;
            }
        }

        yield return null;


        for (int i = 0; i < ExtraRandomItems; i++)
        {
            m_CharView.SetRandomItems();
            yield return StartCoroutine(TakeScreenshot(EnumEquipmentSlot.None, "Rand-" + i));
        }

        yield return null;

        //System.Diagnostics.Process.Start("explorer.exe", m_SvaPath);
    }

    public IEnumerator TakeScreenshot(EnumEquipmentSlot eSlot, string sName)
    {
        yield return null;

        Debug.Log(sName);

        Texture2D pShot =  Screenshot(Width, Height);
        var bytes = pShot.EncodeToPNG();
        string sSavePath = EquipmentSplit ? (m_SavePath + eSlot+ "/") : m_SavePath;
        string sFinalSavePath = string.Format("{0}{1}-Shot-{2}.png", sSavePath, m_CharView.m_Controller.m_eGender, sName);
        if (!Directory.Exists(sSavePath))
            Directory.CreateDirectory(sSavePath);
        File.WriteAllBytes(sFinalSavePath, bytes);
    }


    Texture2D Screenshot(int _width, int _height)
    {
        Camera camera = m_Camera;
        RenderTexture rt = new RenderTexture(_width, _height, 32);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(_width, _height, TextureFormat.ARGB32, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, _width, _height), 0, 0);
        RenderTexture.active = null; // JC: added to avoid errors 
        camera.targetTexture = null;

        return screenShot;
    }

    //[UnityEditor.MenuItem("Raincrow/Screenshots/Male")]
    //public static void TakeScreenshots()
    //{
    //    
    //}
}