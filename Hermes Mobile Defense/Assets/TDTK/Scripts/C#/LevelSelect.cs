using UnityEngine;
using System.Collections;

public class LevelSelect : MonoBehaviour 
{
	//Establishes button variables
	// tutorial
	private UIButton Lvl0;
	// Stage 1
	private UIButton Tier1LvlA;
	private UIButton Tier1LvlB;
	private UIButton Tier2LvlA;
	private UIButton Tier2LvlB;
	private UIButton Tier2LvlC;
	private UIButton Tier3LvlA;
	private UIButton Tier3LvlB;
	private UIButton Tier4LvlA;
	private UIButton Tier4LvlB;
	private UIButton Tier4LvlC;
	private UIButton Tier5;
	// Stage 2
	private UIButton S2Tier1LvlA;
	private UIButton S2Tier1LvlB;
	private UIButton S2Tier2LvlA;
	private UIButton S2Tier2LvlB;
	private UIButton S2Tier2LvlC;
	private UIButton S2Tier3LvlA;
	private UIButton S2Tier3LvlB;
	private UIButton S2Tier4LvlA;
	private UIButton S2Tier4LvlB;
	private UIButton S2Tier4LvlC;	
	private UIButton S2Tier5;
	// Stage 3
	private UIButton S3Tier1LvlA;
	private UIButton S3Tier1LvlB;
	private UIButton S3Tier2LvlA;
	private UIButton S3Tier2LvlB;
	private UIButton S3Tier2LvlC;
	private UIButton S3Tier3LvlA;
	private UIButton S3Tier3LvlB;
	private UIButton S3Tier4LvlA;
	private UIButton S3Tier4LvlB;
	private UIButton S3Tier4LvlC;
	private UIButton S3Tier5;
	// Establishing vertical panel variables
	// vert panels for Stage 1
	private UIVerticalLayout vertPanel0;
	private UIVerticalLayout vertPanel1;
	private UIVerticalLayout vertPanel2;
	private UIVerticalLayout vertPanel3;
	private UIVerticalLayout vertPanel4;
	private UIVerticalLayout vertPanel5;
	// vert panels for Stage 2
	private UIVerticalLayout vertPanel6;
	private UIVerticalLayout vertPanel7;
	private UIVerticalLayout vertPanel8;
	private UIVerticalLayout vertPanel9;
	private UIVerticalLayout vertPanel10;
	// vert panels for Stage 3
	private UIVerticalLayout vertPanel11;
	private UIVerticalLayout vertPanel12;
	private UIVerticalLayout vertPanel13;
	private UIVerticalLayout vertPanel14;
	private UIVerticalLayout vertPanel15;
			
	void Start()
	{
		//Stage 1 - 3 buttons created
		var Stage1Tab = UIButton.create( "playUp.png", "playUp.png", 0, 0 );
		var Stage2Tab = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
		var Stage3Tab = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
		
		Stage1Tab.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Stage2Tab.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Stage3Tab.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		//Stage 1 - 3 positioning
		Stage1Tab.position = new Vector2( 0, 0 );
		Stage2Tab.position = new Vector2( 1* Screen.width/3, 0 );
		Stage3Tab.position = new Vector2( 2* Screen.width/3, 0 );
		//Instantiates buttons
		Buttons();
		//Confirmation that buttons were pushed
		Stage1Tab.onTouchUpInside += sender => Debug.Log("stage 1 tab clicked");
		Stage1Tab.onTouchUpInside += sender => SwitchFunction(1);
		Stage2Tab.onTouchUpInside += sender => Debug.Log("stage 2 tab clicked");
		Stage2Tab.onTouchUpInside += sender => SwitchFunction(2);
		Stage3Tab.onTouchUpInside += sender => Debug.Log("stage 3 tab clicked");
		Stage3Tab.onTouchUpInside += sender => SwitchFunction(3);
	}
	
	void Buttons()
	{	
		// creates a gameObject of the Save object to gain access to the scrip PlayerSave located in it
		GameObject joe = GameObject.Find("Save");
		
		// Very First Level
		Lvl0 = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
		Lvl0.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Lvl0.onTouchUpInside += sender => Application.LoadLevel ("Lvl0");
		Lvl0.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 1, 0 );
		
