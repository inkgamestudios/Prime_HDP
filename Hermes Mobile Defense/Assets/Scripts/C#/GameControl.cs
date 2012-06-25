using UnityEngine;
using System.Collections;

public enum _GameState{Idle, Started, Ended}

[RequireComponent (typeof (ResourceManager))]
[RequireComponent (typeof (LayerManager))]

public class GameControl : MonoBehaviour {
	
	public delegate void GameOverHandler(bool win); //true if win
	public static event GameOverHandler GameOverE;
	
	public delegate void ResourceHandler(); 
	public static event ResourceHandler ResourceE;
	
	public delegate void LifeHandler(); 
	public static event LifeHandler LifeE;

	static public _GameState gameState=_GameState.Idle;
	
	//public int playerResource=100;
	public int playerLife=10;
	
	public float sellTowerRefundRatio=0.5f;

	[HideInInspector] public LayerManager layerManager;
	public SpawnManager spawnManager;
	private int totalWaveCount;
	private int currentWave=0;
	
	public Transform rangeIndicatorH;
	public Transform rangeIndicatorF;
	
	static public GameControl gameControl;
	
	public float buildingBarWidthModifier=1f;
	public float buildingBarHeightModifier=1f;
	public Vector3 buildingBarPosOffset=new Vector3(0, -0.5f, 0);
	

	void Awake(){
		ObjectPoolManager.Init();
		GameMessage.Init();
		
		AudioManager audioManager=(AudioManager)FindObjectOfType(typeof(AudioManager));
		if(audioManager==null){
			AudioManager.Init();
		}
		
		//LayerManager.Init();

		gameControl=this;
		
		gameState=_GameState.Idle;
		
		rangeIndicatorH=(Transform)Instantiate(rangeIndicatorH);
		rangeIndicatorH.parent=transform;
		rangeIndicatorF=(Transform)Instantiate(rangeIndicatorF);
		rangeIndicatorF.parent=transform;
		ClearIndicator();
		
		OverlayManager.SetModifier(buildingBarWidthModifier, buildingBarHeightModifier);
		OverlayManager.SetOffset(buildingBarPosOffset);
	}

	// Use this for initialization
	void Start () {
		totalWaveCount=spawnManager.waves.Length;
		
		SpawnManager.WaveStartSpawnE += WaveStartSpawned;
		SpawnManager.WaveClearedE += WaveCleared;
		//SpawnManager.WaveSpawnedE += WaveSpawned;
		
		Unit.ScoreE += DeductLife;
		UnitTower.DestroyE += TowerDestroy;
		
		//Create OverlayCamera
		Camera mainCam=Camera.main;
		Transform mainCamT=mainCam.transform;
		
		GameObject overlayCamObj=new GameObject();
		overlayCamObj.name="OverlayCamera";
		
		LayerMask layer=1<<LayerManager.LayerOverlay();
		mainCam.cullingMask=mainCam.cullingMask&~layer;
		
		Camera overlayCam=overlayCamObj.AddComponent<Camera>();
		
		overlayCam.clearFlags=CameraClearFlags.Depth;
		overlayCam.depth=mainCam.depth + 1;
		overlayCam.cullingMask=layer;
		overlayCam.fieldOfView=mainCam.fieldOfView;
		
		overlayCamObj.transform.parent=mainCamT;
		overlayCamObj.transform.rotation=mainCamT.rotation;
		overlayCamObj.transform.localPosition=Vector3.zero;
		
		Time.timeScale=1;
	}
	
	void OnDisable(){
		SpawnManager.WaveStartSpawnE -= WaveStartSpawned;
		SpawnManager.WaveClearedE -= WaveCleared;
		//SpawnManager.WaveSpawnedE -= WaveSpawned;
		
		Unit.ScoreE -= DeductLife;
		UnitTower.DestroyE -= TowerDestroy;
	}
	
	void DeductLife(int waveID){
		playerLife-=1;
		if(playerLife<=0) playerLife=0;
		
		if(LifeE!=null) LifeE();
		
		if(playerLife==0){
			//game over, player lost
			gameState=_GameState.Ended;
			if(GameOverE!=null) GameOverE(false);
		}
	}
	
	void TowerDestroy(UnitTower tower){
		if(selectedTower==tower || selectedTower==null || !selectedTower.thisObj.active){
			ClearSelection();
		}
	}
	
