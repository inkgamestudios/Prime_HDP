using UnityEngine;
using System.Collections;

public class LevelSelect : MonoBehaviour 
{
	//Establishes button variables
	private UIButton Lvl0;
	private UIButton Tier1LvlA;
	private UIButton Tier1LvlB;
	private UIButton Tier1LvlC;
	private UIButton Tier2LvlA;
	private UIButton Tier2LvlB;
	private UIButton Tier3LvlA;
	private UIButton Tier3LvlB;
	private UIButton Tier3LvlC;
	private UIButton Tier4;
	private UIButton S2Lvl0;
	private UIButton S2Tier1LvlA;
	private UIButton S2Tier1LvlB;
	private UIButton S2Tier1LvlC;
	private UIButton S2Tier2LvlA;
	private UIButton S2Tier2LvlB;
	private UIButton S2Tier3LvlA;
	private UIButton S2Tier3LvlB;
	private UIButton S2Tier3LvlC;
	private UIButton S2Tier4;
	private UIButton S3Lvl0;
	private UIButton S3Tier1LvlA;
	private UIButton S3Tier1LvlB;
	private UIButton S3Tier1LvlC;
	private UIButton S3Tier2LvlA;
	private UIButton S3Tier2LvlB;
	private UIButton S3Tier3LvlA;
	private UIButton S3Tier3LvlB;
	private UIButton S3Tier3LvlC;
	private UIButton S3Tier4;
	//Establishing vertical panel variables
	private UIVerticalPanel vertPanel0;
	private UIVerticalPanel vertPanel1;
	private UIVerticalPanel vertPanel2;
	private UIVerticalPanel vertPanel3;
	private UIVerticalPanel vertPanel4;
	private UIVerticalPanel vertPanel5;
	private UIVerticalPanel vertPanel6;
	private UIVerticalPanel vertPanel7;
	private UIVerticalPanel vertPanel8;
	private UIVerticalPanel vertPanel9;
	private UIVerticalPanel vertPanel10;
	private UIVerticalPanel vertPanel11;
	private UIVerticalPanel vertPanel12;
	private UIVerticalPanel vertPanel13;
	private UIVerticalPanel vertPanel14;
			
	void Start()
	{
		//Stage 1 - 3 buttons created
		var Stage1Tab = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
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
		Stage3Tab.onTouchUpInside += sender => Debug.Log("stag1e 3 tab clicked");
		Stage3Tab.onTouchUpInside += sender => SwitchFunction(3);
	}
	
