using UnityEngine;
using System.Collections;



public class UIiOS : MonoBehaviour {

	public float fastForwardSpeed=3;
	
	public bool alwaysEnableNextButton=true;
	public string nextLevel="";
	public string mainMenu="";
	
	public GUIText generalUIText;
	public GUIText messageUIText;
	
	public GUIButton spawnButton;
	public GUIToggleButton ffButton;
	public GUIToggleButton pauseButton;
	public GUIButton upgradeButton;
	public GUIToggleButton sellButton;
	
	public GUITexture generalBox;
	public GUIButton menuButton;
	public GUIButton restartButton;
	public GUIButton nextLvlButton;
	
	
	
	
	private UnitTower currentSelectedTower;
	//private bool towerUIOn=false;
	
	void Awake(){
		if(!this.enabled){
			gameObject.SetActiveRecursively(false);
			gameObject.active=true;
		}	
	}
	
	
	// Use this for initialization
	void Start () {
		
		spawnButton.callBackFunc=this.OnSpawnButton;
		StartCoroutine(spawnButton.Update());
		
		ffButton.callBackFunc=this.OnFastForwardButton;
		StartCoroutine(ffButton.Update());
		
		pauseButton.callBackFunc=this.OnPauseButton;
		StartCoroutine(pauseButton.Update());
		
		upgradeButton.callBackFunc=this.OnUpgradeTowerButton;
		StartCoroutine(upgradeButton.Update());
		
		sellButton.callBackFunc=this.OnSellButton;
		StartCoroutine(sellButton.Update());
		
		GenerateButton();
		DisableSelectedTowerUI();
		
		DisableGeneralMenu();
		
		UIButtonRect=new Rect(0, Screen.height-90, 137, 90);
		
		SpawnManager.ClearForSpawningE += ClearForSpawning;
		SpawnManager.WaveStartSpawnE += UpdateGeneralUIText;
		
		GameControl.GameOverE += GameOver;
		
		GameControl.ResourceE += UpdateGeneralUIText;
		GameControl.LifeE += UpdateGeneralUIText;
		
		UnitTower.BuildCompleteE += TowerBuildComplete;
		UnitTower.DestroyE += TowerDestroy;
		
		UpdateGeneralUIText();
		
		BuildManager.InitiateSampleTower();
	}
	
	
	void UpdateGeneralUIText(int val){
		UpdateGeneralUIText();
	}
	
	void UpdateGeneralUIText(){
		string info="";
		
		info+="Life: "+GameControl.GetPlayerLife()+"\n\n";
		//info+=resourceName+": "+GameControl.GetResourceVal()+"\n\n";
		
		Resource[] resourceList=GameControl.GetResourceList();
		foreach(Resource rsc in resourceList){
			info+=rsc.name+": "+rsc.value+"\n";
		}
		info+="\n";
		
		info+="Wave: "+SpawnManager.GetCurrentWave()+"/"+SpawnManager.GetTotalWave();
		
		generalUIText.text=info;
	}
	
	void TowerBuildComplete(UnitTower tower){
		if(currentSelectedTower==tower){
			if(!currentSelectedTower.IsLevelCapped())
				upgradeButton.buttonObj.enabled=true;
			
			UpdateSelectedTowerText();
		}
	}
	
	void TowerDestroy(UnitTower tower){
		if(currentSelectedTower==tower){
			DisableSelectedTowerUI();
		}
	}
	
	void OnDisable(){
		SpawnManager.ClearForSpawningE -= ClearForSpawning;
		SpawnManager.WaveStartSpawnE -= UpdateGeneralUIText;
		
		GameControl.GameOverE -= GameOver;
		
		GameControl.ResourceE -= UpdateGeneralUIText;
		GameControl.LifeE -= UpdateGeneralUIText;
		
		UnitTower.BuildCompleteE -= TowerBuildComplete;
		UnitTower.DestroyE -= TowerDestroy;
	}
	
	//~ void GameOver(bool flag){
		//~ StartCoroutine(_GameOver(flag));
	//~ }
	void GameOver(bool flag){
	//IEnumerator _GameOver(bool flag){
		//yield return null;
		
		winLostFlag=flag;
		
		//generalUIText.anchor=TextAnchor.MiddleCenter;
		//generalUIText.alignment=TextAlignment.Center;
		//generalUIText.transform.position=new Vector3(0.5f, 0.75f, 0);
		
		generalUIText.text="";
		pauseButton.buttonObj.enabled=false;
		
		if(winLostFlag){
			GameMessage.DisplayMessage("level completed");
			messageUIText.text="level completed";
		}
		else{
			GameMessage.DisplayMessage("level failed");
			messageUIText.text="level failed";
		}
		
		EnableGeneralMenu(winLostFlag);
	}
	
