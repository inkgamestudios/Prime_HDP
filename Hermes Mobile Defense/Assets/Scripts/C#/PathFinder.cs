using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

public delegate void SetPathCallback(List<Vector3> wp);

public class PathFinder : MonoBehaviour {

	

	static private List<SearchQueue> queue=new List<SearchQueue>();
	
	static bool searching=false;
	
	static PathFinder pathFinder;
	
	public bool pathSmoothing=false;
	public int ScanNodeLimitPerFrame=350;
	
	
	static public bool IsPathSmoothingOn(){
		return pathFinder.pathSmoothing;
	}
	
	void Awake(){
		pathFinder=this;
	}
	
	void Start(){
		//~ new CallBackDelegate(platform.TestCallBack());
	}
	
	static public void Init(){
		GameObject obj=new GameObject();
		pathFinder=obj.AddComponent<PathFinder>();
		
		obj.name="PathFinder";
	}
	
	static public void CheckInit(){
		if(pathFinder==null) Init();
	}
	
	//~ static public Queue SearchPath(){
		//~ return new Queue();
	//~ }
	
	static public void CallMeBack(SetPathCallback callBackFunc){
		//~ FooCallbackType myDelegate = new FooCallbackType(
			//~ this.TestCallBack );
		//~ callBackFunc("i Hear you!!");
		
		//~ Debug.Log("queue");
		
		//~ queue.Add(new Queue(callBackFunc));
		//~ Debug.Log(queue.Count);
	}
	
	void Update(){
		//~ Debug.Log("update");
		//~ if(queue.Count>0){
			
			//~ Debug.Log("queueX");
			
			//~ List<Vector3> list=new List<Vector3>();
			//~ list.Add(new Vector3(99, 0, 99));
			
			//~ queue[0].callBackFunc(list);
			//~ queue.RemoveAt(0);
			
		//~ }
	}
	
	static public Node GetNearestNode(Vector3 point, Node[] graph){
		float dist=Mathf.Infinity;
		float currentNearest=Mathf.Infinity;
		Node nearestNode=null;
		foreach(Node node in graph){
			if(node.walkable){
				dist=Vector3.Distance(point, node.pos);
				if(dist<currentNearest){
					currentNearest=dist;
					nearestNode=node;
				}
			}
		}
		return nearestNode;
	}
	
	
	
	static public void GetPath(Vector3 startP, Vector3 endP, Node[] graph, SetPathCallback callBackFunc){
		Node startNode=GetNearestNode(startP, graph);
		Node endNode=GetNearestNode(endP, graph);
		
		GetPath(startNode, endNode, null, graph, callBackFunc, false);
	}
	
	static public void GetPath(Vector3 startP, Vector3 endP, Vector3 blockP, Node[] graph, SetPathCallback callBackFunc){
		Node startNode=GetNearestNode(startP, graph);
		Node endNode=GetNearestNode(endP, graph);
		Node blockNode=GetNearestNode(blockP, graph);
		
		GetPath(startNode, endNode, blockNode, graph, callBackFunc, false);
	}
	
	static public void GetPath(Node startN, Node endN, Node[] graph, SetPathCallback callBackFunc){
		GetPath(startN, endN, null, graph, callBackFunc, false);
	}
	
	static public void GetPath(Node startN, Node endN, Node[] graph, SetPathCallback callBackFunc, bool urgent){
		GetPath(startN, endN, null, graph, callBackFunc, urgent);
	}
	
	static public void GetPath(Node startN, Node endN, Node blockN, Node[] graph, SetPathCallback callBackFunc, bool urgent){
		CheckInit();
		
		if(!searching){
			//commence search
			Search(startN, endN, blockN, graph, callBackFunc);
		}
		else{
			//if a serach is in progress, put current request into a list
			SearchQueue q=new SearchQueue(startN, endN, blockN, graph, callBackFunc);
			if(urgent) queue.Insert(0, q);
			else queue.Add(q);
		}
	}
	
	static private void Search(Node startN, Node endN, Node blockN, Node[] graph, SetPathCallback callBackFunc){
		pathFinder.StartCoroutine(pathFinder._Search(startN, endN, blockN, graph, callBackFunc));
	}
	
