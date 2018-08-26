using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IMBAdaptor{
		Transform GetTransform();
		Rect GetRect();
		void SetRectLength(float width, float height);
		void SetRectLengthOnAxis(float length, int dimension);
		Vector2 GetLocalPosition();
		void SetLocalPosition(Vector2 localPos);
		Vector2 GetWorldPosition();
		void SetWorldPosition(Vector2 worldPos);
		Vector2 GetPositionInThisSpace(Vector2 worldPos);
		string GetName();
	}

	public interface IUIAdaptor: IMBAdaptor {
		void GetReadyForActivation(IUIElementBaseConstData passedData);
		IUIElement GetUIElement();
		IUIElement GetParentUIE();
		void SetParentUIE(IUIElement newParentUIE, bool worldPositionStays);
		List<IUIElement> GetAllOffspringUIEs();
		List<IUIElement> GetChildUIEs();
		IUIElementBaseConstData GetDomainActivationData();
		void SetUpCanvasGroupComponent();
		float GetGroupAlpha();
		void SetGroupAlpha(float alpha);
	}
}
