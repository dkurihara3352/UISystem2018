﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility.CurveUtility;
namespace UISystem{
	public interface IScroller: IUIElement{
		void SetRunningElementMotorProcess(IScrollerElementMotorProcess process);
		void ClearScrollerElementMotorProcess(IScrollerElementMotorProcess processToClear);
		bool CheckForDynamicBoundarySnap(float deltaPosOnAxis, int dimension);
	}
	public enum ScrollerAxis{
		Horizontal, Vertical, Both
	}
	public abstract class AbsScroller : AbsUIElement, IScroller{
		public AbsScroller(IScrollerConstArg arg): base(arg){
			thisScrollerAxis = arg.scrollerAxis;
			thisRelativeCursorPosition = arg.relativeCursorPosition;
			thisRubberBandLimitMultiplier = arg.rubberBandLimitMultiplier;

			CacheThisRect();
			MakeSureRectIsSet(thisRect);
			if(thisShouldApplyRubberBand)
				thisRubberBandCalculator = CreateRubberBandCalculator();
		}
		protected Rect thisRect;
		protected Vector2 thisRectLength;
		void CacheThisRect(){
			thisRect = thisUIA.GetRect();
			thisRectLength = new Vector2(thisRect.width, thisRect.height);
		}
		protected void MakeSureRectIsSet(Rect rect){
			if(rect.width == 0f || rect.height == 0f)
				throw new System.InvalidOperationException("rect has at least one dimension not set right");
		}
		readonly  ScrollerAxis thisScrollerAxis;
		/* Rubber */
		readonly protected Vector2 thisRubberBandLimitMultiplier;
		protected abstract bool thisShouldApplyRubberBand{get;}// simply return true if wanna apply
		RubberBandCalculator[] thisRubberBandCalculator;
		RubberBandCalculator[] CreateRubberBandCalculator(){
			RubberBandCalculator[] result = new RubberBandCalculator[2];
			for(int i = 0; i < 2; i++){
				result[i] = new RubberBandCalculator(1f, thisRubberBandLimitMultiplier[i] * thisRectLength[i]);
			}
			return result;
		}
		/* Activation */
		public override void ActivateImple(){
			SetUpCursorTransform();
			SetTheOnlyChildAsScrollerElement();
			CacheScrollerElementRect();
			InitializeScrollerElementForActivation();
			base.ActivateImple();
		}
		/* Cursor Transform */
		void SetUpCursorTransform(){
			thisCursorLength = CalcCursorLength();
			ClampCursorLengthToThisRect();
			thisCursorLocalPosition = CalcCursorLocalPos();
		}
		protected Vector2 thisCursorLength;
		readonly protected Vector2 thisRelativeCursorPosition;
		protected Vector2 thisCursorLocalPosition;
		protected abstract Vector2 CalcCursorLength();
		void ClampCursorLengthToThisRect(){
			for(int i = 0; i < 2; i ++){
				if(thisCursorLength[i] > thisRectLength[i])
					thisCursorLength[i] = thisRectLength[i];
			}
		}
		protected virtual Vector2 CalcCursorLocalPos(){
			Vector2 result = new Vector2();
			for(int i = 0; i < 2; i ++){
				float scrollerRectLength = thisRectLength[i];
				float cursorLength = thisCursorLength[i];
				float diffL = scrollerRectLength - cursorLength;

				float localPos;
				if(thisRelativeCursorPosition[i] == 0f) 
					localPos = 0f;
				else
					localPos = thisRelativeCursorPosition[i] * diffL;
				result[i] = localPos;
			}
			return result;
		}
		protected IUIElement thisScrollerElement;
		protected void SetTheOnlyChildAsScrollerElement(){
			List<IUIElement> childUIEs = GetChildUIEs();
			if(childUIEs == null)
				throw new System.NullReferenceException("childUIEs must not be null");
			if(childUIEs.Count != 1)
				throw new System.InvalidOperationException("Scroller must have only one UIE child as Scroller Element");
			if(childUIEs[0] == null)
				throw new System.InvalidOperationException("Scroller's only child must not be null");
			thisScrollerElement = childUIEs[0];
		}
		protected Rect thisScrollerElementRect;
		protected Vector2 thisScrollerElementLength;
		void CacheScrollerElementRect(){
			IUIAdaptor elementAdaptor = thisScrollerElement.GetUIAdaptor();
			thisScrollerElementRect = elementAdaptor.GetRect();
			thisScrollerElementLength = new Vector2(thisScrollerElementRect.width, thisScrollerElementRect.height);
		}
		protected void InitializeScrollerElementForActivation(){
			Vector2 initialCursorValue = GetInitialPositionNormalizedToCursor();
			PlaceScrollerElement(initialCursorValue);
		}
		protected abstract Vector2 GetInitialPositionNormalizedToCursor();
		/* Drag */
		protected int thisDragAxis = -1;
		protected bool thisDragAxisIsNotDetermined{get{return thisDragAxis == -1;}}
		protected bool thisDragAxisIsHorizontal{get{return thisDragAxis == 0;}}
		protected bool thisDragAxisIsVertical{get{return thisDragAxis == 1;}}
		protected bool thisProcessedDrag;
		protected override void OnReleaseImple(){
			/*  implement spring
				snap to either end if offset, with initial velocity
				overriden by UIElementGroupScroller to implement search snap
			*/
			if(thisProcessedDrag){
				thisProcessedDrag = false;
				CheckForStaticBoundarySnap(thisDragAxis);
			}else
				base.OnReleaseImple();
			if(!thisDragAxisIsNotDetermined)
				ResetDragAxis();
		}
		void ResetDragAxis(){
			thisDragAxis = -1;
		}
		protected override void OnDragImple(ICustomEventData eventData){
			if(thisDragAxisIsNotDetermined){
				int dragAxis = CalcDragAxis(eventData.deltaP);
				thisDragAxis = dragAxis;
				EvaluateShouldProcessDrag();
			}
			if(thisShouldProcessDrag){
				Vector2 deltaV2AlongAxis = GetDeltaV2AlongDragAxis(eventData.deltaP);
				DragImpleInner(eventData.position, deltaV2AlongAxis);
			}else{
				base.OnDragImple(eventData);
			}
		}
		int CalcDragAxis(Vector2 deltaP){
			if(deltaP.x >= deltaP.y)
				return 0;
			else
				return 1;
		}
		protected bool thisShouldProcessDrag;
		void EvaluateShouldProcessDrag(){
			if(thisScrollerAxis == ScrollerAxis.Both)
				thisShouldProcessDrag = true;
			else{
				if(thisScrollerAxis == ScrollerAxis.Horizontal)
					thisShouldProcessDrag = thisDragAxisIsHorizontal;
				else if(thisScrollerAxis == ScrollerAxis.Vertical)
					thisShouldProcessDrag = thisDragAxisIsVertical;
				else
					throw new System.InvalidOperationException("dragAxis should not be None, should be already evaluated");
			}
			if(thisShouldProcessDrag)
				thisProcessedDrag = true;
		}
		Vector2 GetDeltaV2AlongDragAxis(Vector2 deltaP){
			if(thisDragAxisIsHorizontal)
				return new Vector2(deltaP.x, 0f);
			else
				return new Vector2(0f, deltaP.y);
		}
		protected virtual void DragImpleInner(Vector2 position, Vector2 deltaP){
			Vector2 newElementLocalPosition = thisScrollerElement.GetLocalPosition() + deltaP;
			if(thisShouldApplyRubberBand){
				if(thisRequiresToCheckForHorizontalAxis){
					newElementLocalPosition[0] = CheckAndApplyRubberBand(deltaP[0], newElementLocalPosition[0], 0);
				}
				if(thisRequiresToCheckForVerticalAxis){
					newElementLocalPosition[1] = CheckAndApplyRubberBand(deltaP[1], newElementLocalPosition[1], 1);
				}
			}
			thisScrollerElement.SetLocalPosition(newElementLocalPosition);
		}
		float CheckAndApplyRubberBand(float deltaPOnAxis, float elementLocalPosOnAxis, int dimension){
			float result = elementLocalPosOnAxis;
			if(ElementIsScrolledToIncreaseCursorOffset(deltaPOnAxis, elementLocalPosOnAxis, dimension)){
				result = CalcRubberBandedPosOnAxis(elementLocalPosOnAxis, dimension);
			}
			return result;
		}
		protected bool ElementIsScrolledToIncreaseCursorOffset(float deltaPosOnAxis, float elementLocalPosOnAxis, int dimension){
			float cursorOffsetInPixel = GetElementCursorOffsetInPixel(elementLocalPosOnAxis, dimension);
			if(cursorOffsetInPixel == 0f)
				return false;
			else{
				if(cursorOffsetInPixel < 0f)//too right
					return deltaPosOnAxis > 0f;
				else//displacement > 0f: too left
					return deltaPosOnAxis < 0f;
			}
		}
		protected bool thisRequiresToCheckForHorizontalAxis{
			get{return thisScrollerAxis == ScrollerAxis.Horizontal || thisScrollerAxis == ScrollerAxis.Both;}
		}
		protected bool thisRequiresToCheckForVerticalAxis{
			get{return thisScrollerAxis == ScrollerAxis.Vertical || thisScrollerAxis == ScrollerAxis.Both;}
		}
		protected float CalcRubberBandedPosOnAxis(float localPosOnAxis, int dimension){
			float cursorMin = thisCursorLocalPosition[dimension];
			float cursorOffsetInPixel = GetElementCursorOffsetInPixel(localPosOnAxis, dimension);
			bool doesInvert = cursorOffsetInPixel < 0f? true: false;
			float rubberedValue = thisRubberBandCalculator[dimension].CalcRubberBandValue(cursorOffsetInPixel, invert: doesInvert); 
			float basePoint;
			if(cursorOffsetInPixel < 0f)
				basePoint = cursorMin;
			else
				basePoint = cursorMin + thisCursorLength[dimension] - thisScrollerElementLength[dimension];
			
			return basePoint - rubberedValue;
		}
		/* Misc */
		protected bool ElementIsUndersizedTo(Vector2 referenceLength, int dimension){
			return thisScrollerElementLength[dimension] <= referenceLength[dimension];
		}
		protected float GetNormalizedPosition(float elementLocalPosOnAxis, Vector2 referenceLength, Vector2 referenceMin, int dimension){
			/*  (0f, 0f) if cursor rests on top left corner of the element
				(1f, 1f) if cursor rests on bottom right corner of the element
				value below 0f and over 1f indicates the element's displacement beyond cursor bounds
			*/
			if(ElementIsUndersizedTo(referenceLength, dimension)){
				return 0f;
			}else{
				float referenceLengthOnAxis = referenceLength[dimension];
				float referenceMinOnAxis = referenceMin[dimension];
				return (referenceMinOnAxis - elementLocalPosOnAxis)/ (thisScrollerElementLength[dimension] - referenceLengthOnAxis);

			}
		}
		protected float GetNormalizedCursoredPosition(float elementLocalPosOnAxis, int dimension){
			return GetNormalizedPosition(elementLocalPosOnAxis, thisCursorLength, thisCursorLocalPosition, dimension);
		}
		protected float GetNormalizedScrollerPosition(float elementLocalPosOnAxis, int dimension){
			return GetNormalizedPosition(elementLocalPosOnAxis, thisRectLength, Vector2.zero, dimension);
		}
		protected float GetElementCursorOffsetInPixel(float elementLocalPosOnAxis, int dimension){
			/* used to calculate rubberbanding */
			if(ElementIsUndersizedTo(thisCursorLength, dimension)){
				return thisCursorLocalPosition[dimension] - elementLocalPosOnAxis;
			}
			else{
				float elePosNormToCursor = GetNormalizedCursoredPosition(elementLocalPosOnAxis, dimension);
				float normalizedOffset = elePosNormToCursor;
				if(elePosNormToCursor <= 1f && elePosNormToCursor >= 0f)
					normalizedOffset = 0f;
				else if(elePosNormToCursor > 1f)
					normalizedOffset = elePosNormToCursor - 1f;
				return normalizedOffset * thisCursorLength[dimension];
			}
		}
		protected float GetNormalizedCursoredPositionFromPosInElementSpace(float positionInElementSpaceOnAxis, int dimension){
			float prospectiveElementLocalPosOnAxis = thisCursorLocalPosition[dimension] - positionInElementSpaceOnAxis;
			return GetNormalizedCursoredPosition(prospectiveElementLocalPosOnAxis, dimension);
		}
		protected void PlaceScrollerElement(Vector2 targetCursorValue){
			Vector2 newLocalPos = CalcLocalPositionFromNormalizedCursoredPosition(targetCursorValue);
			this.SetLocalPosition(newLocalPos);
		}
		protected Vector2 CalcLocalPositionFromNormalizedCursoredPosition(Vector2 normalizedCursoredPosition){
			Vector2 result = new Vector2();
			for(int i = 0; i < 2; i ++){
				result[i] = CalcLocalPositionFromNormalizedCursoredPositionOnAxis(normalizedCursoredPosition[i], i);
			}
			return result;
		}
		protected float CalcLocalPositionFromNormalizedCursoredPositionOnAxis(float normalizedCursoredPositionOnAxis, int dimension){
			if(ElementIsUndersizedTo(thisCursorLength, dimension))
				return thisCursorLocalPosition[dimension];
			else{
				float elementLocalPosOnAxis = thisCursorLocalPosition[dimension] - (normalizedCursoredPositionOnAxis * (thisScrollerElementLength[dimension] - thisCursorLength[dimension]));
				return elementLocalPosOnAxis;
			}
		}
		/* Swipe */
		protected override void OnSwipeImple(ICustomEventData eventData){
			if(thisProcessedDrag){
				thisProcessedDrag = false;

				int dimension = thisDragAxis;
				float deltaPosOnAxis = eventData.deltaP[dimension];
				if(!CheckForDynamicBoundarySnap(deltaPosOnAxis, dimension)){
					if(thisIsEnabledInertia)
						StartInertialScroll(deltaPosOnAxis, dimension);
				}
			}else
				base.OnSwipeImple(eventData);
			if(!thisDragAxisIsNotDetermined)
				ResetDragAxis();
		}
		readonly bool thisIsEnabledInertia;

