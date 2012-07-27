using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum _TowerType{TurretTower, AOETower, DirectionalAOETower, SupportTower, ResourceTower, Mine, Block};
public enum _TargetMode{Hybrid, Air, Ground};
public enum _TargetingArea{AllAround, DirectionalCone, StraightLine};
public enum _TargetPriority{Nearest, Weakest, Toughest, Random};
public enum _TurretAni{Full, YAxis, None}
public enum _RotationMode{FullTurret, SeparatedBarrel}
	

public class UnitTower : Unit {
	
	public Animation buildAnimationBody;
	public AnimationClip buildAnimation;
	public Animation fireAnimationBody;
	public AnimationClip fireAnimation;
	public Animation fireAnimationBaseBody;
	public AnimationClip fireAnimationBase;
	
	public delegate void BuildCompleteHandler(UnitTower tower);
	public static event BuildCompleteHandler onBuildCompleteE;
	
	public delegate void DestroyHandler(UnitTower tower);
	public static event DestroyHandler onDestroyE;
	
	public int specialID=-1;
	public _TowerType type=_TowerType.TurretTower;

	public _TargetMode targetMode=_TargetMode.Hybrid;
	public _TargetPriority targetPriority=_TargetPriority.Nearest;
	public _TargetingArea targetingArea=_TargetingArea.AllAround;
	private int level=0;
	public int levelCap=0;
	public string description="This blocks of text here should give a brief description of this tower.";
	public TowerStat baseStat;
	public TowerStat[] upgradeStat=new TowerStat[1];
	
	private int[] towerValue=new int[1];
	
	public float GetDamage(){ return damage; }
	public float GetRange(){ return range; }
	public float GetMinRange(){ return minRange; }
	public float GetCooldown(){ return cooldown; }
	public float GetClipSize(){ return clipSize; }
	public float GetReloadDuration(){ return reloadDuration; }
	public float GetCurrentClip(){ return currentClip; }
	public float GetLastReloadTime(){ return lastReloadTime; }
	public float GetAoeRadius(){ return aoeRadius; }
	public float GetStunDuration(){ return stunDuration; }
	public Dot GetDot(){ return dot; }
	public Slow GetSlow(){ return slow; }
	public BuffStat GetBuff(){ return buff; }
	public int[] GetIncomes(){ return incomes; }
	public bool GetMineOneOff() { return mineOneOff; }
	
	private GameObject shootObject;
	
	private float damage=2;
	private float range=8;
	private float minRange=0;
	private float cooldown=1;
	private int clipSize=5;
	private float reloadDuration=4;
	private int currentClip=1;
	private float lastReloadTime;
	private float aoeRadius=1;
	private float stunDuration=1;
	private Dot dot;
	private Slow slow;
	[HideInInspector] public bool mineOneOff=true;
	public float aoeConeAngle=10;
	
	private BuffStat buff;
	private int[] incomes=new int[1];
	
	private Transform turretObject;
	private Transform baseObject;
	private Transform barrelObject;
	
	[HideInInspector] public _TurretAni animateTurret;
	private float turretMaxAngle=0;
	private float turretMaxRange=0;
	private bool targetInLOS=true; 	//in flag indicating if turret is facing the target
													// used to determine if a shot can be fire
	
	[HideInInspector] public float moveSpeed=0;
	[HideInInspector] public bool immuneToSlow=false;

	//to identity each tower built in a game
	private int towerID=-1;
	public void SetTowerID(int ID){ towerID=ID; }
	public int GetTowerID(){ return towerID; }
	
	//for stat editor purpose;
	//[SerializeField] private int towerID=-1; 
	//public void SetTowerID(int id){ towerID=id; }
	//public int GetTowerID(){ return towerID; }

