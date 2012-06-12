using UnityEngine;
using System.Collections;

public class DemoMainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		if(GUI.Button(new Rect(Screen.width/2-50, Screen.height/2-15, 100, 30), "Demo 1")){
			Application.LoadLevel("ExampleScene1");
		}
		if(GUI.Button(new Rect(Screen.width/2-50, Screen.height/2-15+45, 100, 30), "Demo 2")){
			Application.LoadLevel("ExampleScene2");
		}
		if(GUI.Button(new Rect(Screen.width/2-50, Screen.height/2-15+90, 100, 30), "Demo 3")){
			Application.LoadLevel("ExampleScene3");
		}
	}
}
