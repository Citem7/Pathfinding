using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//我们将从编写一个类开始，该类可以在给定直线上的一个点的情况下创建一条直线，也可以创建一个垂直于该直线的点。
public struct Line {
	//100000
	const float verticalLineGradient = 1e5f;
	//梯度，也就是坡度	
	float gradient;
	//y轴截距
	float y_intercept;
	Vector2 pointOnLine_1;
	Vector2 pointOnLine_2;
	//垂线的梯度
	//float gradientPerpendicular;

	bool approachSide;
	//取一个向量到直线上一点，一个垂直于直线上一点的向量
	public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine) {
		float dx = pointOnLine.x - pointPerpendicularToLine.x;
		float dy = pointOnLine.y - pointPerpendicularToLine.y;

		if (dy == 0) {
			gradient = verticalLineGradient;
		} else {
			gradient = -dx / dy;
		}


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

	public bool HasCrossedLine(Vector2 p) {
		return GetSide (p) != approachSide;
	}

	public void DrawWithGizmos(float length) {
		Vector3 lineDir = new Vector3 (1, 0, gradient).normalized;
		Vector3 lineCentre = new Vector3 (pointOnLine_1.x, 0, pointOnLine_1.y) + Vector3.up;
		Gizmos.DrawLine (lineCentre - lineDir * length / 2f, lineCentre + lineDir * length / 2f);
	}

}
