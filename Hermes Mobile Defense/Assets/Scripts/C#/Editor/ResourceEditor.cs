using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ResourceManager))]
public class ResourceEditor : Editor {

	static private List<bool> foldList=new List<bool>();
	
	static private ResourceManager rm;
	
	static int num=1;
	
	void Awake(){
		
		rm = (ResourceManager)target;
		num=rm.resources.Length;
		
		for(int i=foldList.Count; i<num; i++){
			foldList.Add(false);
		}
		
		//Debug.Log("Awake "+rm.resources.Length+"  "+foldList.Count);
	}
	
	public override void OnInspectorGUI(){
		rm = (ResourceManager)target;
		
		GUI.changed = false;
		
		EditorGUILayout.Space();
		num=EditorGUILayout.IntField("Total ResourceType: ", num);
		if(num<=0) num=1;
		EditorGUILayout.Space();
		
		if(num!=rm.resources.Length) UpdateResourceSize(num);
		if(foldList.Count!=rm.resources.Length) UpdateFoldListSize();
		
		for(int i=0; i<rm.resources.Length; i++){
			
			if(rm.resources[i]!=null){
				foldList[i]=EditorGUILayout.Foldout(foldList[i], rm.resources[i].name);
				
				if(foldList[i]){
					rm.resources[i].icon=(Texture)EditorGUILayout.ObjectField("Icon: ", rm.resources[i].icon, typeof(Texture), false);
					
					rm.resources[i].name=EditorGUILayout.TextField("Name:", rm.resources[i].name);
					
					rm.resources[i].value=EditorGUILayout.IntField("Value: ", rm.resources[i].value);
				}
			}
		}
		
		if(GUI.changed) EditorUtility.SetDirty(rm);
	}
	
	void UpdateResourceSize(int n){
		Resource[] tempList=rm.resources;
		
		rm.resources=new Resource[n];
		
		for(int i=0; i<rm.resources.Length; i++){
			if(i>=tempList.Length){
				rm.resources[i]=new Resource();
				foldList.Add(false);
			}
			else{
				rm.resources[i]=tempList[i];
			}
		}
		
		UpdateFoldListSize();
	}
	
	void UpdateFoldListSize(){
		if(rm.resources.Length<foldList.Count){
			int diff=foldList.Count-rm.resources.Length;
			foldList.RemoveRange(rm.resources.Length, diff);
		}
		else{
			for(int i=foldList.Count; i<rm.resources.Length; i++){
				foldList.Add(false);
			}
		}
	}
}