	//IEnumerator _TowerDestroy(UnitTower tower)
	
	
	public static int GetPlayerLife(){
		return gameControl.playerLife;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	void WaveStartSpawned(int waveID){
		currentWave+=1;
		
		//if game is not yet started, start it now
		if(gameState==_GameState.Idle) gameState=_GameState.Started;
	}
	
	void WaveSpawned(int waveID){
		
	}
	
	void WaveCleared(int waveID){
		Debug.Log("Wave "+waveID+" has been cleared");
		if(waveID==totalWaveCount-1){
			//game over, player won
			gameState=_GameState.Ended;
			if(GameOverE!=null) GameOverE(true);
		}
	}
	
	
	
	static public UnitTower selectedTower;
	
	static public UnitTower Select(Vector3 pointer){
		//change this
		int layer=LayerManager.LayerTower();
		
		LayerMask mask=1<<layer;
		Ray ray = Camera.main.ScreenPointToRay(pointer);
		RaycastHit hit;
		if(!Physics.Raycast(ray, out hit, Mathf.Infinity, mask)){
			return null;
		}
		
		selectedTower=hit.transform.gameObject.GetComponent<UnitTower>();
		//selectedTower.Select();
		
		gameControl._ShowIndicator(selectedTower);
		
		return selectedTower;
	}
	
	public static void ShowIndicator(UnitTower tower){
		gameControl._ShowIndicator(tower);
	}
	
	public void _ShowIndicator(UnitTower tower){
		//show range indicator on the tower
		//for support tower, show friendly range indicator
		if(tower.type==_TowerType.SupportTower){
			//Debug.Log(tower.type);
			float range=tower.GetRange();
			if(rangeIndicatorF!=null){
				rangeIndicatorF.position=tower.thisT.position;
				rangeIndicatorF.localScale=new Vector3(2*range/10, 1, 2*range/10);
				rangeIndicatorF.renderer.enabled=true;
			}
			if(rangeIndicatorH!=null) rangeIndicatorH.renderer.enabled=false;
		}
		//for support tower, show hostile range indicator
		else if(tower.type!=_TowerType.ResourceTower){
			float range=tower.GetRange();
			if(rangeIndicatorH!=null){
				rangeIndicatorH.position=tower.thisT.position;
				rangeIndicatorH.localScale=new Vector3(2*range/10, 1, 2*range/10);
				rangeIndicatorH.renderer.enabled=true;
			}
			if(rangeIndicatorF!=null) rangeIndicatorF.renderer.enabled=false;
		}
	}
	
	public static void DragNDropIndicator(UnitTower tower){
		if(tower.type!=_TowerType.ResourceTower){
			gameControl._ShowIndicator(tower);
			gameControl.StartCoroutine(gameControl._DragNDropIndicator(tower));
		}
	}
	IEnumerator _DragNDropIndicator(UnitTower tower){
		while(tower.thisObj!=null && tower.GetTowerID()==-1){
			if(tower.type==_TowerType.SupportTower){
				if(rangeIndicatorF!=null) rangeIndicatorF.position=tower.thisT.position;
			}
			else{
				if(rangeIndicatorH!=null) rangeIndicatorH.position=tower.thisT.position;
			}
				
			yield return null;
		}
		ClearIndicator();
	}
	
	public static void ClearIndicator(){
		gameControl._ClearIndicator();
	}
	
	public void _ClearIndicator(){
		if(rangeIndicatorH!=null) rangeIndicatorH.renderer.enabled=false;
		if(rangeIndicatorF!=null) rangeIndicatorF.renderer.enabled=false;
	}
	
	static public void ClearSelection(){
		selectedTower=null;
		gameControl._ClearIndicator();
	}
	
	//call when a tower complete upgrade, if tower is currently selected, update the range indicator
	static public void TowerUpgradeComplete(UnitTower tower){
		if(tower==selectedTower){
			gameControl._ShowIndicator(tower);
		}
	}
	
	
	static public void GainResource(int val){
		//Debug.Log("gain");
		ResourceManager.GainResource(0, val);
		if(ResourceE!=null) ResourceE();
	}
	
	static public void GainResource(int id, int val){
		//Debug.Log("gain");
		ResourceManager.GainResource(id, val);
		if(ResourceE!=null) ResourceE();
	}
	
	static public void GainResource(int[] val){
		//Debug.Log("gain");
		ResourceManager.GainResource(val);
		if(ResourceE!=null) ResourceE();
	}
	
	static public void SpendResource(int[] val){
		ResourceManager.SpendResource(val);
		if(ResourceE!=null) ResourceE();
	}
	
	static public int GetResourceVal(){
		return ResourceManager.GetResourceVal(0);
	}
	
	static public int GetResourceVal(int id){
		return ResourceManager.GetResourceVal(id);
	}
	
	static public int[] GetAllResourceVal(){
		Resource[] rscList=ResourceManager.GetResourceList();
		
		int[] valList=new int[rscList.Length];
		for(int i=0; i<valList.Length; i++){
			valList[i]=rscList[i].value;
		}
		
		return valList;
	}
	
	static public bool HaveSufficientResource(int[] cost){
		return ResourceManager.HaveSufficientResource(cost);
	}
	
	static public Resource[] GetResourceList(){
		return ResourceManager.GetResourceList();
	}
	
	static public float GetSellTowerRefundRatio(){
		return gameControl.sellTowerRefundRatio;
	}
	
	static public void GradualPause(){
		
	}
	
	static public void GradualResume(){
		
	}
}
