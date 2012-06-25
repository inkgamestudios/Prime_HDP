using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UnitAttribute{
	public float fullHP=10;
	public float HP=10;
	
	public Transform overlayHP;
	public Transform overlayBase;
	public bool alwaysShowOverlay=false;
}

public class Unit : MonoBehaviour {

	public string unitName;
	public Texture icon;
	
	public delegate void DeadHandler(int waveID);
	public static event DeadHandler DeadE;
	
	public delegate void ScoreHandler(int waveID);
	public static event ScoreHandler ScoreE;
	
	public UnitAttribute HPAttribute;
	
	private Vector3 overlayScaleH;
	//private Vector3 overlayOrigin;
	
	private Transform cam;
	private Quaternion offset;
	private bool overlayIsVisible=false;
	
	private bool dead=false;
	private bool scored=false;
	
	[HideInInspector] public Transform thisT;
	[HideInInspector] public GameObject thisObj;
	
	//waypoint and movement related variable
	[HideInInspector] public bool wpMode=false; //if set to true, use wp, else use path
	[HideInInspector] public Path path;
	[HideInInspector] public List<Vector3> wp=new List<Vector3>();
	[HideInInspector] public float currentMoveSpd;
	private float rotateSpd=10;
	[HideInInspector] public int wpCounter=0;
	
	//function and configuration to set if this intance has any been inerited by any child instance
	//these functions are call in Awake() of the inherited clss
	private enum _UnitSubClass{None, Creep, Tower};
	private _UnitSubClass subClass=_UnitSubClass.None;
	private UnitCreep unitC;
	private UnitTower unitT;
	//Call by inherited class UnitCreep, caching inherited UnitCreep instance to this instance
	public void SetSubClassInt(UnitCreep unit){ 
		unitC=unit; 
		subClass=_UnitSubClass.Creep;
		currentMoveSpd=unitC.moveSpeed;
		if(unitC.flying){
			_flying=true;
			_flightHeightOffset=unitC.flightHeightOffset;
		}
	}
	//Call by inherited class UnitTower, caching inherited UnitTower instance to this instance
	public void SetSubClassInt(UnitTower unit){ 
		unitT=unit; 
		subClass=_UnitSubClass.Tower;
		currentMoveSpd=unitT.moveSpeed;
	}
	
	private bool _flying=false;
	private float _flightHeightOffset=0;
	
	public virtual void Awake(){
		thisT=transform;
		thisObj=gameObject;
		
		//if(HPAttribute.overlayBase!=null) HPAttribute.overlayBase.gameObject.layer=LayerManager.LayerOverlay();
		if(HPAttribute.overlayHP!=null) {
			overlayScaleH=HPAttribute.overlayHP.localScale;
			//overlayOrigin=HPAttribute.overlayHP.localPosition;
			
			HPAttribute.overlayHP.gameObject.layer=LayerManager.LayerOverlay();
		}
		
		cam=Camera.main.transform;
		offset=Quaternion.Euler(-90, 0, 0);

		UnitUtility.DestroyColliderRecursively(thisT);
		
	}