		/*  IScrollerElementMotorProcess
				Disable input of all the elements below ScrollerElement! otherwise can't touch to stop the scroll
				ScrollerElementSnapProcess
					spring coefficient (suppleness) is constant acrosss all setup?
						the more the initial velocity, the longer it takes to settle
						the more the displacement, the longer it takes to settle
		*/
		protected void SnapTo(float targetNormalizedCursoredPosOnAxis, float initVelOnAxis, int dimension){
			float targetElementLocalPosOnAxis = CalcLocalPositionFromNormalizedCursoredPositionOnAxis(targetNormalizedCursoredPosOnAxis, dimension);
			IScrollerElementSnapProcess newProcess = thisProcessFactory.CreateScrollerElementSnapProcess(this, thisScrollerElement, targetElementLocalPosOnAxis, initVelOnAxis, dimension);
			newProcess.Run();
		}
		IScrollerElementMotorProcess thisRunningScrollerMotorProcess;
		public void SetRunningElementMotorProcess(IScrollerElementMotorProcess process){
			StopRunningElementMotorProcess();
			thisRunningScrollerMotorProcess = process;
		}
		public void ClearScrollerElementMotorProcess(IScrollerElementMotorProcess processToClear){
			if(thisRunningScrollerMotorProcess == processToClear)
				SetRunningElementMotorProcess(null);
		}
		void StopRunningElementMotorProcess(){
			if(thisRunningScrollerMotorProcess != null)
				thisRunningScrollerMotorProcess.Stop();
		}
		protected void StartInertialScroll(float deltaPosOnAxis, int dimension){
			/*  start a process
					the process is overriden by UIEGroupScroller which requires to check searching snap target element when scroll delta gets below threshold
					the process
						translate the scroller element in a direction with decreasing velocity
						each frame check to see if there's cursor offset or not
							if there is stop the process and snap to it with initial velocity
			*/
			IInertialScrollProcess process = thisProcessFactory.CreateInertialScrollProcess(deltaPosOnAxis, this, thisScrollerElement, dimension);
			process.Run();
		}
		public bool CheckForDynamicBoundarySnap(float deltaPosOnAxis, int dimension){
			float elementLocalPosOnAxis = thisScrollerElement.GetLocalPosition()[dimension];
			if(deltaPosOnAxis != 0f)
				if(ElementIsScrolledToIncreaseCursorOffset(deltaPosOnAxis, elementLocalPosOnAxis, dimension)){
					float snapTargetNormPos;
					if(deltaPosOnAxis > 0f)
						snapTargetNormPos = 0f;
					else	
						snapTargetNormPos = 1f;
					SnapTo(snapTargetNormPos, deltaPosOnAxis, dimension);
					return true;
				}
			return false;
		}
		protected bool CheckForStaticBoundarySnap(int dimension){
			float elementLocalPosOnAxis = thisScrollerElement.GetLocalPosition()[dimension];
			float cursorOffset = GetElementCursorOffsetInPixel(elementLocalPosOnAxis, dimension);
				if(cursorOffset != 0f){
					float snapTargetNormPos;
					if(cursorOffset < 0f)
						snapTargetNormPos = 0f;
					else
						snapTargetNormPos = 1f;
					SnapTo(snapTargetNormPos, 0f, dimension);
					return true;
				}
			return false;
		}
		/* Touch */
		protected override void OnTouchImple(int touchCount){
			/*  Stop scroll if running
			*/
			StopRunningElementMotorProcess();
		}
	}