	void DisableGeneralMenu(){		
		generalBox.enabled=false;
		menuButton.buttonObj.enabled=false;
		restartButton.buttonObj.enabled=false;
		nextLvlButton.buttonObj.enabled=false;
		
		generalRect=new Rect(0, 0, 0, 0);
	}
	
	void EnableGeneralMenu(bool winLostFlag){
		generalBox.enabled=true;
		menuButton.buttonObj.enabled=true;
		menuButton.callBackFunc=this.OnMenuButton;
		StartCoroutine(menuButton.Update());
		
		restartButton.buttonObj.enabled=true;
		restartButton.callBackFunc=this.OnRestartButton;
		StartCoroutine(restartButton.Update());
		
		if(winLostFlag || alwaysEnableNextButton){
			nextLvlButton.buttonObj.enabled=true;
			nextLvlButton.callBackFunc=this.OnNextLvlButton;
			StartCoroutine(nextLvlButton.Update());
		}
		
		generalRect=new Rect(Screen.width/2-70, Screen.height/2-75, 140, 150);
	}
	
	
	void DisablePauseMenu(){		
		generalBox.enabled=false;
		menuButton.buttonObj.enabled=false;
		restartButton.buttonObj.enabled=false;
		//nextLvlButton.buttonObj.enabled=false;
		
		generalRect=new Rect(0, 0, 0, 0);
	}
	
	void EnablePauseMenu(){
		generalBox.enabled=true;
		menuButton.buttonObj.enabled=true;
		menuButton.callBackFunc=this.OnMenuButton;
		StartCoroutine(menuButton.Update());
		
		restartButton.buttonObj.enabled=true;
		restartButton.callBackFunc=this.OnRestartButton;
		StartCoroutine(restartButton.Update());
		
		generalRect=new Rect(Screen.width/2-70, Screen.height/2-75, 140, 150);
	}
	
	
	void OnMenuButton(int ID){
		if(mainMenu!="") Application.LoadLevel(mainMenu);
	}
	
	void OnNextLvlButton(int ID){
		if(nextLevel!="") Application.LoadLevel(nextLevel);
	}
	
	void OnRestartButton(int ID){
		Application.LoadLevel(Application.loadedLevelName);
	}
	
	void ClearForSpawning(bool flag){
		spawnButton.buttonObj.enabled=flag;
	}
	
	private Rect UIButtonRect;
	private Rect towerUIRect;
	private Rect buildListRect;
	private Rect generalRect;
	private bool winLostFlag=false;
	
	public bool IsCursorOnUI(Vector3 point){
		Rect tempRect=new Rect(0, 0, 0, 0);
		
		tempRect=UIButtonRect;
		tempRect.y=Screen.height-tempRect.y-tempRect.height;
		if(tempRect.Contains(point)) return true;
		
		tempRect=generalRect;
		tempRect.y=Screen.height-tempRect.y-tempRect.height;
		if(tempRect.Contains(point)) return true;
		
		tempRect=buildListRect;
		tempRect.y=Screen.height-tempRect.y-tempRect.height;
		if(tempRect.Contains(point)) return true;
		
		tempRect=towerUIRect;
		tempRect.y=Screen.height-tempRect.y-tempRect.height;
		if(tempRect.Contains(point)) return true;
		
		return false;
	}
	
	// Update is called once per frame
	void Update () {
//		if(Input.touchCount>0){
//			foreach(Touch touch in Input.touches){
//				if(touch.phase==TouchPhase.Began){
//					Vector3 pos=touch.position;
//					
//					if(!IsCursorOnUI(pos)){
//						UnitTower tower=GameControl.Select(pos);
//			
//						if(tower!=null){
//							OnSelectTower(tower);
//							DisableBuildMenu();
//							BuildManager.ClearBuildPoint();
//						}
//						else{
//							
//							if(currentSelectedTower!=null) OnUnselectTower();
//							
//							if(BuildManager.CheckBuildPoint(pos)){
//								UpdateBuildList();
//								EnableBuildMenu();
//							}
//							else{
//								if(buildMenu){
//									BuildManager.ClearIndicator();
//									DisableBuildMenu();
//								}
//							}
//						}
//					}
//					
//				}
//			}	
//		}
		
		
		if(Input.GetMouseButtonDown(0) && !IsCursorOnUI(Input.mousePosition) && GameControl.gameState!=_GameState.Ended && !paused){
			
			UnitTower tower=GameControl.Select(Input.mousePosition);
			
			if(tower!=null){
				OnSelectTower(tower);
				DisableBuildMenu();
				BuildManager.ClearBuildPoint();
			}
			else{
				
				if(currentSelectedTower!=null){
					OnUnselectTower();
				}
				
				if(BuildManager.CheckBuildPoint(Input.mousePosition)){
					UpdateBuildList();
					EnableBuildMenu();
				}
				else{
					if(buildMenu){
						BuildManager.ClearIndicator();
						DisableBuildMenu();
					}
				}
			}
			
		}
		else if(Input.GetMouseButtonDown(1)){
			DisableBuildMenu();
			BuildManager.ClearBuildPoint();
			if(currentSelectedTower!=null){
				CheckForTarget();
			}
		}
		
		//~ if(currentSelectedTower==null && towerUIOn){
			//~ DisableSelectedTowerUI();
		//~ }
	}
	
