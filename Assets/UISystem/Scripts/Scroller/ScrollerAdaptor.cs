using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IScrollerAdaptor: IUIAdaptor{
		void ShowCursorRectInGUI(Rect cursorRect);
	}
	public abstract class AbsScrollerAdaptor<T>: UIAdaptor, IScrollerAdaptor where T: class, IScroller{
		public ScrollerAxis scrollerAxis;
		public Vector2 relativeCursorPosition = new Vector2(.5f, .5f);
		public Vector2 rubberBandLimitMultiplier = new Vector2(.1f ,.1f);
		public bool isEnabledInertia = true;
		public float locksInputAboveThisVelocity = 200f;
		public void ShowCursorRectInGUI(Rect cursorRect){
			thisCursorRect = cursorRect;
			cursorRectIsReady = true;
		}
		Rect thisCursorRect;
		bool cursorRectIsReady = false;
		IScroller thisScroller{
			get{
				return (IScroller)GetUIElement();
			}
		}

		protected override void CompleteUIElementReferenceSetUpImple(){
			// thisScroller.UpdateGroupElementLengthAndPadding(thisElementLength, thisPadding); this is done is subclass
			thisScroller.SetUpScrollerElement();
		}
		public void OnDrawGizmosSelected(){
			if(cursorRectIsReady){
				Gizmos.color = Color.red;
				float planeZ = -1f;
				Vector3 bottomLeft = new Vector3(thisCursorRect.x + transform.position.x, thisCursorRect.y + transform.position.y, planeZ);
				Vector3 bottomRight = bottomLeft + Vector3.right * thisCursorRect.width;
				Vector3 topLeft = bottomLeft + Vector3.up * thisCursorRect.height;
				Vector3 topRight = topLeft + Vector3.right * thisCursorRect.width;
				Gizmos.DrawLine(topLeft, topRight);
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(topRight, bottomRight);
				Gizmos.DrawLine(topLeft, bottomLeft);
				Gizmos.DrawLine(bottomLeft, bottomRight);
			}
		}
	}
}
