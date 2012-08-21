using UnityEngine;
using System.Collections;

public class PlayerSave : MonoBehaviour 
{
//
//basics
//
// create variables
	private int[] playerSaveStage1 = new int[60];
	private int[] playerSaveStage2 = new int[55];
	private int[] playerSaveStage3 = new int[55];
	private int cartridge;
	
	private static PlayerSave instance = null;
	
	private int lvlNumber;
	private int score;
	private int stageNumber = 0;
	private string currentStage;
	
	private static bool tower1 = true;
	private static bool tower2 = false;
	private static bool tower3 = false;
	private static bool tower4 = false;
	private static bool tower5 = false;
	private static bool tower6 = false;
	private static bool tower7 = false;
	private static bool tower8 = false;
	private static bool tower9 = false;
	
	private static bool ability1 = false;
	private static bool ability2 = false;
	private static bool ability3 = false;

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
		SetScore();	// not created yet
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
		switch(stageNumber)
		{
			case 1:
				PlayerPrefsX.SetIntArray( currentStage, playerSaveStage1 );
				Debug.Log( "Data for stage 1 saved" );
				break;
			case 2:
				PlayerPrefsX.SetIntArray( currentStage, playerSaveStage2 );
				Debug.Log( "Data for stage 2 saved" );
				break;
			case 3:
				PlayerPrefsX.SetIntArray( currentStage, playerSaveStage3 );
				Debug.Log( "Data for stage 3 saved" );
				break;
			default:
				Debug.Log ("stageNumber is not set to an instance of a stage");
				break;
		}
	}
	
	// checks to see what rewards are unlocked and if a new one is unlocked
	// while on a level where a tower is unlocked upon completion, have said tower available during said lvl.
	void CheckReward()
	{
		tower1 = true;
		// holder lvls for the towers
		int[] holder1 = new int[]{3, 5, 7, 9};
		int[] holder2 = new int[]{3, 4, 5, 6};
		int[] holder3 = new int[]{4, 5, 6, 7};
		// holder lvls for the abilities
		int holder4 = 5;
		int holder5 = 6;
		int holder6 = 7;
		
		bool[] unlockedT = new bool[]{ tower2, tower3, tower4, tower5, tower6, tower7, tower8, tower9 };
		int x = 0;

		if ( GetScoresTier( 1, holder4 ) > 0 )
		{
			ability1 = true;
		}
		if ( GetScoresTier( 2, holder5 ) > 0 )
		{
			ability2 = true;
		}
		if ( GetScoresTier( 3, holder6 ) > 0 )
		{
			ability3 = true;
		}
		
		foreach(int i in holder1)
		{
			if( playerSaveStage1[i] > 0)
			{
				unlockedT[x] = true;
				x++;
			}
				
		}
		foreach(int i in holder2)
		{
			if( playerSaveStage2[i] > 0 )
			{
				unlockedT[x] = true;
				x++;
			}
		}
		foreach(int i in holder3)
		{
			if( playerSaveStage3[i] > 0 )
			{
				unlockedT[x] = true;
				x++;
			}
		}
		
	}
	
	// SetStage is called in Level Select and hardcoded the variables for stage and lvl ref
	public void SetStage( int stage, int lvlRef )
	{
		lvlNumber = lvlRef;
		
		switch(stage)
		{
			case 1:
				currentStage = "playerSaveS1";
				stageNumber = 1;
				Debug.Log( "stage1 Loaded" );
				break;

			case 2:
				currentStage = "playerSaveS2";
				stageNumber = 2;
				Debug.Log( "stage2 Loaded" );
				break;

			case 3:
				currentStage = "playerSaveS3";
				stageNumber = 3;
				Debug.Log( "stage3 Loaded" );
				break;
			
			default:
				Debug.Log( "wrong input for set stage" );
				stageNumber = 0;
				break;
		}
	}
	
	// checks if the level is complete and if it is it saves the end score into the local array.
	private void SetScore()
	{
		if( GameControl.gameState == _GameState.Ended )
		{
			score = GameControl.GetResourceVal(2); // need to check if this works
			Sorter ( stageNumber, lvlNumber );
		}
	}

	// returns the score based on current stage and lvl.
	public int GetScore()
	{
		switch(stageNumber)
		{
			case 1:
				return playerSaveStage1[lvlNumber];
				break;
			case 2:
				return playerSaveStage2[lvlNumber];
				break;
			case 3:
				return playerSaveStage3[lvlNumber];
				break;
			
			default:
				Debug.Log("GetScores basic, something wrong w/ stageNumber");
				return 0;
				break;
		}
	}

	// gets scores on different stages/lvl's other then current.
	public int GetScoresTier(int stage, int lvl)
	{
		switch(stage)
		{
			case 1:
				return playerSaveStage1[lvl];
				break;
			case 2:
				return playerSaveStage2[lvl];
				break;
			case 3:
				return playerSaveStage3[lvl];
				break;
			
			default:
				Debug.Log("invalid input value for GetScore 2nd overload inside playerSave.cs");
				return 0;
				break;
		}
	}
	
