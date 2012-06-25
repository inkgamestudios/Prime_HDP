using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;


public class SpawnEditor : EditorWindow {
	
	static int waveLength=0;
	static int[] subWaveLength=new int[0];
	
	static int spawnType=0;
	static string[] spawnTypeLabel=new string[5];
	
	static bool[] waveFoldList=new bool[0];
	
	static private SpawnEditor window;

	// Add menu named "TowerEditor" to the Window menu
    [MenuItem ("TDTK/SpawnEditor")]
    static void Init(){
        // Get existing open window or if none, make a new one:
        window = (SpawnEditor)EditorWindow.GetWindow(typeof (SpawnEditor));
		window.minSize=new Vector2(620, 620);
		
		spawnTypeLabel[0]="Continous";
		spawnTypeLabel[1]="WaveCleared";
		spawnTypeLabel[2]="RoundBased";
		spawnTypeLabel[3]="SkippableContinous";
		spawnTypeLabel[4]="SkippableWaveCleared";
		
		GetSpawnManager();
    }
    
    static SpawnManager spawnManager;
	static ResourceManager rscManager;
    
	static void GetSpawnManager(){
		spawnManager=(SpawnManager)FindObjectOfType(typeof(SpawnManager));

		if(spawnManager!=null){
			if(spawnManager.spawnMode==_SpawnMode.Continous) spawnType=0;
			else if(spawnManager.spawnMode==_SpawnMode.WaveCleared) spawnType=1;
			else if(spawnManager.spawnMode==_SpawnMode.WaveCleared) spawnType=2;
			else if(spawnManager.spawnMode==_SpawnMode.WaveCleared) spawnType=3;
			else if(spawnManager.spawnMode==_SpawnMode.WaveCleared) spawnType=4;
		
			waveLength=spawnManager.waves.Length;
			//UpdateWaveLength();
			
			waveFoldList=new bool[waveLength];
			for(int i=0; i<waveFoldList.Length; i++){
				waveFoldList[i]=true;
			}
			
			subWaveLength=new int[waveLength];
			for(int i=0; i<waveLength; i++){
				Wave wave=spawnManager.waves[i];
				//Debug.Log(wave.subWaves);
				subWaveLength[i]=wave.subWaves.Length;
			}
		}
		
		rscManager=(ResourceManager)FindObjectOfType(typeof(ResourceManager));
		
	}
	
	private Vector2 scrollPos;
    
