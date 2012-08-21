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
	
	private UIButton cog;
	private UIButton playButton;
	private UIButton sell;
	private UIButton pause;
	private UIButton upgrade;
	private UIButton fastForward;
	
	private UISprite wave;
	private UISprite life;
	private UISprite energy;
	private UISprite power;
	
	private UIButton sound;
	private UIButton quit;
	private UIButton replay;

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
	private UIButton ability4;
	private UIButton ability5;
	
	private UIVerticalPanel pauseScreen;
	
	private Rect topPanelRect=new Rect(-3, -3, Screen.width+6, 28);
	private Rect bottomPanelRect;
	private Rect buildListRect;
	private Rect towerUIRect;
	private Rect[] scatteredRectList=new Rect[0];
	
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
		Debug.Log( "first paused is " + paused );
		paused=!paused;
		Debug.Log( "second paused is " + paused );
		if(paused){
			timeHolder = Time.timeScale;
			Time.timeScale=0;
			
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
		
		
		
		if(Input.GetMouseButtonUp(0) && !IsCursorOnUI(Input.mousePosition) && !paused){
			
			//check if user click on towers
			UnitTower tower=GameControl.Select(Input.mousePosition);
			
			//if user click on tower, select the tower
			if(tower!=null){
				currentSelectedTower=tower;
				
				//if build mode is active, disable buildmode
				if(buildMenu){
					buildMenu=false;
					BuildManager.ClearBuildPoint();
					ClearBuildListRect();
				}
			}
			//no tower is selected
			else{
				//if a tower is selected previously, clear the selection
				if(currentSelectedTower!=null){
					GameControl.ClearSelection();
					currentSelectedTower=null;
					towerUIRect=new Rect(0, 0, 0, 0);
					//UIRect.RemoveRect(towerUIRect);
				}
				
				//if we are in PointNBuild Mode
				if(buildMode==_BuildMode.PointNBuild){
				
					//check for build point, if true initiate build menu
					if(BuildManager.CheckBuildPoint(Input.mousePosition)){
						UpdateBuildList();
						buildMenu=true;
					}
					//if there are no valid build point but we are in build mode, disable it
					else{
						if(buildMenu){
							buildMenu=false;
							BuildManager.ClearBuildPoint();
							ClearBuildListRect();
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
				ClearBuildListRect();
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
	}
	
	
	//check if user has click on creep, if yes and if current tower is eligible to attack it, set the assign it as tower target
	void CheckForTarget(){
		currentSelectedTower.CheckForTarget(Input.mousePosition);
	}
	
	

// not sure if this is needed given the fact that we are using uitoolkit
	//check for all UI screen space, see if user cursor is within any of them, return true if yes
	//this is to prevent user from being able to interact with in game object even when clicking on UI panel and buttons
	public bool IsCursorOnUI(Vector3 point){
		Rect tempRect=new Rect(0, 0, 0, 0);
		
		tempRect=topPanelRect;
		tempRect.y=Screen.height-tempRect.y-tempRect.height;
		if(tempRect.Contains(point)) return true;
		
		tempRect=bottomPanelRect;
		tempRect.y=Screen.height-tempRect.y-tempRect.height;
		if(tempRect.Contains(point)) return true;
		
		tempRect=buildListRect;
		tempRect.y=Screen.height-tempRect.y-tempRect.height;
		if(tempRect.Contains(point)) return true;
		
		tempRect=towerUIRect;
		tempRect.y=Screen.height-tempRect.y-tempRect.height;
		if(tempRect.Contains(point)) return true;
		
		for(int i=0; i<scatteredRectList.Length; i++){
			tempRect=scatteredRectList[i];
			tempRect.y=Screen.height-tempRect.y-tempRect.height;
			if(tempRect.Contains(point)) return true;
		}
		
		return false;
	}
	
	//clear all ui space occupied by build menu
	void ClearBuildListRect(){
		if(buildMode==_BuildMode.PointNBuild){
			buildListRect=new Rect(0, 0, 0, 0);
		}
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
				swapButtons( playButton, pause );
			}
		}
	}
	
	void pauseCommands()
	{
		TogglePause();
		swapButtons( playButton, pause );
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
		pauseScreen.positionCenter();
	}
	
	//draw GUI 
	void DrawGUI()
	{
			
		playButton = UIButton.create( "Play.png", "Play.png", 3, 3 );
		pause = UIButton.create( "Pause.png", "Pause.png", 0, -Screen.height );
		fastForward = UIButton.create( "FastForward.png", "FastForward.png", (int)playButton.width + 5, 3 );
		cog = UIButton.create( "Cog.png", "Cog.png", 0, 0 );
		cog.position = new Vector2( 1, -Screen.height + (cog.height + 1) );
		
		sound = UIButton.create( "PowerIcon.png", "PowerIcon.png", 0, 0 );
		quit = UIButton.create( "EnergyIcon.png", "EnergyIcon.png", 0, 0 );
		replay = UIButton.create( "Cog.png", "Cog.png", 0, 0 );

		pauseScreen = UIVerticalPanel.create( "PanelsTop.png", "PanelsCenter2.png", "PanelsBottom.png" );
		pauseScreen.beginUpdates();
		pauseScreen.spacing = 20;
		pauseScreen.edgeInsets = new UIEdgeInsets( 30, 10, 20, 10 );
		pauseScreen.addChild( sound, quit, replay );
		pauseScreen.endUpdates();

		pauseScreen.positionCenter();
		
		playButton.onTouchUpInside += sender => playButtonCommands();
		pause.onTouchUpInside += sender => pauseCommands();
		fastForward.onTouchUpInside += sender => FFCommands();
		cog.onTouchUpInside += sender => cogCommands();


//		tower1 = UIButton.create( "FillerButton1.png", "FillerButton1.png", 0, 0);
//		tower1.highlightedTouchOffsets = new UIEdgeOffsets( 30 );

//		UIHorizontalLayout towers = new UIHorizontalLayout( 0 );
//		for( int x = 0; x < counter; x++ )
//		{
//			towers.addChild( unlockedTowers[x] );
//		}
//		
//		towers.position = new Vector2( cog.width, -Screen.height + (int)tower1.height );
//			
//		cog.highlightedTouchOffsets = new UIEdgeOffsets( 30 );

		//if not pause, draw the spawnButton and fastForwardButton
		


			//shift the cursor to where the next element will be drawn
//			else buttonX+=65;
//			buttonX+=130;
			
			//draw life and wave infomation
//			GUI.Label(new Rect(200, 5, 100, 30), "Life: "+GameControl.GetPlayerLife());
//			GUI.Label(new Rect(200+(topPanelRect.width-35-buttonX)/2, 5, 100, 30), "Wave: "+SpawnManager.GetCurrentWave()+"/"+SpawnManager.GetTotalWave());
			
			
		//draw bottom panel
//		GUI.BeginGroup (bottomPanelRect);
//			for(int i=0; i<2; i++) GUI.Box(new Rect(0, 0, bottomPanelRect.width, bottomPanelRect.height), "");
		
			//get all the resource information
		Resource[] resourceList=GameControl.GetResourceList();
		float subStartX=10; float subStartY=0; float width=0f;
		
//		GUI.Label(new Rect(subStartX, subStartY+2, 70f, 25f), "Resource:");
		subStartX+=70;
		
		//display all the resource
		for(int i=0; i<resourceList.Length; i++)
		{
			
			//if an icon has been assigned to that particular resource type
			if(resourceList[i].icon!=null)
			{
//				GUI.Label(new Rect(subStartX, subStartY, 20f, 25f), resourceList[i].icon);
				subStartX+=20;
//				GUI.Label(new Rect(subStartX, subStartY+2, 40, 25), resourceList[i].value.ToString());
				subStartX+=40;
			}
			//if not icon, just show the text
			else 
			{
//				GUIContent labelRsc=new GUIContent(resourceList[i].value.ToString()+resourceList[i].name);
//				GUI.skin.GetStyle("Label").CalcMinMaxWidth(labelRsc, out width, out width);
//				GUI.Label(new Rect(subStartX, subStartY+2, width, 25), labelRsc);
//				subStartX+=width;
			}
			
		}
//		GUI.EndGroup ();
		
		
		//if game is still not over
		if(GameControl.gameState!=_GameState.Ended)
		{
			
			//clear tooltip, each button when hovered will assign tooltip with corresponding ID
			//the tooltip will be checked later, if there's anything, we show the corresponding tooltip
			
//			GUI.tooltip="";
			
			//build menu
			if(buildMode==_BuildMode.PointNBuild)
			{
				//if PointNBuild mode, only show menu when there are active buildpoint (build menu on)
				if(buildMenu)
				{
					if(buildMenuType==_BuildMenuType.Fixed) BuildMenuFix();
				}
				//if there's no build menu, clear the buildList rect
//				else buildListRect=new Rect(0, 0, 0, 0);
			}
			
			//if the cursor is hover on top of tower button, the tooltip will bear the ID of the Buildable Tower
			if(GUI.tooltip!="")
			{
				int ID=int.Parse(GUI.tooltip);
				//show the build tooltip
				ShowToolTip(ID);
				//if no preview has been showing, preview the sample tower on the grid, only needed in PointNBuild mode
				if(buildMode==_BuildMode.PointNBuild && showBuildSample) BuildManager.ShowSampleTower(ID); 
			}
			else
			{
				//if a sample tower is shown on the grid, only needed in PointNBuild mode
				if(buildMode==_BuildMode.PointNBuild && showBuildSample) BuildManager.ClearSampleTower();
			}
			
			//selected tower information UI
			if(currentSelectedTower!=null)
			{
				SelectedTowerUI();
			}
			//else towerUIRect=new Rect(0, 0, 0, 0);
			
			//if paused, draw the pause menu
			if(paused)
			{
				float startX=Screen.width/2-100;
				float startY=Screen.height*0.35f;
				
//				GUI.Box(new Rect(startX, startY, 200, 150), "Game Paused");
//				
//				startX+=50;
//				
//				if(GUI.Button(new Rect(startX, startY+=30, 100, 30), "Resume Game")){
//					TogglePause();
//				}
//				if(GUI.Button(new Rect(startX, startY+=35, 100, 30), "Next Level")){
//					Application.LoadLevel(Application.loadedLevelName);
//				}
//				if(GUI.Button(new Rect(startX, startY+=35, 100, 30), "Main Menu")){
//					if(mainMenu!="") Application.LoadLevel(mainMenu);
//				}
			}
			
		}
		//gameOver, draw the game over screen
		else
		{
			
			float startX=Screen.width/2-100;
			float startY=Screen.height*0.35f;
			
			string levelCompleteString="Level Complete";
			if(!winLostFlag) levelCompleteString="Level Lost";
		
			
//			GUI.Box(new Rect(startX, startY, 200, 150), levelCompleteString);
//			startX+=50;
//
//			if(GUI.Button(new Rect(startX, startY+=30, 100, 30), "Restart Level")){
//				Application.LoadLevel(Application.loadedLevelName);
//			}
//			if(alwaysEnableNextButton || winLostFlag){
//				if(GUI.Button(new Rect(startX, startY+=35, 100, 30), "Next Level")){
//					if(nextLevel!="") Application.LoadLevel(nextLevel);
//				}
//			}
//			if(GUI.Button(new Rect(startX, startY+=35, 100, 30), "Main Menu")){
//				if(mainMenu!="") Application.LoadLevel(mainMenu);
//			}
		
		}
		
		
		//if game message is not empty, show the game message.
		//shift the text alignment to LowerRight first then back to UpperLeft after the message
//		if(gameMessage!="")
		{
//			GUI.skin.label.alignment=TextAnchor.LowerRight;
//			GUI.Label(new Rect(0, 0, Screen.width-5, Screen.height+12), gameMessage);
//			GUI.skin.label.alignment=TextAnchor.UpperLeft;
		}
		
	}
	
//	DO NOT NEED TOOLTIP
	//show tooptip when a build button is hovered
	void ShowToolTip(int ID){
		
		//get the tower component first
		UnitTower[] towerList=BuildManager.GetTowerList();
		UnitTower tower=towerList[ID];
		
		//create a new GUIStyle to highlight the tower name with different font size and color
		GUIStyle tempGUIStyle=new GUIStyle();
		
		tempGUIStyle.fontStyle=FontStyle.Bold;
		tempGUIStyle.fontSize=16;
		
		//create the GUIContent that shows the tower's name
		string towerName=tower.unitName;
		GUIContent guiContentTitle=new GUIContent(towerName); 
		
		//calculate the height required
		float heightT=tempGUIStyle.CalcHeight(guiContentTitle, 150);
		
		//switch to normal guiStyle and calculate the height needed to display cost
		tempGUIStyle.fontStyle=FontStyle.Normal;
		int[] cost=tower.GetCost();
		float heightC=0;	
		for(int i=0; i<cost.Length; i++){
			if(cost[i]>0){
				heightC+=25;
			}
		}
		
		//create a guiContent showing the tower description
		string towerDesp=tower.GetDescription();
		GUIContent guiContent=new GUIContent(""+towerDesp); 
		//calculate the height require to show the tower description
		float heightD= GUI.skin.GetStyle("Label").CalcHeight(guiContent, 150);
		
		//sum up all the height, so the tooltip box size can be known
		float height=heightT+heightC+heightD+10;
		
		//set the tooltip draw position
		int y=32;
		int x=5;

		//define the rect then draw the box
		Rect rect=new Rect(x, y, 160, height);
		for(int i=0; i<2; i++) GUI.Box(rect, "");
		
		//display all the guiContent assigned ealier
		GUI.BeginGroup(rect);
			//show tower name, format it to different text style, size and color
			tempGUIStyle.fontStyle=FontStyle.Bold;
			tempGUIStyle.fontSize=16;
			tempGUIStyle.normal.textColor=new Color(1, 1, 0, 1f);
			GUI.Label(new Rect(5, 5, 150, heightT), guiContentTitle, tempGUIStyle);
		
			//show tower's cost
			//get the resourceList from GameControl so we have all the information
			Resource[] rscList=GameControl.GetResourceList();
			//loop throught all the resource type in the cost list
			for(int i=0; i<cost.Length; i++){
				//only show it this the cost required something from this type of resource
				if(cost[i]>0){
					//check if the resource type has a icon or not
					if(rscList[i].icon!=null){
						//show the cost with the resource type icon, draw the icon first then the cost value
						GUI.Label(new Rect(5, 5+heightT+i*20, 25, 25), rscList[i].icon);
						GUI.Label(new Rect(5+25, 5+heightT+i*20+2, 150, 25), "- "+cost[i].ToString());
					}
					else{
						//show the cost with the resource type name
						GUI.Label(new Rect(5, 5+heightT+i*20, 150, 25), " - "+cost[i].ToString()+rscList[i].name);
					}
				}
			}
			
			//show desciption
			GUI.Label(new Rect(5, 5+heightC+heightT, 150, heightD), guiContent);
		GUI.EndGroup();
	}

	void BuildMenuFix(){
		
		int width=50;
		int height=50;
				
		int x=0;
		int y=Screen.height-height-6-(int)bottomPanelRect.height;
		
		for(int i=0; i<3; i++) GUI.Box(buildListRect, "");
		
		//show up the build buttons, scrolling through currentBuildList initiated whevenever the menu is first brought up
		UnitTower[] towerList=BuildManager.GetTowerList();
		
		x+=3;	y+=3;
			
		for(int num=0; num<currentBuildList.Length; num++){
			int ID=currentBuildList[num];
			
			if(ID>=0){
				UnitTower tower=towerList[ID];
				
				GUIContent guiContent=new GUIContent(tower.icon, ID.ToString()); 
				if(GUI.Button(new Rect(x, y, width, height), guiContent)){
					//if building was successful, break the loop can close the panel
					if(BuildButtonPressed(tower)) return;
				}
				
				else x+=width+3;
			}
		}
			
		//clear buildmode button
		if(GUI.Button(new Rect(x, y, width, height), "X")){
			buildMenu=false;
			BuildManager.ClearBuildPoint();
			ClearBuildListRect();
		}
	}
	
	private bool BuildButtonPressed(UnitTower tower){
		if(BuildManager.BuildTowerPointNBuild(tower)){
			//built, clear the build menu flag
			buildMenu=false;
			ClearBuildListRect();
			return true;
		}	
		else{
			//Debug.Log("build failed. invalide position");
			return false;
		}
	}
	
	
	//show to draw the selected tower info panel, which include the upgrade and sell button
	void SelectedTowerUI(){
		
		float startX=Screen.width-260;
		float startY=Screen.height-455-bottomPanelRect.height;
		float widthBox=250;
		float heightBox=450;
		
		towerUIRect=new Rect(startX, startY, widthBox, heightBox);
		for(int i=0; i<3; i++) GUI.Box(towerUIRect, "");
		
		startX=Screen.width-260+20;
		startY=Screen.height-455-bottomPanelRect.height+20;
		
		float width=250-40;
		float height=20;
		
		GUIStyle tempGUIStyle=new GUIStyle();
		
		tempGUIStyle.fontStyle=FontStyle.Bold;
		tempGUIStyle.fontSize=16;
		tempGUIStyle.normal.textColor=new Color(1, 1, 0, 1f);
		
		string towerName=currentSelectedTower.unitName;
		GUIContent guiContentTitle=new GUIContent(towerName); 
		
		GUI.Label(new Rect(startX, startY, width, height), guiContentTitle, tempGUIStyle);
		
		tempGUIStyle.fontStyle=FontStyle.Bold;
		tempGUIStyle.fontSize=13;
		tempGUIStyle.normal.textColor=new Color(1, 0, 0, 1f);
		
		GUI.Label(new Rect(startX, startY+=height, width, height), "Level: "+currentSelectedTower.GetLevel().ToString(), tempGUIStyle);
		
		startY+=20;
		
		
		string towerInfo="";
		
		_TowerType type=currentSelectedTower.type;
		
		//display relevent information based on tower type
		if(type==_TowerType.ResourceTower){
			
			//show resource gain value and cooldown only
			string val=currentSelectedTower.GetDamage().ToString();
			string cd=currentSelectedTower.GetCooldown().ToString("f1");
			
			string rsc="Increase rsc by "+val+" for every cd "+cd+"sec\n";
			
			towerInfo+=rsc;
			
		}
		else if(type==_TowerType.SupportTower){
			
			//show buff info only
			BuffStat buffInfo=currentSelectedTower.GetBuff();
			
			string buff="";
			buff+="Buff damage by "+(buffInfo.damageBuff*100).ToString("f1")+"%\n";
			buff+="Buff range by "+(buffInfo.rangeBuff*100).ToString("f1")+"%\n";
			buff+="Reduce CD by "+(buffInfo.cooldownBuff*100).ToString("f1")+"%\n";
			
			towerInfo+=buff;
			
		}
		else if(type==_TowerType.Mine){
			//show the basic info for mine
			if(currentSelectedTower.GetDamage()>0)
				towerInfo+="Damage: "+currentSelectedTower.GetDamage().ToString("f1")+"\n";
			if(type==_TowerType.TurretTower && currentSelectedTower.GetAoeRadius()>0)
				towerInfo+="AOE Radius: "+currentSelectedTower.GetRange().ToString("f1")+"\n";
			if(currentSelectedTower.GetStunDuration()>0)
				towerInfo+="Stun target for "+currentSelectedTower.GetStunDuration().ToString("f1")+"sec\n";
			
			//if the mine have damage over time value, display it
			Dot dot=currentSelectedTower.GetDot();
			float totalDot=dot.damage*(dot.duration/dot.interval);
			if(totalDot>0){
				string dotInfo="Cause "+totalDot.ToString("f1")+" damage over the next "+dot.duration+" sec\n";
				
				towerInfo+=dotInfo;
			}
			
			//if the mine have slow value, display it
			Slow slow=currentSelectedTower.GetSlow();
			if(slow.duration>0){
				string slowInfo="Slow target by "+(slow.slowFactor*100).ToString("f1")+"% for "+slow.duration.ToString("f1")+"sec\n";
				towerInfo+=slowInfo;
			}
		}
		else if(type==_TowerType.TurretTower || type==_TowerType.AOETower || type==_TowerType.DirectionalAOETower){
			
			//show the basic info for turret and aoeTower
			if(currentSelectedTower.GetDamage()>0)
				towerInfo+="Damage: "+currentSelectedTower.GetDamage().ToString("f1")+"\n";
			if(currentSelectedTower.GetCooldown()>0)
				towerInfo+="Cooldown: "+currentSelectedTower.GetCooldown().ToString("f1")+"sec\n";
			if(type==_TowerType.TurretTower && currentSelectedTower.GetAoeRadius()>0)
				towerInfo+="AOE Radius: "+currentSelectedTower.GetAoeRadius().ToString("f1")+"\n";
			if(currentSelectedTower.GetStunDuration()>0)
				towerInfo+="Stun target for "+currentSelectedTower.GetStunDuration().ToString("f1")+"sec\n";
			
			//if the tower have damage over time value, display it
			Dot dot=currentSelectedTower.GetDot();
			float totalDot=dot.damage*(dot.duration/dot.interval);
			if(totalDot>0){
				string dotInfo="Cause "+totalDot.ToString("f1")+" damage over the next "+dot.duration+" sec\n";
				
				towerInfo+=dotInfo;
			}
			
			//if the tower have slow value, display it
			Slow slow=currentSelectedTower.GetSlow();
			if(slow.duration>0){
				string slowInfo="Slow target by "+(slow.slowFactor*100).ToString("f1")+"% for "+slow.duration.ToString("f1")+"sec\n";
				towerInfo+=slowInfo;
			}
		}
		
		
		//show the tower's description
		towerInfo+="\n\n"+currentSelectedTower.description;
		
		GUIContent towerInfoContent=new GUIContent(towerInfo);
			
		//draw all the information on screen
		float contentHeight= GUI.skin.GetStyle("Label").CalcHeight(towerInfoContent, 200);
		GUI.Label(new Rect(startX, startY+=20, width, contentHeight), towerInfoContent);
	
		
		//reset the draw position
		startY=Screen.height-180-bottomPanelRect.height;
		if(enableTargetPrioritySwitch){
			if(currentSelectedTower.type==_TowerType.TurretTower || currentSelectedTower.type==_TowerType.DirectionalAOETower){
				if(currentSelectedTower.targetingArea!=_TargetingArea.StraightLine){
					GUI.Label(new Rect(startX, startY, 120, 30), "Targeting Priority:");
					if(GUI.Button(new Rect(startX+120, startY-3, 100, 30), currentSelectedTower.targetPriority.ToString())){
						if(currentSelectedTower.targetPriority==_TargetPriority.Nearest) currentSelectedTower.SetTargetPriority(1);
						else if(currentSelectedTower.targetPriority==_TargetPriority.Weakest) currentSelectedTower.SetTargetPriority(2);
						else if(currentSelectedTower.targetPriority==_TargetPriority.Toughest) currentSelectedTower.SetTargetPriority(3);
						else if(currentSelectedTower.targetPriority==_TargetPriority.Random) currentSelectedTower.SetTargetPriority(0);
					}
					startY+=30;
				}
			}
		}
		
		if(enableTargetDirectionSwitch){
			if(currentSelectedTower.type==_TowerType.TurretTower || currentSelectedTower.type==_TowerType.DirectionalAOETower){
				if(currentSelectedTower.targetingArea!=_TargetingArea.AllAround){
					GUI.changed = false;
					GUI.Label(new Rect(startX, startY, 120, 30), "Targeting Direction:");
					float direction=currentSelectedTower.targetingDirection;
					direction=GUI.HorizontalSlider(new Rect(startX+120, startY+4, 100, 30), direction, 0, 359F);
					currentSelectedTower.SetTargetingDirection(direction);
					if(GUI.changed) GameControl.ShowIndicator(currentSelectedTower);
				}
			}
		}

		//check if the tower can be upgrade
		bool upgradable=false;
		if(!currentSelectedTower.IsLevelCapped() && currentSelectedTower.IsBuilt()){
			upgradable=true;
		}
		
		//reset the draw position
		startY=Screen.height-50-bottomPanelRect.height;
		
		//if the tower is eligible to upgrade, draw the upgrade button
		if(upgradable){
			if(GUI.Button(new Rect(startX, startY, 100, 30), new GUIContent("Upgrade", "1"))){
				//upgrade the tower, if false is return, player have insufficient fund
				if(!currentSelectedTower.Upgrade()) Debug.Log("Insufficient Resource");
			}
		}
		//sell button
		if(currentSelectedTower.IsBuilt()){
			if(GUI.Button(new Rect(startX+110, startY, 100, 30), new GUIContent("Sell", "2"))){
				currentSelectedTower.Sell();
			}
		}
		
		//if the cursor is hover on the upgrade button, show the cost
		if(GUI.tooltip=="1"){
			Resource[] rscList=GameControl.GetResourceList();
			int[] cost=currentSelectedTower.GetCost();
			
			int count=0;
			foreach(int val in cost){
				if(val>0) count+=1;
			}
			
			startY-=1+count*25;
			//~ for(int i=0; i<3; i++) GUI.Box(new Rect(startX, startY, count*25-3, 150), "");
			count=0;
			for(int i=0; i<cost.Length; i++){
				if(cost[i]>0){
					if(rscList[i].icon!=null){
						GUI.Label(new Rect(startX+10, startY+count*20, 25, 25), rscList[i].icon);
						GUI.Label(new Rect(startX+10+25, startY+count*20+3, 150, 25), "- "+cost[i].ToString());
					}
					else{
						GUI.Label(new Rect(startX+10, startY+count*20, 150, 25), " - "+cost[i].ToString()+rscList[i].name);
					}
					count+=1;
				}
			}
		}
		//if the cursor is hover on the sell button, show the resource gain
		else if(GUI.tooltip=="2"){
			Resource[] rscList=GameControl.GetResourceList();
			int[] sellValue=currentSelectedTower.GetTowerSellValue();
			
			int count=0;
			foreach(int val in sellValue){
				if(val>0) count+=1;
			}
			
			startY-=1+count*25;
			count=0;
			for(int i=0; i<sellValue.Length; i++){
				if(sellValue[i]>0){
					if(rscList[i].icon!=null){
						GUI.Label(new Rect(startX+120, startY+count*20, 25, 25), rscList[i].icon);
						GUI.Label(new Rect(startX+120+25, startY+count*20+3, 150, 25), "+ "+sellValue[i].ToString());
					}
					else{
						GUI.Label(new Rect(startX+120, startY+count*20, 150, 25), " + "+sellValue[i].ToString()+rscList[i].name);
					}
					count+=1;
				}
			}
		}
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
