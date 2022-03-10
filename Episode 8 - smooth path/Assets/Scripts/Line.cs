using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//我们将从编写一个类开始，该类可以在给定直线上的一个点的情况下创建一条直线，也可以创建一个垂直于该直线的点。
//其实很简单就是在原来路径点的基础上，对每个点（路径点在之前已经经过简化）进行处理，画出转弯边界，一旦经过这个边界，就要转弯
//通过控制转弯的速度，就可以控制路径的弯曲程度，也就是路径的平滑程度
public struct Line {
	//100000
	//这么理解吧，梯度也就是斜率，下面这个变量的意义就是用100000来表示垂直的斜率
	const float verticalLineGradient = 1e5f;
	//梯度，梯度就是梯度，梯度和坡度不一样	
	//梯度就像是y = kx + b中的k，像是直线的斜率
	float gradient;
	//y轴截距
	float y_intercept;
	Vector2 pointOnLine_1;
	Vector2 pointOnLine_2;
	//垂线的梯度
	//这个应该就是转弯边界，也就是垂直线的梯度
	//float gradientPerpendicular;
	//现在我们要处理的是，我们需要按照每一个点的转弯边界作为基准，我们的路径相对于一个转弯边界有一个相对斜率，
	//然后这个相对斜率加上转弯边界在世界坐标下的斜率就是路径在世界坐标下的斜率                                            
	bool approachSide;
	//取一个向量到直线上一点，一个垂直于直线上一点的向量
	//构造函数
	//Perpendicular是垂直的
	public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine) {
		float dx = pointOnLine.x - pointPerpendicularToLine.x;
		float dy = pointOnLine.y - pointPerpendicularToLine.y;

		if (dy == 0) {
			gradient = verticalLineGradient;
		} else {
			gradient = -dx / dy;
		}

		//这条线的全部意义在于确定单位是都已经通过一个弯或者边界
		y_intercept = pointOnLine.y - gradient * pointOnLine.x;
		pointOnLine_1 = pointOnLine;
		pointOnLine_2 = pointOnLine + new Vector2 (1, gradient);

		approachSide = false;
		approachSide = GetSide (pointPerpendicularToLine);
	}

	bool GetSide(Vector2 p) {
		//这个数学公式在下一节中会讲述	
		return (p.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y) > (p.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x);
	}
	//我们要做的就是看看这一点是否在直线的另一边
	public bool HasCrossedLine(Vector2 p) {
		return GetSide (p) != approachSide;
	}

	public void DrawWithGizmos(float length) {
		Vector3 lineDir = new Vector3 (1, 0, gradient).normalized;
		Vector3 lineCentre = new Vector3 (pointOnLine_1.x, 0, pointOnLine_1.y) + Vector3.up;
		Gizmos.DrawLine (lineCentre - lineDir * length / 2f, lineCentre + lineDir * length / 2f);
	}

}
