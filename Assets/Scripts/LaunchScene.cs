using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LaunchScene : MonoBehaviour
{
    //AppDomain��ILRuntime����ڣ��������һ���������б��棬������Ϸȫ�־�һ��������Ϊ��ʾ�����㣬ÿ���������涼��������һ��
    //�������ʽ��Ŀ����ȫ��ֻ����һ��AppDomain
    AppDomain appdomain;

    System.IO.MemoryStream fs;
    System.IO.MemoryStream p;

    private void Awake()
    {
        //DontDestroyOnLoad(transform.gameObject);
    }
    void Start()
    {
        StartCoroutine(LoadHotFixAssembly());
    }

    IEnumerator LoadHotFixAssembly()
    {
        //����ʵ����ILRuntime��AppDomain��AppDomain��һ��Ӧ�ó�����ÿ��AppDomain����һ��������ɳ��
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
        //������Ŀ��Ӧ�������д������ط�����dll�����ߴ����AssetBundle�ж�ȡ��ƽʱ�����Լ�Ϊ����ʾ����ֱ�Ӵ�StreammingAssets�ж�ȡ��
        //��ʽ������ʱ����Ҫ������д������ط���ȡdll

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //���DLL�ļ���ֱ�ӱ���HotFix_Project.sln���ɵģ��Ѿ�����Ŀ�����ú����Ŀ¼ΪStreamingAssets����VS��ֱ�ӱ��뼴�����ɵ���ӦĿ¼�������ֶ�����
        //����Ŀ¼��Assets\Samples\ILRuntime\1.6\Demo\HotFix_Project~
        //���¼���д��ֻΪ��ʾ����û�д����ڱ༭���л���Androidƽ̨�Ķ�ȡ����Ҫ�����޸�
#if UNITY_ANDROID
        WWW www = new WWW(Application.streamingAssetsPath + "/Addressable/ILRuntime" + "/MyHotFix.dll");
#else
        WWW www = new WWW("file:///" + Application.streamingAssetsPath + "/Addressable/ILRuntime" + "/MyHotFix.dll");
#endif
        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            UnityEngine.Debug.LogError(www.error);
        byte[] dll = www.bytes;
        www.Dispose();

        //PDB�ļ��ǵ������ݿ⣬����Ҫ����־����ʾ������кţ�������ṩPDB�ļ����������ڻ��������ڴ棬��ʽ����ʱ�뽫PDBȥ��������LoadAssembly��ʱ��pdb��null����
#if UNITY_ANDROID
        www = new WWW(Application.streamingAssetsPath + "/Addressable/ILRuntime" + "/MyHotFix.pdb");
#else
        www = new WWW("file:///" + Application.streamingAssetsPath + "/Addressable/ILRuntime" + "/MyHotFix.pdb");
#endif
        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            UnityEngine.Debug.LogError(www.error);
        byte[] pdb = www.bytes;
        fs = new MemoryStream(dll);
        p = new MemoryStream(pdb);
        try
        {
            appdomain.LoadAssembly(fs, p, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
        }
        catch
        { 
            Debug.LogError("�����ȸ�DLLʧ�ܣ���ȷ���Ѿ�ͨ��VS��Assets/MyHotFix.sln������ȸ�DLL");
        }

        InitializeILRuntime();
        OnHotFixLoaded();
    }

    void InitializeILRuntime()
    {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
        //����Unity��Profiler�ӿ�ֻ���������߳�ʹ�ã�Ϊ�˱�����쳣����Ҫ����ILRuntime���̵߳��߳�ID������ȷ���������к�ʱ�����Profiler
        appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
        //������һЩILRuntime��ע�ᣬHelloWorldʾ����ʱû����Ҫע���
    }

    void OnHotFixLoaded()
    {
        //HelloWorld����һ�η�������
        //appdomain.Invoke("Hotfix.Class1", "StaticFunTest", null, null);
        IType type = appdomain.LoadedTypes["MyHotFix.TestHotFix"];
        object obj = ((ILType)type).Instantiate();
        IMethod method = type.GetMethod("test1", 0);
        using (var ctx = appdomain.BeginInvoke(method)) //using �÷����뿪��������򼯾ͻᱻ�ͷ�
        {
            ctx.PushObject(obj);
            ctx.Invoke();
            string str = ctx.ReadObject<string>();
            Debug.Log("!! Hotfix.InstanceClass.ID = "+ str);
        }

    }

    private void OnDestroy()
    {
        if (fs != null)
            fs.Close();
        if (p != null)
            p.Close();
        fs = null;
        p = null;
    }
}
