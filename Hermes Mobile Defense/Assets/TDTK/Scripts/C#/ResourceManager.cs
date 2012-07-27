using UnityEngine;
using System.Collections;

[System.Serializable]
public class Resource{
	[HideInInspector] public int ID=0;
	
	public string name="Resource";
	public Texture icon;
	
	public int value=0;
}


public class ResourceManager : MonoBehaviour {

	public Resource[] resources=new Resource[1];
	
	static ResourceManager resourceManager;
	
	
	
	void Awake(){
		resourceManager=this;
		
		for(int i=0; i<resources.Length; i++){
			if(resources[i]==null) resources[i]=new Resource();
		}
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	public static void GainResource(int val){
		resourceManager._GainResource(0, val);
	}
	
	public static void GainResource(int id, int val){
		resourceManager._GainResource(id, val);
	}
	
	void _GainResource(int id, int val){
		if(resources.Length>id){
			resources[id].value=Mathf.Max(0, resources[id].value+=val);
		}
		else Debug.Log("resource type unconfigured");
	}
	
	public static void GainResource(int[] val){
		resourceManager._GainResource(val);
	}
	
	void _GainResource(int[] val){
		for(int i=0; i<val.Length; i++){
			if(i>=resources.Length){
				Debug.Log("resource gain contain unconfigured resource type");
				return;
			}
			else {
				resources[i].value+=val[i];
			}
		}
	}
	
	public static void SpendResource(int[] val){
		resourceManager._SpendResource(val);
	}
	
	void _SpendResource(int[] val){
		for(int i=0; i<val.Length; i++){
			if(i>=resources.Length){
				Debug.Log("costs contain unconfigured resource type");
				return;
			}
			else {
				resources[i].value-=val[i];
				if(resources[i].value<0) resources[i].value=0;
			}
		}
	}
	
	
	public static int GetResourceVal(){
		return resourceManager._GetResource(0);
	}
	
	public static int GetResourceVal(int id){
		return resourceManager._GetResource(id);
	}
	
	int _GetResource(int id){
		if(resources.Length>id){
			return resources[id].value;
		}
		
		return 0;
	}
	
	public static Resource[] GetResourceList(){
		return resourceManager.resources;
	}

	
	public static bool HaveSufficientResource(int[] cost){
		return resourceManager._HaveSufficientResource(cost);
	}
	
	bool _HaveSufficientResource(int[] cost){
		for(int i=0; i<cost.Length; i++){
			//Debug.Log("have:"+resources[i].value+"   cost:"+cost[i]);
			if(i>=resources.Length){
				Debug.Log("costs contain unconfigured resource type");
				return false;
			}
			else if(resources[i].value<cost[i]){
				return false;
			}
		}
		
		return true;
	}
	
}



