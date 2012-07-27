using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UnitAttribute{
	public float fullHP=10;
	[HideInInspector] public float HP=10;
	
	public float fullShield=0;
	[HideInInspector] public float shield=10;
	
	public float shieldRechargeRate=0.5f;
	public float shieldStagger=3;
	
	[HideInInspector] public float lastHitTime=0;
	
	public Transform overlayHP;
	public Transform overlayShield;
	public Transform overlayBase;
	public bool alwaysShowOverlay=false;
	
	private Transform cam;
	private bool overlayIsVisible=false;
	
	private Vector3 overlayScaleH;
	private Vector3 overlayScaleS;
	
	private Vector3 overlayPosH;
	private Vector3 overlayPosS;
	
	private Renderer overlayRendererH;
	private Renderer overlayRendererS;
	
	private bool neverShowBase=false;
	
	private Transform rootTransform;
	
	public void Init(Transform transform){
		fullHP=Mathf.Max(0, fullHP);
		fullShield=Mathf.Max(0, fullShield);
		
		HP=fullHP;
		shield=fullShield;
		
		rootTransform=transform;
		cam=Camera.main.transform;
		
		if(overlayBase==null){
			overlayBase=GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
			
			if(overlayHP){
				overlayBase.position=overlayHP.position;
				overlayBase.rotation=overlayHP.rotation;
			}
			else if(overlayShield){
				overlayBase.position=overlayShield.position;
				overlayBase.rotation=overlayShield.rotation;
			}
			
			UnitUtility.DestroyColliderRecursively(overlayBase);
			overlayBase.renderer.enabled=false;
			overlayBase.parent=transform; 
			
			neverShowBase=true;
		}
		
		offsetB=overlayBase.localPosition;
		
		if(overlayHP!=null) {
			overlayHP.gameObject.layer=LayerManager.LayerOverlay();
			overlayRendererH=overlayHP.renderer;
			overlayHP.parent=overlayBase;
			overlayScaleH=overlayHP.localScale;
			offsetH=overlayHP.localPosition;
			if(alwaysShowOverlay) overlayRendererH.enabled=true;
		}
		if(overlayShield!=null) {
			overlayShield.gameObject.layer=LayerManager.LayerOverlay();
			overlayRendererS=overlayShield.renderer;
			overlayShield.parent=overlayBase;
			overlayScaleS=overlayShield.localScale;
			offsetS=overlayShield.localPosition;
			if(alwaysShowOverlay) overlayRendererS.enabled=true;
		}
		
		if(alwaysShowOverlay){
			overlayIsVisible=true;
		}
		
	}
	
	public void Reset(){
		HP=fullHP;
		shield=fullShield;
		
		UpdateOverlay();
	}
	
	public void GainHP(float val){
		HP=Mathf.Min(fullHP, HP+=val);
	}
	
	public void ApplyDamage(float dmg){
		lastHitTime=Time.time;
		
		if(shield>0){
			if(dmg>shield){
				dmg-=shield;
				shield=0;
			}
			else{
				shield-=dmg;
			}
		}
		
		if(shield==0) HP-=dmg;
		
		HP=Mathf.Clamp(HP, 0, fullHP);
		UpdateOverlay();
	}
	
	public IEnumerator ShieldRoutine(){
		if(fullShield<=0) yield break;
		
		while(true){
			
			//staggered, stop recharging
			if(Time.time-lastHitTime<shieldStagger){
				yield return null;
			}
			//recharging
			else{
				if(shield<fullShield){
					shield=Mathf.Min(fullShield, shield+Time.deltaTime*shieldRechargeRate);
					UpdateOverlay();
					yield return null;
				}
				else{
					yield return null;
				}
			}
		}
	}
	
	private Vector3 offsetS;
	private Vector3 offsetH;
	private Vector3 offsetB;
	
	public IEnumerator Update(){
				
		if(!overlayHP && !overlayShield) yield break;
		
		Quaternion offset=Quaternion.Euler(-90, 0, 0);
		
		float scaleModifierH=1;
		float scaleModifierS=1;
		
		if(overlayHP) scaleModifierH=UnitUtility.GetWorldScale(overlayHP).x*5;
		if(overlayShield) scaleModifierS=UnitUtility.GetWorldScale(overlayShield).x*5;
		
		while(true){
			if(overlayIsVisible){
				if(overlayHP || overlayBase){
					Quaternion rot=cam.rotation*offset;
					
					overlayBase.rotation=rot;
					Vector3 dirRight=overlayBase.TransformDirection(-Vector3.right);
					
					if(overlayHP){
						overlayHP.localPosition=offsetH;
						float dist=((fullHP-HP)/fullHP)*scaleModifierH;
						overlayHP.Translate(dirRight*dist, Space.World);
					}
					
					if(overlayShield){
						overlayShield.localPosition=offsetS;
						float dist=((fullShield-shield)/fullShield)*scaleModifierS;
						overlayShield.Translate(dirRight*dist, Space.World);
					}
				}
			}
			
			yield return null;
		}
	}
	
	
	public void UpdateOverlay(){
		
		if(!overlayHP && !overlayShield) return;
		
		if(!alwaysShowOverlay){
			if(fullShield>0 && overlayShield){
				if(shield>=fullShield){
					if(!overlayHP || HP>=fullHP)
						overlayShield.renderer.enabled=false;
				}
				else if(shield<=0) overlayShield.renderer.enabled=false;
				else overlayShield.renderer.enabled=true;
			}
			
			if(overlayHP){
				if(!overlayShield){
					if(HP>=fullHP) overlayRendererH.enabled=false;
					else if(HP<=0) overlayRendererH.enabled=false;
					else overlayRendererH.enabled=true;
				}
				else{
					if(fullShield>0 && shield<fullShield){
						overlayHP.renderer.enabled=true;
					}
					else{
						if(HP>=fullHP) overlayRendererH.enabled=false;
						else if(HP<=0) overlayRendererH.enabled=false;
						else overlayRendererH.enabled=true;
					}
				}
			}
			
			if(overlayBase && !neverShowBase){
				if((overlayHP && overlayRendererH.enabled) || (overlayShield && overlayShield.renderer.enabled)){
					overlayBase.renderer.enabled=true;
				}
				else{
					if(HP>=fullHP) overlayBase.renderer.enabled=false;
				}
			}
			
			if((overlayHP && overlayRendererH.enabled) || (overlayShield && overlayShield.renderer.enabled)){
				overlayIsVisible=true;
			}
			else{
				overlayIsVisible=false;
			}
		}
		
		if(overlayHP && overlayRendererH.enabled){
			Vector3 scale=new Vector3(HP/fullHP*overlayScaleH.x, overlayScaleH.y, overlayScaleH.z);
			overlayHP.localScale=scale;
		}
		if(overlayShield && overlayRendererS.enabled){
			Vector3 scale=new Vector3(shield/fullShield*overlayScaleS.x, overlayScaleS.y, overlayScaleS.z);
			overlayShield.localScale=scale;
		}
		
	}
	
	public void ClearParent(){
		overlayBase.parent=null;
	}
	
	public void RestoreParent(){
		overlayBase.parent=rootTransform;
		overlayBase.localPosition=offsetB;
	}
	
}

