
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum _AttackCreepType{Attack, Support}
public enum _TargetingA{AllAround, FrontalCone, Obstacle};
public enum _AttackMode{RunNGun, StopNAttack};
public enum _AttackMethod{Range, Melee};
public enum _CDTracking{Easy, Precise};

[RequireComponent (typeof (UnitCreep))]

public class UnitCreepAttack : MonoBehaviour {

	public delegate void BuffHandler(UnitCreepAttack unitCreepAttack);
	public static event BuffHandler buffE;
	
	void Enable(){
		UnitCreepAttack.buffE += Buff;
	}
	
	void Disable(){
		UnitCreepAttack.buffE -= Buff;
	}
	
	void Buff(UnitCreepAttack src){
		if(Vector3.Distance(src.unit.thisT.position, unit.thisT.position)<src.range){
			
		}
	}
	
	
	
	[HideInInspector] public UnitCreep unit;
	
	public _AttackCreepType type=_AttackCreepType.Attack;
	
	public float meleeAttackRange=2f;
	enum _MeleeState{OutOfRange, Attacking}
	private _MeleeState meleeState=_MeleeState.OutOfRange;
	
	
	public _TargetingA targetArea;
	//public _TargetMode targetMode;
	public float frontalConeAngle=35f;
	public Transform turretObject;
	private bool targetInLos=false;
	
	
	public _AttackMode attackMode;
	
	private Unit target;
	private float currentTargetDist;
	
	
	public _AttackMethod attackMethod;
	
	
	public _CDTracking cdTracking;
	
	public GameObject shootObject;
	public Transform sp;
	public float range=5;
	public float cooldown;
	public float damage;
	public float stun;
	public BuffStat buff;
	
	private int clipSize=-1;
	private float reloadDuration=4;
	private int currentClip=1;
	
	public AudioClip attackSound;
	public AnimationClip animationIdle;
	public AnimationClip animationAttack;
	public float aniAttackTimeOffset=0.1f;
	
	protected bool dash=false;
	protected float dashFactor=1.5f;
	
	
	void Awake(){
		unit=gameObject.GetComponent<UnitCreep>();
		if(unit==null) return;
		
		if(animationAttack!=null) unit.SetAttackAnimation(animationAttack);
		if(animationIdle!=null) unit.SetIdleAnimation(animationIdle);
		
		if(shootObject!=null) ObjectPoolManager.New(shootObject, 2);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(type==_AttackCreepType.Attack){
			if(target!=null){
				if(attackMethod==_AttackMethod.Melee){
					if(attackMode==_AttackMode.StopNAttack){
						LayerMask mask=1<<LayerManager.LayerTower();
						if(!Physics.Raycast(unit.thisT.position+new Vector3(0, 1, 0), unit.thisT.forward, meleeAttackRange, mask)){
							MoveToPoint(target.thisT.position);
						}
						else{
							if(meleeState!=_MeleeState.Attacking){
								unit.StopAnimation();
								meleeState=_MeleeState.Attacking;
								targetInLos=true;
							}
						}
					}
					else{
						LayerMask mask=1<<LayerManager.LayerTower();
						
						if(Physics.Raycast(unit.thisT.position+new Vector3(0, 1, 0), unit.thisT.forward, meleeAttackRange, mask)){
							if(meleeState!=_MeleeState.Attacking){
								//unit.StopAnimation();
								meleeState=_MeleeState.Attacking;
								targetInLos=true;
							}
						}
						else{
							if(meleeState==_MeleeState.Attacking){
								meleeState=_MeleeState.OutOfRange;
								targetInLos=false;
							}
						}
					}
				}
				else if(attackMethod==_AttackMethod.Range){
					if(turretObject!=null){
						Quaternion wantedRot=Quaternion.LookRotation(target.thisT.position-unit.thisT.position);
						
						turretObject.rotation=Quaternion.Slerp(turretObject.rotation, wantedRot, Time.deltaTime*10);
						
						if(Quaternion.Angle(wantedRot, turretObject.rotation)<10){
							targetInLos=true;
						}
						else targetInLos=false;
						
						if(attackMode==_AttackMode.StopNAttack){
							Vector3 point=target.thisT.position;
							point.y=unit.thisT.position.y;
							wantedRot=Quaternion.LookRotation(point-unit.thisT.position);
							unit.thisT.rotation=Quaternion.Slerp(unit.thisT.rotation, wantedRot, 10*Time.deltaTime);
						}
					}
					else{
						if(attackMode==_AttackMode.StopNAttack){
							Vector3 point=target.thisT.position;
							point.y=unit.thisT.position.y;
							Quaternion wantedRot=Quaternion.LookRotation(point-unit.thisT.position);
							unit.thisT.rotation=Quaternion.Slerp(unit.thisT.rotation, wantedRot, 10*Time.deltaTime);
							
							if(Quaternion.Angle(unit.thisT.rotation, wantedRot)<7 && !targetInLos) targetInLos=true;
							else if(targetInLos) targetInLos=false;
						}
						else{
							if(!targetInLos) targetInLos=true;
						}
					}
				}
				
			}
			else{
				if(targetInLos) targetInLos=true;
				
				if(attackMethod==_AttackMethod.Melee){
					
				}
				else if(attackMethod==_AttackMethod.Range){
					if(turretObject!=null){
						Quaternion wantedRot=Quaternion.Euler(0, 0, 0);
						
						turretObject.localRotation=Quaternion.Slerp(turretObject.localRotation, wantedRot, Time.deltaTime*10);
					}
				}
			}
		}
		else if(type==_AttackCreepType.Support){
			if(attackMode==_AttackMode.StopNAttack){
				
			}
			else if(attackMode==_AttackMode.RunNGun){
				
			}
		}
	}
	

	
	
	
	public bool MoveToPoint(Vector3 point){
		point.y=unit.thisT.position.y;
		
		float dist=Vector3.Distance(point, unit.thisT.position);
		
		if(dist<0.15f) {
			//if the unit have reached the point specified
			return true;
		}
		
		Quaternion wantedRot=Quaternion.LookRotation(point-unit.thisT.position);
		unit.thisT.rotation=Quaternion.Slerp(unit.thisT.rotation, wantedRot, 10*Time.deltaTime);
		
		Vector3 dir=(point-unit.thisT.position).normalized;
		unit.thisT.Translate(dir*Mathf.Min(dist, dashFactor*unit.GetDefaultMoveSpeed() * Time.deltaTime *unit.GetSlowModifier()), Space.World);
		
		return false;
	}
	
