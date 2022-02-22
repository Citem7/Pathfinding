using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour {

	public Transform seeker, target;
	//创建一个网格
	Grid grid;
	//awake()在脚本调用之前调用，完成对场景初始化的任务
	void Awake() {
		grid = GetComponent<Grid> ();//获得网格组件
	}
	//seeker应该是探测器
	void Update() {
		FindPath (seeker.position, target.position);//seeker这个点应该不是起点，是每次都会更新的点，那会是什么呢
	}

	void FindPath(Vector3 startPos, Vector3 targetPos) {
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

		List<Node> openSet = new List<Node>();//open表
		//散列集合HashSet是一种数据结构，支持泛型。
		//HashSet包含一组不重复出现且无特性顺序的元素
		//HashSet的值不重复且没有顺序，容量会按需自动增加。
		//这个类主要是设计用来做高性能集运算的，例如对两个集合求交集、差集、并集、补集等。
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		while (openSet.Count > 0) {
			Node node = openSet[0];
			//这个循环在找f值最小的那个，但是为什么要比较h值呢。会不会是因为计算g值会出现偏差，导致g值可能会比真实值偏大
			for (int i = 1; i < openSet.Count; i ++) {
				if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost) {
					if (openSet[i].hCost < node.hCost)
						node = openSet[i];
				}
			}
			//将该点从open表移入closed表
			openSet.Remove(node);
			closedSet.Add(node);
			//到达目标点，提前结束
			if (node == targetNode) {
				RetracePath(startNode,targetNode);
				return;
			}

			foreach (Node neighbour in grid.GetNeighbours(node)) {
				//如果该邻居是障碍物或者在closed集中已经存在
				if (!neighbour.walkable || closedSet.Contains(neighbour)) {
					continue;
				}
				//当前点的g值加上当前点到邻居点的距离，有两个10或14（斜着）
				int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
				//newCostToNeighbour < neighbour.gCost会出现这种情况么
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = node;
					//该邻居没有出现在open表中，就加入open表
					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}
	}
	//从后往前叙述路径
	void RetracePath(Node startNode, Node endNode) {
		//新建立一个表
		List<Node> path = new List<Node>();
		Node currentNode = endNode;
		//从后往前叙述路径
		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		//路径在颠倒过来
		path.Reverse();

		grid.path = path;

	}
	//获取距离，对角线距离，有对角线
	int GetDistance(Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}
}
