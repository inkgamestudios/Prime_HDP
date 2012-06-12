using UnityEngine;
using System.Collections;

public delegate void ButtonPressedCallBack(int ID);

[System.Serializable]
public class GUIToggleButton : GUIButton{
	
	//private bool state=false;
	
	public GUIToggleButton(Texture unpressed, Texture pressed, ButtonPressedCallBack func)
	:base(unpressed, pressed, func, 0){
	
	}
	
	public GUIToggleButton(Texture unpressed, Texture pressed, ButtonPressedCallBack func, int id)
	:base(unpressed, pressed, func, id){
		
	}
    
	private void SwapState(){
		if(buttonObj.HitTest(Input.mousePosition) && buttonObj.enabled){
			if(isPressed){
				//state=false;
				Unpressed();
				if(callBackFunc!=null) callBackFunc(ID);
			}
			else if(!isPressed){
				//state=true;
				Pressed();
				if(callBackFunc!=null) callBackFunc(ID);
			}
		}
	}
	
	public override IEnumerator Update(){
		while(true){
			if(buttonObj.enabled){
				
				if(triggerOnPressed){
					if(Input.GetMouseButtonDown(0)){
						SwapState();
					}
				}
				else{
					if(Input.GetMouseButtonUp(0)){
						SwapState();
					}
				}
				
			}
			
			yield return null;	
		}
	}
}

[System.Serializable]
public class GUIContinousButton : GUIButton{
	public GUIContinousButton(Texture unpressed, Texture pressed, ButtonPressedCallBack func, int id)
	:base(unpressed, pressed, func, id){}
    
	
	public override IEnumerator Update(){
		
		while(true){
			if(buttonObj.enabled){
				
				if(Input.GetMouseButton(0)){
					
					if(!buttonObj.HitTest(Input.mousePosition)){
						if(isPressed) Unpressed();
					}
					else{
						if(!isPressed) Pressed();
						if(callBackFunc!=null) callBackFunc(ID);
					}
					
				}
				else if(isPressed) Unpressed();
				
			}
			
			yield return null;	
		}
	}
}


[System.Serializable]
public class GUIButton {
	
	
	public int ID;
	public GUITexture buttonObj;
	public Texture unpressedTex;
	public Texture pressedTex;
	public bool triggerOnPressed=false;
	//public bool isToogle=false;
	//public bool 
	[HideInInspector] public bool isPressed=false;
	
	public ButtonPressedCallBack callBackFunc;
	
	public GUIButton(Texture unpressed, Texture pressed, ButtonPressedCallBack func, int id){
		GameObject obj=new GameObject();
		buttonObj=obj.AddComponent<GUITexture>();
		
		unpressedTex=unpressed;
		pressedTex=pressed;
		callBackFunc=func;
		ID=id;
		
		buttonObj.texture=unpressedTex;
	}
	
	public virtual IEnumerator Update(){
		
		while(true){
			if(buttonObj.enabled){
				//if(isToogle){
					/*
					if(Input.touchCount>0){
						foreach(Touch touch in Input.touches){
							if(touch.phase==TouchPhase.Began && buttonObj.HitTest(touch.position)){
								if(isPressed) Unpressed();
								else if(!isPressed) Pressed();
								
								if(callBackFunc!=null) callBackFunc(ID);
							}
						}
					}
					*/
					
					//~ if(Input.GetMouseButtonDown(0)){
						//~ if(buttonObj.HitTest(Input.mousePosition) && buttonObj.enabled){
							//~ if(isPressed) Unpressed();
							//~ else if(!isPressed) Pressed();
							
							//~ if(callBackFunc!=null) callBackFunc(ID);
						//~ }
					//~ }
				//}
				//else{
					/*
					if(Input.touchCount>0){
						bool press=false;
						
						foreach(Touch touch in Input.touches){
							if(buttonObj.HitTest(touch.position)){
								press=true;
								if(!isPressed) Pressed();
							
								if(touch.phase==TouchPhase.Ended){
									//if(isPressed) Unpressed();
									//else if(!isPressed) Pressed();
									
									if(callBackFunc!=null) callBackFunc(ID);
								}
							}
						}
						
						if(!press) {
							if(isPressed) Unpressed();
						}
					}
					*/
					
					if(Input.GetMouseButton(0)){
						
						if(!buttonObj.HitTest(Input.mousePosition)){
							if(isPressed) Unpressed();
						}
						else{
							if(!isPressed) {
								Pressed();
								if(triggerOnPressed){
									if(callBackFunc!=null) callBackFunc(ID);
								}
							}
						}
						
					}
					else if(isPressed) Unpressed();
					
					
					if(Input.GetMouseButtonUp(0)){
						if(buttonObj.HitTest(Input.mousePosition)){
							if(!triggerOnPressed){
								if(callBackFunc!=null) callBackFunc(ID);
							}
						}
					}
					
				//}
			}
			
			yield return null;	
		}
	}

	
	public void Unpressed(){
		isPressed=false;
		if(pressedTex!=null) buttonObj.texture=unpressedTex;
		else buttonObj.color=new Color(.5f, .5f, .5f, .5f);
	}
	
	public void Pressed(){
		isPressed=true;
		if(pressedTex!=null) buttonObj.texture=pressedTex;
		else buttonObj.color=new Color(1.0f, 1.0f, 1.0f, .1f);
	}
}
