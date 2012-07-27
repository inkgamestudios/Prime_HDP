using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;



[CustomEditor(typeof(UnitCreepAttack))]
public class UnitCreepAttackEditor : Editor {

	private static string[] creepTypeLabel=new string[2];
	private static string[] tgtAreaLabel=new string[3];
	private static string[] attModeLabel=new string[2];
	private static string[] attMethodLabel=new string[2];
	private static string[] cdTrackingLabel=new string[2];
	
	private static int[] typeID=new int[4];
	
	 int creepType=0;
	 int targetArea=0;
	 int attackMode=0;
	 int attackMethod=0;
	 int cdTracking=0;
	
	void Awake(){
		
		creepTypeLabel[0]="Attack";
		creepTypeLabel[1]="Support";
		
		tgtAreaLabel[0]="AllAround";
		tgtAreaLabel[1]="FrontalCone";
		tgtAreaLabel[2]="Obstacle";
		
		attModeLabel[0]="RunNGun";
		attModeLabel[1]="StopNAttack";
		
		attMethodLabel[0]="Range";
		attMethodLabel[1]="Melee";
		
		cdTrackingLabel[0]="Easy";
		cdTrackingLabel[1]="Precise";
		
		typeID[0]=0;
		typeID[1]=1;
		typeID[2]=2;
		typeID[3]=3;
		
	}
	
	
	
