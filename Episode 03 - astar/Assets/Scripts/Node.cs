using UnityEngine;
using System.Collections;

public class Node {
	//是否是障碍物
	public bool walkable;
	//世界坐标
	public Vector3 worldPosition;
	//网格坐标
	public int gridX;
	public int gridY;
   	//离起点的距离，已经走得距离
	public int gCost;
	//离终点的距离，估计值
	public int hCost;
	//回溯路径，记录的父节点
	public Node parent;
	//构造函数
	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) {
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}
	//计算f值
	public int fCost {
		get {
			return gCost + hCost;
		}
	}
}
