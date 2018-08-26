using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IGenericSingleElementScroller: IScroller{}
	public class GenericSingleElementScroller: AbsScroller, IGenericSingleElementScroller{
		public GenericSingleElementScroller(IGenericSingleElementScrollerConstArg arg): base(arg){
			thisRelativeCursorLength = MakeRelativeCursorLengthInRange(arg.relativeCursorLength);
		}
		Vector2 MakeRelativeCursorLengthInRange(Vector2 source){
			Vector2 newSizeV2 = new Vector2(source.x, source.y);
			for(int i = 0; i < 2; i ++){
				if(newSizeV2[i] <= 0f)
					throw new System.InvalidOperationException("relativeCursorLength must be greater than 0");
				else if(newSizeV2[i] > 1f)
					newSizeV2[i] = 1f;
			}
			return newSizeV2;
		}
		protected readonly Vector2 thisRelativeCursorLength;
		protected override bool[] thisShouldApplyRubberBand{
			get{return new bool[]{true, true};}
		}
		protected override Vector2 GetInitialNormalizedCursoredPosition(){
			return Vector2.zero;
		}
		protected override Vector2 CalcCursorLength(){
			Vector2 relativeCursorLength = thisRelativeCursorLength;
			float cursorWidth = thisRect.width * relativeCursorLength.x;
			float cursorHeight = thisRect.height * relativeCursorLength.y;
			return new Vector2(cursorWidth, cursorHeight);
		}
	}


	
	public interface IGenericSingleElementScrollerConstArg: IScrollerConstArg{
		Vector2 relativeCursorLength{get;}
	}
	public class GenericSingleElementScrollerConstArg: ScrollerConstArg, IGenericSingleElementScrollerConstArg{
		public GenericSingleElementScrollerConstArg(
			Vector2 relativeCursorLength, 
			ScrollerAxis scrollerAxis, 
			Vector2 rubberBandLimitMultiplier, 
			Vector2 relativeCursorPosition, 
			bool isEnabledInertia, 
			float newScrollSpeedThreshold,

			IUIManager uim, 
			IUISystemProcessFactory processFactory, 
			IUIElementFactory uieFactory, 
			IGenericSingleElementScrollerAdaptor uia, 
			IUIImage image,
			ActivationMode activationMode
			
		):base(
			scrollerAxis, 
			relativeCursorPosition, 
			rubberBandLimitMultiplier, 
			isEnabledInertia, 
			newScrollSpeedThreshold,

			uim, 
			processFactory, 
			uieFactory, 
			uia, 
			image,
			activationMode
		){
			thisRelativeCursorLength = relativeCursorLength;
		}
		protected Vector2 thisRelativeCursorLength;
		public Vector2 relativeCursorLength{
			get{return thisRelativeCursorLength;}
		}
	}

}
