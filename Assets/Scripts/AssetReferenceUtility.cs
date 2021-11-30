using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D; //spriteAtlases
using UnityEngine.UI;

public class AssetReferenceUtility : MonoBehaviour
{
    //--------����Ԥ����---------
    public AssetReference objectToLoad;//Ԥ����
    public AssetReference accessoryObjectToLoad;//object�� ������������
    private GameObject instantiatedObject;//������
    private GameObject instantiatedAccessoryObject;//������
    //-------���ؾ���----------
    //public AssetReferenceSprite newSprite;//����
    //private SpriteRenderer spriteRenderer;
    //-------ʹ�õ�ַ��ʽ���ؾ���----------
    //public string newSpriteAddress;
    //public bool useAddress;//ֱ��ʹ�õ�ַ���ؿ�Ѱַ����

    public Image image;//Ҳ���Լ��� UI ���Image 

    //---------����spriteAtlas�е�sprite------
    private SpriteRenderer spriteRenderer;
    public AssetReferenceAtlasedSprite newAtlasedSprite;
    public string spriteAtlasAddress;
    public string atlasedSpriteName;
    public bool useAtlasedSpriteName;

    void Start()
    {
        //1, ���ط�ʽ1������Ԥ����
        //Addressables.LoadAssetAsync<GameObject>(objectToLoad).Completed += ObjectLoadDone;
        /* ���ؾ���
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        newSprite.LoadAssetAsync().Completed += SpriteLoaded;
        */
        /*
        //ֱ��ʹ�õ�ַ���ؿ�Ѱַ����
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (useAddress)
            Addressables.LoadAssetAsync<Sprite>(newSpriteAddress).Completed += SpriteLoaded;
        else
            newSprite.LoadAssetAsync().Completed += SpriteLoaded;
        */

        //����spriteAtlas
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (useAtlasedSpriteName)//�Ƿ�ʹ�õ�ַ��ʽ����
        {
            string atlasedSpriteAddress = spriteAtlasAddress + '[' + atlasedSpriteName + ']';
            Addressables.LoadAssetAsync<Sprite>(atlasedSpriteAddress).Completed += SpriteLoaded;
        }
        else
            newAtlasedSprite.LoadAssetAsync().Completed += SpriteLoaded;

        /*
        //����spriteAtlas�� ͼƬ�ĵ�3�ַ���
         spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        Addressables.LoadAssetAsync<SpriteAtlas>(spriteAtlasAddress).Completed += SpriteAtlasLoaded;
        */

    }


    void ObjectLoadDone(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject loadedObject = obj.Result;
            Debug.Log("===  ���سɹ�");
            instantiatedObject = Instantiate(loadedObject);
            Debug.Log("===��������ɹ�");
            if (accessoryObjectToLoad != null)
            {
                //2, ���ط�ʽ2
                accessoryObjectToLoad.InstantiateAsync(instantiatedObject.transform).Completed += op =>
                {
                    if (op.Status == AsyncOperationStatus.Succeeded)
                    {
                        instantiatedAccessoryObject = op.Result;
                        Debug.Log("Successfully loaded and instantiated accessory object.");
                    }
                };
            }
        }
    }


    private void SpriteLoaded(AsyncOperationHandle<Sprite> obj)
    {
        switch (obj.Status)
        {
            case AsyncOperationStatus.Succeeded:
                spriteRenderer.sprite = obj.Result;
                image.sprite = obj.Result;
                break;
            case AsyncOperationStatus.Failed:
                Debug.LogError("===Sprite load failed.");
                break;
            default:
                // case AsyncOperationStatus.None:
                break;
        }
    }

    //����spriteAtlas�� ͼƬ�ĵ�3�ַ���
    private void SpriteAtlasLoaded(AsyncOperationHandle<SpriteAtlas> obj)
    {
        switch (obj.Status)
        {
            case AsyncOperationStatus.Succeeded:
                spriteRenderer.sprite = obj.Result.GetSprite(atlasedSpriteName);
                break;
            case AsyncOperationStatus.Failed:
                Debug.LogError("Sprite load failed. Using default Sprite.");
                break;
            default:
                // case AsyncOperationStatus.None:
                break;
        }
    }

}
