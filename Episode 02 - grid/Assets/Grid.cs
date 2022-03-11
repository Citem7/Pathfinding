using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

	public LayerMask unwalkableMask;//障碍物层
	public Vector2 gridWorldSize;//
	public float nodeRadius;//格子半径
	Node[,] grid;

	float nodeDiameter;
	//网格的格子数
	int gridSizeX, gridSizeY;

	void Start() {
		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);//取整数
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
		CreateGrid();
	}

	void CreateGrid() {
		grid = new Node[gridSizeX,gridSizeY];
		//worldBottomleft是世界空间的左下角
		//世界空间就是Unity最大的空间，可以创建一个无任何父节点的空对象，其position,rotation分量都是0，scale分量都是1，
		//那么可以认为世界空间就是以此物体中心点为原点，物体的朝向为z轴，右方向为x轴，上方向为y轴，
		//即物体的transform.forward为z轴，transform.right为x轴，transform.up为y轴,
		//也就是transform.position是中心点，要获得世界空间的左下角，需要减去x轴右半部分（或加上x轴左半部分vector3.left），减去y轴上半部分（或加上y轴下半部分vector3.back）

		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;
	//遍历所有的格子，不是遍历所有的格子，是挨个创建所有的格子，Physics.CheckSphere（），如果有任何碰撞体与世界坐标系中由 position 和 radius 界定的球体重叠，则返回 true。
		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius,unwalkableMask));
				grid[x,y] = new Node(walkable,worldPoint);//创建的格子有两个属性，walkable能否穿过，worldPoint就是坐标
			}
		}
	}
	//从函数名字我们就能知道，从世界坐标变成格子
	//先将世界坐标变成百分比，然后将百分比乘以格子总数，就得到格子的位置
	//Vector3 worldPosition=>grid[x,y]
	public Node NodeFromWorldPoint(Vector3 worldPosition) {
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		return grid[x,y];
	}
	//应该是绘制这个图
	void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));

	
		if (grid != null) {
			foreach (Node n in grid) {
				Gizmos.color = (n.walkable)?Color.white:Color.red;//能走的格子是白色，不能走的是红色
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));//??
			}
		}
	}
}
//格子类
public class Node {
	//两个属性
	public bool walkable;
	public Vector3 worldPosition;
	//构造函数
	public Node(bool _walkable, Vector3 _worldPos) {
		walkable = _walkable;
		worldPosition = _worldPos;
	}
}
