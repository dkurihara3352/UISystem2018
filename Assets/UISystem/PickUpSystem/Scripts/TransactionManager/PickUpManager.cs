using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IPickUpManager{
		IPickableUIE GetPickedUIE();
		void ClearPickedUIE();
		IPickUpContextUIE GetPickUpContextUIE();
		Vector2 GetDragWorldPosition();
		void SetDragWorldPosition(Vector2 dragWorldPos);
		float GetDragThreshold();
		float GetSmoothCoefficient();
	}
	public abstract class AbsPickUpManager: IPickUpManager{
		public IPickableUIE GetPickedUIE(){
			return thisPickedUIE;
		}
		protected IPickableUIE thisPickedUIE;
		protected void SetPickedUIE(IPickableUIE pickedUIE){
			thisPickedUIE = pickedUIE;
		}
		public void ClearPickedUIE(){
			thisPickedUIE = null;
		}
		public abstract IPickUpContextUIE GetPickUpContextUIE();
		public Vector2 GetDragWorldPosition(){
			return thisDragWorldPosition;
		}
		Vector2 thisDragWorldPosition;
		public void SetDragWorldPosition(Vector2 dragWorldPos){
			thisDragWorldPosition = dragWorldPos;
		}
		public float GetDragThreshold(){return 1f;}
		public float GetSmoothCoefficient(){return 1f;}
	}
}