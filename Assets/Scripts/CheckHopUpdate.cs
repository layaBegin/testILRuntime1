using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class CheckHopUpdate : MonoBehaviour
{

    public Button btnLoadServer;
    public Button btnUpdateCatalog;
    public Button btnUpdateScene;
    public Button btnUpdateStatic;
    public Button btnUpdateNoStatic;
    //Addressables.LoadContentCatalogAsync

    private List<object> _updateKeys  = new List<object>();
    public Transform[] pos;
    public Text outputText;
    public string m_SceneAddressToLoad;
    public string remote_static;
    public string remote_non_static;
    private string str = "";
    // Start is called before the first frame update
    void Start()
    {
        //btnLoadServer.onClick.AddListener(DownLoad);
        btnUpdateCatalog.onClick.AddListener(UpdateCatalog);
        //btnUpdateScene.onClick.AddListener(LoadGamePlayScene);
        //btnUpdateStatic.onClick.AddListener(RemoteStatic);
        btnUpdateNoStatic.onClick.AddListener(RemoteNonStatic);
    }

    ///加载场景
    public void LoadGamePlayScene()
    {
        Addressables.LoadSceneAsync(m_SceneAddressToLoad, UnityEngine.SceneManagement.LoadSceneMode.Single).Completed += LoadScene_Completed;
    }

    private void LoadScene_Completed(AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            UnityEngine.ResourceManagement.ResourceProviders.SceneInstance sceneInstance = obj.Result;
            sceneInstance.ActivateAsync();
        }
    }


    /// <summary>
    /// 加载实例化远程静态资源
    /// </summary>
    public void RemoteStatic()
    {
        Addressables.InstantiateAsync(remote_static, pos[0].transform.position, Quaternion.identity, pos[0]);
    }
    /// <summary>
    /// 加载实例化远程非静态资源
    /// </summary>
    public void RemoteNonStatic()
    {
        Debug.Log("===点击加载预制体");
        ShowLog("==点击加载预制体remote_non_static： "+ remote_non_static);
        Addressables.InstantiateAsync(remote_non_static, pos[1].transform.position, Quaternion.identity, pos[1]).Completed += onPfbCompleted;
    }

    private void onPfbCompleted(AsyncOperationHandle<GameObject> obj)
    {
        Debug.Log("===点击加载预制体 是否成功："+ obj.Status);
        ShowLog("===点击加载预制体 是否成功：" + obj.Status);
        
    }

    /// <summary>
    /// 对比更新Catalog。必要
    /// </summary>
    public async void UpdateCatalog()
    {
        str = "";
        //开始连接服务器检查更新
        var handle = Addressables.CheckForCatalogUpdates(false);
        await handle.Task;
        Debug.Log("===check catalog status " + handle.Status);
        ShowLog("==check catalog status " + handle.Status);
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            List<string> catalogs = handle.Result;
            if (catalogs != null && catalogs.Count > 0)
            {
                foreach (var catalog in catalogs)
                {
                    Debug.Log("==catalog  " + catalog);
                    ShowLog("==catalog:  " + catalog);
                }
                Debug.Log("==download catalog start ");
                str += "==download catalog start \n";
                outputText.text = str;
                var updateHandle = Addressables.UpdateCatalogs(catalogs, false);
                await updateHandle.Task;
                foreach (var item in updateHandle.Result)
                {
                    Debug.Log("==catalog result " + item.LocatorId);
                    ShowLog("==catalog result " + item.LocatorId);
                    foreach (var key in item.Keys)
                    {
                        Debug.Log("==catalog key " + key);
                        ShowLog("==download catalog key: " + key);
                    }
                    _updateKeys.AddRange(item.Keys);
                }
                Debug.Log("==download catalog finish " + updateHandle.Status);
                ShowLog("==download catalog finish " + updateHandle.Status);
            }
            else
            {
                Debug.Log("==dont need update catalogs");
                ShowLog("==dont need update catalogs");
            }
        }
        Addressables.Release(handle);
    }
    /// <summary>
    /// 主界面显示Log
    /// </summary>
    /// <param name="textStr"></param>
    private void ShowLog(string textStr)
    {
        str += textStr + "\n";
        outputText.text = str;
    }

    public IEnumerator DownAssetImpl()
    {
        var downloadsize = Addressables.GetDownloadSizeAsync(_updateKeys);
        yield return downloadsize;
        Debug.Log("==start download size :" + downloadsize.Result);
        ShowLog("==start download size :" + downloadsize.Result);
        if (downloadsize.Result > 0)
        {
            var download = Addressables.DownloadDependenciesAsync(_updateKeys);//, Addressables.MergeMode.Union
            yield return download;
            //await download.Task;
            Debug.Log("==download result type: " + download.Result.GetType());
            ShowLog("==download result type: " + download.Result.GetType());
            foreach (var item in download.Result as List<UnityEngine.ResourceManagement.ResourceProviders.IAssetBundleResource>)
            {
                var ab = item.GetAssetBundle();
                Debug.Log("==ab name " + ab.name);
                ShowLog("==ab name: " + ab.name);
                foreach (var name in ab.GetAllAssetNames())
                {
                    Debug.Log("==asset name " + name);
                    ShowLog("==name: " + name);
                }
            }
            Addressables.Release(download);
        }
        Addressables.Release(downloadsize);
    }
    /// <summary>
    /// 下载资源 , 不需要也能热更成功 
    /// </summary>
    public void DownLoad()
    {
        str = "";
        StartCoroutine(DownAssetImpl());
        //CtrlLoadSlider();
    }

}
