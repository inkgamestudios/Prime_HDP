using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;


public class TowerEditor : EditorWindow {
    
	int levelCap=1;
	
	[SerializeField] static string[] nameList=new string[0];
	
	[SerializeField] static UnitTower[] towerList=new UnitTower[0];
	
	
	UnitTower newTower;
	
	UnitTower tower;
	
	int index=0;
	int towerType=0;
	int towerTargetMode=0;
	int turretAnimateMode=0;
	
	static string[] towerTypeLabel=new string[6];
	static string[] towerTargetModeLabel=new string[3];
	static string[] turretAnimateModeLabel=new string[3];
    
	private bool[] indicatorFlags=new bool[1];
	
	private bool showAnimationList=false;
	private string showAnimationText="Show animation configuration";
	
	private bool showSoundList=false;
	private string showSoundText="Show sfx list";
	
	static private TowerEditor window;
	
    // Add menu named "TowerEditor" to the Window menu
    [MenuItem ("TDTK/TowerEditor")]
    static void Init () {
        // Get existing open window or if none, make a new one:
        window = (TowerEditor)EditorWindow.GetWindow(typeof (TowerEditor));
		window.minSize=new Vector2(700, 650);
		
		GetTower();
		
		towerTypeLabel[0]="Turret Tower";
		towerTypeLabel[1]="AOE Tower";
		towerTypeLabel[2]="Directional AOE Tower";
		towerTypeLabel[3]="Support Tower";
		towerTypeLabel[4]="Resource Tower";
		towerTypeLabel[5]="Mine";
		
		towerTargetModeLabel[0]="Hybrid";
		towerTargetModeLabel[1]="Air";
		towerTargetModeLabel[2]="Ground";
		
		turretAnimateModeLabel[0]="Full";
		turretAnimateModeLabel[1]="Y-Axis Only";
		turretAnimateModeLabel[2]="None";
		
    }
    
	static BuildManager buildManager;
	static ResourceManager rscManager;
	
	static void GetTower(){
		buildManager=(BuildManager)FindObjectOfType(typeof(BuildManager));
		
		if(buildManager!=null){
			towerList=buildManager.towers;
			
			nameList=new string[towerList.Length];
			for(int i=0; i<towerList.Length; i++){
				nameList[i]=towerList[i].name;
			}
		}
		else{
			towerList=new UnitTower[0];
			nameList=new string[0];
		}
		
		rscManager=(ResourceManager)FindObjectOfType(typeof(ResourceManager));
		
	}
	
	float startX, startY, height, spaceY, lW;
	int rscCount=1;
	