public class Unit : MonoBehaviour {

	public string unitName;
	public Texture icon;
	
	public delegate void DeadHandler(int waveID);
	public static event DeadHandler onDeadE;
	
	public UnitAttribute HPAttribute;
	
	protected bool dead=false;
	protected bool scored=false;
	
	[HideInInspector] public Transform thisT;
	[HideInInspector] public GameObject thisObj;
	
	//waypoint and movement related variable
	protected bool wpMode=false; //if set to true, use wp, else use path
	protected PathTD path;
	protected List<Vector3> wp=new List<Vector3>();
	protected float currentMoveSpd;
	protected float rotateSpd=10;
	protected int wpCounter=0;
	
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
	}
	//Call by inherited class UnitTower, caching inherited UnitTower instance to this instance
	public void SetSubClassInt(UnitTower unit){ 
		unitT=unit; 
		subClass=_UnitSubClass.Tower;
	}

	public virtual void Awake(){
		thisT=transform;
		thisObj=gameObject;
		
		HPAttribute.Init(thisT);
		
		UnitUtility.DestroyColliderRecursively(thisT);
	}


	// Use this for initialization
	public virtual void Start () {
		Init();
	}
	
	
	public virtual void Init(){
		HPAttribute.Reset();
		StartCoroutine(HPAttribute.Update());
		StartCoroutine(HPAttribute.ShieldRoutine());
		
		//HPAttribute.HP=HPAttribute.fullHP;
		//UpdateOverlay();
		
		dead=false;
		scored=false;
	}
	
	
	public void SetFullHP(float hp){
		HPAttribute.fullHP=hp;
		HPAttribute.HP=HPAttribute.fullHP;
	}
	
	public void GainHP(float val){
		HPAttribute.GainHP(val);
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
		
	}
	
	
	public virtual bool MoveToPoint(Vector3 point){
		return false;
		
		float dist=Vector3.Distance(point, thisT.position);
		
		//if the unit have reached the point specified
		if(dist<0.15f) return true;
		
		//rotate towards destination
		Quaternion wantedRot=Quaternion.LookRotation(point-thisT.position);
		thisT.rotation=Quaternion.Slerp(thisT.rotation, wantedRot, rotateSpd*Time.deltaTime);
		
		//move, with speed take distance into accrount so the unit wont over shoot
		Vector3 dir=(point-thisT.position).normalized;
		thisT.Translate(dir*Mathf.Min(dist, currentMoveSpd * Time.deltaTime * slowModifier), Space.World);
		
		return false;
	}

	
	public void ApplyDamage(float dmg){
		//HPAttribute.HP-=dmg;
		HPAttribute.ApplyDamage(dmg);
		
		if(subClass==_UnitSubClass.Creep && !dead) unitC.PlayHit();
		
		if(HPAttribute.HP<=0 && !dead){
			HPAttribute.HP=0;
			dead=true;
			
			if(subClass==_UnitSubClass.Creep){
				unitC.Dead();
				if(onDeadE!=null) onDeadE(unitC.waveID);
			}
			else if(subClass==_UnitSubClass.Tower){
				unitT.Dead();
			}
			else{
				ObjectPoolManager.Unspawn(thisObj);
			}
			
		}
		
		//UpdateOverlay();
		//HPAttribute.UpdateOverlayRenderer();
	}
	
	protected bool stunned=false;
	private float stunnedDuration=0;
	public void ApplyStun(float stun){
		if(stun>stunnedDuration) stunnedDuration=stun;
		if(!stunned){
			stunned=true;
			if(subClass==_UnitSubClass.Creep) unitC.Stunned();
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
	}
	
	private List<Slow> slowEffect=new List<Slow>();
	private bool slowRoutine=false;
	protected float slowModifier=1.0f;
	
	public void ApplySlow(Slow slow){
		bool immuned=false;
		
		if(subClass==_UnitSubClass.Creep){ immuned=unitC.immuneToSlow; }
		else if(subClass==_UnitSubClass.Tower){ immuned=unitT.immuneToSlow; }
		
		if(!immuned){
			slow.SetTimeEnd(Time.time+slow.duration);
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
	
	protected bool stopMoving=false;
	public void StopMoving(){
		currentMoveSpd=0;
		stopMoving=true;
	}
	public void ResumeMoving(){
		stopMoving=false;
		if(unitC!=null) currentMoveSpd=unitC.moveSpeed;
		else if(unitT!=null) currentMoveSpd=unitC.moveSpeed;
	}
	
	public float GetSlowModifier(){
		return slowModifier;
	}
	
	public float GetDefaultMoveSpeed(){
		if(unitC!=null) return unitC.moveSpeed;
		else if(unitT!=null) return unitT.moveSpeed;
		
		return currentMoveSpd;
	}
	
	public int GetWPCounter(){
		return wpCounter;
	}
	
	public void SetWPCounter(int num){
		wpCounter=num;
	}
	
	public bool IsStunned(){
		return stunned;
	}
	
	public bool IsDead(){
		return dead;
	}
}