	void CheckForTarget(){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		LayerMask mask=currentSelectedTower.GetTargetMask();
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, mask)){
			Unit unit=hit.collider.gameObject.GetComponent<Unit>();
			if(unit!=null){
				currentSelectedTower.AssignTarget(unit);
			}
		}
	}
	
	public GUITexture SelectedTowerUIbox;
	public GUIText SelectedTowerUIText;
	
	void OnSelectTower(UnitTower tower){
		currentSelectedTower=tower;
		EnableSelectedTowerUI();
	}
	
	void OnUnselectTower(){
		currentSelectedTower=null;
		DisableSelectedTowerUI();
		GameControl.ClearSelection();
	}
	
	void EnableSelectedTowerUI(){
		//towerUIOn=true;
		
		SelectedTowerUIbox.enabled=true;
		SelectedTowerUIText.enabled=true;
		
		if(!currentSelectedTower.IsLevelCapped())
			upgradeButton.buttonObj.enabled=true;
		
		sellButton.buttonObj.enabled=true;
		
		UpdateSelectedTowerText();
		
		towerUIRect=new Rect(Screen.width-270, Screen.height-525, 270, 525);
	}
	
	void DisableSelectedTowerUI(){
		//towerUIOn=false;
		
		SelectedTowerUIbox.enabled=false;
		SelectedTowerUIText.enabled=false;
		
		upgradeButton.buttonObj.enabled=false;
		sellButton.buttonObj.enabled=false;
		
		towerUIRect=new Rect(0, 0, 0, 0);
	}
	
	void UpdateSelectedTowerText(){
		if(currentSelectedTower!=null){
			string towerInfo="";
			
			towerInfo+=currentSelectedTower.unitName+"\n";
			
			int lvl=currentSelectedTower.GetLevel();
			towerInfo+="Level: "+lvl.ToString()+"\n\n\n";
			
			
			
			_TowerType type=currentSelectedTower.type;
			
			if(type==_TowerType.ResourceTower){
				
				string val=currentSelectedTower.GetDamage().ToString();
				string cd=currentSelectedTower.GetCooldown().ToString("f1");
				
				string rsc="Increase rsc by "+val+" for every cd "+cd+"sec\n";
				rsc=FormatString(rsc);
				
				towerInfo+=rsc;
				
			}
			else if(type==_TowerType.SupportTower){
				
				BuffStat buffInfo=currentSelectedTower.GetBuff();
				
				string buff="";
				buff+="Buff damage by "+(buffInfo.damageBuff*100).ToString("f1")+"%\n";
				buff+="Buff range by "+(buffInfo.rangeBuff*100).ToString("f1")+"%\n";
				buff+="Reduce CD by "+(buffInfo.cooldownBuff*100).ToString("f1")+"%\n";
				
				towerInfo+=buff;
				
			}
			else if(type==_TowerType.TurretTower || type==_TowerType.AOETower){
				
				if(currentSelectedTower.GetDamage()>0)
					towerInfo+="Damage: "+currentSelectedTower.GetDamage().ToString("f1")+"\n";
				if(currentSelectedTower.GetCooldown()>0)
					towerInfo+="Cooldown: "+currentSelectedTower.GetCooldown().ToString("f1")+"sec\n";
				if(type==_TowerType.TurretTower && currentSelectedTower.GetAoeRadius()>0)
					towerInfo+="AOE Radius: "+currentSelectedTower.GetAoeRadius().ToString("f1")+"\n";
				if(currentSelectedTower.GetStunDuration()>0)
					towerInfo+="Stun target for "+currentSelectedTower.GetStunDuration().ToString("f1")+"sec\n";
				
				Dot dot=currentSelectedTower.GetDot();
				float totalDot=dot.damage*(dot.duration/dot.interval);
				if(totalDot>0){
					string dotInfo="Cause "+totalDot.ToString("f1")+" damage over the next "+dot.duration+" sec\n";
					dotInfo=FormatString(dotInfo);
					
					towerInfo+=dotInfo;
				}
				
				Slow slow=currentSelectedTower.GetSlow();
				if(slow.duration>0){
					string slowInfo="Slow target by "+(slow.slowFactor*100).ToString("f1")+"% for "+slow.duration.ToString("f1")+"sec\n";
					slowInfo=FormatString(slowInfo);
					
					towerInfo+=slowInfo;
				}
			}
			
			string desp=FormatString("\n"+currentSelectedTower.GetDescription());
			
			towerInfo+=desp;
			
			
			towerInfo+="\n\n\n";
			Resource[] rscList=GameControl.GetResourceList();
			int count=0;
			
			if(!currentSelectedTower.IsLevelCapped() && currentSelectedTower.IsBuilt()){
				//~ int cost=currentSelectedTower.GetUpgradeCost();
				//~ towerInfo+="UpgradeCost: "+cost.ToString()+resourceName+"\n";
				
				int[] cost=currentSelectedTower.GetCost();
				
				//GUI.Label(new Rect(startX, startY, 150, 25), "Upgrade Cost:");
				towerInfo+="Upgrade Cost:\n ";
				
				
				for(int i=0; i<cost.Length; i++){
					if(cost[i]>0){
						count+=1;
						//if(rscList[i].icon!=null){
						//	GUI.Label(new Rect(startX+10, startY+count*20, 25, 25), rscList[i].icon);
						//	GUI.Label(new Rect(startX+10+25, startY+count*20+3, 150, 25), "- "+cost[i].ToString());
						//}
						//else{
							towerInfo+="- "+cost[i].ToString()+rscList[i].name+"  ";
						//}
					}
				}
				
				towerInfo+="\n\n";
			}
			
			int[] sellValue=currentSelectedTower.GetTowerSellValue();
				
			//GUI.Label(new Rect(startX, startY, 150, 25), "Upgrade Cost:");
			towerInfo+="Sell Cost:\n ";
			
			count=0;
			for(int i=0; i<sellValue.Length; i++){
				if(sellValue[i]>0){
					count+=1;
					//if(rscList[i].icon!=null){
					//	GUI.Label(new Rect(startX+10, startY+count*20, 25, 25), rscList[i].icon);
					//	GUI.Label(new Rect(startX+10+25, startY+count*20+3, 150, 25), "- "+cost[i].ToString());
					//}
					//else{
						towerInfo+="+ "+sellValue[i].ToString()+rscList[i].name+"  ";
					//}
				}
			}
			//towerInfo+="SellValue: "+sellValue.ToString()+"\n\n";
			
			
			SelectedTowerUIText.text=towerInfo;
		}
	}
	
	public string FormatString(string s) {
		char[] delimitor = new char[1] {' '};
		string[] words = s.Split(delimitor); //Split the string into seperate words
		string result = "";
		int runningLength = 0;
		foreach (string word in words) {
			if (runningLength + word.Length+1 <= 32) {
				result += " " + word;
				runningLength += word.Length+1;
			}
			else {
				result += "\n" + word;
				runningLength = word.Length;
			}
		}
		   
		return result.Remove(0,1); //Remove the first space
    }
	
	void OnPressed(int id){
		Debug.Log("buttonPressed");
	}
	
	void OnSellButton(int id){
		if(currentSelectedTower!=null){
			if(!sellButton.isPressed) {
				
				currentSelectedTower.Sell();
				
				sellButton.buttonObj.enabled=false;
			}
		}
	}
	
	
	void OnSpawnButton(int id){
		if(GameControl.gameState!=_GameState.Ended){
			//if(SpawnManager.Spawn()) spawnButton.buttonObj.enabled=false;
			SpawnManager.Spawn();
		}
	}

	
	void OnUpgradeTowerButton(int ID){
		if(currentSelectedTower!=null){
			//Debug.Log("upgrade tower");
			upgradeButton.buttonObj.enabled=false;
			currentSelectedTower.Upgrade();
			UpdateSelectedTowerText();
		}
	}
	
	void OnBuildButton(int ID){
		UnitTower[] towerList=BuildManager.GetTowerList();
		
		UnitTower tower=towerList[ID];
		
		//Debug.Log("Build tower id "+ID);
		
		if(BuildManager.BuildTowerPointNBuild(tower)){
			DisableBuildMenu();
			BuildManager.ClearSampleTower();
		}
	}
	
	void OnShowSampleTower(int ID, bool flag){
		if(flag) BuildManager.ShowSampleTower(ID); 
		else BuildManager.ClearSampleTower();
	}
	
	void OnFastForwardButton(int ID){
		if(Time.timeScale==1) Time.timeScale=fastForwardSpeed;
		else Time.timeScale=1;
	}
	
	private bool paused=false;
	void OnPauseButton(int ID){
		TogglePaused();
	}
	
	void TogglePaused(){
		paused=!paused;
		if(paused){
			Time.timeScale=0;
			messageUIText.text="Game Paused";
			spawnButton.buttonObj.enabled=false;
			ffButton.buttonObj.enabled=false;
			
			if(currentSelectedTower!=null){
				OnUnselectTower();
			}
			if(buildMenu){
				BuildManager.ClearIndicator();
				DisableBuildMenu();
			}
			
			EnablePauseMenu();
		}
		else{
			Time.timeScale=1;
			messageUIText.text="";
			spawnButton.buttonObj.enabled=true;
			ffButton.buttonObj.enabled=true;
			
			DisablePauseMenu();
		}
	}
	
	private GUIButton[] buildButtonList=new GUIButton[0];
	private int[] currentBuildList=new int[0];
	
	private bool buildMenu=false;
	
	float buildButtonSize=60;
	
	//called whevenever the build list is called up
	//compute the number of tower that can be build in this build pointer
	//store the tower that can be build in an array of number that reference to the towerlist
	//this is so these dont need to be calculated in every frame in OnGUI()
	private void UpdateBuildList(){
		//get the current buildinfo in buildmanager
		BuildableInfo currentBuildInfo=BuildManager.GetBuildInfo();
		
		//get the current tower list in buildmanager
		UnitTower[] towerList=BuildManager.GetTowerList();
		
		//construct a temporary interger array the length of the buildinfo
		int[] tempBuildList=new int[towerList.Length];
		//for(int i=0; i<currentBuildList.Length; i++) tempBuildList[i]=-1;
		
		//scan through the towerlist, if the tower matched the build type, 
		//put the tower ID in the towerlist into the interger array
		int count=0;	//a number to record how many towers that can be build
		for(int i=0; i<towerList.Length; i++){
			UnitTower tower=towerList[i];
				
			//check if this type of tower can be build on this platform
			foreach(_TowerType type in currentBuildInfo.buildableType){
				if(tower.type==type){
					tempBuildList[count]=i;
					count+=1;
					break;
				}
			}

		}
		
		//for as long as the number that can be build, copy from the temp buildList to the real buildList
		currentBuildList=new int[count];
		for(int i=0; i<currentBuildList.Length; i++) currentBuildList[i]=tempBuildList[i];
	}
	
	private void EnableBuildMenu(){
		if(buildMenu) DisableBuildMenu();
		
		buildMenu=true;
		UpdateBuildList();
		
		int count=0;
		foreach(int i in currentBuildList){
			buildButtonList[i].buttonObj.enabled=true;
			
			Vector3 pos=new Vector3((140f+count*(buildButtonSize+7))/Screen.width, 5f/Screen.height, 1);
			buildButtonList[i].buttonObj.transform.position=pos;
			
			count+=1;
		}
		
		buildListRect=new Rect(137, Screen.height-buildButtonSize-12, count*(buildButtonSize+7), buildButtonSize+12);
	}
	
	private void DisableBuildMenu(){
		foreach(GUIButton button in buildButtonList){
			button.buttonObj.enabled=false;
		}
		
		buildListRect=new Rect(0, 0, 0, 0);
		
		buildMenu=false;
		
	}
	
	void GenerateButton(){
		UnitTower[] towerList=BuildManager.GetTowerList();
		
		buildButtonList=new GUIButton[towerList.Length];
		
		for(int i=0; i<towerList.Length; i++){
			UnitTower tower=towerList[i];
			
			buildButtonList[i]=new GUIButton(tower.icon, null, OnBuildButton, OnShowSampleTower, i);
			
			buildButtonList[i].buttonObj.pixelInset=new Rect(0, 0, buildButtonSize, buildButtonSize);
			
			//Vector3 pos=new Vector3((140f+i*(size+7))/Screen.width, 5f/Screen.height, 1);
			//buildButtonList[i].buttonObj.transform.position=pos;
			buildButtonList[i].buttonObj.transform.localScale=new Vector3(0, 0, 1);
			
			StartCoroutine(buildButtonList[i].Update());
		}
		
		DisableBuildMenu();
	}
	
}