	void OnEnable(){
		if(unit.thisT!=null){
			StartCoroutine(ScanForTarget());
			if(type==_AttackCreepType.Attack){
				StartCoroutine(AttackRoutine());
			}
			else{
				StartCoroutine(SupportRoutine());
			}
			
			if(!dash) dashFactor=1;
		}
	}
	
	
	
	IEnumerator Reload(){
		yield return new WaitForSeconds(reloadDuration);
		currentClip=clipSize;
	}
	
	
	
	IEnumerator ScanForTarget(){
		LayerMask maskTarget=1<<LayerManager.LayerTower();
		while(true){
			if(target==null){
				if(targetArea==_TargetingA.AllAround){
					
						Collider[] cols=Physics.OverlapSphere(unit.thisT.position, range, maskTarget);
					
						if(cols.Length>0){
							Collider currentCollider=cols[Random.Range(0, cols.Length)];
							Unit targetTemp=currentCollider.gameObject.GetComponent<Unit>();
							if(targetTemp!=null && targetTemp.HPAttribute.HP>0){
								target=targetTemp;
								
								if(attackMode==_AttackMode.StopNAttack){
									if(attackMethod!=_AttackMethod.Melee) unit.StopAnimation();
									unit.StopMoving();
									//if(dash){
									//	if(Vector3.Distance(unit.thisT.position, target.thisT.position)>8){
									//		//unit.PlayDash();
									//	}
									//}
								}
							}
						}
					
				}
				else if(targetArea==_TargetingA.FrontalCone){
					
						Collider[] cols=Physics.OverlapSphere(unit.thisT.position, range, maskTarget);
						if(cols.Length>0){
							Collider currentCollider=cols[0];
							foreach(Collider col in cols){
								Quaternion targetRot=Quaternion.LookRotation(col.transform.position-unit.thisT.position);
								if(Quaternion.Angle(targetRot, unit.thisT.rotation)<frontalConeAngle){
									Unit targetTemp=currentCollider.gameObject.GetComponent<Unit>();
									if(targetTemp!=null && targetTemp.HPAttribute.HP>0){
										target=targetTemp;
										if(attackMode==_AttackMode.StopNAttack){
											if(attackMethod!=_AttackMethod.Melee) unit.StopAnimation();
											unit.StopMoving();
											//if(dash){
											//	if(Vector3.Distance(unit.thisT.position, target.thisT.position)>8){
											//		//unit.PlayDash();
											//	}
											//}
										}
										break;
									}
								}
							}
						}
					
				}
				else if(targetArea==_TargetingA.Obstacle){
					
						RaycastHit hit;
						if(Physics.Raycast(unit.thisT.position, unit.thisT.forward, out hit, range, maskTarget)){
							Unit targetTemp=hit.collider.gameObject.GetComponent<Unit>();
							if(targetTemp!=null && targetTemp.HPAttribute.HP>0){
								target=targetTemp;
								if(attackMode==_AttackMode.StopNAttack){
									if(attackMethod!=_AttackMethod.Melee) unit.StopAnimation();
									unit.StopMoving();
									//if(dash){
									//	if(Vector3.Distance(unit.thisT.position, target.thisT.position)>8){
									//		//unit.PlayDash();
									//	}
									//}
								}
							}
						}
					
				}
			}
			else{
				//if target is out of range or dead or inactive, clear target
				currentTargetDist=Vector3.Distance(unit.thisT.position, target.thisT.position);
				if(currentTargetDist>range*1.25f || target.HPAttribute.HP<=0 || !target.thisObj.active){
					target=null;
					if(attackMode==_AttackMode.StopNAttack){
						unit.ResumeAnimation();
						unit.ResumeMoving();
					}
					if(meleeState==_MeleeState.Attacking){
						meleeState=_MeleeState.OutOfRange;
					}
				}
			}
			yield return null;
		}
	}
	
