using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D; //spriteAtlases
using UnityEngine.UI;

public class AssetReferenceUtility : MonoBehaviour
{
    //--------加载预制体---------
    public AssetReference objectToLoad;//预制体
    public AssetReference accessoryObjectToLoad;//object的 附件，子物体
    private GameObject instantiatedObject;//父物体
    private GameObject instantiatedAccessoryObject;//子物体
    //-------加载精灵----------
    //public AssetReferenceSprite newSprite;//精灵
    //private SpriteRenderer spriteRenderer;
    //-------使用地址方式加载精灵----------
    //public string newSpriteAddress;
    //public bool useAddress;//直接使用地址加载可寻址精灵

    public Image image;//也可以加载 UI 里的Image 

    //---------加载spriteAtlas中的sprite------
    private SpriteRenderer spriteRenderer;
    public AssetReferenceAtlasedSprite newAtlasedSprite;
    public string spriteAtlasAddress;
    public string atlasedSpriteName;
    public bool useAtlasedSpriteName;

    void Start()
    {
        //1, 加载方式1，加载预制体
        //Addressables.LoadAssetAsync<GameObject>(objectToLoad).Completed += ObjectLoadDone;
        /* 加载精灵
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        newSprite.LoadAssetAsync().Completed += SpriteLoaded;
        */
        /*
        //直接使用地址加载可寻址精灵
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (useAddress)
            Addressables.LoadAssetAsync<Sprite>(newSpriteAddress).Completed += SpriteLoaded;
        else
            newSprite.LoadAssetAsync().Completed += SpriteLoaded;
        */

        //加载spriteAtlas
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (useAtlasedSpriteName)//是否使用地址方式加载
        {
            string atlasedSpriteAddress = spriteAtlasAddress + '[' + atlasedSpriteName + ']';
            Addressables.LoadAssetAsync<Sprite>(atlasedSpriteAddress).Completed += SpriteLoaded;
        }
        else
            newAtlasedSprite.LoadAssetAsync().Completed += SpriteLoaded;

        /*
        //加载spriteAtlas里 图片的第3种方法
         spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        Addressables.LoadAssetAsync<SpriteAtlas>(spriteAtlasAddress).Completed += SpriteAtlasLoaded;
        */

    }


    void ObjectLoadDone(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject loadedObject = obj.Result;
            Debug.Log("===  加载成功");
            instantiatedObject = Instantiate(loadedObject);
            Debug.Log("===生成物体成功");
            if (accessoryObjectToLoad != null)
            {
                //2, 加载方式2
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

    //加载spriteAtlas里 图片的第3种方法
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