	// Use this for initialization
	public virtual void Start () {
		Init();
		
		//StartCoroutine(TestOverlay());
	}
	
	
	public virtual void Init(){
		HPAttribute.HP=HPAttribute.fullHP;
		UpdateOverlay();
		
		//reset waypoint
		SetWPCounter(0);
		currentPS=null;
		
		dead=false;
		scored=false;
	}
	
	
	public void SetFullHP(float hp){
		HPAttribute.fullHP=hp;
		HPAttribute.HP=HPAttribute.fullHP;
	}
	
	
	//a test function call to demonstrate overlay in action
	IEnumerator TestOverlay(){
		yield return new WaitForSeconds(0.75f);
		while(true){
			ApplyDamage(0.1f*HPAttribute.fullHP*0.1f);
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	// Update is called once per frame
	public virtual void Update () {
		if(overlayIsVisible) AdjustOverlayPosition();
		
		if(!stunned && !dead) {
			//execute appropriate move routine
			if(wpMode) MoveWPMode();
			else MovePathMode();
		}
		
		//if(unitC!=null) Debug.Log(wpCounter+"   "+thisObj.name);
	}
	
	[HideInInspector] public PathSection currentPS;
	[HideInInspector] public List<Vector3> subPath=new List<Vector3>();
	[HideInInspector] public int currentPathID=0;
	[HideInInspector] public int subWPCounter=0;
	
	//this is to resume a half completed path, called when the unit is a UnitCreep spawned by other UnitCreep
	public IEnumerator ResumeParentPath(bool wpM, List<Vector3> w, int wpC, PathSection cPS, List<Vector3> sP, int pID, int subWPC){
		yield return null;
		
		wpMode=wpM;
		wpCounter=wpC;
		wp=w;
		
		currentPS=cPS;
		subPath=sP;
		currentPathID=pID;
		subWPCounter=subWPC;
		
		//Debug.Log("resume path "+wpCounter);
	}
	
	//get subpath from current pathSection
	private void GetSubPath(){
		subPath=currentPS.GetSectionPath();
		currentPathID=currentPS.GetPathID();
		subWPCounter=0;
	}
	
	//find a new independant subpath based on current pathSection's platform graph
	private void SearchSubPath(){
		currentPathID=currentPS.GetPathID();
		
		Vector3 pos=thisT.TransformPoint(0, 0, BuildManager.GetGridSize());
		
		PathFinder.GetPath(pos, subPath[subPath.Count-1], currentPS.platform.GetNodeGraph(), this.SetSubPath);
		//PathFinder.GetPath(thisT.position, subPath[subPath.Count-1], currentPS.platform.GetNodeGraph(), this.SetSubPath);
	}
	
	//callback function for PathFinder
	public void SetSubPath(List<Vector3> wp){
		subPath=wp;
		subWPCounter=0;
	}
	
	//for complex mode, using path where waypoint may sometime be a field
	void MovePathMode(){
		//check if current PathSection is assigned
		if(currentPS==null){
			//make sure we have a path
			if(path==null) return;
			
			List<PathSection> PSList=path.GetPath();
			if(wpCounter<PSList.Count){
				currentPS=PSList[wpCounter];
				
				GetSubPath();
			}
			else ReachDestination();
		}
		
		//execute as long as there are valid pathSection
		if(currentPS!=null){
			
				if(currentPathID!=currentPS.GetPathID()){
					SearchSubPath();
				}
				
					//move to the next waypoint, if return true, then update to the next waypoint
					if(MoveToPoint(subPath[subWPCounter])){
						
						if(_flying && subWPCounter==0) subWPCounter=subPath.Count+1;
						else subWPCounter+=1;
							
						//if the unit have reach the end of the subpath, update to next pathSection
						if(subWPCounter>=subPath.Count){
							wpCounter+=1;
							currentPS=null;
						}
					}
			
		}
		
	}
	
	//for using simple point to point path
	void MoveWPMode(){
		//execute as long as there are unreached waypoint in the path
		if(wpCounter<wp.Count){
			//move to the next waypoint, if return true, then update to the next waypoint
			if(MoveToPoint(wp[wpCounter])){
				wpCounter+=1;
			}
		}
		else ReachDestination();
	}
	
	//function call to rotate and move toward a pecific point, return true when the point is reached
	bool MoveToPoint(Vector3 point){
		//this is for dynamic waypoint, each unit creep have it's own offset pos
		if(subClass==_UnitSubClass.Creep && unitC!=null) point+=unitC.dynamicOffset;
		
		if(_flying) point+=new Vector3(0, _flightHeightOffset, 0);
		
		float dist=Vector3.Distance(point, thisT.position);
		
		if(dist<0.15f) {
			//if the unit have reached the point specified
			return true;
		}
		
		//rotate towards destination
		Quaternion wantedRot=Quaternion.LookRotation(point-thisT.position);
		thisT.rotation=Quaternion.Slerp(thisT.rotation, wantedRot, rotateSpd*Time.deltaTime);
		
		//move, with speed take distance into accrount so the unit wont over shoot
		Vector3 dir=(point-thisT.position).normalized;
		thisT.Translate(dir*Mathf.Min(dist, currentMoveSpd * Time.deltaTime * slowModifier), Space.World);
		
		return false;
	}
	
	
	void ReachDestination(){
		//if unit is a standard TD creep, score and killself
		if(subClass==_UnitSubClass.Creep && !scored){
			scored=true;
			ScoreE(unitC.waveID);
			if(subClass==_UnitSubClass.Creep) unitC.Score();
			//ObjectPoolManager.Unspawn(thisObj);
		}
	}

	
	void AdjustOverlayPosition(){
		if(HPAttribute.overlayHP || HPAttribute.overlayBase){
			Quaternion rot=cam.rotation*offset;
			
			if(HPAttribute.overlayBase) HPAttribute.overlayBase.rotation=rot;
			
			if(HPAttribute.overlayHP){
				HPAttribute.overlayHP.rotation=rot;
				if(HPAttribute.overlayBase){
					//Vector3 dirUp=HPAttribute.overlayHP.TransformDirection(Vector3.up);
					HPAttribute.overlayHP.position=HPAttribute.overlayBase.position;
					
					Vector3 dirRight=HPAttribute.overlayHP.TransformDirection(-Vector3.right);
					float dist=5*((HPAttribute.fullHP-HPAttribute.HP)/HPAttribute.fullHP*overlayScaleH.x)/2;
					
					HPAttribute.overlayHP.Translate(dirRight*dist, Space.World);
				}
			}
			
		}
	}
	
	public void UpdateOverlay(){
		if(HPAttribute.overlayHP!=null){
			Vector3 scale=new Vector3(HPAttribute.HP/HPAttribute.fullHP*overlayScaleH.x, 1, overlayScaleH.z);
			HPAttribute.overlayHP.localScale=scale;
		}
		
		if(HPAttribute.HP>=HPAttribute.fullHP && !HPAttribute.alwaysShowOverlay){
			overlayIsVisible=false;
			if(HPAttribute.overlayHP!=null) HPAttribute.overlayHP.renderer.enabled=false;
			if(HPAttribute.overlayBase!=null) HPAttribute.overlayBase.renderer.enabled=false;
		}
		else{
			overlayIsVisible=true;
			if(HPAttribute.overlayHP!=null && HPAttribute.HP>0) HPAttribute.overlayHP.renderer.enabled=true;
			else HPAttribute.overlayHP.renderer.enabled=false;
			if(HPAttribute.overlayBase!=null) HPAttribute.overlayBase.renderer.enabled=true;
		}
	}
	
	public void ApplyDamage(float dmg){
		HPAttribute.HP-=dmg;
		
		if(subClass==_UnitSubClass.Creep && !dead) unitC.PlayHit();
		
		if(HPAttribute.HP<=0 && !dead){
			HPAttribute.HP=0;
			dead=true;
			
			if(subClass==_UnitSubClass.Creep){
				unitC.Dead();
				DeadE(unitC.waveID);
			}
			else if(subClass==_UnitSubClass.Tower){
				unitT.Dead();
				DeadE(unitC.waveID);
			}
			else{
				ObjectPoolManager.Unspawn(thisObj);
			}
			
		}
		
		UpdateOverlay();
	}
	
	[HideInInspector] public bool stunned=false;
	private float stunnedDuration=0;
	public void ApplyStun(float stun){
		if(stun>stunnedDuration) stunnedDuration=stun;
		if(!stunned){
			stunned=true;
			
			if(subClass==_UnitSubClass.Creep) unitC.Stunned();
			//else if(subClass==_UnitSubClass.Tower) unitT.Stunned();
			
			StartCoroutine(StunRoutine());
		}
	}
	
	IEnumerator StunRoutine(){
		while(stunnedDuration>0){
			stunnedDuration-=Time.deltaTime;
			yield return null;
		}
		stunned=false;
		
		if(subClass==_UnitSubClass.Creep) unitC.Unstunned();
		//else if(subClass==_UnitSubClass.Tower) unitT.Unstunned();
		
	}
	
	private List<Slow> slowEffect=new List<Slow>();
	private float slowModifier=1.0f;
	private bool slowRoutine=false;
	
	//~ private float slowDuration=0;
	public void ApplySlow(Slow slow){
		bool immuned=false;
		
		if(subClass==_UnitSubClass.Creep){ immuned=unitC.immuneToSlow; }
		else if(subClass==_UnitSubClass.Tower){ immuned=unitT.immuneToSlow; }
		
		if(!immuned){
			slow.SetTimeEnd(Time.time+slow.duration);
			Debug.Log(Time.time+"   "+slow.GetTimeEnd());
			slowEffect.Add(slow);
			if(!slowRoutine) StartCoroutine(SlowRoutine());
		}
	}
	
	private IEnumerator SlowRoutine(){
		slowRoutine=true;
		while(slowEffect.Count>0){
			float targetVal=1.0f;
			for(int i=0; i<slowEffect.Count; i++){
				Slow slow=slowEffect[i];
				
				//check if the effect has expired
				if(Time.time>=slow.GetTimeEnd()){
					slowEffect.RemoveAt(i);
					i--;
				}
				
				//if the effect is not expire, check the slowFactor
				//record the val if the slowFactor is slower than the previous entry
				else if(1-slow.slowFactor<targetVal){
					targetVal=1-slow.slowFactor;
				}
			}
			
			slowModifier=Mathf.Lerp(slowModifier, targetVal, Time.deltaTime*10);
			yield return null;
		}
		slowRoutine=false;
		
		while(slowEffect.Count==0){
			slowModifier=Mathf.Lerp(slowModifier, 1, Time.deltaTime*10);
			yield return null;
		}
	}
	
	
	
	public void ApplyDot(Dot dot){
		StartCoroutine(DotRoutine(dot));
	}
	
	private IEnumerator DotRoutine(Dot dot){
		float timeStart=Time.time;
		while(Time.time-timeStart<dot.duration){
			ApplyDamage(dot.damage);
			yield return new WaitForSeconds(dot.interval);
		}
	}
	
	public void StopMoving(){
		currentMoveSpd=0;
		
	}
	public void ResumeMoving(){
		if(unitC!=null) currentMoveSpd=unitC.moveSpeed;
		else if(unitT!=null) currentMoveSpd=unitC.moveSpeed;
	}
	
	
	public int GetWPCounter(){
		return wpCounter;
	}
	
	public void SetWPCounter(int num){
		wpCounter=num;
	}
	
	public bool IsDead(){
		return dead;
	}
}
