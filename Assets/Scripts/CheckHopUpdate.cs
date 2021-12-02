using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class CheckHopUpdate : MonoBehaviour
{

    public Button btnLoadServer;
    public Button btnUpdateCatalog;
    public Button btnUpdateScene;
    public Button btnUpdateStatic;
    public Button btnUpdateNoStatic;
    public Button btnUpdateDll;
    //Addressables.LoadContentCatalogAsync

    private List<object> _updateKeys  = new List<object>();
    public Transform[] pos;
    public Text outputText;
    public string m_SceneAddressToLoad;
    public string remote_static;
    public string remote_non_static;
    public string dllPath = "";
    public string pbPath = "";
    private string str = "";


    private static string DLLPath_File = Application.streamingAssetsPath + "/Addressable/ILRuntime/MyHotFix_dll_res.bytes";
    private static string PBDPath_File = Application.streamingAssetsPath + "/Addressable/ILRuntime/MyHotFix_pdb_res.bytes";

    // Start is called before the first frame update
    void Start()
    {
        //btnLoadServer.onClick.AddListener(DownLoad);
        btnUpdateCatalog.onClick.AddListener(UpdateCatalog);
        //btnUpdateScene.onClick.AddListener(LoadGamePlayScene);
        //btnUpdateStatic.onClick.AddListener(RemoteStatic);
        btnUpdateNoStatic.onClick.AddListener(RemoteNonStatic);
        btnUpdateDll.onClick.AddListener(onBtnLoadScriptsAsync);
    }

    ///���س���
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
    /// ����ʵ����Զ�̾�̬��Դ
    /// </summary>
    public void RemoteStatic()
    {
        Addressables.InstantiateAsync(remote_static, pos[0].transform.position, Quaternion.identity, pos[0]);
    }
    /// <summary>
    /// ����ʵ����Զ�̷Ǿ�̬��Դ
    /// </summary>
    public void RemoteNonStatic()
    {
        Debug.Log("===�������Ԥ����");
        ShowLog("==�������Ԥ����remote_non_static�� "+ remote_non_static);
        Addressables.InstantiateAsync(remote_non_static, pos[1].transform.position, Quaternion.identity, pos[1]).Completed += onPfbCompleted;
    }

    public async void onBtnLoadScriptsAsync()
    {
       // ILRuntimeInstance.Instance.HotFixLoaded += HotFixInvoke;

        await setAppDomainAsync();
        //setAppDomainAsyncLocal();


        //StartCoroutine(ILRuntimeInstance.Instance.LoadHotFixAssembly());

    }

    public void HotFixInvoke()
    {
        //HelloWorld����һ�η�������
        //appdomain.Invoke("Hotfix.Class1", "StaticFunTest", null, null);
        IType type = ILRuntimeInstance.Instance.appdomain.LoadedTypes["MyHotFix.TestHotFix"];
        object obj = ((ILType)type).Instantiate();
        IMethod method = type.GetMethod("test2", 0);
        using (var ctx = ILRuntimeInstance.Instance.appdomain.BeginInvoke(method)) //using �÷����뿪��������򼯾ͻᱻ�ͷ�
        {
            ctx.PushObject(obj);
            ctx.Invoke();
            string str = ctx.ReadObject<string>();
            Debug.Log("!! Hotfix.InstanceClass.ID = "+ str);
            ShowLog($"===��ӡ��{str}");
        }
    }

    private void onPfbCompleted(AsyncOperationHandle<GameObject> obj)
    {
        Debug.Log("===�������Ԥ���� �Ƿ�ɹ���"+ obj.Status);
        ShowLog("===�������Ԥ���� �Ƿ�ɹ���" + obj.Status);
        
    }

    /// <summary>
    /// �Աȸ���Catalog����Ҫ
    /// </summary>
    public async void UpdateCatalog()
    {
        str = "";
        //��ʼ���ӷ�����������
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
    /// ��������ʾLog
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
    /// ������Դ , ����ҪҲ���ȸ��ɹ� 
    /// </summary>
    public void DownLoad()
    {
        str = "";
        StartCoroutine(DownAssetImpl());
        //CtrlLoadSlider();
    }



    //���ز���DLL bytes �ļ�
    public void setAppDomainAsyncLocal()
    {
        byte[] dll = LoadFile(dllPath);
        byte[] pdb = LoadFile(pbPath);
        MemoryStream fs = new MemoryStream(dll);
        MemoryStream p = new MemoryStream(pdb);
        try
        {
            ILRuntimeInstance.Instance.Appdomain.LoadAssembly(fs, p, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
        }
        catch
        {
            Debug.LogError("�����ȸ�DLLʧ�ܣ���ȷ���Ѿ�ͨ��VS��Assets/Samples/ILRuntime/1.6/Demo/HotFix_Project/HotFix_Project.sln������ȸ�DLL");
        }
        ILRuntimeInstance.Instance.InitializeILRuntime();
        HotFixInvoke();
    }

    //���ȸ��м���DLL �ļ�
    public async Task setAppDomainAsync()
    {

        byte[] dll = await LoadFile_Addressables(dllPath);
        byte[] pdb = await LoadFile_Addressables(pbPath);
        MemoryStream fs = new MemoryStream(dll);
        MemoryStream p = new MemoryStream(pdb);
        try
        {
            ILRuntimeInstance.Instance.Appdomain.LoadAssembly(fs, p, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
        }
        catch
        {
            Debug.LogError("�����ȸ�DLLʧ�ܣ���ȷ���Ѿ�ͨ��VS��Assets/Samples/ILRuntime/1.6/Demo/HotFix_Project/HotFix_Project.sln������ȸ�DLL");
        }
        ILRuntimeInstance.Instance.InitializeILRuntime();
        HotFixInvoke();
    }

    //����dll bytes�ļ�
    private static async Task<byte[]> LoadFile_WebRequest(string path)
    {
        var request = UnityWebRequest.Get(path); 
        request.SendWebRequest();
        while (!request.isDone)
            await Task.Delay(200);
        if (!string.IsNullOrEmpty(request.error))
            Debug.LogError(request.error);
        byte[] bytes = request.downloadHandler.data;
        //�����������
        request.Dispose();

        return bytes;
    }


    //���� dll bytes�ļ�
    private static byte[] LoadFile(string path)
    {
        Debug.Log(path);
        return System.IO.File.ReadAllBytes(path);
    }
    private static async Task<byte[]> LoadFile_Addressables(string path)
    {
        Debug.Log(path);
        var text = await Addressables.LoadAssetAsync<TextAsset>(path).Task;

        return text.bytes;
    }
}
