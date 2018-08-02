using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IMBAdaptor{
		Transform GetTransform();
		Rect GetRect();
		void SetRectLength(float width, float height);
		Vector2 GetLocalPosition();
		void SetLocalPosition(Vector2 localPos);
		Vector2 GetWorldPosition();
		void SetWorldPosition(Vector2 worldPos);
		Vector2 GetPositionInThisSpace(Vector2 worldPos);
	}

	public interface IUIAdaptor: IMBAdaptor {
		void GetReadyForActivation(IUIAActivationData passedData);
		IUIElement GetUIElement();
		IUIElement GetParentUIE();
		void SetParentUIE(IUIElement newParentUIE, bool worldPositionStays);
		List<IUIElement> GetAllOffspringUIEs();
		List<IUIElement> GetChildUIEs();
		IUIAActivationData GetDomainActivationData();
		void ActivateUIElement();
	}
}
