using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

public class GameUI : MonoBehaviour {
	
	public enum _BuildMenuType{Fixed}
	public _BuildMenuType buildMenuType;
	
	public enum _BuildMode{PointNBuild}
	public _BuildMode buildMode;
	
	public bool showBuildSample=true;
	
	public int fastForwardSpeed=3;
	
	public bool enableTargetPrioritySwitch=true;
	public bool enableTargetDirectionSwitch=true;
	
	public bool alwaysEnableNextButton=true;
	
	public string nextLevel="";
	public string mainMenu="";

	private bool enableSpawnButton=true;
	
	private bool buildMenu=false;
	
	private UnitTower currentSelectedTower;
	
	private int[] currentBuildList=new int[0];
	
	//indicate if the player have won or lost the game
	private bool winLostFlag=false;
	
	private bool paused=false;
	private float timeHolder = 1;
	
	private bool buildMenuActive = false;
	
	private UIButton cog;
	private UIButton playButton;
	private UIButton sell;
	private UIButton pause;
	private UIButton upgrade;
	private UIButton fastForward;
	private UIButton box;
	
	private UIButton sound;
	private UIButton quit;
	private UIButton replay;
	
	private UIButton energyIcon;
	private UIButton powerIcon;
	
	private UIVerticalLayout vertBox;
	private UIVerticalLayout abilityBox;
	private UIHorizontalLayout towerBox;

	private UIButton tower1;
	private UIButton tower2;
	private UIButton tower3;
	private UIButton tower4;
	private UIButton tower5;
	private UIButton tower6;
	private UIButton tower7;
	private UIButton tower8;
	private UIButton tower9;
	
	private UIButton ability1;
	private UIButton ability2;
	private UIButton ability3;
	
	private UIText text;
	private UITextInstance waveInfo;
	private UITextInstance lifeInfo;
	private UITextInstance energyInfo;
	private UITextInstance ppInfo;
	
//	private Rect topPanelRect=new Rect(-3, -3, Screen.width+6, 28);
//	private Rect bottomPanelRect;
//	private Rect buildListRect;
//	private Rect towerUIRect;
//	private Rect[] scatteredRectList=new Rect[0];
	
	private static GameUI gameui;
	void Awake(){
		gameui=this;
	}
	
	private string gameMessage="";
	private float lastMsgTime=0;
	public static void ShowMessage(string msg){
		gameui.gameMessage=gameui.gameMessage+msg+"\n";
		gameui.lastMsgTime=Time.time;
	}
	IEnumerator MessageCountDown(){
		while(true){
			if(gameMessage!=""){
				while(Time.time-lastMsgTime<3){
					yield return null;
				}
				gameMessage="";
			}
			yield return null;
		}
	}
	
	// Use this for initialization
	void Start () {	
		
		DrawGUI();
		text = new UIText( "neuropol", "neuropol.png" );
		TextInfo();
		
		//initiate sample menu, so player can preview the tower in pointNBuild buildphase
		if(buildMode==_BuildMode.PointNBuild && showBuildSample) BuildManager.InitiateSampleTower();
		StartCoroutine(MessageCountDown());
		
	}

	void OnEnable(){
		SpawnManager.onClearForSpawningE += OnClearForSpawning;
		GameControl.onGameOverE += OnGameOver;
		UnitTower.onDestroyE += OnTowerDestroy;
	}
	
	void OnDisable(){
		SpawnManager.onClearForSpawningE -= OnClearForSpawning;
		GameControl.onGameOverE -= OnGameOver;
		UnitTower.onDestroyE -= OnTowerDestroy;
	}
	
	//called when game is over, flag passed indicate user win or lost
	void OnGameOver(bool flag){
		winLostFlag=flag;
	}
	
	//called if a tower is unbuilt or destroyed, check to see if that tower is selected, if so, clear the selection
	void OnTowerDestroy(UnitTower tower){
		if(currentSelectedTower==tower) currentSelectedTower=null;
	}
	
