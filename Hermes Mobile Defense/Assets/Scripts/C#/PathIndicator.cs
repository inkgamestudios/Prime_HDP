using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathIndicator : MonoBehaviour {

	private ParticleSystem indicator;
	public Transform indicatorT;
	
	public float stepDist=1;
	public float updateRate=0.25f;
	
	private int wpCounter=0;
	
	private PathSection currentPS;
	private List<Vector3> subPath=new List<Vector3>();
	private int currentPathID=0;
	private int subWPCounter=0;
	
	private Path path;
	//[HideInInspector] public List<Vector3> wp=new List<Vector3>();
	
	// Use this for initialization
	void Start () {
		indicatorT=(Transform)Instantiate(indicatorT);
		indicator=indicatorT.gameObject.GetComponent<ParticleSystem>();
		indicator.emissionRate=0;
		
		path=gameObject.GetComponent<Path>();
		
		StartCoroutine(EmitRoutine());
	}
	
	IEnumerator EmitRoutine(){
		
		yield return null;
		
		List<PathSection> PSList=path.GetPath();
		while(currentPS==null){
			Reset(PSList);
			yield return null;
		}
		
		while(true){
			//~ indicatorT.Translate(Vector3.forward*stepDist);
			
			//~ indicator.Emit(3);
			
			if(currentPathID!=currentPS.GetPathID()){
				GetSubPath();
				
				indicatorT.position=subPath[subWPCounter];
			}
			
			float dist=Vector3.Distance(subPath[subWPCounter], indicatorT.position);
			
			float thisStep=stepDist;
			if(dist<stepDist) {
				thisStep=stepDist-dist;
				indicatorT.position=subPath[subWPCounter];
				
				subWPCounter+=1;
				if(subWPCounter>=subPath.Count){
					wpCounter+=1;
					if(!GetNextWP()){
						thisStep=0;
					}
				}
			}
			
			if(thisStep>0){
				//rotate towards destination
				Vector3 pos=new Vector3(subPath[subWPCounter].x, indicatorT.position.y, subPath[subWPCounter].z);
				Quaternion wantedRot=Quaternion.LookRotation(pos-indicatorT.position);
				//set particlesystem to wantedRot
				indicator.startRotation=(wantedRot.eulerAngles.y-45)*Mathf.Deg2Rad;
				
				indicatorT.LookAt(subPath[subWPCounter]);
				
				//move, with speed take distance into accrount so the unit wont over shoot
				indicatorT.Translate(Vector3.forward*thisStep);
				
				indicator.Emit(1);
			}
			
			yield return new WaitForSeconds(updateRate*Time.timeScale);
		}
	}
	
	bool GetNextWP(){
		List<PathSection> PSList=path.GetPath();
		
		if(wpCounter<PSList.Count){
			currentPS=PSList[wpCounter];
			
			GetSubPath();
			
			return true;
		}
		else{
			//reset all
			Reset(PSList);
			
			return false;
		}
	}
	
	private void GetSubPath(){
		if(currentPS!=null){
			subPath=currentPS.GetSectionPath();
			currentPathID=currentPS.GetPathID();
		}
		subWPCounter=0;
	}
	
	private void Reset(List<PathSection> PSList){
		wpCounter=0;
		subWPCounter=0;
		
		if(PSList!=null && PSList.Count>0) currentPS=PSList[wpCounter];
		GetSubPath();
		
		if(subPath!=null && subPath.Count>0) indicatorT.position=subPath[subWPCounter];
	}
	
}




//use single ParticleSystem to cover multiple path
//doesnt work, somehow the ParticleSystem is too slow keep up with the position change
/*
public class Indicator : MonoBehaviour {

	public ParticleSystem indicator;
	private Transform indiT;
	private Transform[] indicatorT;
	
	public float stepDist=1;
	public float updateRate=0.25f;
	
	public Path[] path;
	
	// Use this for initialization
	void Start () {
		indiT=indicator.transform;
		
		indicatorT=new Transform[path.Length];
		for(int i=0; i<path.Length; i++){
			GameObject obj=new GameObject();
			obj.name="indicatorObj";
			
			indicatorT[i]=obj.transform;
			
			StartCoroutine(EmitRoutine(i, i*0.3f+0.1f));
		}
	}
	
	IEnumerator EmitRoutine(int ID, float startDelay){
		
		Debug.Log("coroutine "+ID);
		
		int wpCounter=0;
	
		PathSection currentPS;
		List<Vector3> subPath=new List<Vector3>();
		int currentPathID=0;
		int subWPCounter=0;
		
		yield return new WaitForSeconds(startDelay);
		
		List<PathSection> PSList=path[ID].GetPath();
		
		currentPS=PSList[wpCounter];
		subPath=currentPS.GetSectionPath();
		currentPathID=currentPS.GetPathID();
		indicatorT[ID].position=subPath[subWPCounter];
		
		
		while(true){
			if(currentPathID!=currentPS.GetPathID()){
				//GetSubPath();
				subPath=currentPS.GetSectionPath();
				currentPathID=currentPS.GetPathID();
				subWPCounter=0;
			}
			
			float dist=Vector3.Distance(subPath[subWPCounter], indicatorT[ID].position);
			
			float thisStep=stepDist;
			if(dist<stepDist) {
				thisStep=stepDist-dist;
				indicatorT[ID].position=subPath[subWPCounter];
				
				subWPCounter+=1;
				if(subWPCounter>=subPath.Count){
					wpCounter+=1;
					
					PSList=path[ID].GetPath();
		
					if(wpCounter<PSList.Count){
						currentPS=PSList[wpCounter];
						
						//GetSubPath();
						subPath=currentPS.GetSectionPath();
						currentPathID=currentPS.GetPathID();
						subWPCounter=0;
						
					}
					else{
						//reset all
						wpCounter=0;
						subWPCounter=0;
		
						currentPS=PSList[wpCounter];
						
						subPath=currentPS.GetSectionPath();
						currentPathID=currentPS.GetPathID();
						
						indicatorT[ID].position=subPath[subWPCounter];
						
						thisStep=0;
					}
					
				}
			}
			
			//rotate towards destination
			if(thisStep>0){
				Vector3 pos=new Vector3(subPath[subWPCounter].x, indicatorT[ID].position.y, subPath[subWPCounter].z);
				Quaternion wantedRot=Quaternion.LookRotation(pos-indicatorT[ID].position);
			
				indicatorT[ID].LookAt(subPath[subWPCounter]);
				
				//move, with speed take distance into accrount so the unit wont over shoot
				indicatorT[ID].Translate(Vector3.forward*thisStep);
				
				indiT.position=indicatorT[ID].position;
				indicator.startRotation=(wantedRot.eulerAngles.y-45)*Mathf.Deg2Rad;
				indicator.Emit(1);
			}
			
			yield return new WaitForSeconds(updateRate);
		}
	}
	
}
*/
