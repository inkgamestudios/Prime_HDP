using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

public class GameUI : MonoBehaviour {
	
	public enum _BuildMenuType{Fixed}
	public _BuildMenuType buildMenuType;
	
	public enum _BuildMode{PointNBuild}
	public _BuildMode buildMode;
	
	public int fastForwardSpeed=3;
	
	public bool enableTargetPrioritySwitch=true;
	public bool enableTargetDirectionSwitch=true;

	private bool enableSpawnButton=true;
	
	private bool buildMenu=false;
	
	private UnitTower currentSelectedTower;
	
	private int[] currentBuildList=new int[0];
	
#pragma warning disable 0414
	//indicate if the player have won or lost the game
	private bool winLostFlag=false;
#pragma warning restore 0414
	
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
#pragma warning disable 0414
	private UIButton energyIcon;
	private UIButton powerIcon;
#pragma warning restore 0414
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
	
	private UIStateButton tPriority;
	
	private UIText text;
	private UITextInstance waveInfo;
	private UITextInstance lifeInfo;
	private UITextInstance energyInfo;
	private UITextInstance ppInfo;
	private UITextInstance endInfo;
	private UITextInstance towerUIName;
	private UITextInstance towerUILevel;
	private UITextInstance towerUIType;
	private UITextInstance towerUIDamage;
	private UITextInstance towerUICost1;
	private UITextInstance towerUICost2;
	private UITextInstance towerUISellVal1;
	private UITextInstance towerUISellVal2;
	
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
		if(flag == true )
		{
			EndScenerio("You won!");
			Debug.Log("You won!");
		}
		else
		{
			EndScenerio("You Lost!");
			Debug.Log("You Lost!");
		}
	}
	
	void EndScenerio( string ending )
	{
		endInfo = text.addTextInstance( ending, 0, 0 );
		endInfo.position = new Vector3( Screen.width/2 - endInfo.width/2, -Screen.height/4, 1 );
		TogglePause();
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
		if(GameControl.gameState!=_GameState.Ended)
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
		else if(GameControl.gameState == _GameState.Ended)
		{
			if(Time.timeScale != 0){
				paused=!paused;
				Time.timeScale=0;
				cogCommands(); // replace w/ w/e our end menu screen will be **
				swapButtons( playButton, pause );
				
				// close all the button, so user can interact with game when it's paused
				if(currentSelectedTower!=null){
					GameControl.ClearSelection();
					currentSelectedTower=null;
				}
				if(buildMenu){
					buildMenu=false;
					BuildManager.ClearBuildPoint();
				}
			}
		}
	}
	
	void ResetGUI(int choice = 0)
	{
		switch(choice)
		{
			// for all gui
		case 0:
			buildMenu=false;
			DestroyTowerUI(1);
			BuildManager.ClearBuildPoint();
			GameControl.ClearSelection();
			towerBox.position = new Vector2( 0, Screen.height );
			break;
			// for towerUI
		case 1:
			DestroyTowerUI(1);
			GameControl.ClearSelection();
			break;
			// for tower selection gui
		case 2:
			buildMenu = false;
			BuildManager.ClearBuildPoint();
			towerBox.position = new Vector2( 0, Screen.height );
			break;
			// clear build point
		case 3:
			buildMenu = false;
			BuildManager.ClearBuildPoint();
			break;
		default:
			Debug.Log("error out of bounds");
			break;
		}
	}
	
	private bool click = false;
	// Update is called once per frame
	void Update () 
	{
		#if !UNITY_IPHONE && !UNITY_ANDROID
			if(buildMode==_BuildMode.PointNBuild)
				BuildManager.SetIndicator(Input.mousePosition);
		#endif
		upgrade.onTouchUpInside += sender => click = CheckUpgrade();
		sell.onTouchUpInside += sender => click = SellTower();
		
		
		if(Input.GetMouseButtonUp(0))
		{
			UnitTower tower = GameControl.Select(Input.mousePosition);
			// if gui
			if(click)
			{
				click = false;
			}
			// else all other options while not paused
			else if(!paused)
			{
				// if tower selected
				if(tower != null )
				{
					Debug.Log("check point 1");
					if(currentSelectedTower != null)
					{
						Debug.Log("current selected tower doesn't = null :( ");
						// turning off towerUI
						ResetGUI(1);
					}
					//if the build menu is active and your clicking on a button
					if(buildMenu)
					{
						Debug.Log( "build menu again :/" );
						// turning off build menu if selecting a tower
						ResetGUI(2);
					}
					currentSelectedTower = tower;
					SelectedTowerUI();
				}
				// tower is not selected
				else
				{
					if(buildMenu)
					{
						Debug.Log ("oh no build menu was on when clicking on a tower");
						// disable build menu
						ResetGUI(2);
					}
					// checking build point
					if(BuildManager.CheckBuildPoint(Input.mousePosition))
					{
						Debug.Log("you clicked the grid didn't you?");
						UpdateBuildList();
						buildMenu=true;
						BuildMenuFix();
					}
					else
					{
						if(buildMenu)
						{
							Debug.Log("should never get here buildmenu else statement");
							SelectedTowerUI();
							DestroyTowerUI(1);
							SelectedTowerUI();
							ResetGUI(1);
						}
						if(towerUIActivated)
						{
							Debug.Log("should never get here either toweruiactivated");
							ResetGUI(1);
						}
					}
				}
			}
		}
		
		
		
		
		
//		if(Input.GetMouseButtonUp(0) && !paused){
//			
//			//check if user click on towers
//			UnitTower tower=GameControl.Select(Input.mousePosition);
//			// check for gui clicks
//			if(click)
//			{
//				click = false;
//				if(currentSelectedTower != null )
//				{
//					DestroyTowerUI(1);
//					SelectedTowerUI();
//// shouldn't need this		towerBox.position = new Vector2( 0, Screen.height );
//				}
//			}
//
//			//if user click on tower, select the tower
//			else if(tower!=null){
//				currentSelectedTower=tower;
//				
//				//if build mode is active, disable buildmode
//				if(buildMenu){
//					buildMenu=false;
//					BuildManager.ClearBuildPoint();
//					towerBox.position = new Vector2( 0, Screen.height );
//					DestroyTowerUI(1);
//				}
//				if(currentSelectedTower != null)
//				{
//					if(towerUIActivated)
//					{
//						DestroyTowerUI(1);
//					}
//					SelectedTowerUI();
//				}
//			}
//			no tower is selected
//			else{
//				//if a tower is selected previously, clear the selection
//				if(currentSelectedTower!=null){
//					DestroyTowerUI(1);
//					GameControl.ClearSelection();
//					currentSelectedTower=null;
//					towerBox.position = new Vector2( 0, Screen.height );
//				}
//				
//				//if we are in PointNBuild Mode
//				if(buildMode==_BuildMode.PointNBuild){
//					//check for build point, if true initiate build menu
//					if(BuildManager.CheckBuildPoint(Input.mousePosition) && !buildMenuActive && !towerUIActivated){
//							UpdateBuildList();
//							buildMenu=true;
//							buildMenuActive = true;
//							BuildMenuFix();
//					}
//					//if the build menu is active and your clicking on a button
//					else if(buildMenuActive)
//					{
//						buildMenu=false;
//						buildMenuActive = false;
//						BuildManager.ClearBuildPoint();
//						towerBox.position = new Vector2( 0, Screen.height );
//					}
//					//if there are no valid build point but we are in build mode, disable it
//					else{
//						if(buildMenu){
//							buildMenu=false;
//							buildMenuActive = false;
//							BuildManager.ClearBuildPoint();
//							towerBox.position = new Vector2( 0, Screen.height );
//						}
//					}
//				}
//			}
//		}
		
		//if escape key is pressed, toggle pause
		if(Input.GetKeyUp(KeyCode.Escape)){
			TogglePause();
		}
		// destroying and instantiating the text assets since editing them doesn't work.
		TextDestroy();
		TextInfo();
	}

	//check if user has click on creep, if yes and if current tower is eligible to attack it, set the assign it as tower target
	void CheckForTarget(){
		currentSelectedTower.CheckForTarget(Input.mousePosition);
	}
	
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
		click = true;
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
		click = true;
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
		click = true;
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
	{ 
		click = true;
		Debug.Log("sound button clicked");
	}
	
	void quitCommands()
	{
		click = true;
		Debug.Log("quit button clicked");
	}
	
	void GameOverScreen()
	{
		// not sure what we are doing for the game over screen yet so this for now... *****
	}
	
	void TextInfo()
	{
		waveInfo = text.addTextInstance( "Wave: " +SpawnManager.GetCurrentWave() +"/" +SpawnManager.GetTotalWave(), Screen.width/2 - 50, 5 );
		lifeInfo = text.addTextInstance( "Life: " +GameControl.GetPlayerLife(), Screen.width - 100, 5 );
		energyInfo = text.addTextInstance( ResourceManager.GetResourceVal(0).ToString(), Screen.width - 50, Screen.height - 45 );
		ppInfo = text.addTextInstance( ResourceManager.GetResourceVal(1).ToString(), Screen.width - 50, Screen.height - 30 );
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
		
#pragma warning disable 0414
		energyIcon = UIButton.create( "EnergyIcon.png", "EnergyIcon.png", Screen.width - 100, Screen.height - 45 );
		powerIcon = UIButton.create( "PowerIcon.png", "PowerIcon.png", Screen.width - 100, Screen.height - 30 );
#pragma warning restore 0414
		
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
		
		upgrade = UIButton.create("upgrade.png", "upgrade.png", 0, Screen.height );
		sell = UIButton.create("sell.png", "sell.png", 0, Screen.height );
		// toggle button
		string[] upStates = new string[] {"FillerButton1.png", "FillerButton2.png", "FillerButton3.png", "FillerButton4.png"};
		string[] downStates = new string[] {"FillerButton1.png", "FillerButton2.png", "FillerButton3.png", "FillerButton4.png"};
		tPriority = UIStateButton.create( upStates, downStates, 0, Screen.height );
		tPriority.onStateChange += (sender, state) => click = currentSelectedTower.SetTargetPriority(state);
		
		cog.position = new Vector2( 1, -Screen.height + (cog.height + 1) );
		box.position = new Vector3( 0, -Screen.height, 2);
		
		// options screen buttons
		sound = UIButton.create( "PowerIcon.png", "PowerIcon.png", 0, 0 );
		quit = UIButton.create( "EnergyIcon.png", "EnergyIcon.png", 0, 0 );
		replay = UIButton.create( "Cog.png", "Cog.png", 0, 0 );
		// options button holder
		vertBox = new UIVerticalLayout( 20 );
		vertBox.addChild( sound, quit, replay );
		vertBox.position = new Vector2( 0, Screen.height );
		// ability button holder
		abilityBox = new UIVerticalLayout( 0 );
		abilityBox.addChild( ability1, ability2, ability3 );
		abilityBox.position = new Vector2( 0 , -Screen.height + cog.height*4 +10 );
		// button commands
		playButton.onTouchUpInside += sender => playButtonCommands();
		pause.onTouchUpInside += sender => TogglePause();
		fastForward.onTouchUpInside += sender => FFCommands();
		cog.onTouchUpInside += sender => TogglePause();

		sound.onTouchUpInside += sender => soundCommands();
		quit.onTouchUpInside += sender => quitCommands();
		replay.onTouchUpInside += sender => Application.LoadLevel(Application.loadedLevel);
		
		ability1.onTouchUpInside += sender => Abilities(1);
		ability2.onTouchUpInside += sender => Abilities(2);
		ability3.onTouchUpInside += sender => Abilities(3);
	}
	
	void Abilities( int effect )
	{
		click = true;
		switch( effect )
		{
		case 1:
			// do something 1
			Debug.Log("pushed ability 1");
			break;
		case 2:
			// do something 2
			Debug.Log("pushed ability 2");
			break;
		case 3:
			// do something 3
			Debug.Log("pushed ability 3");
			break;
		default:
			// oh no error!
			Debug.Log("Error abilities is out of bounds");
			break;
		}
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
			click = true;
			towerBox.position = new Vector2( 0, Screen.height );
			return true;
		}	
		else{
			//Debug.Log("build failed. invalide position");
			return false;
		}
	}
	
	void DestroyTowerUI(int choices = 1)
	{
		switch(choices)
		{
		case 1:
			towerUIName.destroy();
			towerUILevel.destroy();
			towerUIType.destroy();
			towerUIDamage.destroy();
			towerUICost1.destroy();
			towerUICost2.destroy();
			towerUISellVal1.destroy();
			towerUISellVal2.destroy();

			sell.position = new Vector2( 0, Screen.height );
			upgrade.position = new Vector2( 0, Screen.height );
			tPriority.position = new Vector2( 0, Screen.height );
			
			towerUIActivated = false;
			BuildManager.ClearBuildPoint();
			break;
		case 2:
			upgrade.position = new Vector2( 0, Screen.height);
			break;
		default:
			Debug.Log("DestroyTowerUI has a wrong input value" );
			break;
		}
	}
	
	bool towerUIActivated = false;
	//show to draw the selected tower info panel, which include the upgrade and sell button
	void SelectedTowerUI(){
		if(currentSelectedTower != null )
		{
			towerUIActivated = true;
			int widthPosition = Screen.width/2 + 50;
			// TOWER NAME
			towerUIName = text.addTextInstance( currentSelectedTower.unitName, widthPosition, Screen.height/3 );
			// TOWER LVL
			towerUILevel = text.addTextInstance( "Lvl: " +currentSelectedTower.GetLevel(), widthPosition + towerUIName.width + 20, Screen.height/3 );
			// TOWER TYPE
			towerUIType = text.addTextInstance( currentSelectedTower.type.ToString() , widthPosition, Screen.height/3 + 20 );
			// DAMAGE
			towerUIDamage = text.addTextInstance( "Damage: " +currentSelectedTower.baseStat.damage, widthPosition, Screen.height/3 +40 );
			// cost to upgrade
			int[] upgradeCost = currentSelectedTower.GetCost();
			towerUICost1 = text.addTextInstance( "PP: " +upgradeCost[0], widthPosition, Screen.height/3 + 60 );
			towerUICost2 = text.addTextInstance( "E: " +upgradeCost[1], widthPosition, Screen.height/3 + 80 );
			// SELL VALUE
			int[] sellValue = currentSelectedTower.GetTowerSellValue();
			towerUISellVal1 = text.addTextInstance( "Sell Value PP: " +sellValue[0], widthPosition, Screen.height/3 + 100 );
			towerUISellVal2 = text.addTextInstance( "Sell Value E: " +sellValue[1], widthPosition, Screen.height/3 + 120 );
			
			//reset the draw position
			if(enableTargetPrioritySwitch){
				if(currentSelectedTower.type==_TowerType.TurretTower || currentSelectedTower.type==_TowerType.DirectionalAOETower){
					if(currentSelectedTower.targetingArea!=_TargetingArea.StraightLine){
						// Nearest, weakest, toughest, random
						tPriority.position = new Vector2( widthPosition, -Screen.height/3 - 140 );
						
						if(currentSelectedTower.targetPriority==_TargetPriority.Nearest){
							currentSelectedTower.SetTargetPriority(0);
							tPriority.state = 0;
						}
						else if(currentSelectedTower.targetPriority==_TargetPriority.Weakest){
							currentSelectedTower.SetTargetPriority(1);
							tPriority.state = 1;
						}
						else if(currentSelectedTower.targetPriority==_TargetPriority.Toughest){
							currentSelectedTower.SetTargetPriority(2);
							tPriority.state = 2;
						}
						else if(currentSelectedTower.targetPriority==_TargetPriority.Random){
							currentSelectedTower.SetTargetPriority(3);
							tPriority.state = 3;
						}
					}
				}
			}
			
			
			//check if the tower can be upgrade
			bool upgradable=false;
			if(!currentSelectedTower.IsLevelCapped() && currentSelectedTower.IsBuilt()){
				upgradable=true;
				
			}
			if(!upgradable)
			{
				DestroyTowerUI(2);
			}
			//if the tower is eligible to upgrade, draw the upgrade button
			if(upgradable){
				upgrade.position= new Vector2( widthPosition, -Screen.height/3 - 200 );
			}
			// SELL BUTTON
			if(currentSelectedTower.IsBuilt()){
				sell.position = new Vector2( widthPosition + upgrade.width + 5, -Screen.height/3 - 200 );
			}
		}
	}
	
	bool CheckUpgrade()
	{
		if(currentSelectedTower.Upgrade())
		{
			Debug.Log("upgrade affordable and now upgraded");
			DestroyTowerUI(1);
			return true;
		}
		else
		{
			Debug.Log("Upgrade is not affordable");
			return false;
		}
	}
	bool SellTower()
	{
		currentSelectedTower.Sell();
		GameControl.ClearSelection();
		currentSelectedTower = null;
		DestroyTowerUI(1);
		return true;
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
