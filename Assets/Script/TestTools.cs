using UnityEngine;
using System.Collections;

public class TestTools : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //Debug.Log(ABTools.Instance.Get(10));

        AssetBundle ab = ABTools.Instance.GetABByName("Cube.ab");

        Instantiate( ab.LoadAsset<GameObject>("Cube"));

        //ABTools.Instance.DebugLog();


    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