	//caleed when SpawnManager clearFor Spawing event is detected, enable spawnButton
	void OnClearForSpawning(bool flag){
		enableSpawnButton=flag;
	}
	
	//call to enable/disable pause
	void TogglePause()
	{
		paused=!paused;
		if(paused){
			timeHolder = Time.timeScale;
			Time.timeScale=0;
			cogCommands();
			swapButtons( playButton, pause );
			
			//close all the button, so user can interact with game when it's paused
			if(currentSelectedTower!=null){
				GameControl.ClearSelection();
				currentSelectedTower=null;
			}
			if(buildMenu){
				buildMenu=false;
				BuildManager.ClearBuildPoint();
			}
		}
		else 
		{
			cogCommands();
			swapButtons( playButton, pause );
			Time.timeScale=timeHolder;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		#if !UNITY_IPHONE && !UNITY_ANDROID
			if(buildMode==_BuildMode.PointNBuild)
				BuildManager.SetIndicator(Input.mousePosition);
		#endif

		if(Input.GetMouseButtonUp(0) && !paused){
			
			//check if user click on towers
			UnitTower tower=GameControl.Select(Input.mousePosition);
			
			//if user click on tower, select the tower
			if(tower!=null){
				currentSelectedTower=tower;
				
				//if build mode is active, disable buildmode
				if(buildMenu){
					buildMenu=false;
					BuildManager.ClearBuildPoint();
//					ClearBuildListRect();
					towerBox.position = new Vector2( 0, Screen.height );
				}
				if(currentSelectedTower != null)
				{
					SelectedTowerUI();
				}
			}
			//no tower is selected
			else{
				//if a tower is selected previously, clear the selection
				if(currentSelectedTower!=null){
					GameControl.ClearSelection();
					currentSelectedTower=null;
				}
				
				//if we are in PointNBuild Mode
				if(buildMode==_BuildMode.PointNBuild){
					//check for build point, if true initiate build menu
					if(BuildManager.CheckBuildPoint(Input.mousePosition) && !buildMenuActive){
							UpdateBuildList();
							buildMenu=true;
							buildMenuActive = true;
							BuildMenuFix();
					}
					//if the build menu is active and your clicking on a button
					else if(buildMenuActive)
					{
						buildMenu=false;
						buildMenuActive = false;
						BuildManager.ClearBuildPoint();
//						ClearBuildListRect();
						towerBox.position = new Vector2( 0, Screen.height );
					}
					//if there are no valid build point but we are in build mode, disable it
					else{
						if(buildMenu){
							buildMenu=false;
							buildMenuActive = false;
							BuildManager.ClearBuildPoint();
//							ClearBuildListRect();
							towerBox.position = new Vector2( 0, Screen.height );
						}
					}
				}
			}
			
		}
		//if right click, 
		else if(Input.GetMouseButtonUp(1)){
			//clear the menu
			if(buildMenu){
				buildMenu=false;
				BuildManager.ClearBuildPoint();
//				ClearBuildListRect();
				towerBox.position = new Vector2( 0, Screen.height );
			}
			
			//if there are tower currently being selected
			if(currentSelectedTower!=null){
				CheckForTarget();
			}
		}
		
		//if escape key is pressed, toggle pause
		if(Input.GetKeyUp(KeyCode.Escape)){
			TogglePause();
		}
		
		if(GameControl.gameState == _GameState.Ended)
		{
			GameOverScreen();
		}

		TextDestroy();
		TextInfo();
	}

	//check if user has click on creep, if yes and if current tower is eligible to attack it, set the assign it as tower target
	void CheckForTarget(){
		currentSelectedTower.CheckForTarget(Input.mousePosition);
	}
	
//	//clear all ui space occupied by build menu
//	void ClearBuildListRect(){
//		if(buildMode==_BuildMode.PointNBuild){
//			buildListRect=new Rect(0, 0, 0, 0);
//		}
//	}
	
	// simple gamespeed method
	void GameSpeed(int speed){
		Time.timeScale = speed;
	}
	
// used to swap the play and pause button positions
	void swapButtons( UIButton orig, UIButton swaped )
	{
		Vector3 holder;
		holder = orig.position;
		orig.position = swaped.position;
		swaped.position = holder;
	}
	
	void playButtonCommands()
	{
		if(enableSpawnButton)
		{
			//if the game is not ended
			if(GameControl.gameState!=_GameState.Ended)
			{
				//if spawn is successful, disable the spawnButton
				SpawnManager.Spawn();
				Debug.Log( "play button pushed" );
				swapButtons( playButton, pause );
				enableSpawnButton=false;
			}
		}
		else if(!enableSpawnButton)
		{
			if(GameControl.gameState!=_GameState.Ended)
			{
				TogglePause();
			}
		}
	}
	
	void FFCommands()
	{
		if(!paused)
		{
			if(Time.timeScale == 1)
			{
				// need a fastforward up and down phase
				GameSpeed(fastForwardSpeed);
				Debug.Log ("timescale now increased");
			}
			else if(Time.timeScale == fastForwardSpeed)
			{
				GameSpeed( 1 );
				Debug.Log ("timescale now 1");
			}
		}
	}
	
	void cogCommands()
	{
		if(paused)
		{
			vertBox.position = new Vector2( Screen.width/2- sound.width/2, - Screen.height/2 +sound.height*2 + 20 );
			box.position = new Vector3( Screen.width/2- box.width/2, -Screen.height/2 + box.height/2, 3 );
		}
		else if(!paused)
		{
			vertBox.positionFromTop( Screen.height );
			box.position = new Vector3( 0, -Screen.height, 3);
		}
	}
	
	void soundCommands()
	{ Debug.Log("sound button clicked"); }
	
	void quitCommands()
	{ Debug.Log("quit button clicked"); }
	
	void GameOverScreen()
	{
		// not sure what we are doing for the game over screen yet so this for now... *****
		
		string levelCompleteString="Level Complete";
		if(!winLostFlag) levelCompleteString="Level Lost";
	}
	
	void TextInfo()
	{
		waveInfo = text.addTextInstance( "Wave: " +SpawnManager.GetCurrentWave() +"/" +SpawnManager.GetTotalWave(), Screen.width/2 - 50, 5 );
		lifeInfo = text.addTextInstance( "Life: " +GameControl.GetPlayerLife(), Screen.width - 100, 5 );
		energyInfo = text.addTextInstance( "" +ResourceManager.GetResourceVal(0), Screen.width - 50, Screen.height - 45 );
		ppInfo = text.addTextInstance( "" +ResourceManager.GetResourceVal(1), Screen.width - 50, Screen.height - 30 );
	}
	
	void TextDestroy()
	{
		waveInfo.destroy();
		lifeInfo.destroy();
		energyInfo.destroy();
		ppInfo.destroy();
	}
	
	//draw GUI 
	void DrawGUI()
	{		
		playButton = UIButton.create( "Play.png", "Play.png", 3, 3 );
		pause = UIButton.create( "Pause.png", "Pause.png", 0, -Screen.height );
		fastForward = UIButton.create( "FastForward.png", "FastForward.png", (int)playButton.width + 5, 3 );
		
		cog = UIButton.create( "Cog.png", "Cog.png", 0, 0 );
		box = UIButton.create( "PanelsCenter2.png", "PanelsCenter2.png", 0, 0 );
		
		energyIcon = UIButton.create( "EnergyIcon.png", "EnergyIcon.png", Screen.width - 100, Screen.height - 45 );
		powerIcon = UIButton.create( "PowerIcon.png", "PowerIcon.png", Screen.width - 100, Screen.height - 30 );
		
		tower1 = UIButton.create( "FillerButton1.png", "FillerButton1.png", 0, Screen.height );
		tower2 = UIButton.create( "FillerButton2.png", "FillerButton2.png", 0, Screen.height );
		tower3 = UIButton.create( "FillerButton3.png", "FillerButton3.png", 0, Screen.height );
		tower4 = UIButton.create( "FillerButton4.png", "FillerButton4.png", 0, Screen.height );
		tower5 = UIButton.create( "FillerButton5.png", "FillerButton5.png", 0, Screen.height );
		tower6 = UIButton.create( "FillerButton6.png", "FillerButton6.png", 0, Screen.height );
		tower7 = UIButton.create( "FillerButton7.png", "FillerButton7.png", 0, Screen.height );
		tower8 = UIButton.create( "FillerButton8.png", "FillerButton8.png", 0, Screen.height );
		tower9 = UIButton.create( "FillerButton9.png", "FillerButton9.png", 0, Screen.height );
		
		ability1 = UIButton.create( "FillerButton1.png", "FillerButton1.png", 0, 0 );
		ability2 = UIButton.create( "FillerButton2.png", "FillerButton2.png", 0, 0 );
		ability3 = UIButton.create( "FillerButton3.png", "FillerButton3.png", 0, 0 );
		
		cog.position = new Vector2( 1, -Screen.height + (cog.height + 1) );
		box.position = new Vector3( 0, -Screen.height, 2);
		
		// options screen buttons
		sound = UIButton.create( "PowerIcon.png", "PowerIcon.png", 0, 0 );
		quit = UIButton.create( "EnergyIcon.png", "EnergyIcon.png", 0, 0 );
		replay = UIButton.create( "Cog.png", "Cog.png", 0, 0 );
		
		vertBox = new UIVerticalLayout( 20 );
		vertBox.addChild( sound, quit, replay );
		vertBox.position = new Vector2( 0, Screen.height );
		
		abilityBox = new UIVerticalLayout( 0 );
		abilityBox.addChild( ability1, ability2, ability3 );
		abilityBox.position = new Vector2( 0 , -Screen.height + cog.height*4 +10 );
		
		playButton.onTouchUpInside += sender => playButtonCommands();
		pause.onTouchUpInside += sender => TogglePause();
		fastForward.onTouchUpInside += sender => FFCommands();
		cog.onTouchUpInside += sender => TogglePause();

		sound.onTouchUpInside += sender => soundCommands();
		quit.onTouchUpInside += sender => quitCommands();
		replay.onTouchUpInside += sender => Application.LoadLevel(Application.loadedLevel);

	}

	void BuildMenuFix(){
		
		if(GameControl.gameState!=_GameState.Ended)
		{
			//if PointNBuild mode, only show menu when there are active buildpoint (build menu on)
			if(buildMenu)
			{
				//show up the build buttons, scrolling through currentBuildList initiated whevenever the menu is first brought up
				UnitTower[] towerList=BuildManager.GetTowerList();
				UIButton[] towerButtons = new UIButton[] { tower1, tower2, tower3, tower4, tower5, tower6, tower7, tower8, tower9 };
				
				towerBox = new UIHorizontalLayout( 0 );
				
				for ( int i = 0; i < currentBuildList.Length; ++i )
				{
					int ID = currentBuildList[i];
					if( ID >= 0 )
					{
						towerBox.addChild( towerButtons[i] );
					}
				}
				
				towerBox.position = new Vector2( cog.width, -Screen.height + cog.height );
				
				tower1.onTouchUpInside += sender => BuildButtonPressed(towerList[0]);
				tower2.onTouchUpInside += sender => BuildButtonPressed(towerList[1]);
				tower3.onTouchUpInside += sender => BuildButtonPressed(towerList[2]);
				tower4.onTouchUpInside += sender => BuildButtonPressed(towerList[3]);
				tower5.onTouchUpInside += sender => BuildButtonPressed(towerList[4]);
				tower6.onTouchUpInside += sender => BuildButtonPressed(towerList[5]);
				tower7.onTouchUpInside += sender => BuildButtonPressed(towerList[6]);
				tower8.onTouchUpInside += sender => BuildButtonPressed(towerList[7]);
				tower9.onTouchUpInside += sender => BuildButtonPressed(towerList[8]);
			}
		}
	}
	
	private bool BuildButtonPressed(UnitTower tower){
		if(BuildManager.BuildTowerPointNBuild(tower)){
			//built, clear the build menu flag
			buildMenu=false;
//			ClearBuildListRect();
			towerBox.position = new Vector2( 0, Screen.height );
			return true;
		}	
		else{
			//Debug.Log("build failed. invalide position");
			return false;
		}
	}
	
	
	//show to draw the selected tower info panel, which include the upgrade and sell button
	void SelectedTowerUI(){
		// tower name
		
		// tower level
		
		// cost to upgrade
		
		// sell button
		
		// upgrade button
		
		// abilities will be static not called when clicking on a tower... Sean's understanding atm
		
		
		
		
		
		
		
		
		
		
		
		
//		string towerName=currentSelectedTower.unitName;
//		
//
//		GUI.Label(new Rect(startX, startY+=height, width, height), "Level: "+currentSelectedTower.GetLevel().ToString(), tempGUIStyle);
//
//		string towerInfo="";
//		
//		_TowerType type=currentSelectedTower.type;
//		
//		//display relevent information based on tower type
//		if(type==_TowerType.ResourceTower){
//			
//			//show resource gain value and cooldown only
//			string val=currentSelectedTower.GetDamage().ToString();
//			string cd=currentSelectedTower.GetCooldown().ToString("f1");
//			
//			string rsc="Increase rsc by "+val+" for every cd "+cd+"sec\n";
//			
//			towerInfo+=rsc;
//			
//		}
//		else if(type==_TowerType.SupportTower){
//			
//			//show buff info only
//			BuffStat buffInfo=currentSelectedTower.GetBuff();
//			
//			string buff="";
//			buff+="Buff damage by "+(buffInfo.damageBuff*100).ToString("f1")+"%\n";
//			buff+="Buff range by "+(buffInfo.rangeBuff*100).ToString("f1")+"%\n";
//			buff+="Reduce CD by "+(buffInfo.cooldownBuff*100).ToString("f1")+"%\n";
//			
//			towerInfo+=buff;
//			
//		}
//		else if(type==_TowerType.Mine){
//			//show the basic info for mine
//			if(currentSelectedTower.GetDamage()>0)
//				towerInfo+="Damage: "+currentSelectedTower.GetDamage().ToString("f1")+"\n";
//			if(type==_TowerType.TurretTower && currentSelectedTower.GetAoeRadius()>0)
//				towerInfo+="AOE Radius: "+currentSelectedTower.GetRange().ToString("f1")+"\n";
//			if(currentSelectedTower.GetStunDuration()>0)
//				towerInfo+="Stun target for "+currentSelectedTower.GetStunDuration().ToString("f1")+"sec\n";
//			
//			//if the mine have damage over time value, display it
//			Dot dot=currentSelectedTower.GetDot();
//			float totalDot=dot.damage*(dot.duration/dot.interval);
//			if(totalDot>0){
//				string dotInfo="Cause "+totalDot.ToString("f1")+" damage over the next "+dot.duration+" sec\n";
//				
//				towerInfo+=dotInfo;
//			}
//			
//			//if the mine have slow value, display it
//			Slow slow=currentSelectedTower.GetSlow();
//			if(slow.duration>0){
//				string slowInfo="Slow target by "+(slow.slowFactor*100).ToString("f1")+"% for "+slow.duration.ToString("f1")+"sec\n";
//				towerInfo+=slowInfo;
//			}
//		}
//		else if(type==_TowerType.TurretTower || type==_TowerType.AOETower || type==_TowerType.DirectionalAOETower){
//			
//			//show the basic info for turret and aoeTower
//			if(currentSelectedTower.GetDamage()>0)
//				towerInfo+="Damage: "+currentSelectedTower.GetDamage().ToString("f1")+"\n";
//			if(currentSelectedTower.GetCooldown()>0)
//				towerInfo+="Cooldown: "+currentSelectedTower.GetCooldown().ToString("f1")+"sec\n";
//			if(type==_TowerType.TurretTower && currentSelectedTower.GetAoeRadius()>0)
//				towerInfo+="AOE Radius: "+currentSelectedTower.GetAoeRadius().ToString("f1")+"\n";
//			if(currentSelectedTower.GetStunDuration()>0)
//				towerInfo+="Stun target for "+currentSelectedTower.GetStunDuration().ToString("f1")+"sec\n";
//			
//			//if the tower have damage over time value, display it
//			Dot dot=currentSelectedTower.GetDot();
//			float totalDot=dot.damage*(dot.duration/dot.interval);
//			if(totalDot>0){
//				string dotInfo="Cause "+totalDot.ToString("f1")+" damage over the next "+dot.duration+" sec\n";
//				
//				towerInfo+=dotInfo;
//			}
//			
//			//if the tower have slow value, display it
//			Slow slow=currentSelectedTower.GetSlow();
//			if(slow.duration>0){
//				string slowInfo="Slow target by "+(slow.slowFactor*100).ToString("f1")+"% for "+slow.duration.ToString("f1")+"sec\n";
//				towerInfo+=slowInfo;
//			}
//		}
//		
//		
//		//show the tower's description
//		towerInfo+="\n\n"+currentSelectedTower.description;
//		
//		GUIContent towerInfoContent=new GUIContent(towerInfo);
//			
//		//draw all the information on screen
//		float contentHeight= GUI.skin.GetStyle("Label").CalcHeight(towerInfoContent, 200);
//		GUI.Label(new Rect(startX, startY+=20, width, contentHeight), towerInfoContent);
//	
//		
//		//reset the draw position
//		startY=Screen.height-180-bottomPanelRect.height;
//		if(enableTargetPrioritySwitch){
//			if(currentSelectedTower.type==_TowerType.TurretTower || currentSelectedTower.type==_TowerType.DirectionalAOETower){
//				if(currentSelectedTower.targetingArea!=_TargetingArea.StraightLine){
//					GUI.Label(new Rect(startX, startY, 120, 30), "Targeting Priority:");
//					if(GUI.Button(new Rect(startX+120, startY-3, 100, 30), currentSelectedTower.targetPriority.ToString())){
//						if(currentSelectedTower.targetPriority==_TargetPriority.Nearest) currentSelectedTower.SetTargetPriority(1);
//						else if(currentSelectedTower.targetPriority==_TargetPriority.Weakest) currentSelectedTower.SetTargetPriority(2);
//						else if(currentSelectedTower.targetPriority==_TargetPriority.Toughest) currentSelectedTower.SetTargetPriority(3);
//						else if(currentSelectedTower.targetPriority==_TargetPriority.Random) currentSelectedTower.SetTargetPriority(0);
//					}
//					startY+=30;
//				}
//			}
//		}
//		
//		if(enableTargetDirectionSwitch){
//			if(currentSelectedTower.type==_TowerType.TurretTower || currentSelectedTower.type==_TowerType.DirectionalAOETower){
//				if(currentSelectedTower.targetingArea!=_TargetingArea.AllAround){
//					GUI.changed = false;
//					GUI.Label(new Rect(startX, startY, 120, 30), "Targeting Direction:");
//					float direction=currentSelectedTower.targetingDirection;
//					direction=GUI.HorizontalSlider(new Rect(startX+120, startY+4, 100, 30), direction, 0, 359F);
//					currentSelectedTower.SetTargetingDirection(direction);
//					if(GUI.changed) GameControl.ShowIndicator(currentSelectedTower);
//				}
//			}
//		}
//
//		//check if the tower can be upgrade
//		bool upgradable=false;
//		if(!currentSelectedTower.IsLevelCapped() && currentSelectedTower.IsBuilt()){
//			upgradable=true;
//		}
//		
//		//reset the draw position
//		startY=Screen.height-50-bottomPanelRect.height;
//		
//		//if the tower is eligible to upgrade, draw the upgrade button
//		if(upgradable){
//			if(GUI.Button(new Rect(startX, startY, 100, 30), new GUIContent("Upgrade", "1"))){
//				//upgrade the tower, if false is return, player have insufficient fund
//				if(!currentSelectedTower.Upgrade()) Debug.Log("Insufficient Resource");
//			}
//		}
//		//sell button
//		if(currentSelectedTower.IsBuilt()){
//			if(GUI.Button(new Rect(startX+110, startY, 100, 30), new GUIContent("Sell", "2"))){
//				currentSelectedTower.Sell();
//			}
//		}
//		
//		//if the cursor is hover on the upgrade button, show the cost
//		if(GUI.tooltip=="1"){
//			Resource[] rscList=GameControl.GetResourceList();
//			int[] cost=currentSelectedTower.GetCost();
//			
//			int count=0;
//			foreach(int val in cost){
//				if(val>0) count+=1;
//			}
//			
//			startY-=1+count*25;
//			//~ for(int i=0; i<3; i++) GUI.Box(new Rect(startX, startY, count*25-3, 150), "");
//			count=0;
//			for(int i=0; i<cost.Length; i++){
//				if(cost[i]>0){
//					if(rscList[i].icon!=null){
//						GUI.Label(new Rect(startX+10, startY+count*20, 25, 25), rscList[i].icon);
//						GUI.Label(new Rect(startX+10+25, startY+count*20+3, 150, 25), "- "+cost[i].ToString());
//					}
//					else{
//						GUI.Label(new Rect(startX+10, startY+count*20, 150, 25), " - "+cost[i].ToString()+rscList[i].name);
//					}
//					count+=1;
//				}
//			}
//		}
//		//if the cursor is hover on the sell button, show the resource gain
//		else if(GUI.tooltip=="2"){
//			Resource[] rscList=GameControl.GetResourceList();
//			int[] sellValue=currentSelectedTower.GetTowerSellValue();
//			
//			int count=0;
//			foreach(int val in sellValue){
//				if(val>0) count+=1;
//			}
//			
//			startY-=1+count*25;
//			count=0;
//			for(int i=0; i<sellValue.Length; i++){
//				if(sellValue[i]>0){
//					if(rscList[i].icon!=null){
//						GUI.Label(new Rect(startX+120, startY+count*20, 25, 25), rscList[i].icon);
//						GUI.Label(new Rect(startX+120+25, startY+count*20+3, 150, 25), "+ "+sellValue[i].ToString());
//					}
//					else{
//						GUI.Label(new Rect(startX+120, startY+count*20, 150, 25), " + "+sellValue[i].ToString()+rscList[i].name);
//					}
//					count+=1;
//				}
//			}
//		}
	}
	
	//called whevenever the build list is called up
	//compute the number of tower that can be build in this build pointer
	//store the tower that can be build in an array of number that reference to the towerlist
	//this is so these dont need to be calculated in every frame in OnGUI()
	void UpdateBuildList(){
		
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
				
			if(currentBuildInfo.specialBuildableID!=null && currentBuildInfo.specialBuildableID.Length>0){
				foreach(int specialBuildableID in currentBuildInfo.specialBuildableID){
					if(specialBuildableID==tower.specialID){
						count+=1;
						break;
					}
				}
			}
			else{
				if(tower.specialID<0){
					//check if this type of tower can be build on this platform
					foreach(_TowerType type in currentBuildInfo.buildableType){
						if(tower.type==type && tower.specialID<0){
							tempBuildList[count]=i;
							count+=1;
							break;
						}
					}
				}
			}
		}
		
		//for as long as the number that can be build, copy from the temp buildList to the real buildList
		currentBuildList=new int[count];
		for(int i=0; i<currentBuildList.Length; i++) currentBuildList[i]=tempBuildList[i];
	}
}