    void OnGUI(){
    	
    	float startX=3;
		float startY=3;
		
		float height=18;
		float spaceY=height+startX;
    	
    	if(spawnManager==null){
    		if(GUI.Button(new Rect(startX, startY, 100, height), "Update")) GetSpawnManager();
    	}
    	else{
	    	if(GUI.Button(new Rect(Mathf.Max(startX+500, window.position.width-120), startY, 100, height), "Update")) GetSpawnManager();
	    	
	    	spawnType = EditorGUI.Popup(new Rect(startX, startY, 300, 15), "Spawn Mode:", spawnType, spawnTypeLabel);
	        if(spawnType==0) spawnManager.spawnMode=_SpawnMode.Continous;
	        else if(spawnType==1) spawnManager.spawnMode=_SpawnMode.WaveCleared;
	        else if(spawnType==2) spawnManager.spawnMode=_SpawnMode.RoundBased;
	        else if(spawnType==3) spawnManager.spawnMode=_SpawnMode.SkippableContinous;
	        else if(spawnType==4) spawnManager.spawnMode=_SpawnMode.SkippableWaveCleared;
	    	
	    	spawnManager.defaultPath=(Path)EditorGUI.ObjectField(new Rect(startX, startY+=spaceY, 300, 15), "Default Path: ", spawnManager.defaultPath, typeof(Path), true);
        	
        	waveLength=EditorGUI.IntField(new Rect(startX, startY+=spaceY, 300, 15), "Wave Size:", waveLength);
			if(waveLength!=spawnManager.waves.Length){
        		UpdateWaveLength();
        	}
        	
			if(rscManager!=null) rscCount=rscManager.resources.Length;
			else rscCount=1;

			int maxSubWave=GetMaximumSubWaveCount();
			//GUI.BeginGroup(new Rect(startX, startY+=spaceY, 300, 15));
			scrollPos = GUI.BeginScrollView(new Rect(startX, startY+=spaceY+2, window.position.width-25, window.position.height-startY), scrollPos, new Rect(0, 0, maxSubWave*200, waveLength*260+waveLength*rscCount*20));
			//~ scrollPos=EditorGUILayout.BeginScrollView(scrollPos, true, true, GUILayout.Width (500), GUILayout.Height (600));
			
			startY=-12;
			
			
			
        	for(int i=0; i<spawnManager.waves.Length; i++){
        		waveFoldList[i] = EditorGUI.Foldout(new Rect(startX, startY+=spaceY, 60, 15), waveFoldList[i], "wave "+(i+1).ToString());
				
        		if(waveFoldList[i]){
					
					if(spawnManager.waves[i].subWaves==null){ 
						spawnManager.waves[i].subWaves=new SubWave[1];
						spawnManager.waves[i].subWaves[0]=new SubWave();
					}
					
					GUI.Box(new Rect(startX, startY+spaceY-1, Mathf.Max(310, spawnManager.waves[i].subWaves.Length*199+3), 210+(rscCount*21)), "");
	        		
	        		startX+=3; startY+=3;
	        		
	        		subWaveLength[i]=EditorGUI.IntField(new Rect(startX, startY+=spaceY, 300, 15), "Number of SubWave:", subWaveLength[i]);
			        if(subWaveLength[i]==0) subWaveLength[i]=1;
			        if(subWaveLength[i]!=spawnManager.waves[i].subWaves.Length){
			        	UpdateUnit(i, subWaveLength[i]);
			        }
        			
	        		float startYTab=startY;
					
	        		for(int j=0; j<spawnManager.waves[i].subWaves.Length; j++){
	        			SubWave subWave=spawnManager.waves[i].subWaves[j];
	        			
	        			GUI.Box(new Rect(startX, startY+spaceY-1, 188, 148), "");
	        			startX+=4; startY+=4;
	        			
						EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, 15), "Unit Prefab:");
	        			subWave.unit=(GameObject)EditorGUI.ObjectField(new Rect(startX+100, startY+=spaceY, 80, 15), subWave.unit, typeof(GameObject), false);
	        			
	        			EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, 15), "Number of Unit:");
						subWave.count=EditorGUI.IntField(new Rect(startX+100, startY+=spaceY, 80, 15), subWave.count);
        				
        				EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, 15), "Spawn Interval:");
						subWave.interval=EditorGUI.FloatField(new Rect(startX+100, startY+=spaceY, 80, 15), subWave.interval);
        				
        				EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, 15), "Pre-Spawn Delay:");
						subWave.delay=EditorGUI.FloatField(new Rect(startX+100, startY+=spaceY, 80, 15), subWave.delay);
        				
        				EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, 15), "Alternate Path:");
						subWave.path=(Path)EditorGUI.ObjectField(new Rect(startX+100, startY+=spaceY, 80, 15), subWave.path, typeof(Path), true);
	        			
						EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, 15), "OverrideHP:");
						subWave.overrideHP=EditorGUI.FloatField(new Rect(startX+100, startY+=spaceY, 80, 15), subWave.overrideHP);
        				
						EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, 15), "OverrideSpeed:");
						subWave.overrideMoveSpd=EditorGUI.FloatField(new Rect(startX+100, startY+=spaceY, 80, 15), subWave.overrideMoveSpd);
        				
						
	        			startX+=195;
	        			if(j<spawnManager.waves[i].subWaves.Length-1) startY=startYTab;
	        			else startY+=5;
					}
					
					startX=6;
	        		
	        		//spawnManager.waves[i].waveInterval=EditorGUI.FloatField(new Rect(startX, startY+=spaceY, 300, 15), "Duration before next Wave:", spawnManager.waves[i].waveInterval);
	        		//spawnManager.waves[i].resource=EditorGUI.IntField(new Rect(startX, startY+=spaceY, 300, 15), "Resource Upon Wave Clear:", spawnManager.waves[i].resource);
					
					EditorGUI.LabelField(new Rect(startX, startY+spaceY, 200, 15), "Time before next wave:");
	        		spawnManager.waves[i].waveInterval=EditorGUI.FloatField(new Rect(startX+150, startY+=spaceY, 80, 15), spawnManager.waves[i].waveInterval);
	        		EditorGUI.LabelField(new Rect(startX+250, startY, 200, 15), CalculateWaveSpawnDuration(spawnManager.waves[i].subWaves).ToString());
        			
					if(rscCount!=spawnManager.waves[i].resourceGain.Length){
						UpdateResourceGain(i, rscCount);
					}
					
					if(spawnManager.waves[i].resourceGain.Length==1){
						spawnManager.waves[i].resourceGain[0]=EditorGUI.IntField(new Rect(startX, startY+=spaceY, 300, 15), "Resource Upon Wave Clear:", spawnManager.waves[i].resourceGain[0]);
					}
					else{
						EditorGUI.LabelField(new Rect(startX, startY+=spaceY-5, 200, height), "Resource Upon Wave Clear: ");
						for(int j=0; j<spawnManager.waves[i].resourceGain.Length; j++){
							string rscName="";
							if(rscManager!=null) rscName=rscManager.resources[j].name;
							EditorGUI.LabelField(new Rect(startX, startY+spaceY-3, 200, height), " - "+rscName+": ");
							
							spawnManager.waves[i].resourceGain[j] = EditorGUI.IntField(new Rect(startX+150, startY+=spaceY-3, 50, height-2), spawnManager.waves[i].resourceGain[j]);
						}
						//startY+=8;
					}
					
        			startY+=15;
        		}
        		
        		startX=3;
        	}
			
			
			GUI.EndScrollView();
			//EditorGUILayout.EndScrollView();
			//GUI.EndGroup();
			
    	}
    }
	
	private int rscCount=1;
	void UpdateResourceGain(int index, int length){
		int[] tempList=spawnManager.waves[index].resourceGain;
		
		spawnManager.waves[index].resourceGain=new int[length];
		for(int i=0; i<spawnManager.waves[index].resourceGain.Length; i++){
			if(i>=tempList.Length){
				spawnManager.waves[index].resourceGain[i]=0;
			}
			else{
				spawnManager.waves[index].resourceGain[i]=tempList[i];
			}
		}
	}
    
    void UpdateWaveLength(){
    	if(waveLength!=spawnManager.waves.Length){
    		if(waveLength>spawnManager.waves.Length){
    			
    			Wave[] tempWaveList=new Wave[waveLength];
    			bool[] tempwaveFoldList=new bool[waveLength];
    			
    			for(int i=0; i<tempWaveList.Length; i++){
    				if(i<spawnManager.waves.Length){
    					tempWaveList[i]=spawnManager.waves[i];
    					tempwaveFoldList[i]=waveFoldList[i];
    				}
    				else{
    					tempWaveList[i]=CopyWaveInfo(spawnManager.waves[spawnManager.waves.Length-1]);
    					tempwaveFoldList[i]=false;
    				}
    			}
    			
    			spawnManager.waves=tempWaveList;
    			waveFoldList=tempwaveFoldList;
    			
    		}
    		else{
    			
    			Wave[] tempWaveList=new Wave[waveLength];
    			bool[] tempwaveFoldList=new bool[waveLength];
    			
    			for(int i=0; i<tempWaveList.Length; i++){
    				tempWaveList[i]=spawnManager.waves[i];
    				tempwaveFoldList[i]=waveFoldList[i];
    			}
    			
    			spawnManager.waves=tempWaveList;
    			waveFoldList=tempwaveFoldList;
    			
    		}
    	}
		
		subWaveLength=new int[waveLength];
		for(int i=0; i<waveLength; i++){
			subWaveLength[i]=spawnManager.waves[i].subWaves.Length;
		}
		//Debug.Log(subWaveLength.Length);
    }
    
    Wave CopyWaveInfo(Wave srcWave){
    	Wave tempWave=new Wave();
    	
    	tempWave.subWaves=new SubWave[srcWave.subWaves.Length];
    	for(int i=0; i<tempWave.subWaves.Length; i++){
    		tempWave.subWaves[i]=CopySubWaveInfo(srcWave.subWaves[i]);
    	}
    	
    	tempWave.waveInterval=srcWave.waveInterval;
    	tempWave.resource=srcWave.resource;
    	
    	return tempWave;
    }
    
    SubWave CopySubWaveInfo(SubWave srcSubWave){
    	SubWave tempSubWave=new SubWave();
    	
    	tempSubWave.unit=srcSubWave.unit;
    	tempSubWave.count=srcSubWave.count;
    	tempSubWave.interval=srcSubWave.interval;
    	tempSubWave.delay=srcSubWave.delay;
    	tempSubWave.path=srcSubWave.path;
    	
    	return tempSubWave;
    }
    
    void UpdateUnit(int ID, int length){
    	Wave wave=spawnManager.waves[ID];
    	
    	if(length!=wave.subWaves.Length){
    		if(length>wave.subWaves.Length){
    			SubWave[] tempSubWaveList=new SubWave[length];
    			
    			for(int i=0; i<tempSubWaveList.Length; i++){
    				
    				if(i<wave.subWaves.Length){
    					tempSubWaveList[i]=wave.subWaves[i];
    				}
    				else{
    					if(wave.subWaves.Length!=0)
    						tempSubWaveList[i]=CopySubWaveInfo(wave.subWaves[wave.subWaves.Length-1]);
    					else tempSubWaveList[i]=new SubWave();
    				}
    			}
    			
    			spawnManager.waves[ID].subWaves=tempSubWaveList;
    		}
    		else{
    			SubWave[] tempSubWaveList=new SubWave[length];
    			//bool[] tempwaveFoldList=new bool[waveLength];
    			
    			for(int i=0; i<tempSubWaveList.Length; i++){
    				tempSubWaveList[i]=wave.subWaves[i];
    				//tempwaveFoldList[i]=waveFoldList[i];
    			}
    			
    			spawnManager.waves[ID].subWaves=tempSubWaveList;
    			//waveFoldList=tempwaveFoldList;
    		}
    	}
		
		
    }
	
	int GetMaximumSubWaveCount(){
		int count=0;
		for(int i=0; i<subWaveLength.Length; i++){
			if(subWaveLength[i]>count){
				count=subWaveLength[i];
			}
		}
		return count;
	}
	
	float CalculateWaveSpawnDuration(SubWave[] subWaveList){
		float duration=0;
		foreach(SubWave subWave in subWaveList){
			float tempDuration=subWave.count*subWave.interval+subWave.delay;
			if(tempDuration>duration){
				duration=tempDuration;
			}
		}
		return duration;
	}
    
}

