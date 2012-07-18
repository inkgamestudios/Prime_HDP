using UnityEngine;
using System.Collections;

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

public class CameraControl : MonoBehaviour {

	//public enum _Platform{Hybird, Mouse&Keyboard, Touch}
	//public _Platform platform;

	public float panSpeed=5;
	public float zoomSpeed=5;
	
	private float initialMousePosX;
	private float initialMousePosY;
	
	private float initialRotX;
	private float initialRotY;
	
	//for iOS touch input panning
	public bool iOSEnablePan=true;
	private Vector3 lastTouchPos=new Vector3(9999, 9999, 9999);
	private Vector3 moveDir=Vector3.zero;
	
	public bool iOSEnableZoom=true;
	private float touchZoomSpeed;
	
	public bool iOSEnableRotate=false;
	
	public float minPosX=-10;
	public float maxPosX=10;
	
	public float minPosZ=-10;
	public float maxPosZ=10;
	
	public float minRadius=8;
	public float maxRadius=30;

	//calculated deltaTime based on timeScale so camera movement speed always remain constant
	private float deltaT;
	
	private Transform cam;
	private Transform thisT;

	void Awake(){
		thisT=transform;
		
		cam=Camera.main.transform;
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Time.timeScale==1) deltaT=Time.deltaTime;
		else deltaT=Time.deltaTime/Time.timeScale;
		
		#if UNITY_IPHONE || UNITY_ANDROID
	
		if(iOSEnablePan){
			Quaternion camDir=Quaternion.Euler(0, transform.eulerAngles.y, 0);
			if(Input.touchCount==1){
				Touch touch=Input.touches[0];
				if (touch.phase == TouchPhase.Moved){
					Vector3 deltaPos = touch.position;
					
					if(lastTouchPos!=new Vector3(9999, 9999, 9999)){
						deltaPos=deltaPos-lastTouchPos;
						moveDir=new Vector3(deltaPos.x, 0, deltaPos.y).normalized*-1;
					}
	
					lastTouchPos=touch.position;
				}
			}
			else lastTouchPos=new Vector3(9999, 9999, 9999);
			
			Vector3 dir=thisT.InverseTransformDirection(camDir*moveDir)*1.5f;
			thisT.Translate (dir * panSpeed * deltaT);
			
			moveDir=moveDir*(1-deltaT*5);
		}
		
		if(iOSEnableZoom){
			if(Input.touchCount==2){
				Touch touch1 = Input.touches[0];
				Touch touch2 = Input.touches[1];
				
				if(touch1.phase==TouchPhase.Moved && touch1.phase==TouchPhase.Moved){
					//float dot=Vector2.Dot(touch1.deltaPosition, touch1.deltaPosition);
					Vector3 dirDelta=(touch1.position-touch1.deltaPosition)-(touch2.position-touch2.deltaPosition);
					Vector3 dir=touch1.position-touch2.position;
					float dot=Vector3.Dot(dirDelta.normalized, dir.normalized);
					
					if(Mathf.Abs(dot)>0.7f){	
						touchZoomSpeed=dir.magnitude-dirDelta.magnitude;
					}	
				}
				
			}
			
			//cam.Translate(Vector3.forward * touchZoomSpeed * zoomSpeed * Time.deltaTime * 0.1f);
			
			if(touchZoomSpeed<0){
				if(Vector3.Distance(cam.position, thisT.position)<maxRadius){
					cam.Translate(Vector3.forward*Time.deltaTime*zoomSpeed*touchZoomSpeed);
				}
			}
			else if(touchZoomSpeed>0){
				if(Vector3.Distance(cam.position, thisT.position)>minRadius){
					cam.Translate(Vector3.forward*Time.deltaTime*zoomSpeed*touchZoomSpeed);
				}
			}
			
			if(cam.transform.localPosition.z>0){
				cam.localPosition=new Vector3(cam.localPosition.x, cam.localPosition.y, 0);
			}
				
			touchZoomSpeed=touchZoomSpeed*(1-Time.deltaTime*5);
		}
		
		#endif
		
		#if UNITY_EDITOR || (!UNITY_IPHONE && !UNITY_ANDROID)
		
		
		Quaternion direction=Quaternion.Euler(0, thisT.eulerAngles.y, 0);
		
		if(Input.GetButton("Horizontal")) {
			Vector3 dir=transform.InverseTransformDirection(direction*Vector3.right);
			thisT.Translate (dir * panSpeed * deltaT * Input.GetAxisRaw("Horizontal"));
		}

		if(Input.GetButton("Vertical")) {
			Vector3 dir=transform.InverseTransformDirection(direction*Vector3.forward);
			thisT.Translate (dir * panSpeed * deltaT * Input.GetAxisRaw("Vertical"));
		}
		
		//cam.Translate(Vector3.forward*zoomSpeed*Input.GetAxis("Mouse ScrollWheel"));
		
		if(Input.GetAxis("Mouse ScrollWheel")<0){
			if(Vector3.Distance(cam.position, thisT.position)<maxRadius){
				cam.Translate(Vector3.forward*zoomSpeed*Input.GetAxis("Mouse ScrollWheel"));
			}
		}
		else if(Input.GetAxis("Mouse ScrollWheel")>0){
			if(Vector3.Distance(cam.position, thisT.position)>minRadius){
				cam.Translate(Vector3.forward*zoomSpeed*Input.GetAxis("Mouse ScrollWheel"));
			}
		}
		
		//thisT.Translate(cam.forward*zoomSpeed*Input.GetAxis("Mouse ScrollWheel"), Space.World);
		
		#endif
		
		float x=Mathf.Clamp(thisT.position.x, minPosX, maxPosX);
		float z=Mathf.Clamp(thisT.position.z, minPosZ, maxPosZ);
		//float y=Mathf.Clamp(thisT.position.y, verticalLimitBottom, verticalLimitTop);
		
		thisT.position=new Vector3(x, 0, z);
		
	}
	
}
