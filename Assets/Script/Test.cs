using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Test : MonoBehaviour {

    public Image image;

    public AssetBundle ab;

    public Sprite sp;

	// Use this for initialization
	void Start () {

        //1.加载AB包
        ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundle/scene.normal");

        //2.从AB包中加载你需要的资源
        sp = ab.LoadAsset<Sprite>("CS1");

        Sprite sp1 = Instantiate(sp) as Sprite;

        image.sprite = sp1;

        image.SetNativeSize();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("卸载ab包");
            ab.Unload(false);
        }
	}
}
