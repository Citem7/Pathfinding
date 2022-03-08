using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;//引入这个文件是为了使用action东西
//理想情况下，我们想要创建一个能够接受所有请求并将它们分散到多个帧上的系统
//看这个文件的名字就知道是用来管理寻路请求的
public class PathRequestManager : MonoBehaviour {
	//或许这是一个队列，用来存储路径请求
	//就是用来存储路径请求
	Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
	//当前的路径请求
	PathRequest currentPathRequest;
	//我们想要从静态方法中访问东西，我们需要创建一个静态变量
	//实例
	static PathRequestManager instance;
	Pathfinding pathfinding;

	bool isProcessingPath;
	
	//将相同的路径放在不同的帧里，好像是这样
	void Awake() {
		instance = this; 
		//调用查找路径组件
		pathfinding = GetComponent<Pathfinding>();
	}
	//这个action不会立即响应，我们要把请求分散到一些帧上
	//一旦我们实际计算了它的路径，我们就把他存储在一个action中
	//action有两个参数，一个是实际路径的向量数组，一个路径请求是否成功
	public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {
		PathRequest newRequest = new PathRequest(pathStart,pathEnd,callback);
		//将请求添加到队列中,入队
		instance.pathRequestQueue.Enqueue(newRequest);
		instance.TryProcessNext();
	}
	//处理下一个请求
	void TryProcessNext() {
		//如果我们没有正在寻找路径，并且路径查找请求不为0
		if (!isProcessingPath && pathRequestQueue.Count > 0) {
			//当前查找请求 = 队列中第一个，出队操作
			currentPathRequest = pathRequestQueue.Dequeue();
			isProcessingPath = true;
			pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
		}
	}
	//一旦完成了寻径，将会被寻径脚本调用
	public void FinishedProcessingPath(Vector3[] path, bool success) {
		currentPathRequest.callback(path,success);
		//已经完成了，就将正在寻路标志为false
		isProcessingPath = false;
		TryProcessNext();
	}

	struct PathRequest {
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector3[], bool> callback;

		public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback) {
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
		}

	}
}