	IEnumerator _Search(Node startN, Node endN, Node blockN, Node[] graph, SetPathCallback callBackFunc){
		
		//block node is for checking if a path exist if that particular node is block
		if(blockN!=null){
			//var switched:boolean=true;
			//if(block.walkable) block.walkable=false;
			//else switched=false;
			
			blockN.walkable=false;
		}

		//time tracker for performance check
		float timeStart=Time.realtimeSinceStartup;

		//mark that a serac has started, any further query will be queued
		searching=true;
		bool pathFound=true;
		
		int searchCounter=0;	//used to count the total amount of node that has been searched
		int loopCounter=0;		//used to count how many node has been search in the loop, if it exceed a value, bring it to the next frame
		//float LoopTime=Time.realtimeSinceStartup;

		//closelist, used to store all the node that are on the path
		List<Node> closeList=new List<Node>();
		//openlist, all the possible node that yet to be on the path, the number can only be as much as the number of node in the garph
		Node[] openList=new Node[graph.Length];
		
		//an array use to record the element number in the open list which is empty after the node is removed to be use as currentNode,
		//so we can use builtin array with fixed length for openlist, also we can loop for the minimal amount of node in every search
		List<int> openListRemoved=new List<int>();
		//current count of elements that are occupied in openlist, openlist[n>openListCounter] are null
		int openListCounter=0;

		//set start as currentNode
		Node currentNode=startN;
		
		//use to compare node in the openlist has the lowest score, alwyas set to Infinity when not in used
		float currentLowestF=Mathf.Infinity;
		int id=0;	//use element num of the node with lowest score in the openlist during the comparison process
		int i=0;		//universal int value used for various looping operation

		//loop start
		while(true){
		
			//if we have reach the destination
			if(currentNode==endN) break;
			
			//for gizmo debug purpose
			//currentNodeBeingProcess=currentNode;

			//move currentNode to closeList;
			closeList.Add(currentNode);
			currentNode.listState=_ListState.Close;
			
			//loop through the neighbour of current loop, calculate  score and stuff
			currentNode.ProcessNeighbour(endN);
			
			//put all neighbour in openlist
			foreach(Node neighbour in currentNode.neighbourNode){
				if(neighbour.listState==_ListState.Unassigned && neighbour.walkable) {
					//set the node state to open
					neighbour.listState=_ListState.Open;
					//if there's an open space in openlist, fill the space
					if(openListRemoved.Count>0){
						openList[openListRemoved[0]]=neighbour;
						//remove the number from openListRemoved since this element has now been occupied
						openListRemoved.RemoveAt(0);
					}
					//else just stack on it and increase the occupication counter
					else{
						openList[openListCounter]=neighbour;
						openListCounter+=1;
					}
				}
			}
			
			//clear the current node, before getting a new one, so we know if there isnt any suitable next node
			currentNode=null;
			
			//get the next point from openlist, set it as current point
			//just loop through the openlist until we reach the maximum occupication
			//while that, get the node with the lowest score
			currentLowestF=Mathf.Infinity;
			id=0;
			for(i=0; i<openListCounter; i++){
				if(openList[i]!=null){
					if(openList[i].scoreF<currentLowestF){
						currentLowestF=openList[i].scoreF;
						currentNode=openList[i];
						id=i;
					}
				}
			}
			
			//if there's no node left in openlist, path doesnt exist
			if(currentNode==null) {
				pathFound=false;
				break;
			}
			
			//remove the new currentNode from openlist
			openList[id]=null;
			//put the id into openListRemoved so we know there's an empty element that can be filled in the next loop
			openListRemoved.Add(id);
			
			

			//increase the counter
			searchCounter+=1;
			loopCounter+=1;

			//if exceed the search limit per frame, bring the search to the next frame
			if(loopCounter>ScanNodeLimitPerFrame){
				loopCounter=0;	//reset the loopCounter for the next frame
				yield return null;
			}
		}

		//see how long it has taken to complete the search
		//float timeUsed=Time.realtimeSinceStartup-timeStart;
		//if(pathFound)	Debug.Log("Path found. Searched "+searchCounter+" nodes, used "+timeUsed+"seconds");
		//else	Debug.Log("no path. Searched "+searchCounter+" nodes, used "+timeUsed+"seconds");


		//trace back the path through closeList
		List<Vector3> p=new List<Vector3>();
			
		if(pathFound){
			//track back the node's parent to form a path
			while(currentNode!=null){
				p.Add(currentNode.pos);
				currentNode=currentNode.parent;
			}
			
			//since the path is now tracked from endN ot startN, invert the list
			p=InvertArray(p);
			if(pathSmoothing) {
				//Debug.Log("smoothing path");
				p=LOSPathSmoothingBackward(p);
				p=LOSPathSmoothingForward(p);
			}
			//else Debug.Log("skip smoothing path");
		}
		//~ else Debug.Log("no path found");
		
		//if this is just to check if a path is blocked, now we can clear the assumed blocked node
		if(blockN!=null) blockN.walkable=true; 
		
		callBackFunc(p);

		//reset the all the nodegraph's node state?
		//Reset();
		
		//clear searching so indicate the search has end and a new serach can be called
		searching=false;
		
		ResetGraph(graph);
	
	}
	
	
	//make cause system to slow down, use with care
	static public List<Vector3> ForceSearch(Node startN, Node endN, Node blockN, Node[] graph){
		
		if(blockN!=null){
			blockN.walkable=false;
			//Debug.Log("block reference node");
		}
		
		bool pathFound=true;
		
		int searchCounter=0;	//used to count the total amount of node that has been searched
		
		List<Node> closeList=new List<Node>();
		Node[] openList=new Node[graph.Length];
		
		List<int> openListRemoved=new List<int>();
		int openListCounter=0;

		Node currentNode=startN;
		
		float currentLowestF=Mathf.Infinity;
		int id=0;	//use element num of the node with lowest score in the openlist during the comparison process
		int i=0;		//universal int value used for various looping operation
		
		while(true){
		
			if(currentNode==endN) break;
			
			closeList.Add(currentNode);
			currentNode.listState=_ListState.Close;
			
			currentNode.ProcessNeighbour(endN);
			
			foreach(Node neighbour in currentNode.neighbourNode){
				if(neighbour.listState==_ListState.Unassigned && neighbour.walkable) {
					neighbour.listState=_ListState.Open;
					if(openListRemoved.Count>0){
						openList[openListRemoved[0]]=neighbour;
						openListRemoved.RemoveAt(0);
					}
					else{
						openList[openListCounter]=neighbour;
						openListCounter+=1;
					}
				}
			}
			
			currentNode=null;
			
			currentLowestF=Mathf.Infinity;
			id=0;
			for(i=0; i<openListCounter; i++){
				if(openList[i]!=null){
					if(openList[i].scoreF<currentLowestF){
						currentLowestF=openList[i].scoreF;
						currentNode=openList[i];
						id=i;
					}
				}
			}
			
			if(currentNode==null) {
				pathFound=false;
				break;
			}
			
			openList[id]=null;
			openListRemoved.Add(id);

			searchCounter+=1;
			
		}

		//~ float timeUsed=Time.realtimeSinceStartup-timeStart;
		//~ if(pathFound)	Debug.Log("Path found. Searched "+searchCounter+" nodes, used "+timeUsed+"seconds");
		//~ else	Debug.Log("no path. Searched "+searchCounter+" nodes, used "+timeUsed+"seconds");


		List<Vector3> p=new List<Vector3>();
			
		if(pathFound){
			while(currentNode!=null){
				p.Add(currentNode.pos);
				currentNode=currentNode.parent;
			}
			
			p=InvertArray(p);
			//if(pathFinder.pathSmoothing) {
			//	p=pathFinder.LOSPathSmoothingBackward(p);
			//	p=pathFinder.LOSPathSmoothingForward(p);
			//}
		}
		
		if(blockN!=null) {
			blockN.walkable=true; 
			//Debug.Log("unblock reference node");
		}
		
		ResetGraph(graph);
		
		return p;

	}
	
