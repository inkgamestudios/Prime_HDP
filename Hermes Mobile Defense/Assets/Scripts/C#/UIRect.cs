using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIRect : MonoBehaviour {

	static private List<Rect> uiRect=new List<Rect>();
	
	static public void AddRect(Rect rect){
		uiRect.Add(rect);
	}
	
	static public void RemoveRect(Rect rect){
		
		for(int i=uiRect.Count-1; i>=0; i--){
			if(uiRect[i].x==rect.x && uiRect[i].y==rect.y && 
				uiRect[i].width==rect.width && uiRect[i].height==rect.height){
				
				uiRect.RemoveAt(i);
				break;
			}
		}
		
	}
	
	static public bool IsCursorOnUI(Vector3 point){
		
		for(int i=0; i<uiRect.Count; i++){
			Rect tempRect=new Rect(0, 0, 0, 0);
		
			tempRect=uiRect[i];
			tempRect.y=Screen.height-tempRect.y-tempRect.height;
			if(tempRect.Contains(point)) return true;
		}
		
		return false;
		
	}
	
	void OnDrawGizmos(){
		
		foreach(Rect tempRect in uiRect){
			
			Rect rect=tempRect;
			rect.y=Screen.height-rect.y-rect.height;
			
			Vector3[] p=new Vector3[4];
			
			p[0]=new Vector3(rect.x, rect.y, 0.5f);
			p[1]=new Vector3(rect.x+rect.width, rect.y, 0.5f);
			p[2]=new Vector3(rect.x+rect.width, rect.y+rect.height, 0.5f);
			p[3]=new Vector3(rect.x, rect.y+rect.height, 0.5f);
			
			
			for(int i=0; i<4; i++){
				Vector3 p1=Camera.main.ScreenToWorldPoint(p[i]);
				
				int ix=i+1;
				if(ix==4) ix=0;
				
				Vector3 p2=Camera.main.ScreenToWorldPoint(p[ix]);
				
				Gizmos.DrawLine(p1, p2);
			}
			
		}
	}
		
}