	public interface IScrollerConstArg: IUIElementConstArg{
		ScrollerAxis scrollerAxis{get;}
		Vector2 relativeCursorPosition{get;}
		Vector2 rubberBandLimitMultiplier{get;}
		bool isEnabledInertia{get;}
	}


	public abstract class ScrollerConstArg: UIElementConstArg, IScrollerConstArg{
		public ScrollerConstArg(ScrollerAxis scrollerAxis, Vector2 relativeCursorPosition, Vector2 rubberBandLimitMultiplier, bool isEnabledInertia, IUIManager uim, IUISystemProcessFactory processFactory, IUIElementFactory uieFactory, IScrollerAdaptor uia, IUIImage uiImage): base(uim, processFactory, uieFactory, uia, uiImage){
			thisScrollerAxis = scrollerAxis;
			thisRelativeCursorPos = ClampVector2ZeroToOne(relativeCursorPosition);
			thisRubberBandLimitMultiplier = MakeRubberBandLimitMultiplierInRange(rubberBandLimitMultiplier);
			thisIsEnabledInertia = isEnabledInertia;
		}
		readonly ScrollerAxis thisScrollerAxis;
		public ScrollerAxis scrollerAxis{
			get{return thisScrollerAxis;}
		}
		readonly Vector2 thisRelativeCursorPos;
		public Vector2 relativeCursorPosition{get{return thisRelativeCursorPos;}}
		readonly Vector2 thisRubberBandLimitMultiplier;
		public Vector2 rubberBandLimitMultiplier{get{return thisRubberBandLimitMultiplier;}}
		const float thisMinimumRubberBandMultiplier = .01f;
		Vector2 MakeRubberBandLimitMultiplierInRange(Vector2 source){
			Vector2 result = new Vector2(source.x, source.y);
			for(int i = 0; i < 2; i++)
				if(result[i] <= 0f)
					result[i] = thisMinimumRubberBandMultiplier;
				else if(result[i] > 1f)
					result[i] = 1f;
			return result;
		}
		readonly bool thisIsEnabledInertia;
		public bool isEnabledInertia{get{return thisIsEnabledInertia;}}
	}
}
