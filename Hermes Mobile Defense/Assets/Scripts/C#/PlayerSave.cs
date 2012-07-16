using UnityEngine;
using System.Collections;

public class PlayerSave : MonoBehaviour 
{
//
//basics
//
// create variables
	private int[] playerSaveStage1 = new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
	private int[] playerSaveStage2 = new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
	private int[] playerSaveStage3 = new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
	private int cartridge;
	private static PlayerSave instance = null;
	private int lvlNumber;
	private int lvlScore;
	private string currentStage;
	
	
	//playerPrefs save names
	//playerSaveStage1 = playerSaveS1
	//playerSaveStage2 = playerSaveS2
	//playerSaveStage3 = playerSaveS3
	//cartridge = cart
	
	// creates an instance of this script
	public static PlayerSave Instance
    {
        get
        {
            return instance;
        }
    }
	
	// ensures that the save function carries through the levels
	void Awake()
	{
		// if this script exists already destroy it
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
	}
	
	void Start()
	{
		// currentStage and lvlNumber get set when selecting a button in LevelSelect.cs
		// get if lvl passed, get score, then run set array
		
		// need if statement for if playerSaveS1 - 3 already exists don't do this else create them
		int[] check = PlayerPrefsX.GetIntArray( "playerSaveS1" );
		int rad = check.Length;
		if( rad < 9 )
		{
			PlayerPrefsX.SetIntArray( "playerSaveS1", playerSaveStage1 );
			PlayerPrefsX.SetIntArray( "playerSaveS2", playerSaveStage2 );
			PlayerPrefsX.SetIntArray( "playerSaveS3", playerSaveStage3 );
			PlayerPrefs.SetInt( "cart", 0 );
		}
		
		LoadArrayUpdate();
		
		// if (levelPassed)
		// sets score into the local array
		SetScore(); // not created yet
		SetArrayUpdate();

	}
	
	// loads Array into editable arrays from playerprefs
	void LoadArrayUpdate()
	{
		playerSaveStage1 = PlayerPrefsX.GetIntArray( "playerSaveS1" );
		playerSaveStage2 = PlayerPrefsX.GetIntArray( "playerSaveS2" );
		playerSaveStage3 = PlayerPrefsX.GetIntArray( "playerSaveS3" );
		cartridge = PlayerPrefs.GetInt( "cart" );
	}
	
	// not sure if this works or not *****
	public void SetArrayUpdate()
	{	
		bool ps1;
		bool ps2;
		bool ps3;
		ps1 = string.Equals( currentStage,  "playerSaveS1");
		ps2 = string.Equals( currentStage,  "playerSaveS2");
		ps3 = string.Equals( currentStage,  "playerSaveS3");
		
		if( ps1 == true )
		{
			PlayerPrefsX.SetIntArray( currentStage, playerSaveStage1 );
			ps1 = false;
			Debug.Log( "Data for stage 1 saved" );
		}
		else if( ps2 == true )
		{
			PlayerPrefsX.SetIntArray( currentStage, playerSaveStage2 );
			ps2 = false;
			Debug.Log( "Data for stage 2 saved" );
		}
		else if( ps3 == true )
		{
			PlayerPrefsX.SetIntArray( currentStage, playerSaveStage3 );
			ps3 = false;
			Debug.Log( "Data for stage 3 saved" );
		}
	}
	
	// checks to see what rewards are unlocked and if a new one is unlocked
	void CheckReward()
	{}
	
	// SetStage is called in Level Select and hardcoded the variables for stage and lvl ref
	public void SetStage( int stage, int lvlRef )
	{
		switch(stage)
		{
			case 1:
			{
				currentStage = "playerSaveS1";
				lvlNumber = lvlRef;
				//Debug.Log( "stage1 Loaded" );
				break;
			}
			case 2:
			{
				currentStage = "playerSaveS2";
				lvlNumber = lvlRef;
				Debug.Log( "stage2 Loaded" );
				break;
			}
			case 3:
			{
				currentStage = "playerSaveS3";
				lvlNumber = lvlRef;
				Debug.Log( "stage3 Loaded" );
				break;
			}
			default:
			{
				Debug.Log( "wrong input for set stage" );
				break;
			}
		}
	}
	
	// checks if the level is complete and if it is it saves the end score into the local array.
	void SetScore()
	{
		
	}
	
	private void ReturnScore()
	{
		
	}


// return high scores
// if changes record new variables into save function
// if level passed get score, set score, then save score

//
//advanced
//
// if passed unlock lvl's and rewards
// if reward = fuel cartridge, save cartridge
// spend cartridge function to get, set, return, and edit initial spending resources

// how we are recording the score for the game

}