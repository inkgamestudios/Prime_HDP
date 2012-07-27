using UnityEngine;
using System.Collections;


public class Menu : MonoBehaviour
{
	void Start()
	{
		// IMPORTANT: depth is 1 on top higher numbers on the bottom.  This means the lower the number is the closer it gets to the camera.
		var playButton = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
		playButton.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		playButton.onTouchUpInside += sender => Application.LoadLevel ("LevelSelect");
		
		var highScores = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		highScores.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		highScores.onTouchUpInside += sender => Application.LoadLevel ("HighScores");
		
		var settings = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		settings.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		settings.onTouchUpInside += sender => Application.LoadLevel ("Settings");
		
		var vertPanel = UIVerticalPanel.create( "vertPanelTop.png", "vertPanelMiddle.png", "vertPanelBottom.png" );
		vertPanel.beginUpdates();
		vertPanel.spacing = 20;
		vertPanel.edgeInsets = new UIEdgeInsets ( 30, 10, 20, 10 );
		vertPanel.addChild( playButton, highScores, settings );
		vertPanel.endUpdates();
		
		vertPanel.positionCenter();
		
		
	}
	

}
