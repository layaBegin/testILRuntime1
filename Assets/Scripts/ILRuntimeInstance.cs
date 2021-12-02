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

    //AppDomain是ILRuntime的入口，最好是在一个单例类中保存，整个游戏全局就一个，这里为了示例方便，每个例子里面都单独做了一个
    //大家在正式项目中请全局只创建一个AppDomain
    public  ILRuntime.Runtime.Enviorment.AppDomain appdomain;

    System.IO.MemoryStream fs;
    System.IO.MemoryStream p;

    public int a = 3;
    public event Action HotFixLoaded;//委托
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
        //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
        //正常项目中应该是自行从其他地方下载dll，或者打包在AssetBundle中读取，平时开发以及为了演示方便直接从StreammingAssets中读取，
        //正式发布的时候需要大家自行从其他地方读取dll

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //这个DLL文件是直接编译HotFix_Project.sln生成的，已经在项目中设置好输出目录为StreamingAssets，在VS里直接编译即可生成到对应目录，无需手动拷贝
        //工程目录在Assets\Samples\ILRuntime\1.6\Demo\HotFix_Project~
        //以下加载写法只为演示，并没有处理在编辑器切换到Android平台的读取，需要自行修改
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

        //PDB文件是调试数据库，如需要在日志中显示报错的行号，则必须提供PDB文件，不过由于会额外耗用内存，正式发布时请将PDB去掉，下面LoadAssembly的时候pdb传null即可
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
            Debug.LogError("加载热更DLL失败，请确保已经通过VS打开Assets/MyHotFix.sln编译过热更DLL");
        }

        InitializeILRuntime();
        //OnHotFixLoaded();
        HotFixLoaded();
    }


    public void InitializeILRuntime()
    {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
        //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
        appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
        //这里做一些ILRuntime的注册，HelloWorld示例暂时没有需要注册的
    }

    void OnHotFixLoaded()
    {
        //HelloWorld，第一次方法调用
        //appdomain.Invoke("Hotfix.Class1", "StaticFunTest", null, null);
        IType type = appdomain.LoadedTypes["MyHotFix.TestHotFix"];
        object obj = ((ILType)type).Instantiate();
        IMethod method = type.GetMethod("test1", 0);
        using (var ctx = appdomain.BeginInvoke(method)) //using 用法，离开了这个程序集就会被释放
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

        Debug.Log(Thread.CurrentThread.GetHashCode() + " 开始 Excute " + DateTime.Now);

         int a = await AsyncTest();
        Debug.Log("==结果："+ a);
        Debug.Log(Thread.CurrentThread.GetHashCode() + " 结束 Excute " + DateTime.Now);

    }


    public static async void Excute()

    {

        Debug.Log(Thread.CurrentThread.GetHashCode() + " 开始 Excute " + DateTime.Now);

        var waitTask = AsyncTestRun();

        waitTask.Start();

        int i = await waitTask;

        Debug.Log(Thread.CurrentThread.GetHashCode() + " i " + i);

        Debug.Log(Thread.CurrentThread.GetHashCode() + " 结束 Excute " + DateTime.Now);

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