		//First Tier
		Tier1LvlA = UIButton.create( "playUp.png", "playDown.png", 0, 0  );
		Tier1LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier1LvlA.onTouchUpInside += sender => Application.LoadLevel ("Tier1LvlA");
		Tier1LvlA.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 1, 5 );
		
		Tier1LvlB = UIButton.create( "playUp.png", "playDown.png", 0, 0  );
		Tier1LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier1LvlB.onTouchUpInside += sender => Application.LoadLevel ("Tier1LvlB");
		Tier1LvlB.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 1, 10 );
		
		//Second Tier
		Tier2LvlA = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
		Tier2LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier2LvlA.onTouchUpInside += sender => Application.LoadLevel ("Tier2LvlA");
		Tier2LvlA.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 1, 15 );
		
		Tier2LvlB = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
		Tier2LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier2LvlB.onTouchUpInside += sender => Application.LoadLevel ("Tier2LvlB");
		Tier2LvlB.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 1, 20 );
		
		Tier2LvlC = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
		Tier2LvlC.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier2LvlC.onTouchUpInside += sender => Application.LoadLevel ("Tier2LvlC");
		Tier2LvlC.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 1, 25 );
		
		//Third Tier
		Tier3LvlA = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
		Tier3LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier3LvlA.onTouchUpInside += sender => Application.LoadLevel ("Tier3LvlA");
		Tier3LvlA.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 1, 30 );
		
		Tier3LvlB = UIButton.create( "playUp.png", "playDown.png", 0, 0  );
		Tier3LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier3LvlB.onTouchUpInside += sender => Application.LoadLevel ("Tier3LvlB");
		Tier3LvlB.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 1, 35 );
		
		//Fourth Tier
		Tier4LvlA = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
		Tier4LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier4LvlA.onTouchUpInside += sender => Application.LoadLevel ("Tier4LvlA");
		Tier4LvlA.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 1, 40 );
		
		Tier4LvlB = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
		Tier4LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier4LvlB.onTouchUpInside += sender => Application.LoadLevel ("Tier4LvlB");
		Tier4LvlB.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 1, 45 );
		
		Tier4LvlC = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
		Tier4LvlC.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier4LvlC.onTouchUpInside += sender => Application.LoadLevel ("Tier4LvlC");
		Tier4LvlC.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 1, 50 );
		
		//5th Tier boss
		Tier5 = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
		Tier5.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier5.onTouchUpInside += sender => Application.LoadLevel ("Tier5");
		Tier5.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 1, 55 );
		
		//Establishes vertical panels
		vertPanel0 = new UIVerticalLayout(20);
		vertPanel0.addChild( Lvl0 );
		
		vertPanel1 = new UIVerticalLayout(20);
		vertPanel1.addChild( Tier1LvlA, Tier1LvlB );
		
		vertPanel2 = new UIVerticalLayout(20);
		vertPanel2.addChild( Tier2LvlA, Tier2LvlB, Tier2LvlC );

		vertPanel3 = new UIVerticalLayout(20);
		vertPanel3.addChild( Tier3LvlA, Tier3LvlB );

		vertPanel4 = new UIVerticalLayout(20);
		vertPanel4.addChild( Tier4LvlA, Tier4LvlB, Tier4LvlC );
		
		vertPanel5 = new UIVerticalLayout(20);
		vertPanel5.addChild( Tier5 );
		
		//
		//Divide between first and second stages
		//
		//Establishes buttons for 2nd stage
		//First lvl of 2nd stage
		//First Tier
		S2Tier1LvlA = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier1LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier1LvlA.onTouchUpInside += sender => Application.LoadLevel ("S2Tier1LvlA");
		S2Tier1LvlA.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 2, 0 );
		
		S2Tier1LvlB = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier1LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier1LvlB.onTouchUpInside += sender => Application.LoadLevel ("S2Tier1LvlB");
		S2Tier1LvlB.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 2, 5 );
		
		//Second Tier
		S2Tier2LvlA= UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier2LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier2LvlA.onTouchUpInside += sender => Application.LoadLevel ("S2Tier2LvlA");
		S2Tier2LvlA.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 2, 10 );
		
		S2Tier2LvlB = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier2LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier2LvlB.onTouchUpInside += sender => Application.LoadLevel ("S2Tier2LvlB");
		S2Tier2LvlB.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 2, 15 );
		
		S2Tier2LvlC = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier2LvlC.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier2LvlC.onTouchUpInside += sender => Application.LoadLevel ("S2Tier2LvlC");
		S2Tier2LvlC.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 2, 20 );
		
		//Third Tier
		S2Tier3LvlA = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier3LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier3LvlA.onTouchUpInside += sender => Application.LoadLevel ("S2Tier3LvlA");
		S2Tier3LvlA.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 2, 25 );
		
		S2Tier3LvlB = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier3LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier3LvlB.onTouchUpInside += sender => Application.LoadLevel ("S2Tier3LvlB");
		S2Tier3LvlB.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 2, 30 );
		
		//Fourth Tier
		S2Tier4LvlA= UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier4LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier4LvlA.onTouchUpInside += sender => Application.LoadLevel ("S2Tier4LvlA");
		S2Tier4LvlA.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 2, 35 );
		
		S2Tier4LvlB = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier4LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier4LvlB.onTouchUpInside += sender => Application.LoadLevel ("S2Tier4LvlB");
		S2Tier4LvlB.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 2, 40 );
		
		S2Tier4LvlC = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier4LvlC.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier4LvlC.onTouchUpInside += sender => Application.LoadLevel ("S2Tier4LvlC");
		S2Tier4LvlC.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 2, 45 );
		
		//5th Tier boss
		S2Tier5 = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier5.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier5.onTouchUpInside += sender => Application.LoadLevel ("S2Tier5");
		S2Tier5.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 2, 50 );
		
		//Establishes vertical panels
		vertPanel6 = new UIVerticalLayout(20);
		vertPanel6.addChild( S2Tier1LvlA, S2Tier1LvlB );

		vertPanel7 = new UIVerticalLayout(20);
		vertPanel7.addChild( S2Tier2LvlA, S2Tier2LvlB, S2Tier2LvlC );
		
		vertPanel8 = new UIVerticalLayout(20);
		vertPanel8.addChild( S2Tier3LvlA, S2Tier3LvlB);
		
		vertPanel9 = new UIVerticalLayout(20);
		vertPanel9.addChild( S2Tier4LvlA, S2Tier4LvlB, S2Tier4LvlC );
		
		vertPanel10 = new UIVerticalLayout(20);
		vertPanel10.addChild( S2Tier5 );
		
		//
		//Divide between 2nd and 3rd stages
		//
		//Establishes buttons for Third Stage
		//First level of the Third Stage
		//First Tier
		S3Tier1LvlA = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier1LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier1LvlA.onTouchUpInside += sender => Application.LoadLevel ("S3Tier1LvlA");
		S3Tier1LvlA.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 3, 0 );
		
		S3Tier1LvlB = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier1LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier1LvlB.onTouchUpInside += sender => Application.LoadLevel ("S3Tier1LvlB");
		S3Tier1LvlB.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 3, 5 );

		//Second Tier
		S3Tier2LvlA = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier2LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier2LvlA.onTouchUpInside += sender => Application.LoadLevel ("S3Tier2LvlA");
		S3Tier2LvlA.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 3, 10 );
		
		S3Tier2LvlB = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier2LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier2LvlB.onTouchUpInside += sender => Application.LoadLevel ("S3Tier2LvlB");
		S3Tier2LvlB.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 3, 15 );
		
		S3Tier2LvlC = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier2LvlC.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier2LvlC.onTouchUpInside += sender => Application.LoadLevel ("S3Tier2LvlC");
		S3Tier2LvlC.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 3, 20 );
		
		//Third Tier
		S3Tier3LvlA = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier3LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier3LvlA.onTouchUpInside += sender => Application.LoadLevel ("S3Tier3LvlA");
		S3Tier3LvlA.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 3, 25 );
		
		S3Tier3LvlB = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier3LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier3LvlB.onTouchUpInside += sender => Application.LoadLevel ("S3Tier3LvlB");
		S3Tier3LvlB.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 3, 30 );

		//Fourth Tier
		S3Tier4LvlA = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier4LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier4LvlA.onTouchUpInside += sender => Application.LoadLevel ("S3Tier4LvlA");
		S3Tier4LvlA.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 3, 35 );
		
		S3Tier4LvlB = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier4LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier4LvlB.onTouchUpInside += sender => Application.LoadLevel ("S3Tier4LvlB");
		S3Tier4LvlB.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 3, 40 );
		
		S3Tier4LvlC = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier4LvlC.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier4LvlC.onTouchUpInside += sender => Application.LoadLevel ("S3Tier4LvlC");
		S3Tier4LvlC.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 3, 45 );
		
		//5th Tier boss
		S3Tier5 = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier5.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier5.onTouchUpInside += sender => Application.LoadLevel ("S3Tier5");
		S3Tier5.onTouchUpInside += sender => joe.GetComponent<PlayerSave>().SetStage( 3, 50 );
		
		//Establishes vertical panels
		vertPanel11 = new UIVerticalLayout(20);
		vertPanel11.addChild( S3Tier1LvlA, S3Tier1LvlB );
		
		vertPanel12 = new UIVerticalLayout(20);
		vertPanel12.addChild( S3Tier2LvlA, S3Tier2LvlB, S3Tier2LvlC );

		vertPanel13 = new UIVerticalLayout(20);
		vertPanel13.addChild( S3Tier3LvlA, S3Tier3LvlB );

		vertPanel14 = new UIVerticalLayout(20);
		vertPanel14.addChild( S3Tier4LvlA, S3Tier4LvlB, S3Tier4LvlC );
		
		vertPanel15 = new UIVerticalLayout(20);
		vertPanel15.addChild( S3Tier5 );
		
		// sets default value of arranging buttons
		SwitchFunction( 1 );
		
	}
	
	void SwitchFunction ( int tabNumber)
	{
		switch( tabNumber )
			{
			case 2:
				//Tab 2
				//Horizontal Alignment of the vertical panels
				vertPanel6.position = new Vector2( 0, -Screen.height/2 );
				vertPanel7.position = new Vector2( 1* Screen.width/5, -Screen.height/2 );
				vertPanel8.position = new Vector2( 2* Screen.width/5, -Screen.height/2 );
				vertPanel9.position = new Vector2( 3* Screen.width/5, -Screen.height/2 );
				vertPanel10.position = new Vector2( 4* Screen.width/5, -Screen.height/2 );
			
				//repositioning other tabs out of view
				vertPanel0.position = new Vector2( 0, Screen.height );
				vertPanel1.position = new Vector2( 0, Screen.height );
				vertPanel2.position = new Vector2( 0, Screen.height );
				vertPanel3.position = new Vector2( 0, Screen.height );
				vertPanel4.position = new Vector2( 0, Screen.height );
				vertPanel5.position = new Vector2( 0, Screen.height );
			
				vertPanel11.position = new Vector2( 0, Screen.height );
				vertPanel12.position = new Vector2( 0, Screen.height );
				vertPanel13.position = new Vector2( 0, Screen.height );
				vertPanel14.position = new Vector2( 0, Screen.height );
				vertPanel15.position  = new Vector2( 0, Screen.height );
				
				break;
			
			case 3:
				//Tab 3
				//Horizontal Alignment of the vertical panels
				vertPanel11.position = new Vector2( 0, -Screen.height/2 );
				vertPanel12.position = new Vector2( 1* Screen.width/5, -Screen.height/2 );
				vertPanel13.position = new Vector2( 2* Screen.width/5, -Screen.height/2 );
				vertPanel14.position = new Vector2( 3* Screen.width/5, -Screen.height/2 );
				vertPanel15.position = new Vector2( 4* Screen.width/5, -Screen.height/2 );
			
				//repositioning other tabs out of view
				vertPanel0.position  = new Vector2( 0, Screen.height );
				vertPanel1.position  = new Vector2( 0, Screen.height );
				vertPanel2.position  = new Vector2( 0, Screen.height );
				vertPanel3.position  = new Vector2( 0, Screen.height );
				vertPanel4.position  = new Vector2( 0, Screen.height );
				vertPanel5.position  = new Vector2( 0, Screen.height );
			
				vertPanel6.position  = new Vector2( 0, Screen.height );
				vertPanel7.position  = new Vector2( 0, Screen.height );
				vertPanel8.position  = new Vector2( 0, Screen.height );
				vertPanel9.position  = new Vector2( 0, Screen.height );
				vertPanel10.position = new Vector2( 0, Screen.height );
				
				break;
				
			case 1:
			default:
				//Tab 1
				//Horizontal Alignment of the vertical panels
				vertPanel0.position = new Vector2( 0, -Screen.height/2 );
				vertPanel1.position = new Vector2( 1* Screen.width/6, -Screen.height/2 );
				vertPanel2.position = new Vector2( 2* Screen.width/6, -Screen.height/2 );
				vertPanel3.position = new Vector2( 3* Screen.width/6, -Screen.height/2 );
				vertPanel4.position = new Vector2( 4* Screen.width/6, -Screen.height/2 );
				vertPanel5.position = new Vector2( 5* Screen.width/6, -Screen.height/2 );
			
				//repositioning other tabs out of view
				vertPanel6.position  = new Vector2( 0, Screen.height );
				vertPanel7.position  = new Vector2( 0, Screen.height );
				vertPanel8.position  = new Vector2( 0, Screen.height );
				vertPanel9.position  = new Vector2( 0, Screen.height );
				vertPanel10.position  = new Vector2( 0, Screen.height );
			
				vertPanel11.position  = new Vector2( 0, Screen.height );
				vertPanel12.position  = new Vector2( 0, Screen.height );
				vertPanel13.position  = new Vector2( 0, Screen.height );
				vertPanel14.position  = new Vector2( 0, Screen.height );
				vertPanel15.position  = new Vector2( 0, Screen.height );
				
				break;
			}
	}
}