// Need the names of the towers to finish this part of class.	******
	public UnitTower[] UnlockedTowers()
	{
		int total = 0;
		int count = 0;
		bool[] unlocks = new bool[9] {tower1, tower2, tower3, tower4, tower5, tower6, tower7, tower8, tower9};
		foreach( bool i in unlocks)
		{
			if( i )
			{
				total++;
			}
		}

		UnitTower[] towers = new UnitTower[total];
		if( tower1 )
		{
			//towers[count] = nameOfTower;
			count++;
		}
		if( tower2 )
		{
			//towers[count] = nameOfTower2;
			count++;
		}
		if( tower3 )
		{
			//towers[count] = nameofTower3;
			count++;
		}
		if( tower4 )
		{
			//towers[count] = nameofTower4;
			count++;
		}
		if( tower5 )
		{
			//towers[count] = nameofTower5;
			count++;
		}
		if( tower6 )
		{
			//towers[count] = nameofTower6;
			count++;
		}
		if( tower7 )
		{
			//towers[count] = nameofTower7;
			count++;
		}
		if( tower8 )
		{
			//towers[count] = nameofTower8;
			count++;
		}
		if( tower9 )
		{
			//towers[count] = nameofTower9;
			count++;
		}
		return towers;
	}
	
	// bubble sort and save results back into local array.
	private void Sorter( int stage, int lvl )
	{
		int pos1= 0;
		int pos2= 0;
		int pos3= 0;
		int pos4= 0;
		int pos5= 0;
		int posHolder = score;
		int holder= 0;
		
		switch(stage)
		{
			case 1:
				pos1 = playerSaveStage1[lvl];
				pos2 = playerSaveStage1[lvl++];
				pos3 = playerSaveStage1[lvl++];
				pos4 = playerSaveStage1[lvl++];
				pos5 = playerSaveStage1[lvl++];
				break;
			case 2:
				pos1 = playerSaveStage2[lvl];
				pos2 = playerSaveStage2[lvl++];
				pos3 = playerSaveStage2[lvl++];
				pos4 = playerSaveStage2[lvl++];
				pos5 = playerSaveStage2[lvl++];
				break;
			case 3:
				pos1 = playerSaveStage3[lvl];
				pos2 = playerSaveStage3[lvl++];
				pos3 = playerSaveStage3[lvl++];
				pos4 = playerSaveStage3[lvl++];
				pos5 = playerSaveStage3[lvl++];
				break;
			default:
				Debug.Log("Wrong input number for stage");
				break;
		}
		lvl -= 5;
		
		for( int i = 0; i < 5; i++)
		{
			if(pos1 > pos2){}
			else
			{
				// switch
				holder = pos2;
				pos2 = pos1;
				pos1 = holder;
			}
			if( pos2 > pos3 ){}
			else
			{
				// switch
				holder = pos3;
				pos3 = pos2;
				pos2 = holder;
			}
			if( pos3 > pos4 ){}
			else
			{
				holder = pos4;
				pos4 = pos3;
				pos3 = holder;
			}
			if(pos4 > pos5){}
			else
			{
				holder = pos5;
				pos5 = pos4;
				pos4 = holder;
			}
			if(pos5 > posHolder){}
			else
			{
				holder = posHolder;
				posHolder = pos5;
				pos5 = holder;
			}
		}
		switch(stage)
		{
			case 1:
				playerSaveStage1[lvl] = pos1;
				playerSaveStage1[lvl++] = pos2;
				playerSaveStage1[lvl++] = pos3;
				playerSaveStage1[lvl++] = pos4;
				playerSaveStage1[lvl++] = pos5;
				break;
			case 2:
				playerSaveStage2[lvl] = pos1;
				playerSaveStage2[lvl++] = pos2;
				playerSaveStage2[lvl++] = pos3;
				playerSaveStage2[lvl++] = pos4;
				playerSaveStage2[lvl++] = pos5;
				break;
			case 3:
				playerSaveStage3[lvl]  = pos1;
				playerSaveStage3[lvl++]  = pos2;
				playerSaveStage3[lvl++]  = pos3;
				playerSaveStage3[lvl++]  = pos4;
				playerSaveStage3[lvl++]  = pos5;
				break;
			default:
				Debug.Log("Wrong input number for stage");
				break;
		}
	}

// return high scores
	public int[] getHighScores(int stage, int lvl )
	{
		int[] highScores = new int[5];
		foreach(int i in highScores)
		{
			GetScoresTier( stage, lvl );
			lvl++;
		}
		return highScores;
	}
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