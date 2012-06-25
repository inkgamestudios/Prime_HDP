using UnityEngine;
using System.Collections;

public class CursorManager : MonoBehaviour {

	//public GUITexture cursor;
	//private Transform cursorT;
	
	public Texture pointer;
	public Texture hostile;
	public Texture friendly;
	private Texture currentTexture;
	
	#if !Unity_IPhone || !Unity_Android
	
	// Use this for initialization
	void Start () {
		
		//cursorT=cursor.transform;
		//cursorT.gameObject.layer=LayerManager.LayerOverlay();
		//cursorT.gameObject.active=false;
		Screen.showCursor=false;
		
		currentTexture=pointer;
	}
	
	void OnDisable(){
		Screen.showCursor=true;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 mousePos=Input.mousePosition;
		
		//float x=mousePos.x/Screen.width;
		//float y=mousePos.y/Screen.height;
		//Vector3 pos=new Vector3(x, y, 100);
		//cursorT.position=pos;
		
		if(UIRect.IsCursorOnUI(mousePos)){
			//cursor.texture=pointer;
			currentTexture=pointer;
		}
		else{
			Ray ray = Camera.main.ScreenPointToRay(mousePos);
			RaycastHit hit;
	   
			if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
				
				if(hit.collider.gameObject.layer==LayerManager.LayerCreep()){
					if(GameControl.selectedTower!=null){
						//cursor.texture=hostile;
						currentTexture=hostile;
					}
				}
				else if(hit.collider.gameObject.layer==LayerManager.LayerCreepF()){
					if(GameControl.selectedTower!=null){
						//cursor.texture=hostile;
						currentTexture=hostile;
					}
				}
				else if(hit.collider.gameObject.layer==LayerManager.LayerTower()){
					//cursor.texture=friendly;
					currentTexture=friendly;
				}
				else{
					//cursor.texture=pointer;
					currentTexture=pointer;
				}
			}
		}
	}
	
	void OnGUI(){
		GUI.depth=0;
		
		Vector3 pos=Input.mousePosition;
		pos.y=Screen.height-pos.y;
		GUI.DrawTexture(new Rect(pos.x-15, pos.y-15, 30, 30), currentTexture, ScaleMode.ScaleToFit, true, 0.0F);
	}
	
	#endif
	
}
