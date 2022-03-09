using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour {
	
	PathRequestManager requestManager;
	Grid grid;
	
	void Awake() {
		//调用路径请求管理器
		requestManager = GetComponent<PathRequestManager>();
		grid = GetComponent<Grid>();
	}
	
	//我们不再需要update函数
	//我们不在需要seeker和target，现在使用的是路径请求管理器
	//一旦发现路径，需要在路径请求管理器脚本中调用完成的处理路径
	public void StartFindPath(Vector3 startPos, Vector3 targetPos) {
		//找到路径并传递起始和目标位置
		
		StartCoroutine(FindPath(startPos,targetPos));
	}
	
	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {
		//存储路径点
		Vector3[] waypoints = new Vector3[0];
		//默认情况下，这等于false，只有当我们找到路径时，这个才会为真
		bool pathSuccess = false;
		
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);
		
		//在这里判断节点是否可以行走
		if (startNode.walkable && targetNode.walkable) {
			Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);
			
			while (openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);
				
				if (currentNode == targetNode) {
					//所以如果当前的note等于目标节，为真
					pathSuccess = true;
					break;
				}
				
				foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
					if (!neighbour.walkable || closedSet.Contains(neighbour)) {
						continue;
					}
					
					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
						neighbour.parent = currentNode;
						
						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
					}
				}
			}
		}
		yield return null;
		//路径从这里返回
		if (pathSuccess) {
			waypoints = RetracePath(startNode,targetNode);
		}
		requestManager.FinishedProcessingPath(waypoints,pathSuccess);
		
	}
	//路径平滑将在下一节中，现在要做的就是简化路径
	//路径点只会再路径改变的方向出现，所以不会有几百个多余的路径点
	Vector3[] RetracePath(Node startNode, Node endNode) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;
		
		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		//在我们反转路径之前，先简化路径		
		Vector3[] waypoints = SimplifyPath(path);
		//我们不逆转路径，我们逆转路径点，最后我们返回路径点
		//这里是一个数组
		Array.Reverse(waypoints);
		return waypoints;
		
	}
	//简化路径
	Vector3[] SimplifyPath(List<Node> path) {
		//创建列表存储路径点
		List<Vector3> waypoints = new List<Vector3>();
		//创建一个向量，来存储最后两个节点的方向，所以就是旧的方向
		Vector2 directionOld = Vector2.zero;
		
		for (int i = 1; i < path.Count; i ++) {
			Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX,path[i-1].gridY - path[i].gridY);
			if (directionNew != directionOld) {
				//路径实际上改变了方向，我们把这个路径点添加到路径点列表中
				waypoints.Add(path[i].worldPosition);
			}
			directionOld = directionNew;
		}
		//一旦我们完成了这个循环，我们只需要将这个列表转换成一个数组并返回它
		return waypoints.ToArray();
	}
	
	int GetDistance(Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
		
		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}
	
	
}
