using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPortraitUtil : MonoBehaviour
{
    public static PlayerPortraitUtil Instance { get; private set; }

    [SerializeField] private Camera m_Camera;
    [SerializeField] private ApparelView m_MaleView;
    [SerializeField] private ApparelView m_FemaleView;

    public Texture2D texture { get; private set; }
    public Sprite sprite { get; private set; }

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("MULTIPLE INSTANCES IN SCENE");
        }
        Instance = this;
        m_Camera.gameObject.SetActive(false);
    }

    public void UpdatePortrait(System.Action<Sprite> callback)
    {
        StartCoroutine(UpdatePortraitCoroutine(256, 256, callback));
    }

    private IEnumerator UpdatePortraitCoroutine(int width, int height, System.Action<Sprite> callback)
    {
        yield return new WaitForEndOfFrame();

        //initialize vars
        ApparelView characterView;
        if (PlayerDataManager.playerData.male)
            characterView = m_MaleView;
        else
            characterView = m_FemaleView;
        Transform root = characterView.transform.root;
        bool prevRootState = root.gameObject.activeSelf;
        Vector3 prevRootPos = root.transform.position;
        bool prevState = characterView.gameObject.activeSelf;

        //move the character out of screen, and set it up
        root.transform.position = new Vector3(-1000, 0, 0);
        root.gameObject.SetActive(true);
        characterView.gameObject.SetActive(true);
        characterView.InitCharacter(PlayerDataManager.playerData.equipped);
        m_Camera.gameObject.SetActive(true);

        //generate the texture and sprite
        RenderTexture rt = new RenderTexture(width, height, 24);
        m_Camera.targetTexture = rt;
        texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        m_Camera.Render();
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();
        RenderTexture.active = prev;
        m_Camera.targetTexture = null;
        sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));

        //reset character to initial state
        m_Camera.gameObject.SetActive(false);
        root.transform.position = prevRootPos;
        root.gameObject.SetActive(prevRootState);
        characterView.gameObject.SetActive(prevState);

        callback?.Invoke(sprite);
    }
}
