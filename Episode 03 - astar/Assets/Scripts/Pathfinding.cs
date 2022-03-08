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
		//不好好看代码，seeker就是起点啊，看FindPath的函数
		FindPath (seeker.position, target.position);//seeker这个点应该不是起点，是每次都会更新的点，那会是什么呢
	}

	void FindPath(Vector3 startPos, Vector3 targetPos) {
		//根据位置获得网格，就是把坐标转换成网格
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);
		//根据伪代码，先创建两个表，open表和close表
		List<Node> openSet = new List<Node>();//open表
		//散列集合HashSet是一种数据结构，支持泛型。
		//HashSet包含一组不重复出现且无特性顺序的元素
		//HashSet的值不重复且没有顺序，容量会按需自动增加。
		//这个类主要是设计用来做高性能集运算的，例如对两个集合求交集、差集、并集、补集等。
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);
		//这一步是在遍历整个open表去找最小的值，就在这里进行优化，小顶堆
		//这个循环在找f值最小的那个，但是为什么要比较h值呢。会不会是因为计算g值会出现偏差，导致g值可能会比真实值偏大
		while (openSet.Count > 0) {
			Node node = openSet[0];			
			for (int i = 1; i < openSet.Count; i ++) {
				//因为当f值相同的时候，就比较h值，因为h值记录的估计值，也就是距离目标点的距离
				if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost) {
					if (openSet[i].hCost < node.hCost)
						node = openSet[i];
				
				//上面这两个if语句可以合并成一个
				//就是要找到f值最小的那个，但是如果f值相同，那么我们就选择估计值最小的那个
				//if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost && openSet[i].hCost < node.hCost)
				//	node = openSet[i];
			}
			//将该点从open表移入closed表
			openSet.Remove(node);
			closedSet.Add(node);
			//到达目标点，提前结束
			//如果选择的点是目标节点，那么直接结束
			if (node == targetNode) {
				RetracePath(startNode,targetNode);
				return;
			}
			//遍历该点的邻居
			foreach (Node neighbour in grid.GetNeighbours(node)) {
				//如果该邻居是障碍物或者在closed集中已经存在，跳过
				if (!neighbour.walkable || closedSet.Contains(neighbour)) {
					continue;
				}
				//当前点的g值加上当前点到邻居点的距离，有两种距离：10或14（斜着）
				//新的到达邻居的距离，也就是如果到达邻居这个节点的距离变短啦，那么就是更新这个距离
				int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
				//newCostToNeighbour < neighbour.gCost会出现这种情况么，肯定会出现的，因为现在是有对角线这个方向，也就是现在有8个方向
				//如果到达邻居的距离变短或者这个邻居节点没有出现在open表中，是一个新的节点，那么就要把他更新距离还有parent节点
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
	//当我们到达终点，我们就根据每个结点的parent值，把路径绘制出来，但是绘制出的路径是倒着的
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
