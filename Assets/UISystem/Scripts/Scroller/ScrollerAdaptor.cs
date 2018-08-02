using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IScrollerAdaptor: IUIAdaptor{
		void ShowCursorRectInGUI(Rect cursorRect);
	}
	public abstract class AbsScrollerAdaptor<T>: UIAdaptor, IScrollerAdaptor where T: class, IScroller{
		public ScrollerAxis scrollerAxis;
		public Vector2 relativeCursorPosition;
		public Vector2 rubberBandLimitMultiplier;
		public bool isEnabledInertia;
		public void ShowCursorRectInGUI(Rect cursorRect){
			thisCursorRect = cursorRect;
			cursorRectIsReady = true;
		}
		Rect thisCursorRect;
		bool cursorRectIsReady = false;
		public void OnDrawGizmos(){
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
		void OnGUI(){
			if(cursorRectIsReady){
				IScroller scroller = this.GetUIElement() as IScroller;
				IUIElement scrollerElement = scroller.GetChildUIEs()[0];
				Vector2 elementLocalPos = scrollerElement.GetLocalPosition();
				Vector2 cursorOffset = new Vector2(scroller.GetElementCursorOffsetInPixel(elementLocalPos.x, 0), scroller.GetElementCursorOffsetInPixel(elementLocalPos.y, 1));
				Vector2 rectPos = new Vector2(10f, 10f);
				Vector2 rectLength = new Vector2(300f, 20f);
				GUI.Label(new Rect(rectPos, rectLength), "CursorOffset: " + cursorOffset.ToString());
				GUI.Label(new Rect(new Vector2(10f, 30f), rectLength), "rubberMult: " + scroller.rubberBandLimitMultiplier.ToString());
				GUI.Label(new Rect(new Vector2(10f, 50f), rectLength), "rubberLimit: " + scroller.rubberLimit.ToString());
			}
		}
	}
}