    void OnGUI () {
		
		GUI.changed = false;
		
		startX=3;
		startY=3;
		height=18;
		spaceY=height+startX;
		
		lW=100;	//label width, the offset from label to the editable field
		
		if(towerList.Length>0) {
			index = EditorGUI.Popup(new Rect(startX, startY, 300, height), "Tower:", index, nameList);
			
			//new tower, update index
			levelCap=towerList[index].levelCap;			
			EditorGUI.LabelField(new Rect(320+startX, startY, 200, height), "LevelCap: "+towerList[index].levelCap.ToString());
			EditorGUI.LabelField(new Rect(395+startX, startY, 200, height), "Change to: ");
			levelCap = EditorGUI.IntField(new Rect(startX+465, startY, 20, height), levelCap);
			levelCap = Mathf.Max(1, levelCap);
			UpdateIndicatorFlags(levelCap);
			
			if(levelCap!=towerList[index].levelCap){
				towerList[index].levelCap=levelCap;
				towerList[index].UpdateTowerUpgradeStat(levelCap-1);
			}
			
			//assign appropriate towerType index
			if(towerList[index].type==_TowerType.TurretTower) towerType=0;
			else if(towerList[index].type==_TowerType.AOETower) towerType=1;
			else if(towerList[index].type==_TowerType.DirectionalAOETower) towerType=2;
			else if(towerList[index].type==_TowerType.SupportTower) towerType=3;
			else if(towerList[index].type==_TowerType.ResourceTower) towerType=4;
			else if(towerList[index].type==_TowerType.Mine) towerType=5;
			
			//assign appropriate towerTargetMode index
			if(towerList[index].targetMode==_TargetMode.Hybrid) towerTargetMode=0;
			else if(towerList[index].targetMode==_TargetMode.Air) towerTargetMode=1;
			else if(towerList[index].targetMode==_TargetMode.Ground) towerTargetMode=2;
			
			if(towerList[index].type==_TowerType.TurretTower){
                if(towerList[index].animateTurret==_TurretAni.Full) turretAnimateMode=0;
                if(towerList[index].animateTurret==_TurretAni.YAxis) turretAnimateMode=1;
                if(towerList[index].animateTurret==_TurretAni.None) turretAnimateMode=2;
            }
			
			if(GUI.Button(new Rect(Mathf.Max(startX+500, window.position.width-120), startY, 100, height), "Update")) GetTower();
		}
		else{
			if(GUI.Button(new Rect(startX, startY, 100, height), "Update")) GetTower();
			return;
		}
		
		towerList[index].unitName = EditorGUI.TextField(new Rect(startX, startY+=30, 300, height-3), "TowerName:", towerList[index].unitName);
        
        EditorGUI.LabelField(new Rect(startX+320, startY, 70, height), "Icon: ");
        towerList[index].icon=(Texture)EditorGUI.ObjectField(new Rect(startX+360, startY, 70, 70), towerList[index].icon, typeof(Texture), false);
                  
        towerType = EditorGUI.Popup(new Rect(startX, startY+=20, 300, 15), "TowerType:", towerType, towerTypeLabel);
        if(towerType==0) towerList[index].type=_TowerType.TurretTower;
        else if(towerType==1) towerList[index].type=_TowerType.AOETower;
        else if(towerType==2) towerList[index].type=_TowerType.DirectionalAOETower;
        else if(towerType==3) towerList[index].type=_TowerType.SupportTower;
        else if(towerType==4) towerList[index].type=_TowerType.ResourceTower;
        else if(towerType==5) towerList[index].type=_TowerType.Mine;
        
		if(towerList[index].type==_TowerType.TurretTower || towerList[index].type==_TowerType.DirectionalAOETower || towerList[index].type==_TowerType.AOETower){
			towerTargetMode = EditorGUI.Popup(new Rect(startX, startY+=20, 300, 15), "TargetMode:", towerTargetMode, towerTargetModeLabel);
			if(towerTargetMode==0) towerList[index].targetMode=_TargetMode.Hybrid;
			else if(towerTargetMode==1) towerList[index].targetMode=_TargetMode.Air;
			else if(towerTargetMode==2) towerList[index].targetMode=_TargetMode.Ground;
		}
		else startY+=20;
		
		if(towerList[index].type==_TowerType.TurretTower || towerList[index].type==_TowerType.DirectionalAOETower){
			turretAnimateMode = EditorGUI.Popup(new Rect(startX, startY+=20, 300, 15), "TurretAnimateMode:", turretAnimateMode, turretAnimateModeLabel);
			if(turretAnimateMode==0) towerList[index].animateTurret=_TurretAni.Full;
			else if(turretAnimateMode==1) towerList[index].animateTurret=_TurretAni.YAxis;
			else if(turretAnimateMode==2) towerList[index].animateTurret=_TurretAni.None;
		}
		else startY+=20;
		
		if(towerList[index].type==_TowerType.DirectionalAOETower){
			towerList[index].projectingArc = EditorGUI.FloatField(new Rect(startX, startY+=20, 300, height-3), "ProjectingArcAngle:", towerList[index].projectingArc);
		}
		
		if(towerList[index].type==_TowerType.Mine){
			towerList[index].mineOneOff=EditorGUI.Toggle(new Rect(startX, startY, 300, 15), "DestroyUponTriggered:", towerList[index].mineOneOff);
		}
		
		showAnimationList=EditorGUI.Foldout(new Rect(startX, startY+=spaceY, 300, 15), showAnimationList, showAnimationText);
		if(showAnimationList){
			towerList[index].buildAnimationBody=(Animation)EditorGUI.ObjectField(new Rect(startX, startY+=spaceY, 300, 17), " - Build Animation Body: ", towerList[index].buildAnimationBody, typeof(Animation), false);
			towerList[index].buildAnimation=(AnimationClip)EditorGUI.ObjectField(new Rect(startX, startY+=spaceY, 300, 17), " - Build Animation: ", towerList[index].buildAnimation, typeof(AnimationClip), false);
			towerList[index].fireAnimationBody=(Animation)EditorGUI.ObjectField(new Rect(startX, startY+=spaceY, 300, 17), " - Fire Animation Body: ", towerList[index].fireAnimationBody, typeof(Animation), false);
			towerList[index].fireAnimation=(AnimationClip)EditorGUI.ObjectField(new Rect(startX, startY+=spaceY, 300, 17), " - Fire Animation: ", towerList[index].fireAnimation, typeof(AnimationClip), false);
			
			showAnimationText="Hide animation configuration";
		}
		else{
			showAnimationText="Show animation configuration";
		}
		
		showSoundList=EditorGUI.Foldout(new Rect(startX, startY+=spaceY, 300, 15), showSoundList, showSoundText);
		if(showSoundList){
			towerList[index].shootSound=(AudioClip)EditorGUI.ObjectField(new Rect(startX, startY+=spaceY, 300, 17), " - Shoot Sound: ", towerList[index].shootSound, typeof(AudioClip), false);
			towerList[index].buildingSound=(AudioClip)EditorGUI.ObjectField(new Rect(startX, startY+=spaceY, 300, 17), " - Building Sound: ", towerList[index].buildingSound, typeof(AudioClip), false);
			towerList[index].builtSound=(AudioClip)EditorGUI.ObjectField(new Rect(startX, startY+=spaceY, 300, 17), " - Built Sound: ", towerList[index].builtSound, typeof(AudioClip), false);
			towerList[index].soldSound=(AudioClip)EditorGUI.ObjectField(new Rect(startX, startY+=spaceY, 300, 17), " - Sold Sound: ", towerList[index].soldSound, typeof(AudioClip), false);
			
			showSoundText="Hide sfx list";
		}
		else{
			showSoundText="Show sfx list";
		}
        
        
		EditorGUI.LabelField(new Rect(startX, startY+=25, 150, height), "Tower Description: ");
		towerList[index].description=EditorGUI.TextArea(new Rect(startX, startY+=17, 485, 50), towerList[index].description);
		startY+=25;
		
		
		//position in which the stat editor for tower levels start
		startY+=20;
		float tabYPos=startY;
		
		
		if(rscManager!=null) rscCount=rscManager.resources.Length;
		else rscCount=1;
		
		
		//for turretTower and AoeTower
		if(index>=0 && index<towerList.Length){
			
			indicatorFlags[0] = EditorGUI.Toggle(new Rect(startX, startY+spaceY-10, 20, height), indicatorFlags[0]);
			startY+=10;
			
			if(indicatorFlags[0]){
				GUI.Box(new Rect(startX, startY+spaceY-1, 175, 445+(rscCount*20)), "");
				startX+=3;
				
				EditorGUI.LabelField(new Rect(50+startX, startY+=spaceY, 200, height), "Level 1: ");
				
				
				if(rscCount!=towerList[index].baseStat.costs.Length){
					UpdateBaseStatCost(index, rscCount);
				}
				
				if(towerList[index].baseStat.costs.Length==1){
					EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "Cost: ");
					towerList[index].baseStat.costs[0] = EditorGUI.IntField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.costs[0]);
				}
				else{
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY-5, 200, height), "Cost: ");
					for(int i=0; i<towerList[index].baseStat.costs.Length; i++){
						string rscName="";
						if(rscManager!=null) rscName=rscManager.resources[i].name;
						EditorGUI.LabelField(new Rect(startX, startY+spaceY-3, 200, height), " - "+rscName+": ");
						
						towerList[index].baseStat.costs[i] = EditorGUI.IntField(new Rect(startX+lW, startY+=spaceY-3, 50, height-2), towerList[index].baseStat.costs[i]);
					}
					startY+=8;
				}
				
				EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "BuildDuration: ");
				towerList[index].baseStat.buildDuration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.buildDuration);
				
				startY+=3;
				
				TypeDependentBaseStat(index);
				
				spaceY+=2;	startY+=8;
				
				if(towerList[index].type!=_TowerType.Mine){
					EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "ShootObj: ");
					towerList[index].baseStat.shootObject=(Transform)EditorGUI.ObjectField(new Rect(startX+lW-30, startY+=spaceY, 100, height-2), towerList[index].baseStat.shootObject, typeof(Transform), false);
					
					EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "TurretObj: ");
					towerList[index].baseStat.turretObject=(Transform)EditorGUI.ObjectField(new Rect(startX+lW-30, startY+=spaceY, 100, height-2), towerList[index].baseStat.turretObject, typeof(Transform), false);
					
					EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "BaseObj: ");
					towerList[index].baseStat.baseObject=(Transform)EditorGUI.ObjectField(new Rect(startX+lW-30, startY+=spaceY, 100, height-2), towerList[index].baseStat.baseObject, typeof(Transform), false);
				}
				
				startX+=190;	startY=tabYPos;	spaceY=21;
			}
			else{
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, 200, height), "1");
				startX+=35;	startY=tabYPos;	
			}
			
			for(int i=0; i<towerList[index].upgradeStat.Length; i++){
				
				if(towerList[index]!=null && towerList[index].upgradeStat[i]!=null){
				
					indicatorFlags[i+1] = EditorGUI.Toggle(new Rect(startX, startY+spaceY-10, 20, height), indicatorFlags[i+1]);
					startY+=10;
					
					if(indicatorFlags[i+1]){
						GUI.Box(new Rect(startX, startY+spaceY-1, 175, 445+(rscCount*20)), "");
						startX+=3;
						
						EditorGUI.LabelField(new Rect(50+startX, startY+=spaceY, 200, height), "Level "+(i+2).ToString()+": ");
						
						//EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "Cost: ");
						//towerList[index].upgradeStat[i].cost = EditorGUI.IntField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[i].cost);
						
						if(rscCount!=towerList[index].upgradeStat[i].costs.Length){
							UpdateUpgradeStatCost(index, rscCount);
						}
						
						if(towerList[index].upgradeStat[i].costs.Length==1){
							EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "Cost: ");
							towerList[index].upgradeStat[i].costs[0] = EditorGUI.IntField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[i].costs[0]);
						}
						else{
							EditorGUI.LabelField(new Rect(startX, startY+=spaceY-5, 200, height), "Cost: ");
							for(int j=0; j<towerList[index].upgradeStat[i].costs.Length; j++){
								string rscName="";
								if(rscManager!=null) rscName=rscManager.resources[j].name;
								EditorGUI.LabelField(new Rect(startX, startY+spaceY-3, 200, height), " - "+rscName+": ");
								
								towerList[index].upgradeStat[i].costs[j] = EditorGUI.IntField(new Rect(startX+lW, startY+=spaceY-3, 50, height-2), towerList[index].upgradeStat[i].costs[j]);
							}
							startY+=8;
						}
						
						
						EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "BuildDuration: ");
						towerList[index].upgradeStat[i].buildDuration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[i].buildDuration);
						startY+=3;
						
						TypeDependentUpgradeStat(index, i);
						
						spaceY+=2;	startY+=8;
						
						if(towerList[index].type!=_TowerType.Mine){
							EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "ShootObj: ");
							towerList[index].upgradeStat[i].shootObject=(Transform)EditorGUI.ObjectField(new Rect(startX+lW-30, startY+=spaceY, 100, height-2), towerList[index].upgradeStat[i].shootObject, typeof(Transform), false);
							
							EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "TurretObj: ");
							towerList[index].upgradeStat[i].turretObject=(Transform)EditorGUI.ObjectField(new Rect(startX+lW-30, startY+=spaceY, 100, height-2), towerList[index].upgradeStat[i].turretObject, typeof(Transform), false);
							
							EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "BaseObj: ");
							towerList[index].upgradeStat[i].baseObject=(Transform)EditorGUI.ObjectField(new Rect(startX+lW-30, startY+=spaceY, 100, height-2), towerList[index].upgradeStat[i].baseObject, typeof(Transform), false);
						}
							
						startX+=190;
						startY=tabYPos;
						spaceY=21;
					}
					else{
						EditorGUI.LabelField(new Rect(startX, startY+=spaceY, 200, height), (i+2).ToString());
						startX+=25;	startY=tabYPos;
					}
				}
			
			}
			
		}
	
		if(GUI.changed) EditorUtility.SetDirty(towerList[index]);
    }
	
	void TypeDependentBaseStat(int index){
		if(towerType==0){
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY+5, 200, height), "Damage: ");
			towerList[index].baseStat.damage = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY+5, 50, height-2), towerList[index].baseStat.damage);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "Cooldown: ");
			towerList[index].baseStat.cooldown = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.cooldown);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "ReloadDuration: ");
			towerList[index].baseStat.reloadDuration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.reloadDuration);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "ClipSize: ");
			towerList[index].baseStat.clipSize = EditorGUI.IntField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.clipSize);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "Range: ");
			towerList[index].baseStat.range = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.range);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "AoeRadius: ");
			towerList[index].baseStat.aoeRadius = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.aoeRadius);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "StunDuration: ");
			towerList[index].baseStat.stunDuration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.stunDuration);
			
			
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY+3, 200, height), "Slow Effect: ");
			spaceY-=3;
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Duration: ");
			towerList[index].baseStat.slow.duration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.slow.duration);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- SlowFactor: ");
			towerList[index].baseStat.slow.slowFactor = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.slow.slowFactor);
			
			spaceY+=3;
			
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY+3, 200, height), "DamageOverTime: ");
			spaceY-=3;
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Damage: ");
			towerList[index].baseStat.dot.damage = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.dot.damage);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Duration: ");
			towerList[index].baseStat.dot.duration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.dot.duration);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Interval: ");
			towerList[index].baseStat.dot.interval = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.dot.interval);
			
			float ttDmg=towerList[index].baseStat.dot.damage*towerList[index].baseStat.dot.duration/towerList[index].baseStat.dot.interval;
			EditorGUI.LabelField(new Rect(startX+10, startY+=spaceY+3, 160, height), "TotalDamage:  "+ttDmg.ToString("f1"));
			
			//spaceY+=3;
			//startY+=5;
			
		}
		else if(towerType==1){
			EditorGUI.LabelField(new Rect(startX, startY+spaceY+5, 200, height), "Damage: ");
			towerList[index].baseStat.damage = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY+5, 50, height-2), towerList[index].baseStat.damage);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "Cooldown: ");
			towerList[index].baseStat.cooldown = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.cooldown);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "Range: ");
			towerList[index].baseStat.range = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.range);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "StunDuration: ");
			towerList[index].baseStat.stunDuration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.stunDuration);
			
			
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY+3, 200, height), "Slow Effect: ");
			spaceY-=3;
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Duration: ");
			towerList[index].baseStat.slow.duration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.slow.duration);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- SlowFactor: ");
			towerList[index].baseStat.slow.slowFactor = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.slow.slowFactor);
			
			spaceY+=3;
			
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY+3, 200, height), "DamageOverTime: ");
			spaceY-=3;
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Damage: ");
			towerList[index].baseStat.dot.damage = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.dot.damage);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Duration: ");
			towerList[index].baseStat.dot.duration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.dot.duration);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Interval: ");
			towerList[index].baseStat.dot.interval = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.dot.interval);
			
			float ttDmg=towerList[index].baseStat.dot.damage*towerList[index].baseStat.dot.duration/towerList[index].baseStat.dot.interval;
			EditorGUI.LabelField(new Rect(startX+10, startY+=spaceY+3, 160, height), "TotalDamage:  "+ttDmg.ToString("f1"));
			
		}
		else if(towerType==2){
			EditorGUI.LabelField(new Rect(startX, startY+spaceY+5, 200, height), "Damage: ");
			towerList[index].baseStat.damage = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY+5, 50, height-2), towerList[index].baseStat.damage);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "Cooldown: ");
			towerList[index].baseStat.cooldown = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.cooldown);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "ReloadDuration: ");
			towerList[index].baseStat.reloadDuration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.reloadDuration);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "ClipSize: ");
			towerList[index].baseStat.clipSize = EditorGUI.IntField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.clipSize);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "Range: ");
			towerList[index].baseStat.range = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.range);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "StunDuration: ");
			towerList[index].baseStat.stunDuration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.stunDuration);
			
			
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY+3, 200, height), "Slow Effect: ");
			spaceY-=3;
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Duration: ");
			towerList[index].baseStat.slow.duration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.slow.duration);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- SlowFactor: ");
			towerList[index].baseStat.slow.slowFactor = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.slow.slowFactor);
			
			spaceY+=3;
			
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY+3, 200, height), "DamageOverTime: ");
			spaceY-=3;
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Damage: ");
			towerList[index].baseStat.dot.damage = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.dot.damage);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Duration: ");
			towerList[index].baseStat.dot.duration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.dot.duration);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Interval: ");
			towerList[index].baseStat.dot.interval = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.dot.interval);
			
			float ttDmg=towerList[index].baseStat.dot.damage*towerList[index].baseStat.dot.duration/towerList[index].baseStat.dot.interval;
			EditorGUI.LabelField(new Rect(startX+10, startY+=spaceY+3, 160, height), "TotalDamage:  "+ttDmg.ToString("f1"));
			
			//spaceY+=3;
			//startY+=5;
			
		}
		else if(towerType==3){
			
			spaceY+=5;
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY+3, 200, height), "Buff Effect: ");
			spaceY-=5;
			startY-=3;
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Damage: ");
			towerList[index].baseStat.buff.damageBuff = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.buff.damageBuff);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Cooldown: ");
			towerList[index].baseStat.buff.cooldownBuff = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.buff.cooldownBuff);
			towerList[index].baseStat.buff.cooldownBuff = Mathf.Clamp(towerList[index].baseStat.buff.cooldownBuff, -0.8f, 0.8f);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Range: ");
			towerList[index].baseStat.buff.rangeBuff = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.buff.rangeBuff);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY+10, 200, height), "Cooldown: ");
			towerList[index].baseStat.cooldown = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY+10, 50, height-2), towerList[index].baseStat.cooldown);
			
			
		}
		else if(towerType==4){
			
			//EditorGUI.LabelField(new Rect(startX, startY+spaceY+10, 200, height), "IncomeValue: ");
			//towerList[index].baseStat.incomeValue = EditorGUI.IntField(new Rect(startX+lW, startY+=spaceY+10, 50, height-2), towerList[index].baseStat.incomeValue);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY+10, 200, height), "Cooldown: ");
			towerList[index].baseStat.cooldown = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY+10, 50, height-2), towerList[index].baseStat.cooldown);
			
			startY+=10;
			
			if(rscCount!=towerList[index].baseStat.incomes.Length){
				UpdateBaseStatIncomes(index, rscCount);
			}
			
			if(towerList[index].baseStat.incomes.Length==1){
				EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "resources:");
				towerList[index].baseStat.incomes[0] = EditorGUI.IntField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.incomes[0]);
			}
			else{
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY-5, 200, height), "Resources Per CD:");
				for(int i=0; i<towerList[index].baseStat.incomes.Length; i++){
					string rscName="";
					if(rscManager!=null) rscName=rscManager.resources[i].name;
					EditorGUI.LabelField(new Rect(startX, startY+spaceY-3, 200, height), " - "+rscName+": ");
					
					towerList[index].baseStat.incomes[i] = EditorGUI.IntField(new Rect(startX+lW, startY+=spaceY-3, 50, height-2), towerList[index].baseStat.incomes[i]);
				}
				startY+=8;
			}
			
		}
		else if(towerType==5){
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY+5, 200, height), "Damage: ");
			towerList[index].baseStat.damage = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY+5, 50, height-2), towerList[index].baseStat.damage);
			
			if(!towerList[index].mineOneOff){
				EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "Cooldown: ");
				towerList[index].baseStat.cooldown = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.cooldown);
			}
				
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "EffectiveRange: ");
			towerList[index].baseStat.range = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.range);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "StunDuration: ");
			towerList[index].baseStat.stunDuration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.stunDuration);
			
			
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY+3, 200, height), "Slow Effect: ");
			spaceY-=3;
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Duration: ");
			towerList[index].baseStat.slow.duration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.slow.duration);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- SlowFactor: ");
			towerList[index].baseStat.slow.slowFactor = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.slow.slowFactor);
			
			spaceY+=3;
			
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY+3, 200, height), "DamageOverTime: ");
			spaceY-=3;
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Damage: ");
			towerList[index].baseStat.dot.damage = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.dot.damage);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Duration: ");
			towerList[index].baseStat.dot.duration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.dot.duration);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Interval: ");
			towerList[index].baseStat.dot.interval = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].baseStat.dot.interval);
			
			float ttDmg=towerList[index].baseStat.dot.damage*towerList[index].baseStat.dot.duration/towerList[index].baseStat.dot.interval;
			EditorGUI.LabelField(new Rect(startX+10, startY+=spaceY+3, 160, height), "TotalDamage:  "+ttDmg.ToString("f1"));
			
		}
	}
	
	
	
	
	
	
	
	
	
	void TypeDependentUpgradeStat(int index, int lvl){
		if(towerType==0){
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY+5, 200, height), "Damage: ");
			towerList[index].upgradeStat[lvl].damage = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY+5, 50, height-2), towerList[index].upgradeStat[lvl].damage);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "Cooldown: ");
			towerList[index].upgradeStat[lvl].cooldown = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].cooldown);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "ReloadDuration: ");
			towerList[index].upgradeStat[lvl].reloadDuration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].reloadDuration);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "ClipSize: ");
			towerList[index].upgradeStat[lvl].clipSize = EditorGUI.IntField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].clipSize);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "Range: ");
			towerList[index].upgradeStat[lvl].range = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].range);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "AoeRadius: ");
			towerList[index].upgradeStat[lvl].aoeRadius = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].aoeRadius);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "StunDuration: ");
			towerList[index].upgradeStat[lvl].stunDuration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].stunDuration);
			
			
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY+3, 200, height), "Slow Effect: ");
			spaceY-=3;
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Duration: ");
			towerList[index].upgradeStat[lvl].slow.duration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].slow.duration);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- SlowFactor: ");
			towerList[index].upgradeStat[lvl].slow.slowFactor = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].slow.slowFactor);
			
			spaceY+=3;
			
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY+3, 200, height), "DamageOverTime: ");
			spaceY-=3;
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Damage: ");
			towerList[index].upgradeStat[lvl].dot.damage = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].dot.damage);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Duration: ");
			towerList[index].upgradeStat[lvl].dot.duration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].dot.duration);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Interval: ");
			towerList[index].upgradeStat[lvl].dot.interval = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].dot.interval);
			
			float ttDmg=towerList[index].baseStat.dot.damage*towerList[index].baseStat.dot.duration/towerList[index].baseStat.dot.interval;
			EditorGUI.LabelField(new Rect(startX+10, startY+=spaceY+3, 160, height), "TotalDamage:  "+ttDmg.ToString("f1"));
			
			//spaceY+=3;
			//startY+=5;
			
		}
		else if(towerType==1){
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY+5, 200, height), "Damage: ");
			towerList[index].upgradeStat[lvl].damage = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY+5, 50, height-2), towerList[index].upgradeStat[lvl].damage);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "Cooldown: ");
			towerList[index].upgradeStat[lvl].cooldown = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].cooldown);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "Range: ");
			towerList[index].upgradeStat[lvl].range = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].range);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "StunDuration: ");
			towerList[index].upgradeStat[lvl].stunDuration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].stunDuration);
			
			
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY+3, 200, height), "Slow Effect: ");
			spaceY-=3;
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Duration: ");
			towerList[index].upgradeStat[lvl].slow.duration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].slow.duration);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- SlowFactor: ");
			towerList[index].upgradeStat[lvl].slow.slowFactor = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].slow.slowFactor);
			
			spaceY+=3;
			
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY+3, 200, height), "DamageOverTime: ");
			spaceY-=3;
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Damage: ");
			towerList[index].upgradeStat[lvl].dot.damage = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].dot.damage);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Duration: ");
			towerList[index].upgradeStat[lvl].dot.duration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].dot.duration);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Interval: ");
			towerList[index].upgradeStat[lvl].dot.interval = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].dot.interval);
			
			float ttDmg=towerList[index].baseStat.dot.damage*towerList[index].baseStat.dot.duration/towerList[index].baseStat.dot.interval;
			EditorGUI.LabelField(new Rect(startX+10, startY+=spaceY+3, 160, height), "TotalDamage:  "+ttDmg.ToString("f1"));
		}
		else if(towerType==2){
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY+5, 200, height), "Damage: ");
			towerList[index].upgradeStat[lvl].damage = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY+5, 50, height-2), towerList[index].upgradeStat[lvl].damage);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "Cooldown: ");
			towerList[index].upgradeStat[lvl].cooldown = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].cooldown);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "ReloadDuration: ");
			towerList[index].upgradeStat[lvl].reloadDuration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].reloadDuration);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "ClipSize: ");
			towerList[index].upgradeStat[lvl].clipSize = EditorGUI.IntField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].clipSize);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "Range: ");
			towerList[index].upgradeStat[lvl].range = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].range);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "StunDuration: ");
			towerList[index].upgradeStat[lvl].stunDuration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].stunDuration);
			
			
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY+3, 200, height), "Slow Effect: ");
			spaceY-=3;
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Duration: ");
			towerList[index].upgradeStat[lvl].slow.duration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].slow.duration);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- SlowFactor: ");
			towerList[index].upgradeStat[lvl].slow.slowFactor = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].slow.slowFactor);
			
			spaceY+=3;
			
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY+3, 200, height), "DamageOverTime: ");
			spaceY-=3;
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Damage: ");
			towerList[index].upgradeStat[lvl].dot.damage = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].dot.damage);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Duration: ");
			towerList[index].upgradeStat[lvl].dot.duration = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].dot.duration);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Interval: ");
			towerList[index].upgradeStat[lvl].dot.interval = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].dot.interval);
			
			float ttDmg=towerList[index].baseStat.dot.damage*towerList[index].baseStat.dot.duration/towerList[index].baseStat.dot.interval;
			EditorGUI.LabelField(new Rect(startX+10, startY+=spaceY+3, 160, height), "TotalDamage:  "+ttDmg.ToString("f1"));
			
			//spaceY+=3;
			//startY+=5;
			
		}
		else if(towerType==3){
			
			spaceY+=5;
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY+3, 200, height), "Buff Effect: ");
			spaceY-=5;
			startY-=3;
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Damage: ");
			towerList[index].upgradeStat[lvl].buff.damageBuff = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].buff.damageBuff);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Cooldown: ");
			towerList[index].upgradeStat[lvl].buff.cooldownBuff = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].buff.cooldownBuff);
			towerList[index].upgradeStat[lvl].buff.cooldownBuff = Mathf.Clamp(towerList[index].upgradeStat[lvl].buff.cooldownBuff, -0.8f, 0.8f);
			
			EditorGUI.LabelField(new Rect(startX+10, startY+spaceY, 200, height), "- Range: ");
			towerList[index].upgradeStat[lvl].buff.rangeBuff = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].buff.rangeBuff);
			
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY+10, 200, height), "Cooldown: ");
			towerList[index].upgradeStat[lvl].cooldown = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY+10, 50, height-2), towerList[index].upgradeStat[lvl].cooldown);
		
		}
		else if(towerType==4){
			
			//EditorGUI.LabelField(new Rect(startX, startY+spaceY+10, 200, height), "Income Value: ");
			//towerList[index].upgradeStat[lvl].incomeValue = EditorGUI.IntField(new Rect(startX+lW, startY+=spaceY+10, 50, height-2), towerList[index].upgradeStat[lvl].incomeValue);
			
			EditorGUI.LabelField(new Rect(startX, startY+spaceY+10, 200, height), "Cooldown: ");
			towerList[index].upgradeStat[lvl].cooldown = EditorGUI.FloatField(new Rect(startX+lW, startY+=spaceY+10, 50, height-2), towerList[index].upgradeStat[lvl].cooldown);
		
			startY+=10;
			
			if(rscCount!=towerList[index].upgradeStat[lvl].incomes.Length){
				UpdateUpgradeStatIncomes(index, rscCount);
			}
			
			if(towerList[index].upgradeStat[lvl].incomes.Length==1){
				EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, height), "resources:");
				towerList[index].upgradeStat[lvl].incomes[0] = EditorGUI.IntField(new Rect(startX+lW, startY+=spaceY, 50, height-2), towerList[index].upgradeStat[lvl].incomes[0]);
			}
			else{
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY-5, 200, height), "Resources Per CD:");
				for(int i=0; i<towerList[index].upgradeStat[lvl].incomes.Length; i++){
					string rscName="";
					if(rscManager!=null) rscName=rscManager.resources[i].name;
					EditorGUI.LabelField(new Rect(startX, startY+spaceY-3, 200, height), " - "+rscName+": ");
					
					towerList[index].upgradeStat[lvl].incomes[i] = EditorGUI.IntField(new Rect(startX+lW, startY+=spaceY-3, 50, height-2), towerList[index].upgradeStat[lvl].incomes[i]);
				}
				startY+=8;
			}
		
		}
		
	}
	
	void UpdateIndicatorFlags(int size){
		if(indicatorFlags.Length!=size){
			indicatorFlags=new bool[size];
			for(int i=0; i<indicatorFlags.Length; i++) indicatorFlags[i]=true;
		}
	}
	
	
	void UpdateBaseStatIncomes(int id, int length){
		int[] tempIncList=towerList[index].baseStat.incomes;
		
		towerList[index].baseStat.incomes=new int[length];
		
		for(int i=0; i<length; i++){
			if(i>=tempIncList.Length){
				towerList[index].baseStat.incomes[i]=0;
			}
			else{
				towerList[index].baseStat.incomes[i]=tempIncList[i];
			}
		}
	}
	
	void UpdateUpgradeStatIncomes(int id, int length){
		for(int j=0; j<towerList[index].upgradeStat.Length; j++){
			int[] tempIncList=towerList[index].upgradeStat[j].incomes;
			
			towerList[index].upgradeStat[j].incomes=new int[length];
			
			for(int i=0; i<length; i++){
				if(i>=tempIncList.Length){
					towerList[index].upgradeStat[j].incomes[i]=0;
				}
				else{
					towerList[index].upgradeStat[j].incomes[i]=tempIncList[i];
				}
			}
		}
	}
	
	void UpdateBaseStatCost(int id, int length){
		int[] tempCostList=towerList[index].baseStat.costs;
		
		towerList[index].baseStat.costs=new int[length];
		
		for(int i=0; i<length; i++){
			if(i>=tempCostList.Length){
				towerList[index].baseStat.costs[i]=0;
			}
			else{
				towerList[index].baseStat.costs[i]=tempCostList[i];
			}
		}
	}
	
	void UpdateUpgradeStatCost(int id, int length){
		for(int j=0; j<towerList[index].upgradeStat.Length; j++){
			int[] tempCostList=towerList[index].upgradeStat[j].costs;
			
			towerList[index].upgradeStat[j].costs=new int[length];
			
			for(int i=0; i<length; i++){
				if(i>=tempCostList.Length){
					towerList[index].upgradeStat[j].costs[i]=0;
				}
				else{
					towerList[index].upgradeStat[j].costs[i]=tempCostList[i];
				}
			}
		}
	}
	

}