	private List<Collider> colList=new List<Collider>();
	IEnumerator SupportRoutine(){
		buff.buffID=unit.GetUnitID();
		LayerMask maskCreep=1<<LayerManager.LayerCreep();
		while(true){
			if(!unit.IsStunned() && !unit.IsDead()){
				Collider[] cols=Physics.OverlapSphere(unit.thisT.position, range, maskCreep);
				
				List<Collider> tempList=new List<Collider>();
				
				foreach(Collider col in cols){
					tempList.Add(col);
				}
				
				for(int i=0; i<colList.Count; i++){
					if(!tempList.Contains(colList[i])){
						colList[i].gameObject.GetComponent<UnitCreepAttack>().UnBuff(buff.buffID);
						colList.RemoveAt(i);
						i--;
					}
				}
				
				for(int i=0; i<tempList.Count; i++){
					if(!colList.Contains(tempList[i])){
						tempList[i].gameObject.GetComponent<UnitCreepAttack>().Buff(buff);
					}
				}

				yield return new WaitForSeconds(0.2f);
			}
		}
	}
	
	//called by a support creep to buff this creep
	private List<BuffStat> activeBuffList=new List<BuffStat>();
	public void Buff(BuffStat newBuff){
		if(activeBuffList.Contains(newBuff)) return;
		
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
			unit.GainHP(Time.deltaTime*buff.regenHP);
			yield return null;
		}
	}
	

	IEnumerator AttackRoutine(){
		if(sp==null) sp=unit.thisT;
		while(true){
			
			float delay=0;
			
			if(target!=null && !unit.IsStunned() && !unit.IsDead() && targetInLos && currentClip>0){	
				
				if(unit.PlayAttack()){
					delay=aniAttackTimeOffset;
					yield return new WaitForSeconds(delay);
				}
				
				if(attackMethod==_AttackMethod.Range){
					GameObject obj=ObjectPoolManager.Spawn(shootObject, sp.position, sp.rotation);
					ShootObject shootObj=obj.GetComponent<ShootObject>();
					shootObj.Shoot(target, this, sp);
					
					//currentClip-=1;
					//if(currentClip==0) StartCoroutine(Reload());
				}
				else if(attackMethod==_AttackMethod.Melee){
					ApplyEffect(target);
				}
				
				if(attackSound!=null) AudioManager.PlaySound(attackSound, unit.thisT.position);
				
				yield return new WaitForSeconds(cooldown-delay);
			}
			else{
				if(cdTracking==_CDTracking.Precise) yield return null;
				else yield return new WaitForSeconds(Random.Range(0, cooldown));
			}
		
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
		if(tgt!=null && tgt.gameObject!=null && tgt.gameObject.active){
			ApplyEffect(tgt, effect, div);
		}
	}
	
	//check stat and apply valid effect to the target
	void ApplyEffect(Unit targetUnit, bool effect, int div){
		if(targetUnit.thisObj.active){
			if(damage>0){
				if(div>0) targetUnit.ApplyDamage(damage/div);
				else targetUnit.ApplyDamage(damage);
			}
		}
	}
	
	void ApplyEffect(Unit targetUnit){
		if(damage>0) targetUnit.ApplyDamage(damage);
	}
	
	public Unit GetTarget(){
		return target;
	}
	
	void OnDrawGizmos(){
		if(target!=null){
			Gizmos.color=Color.red;
			Gizmos.DrawLine(unit.thisT.position, target.thisT.position);
		}
	}
}