	public override void OnInspectorGUI(){
		UnitCreepAttack creep = (UnitCreepAttack)target;
		
		GUI.changed = false;
		
		EditorGUILayout.Space();
		
		
		if(creep.type==_AttackCreepType.Attack) targetArea=0;
		else if(creep.type==_AttackCreepType.Support) targetArea=1;
		
		creepType = EditorGUILayout.IntPopup("CreepType: ", creepType, creepTypeLabel, typeID);
		
		if(creepType==0){
			creep.type=_AttackCreepType.Attack;
		}
		else if(creepType==1){
			creep.type=_AttackCreepType.Support;
		}
		
		if(creep.attackMode==_AttackMode.RunNGun) attackMode=0;
		else if(creep.attackMode==_AttackMode.StopNAttack) attackMode=1;
			
		attackMode = EditorGUILayout.IntPopup("Mode: ", attackMode, attModeLabel, typeID);
		
		if(attackMode==0){
			creep.attackMode=_AttackMode.RunNGun;
		}
		else if(attackMode==1){
			creep.attackMode=_AttackMode.StopNAttack;
		}		
		
		if(creep.type==_AttackCreepType.Attack){
			if(creep.targetArea==_TargetingA.AllAround) targetArea=0;
			else if(creep.targetArea==_TargetingA.FrontalCone) targetArea=1;
			else if(creep.targetArea==_TargetingA.Obstacle) targetArea=2;
			
			targetArea = EditorGUILayout.IntPopup("TargetingArea: ", targetArea, tgtAreaLabel, typeID);
			
			if(targetArea==0){
				creep.targetArea=_TargetingA.AllAround;
			}
			else if(targetArea==1){
				creep.targetArea=_TargetingA.FrontalCone;
				creep.frontalConeAngle = EditorGUILayout.FloatField("frontalConeAngle:", creep.frontalConeAngle);
			}
			else if(targetArea==2){
				creep.targetArea=_TargetingA.Obstacle;
			}
			
			
			if(creep.attackMethod==_AttackMethod.Range) attackMethod=0;
			else if(creep.attackMethod==_AttackMethod.Melee) attackMethod=1;
			
			if(creep.cdTracking==_CDTracking.Easy) cdTracking=0;
			else if(creep.cdTracking==_CDTracking.Precise) cdTracking=1;
			
			attackMethod = EditorGUILayout.IntPopup("AttackMethod: ", attackMethod, attMethodLabel, typeID);
			cdTracking = EditorGUILayout.IntPopup("CDTracking: ", cdTracking, cdTrackingLabel, typeID);
			
			
			
			if(attackMethod==0){
				creep.attackMethod=_AttackMethod.Range;
			}
			else if(attackMethod==1){
				creep.attackMethod=_AttackMethod.Melee;
				creep.meleeAttackRange = EditorGUILayout.FloatField("AttackRange:", creep.meleeAttackRange);
			}
			
			if(cdTracking==0){
				creep.cdTracking=_CDTracking.Easy;
			}
			else if(cdTracking==1){
				creep.cdTracking=_CDTracking.Precise;
			}
			
			if(creep.attackMethod==_AttackMethod.Range) 
				creep.range = EditorGUILayout.FloatField("AttackRange:", creep.range);
			else if(creep.attackMethod==_AttackMethod.Melee) 
				creep.range = EditorGUILayout.FloatField("TargetRange:", creep.range);
			
			
			creep.cooldown = EditorGUILayout.FloatField("AttackCooldown:", creep.cooldown);
			creep.damage = EditorGUILayout.FloatField("Damage:", creep.damage);
			creep.stun = EditorGUILayout.FloatField("StunDuration:", creep.stun);
			
			if(creep.attackMethod==_AttackMethod.Range){
				creep.turretObject=(Transform)EditorGUILayout.ObjectField("TurretObject:", creep.turretObject, typeof(Transform), false);
				creep.sp=(Transform)EditorGUILayout.ObjectField("ShootPoint:", creep.sp, typeof(Transform), false);
				
				creep.shootObject=(GameObject)EditorGUILayout.ObjectField("ShootObject:", creep.shootObject, typeof(GameObject), false);
			}
				
			creep.attackSound=(AudioClip)EditorGUILayout.ObjectField("AttackSound:", creep.attackSound, typeof(AudioClip), false);
			creep.animationIdle=(AnimationClip)EditorGUILayout.ObjectField("IdleAnimation:", creep.animationIdle, typeof(AnimationClip), false);
			creep.animationAttack=(AnimationClip)EditorGUILayout.ObjectField("AttackAnimation:", creep.animationAttack, typeof(AnimationClip), false);
			if(creep.animationAttack!=null) creep.aniAttackTimeOffset=EditorGUILayout.FloatField("AttackAnimationTimeOffset:", creep.aniAttackTimeOffset);

		}
		else if(creep.type==_AttackCreepType.Support){
			if(creep.attackMode==_AttackMode.RunNGun) attackMode=0;
			//~ else if(creep.attackMode==_AttackMode.StopNAttack) attackMode=1;
			
			//~ attackMode = EditorGUILayout.IntPopup("Mode: ", attackMode, attModeLabel, typeID);
			
			//~ if(attackMode==0) creep.attackMode=_AttackMode.RunNGun;
			//~ else if(attackMode==1) creep.attackMode=_AttackMode.StopNAttack;
			
			creep.range = EditorGUILayout.FloatField("EffectiveRange:", creep.range);
			
			
			EditorGUILayout.LabelField("Buff");
			creep.buff.damageBuff = EditorGUILayout.FloatField("DamageBuff:", creep.buff.damageBuff);
			creep.buff.rangeBuff = EditorGUILayout.FloatField("RangeBuff:", creep.buff.rangeBuff);
			creep.buff.cooldownBuff = EditorGUILayout.FloatField("cooldownBuff:", creep.buff.cooldownBuff);
			creep.buff.regenHP = EditorGUILayout.FloatField("HPRegenRate:", creep.buff.regenHP);
			
			
			creep.sp=(Transform)EditorGUILayout.ObjectField("ShootPoint:", creep.sp, typeof(Transform), false);
			creep.shootObject=(GameObject)EditorGUILayout.ObjectField("ShootObject:", creep.shootObject, typeof(GameObject), false);
			creep.cooldown = EditorGUILayout.FloatField("EffectCooldown:", creep.cooldown);
			creep.animationIdle=(AnimationClip)EditorGUILayout.ObjectField("IdleAnimation:", creep.animationIdle, typeof(AnimationClip), false);
			
		}
		
		if(GUI.changed) EditorUtility.SetDirty(creep);
	}
	
}
