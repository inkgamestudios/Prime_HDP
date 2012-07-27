using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniMap : MonoBehaviour {
	
	public float updateRate=20f;
	private float updateInterval;
	
	
	public int minimapLayer;
	public Rect mapRect=new Rect(5, 5, 50, 50);
	public Vector2 mapCenter=new Vector2(0, 0);
	
	public Vector2 mapSize;
	public Texture mapTexture;
	
	private Transform camT;
	private Camera cam;
	
	private Transform thisT;
	
	//public int mapSize=50
	
	public enum _MapPanelAlignment{Top, Bottom, Left, Right}
	public _MapPanelAlignment panelAlignment;
	
	public Transform trackObj;
	public bool trackPosition=true;
	public bool trackRotation=true;
	private Transform mainCamT;
	
	private Vector2 panelStartPos;
	private Vector2 panelPadding;
	
	void Awake(){
		thisT=transform;
		
		thisT.position=new Vector3(mapCenter.x, 10, mapCenter.y);
		
		if(updateRate>0) updateInterval=1/updateRate;
		else updateInterval=0;		
	}
	
	#if !UNITY_IPHONE || !UNITY_ANDROID
	void OnGUI(){
		int x=(int)panelStartPos.x;
		int y=(int)panelStartPos.y;
		int padX=(int)panelPadding.x;
		int padY=(int)panelPadding.y;
		
		float rate=Time.deltaTime/Time.timeScale;
		
		if(GUI.RepeatButton(new Rect(x, y, 25, 25), "+")){
			cam.orthographicSize=Mathf.Max(0, cam.orthographicSize-25f*rate);
		}
		if(GUI.RepeatButton(new Rect(x+=padX, y+=padY, 25, 25), "-")){
			cam.orthographicSize=Mathf.Min(Mathf.Max(mapSize.x, mapSize.y)*0.5f, cam.orthographicSize+25f*rate);
		}
		if(GUI.Button(new Rect(x+=padX*2, y+=padY*2, 25, 25), "R")){
			trackRotation=!trackRotation;
			
			if(!trackRotation) camT.rotation=Quaternion.Euler(90, 0, 0);
		}
	}
	#endif

	// Use this for initialization
	void Start () {
		Rect mapRectNormalized=new Rect(mapRect.x/Screen.width, (Screen.height-mapRect.y-mapRect.height)/Screen.height, mapRect.width/Screen.width, mapRect.height/Screen.height);
		
		camT=new GameObject ("camera_minimap").transform;
		camT.parent=thisT;
		camT.position=thisT.position;
		camT.rotation=Quaternion.Euler(90, 0, 0);
		
		cam=camT.gameObject.AddComponent<Camera>();
		cam.orthographic=true;
		cam.orthographicSize=Mathf.Max(mapSize.x, mapSize.y)*0.5f; 
		cam.backgroundColor=new Color(0, 0, 0, 1);
		cam.clearFlags=CameraClearFlags.SolidColor;
		cam.depth=Mathf.Clamp(Camera.main.depth + 90, 0, 90);
		cam.rect=mapRectNormalized;
		cam.cullingMask=1<<minimapLayer;
		
		Camera mainCamera=Camera.main;
		mainCamera.cullingMask=~(1<<minimapLayer);
		if(trackObj==null) trackObj=mainCamera.transform;
		
		if(mapTexture!=null){
			Transform map=GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
			//map.transform.position=new Vector3(0, -10, 0);
			map.transform.localScale=new Vector3(0.1f*mapSize.x, 0, 0.1f*mapSize.y);
			//map.renderer.material.shader=shader;
			map.renderer.material.mainTexture=mapTexture;
			map.gameObject.layer=minimapLayer;
			map.gameObject.name="map";
			map.parent=thisT;
			map.localPosition=new Vector3(0, -100, 0);
			map.rotation=Quaternion.Euler(0, 180, 0);
			Destroy(map.collider);
		}
		
		foreach(Trackable trackable in trackables){
			Transform blip=GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
			blip.position=new Vector3(0, -10, 0);
			blip.localScale=new Vector3(0.1f*trackable.blipSizeModifier, 0, 0.1f*trackable.blipSizeModifier);
			blip.renderer.material.shader=Shader.Find("Particles/Alpha Blended");
			blip.renderer.material.mainTexture=trackable.blipTexture;
			blip.gameObject.layer=minimapLayer;
			//blip.localPosition=new Vector3(0, -100, 0);
			Destroy(blip.collider);
			
			blip.parent=thisT;
			trackable.blipPrefab=blip;
			
			if(trackable.maxNum==0) trackable.maxNum=50;
			ObjectPoolManager.New(trackable.blipPrefab.gameObject, trackable.maxNum);
			trackable.blipPrefab.gameObject.active=false;
		}
		
		if(panelAlignment==_MapPanelAlignment.Top){
			panelStartPos.x=mapRect.x;
			panelStartPos.y=mapRect.y-25;
			panelPadding=new Vector2(30, 0);
		}
		else if(panelAlignment==_MapPanelAlignment.Bottom){
			panelStartPos.x=mapRect.x;
			panelStartPos.y=mapRect.y+mapRect.height;
			panelPadding=new Vector2(30, 0);
		}
		else if(panelAlignment==_MapPanelAlignment.Left){
			panelStartPos.x=mapRect.x-25;
			panelStartPos.y=mapRect.y;
			panelPadding=new Vector2(0, 30);
		}
		else if(panelAlignment==_MapPanelAlignment.Right){
			panelStartPos.x=mapRect.x+mapRect.width;
			panelStartPos.y=mapRect.y;
			panelPadding=new Vector2(0, 30);
		}
		
		StartCoroutine(MiniMapUpdate());
	}
	
	// Update is called once per frame
	IEnumerator MiniMapUpdate () {
		while(true){
			CheckForUnusedObject();
			ScanForObject();
			
			DrawObject();
			
			yield return new WaitForSeconds(updateInterval);
		}
	}
	
	void Update(){
		if(trackPosition){
			camT.position=trackObj.position;
			
			float x=Mathf.Clamp(camT.position.x, mapCenter.x-mapSize.x/2+cam.orthographicSize, mapCenter.x+mapSize.x/2-cam.orthographicSize);
			float z=Mathf.Clamp(camT.position.z, mapCenter.y-mapSize.y/2+cam.orthographicSize, mapCenter.x+mapSize.y/2-cam.orthographicSize);
			float y=camT.position.y;
			
			camT.position=new Vector3(x, y, z);
			
		}
		if(trackRotation){
			camT.rotation=Quaternion.Euler(0, trackObj.rotation.eulerAngles.y, 0)*Quaternion.Euler(90, 0, 0);
		}
	}
	
	void DrawObject(){
		foreach(Trackable trackable in trackables){
			if(!trackable.isStatic){
				foreach(Blip blip in trackable.objList){
					blip.blipT.position=blip.sourceT.position;
					blip.blipT.rotation=blip.sourceT.rotation;
				}
			}
		}
	}
	
	void CheckForUnusedObject(){
		foreach(Trackable trackable in trackables){
			
			//look into this, maybe there's a more efficient way
			List<Blip> tempList=new List<Blip>();
			foreach(Blip blip in trackable.objList){
				tempList.Add(blip);
			}
			
			for(int i=0; i<tempList.Count; i++){
				Blip blip=tempList[i];
				
				if(blip.sourceObj==null || !blip.sourceObj.active){
					ObjectPoolManager.Unspawn(blip.blipT);
					tempList.RemoveAt(i);
					i-=1;
				}
			}
			
			trackable.objList=tempList.ToArray();
			
		}
	}
	
	void ScanForObject(){
		foreach(Trackable trackable in trackables){
			LayerMask mask=1<<trackable.layer;
			Collider[] objects=Physics.OverlapSphere(Vector3.zero, Mathf.Infinity, mask);
			
			List<Blip> tempList=new List<Blip>();
			
			foreach(Blip blip in trackable.objList){
				tempList.Add(blip);
			}
			
			foreach(Collider col in objects){
				
				Transform colT=col.transform;
				
				bool match=false;
				foreach(Blip blip in trackable.objList){
					if(colT==blip.sourceT){
						match=true;
						break;
					}
				}
				
				if(!match){
					//add to list
					Transform blipObj=ObjectPoolManager.Spawn(trackable.blipPrefab).transform;
					blipObj.parent=thisT;
					
					Blip blip=new Blip(colT.gameObject, colT, blipObj);
					tempList.Add(blip);
				}
				
			}
			
			//now add everything in the tempList to the actual list
			trackable.objList=tempList.ToArray();
			
		}
	}
	
	public List<Trackable> trackables;
}




[System.Serializable]
public class Trackable{
	public int layer=0;
	public Texture blipTexture;
	public float blipSizeModifier;
	
	public int maxNum=0;
	public bool isStatic=false;
	
	[HideInInspector] public Transform blipPrefab;
	[HideInInspector] public Blip[] objList;
}

[System.Serializable]
public class Blip{
	public GameObject sourceObj;
	public Transform sourceT;
	public Transform blipT;
	
	public Blip(GameObject srcObj, Transform srcT, Transform bT){
		sourceObj=srcObj;
		sourceT=srcT;
		blipT=bT;
	}
}