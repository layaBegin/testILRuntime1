using System.Collections;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Threading;

public class ILRuntimeInstance
{
    private static ILRuntimeInstance instance;

    //AppDomain��ILRuntime����ڣ��������һ���������б��棬������Ϸȫ�־�һ��������Ϊ��ʾ�����㣬ÿ���������涼��������һ��
    //�������ʽ��Ŀ����ȫ��ֻ����һ��AppDomain
    public  ILRuntime.Runtime.Enviorment.AppDomain appdomain;

    System.IO.MemoryStream fs;
    System.IO.MemoryStream p;

    public int a = 3;
    public event Action HotFixLoaded;//ί��
    public static ILRuntimeInstance Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ILRuntimeInstance();
            }
            return instance;

        }
    }

    public ILRuntime.Runtime.Enviorment.AppDomain Appdomain
    {
        get
        {
            if (appdomain == null)
            {
                appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            }
            return appdomain;
        }
    }




    public IEnumerator LoadHotFixAssembly()
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
        //OnHotFixLoaded();
        HotFixLoaded();
    }


    public void InitializeILRuntime()
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
            Debug.Log("!! Hotfix.InstanceClass.ID = " + str);
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

    public static async void Excute0()

    {

        Debug.Log(Thread.CurrentThread.GetHashCode() + " ��ʼ Excute " + DateTime.Now);

         int a = await AsyncTest();
        Debug.Log("==�����"+ a);
        Debug.Log(Thread.CurrentThread.GetHashCode() + " ���� Excute " + DateTime.Now);

    }


    public static async void Excute()

    {

        Debug.Log(Thread.CurrentThread.GetHashCode() + " ��ʼ Excute " + DateTime.Now);

        var waitTask = AsyncTestRun();

        waitTask.Start();

        int i = await waitTask;

        Debug.Log(Thread.CurrentThread.GetHashCode() + " i " + i);

        Debug.Log(Thread.CurrentThread.GetHashCode() + " ���� Excute " + DateTime.Now);

    }

   

    public static async Task<int> AsyncTest()

    {

        await Task.Run(() =>

        {
            Thread.Sleep(1000);

            Debug.Log(Thread.CurrentThread.GetHashCode() + " Run1 " + DateTime.Now);


        });

        return 1;

    }



    public static Task<int> AsyncTestRun()

    {

        Task<int> t = new Task<int>(() => {

            Debug.Log(Thread.CurrentThread.GetHashCode() + " Run1 " + DateTime.Now);

            Thread.Sleep(1000);

            return 100;

        });

        return t;

    }

}
