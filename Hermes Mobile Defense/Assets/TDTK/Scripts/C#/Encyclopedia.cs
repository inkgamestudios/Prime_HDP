using UnityEngine;
using System.Collections;

public class Encyclopedia : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// default info
		InfoSwitch(-1);
		// establishes vertical scrolling buttons up and down states
		string[] upStates = new string[] { "", "", "", "" };
		string[] downStates = new string[] {"", "", "", "" };
		
		
		var scrollableButtons = new UIScrollableVerticalLayout( 10 );
		scrollableButtons.beginUpdates();
		
		scrollableButtons.setSize(100, Screen.height -200 );
		scrollableButtons.position = new Vector2( 50, 50 );
		
		// establishes vertical scrolling buttons
		for( int i = 0; i < 4; ++i )
		{
			UITouchableSprite holder;
			holder = UIButton.create(upStates[i], downStates[i], 0, 0 );
			
			if(holder is UIButton)
			{
				var button = holder as UIButton;
				
				var j = i;
				
				button.onTouchUpInside += sender => InfoSwitch(j);
			}
			
			scrollableButtons.addChild( holder );
		}
		
		scrollableButtons.endUpdates();
		
		
		// creates buttons to activate animations or posess
		AnimationChoices();
	}
	
	void InfoSwitch(int choice)
	{
		// load different info based on button pushed
		switch(choice)
		{
		case -1:
			// show default info
			break;
		case 0:
			// show first button choice
			break;
		}
		
	}
	
	void AnimationChoices()
	{
		var pos1 = UIButton.create("", "", 0, 0 );
		var pos2 = UIButton.create("", "", 0, 0 );
		var pos3 = UIButton.create("", "", 0, 0 );
		var pos4 = UIButton.create("", "", 0, 0 );
		
		pos1.onTouchUpInside += sender => /* use w/e script to do said animation wanted */ Debug.Log("pew pew 1");
		pos2.onTouchUpInside += sender => Debug.Log("pew pew 2");
		pos3.onTouchUpInside += sender => Debug.Log("pew pew 3");
		pos4.onTouchUpInside += sender => Debug.Log("pew pew 4");
		
	}
	
	
}