	void Buttons()
	{	
		// Very First Level
		Lvl0 = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
		Lvl0.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Lvl0.onTouchUpInside += sender => Application.LoadLevel ("Lvl0");
		
		//First Tier
		Tier1LvlA = UIButton.create( "playUp.png", "playDown.png", 0, 0  );
		Tier1LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier1LvlA.onTouchUpInside += sender => Application.LoadLevel ("Tier1LvlA");
		
		Tier1LvlB = UIButton.create( "playUp.png", "playDown.png", 0, 0  );
		Tier1LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier1LvlB.onTouchUpInside += sender => Application.LoadLevel ("Tier1LvlB");
		
		Tier1LvlC = UIButton.create( "playUp.png", "playDown.png", 0, 0  );
		Tier1LvlC.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier1LvlC.onTouchUpInside += sender => Application.LoadLevel ("Tier1LvlC");
		//Second Tier
		Tier2LvlA = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
		Tier2LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier2LvlA.onTouchUpInside += sender => Application.LoadLevel ("Tier2LvlA");
		
		Tier2LvlB = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
		Tier2LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier2LvlB.onTouchUpInside += sender => Application.LoadLevel ("Tier2LvlB");
		
		//Third Tier
		Tier3LvlA = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
		Tier3LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier3LvlA.onTouchUpInside += sender => Application.LoadLevel ("Tier3LvlA");
		
		Tier3LvlB = UIButton.create( "playUp.png", "playDown.png", 0, 0  );
		Tier3LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier3LvlB.onTouchUpInside += sender => Application.LoadLevel ("Tier3LvlB");
		
		Tier3LvlC = UIButton.create( "playUp.png", "playDown.png", 0, 0  );
		Tier3LvlC.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier3LvlC.onTouchUpInside += sender => Application.LoadLevel ("Tier3LvlC");
		//Fourth Tier
		Tier4 = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
		Tier4.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		Tier4.onTouchUpInside += sender => Application.LoadLevel ("Tier4");
		
		//Establishes vertical panels
		vertPanel0 = UIVerticalPanel.create( "vertPanelTop.png", "vertPanelMiddle.png", "vertPanelBottom.png" );
		vertPanel0.beginUpdates();
		vertPanel0.spacing = 20;
		vertPanel0.edgeInsets = new UIEdgeInsets( 30, 10, 20, 10 );
		vertPanel0.addChild( Lvl0 );
		vertPanel0.endUpdates();
		
		vertPanel1 = UIVerticalPanel.create( "vertPanelTop.png", "vertPanelMiddle.png", "vertPanelBottom.png" );
		vertPanel1.beginUpdates();
		vertPanel1.spacing = 20;
		vertPanel1.edgeInsets = new UIEdgeInsets( 30, 10, 20, 10 );
		vertPanel1.addChild( Tier1LvlA, Tier1LvlB, Tier1LvlC );
		vertPanel1.endUpdates();
		
		vertPanel2 = UIVerticalPanel.create( "vertPanelTop.png", "vertPanelMiddle.png", "vertPanelBottom.png" );
		vertPanel2.beginUpdates();
		vertPanel2.spacing = 20;
		vertPanel2.edgeInsets = new UIEdgeInsets( 30, 10, 20, 10 );
		vertPanel2.addChild( Tier2LvlA, Tier2LvlB);
		vertPanel2.endUpdates();
		
		vertPanel3 = UIVerticalPanel.create( "vertPanelTop.png", "vertPanelMiddle.png", "vertPanelBottom.png" );
		vertPanel3.beginUpdates();
		vertPanel3.spacing = 20;
		vertPanel3.edgeInsets = new UIEdgeInsets( 30, 10, 20, 10 );
		vertPanel3.addChild( Tier3LvlA, Tier3LvlB, Tier3LvlC );
		vertPanel3.endUpdates();
		
		vertPanel4 = UIVerticalPanel.create( "vertPanelTop.png", "vertPanelMiddle.png", "vertPanelBottom.png" );
		vertPanel4.beginUpdates();
		vertPanel4.spacing = 20;
		vertPanel4.edgeInsets = new UIEdgeInsets( 30, 10, 20, 10 );
		vertPanel4.addChild( Tier4 );
		vertPanel4.endUpdates();
		//
		//Divide between first and second stages
		//
		//Establishes buttons for 2nd stage
		//First lvl of 2nd stage
		S2Lvl0 = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Lvl0.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Lvl0.onTouchUpInside += sender => Application.LoadLevel ("S2Lvl0");
		//First Tier
		S2Tier1LvlA = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier1LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier1LvlA.onTouchUpInside += sender => Application.LoadLevel ("S2Tier1LvlA");
		
		S2Tier1LvlB = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier1LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier1LvlB.onTouchUpInside += sender => Application.LoadLevel ("S2Tier1LvlB");
		
		S2Tier1LvlC = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier1LvlC.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier1LvlC.onTouchUpInside += sender => Application.LoadLevel ("S2Tier1LvlC");
		//Second Tier
		S2Tier2LvlA= UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier2LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier2LvlA.onTouchUpInside += sender => Application.LoadLevel ("S2Tier2LvlA");
		
		S2Tier2LvlB = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier2LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier2LvlB.onTouchUpInside += sender => Application.LoadLevel ("S2Tier2LvlB");
		//Third Tier
		S2Tier3LvlA = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier3LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier3LvlA.onTouchUpInside += sender => Application.LoadLevel ("S2Tier3LvlA");
		
		S2Tier3LvlB = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier3LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier3LvlB.onTouchUpInside += sender => Application.LoadLevel ("S2Tier3LvlB");
		
		S2Tier3LvlC = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier3LvlC.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier3LvlC.onTouchUpInside += sender => Application.LoadLevel ("S2Tier3LvlC");
		//Fourth Tier
		S2Tier4 = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		S2Tier4.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S2Tier4.onTouchUpInside += sender => Application.LoadLevel ("S2Tier4");
		//Establishes vertical panels
		vertPanel5 = UIVerticalPanel.create( "vertPanelTop.png", "vertPanelMiddle.png", "vertPanelBottom.png" );
		vertPanel5.beginUpdates();
		vertPanel5.spacing = 20;
		vertPanel5.edgeInsets = new UIEdgeInsets( 30, 10, 20, 10 );
		vertPanel5.addChild( S2Lvl0 );
		vertPanel5.endUpdates();
		
		vertPanel6 = UIVerticalPanel.create( "vertPanelTop.png", "vertPanelMiddle.png", "vertPanelBottom.png" );
		vertPanel6.beginUpdates();
		vertPanel6.spacing = 20;
		vertPanel6.edgeInsets = new UIEdgeInsets( 30, 10, 20, 10 );
		vertPanel6.addChild( S2Tier1LvlA, S2Tier1LvlB, S2Tier1LvlC );
		vertPanel6.endUpdates();
		
		vertPanel7 = UIVerticalPanel.create( "vertPanelTop.png", "vertPanelMiddle.png", "vertPanelBottom.png" );
		vertPanel7.beginUpdates();
		vertPanel7.spacing = 20;
		vertPanel7.edgeInsets = new UIEdgeInsets( 30, 10, 20, 10 );
		vertPanel7.addChild( S2Tier2LvlA, S2Tier2LvlB);
		vertPanel7.endUpdates();
		
		vertPanel8 = UIVerticalPanel.create( "vertPanelTop.png", "vertPanelMiddle.png", "vertPanelBottom.png" );
		vertPanel8.beginUpdates();
		vertPanel8.spacing = 20;
		vertPanel8.edgeInsets = new UIEdgeInsets( 30, 10, 20, 10 );
		vertPanel8.addChild( S2Tier3LvlA, S2Tier3LvlB, S2Tier3LvlC );
		vertPanel8.endUpdates();
		
		vertPanel9 = UIVerticalPanel.create( "vertPanelTop.png", "vertPanelMiddle.png", "vertPanelBottom.png" );
		vertPanel9.beginUpdates();
		vertPanel9.spacing = 20;
		vertPanel9.edgeInsets = new UIEdgeInsets( 30, 10, 20, 10 );
		vertPanel9.addChild( S2Tier4 );
		vertPanel9.endUpdates();
		//
		//Divide between 2nd and 3rd stages
		//
		//Establishes buttons for Third Stage
		//First level of the Third Stage
		S3Lvl0 = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Lvl0.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Lvl0.onTouchUpInside += sender => Application.LoadLevel ("S3Lvl0");
		//First Tier
		S3Tier1LvlA = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier1LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier1LvlA.onTouchUpInside += sender => Application.LoadLevel ("S3Tier1LvlA");
		
		S3Tier1LvlB = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier1LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier1LvlB.onTouchUpInside += sender => Application.LoadLevel ("S3Tier1LvlB");
		
		S3Tier1LvlC = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier1LvlC.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier1LvlC.onTouchUpInside += sender => Application.LoadLevel ("S3Tier1LvlC");
		//Second Tier
		S3Tier2LvlA = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier2LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier2LvlA.onTouchUpInside += sender => Application.LoadLevel ("S3Tier2LvlA");
		
		S3Tier2LvlB = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier2LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier2LvlB.onTouchUpInside += sender => Application.LoadLevel ("S3Tier2LvlB");
		//Third Tier
		S3Tier3LvlA = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier3LvlA.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier3LvlA.onTouchUpInside += sender => Application.LoadLevel ("S3Tier3LvlA");
		
		S3Tier3LvlB = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier3LvlB.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier3LvlB.onTouchUpInside += sender => Application.LoadLevel ("S3Tier3LvlB");
		
		S3Tier3LvlC = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier3LvlC.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier3LvlC.onTouchUpInside += sender => Application.LoadLevel ("S3Tier3LvlC");
		//Fourth Tier
		S3Tier4 = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		S3Tier4.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		S3Tier4.onTouchUpInside += sender => Application.LoadLevel ("S3Tier4");
		//Establishes vertical panels
		vertPanel10 = UIVerticalPanel.create( "vertPanelTop.png", "vertPanelMiddle.png", "vertPanelBottom.png" );
		vertPanel10.beginUpdates();
		vertPanel10.spacing = 20;
		vertPanel10.edgeInsets = new UIEdgeInsets( 30, 10, 20, 10 );
		vertPanel10.addChild( S3Lvl0 );
		vertPanel10.endUpdates();
		
		vertPanel11 = UIVerticalPanel.create( "vertPanelTop.png", "vertPanelMiddle.png", "vertPanelBottom.png" );
		vertPanel11.beginUpdates();
		vertPanel11.spacing = 20;
		vertPanel11.edgeInsets = new UIEdgeInsets( 30, 10, 20, 10 );
		vertPanel11.addChild( S3Tier1LvlA, S3Tier1LvlB, S3Tier1LvlC );
		vertPanel11.endUpdates();
		
		vertPanel12 = UIVerticalPanel.create( "vertPanelTop.png", "vertPanelMiddle.png", "vertPanelBottom.png" );
		vertPanel12.beginUpdates();
		vertPanel12.spacing = 20;
		vertPanel12.edgeInsets = new UIEdgeInsets( 30, 10, 20, 10 );
		vertPanel12.addChild( S3Tier2LvlA, S3Tier2LvlB);
		vertPanel12.endUpdates();
		
		vertPanel13 = UIVerticalPanel.create( "vertPanelTop.png", "vertPanelMiddle.png", "vertPanelBottom.png" );
		vertPanel13.beginUpdates();
		vertPanel13.spacing = 20;
		vertPanel13.edgeInsets = new UIEdgeInsets( 30, 10, 20, 10 );
		vertPanel13.addChild( S3Tier3LvlA, S3Tier3LvlB, S3Tier3LvlC );
		vertPanel13.endUpdates();
		
		vertPanel14 = UIVerticalPanel.create( "vertPanelTop.png", "vertPanelMiddle.png", "vertPanelBottom.png" );
		vertPanel14.beginUpdates();
		vertPanel14.spacing = 20;
		vertPanel14.edgeInsets = new UIEdgeInsets( 30, 10, 20, 10 );
		vertPanel14.addChild( S3Tier4 );
		vertPanel14.endUpdates();
		
		SwitchFunction( 1 );
		
	}
	void SwitchFunction ( int tabNumber)
	{
		switch( tabNumber )
			{
			case 2:
				//Tab 2
				//Horizontal Alignment of the vertical panels
				vertPanel5.position = new Vector2( 0, 0 );
				vertPanel6.position = new Vector2( 1* Screen.width/5, 0 );
				vertPanel7.position = new Vector2( 2* Screen.width/5, 0 );
				vertPanel8.position = new Vector2( 3* Screen.width/5, 0 );
				vertPanel9.position = new Vector2( 4* Screen.width/5, 0 );
				//repositioning other tabs
				vertPanel0.position = new Vector2( 0, Screen.height );
				vertPanel1.position = new Vector2( 0, Screen.height );
				vertPanel2.position = new Vector2( 0, Screen.height );
				vertPanel3.position = new Vector2( 0, Screen.height );
				vertPanel4.position = new Vector2( 0, Screen.height );
				vertPanel10.position = new Vector2( 0, Screen.height );
				vertPanel11.position = new Vector2( 0, Screen.height );
				vertPanel12.position = new Vector2( 0, Screen.height );
				vertPanel13.position = new Vector2( 0, Screen.height );
				vertPanel14.position = new Vector2( 0, Screen.height );
				
				break;
				
			case 3:
				//Tab 3
				//Horizontal Alignment of the vertical panels
				vertPanel10.position = new Vector2( 0, 0 );
				vertPanel11.position = new Vector2( 1* Screen.width/5, 0 );
				vertPanel12.position = new Vector2( 2* Screen.width/5, 0 );
				vertPanel13.position = new Vector2( 3* Screen.width/5, 0 );
				vertPanel14.position = new Vector2( 4* Screen.width/5, 0 );
				//repositioning other tabs
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
				
				break;
				
			case 1:
			default:
				//Tab 1
				//Horizontal Alignment of the vertical panels
				vertPanel0.position = new Vector2( 0, 0 );
				vertPanel1.position = new Vector2( 1* Screen.width/5, 0 );
				vertPanel2.position = new Vector2( 2* Screen.width/5, 0 );
				vertPanel3.position = new Vector2( 3* Screen.width/5, 0 );
				vertPanel4.position = new Vector2( 4* Screen.width/5, 0 );
				//repositioning other tabs
				vertPanel5.position  = new Vector2( 0, Screen.height );
				vertPanel6.position  = new Vector2( 0, Screen.height );
				vertPanel7.position  = new Vector2( 0, Screen.height );
				vertPanel8.position  = new Vector2( 0, Screen.height );
				vertPanel9.position  = new Vector2( 0, Screen.height );
				vertPanel10.position  = new Vector2( 0, Screen.height );
				vertPanel11.position  = new Vector2( 0, Screen.height );
				vertPanel12.position  = new Vector2( 0, Screen.height );
				vertPanel13.position  = new Vector2( 0, Screen.height );
				vertPanel14.position  = new Vector2( 0, Screen.height );
				
				break;
			}
	}
}