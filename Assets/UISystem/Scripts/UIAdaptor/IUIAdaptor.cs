using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IMBAdaptor{
		Transform GetTransform();
		Rect GetRect();
		void SetRectLengthOnAxis(float length, int dimension);
		Vector2 GetLocalPosition();
		void SetLocalPosition(Vector2 localPos);
		string GetName();
	}

	public interface IUIAdaptor: IMBAdaptor {
		void UpdateUIAdaptorHiearchyRecursively();
		void InitializeUIAdaptorRecursively(IUIAdaptorBaseInitializationData initData);
		void CreateAndSetUIElementRecursively();
		void SetUpUIElementReferenceRecursively();
		void CompleteUIElementReferenceSetUpRecursively();

		IUIElement GetUIElement();
		IUIElement GetParentUIE();
		void SetParentUIE(IUIElement newParentUIE, bool worldPositionStays);
		List<IUIElement> GetAllOffspringUIEs();
		List<IUIElement> GetChildUIEs();
		IUIAdaptorBaseInitializationData GetDomainInitializationData();
		void SetUpCanvasGroupComponent();
		float GetGroupAlpha();
		void SetGroupAlpha(float alpha);
	}
}