	static public List<Vector3> SmoothPath(List<Vector3> p){
		if(pathFinder.pathSmoothing) {
			p=pathFinder.LOSPathSmoothingBackward(p);
			p=pathFinder.LOSPathSmoothingForward(p);
		}
		
		return p;
	}
	
	static private List<Vector3> InvertArray(List<Vector3> p){
		List<Vector3> pInverted=new List<Vector3>();
		for(int i=0; i<p.Count; i++){
			pInverted.Add(p[p.Count-(i+1)]);
		}
		return pInverted;
	}
	
	//pathSmoothing forward
	private List<Vector3> LOSPathSmoothingForward(List<Vector3> p){
		float gridSize=BuildManager.GetGridSize();
		int num=0;
		float allowance=gridSize*0.4f;
		while (num+2<p.Count){
			bool increase=false;
			Vector3 p1=p[num];
			Vector3 p2=p[num+2];
			RaycastHit hit;
			Vector3 dir=p2-p1;
			
			if(!Physics.SphereCast(p1, allowance, dir, out hit, Vector3.Distance(p2, p1))){
				if(p1.y==p2.y) p.RemoveAt(num+1);
				else increase=true;
			}
			else {
				increase=true;
			}
			
			if(increase) num+=1;

		}
		return p;
	}



	//pathSmoothing backward
	private List<Vector3> LOSPathSmoothingBackward(List<Vector3> p){
		float gridSize=BuildManager.GetGridSize();
		int num=p.Count-1;
		float allowance=gridSize*0.4f;
		while (num>1){
			bool decrease=false;
			Vector3 p1=p[num];
			Vector3 p2=p[num-2];
			RaycastHit hit;
			Vector3 dir=p2-p1;
			
			if(!Physics.SphereCast(p1, allowance, dir, out hit, Vector3.Distance(p2, p1))){
				if(p1.y==p2.y) p.RemoveAt(num-1);
				else decrease=true;
			}
			else {
				decrease=true;
			}
			
			num-=1;
			if(decrease) num-=1;

		}
		return p;
	}
	
	static public void ResetGraph(Node[] nodeGraph){
		foreach(Node node in nodeGraph){
			node.listState=_ListState.Unassigned;
			node.parent=null;
		}
	}
	
}

//~ public class QueueElement{
	//~ public SetPathCallback callBackFunc;
	
	//~ public Queue(SetPathCallback callBack){
		//~ callBackFunc=callBack;
	//~ }
//~ }

class SearchQueue{
	public Node startNode;
	public Node endNode;
	public Node blockNode;
	public Node[] graph;
	public SetPathCallback callBackFunc;
	
	public SearchQueue(Node n1, Node n2, Node n3, Node[] g, SetPathCallback func){
		startNode=n1;
		endNode=n2;
		blockNode=n3;
		graph=g;
		callBackFunc=func;
	}
}
