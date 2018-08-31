using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IMBAdaptor{
		Transform GetTransform();
		Rect GetRect();
		void SetRectLength(Vector2 length);
		Vector2 GetLocalPosition();
		void SetLocalPosition(Vector2 localPos);
		string GetName();
	}

	public interface IUIAdaptor: IMBAdaptor {
		void GetReadyForActivation(
			IUIAdaptorBaseInitializationData data, 
			bool recursively
		);
		void UpdateUIAdaptorHiearchy(
			bool recursively
		);
		void InitializeUIAdaptor(
			IUIAdaptorBaseInitializationData initData,
			bool recursively
		);
		void CreateAndSetUIElement(
			bool recursively
		);
		void SetUpUIElementReference(
			bool recursively
		);
		void CompleteUIElementReferenceSetUp(
			bool recursively
		);

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
