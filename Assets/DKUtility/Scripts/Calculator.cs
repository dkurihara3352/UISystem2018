using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DKUtility{
	public static class Calculator{
		public static  void CalcSineAndCosine(Vector2 deltaPos, out float sine, out float cosine){
			Vector3 vecA = Vector3.right;
			Vector3 vecB = new Vector3(deltaPos.x, deltaPos.y, 0f);
			Vector3 crossP = Vector3.Cross(vecA, vecB);
			float deltaMag = deltaPos.magnitude;
			float sin = crossP.magnitude / deltaMag;
			if(deltaPos.y < 0f)
				sin *= -1f;
			float cos = Vector2.Dot(Vector2.right, deltaPos) / deltaMag;

			sine = sin;
			cosine = cos;
		}
	}
}
