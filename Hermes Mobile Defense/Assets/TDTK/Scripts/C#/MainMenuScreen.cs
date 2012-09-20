using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {
	private bool SoundOn = true;
	
	// Use this for initialization
	void Start () {
		
		// left vertical buttons
		var Start = UIButton.create("", "", 0, 0 );
		var Encyclopedia = UIButton.create("", "", 0, 0 );
		var HighScores = UIButton.create("", "", 0, 0 );
		var Credits = UIButton.create("", "", 0, 0 );
		
		// button commands
		Start.onTouchUpInside += sender => Application.LoadLevel("");
		Encyclopedia.onTouchUpInside += sender => Application.LoadLevel("");
		HighScores.onTouchUpInside += sender => Application.LoadLevel("");
		Credits.onTouchUpInside += sender => Application.LoadLevel("");
		
		// art box on the right
		var Art = UIButton.create("", "", 0, 0 );
		Art.position = new Vector2( Screen.width/2, Screen.width/2 - Art.height/2);
		
		// panel for the buttons
		var vertpanel = new UIVerticalLayout(20);
		vertpanel.addChild( Start, Encyclopedia, HighScores, Credits );
		vertpanel.position = new Vector2( 0, Screen.height/2 - vertpanel.height/2 );
		
		// text elements
		var text = new UIText( "neuropol", "neuropol.png" );
		var Title = text.addTextInstance( "Hermes Defense Protocol", 0, 0 );
		Title.position = new Vector2( Screen.width/2 - Title.width/2, 5 );
		
		// twatter
		var Twatter = UIButton.create("", "", 0, 0 );
		Twatter.onTouchUpInside += sender => /* load twatter script */ Application.LoadLevel("");
		// sound
		var Sound = UIButton.create("", "", 0, 0 );
		Sound.onTouchUpInside += sender => SoundToggle();
	}
	
	// method to turn sound off and on
	void SoundToggle()
	{
		SoundOn = !SoundOn;
		if(!SoundOn)
		{
			// turn sound off
		}
		else{
			// turn sound on
		}
	}
}
