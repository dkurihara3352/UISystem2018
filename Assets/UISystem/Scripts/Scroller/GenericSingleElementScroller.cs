using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IGenericSingleElementScroller: IScroller{}
	public class GenericSingleElementScroller: AbsScroller, IGenericSingleElementScroller{
		public GenericSingleElementScroller(IGenericScrollerConstArg arg): base(arg){
		}
		protected override bool thisShouldApplyRubberBand{
			get{return true;}
		}
		protected override void InitializeSelectabilityState(){
			BecomeSelectable();
		}
		protected override Vector2 CalcCursorDimension(IScrollerConstArg arg){
			IGenericScrollerConstArg genericArg = (IGenericScrollerConstArg)arg;
			Vector2 relativeCursorSize = genericArg.relativeCursorSize;
			Rect thisRect = GetUIAdaptor().GetRect();
			float cursorWidth = thisRect.width * relativeCursorSize.x;
			float cursorHeight = thisRect.height * relativeCursorSize.y;
			return new Vector2(cursorWidth, cursorHeight);
		}
	}
	public interface IGenericScrollerConstArg: IScrollerConstArg{
		Vector2 relativeCursorSize{get;}
	}
	public class GenericScrollerConstArg: AbsScrollerConstArg, IGenericScrollerConstArg{
		public GenericScrollerConstArg(Vector2 relativeCursorSize, Vector2 relativeCursorPosition, IUIManager uim, IUISystemProcessFactory processFactory, IUIElementFactory uieFactory, IGenericScrollerAdaptor uia, IUIImage image):base(relativeCursorPosition, uim, processFactory, uieFactory, uia, image){
			thisRelativeCursorSize = MakeRelativeCursorSizeInRange(relativeCursorSize);
		}
		Vector2 MakeRelativeCursorSizeInRange(Vector2 source){
			Vector2 newSizeV2 = new Vector2();
			for(int i = 0; i < 2; i ++){
				float newSize = Mathf.Clamp01(source[i]);
				if(newSize == 0)
					newSize = 1f;
				newSizeV2[i] = newSize;
			}
			return newSizeV2;
		}
		protected Vector2 thisRelativeCursorSize;
		public Vector2 relativeCursorSize{
			get{return thisRelativeCursorSize;}
		}
	}
	public interface IGenericScrollerAdaptor: IScrollerAdaptor{}
}
