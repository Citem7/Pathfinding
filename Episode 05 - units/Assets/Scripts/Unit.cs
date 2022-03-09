using UnityEngine;
using System.Collections;
//创建一个unit类，来测试一下
public class Unit : MonoBehaviour {

	//目的地
	public Transform target;
	//速度
	float speed = 20;
	//路径
	Vector3[] path;
	//解释我们什么时候到达那里
	int targetIndex;

	void Start() {
		//当前位置，目标位置，路径
		PathRequestManager.RequestPath(transform.position,target.position, OnPathFound);
	}

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
		if (pathSuccessful) {
			//如果路径成功，路径赋值
			path = newPath;
			targetIndex = 0;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	IEnumerator FollowPath() {
		Vector3 currentWaypoint = path[0];
		while (true) {
			if (transform.position == currentWaypoint) {
				targetIndex ++;
				if (targetIndex >= path.Length) {
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}
			//计算位置
			transform.position = Vector3.MoveTowards(transform.position,currentWaypoint,speed * Time.deltaTime);
			yield return null;

		}
	}
	//画出路径
	public void OnDrawGizmos() {
		if (path != null) {
			for (int i = targetIndex; i < path.Length; i ++) {
				//黑色
				Gizmos.color = Color.black;
				//画出路径点
				Gizmos.DrawCube(path[i], Vector3.one);
				//画线
				if (i == targetIndex) {
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else {
					Gizmos.DrawLine(path[i-1],path[i]);
				}
			}
		}
	}
}