	private Unit target;
	private LayerMask maskTarget;
	private float currentTargetDist=0;
	public LayerMask GetTargetMask(){
		return maskTarget;
	}
	public void AssignTarget(Unit tgt){
		if(Vector3.Distance(tgt.thisT.position, thisT.position)<range){
			target=tgt;
		}
	}
	public void CheckForTarget(Vector3 pos){
		Ray ray = Camera.main.ScreenPointToRay(pos);
		RaycastHit hit;
		//LayerMask mask=currentSelectedTower.GetTargetMask();
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, maskTarget)){
			Unit unit=hit.collider.gameObject.GetComponent<Unit>();
			if(unit!=null){
				AssignTarget(unit);
			}
		}
	}
	
	private bool built=false;
	private float currentBuildDuration=0;
	private float remainingBuildDuration=0;

	private int experience=0;

	
	public AudioClip shootSound;
	//public AudioClip reloadSound;
	public AudioClip buildingSound;
	public AudioClip builtSound;
	public AudioClip soldSound;
	
	[HideInInspector] public Transform[] shootPoint;
	
	
	
	public override void Awake(){
		base.Awake();
		
		if(thisObj.collider==null){
			SphereCollider col=thisObj.AddComponent<SphereCollider>();
			col.center=new Vector3(0, 0.5f, 0);
			
			//scale the collider radius to match the gridsize
			float scale=thisT.localScale.x;
			if(scale>=1)	col.radius=0.5f*BuildManager.GetGridSize()/scale;
			if(scale<1)	col.radius=0.5f*BuildManager.GetGridSize();
		}
		
		//assign base stat
		InitStat();
	}
	
	// Use this for initialization
	public override void Start () {
		base.Start();
		
		thisObj.layer=LayerManager.LayerTower();
		
		if(Time.timeSinceLevelLoad<0.25f){
			baseStat.buildDuration=0;
			InitTower(BuildManager.PrePlaceTower());
		}
	}
	
	//called when this tower is confirmed built, this will make the tower operational
	//called immediately upon built if the buildmode is point and build
	public void InitTower(int ID){
		towerID=ID;
		//Debug.Log("towerID: "+ID);
		
		if(targetMode==_TargetMode.Hybrid){
			LayerMask mask1=1<<LayerManager.LayerCreep();
			LayerMask mask2=1<<LayerManager.LayerCreepF();
			
			//~ Debug.Log(LayerManager.LayerCreep());
			//~ Debug.Log(LayerManager.LayerCreepF());
			
			maskTarget=mask1 | mask2;
			//~ maskTarget=2<<LayerManager.LayerCreepF();
		}
		else if(targetMode==_TargetMode.Air) maskTarget=1<<LayerManager.LayerCreepF();
		else if(targetMode==_TargetMode.Ground) maskTarget=1<<LayerManager.LayerCreep();
		
		if(shootObject!=null){
			ObjectPoolManager.New(shootObject, 2);
		}
		
		foreach(TowerStat stat in upgradeStat){
			if(stat.shootObject!=null){
				ObjectPoolManager.New(stat.shootObject, 2);
			}
		}
		
		if(type==_TowerType.TurretTower){
			//calculate turret offset if this tower uses a projectile with elevated shoot angle
			ShootObject shootObj=shootObject.GetComponent<ShootObject>();
			if(shootObj.type==_ShootObjectType.Projectile){
				turretMaxAngle=shootObj.maxShootAngle;
				turretMaxRange=shootObj.maxShootRange;
			}
			
			if(targetingArea==_TargetingArea.AllAround) StartCoroutine(ScanForTargetAllAround());
			else if(targetingArea==_TargetingArea.DirectionalCone) StartCoroutine(ScanForTargetDirectional());
			else if(targetingArea==_TargetingArea.StraightLine) StartCoroutine(ScanForTargetStraightLine());
			StartCoroutine(TurretRoutine());
		}
		else if(type==_TowerType.DirectionalAOETower){
			if(targetingArea==_TargetingArea.AllAround) StartCoroutine(ScanForTargetAllAround());
			else if(targetingArea==_TargetingArea.DirectionalCone) StartCoroutine(ScanForTargetDirectional());
			else if(targetingArea==_TargetingArea.StraightLine) StartCoroutine(ScanForTargetStraightLine());
			StartCoroutine(DirectionalAOERoutine());
		}
		else if(type==_TowerType.AOETower){
			StartCoroutine(AOERoutine());
		}
		else if(type==_TowerType.SupportTower){
			StartCoroutine(SupportRoutine());
		}
		else if(type==_TowerType.ResourceTower){
			StartCoroutine(ResourceRoutine());
		}
		else if(type==_TowerType.Mine){
			StartCoroutine(MineRoutine());
		}
		
		level=1;
		StartCoroutine(Building(baseStat.buildDuration, false));
		
		//if turret is not animating, then enable turret shoot under all circumstance
		//else turret can only shoot when facing target
		if(animateTurret==_TurretAni.None) targetInLOS=true;
		
		if(buildAnimationBody!=null && buildAnimation!=null){
			buildAnimationBody.AddClip(buildAnimation, buildAnimation.name);
			buildAnimationBody.Play(buildAnimation.name);
		}
	}
	

	//called immediately upon creation if the buildmode is DragNDrop
	public IEnumerator DragNDropRoutine(){
		
		HPAttribute.ClearParent();
		
		//set to additive and red color by default
		UnitUtility.SetMat2AdditiveRecursively(thisT);
		UnitUtility.SetAdditiveMatColorRecursively(thisT, Color.red);
		
		//delay a frame, make sure awake is executed
		yield return null;
		
		//disable the collider so it wont get in the way
		thisObj.collider.enabled=false;
		
		//show range indicator
		GameControl.DragNDropIndicator(this);
		
		//sure to check if the current visual state, red/green
		bool buildEnable=false;
		//a reference position use to check if the mouse has move on to a new build position
		Vector3 lastPos=Vector3.zero;
		
		while(true){
			bool flag=BuildManager.CheckBuildPoint(Input.mousePosition, type, specialID);
			BuildableInfo currentBuildInfo=BuildManager.GetBuildInfo();
			
			
			//change the visual of the tower appropriately according to buildablity of the position
			
			if(currentBuildInfo!=null){
				//only execute if only the currentBuidPos has been updated
				//so the object remain "green" when the cursor move to somewhere without collider
				if(currentBuildInfo.position!=lastPos){
					lastPos=currentBuildInfo.position;
					
					//change color/state if need be
					if(flag && !buildEnable){
						//Debug.Log("update true");
						buildEnable=true;
						UnitUtility.SetAdditiveMatColorRecursively(thisT, Color.green);
					}
					else if(!flag && buildEnable){
						//Debug.Log("update false");
						buildEnable=false;
						UnitUtility.SetAdditiveMatColorRecursively(thisT, Color.red);
					}
				}
			}
			
			//position the tower
			//this is just in case when the tower first spawn and the mouse is not pointing at a platform
			if(currentBuildInfo==null){
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit, Mathf.Infinity)) thisT.position=hit.point;
				//this there is no collier, randomly place it 30unit from camera
				else thisT.position=ray.GetPoint(30);
			}
			else thisT.position=currentBuildInfo.position;
			
			
			//left-click, build
			if(Input.GetMouseButtonDown(0)){
				//if current mouse point position is valid, build the tower
				if(flag) DragNDropBuilt();
				else{
					//GameMessage.DisplayMessage("Invalid Build Position");
					DragNDropCancel();
				}
				break;
			}
			
			//right-click, cancel
			if(Input.GetMouseButtonDown(1)){
				DragNDropCancel();
			}
			
			yield return null;
		}
	}
	
	void DragNDropCancel(){
		BuildManager.ClearBuildPoint();
		HPAttribute.RestoreParent();
		Destroy(thisObj);
	}
	
	//called when DragNDrop build is commenced
	void DragNDropBuilt(){
		//first check for resources
		int[] cost=GetCost();
		if(GameControl.HaveSufficientResource(cost)){
			GameControl.SpendResource(cost);
			
			//revert material to default stat and re-enable the collider
			UnitUtility.SetMat2DiffuseRecursively(thisT);
			thisObj.collider.enabled=true;
			
			HPAttribute.RestoreParent();
			
			//inform BuildManager, this is to setup platform block, pathfinding and so on
			BuildManager.DragNDropBuilt(this);
			
			//put tower into operation
			InitTower(towerID);
		}
		else{
			GameMessage.DisplayMessage("Insufficient Resource");
			DragNDropCancel();
		}
	}
	
	
	public float GetCurrentBuildDuration(){
		//BuildManager.ClearBuildPoint();
		return currentBuildDuration;
	}
	
	public float GetRemainingBuildDuration(){
		return remainingBuildDuration;
	}
	
	
	public float targetingDirection;
	public float targetingFOV=30;
	public bool matchTowerDir2TargetDir=false;
	protected Vector3 scanDirection;
	protected Quaternion scanQuaternion;
	
	public bool SetTargetingArea(int tgtArea){
		if(type==_TowerType.TurretTower || type==_TowerType.DirectionalAOETower){
			if(tgtArea==0) return SetTargetingArea(_TargetingArea.AllAround);
			else if(tgtArea==1) return SetTargetingArea(_TargetingArea.DirectionalCone);
			else if(tgtArea==2) return SetTargetingArea(_TargetingArea.StraightLine);
			else return false;
		}
		else return false;
	}
	
	public bool SetTargetingArea(_TargetingArea tgtArea){
		if(type==_TowerType.TurretTower || type==_TowerType.DirectionalAOETower){
			if(targetingArea!=tgtArea){
				targetingArea=tgtArea;
				if(targetingArea==_TargetingArea.AllAround) StartCoroutine(ScanForTargetAllAround());
				else if(targetingArea==_TargetingArea.DirectionalCone) StartCoroutine(ScanForTargetDirectional());
				else if(targetingArea==_TargetingArea.StraightLine) StartCoroutine(ScanForTargetStraightLine());
				return true;
			}
			else return false;
		}
		else return false;
	}
	
	public void SetTargetingDirection(float angle){
		targetingDirection=angle;
		scanDirection.x=Mathf.Cos(targetingDirection*Mathf.Deg2Rad);
		scanDirection.z=Mathf.Sin(targetingDirection*Mathf.Deg2Rad);
		scanQuaternion=Quaternion.LookRotation((thisT.position+scanDirection)-thisT.position);
		
		if(matchTowerDir2TargetDir){
			thisT.rotation=Quaternion.Euler(0, angle-90, 0);
		}
	}
	
	public bool SetTargetPriority(int priority){
		if(priority==0) return SetTargetPriority(_TargetPriority.Nearest);
		else if(priority==1) return SetTargetPriority(_TargetPriority.Weakest);
		else if(priority==2) return SetTargetPriority(_TargetPriority.Toughest);
		else if(priority==3) return SetTargetPriority(_TargetPriority.Random);
		else return false;
	}
	
	public bool SetTargetPriority(_TargetPriority priority){
		if(targetingArea==_TargetingArea.StraightLine) return false;
		targetPriority=priority;
		return true;
	}
	
	IEnumerator ScanForTargetStraightLine(){
		
		//rotate tower to match scan direction
		if(matchTowerDir2TargetDir){
			thisT.rotation=Quaternion.Euler(0, targetingDirection-90, 0);
		}
		
		scanDirection=Vector3.zero;
		scanDirection.x=Mathf.Cos(targetingDirection*Mathf.Deg2Rad);
		scanDirection.z=Mathf.Sin(targetingDirection*Mathf.Deg2Rad);
		RaycastHit hit;		
		while(targetingArea==_TargetingArea.StraightLine){
			if(built && !stunned){
				//~ if(target==null){
					target=null;
					if(Physics.Raycast(thisT.position, scanDirection, out hit, range, maskTarget)){
						Unit currentTarget=hit.collider.gameObject.GetComponent<Unit>();
						if(currentTarget!=null && currentTarget.HPAttribute.HP>0) target=currentTarget;
					}
				//~ }
				//~ else{
					//if target is out of range or dead or inactive, clear target
					//~ currentTargetDist=Vector3.Distance(thisT.position, target.thisT.position);
					//~ if(currentTargetDist>range || target.HPAttribute.HP<=0 || !target.thisObj.active){
						//~ target=null;
					//~ }
				//~ }
			}
			yield return null;
		}
	}
	
	
	IEnumerator ScanForTargetDirectional(){
		
		//rotate tower to match scan direction
		if(matchTowerDir2TargetDir){
			thisT.rotation=Quaternion.Euler(0, targetingDirection-90, 0);
		}
		
		scanDirection.x=Mathf.Cos(targetingDirection*Mathf.Deg2Rad);
		scanDirection.z=Mathf.Sin(targetingDirection*Mathf.Deg2Rad);
		scanQuaternion=Quaternion.LookRotation((thisT.position+scanDirection)-thisT.position);
		
		while(targetingArea==_TargetingArea.DirectionalCone){
			if(built && !stunned){
				if(target==null){
					Collider[] cols=Physics.OverlapSphere(thisT.position, range, maskTarget);
					Transform[] targetList=FilterTarget(cols, scanQuaternion);
					if(targetList.Length>0){
						
						Unit currentTarget=null;
						Unit targetTemp=null;
						
						if(targetPriority==_TargetPriority.Nearest){
							float dist=Mathf.Infinity;
							Transform currentTargetT=null;
							foreach(Transform targetT in targetList){
								float currentDist=Vector3.Distance(thisT.position, targetT.position);
								if(currentDist<dist){
									currentTargetT=targetT;
									dist=currentDist;
								}
							}
							currentTarget=currentTargetT.gameObject.GetComponent<Unit>();
							if(currentTarget!=null && currentTarget.HPAttribute.HP>0) target=currentTarget;
						}
						else if(targetPriority==_TargetPriority.Weakest){
							float hp=Mathf.Infinity;
							foreach(Transform targetT in targetList){
								targetTemp=targetT.gameObject.GetComponent<Unit>();
								if(targetTemp.HPAttribute.HP<hp && targetTemp.HPAttribute.HP>0){
									hp=targetTemp.HPAttribute.HP;
									currentTarget=targetTemp;
								}
							}
							if(currentTarget!=null && currentTarget.HPAttribute.HP>0) target=currentTarget;
						}
						else if(targetPriority==_TargetPriority.Toughest){
							float hp=0;
							foreach(Transform targetT in targetList){
								targetTemp=targetT.gameObject.GetComponent<Unit>();
								if(targetTemp.HPAttribute.HP>hp){
									hp=targetTemp.HPAttribute.HP;
									currentTarget=targetTemp;
								}
							}
							if(currentTarget!=null && currentTarget.HPAttribute.HP>0) target=currentTarget;
						}
						else{
							Transform targetT=targetList[Random.Range(0, cols.Length)];
							currentTarget=targetT.gameObject.GetComponent<Unit>();
							if(currentTarget!=null && currentTarget.HPAttribute.HP>0) target=currentTarget;
						}
						
					}
				}
				else{
					//if target is out of range or dead or inactive, clear target
					currentTargetDist=Vector3.Distance(thisT.position, target.thisT.position);
					if(currentTargetDist>range || target.HPAttribute.HP<=0 || !target.thisObj.active){
						target=null;
					}
					else{
						//if target is out of fov, clear the target
						if(!CheckIfInFOV(target.thisT, scanQuaternion)) target=null;
					}
				}
			}
			yield return null;
		}
	}
	
	bool CheckIfInFOV(Transform t, Quaternion wantedRot){
		Quaternion currentRot=Quaternion.LookRotation(t.position-thisT.position);
		if(Quaternion.Angle(wantedRot, currentRot)<targetingFOV){
			return true;
		}
		return false;
	}
	
	Transform[] FilterTarget(Collider[] cols, Quaternion wantedRot){
		List<Transform> filtered=new List<Transform>();
		foreach(Collider col in cols){
			Transform unit=col.transform;
			Quaternion currentRot=Quaternion.LookRotation(unit.position-thisT.position);
			if(Quaternion.Angle(wantedRot, currentRot)<targetingFOV){
				filtered.Add(unit);
			}
		}
		return filtered.ToArray();
	}	
	
	IEnumerator ScanForTargetAllAround(){
		while(targetingArea==_TargetingArea.AllAround){
			if(built && !stunned){
				if(target==null){
					Collider[] cols=Physics.OverlapSphere(thisT.position, range, maskTarget);
					if(cols.Length>0){
						
						Unit currentTarget=null;
						Unit targetTemp=null;
						
						if(targetPriority==_TargetPriority.Nearest){
							float dist=Mathf.Infinity;
							Collider currentCollider=cols[0];
							foreach(Collider col in cols){
								float currentDist=Vector3.Distance(thisT.position, col.transform.position);
								if(currentDist<dist){
									currentCollider=col;
									dist=currentDist;
								}
							}
							currentTarget=currentCollider.gameObject.GetComponent<Unit>();
							if(currentTarget!=null && currentTarget.HPAttribute.HP>0) target=currentTarget;
						}
						else if(targetPriority==_TargetPriority.Weakest){
							float hp=Mathf.Infinity;
							foreach(Collider col in cols){
								targetTemp=col.gameObject.GetComponent<Unit>();
								if(targetTemp.HPAttribute.HP<hp && targetTemp.HPAttribute.HP>0){
									hp=targetTemp.HPAttribute.HP;
									currentTarget=targetTemp;
								}
							}
							if(currentTarget!=null && currentTarget.HPAttribute.HP>0) target=currentTarget;
						}
						else if(targetPriority==_TargetPriority.Toughest){
							float hp=0;
							foreach(Collider col in cols){
								targetTemp=col.gameObject.GetComponent<Unit>();
								if(targetTemp.HPAttribute.HP>hp){
									hp=targetTemp.HPAttribute.HP;
									currentTarget=targetTemp;
								}
							}
							if(currentTarget!=null && currentTarget.HPAttribute.HP>0) target=currentTarget;
						}
						else{
							Collider currentCollider=cols[Random.Range(0, cols.Length)];
							currentTarget=currentCollider.gameObject.GetComponent<Unit>();
							if(currentTarget!=null && currentTarget.HPAttribute.HP>0) target=currentTarget;
						}
						
					}
				}
				else{
					//if target is out of range or dead or inactive, clear target
					currentTargetDist=Vector3.Distance(thisT.position, target.thisT.position);
					if(currentTargetDist>range || target.HPAttribute.HP<=0 || !target.thisObj.active){
						target=null;
					}
				}
			}
			yield return null;
		}
	}
	
	//default scan for target, get nearest target from all-direction, Obsolete
	IEnumerator ScanForTarget(){
		while(true){
			if(built && !stunned){
				if(target==null){
					Collider[] cols=Physics.OverlapSphere(thisT.position, range, maskTarget);
					if(cols.Length>0){
						float dist=Mathf.Infinity;
						Collider currentCollider=cols[0];
						foreach(Collider col in cols){
							float currentDist=Vector3.Distance(thisT.position, col.transform.position);
							if(currentDist<dist){
								currentCollider=col;
								dist=currentDist;
							}
						}
						Unit targetTemp=currentCollider.gameObject.GetComponent<Unit>();
						if(targetTemp!=null && targetTemp.HPAttribute.HP>0) target=targetTemp;
						//target=cols[0].gameObject.GetComponent<Unit>();
					}
				}
				else{
					//if target is out of range or dead or inactive, clear target
					currentTargetDist=Vector3.Distance(thisT.position, target.thisT.position);
					if(currentTargetDist>range || target.HPAttribute.HP<=0 || !target.thisObj.active){
						target=null;
					}
				}
			}
			yield return null;
		}
	}
	
	
	IEnumerator Reload(){
		lastReloadTime=Time.time;
		yield return new WaitForSeconds(reloadDuration);
		currentClip=clipSize;
	}
	
	
	
	IEnumerator TurretRoutine(){
		
		if(fireAnimationBody!=null && fireAnimation!=null) fireAnimationBody.AddClip(fireAnimation, fireAnimation.name);
		if(fireAnimationBaseBody!=null && fireAnimationBase!=null) fireAnimationBody.AddClip(fireAnimationBase, fireAnimationBase.name);
		
		while(true){
			if(built && target!=null && Vector3.Distance(thisT.position, target.thisT.position)<range && targetInLOS && !stunned && currentClip!=0){
				
				if(fireAnimationBody!=null && fireAnimation!=null) fireAnimationBody.Play(fireAnimation.name);
				if(fireAnimationBaseBody!=null && fireAnimationBase!=null) fireAnimationBody.Play(fireAnimationBase.name);
				
				foreach(Transform sp in shootPoint){
					GameObject obj=ObjectPoolManager.Spawn(shootObject, sp.position, sp.rotation);
					ShootObject shootObj=obj.GetComponent<ShootObject>();
					
					if(shootSound!=null) AudioManager.PlaySound(shootSound, thisT.position);
					
					shootObj.Shoot(target, this, sp);
				}
				
				currentClip-=1;
				if(currentClip==0) StartCoroutine(Reload());
				
				yield return new WaitForSeconds(Mathf.Max(0.05f, cooldown));
			}
			else{
				yield return null;
			}
		}
	}	
	
	IEnumerator DirectionalAOERoutine(){
		
		if(fireAnimationBody!=null && fireAnimation!=null) fireAnimationBody.AddClip(fireAnimation, fireAnimation.name);
		if(fireAnimationBaseBody!=null && fireAnimationBase!=null) fireAnimationBody.AddClip(fireAnimationBase, fireAnimationBase.name);
		
		while(true){
			if(built && target!=null && Vector3.Distance(thisT.position, target.thisT.position)<range && targetInLOS && !stunned && currentClip!=0){
				
				Vector3 srcPos=thisT.position;
				if(turretObject!=null) srcPos=turretObject.position;
				Quaternion wantedRotation=Quaternion.LookRotation(target.thisT.position-srcPos);
				
				Collider[] cols=Physics.OverlapSphere(thisT.position, range, maskTarget);
				foreach(Collider col in cols){
					Quaternion tgtRotation=Quaternion.LookRotation(col.transform.position-srcPos);
					if(Quaternion.Angle(wantedRotation, tgtRotation)<aoeConeAngle/2){
						Unit unit=col.gameObject.GetComponent<Unit>();
						
						ApplyEffect(unit);
						
						Debug.DrawLine(thisT.position, unit.thisT.position, Color.red, 0.25f);
					}
				}
				
				if(fireAnimationBody!=null && fireAnimation!=null) fireAnimationBody.Play(fireAnimation.name);
				if(fireAnimationBaseBody!=null && fireAnimationBase!=null) fireAnimationBody.Play(fireAnimationBase.name);
				
				if(shootObject!=null){
					foreach(Transform sp in shootPoint){
						ObjectPoolManager.Spawn(shootObject, sp.position, sp.rotation);
						//ObjectPoolManager.Unspawn(obj);
					}
				}
				
				if(shootSound!=null) AudioManager.PlaySound(shootSound, thisT.position);
				
				currentClip-=1;
				if(currentClip==0) StartCoroutine(Reload());
				
				yield return new WaitForSeconds(Mathf.Max(0.05f, cooldown));
			}
			else{
				yield return null;
			}
		}
	}
	
	IEnumerator AOERoutine(){
		
		if(fireAnimationBody!=null && fireAnimation!=null) fireAnimationBody.AddClip(fireAnimation, fireAnimation.name);
		if(fireAnimationBaseBody!=null && fireAnimationBase!=null) fireAnimationBody.AddClip(fireAnimationBase, fireAnimationBase.name);
		
		while(true){
			if(built && !stunned){
				Collider[] cols=Physics.OverlapSphere(thisT.position, range, maskTarget);
				foreach(Collider col in cols){
					Unit unit=col.gameObject.GetComponent<Unit>();
					
					ApplyEffect(unit);
					
					Debug.DrawLine(thisT.position, unit.thisT.position, Color.red, 0.25f);
				}
				
				if(fireAnimationBody!=null && fireAnimation!=null) fireAnimationBody.Play(fireAnimation.name);
				if(fireAnimationBaseBody!=null && fireAnimationBase!=null) fireAnimationBody.Play(fireAnimationBase.name);
				
				if(shootObject!=null){
					foreach(Transform sp in shootPoint){
						ObjectPoolManager.Spawn(shootObject, sp.position, thisT.rotation);
						//ObjectPoolManager.Unspawn(obj);
					}
				}
				
				if(shootSound!=null) AudioManager.PlaySound(shootSound, thisT.position);
			
				yield return new WaitForSeconds(cooldown);
			}
			else yield return null;
		}
	}
	
	IEnumerator ResourceRoutine(){
		
		if(fireAnimationBody!=null && fireAnimation!=null) fireAnimationBody.AddClip(fireAnimation, fireAnimation.name);
		if(fireAnimationBaseBody!=null && fireAnimationBase!=null) fireAnimationBody.AddClip(fireAnimationBase, fireAnimationBase.name);
		
		while(true){
			if(built && !stunned){
				//gain resource
				//only valid when game is progressing, so resource tower cant be exploited
				if(GameControl.gameState==_GameState.Started) GameControl.GainResource(incomes);
				
				if(fireAnimationBody!=null && fireAnimation!=null) fireAnimationBody.Play(fireAnimation.name);
				if(fireAnimationBaseBody!=null && fireAnimationBase!=null) fireAnimationBody.Play(fireAnimationBase.name);
				
				if(shootObject!=null){
					foreach(Transform sp in shootPoint){
						ObjectPoolManager.Spawn(shootObject, sp.position, thisT.rotation);
					}
				}
				
				yield return new WaitForSeconds(cooldown);
			}
			else yield return null;
		}
	}
	
	
	private UnitTower[] buffList=new UnitTower[0];
	IEnumerator SupportRoutine(){
		buff.buffID=towerID;
		//when tower is upgraded, UpgradeStat() will take care of the buffID
		
		if(shootObject!=null){
			StartCoroutine(SupportTowerShootRoutine());
		}
		
		LayerMask maskTower=1<<LayerManager.LayerTower();
		while(true){
			if(built && !stunned){
				Collider[] cols=Physics.OverlapSphere(thisT.position, range, maskTower);

				if(buffList.Length>cols.Length){
					List<UnitTower> tempBuffList = new List<UnitTower>(buffList.Length);
					tempBuffList.AddRange(buffList);
					
					for(int i=0; i<tempBuffList.Count; i++){
						if(tempBuffList[i]==null){
							tempBuffList.RemoveAt(i);
							i--;
						}
					}
					
					buffList=tempBuffList.ToArray();
				}
				else if(buffList.Length<cols.Length){
					buffList=new UnitTower[cols.Length];
				
					for(int i=0; i<buffList.Length; i++){
						buffList[i]=cols[i].gameObject.GetComponent<UnitTower>();
						
						buffList[i].Buff(buff);
					}
				}
				
				yield return new WaitForSeconds(0.2f);
			}
			else if(!built){
				UnBuffAll();
				while(!built) yield return null;
				ReBuffAll();
			}
			else yield return null;
		}
	}
	
	IEnumerator SupportTowerShootRoutine(){
		
		if(fireAnimationBody!=null && fireAnimation!=null) fireAnimationBody.AddClip(fireAnimation, fireAnimation.name);
		if(fireAnimationBaseBody!=null && fireAnimationBase!=null) fireAnimationBody.AddClip(fireAnimationBase, fireAnimationBase.name);
		
		while(true){
			while(!built) yield return null;
			
			if(fireAnimationBody!=null && fireAnimation!=null) fireAnimationBody.Play(fireAnimation.name);
			if(fireAnimationBaseBody!=null && fireAnimationBase!=null) fireAnimationBody.Play(fireAnimationBase.name);
			
			if(shootObject!=null){
				foreach(Transform sp in shootPoint){
					ObjectPoolManager.Spawn(shootObject, sp.position, thisT.rotation);
				}
			}
			
			yield return new WaitForSeconds(cooldown);
		}
	}
	
	//thanks to GIOWorks
	IEnumerator MineRoutine(){
		
		float gridSize=BuildManager.GetGridSize();
		
		while(true){
			
			if(built && !stunned){
				Collider[] cols=Physics.OverlapSphere(thisT.position, gridSize/4, maskTarget);
				if(cols.Length>0){
					
					AudioManager.PlaySound(shootSound, thisT.position);
					
					Collider[] targets=Physics.OverlapSphere(thisT.position, range, maskTarget);
					
					foreach(Collider col in targets){
						Unit unit=col.gameObject.GetComponent<Unit>();
						
						ApplyEffect(unit);
					}
					
					if(mineOneOff){
						if(onDestroyE!=null) onDestroyE(this);
						if(thisT.childCount>0) thisObj.SetActiveRecursively(false);
						else thisObj.active=false;
					}
					else{
						yield return new WaitForSeconds(cooldown);
					}
					
				}
			}
			
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	//call by projectile when the target is hit
	public void HitTarget(Vector3 pos, Unit tgt){
		HitTarget(pos, tgt, true, 0);
	}
	
	//div is sample count for continous damage over the duration of the beam shootObj
	//normal projectile shootObject has a default div value of 0
	//effect flag indicate if side effect is to be applied, only false for continous damage before the final tick
	public void HitTarget(Vector3 pos, Unit tgt, bool effect, int div){
		if(aoeRadius<=0){
			if(tgt.gameObject!=null && tgt.gameObject.active){
				ApplyEffect(tgt, effect, div);
				GainExperience();
			}
		}
		else{
			Collider[] cols=Physics.OverlapSphere(pos, aoeRadius, maskTarget);
			foreach(Collider col in cols){
				Unit subTarget=col.gameObject.GetComponent<Unit>();
				if(subTarget!=null){
					ApplyEffect(subTarget, effect, div);
				}
			}
			
			if(cols.Length>0) GainExperience();
		}
	}
	
	//check stat and apply valid effect to the target
	void ApplyEffect(Unit unit, bool effect, int div){
		if(unit.thisObj.active){
			if(damage>0){
				if(div>0) unit.ApplyDamage(damage/div);
				else unit.ApplyDamage(damage);
				//if(!continousDamage) unit.ApplyDamage(damage);
				//else unit.ApplyDamage(damage*0.05f);
			}
			if(unit.HPAttribute.HP>0){
				if(effect){
					if(stunDuration>0) unit.ApplyStun(stunDuration);
					if(slow.duration*slow.slowFactor>0) unit.ApplySlow(slow);
					if(dot.damage*dot.duration*dot.interval>0) unit.ApplyDot(dot);
				}
			}
		}
	}
	
	void ApplyEffect(Unit unit){
		if(damage>0) unit.ApplyDamage(damage);
		if(unit.HPAttribute.HP>0){
			if(stunDuration>0) unit.ApplyStun(stunDuration);
			if(slow.duration*slow.slowFactor>0) unit.ApplySlow(slow);
			if(dot.damage*dot.duration*dot.interval>0) unit.ApplyDot(dot);
		}
	}
	
	private bool gainExp=false;
	
	void GainExperience(){
		//gain experience code goes here
		if(!gainExp) return;
		
		if(level<levelCap){
			experience+=1;
			
			int[] expCap=GetCost();
			if(experience>=expCap[0]){
				experience=experience-expCap[0];
				Upgrade();
			}
		}
	}
	
	public int GetExperience(){
		return experience;
	}

	
	//public void Select(){
	//	rangeIndicator.renderer.enabled=true;
	//}
	
	//public void Unselect(){
	//	rangeIndicator.renderer.enabled=false;
	//}
	
	//call for support tower to rebuff all the towers on bufflist, when the support tower is upgraded or etc
	private void ReBuffAll(){
		foreach(UnitTower unit in buffList){
			unit.Buff(buff);
		}
	}
	
	private void UnBuffAll(){
		//Debug.Log("unbuff all");
		foreach(UnitTower unit in buffList){
			unit.UnBuff(towerID);
		}
	}
	
	
	//called by a support tower to buff this tower
	private List<BuffStat> activeBuffList=new List<BuffStat>();
	public void Buff(BuffStat newBuff){
		if(activeBuffList.Contains(newBuff) || type==_TowerType.SupportTower) return;
		
		activeBuffList.Add(newBuff);
		
		damage=damage*(1+newBuff.damageBuff);
		range=range*(1+newBuff.rangeBuff);
		cooldown=cooldown*(1-newBuff.cooldownBuff);
		if(newBuff.regenHP>0) StartCoroutine(RegenHPRoutine(newBuff));
		
		//if(type==_TowerType.TurretTower) Debug.Log(unitName+"  "+newBuff.buffID+" damage: "+damage+"  range:"+range+"  cooldown:"+cooldown);
	}
	
	//remove buff effect, called when a support tower is destroy/sold
	public void UnBuff(int buffID){
		BuffStat tempBuff;
		for(int i=0; i<activeBuffList.Count; i++){
			tempBuff=activeBuffList[i];
			if(tempBuff.buffID==buffID){
				damage=damage/(1+tempBuff.damageBuff);
				range=range/(1+tempBuff.rangeBuff);
				cooldown=cooldown/(1-tempBuff.cooldownBuff);
				
				activeBuffList.RemoveAt(i);
				
				break;
			}
		}
		
		//if(type==_TowerType.TurretTower) Debug.Log(buffID+" unbuffing "+unitName+" damage: "+damage+"  range:"+range+"  cooldown:"+cooldown);
	}
	
	IEnumerator RegenHPRoutine(BuffStat buff){
		while(activeBuffList.Contains(buff)){
			//HPAttribute.HP=Mathf.Min(HPAttribute.fullHP, HP+);
			GainHP(Time.deltaTime*buff.regenHP);
			yield return null;
		}
	}
	
	
	
	public _RotationMode turretRotationModel=_RotationMode.FullTurret;
	//public Transform barrel;
	public override void Update(){
		base.Update();
		
		if(animateTurret!=_TurretAni.None && turretObject!=null && target!=null && !stunned){
			
				if(animateTurret==_TurretAni.YAxis){
					Vector3 targetPos=target.thisT.position;
					targetPos.y=turretObject.position.y;
					
					Quaternion wantedRot=Quaternion.LookRotation(targetPos-turretObject.position);
					turretObject.rotation=Quaternion.Slerp(turretObject.rotation, wantedRot, 15*Time.deltaTime);
					
					if(Quaternion.Angle(turretObject.rotation, wantedRot)<45) targetInLOS=true;
					else targetInLOS=false;
				}
				else if(animateTurret==_TurretAni.Full){
					if(turretRotationModel==_RotationMode.FullTurret){
						Vector3 targetPos=target.thisT.position;
						Quaternion wantedRot=Quaternion.LookRotation(targetPos-turretObject.position);
						
						//calculate elavation offset
						float distFactor=Mathf.Min(1, Vector3.Distance(turretObject.position, targetPos)/turretMaxRange);
						float offset=distFactor*turretMaxAngle;
						wantedRot*=Quaternion.Euler(-offset, 0, 0);
						
						turretObject.rotation=Quaternion.Slerp(turretObject.rotation, wantedRot, 15*Time.deltaTime);
						
						if(Quaternion.Angle(turretObject.rotation, wantedRot)<45) targetInLOS=true;
						else targetInLOS=false;
					}
					else if(turretRotationModel==_RotationMode.SeparatedBarrel){
						Vector3 targetPos=target.thisT.position;
						targetPos.y=turretObject.position.y;
						
						Quaternion wantedRot=Quaternion.LookRotation(targetPos-turretObject.position);
						turretObject.rotation=Quaternion.Slerp(turretObject.rotation, wantedRot, 15*Time.deltaTime);
						
						//calculate elavation offset
						if(barrelObject!=null){
							float distFactor=Mathf.Min(1, Vector3.Distance(turretObject.position, targetPos)/turretMaxRange);
							float offset=distFactor*turretMaxAngle;
							wantedRot=Quaternion.Euler(0, barrelObject.rotation.eulerAngles.y, 0);
							wantedRot*=Quaternion.Euler(-offset, 0, 0);
							
							barrelObject.rotation=Quaternion.Slerp(barrelObject.rotation, wantedRot, 15*Time.deltaTime);
							
							if(Quaternion.Angle(barrelObject.rotation, wantedRot)<45) targetInLOS=true;
							else targetInLOS=false;
						}
						else{
							if(Quaternion.Angle(turretObject.rotation, wantedRot)<45) targetInLOS=true;
							else targetInLOS=false;
						}
					}
				}

		}
		
	}
	
	
	
	//initialise stat, call when tower is first built
	private void InitStat(){
		//level=1;
		cooldown=baseStat.cooldown;
		
		range=baseStat.range;
		
		if(type==_TowerType.TurretTower || type==_TowerType.DirectionalAOETower || type==_TowerType.AOETower || type==_TowerType.Mine){
			damage=baseStat.damage;
			
			clipSize=baseStat.clipSize;
			currentClip=clipSize;
			if(currentClip<=0) currentClip=-1;
			reloadDuration=baseStat.reloadDuration;
			aoeRadius=baseStat.aoeRadius;
			stunDuration=baseStat.stunDuration;
			slow=baseStat.slow;
			dot=baseStat.dot;
		}
		else if(type==_TowerType.SupportTower){
			buff=baseStat.buff;
		}
		else if(type==_TowerType.ResourceTower){
			incomes=baseStat.incomes;
		}
		
		
		if(baseStat.shootObject!=null){
			shootObject=baseStat.shootObject.gameObject;
		}
		else{
			if(type==_TowerType.TurretTower){
				GameObject tempObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				tempObj.AddComponent<ShootObject>();
				tempObj.active=false;
				
				shootObject=tempObj;
			}
		}
		
		if(baseStat.turretObject!=null){
			turretObject=baseStat.turretObject;
		}
		if(baseStat.barrelObject!=null){
			barrelObject=baseStat.barrelObject;
		}
		
		//since this is initialization, update shootpoint regardless of if there's a turretObject
		UpdateShootPoint();
		
		if(baseStat.baseObject!=null){
			baseObject=baseStat.baseObject;
		}
		
		UpdateTowerValue();
	}
	
	public GameObject nextLevelTower;
	public bool UpgradeBuildNew(){
		if(nextLevelTower!=null){
			int[] cost=GetCost();
			if(GameControl.HaveSufficientResource(cost)){
				GameControl.SpendResource(cost);
			}
			else{
				GameMessage.DisplayMessage("Insufficient Resource");
				return false;
			}
			
			GameObject towerObj=(GameObject)Instantiate(nextLevelTower, thisT.position, Quaternion.identity);
			UnitTower towerCom=towerObj.GetComponent<UnitTower>();
			towerCom.InitTower(towerID);
			
			Destroy();
			
			return true;
		}
		else GameMessage.DisplayMessage("Tower is fully upgraded");
		
		return false;
	}
	
	//public function call to level up tower, 
	public bool Upgrade(){
		if(level<levelCap){
			int levelM=level-1;
			Debug.Log(levelM);
			
			//check if there are sufficient resource
			int[] cost=GetCost();
			if(GameControl.HaveSufficientResource(cost)){
				GameControl.SpendResource(cost);
			}
			else{
				GameMessage.DisplayMessage("Insufficient Resource");
				return false;
			}
			
			//start building process, stat will be update by the end of this coroutine
			StartCoroutine(Building(upgradeStat[levelM].buildDuration, true));
			return true;
		}
		
		GameMessage.DisplayMessage("Tower is fully upgraded");
		return false;
	}
	
	//called when the tower is being build or upgrade
	private IEnumerator Building(float dur, bool isUpgrade){
		built=false;
		//level=1;
		
		if(buildingSound!=null) AudioManager.PlaySound(buildingSound, thisT.position);
		else AudioManager.PlayTowerBuilding();
		
		currentBuildDuration=dur;
		remainingBuildDuration=dur;
		
		OverlayManager.Building(this);
		
		while(remainingBuildDuration>0){
			remainingBuildDuration-=Time.deltaTime;
			
			yield return null;
		}
		
		if(builtSound!=null) AudioManager.PlaySound(builtSound, thisT.position);
		else AudioManager.PlayTowerBuilt();
		
		built=true;
		
		//when the tower is first built, there's no need to update stat since the stat has been updated
		if(isUpgrade) UpgradeStat();
		
		if(onBuildCompleteE!=null) onBuildCompleteE(this);
	}
	
	
	
	private void UpgradeStat(){
		
		int levelM=level-1;
			
		if(type==_TowerType.TurretTower || type==_TowerType.DirectionalAOETower || type==_TowerType.AOETower){
			damage=upgradeStat[levelM].damage;
			cooldown=upgradeStat[levelM].cooldown;
			clipSize=upgradeStat[levelM].clipSize;
			currentClip=clipSize;
			if(currentClip<=0) currentClip=-1;
			reloadDuration=upgradeStat[levelM].reloadDuration;
			range=upgradeStat[levelM].range;
			minRange=upgradeStat[levelM].minRange;
			aoeRadius=upgradeStat[levelM].aoeRadius;
			stunDuration=upgradeStat[levelM].stunDuration;
			slow=upgradeStat[levelM].slow;
			dot=upgradeStat[levelM].dot;
		}
		else if(type==_TowerType.SupportTower){
			buff=upgradeStat[levelM].buff;
			buff.buffID=towerID;
		}
		else if(type==_TowerType.ResourceTower){
			incomes=upgradeStat[levelM].incomes;
			cooldown=upgradeStat[levelM].cooldown;
		}
		
		if(upgradeStat[levelM].shootObject!=null) 
			shootObject=upgradeStat[levelM].shootObject.gameObject;
		
		if(upgradeStat[levelM].turretObject!=null){
			
			if(turretObject.childCount>0) turretObject.gameObject.SetActiveRecursively(false);
			else turretObject.gameObject.active=false;
			
			Transform turretTemp=(Transform)Instantiate(upgradeStat[levelM].turretObject);
			turretTemp.position=turretObject.position;
			turretTemp.rotation=turretObject.rotation;
			turretTemp.parent=thisT;
			turretObject=turretTemp;
			//turretObject=upgradeStat[levelM].turretObject;
			
			UpdateShootPoint();
		}
		if(upgradeStat[levelM].barrelObject!=null){
			barrelObject=upgradeStat[levelM].barrelObject;
		}
		
		if(upgradeStat[levelM].baseObject!=null){
			
			if(baseObject.childCount>0) baseObject.gameObject.SetActiveRecursively(false);
			else baseObject.gameObject.active=false;
			
			Transform baseTemp=(Transform)Instantiate(upgradeStat[levelM].baseObject);
			baseTemp.position=baseObject.position;
			baseTemp.rotation=baseObject.rotation;
			baseTemp.parent=thisT;
			baseObject=baseTemp;
			
			//baseObject=upgradeStat[levelM].baseObject;
		}
			
		level+=1;
			
		UpdateTowerValue();
		GameControl.TowerUpgradeComplete(this);
	}
	
	private void UpdateShootPoint(){
		//get shootpoint, assigned to TurretObject component on turretObject
		if(turretObject!=null){
			TurretObject turretObj=turretObject.gameObject.GetComponent<TurretObject>();
			if(turretObj!=null){
				//make sure the shootpoint is not null
				if(turretObj.shootPoint!=null && turretObj.shootPoint.Length>0){
					shootPoint=turretObj.shootPoint;
					return;
				}
			}
			else{
				//no specify shootpoint, use turretObject itself
				shootPoint=new Transform[1];
				shootPoint[0]=turretObject;
				return;
			}
		}
		
		//this tower have no turretObject, use thisT as shootPoint
		shootPoint=new Transform[1];
		shootPoint[0]=thisT;
		return;
	}

	
	public bool IsLevelCapped(){
		if(level<levelCap) return false;
		else return true;
	}
	
	public int GetLevel(){
		return level;
	}
	
	public int[] GetCost(){
		if(level<=0) return baseStat.costs;
		else{
			if(level-1<upgradeStat.Length){
				return upgradeStat[level-1].costs;
			}
			else
				return upgradeStat[Mathf.Max(0, level-2)].costs;
		}
	}
	
	//~ public int GetBuildCost(){
		//~ return baseStat.cost;
	//~ }
	
	//~ public int GetUpgradeCost(){
		//~ if(level<upgradeStat.Length)
			//~ return upgradeStat[level-1].cost;
		//~ else
			//~ return upgradeStat[Mathf.Max(0, level-2)].cost;
	//~ }
	
	public TowerStat GetCurretStat(){
		if(level==1) return baseStat;
		else return upgradeStat[level-2];
	}
	
	public TowerStat GetBaseStat(){
		return baseStat;
	}
	
	public string GetDescription(){
		return description;
	}
	
	private OccupiedPlatform occupiedPlatform;
	
	//set a parent platform and node to this tower when it's build on a walkable platform
	public void SetPlatform(Platform platform, Node node){
		occupiedPlatform=new OccupiedPlatform(platform, node);
	}
	
	public bool IsBuilt(){
		return built;
	}
	
	public void Sell(){
		StartCoroutine(Unbuilding());
	}
	
	private IEnumerator Unbuilding(){
		built=false;
		
		//stunned=true;
		//if(type==_TowerType.SupportTower) UnBuffAll();
		
		//currentBuildDuration=dur;
		//remainingBuildDuration=0;
		
		//Debug.Log(currentBuildDuration);
		
		OverlayManager.Unbuilding(this);
		
		while(remainingBuildDuration<currentBuildDuration){
			remainingBuildDuration+=Time.deltaTime;
			yield return null;
		}
		
		if(soldSound!=null) AudioManager.PlaySound(soldSound, thisT.position);
		else AudioManager.PlayTowerSold();
		
		int[] sellValue=GetTowerSellValue();
		//~ GameControl.GetSellTowerRefundRatio();
		//~ for(int i=0; i<towerValue.Length; i++){
			//~ sellValue[i]=(int)Mathf.Floor(sellValue[i]*GameControl.GetSellTowerRefundRatio());
		//~ }
		GameControl.GainResource(sellValue);
		
		//GameControl.ClearSelection();
		
		Destroy();
	}
	
	//call when the tower is sold or destroy
	public void Destroy(){
		if(onDestroyE!=null) onDestroyE(this);
		
		//tells platform to reactivate node if platform is walkable
		if(occupiedPlatform!=null){
			//Debug.Log("clear attached platform");
			occupiedPlatform.platform.UnBuild(occupiedPlatform.node);
			occupiedPlatform=null;
		}
		//else Debug.Log("no platform attached");
		
		//tells gamecontrol to refund the tower
		
		
		
		//thisObj.SetActiveRecursively(false);
		Destroy(thisObj);
	}
	
	public GameObject destroyEffect;
	
	public void Dead(){
		if(destroyEffect!=null) ObjectPoolManager.Spawn(destroyEffect);
		Destroy();
	}
	
	
	private void UpdateTowerValue(){
		//Debug.Log("update tower value, level "+level);
		towerValue=new int[baseStat.costs.Length];
		
		towerValue=baseStat.costs;
		for(int i=0; i<level-1; i++){
			for(int j=0; j<towerValue.Length; j++){
				towerValue[j]+=upgradeStat[i].costs[j];
			}
		}
		
		//for(int j=0; j<towerValue.Length; j++){
		//	Debug.Log("tower value "+j+": "+towerValue[j]);
		//}
		
	}
	
	
	public int[] GetTowerValue(){
		return towerValue;
	}
	
	public int[] GetTowerSellValue(){
		int[] sellValue=new int[towerValue.Length];
		
		for(int j=0; j<sellValue.Length; j++){
			sellValue[j]=towerValue[j];
		}
		
		GameControl.GetSellTowerRefundRatio();
		for(int i=0; i<towerValue.Length; i++){
			sellValue[i]=(int)Mathf.Floor(sellValue[i]*GameControl.GetSellTowerRefundRatio());
		}
		
		return sellValue;
	}
	
	//for editor, when updating upgrade stat
	public void UpdateTowerUpgradeStat(int size){
		TowerStat[] temp=upgradeStat;
		
		if(temp.Length==0){
			temp=new TowerStat[1];
			temp[0]=new TowerStat();
			temp[0].slow=new Slow();
			temp[0].dot=new Dot();
			temp[0].buff=new BuffStat();
		}
		
		upgradeStat=new TowerStat[size];
		
		for(int i=0; i<upgradeStat.Length; i++){
			if(i>=temp.Length) upgradeStat[i]=temp[temp.Length-1].Clone();
			else upgradeStat[i]=temp[i];
		}
	}
	
	void OnDrawGizmos(){
		if(target!=null){
			Gizmos.DrawLine(thisT.position, target.thisT.position);
		}
	}
	
}




[System.Serializable]
public class TowerStat{
	public int cost=10;
	public int[] costs=new int[1];
	
	public float damage=5;
	public float cooldown=1;
	public int clipSize=5;
	public float reloadDuration=1;
	public float range=10;
	public float minRange=0;
	public float aoeRadius=0;
	public float stunDuration=0;
	public Slow slow;
	public Dot dot;
	//public bool mineOneOff;
	
	public BuffStat buff;
	public int[] incomes=new int[1];
	
	public float buildDuration=1;
	
	public Transform shootObject;
	
	public Transform barrelObject;
	public Transform turretObject;
	public Transform baseObject;
	
	
	public TowerStat Clone(){
		TowerStat clone=new TowerStat();
		clone.cost=cost;
		//clone.costs=costs;
		
		clone.costs=new int[costs.Length];
		for(int i=0; i<costs.Length; i++){
			clone.costs[i]=costs[i];
		}
		
		clone.damage=damage;
		clone.cooldown=cooldown;
		clone.clipSize=clipSize;
		clone.reloadDuration=reloadDuration;
		clone.range=range;
		clone.minRange=minRange;
		clone.aoeRadius=aoeRadius;
		clone.stunDuration=stunDuration;
		clone.slow=slow.Clone();
		clone.dot=dot.Clone();
		clone.buff=buff.Clone();
		//clone.incomes=incomes;
		
		clone.incomes=new int[incomes.Length];
		for(int i=0; i<incomes.Length; i++){
			clone.incomes[i]=incomes[i];
		}
		
		clone.buildDuration=buildDuration;
		
		return clone;
	}
	
}

[System.Serializable]
public class Dot{
	public float damage=1f;
	public float duration=3;
	public float interval=0.5f;
	
	public Dot Clone(){
		Dot clone=new Dot();
		clone.damage=damage;
		clone.duration=duration;
		clone.interval=interval;
		
		return clone;
	}
}

[System.Serializable]
public class Slow{
	public float duration=3;
	public float slowFactor=0.5f;
	private float timeEnd;
	
	public float GetTimeEnd(){
		return timeEnd;
	}
	public void SetTimeEnd(float val){
		timeEnd=val;
	}
	
	public Slow Clone(){
		Slow clone=new Slow();
		clone.slowFactor=slowFactor;
		clone.duration=duration;
		clone.timeEnd=timeEnd;
		
		return clone;
	}
}

[System.Serializable]
public class BuffStat{
	//buff doesnt stack, higher level override lowerlevel buff
	[HideInInspector] public int buffID=0;
	public float damageBuff=0.1f;
	public float cooldownBuff=0.1f;
	public float rangeBuff=0.1f;
	public float regenHP=1.0f;
	
	public BuffStat Clone(){
		BuffStat clone=new BuffStat();
		clone.buffID=buffID;
		clone.damageBuff=damageBuff;
		clone.cooldownBuff=cooldownBuff;
		clone.rangeBuff=rangeBuff;
		clone.regenHP=regenHP;
		
		return clone;
	}
}

public class Buffed{
//	private List<BuffStat> buffList=new List<BuffList>();
//	
//	
//	public void AddBuff(int ID, BuffStat){
//		
//	}
}

public class OccupiedPlatform{
	public Platform platform;
	public Node node;
	
	public OccupiedPlatform(Platform p, Node n){
		platform=p;
		node=n;
	}
}


