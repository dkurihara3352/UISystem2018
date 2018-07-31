using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility.CurveUtility;
namespace UISystem{
	public interface IScroller: IUIElement{
		void SwitchRunningElementMotorProcess(IScrollerElementMotorProcess process, int dimension);
		void ClearScrollerElementMotorProcess(IScrollerElementMotorProcess processToClear, int dimension);
		bool CheckForDynamicBoundarySnap(float deltaPosOnAxis, int dimension);
		void SetScrollerElementLocalPosOnAxis(float localPosOnAxis, int dimension);
	}
	public enum ScrollerAxis{
		Horizontal, Vertical, Both
	}
	public abstract class AbsScroller : AbsUIElement, IScroller{
		public AbsScroller(IScrollerConstArg arg): base(arg){
			thisScrollerAxis = arg.scrollerAxis;
			thisRelativeCursorPosition = MakeSureRelativeCursorPosIsClampedZeroToOne(arg.relativeCursorPosition);
			thisRubberBandLimitMultiplier = MakeRubberBandLimitMultiplierInRange(arg.rubberBandLimitMultiplier);
			thisIsEnabledInertia = arg.isEnabledInertia;

			CacheThisRect();
			MakeSureRectIsSet(thisRect);
			for(int i = 0; i < 2; i ++)
				if(thisShouldApplyRubberBand[i])
					thisRubberBandCalculator = CreateRubberBandCalculator();
			
			thisRunningScrollerMotorProcess = new IScrollerElementMotorProcess[2];
		}
		Vector2 MakeSureRelativeCursorPosIsClampedZeroToOne(Vector2 source){
			Vector2 result = source;
			for(int i = 0; i < 2; i ++)
				if(result[i] < 0f)
					result[i] = 0f;
				else if(result[i] > 1f)
					result[i] = 1f;
			return result;
		}
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
		protected readonly ScrollerAxis thisScrollerAxis;
		/* Rubber */
		readonly protected Vector2 thisRubberBandLimitMultiplier;
		protected abstract bool[] thisShouldApplyRubberBand{get;}// simply return true if wanna apply
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
			IUIAdaptor scrollerElementAdaptor = thisScrollerElement.GetUIAdaptor();
			thisScrollerElementRect = scrollerElementAdaptor.GetRect();
			thisScrollerElementLength = new Vector2(thisScrollerElementRect.width, thisScrollerElementRect.height);
		}
		protected virtual void InitializeScrollerElementForActivation(){
			Vector2 initialCursorValue = GetInitialNormalizedCursoredPosition();
			PlaceScrollerElement(initialCursorValue);
		}
		protected abstract Vector2 GetInitialNormalizedCursoredPosition();
		public virtual void SetScrollerElementLocalPosOnAxis(float localPosOnAxis, int dimension){
			Vector2 newScrollerElementLocalPos = thisScrollerElement.GetLocalPosition();
			newScrollerElementLocalPos[dimension] = localPosOnAxis;
			thisScrollerElement.SetLocalPosition(newScrollerElementLocalPos);
		}
		
		
		/* Drag */
		protected bool thisHasDoneDragEvaluation;
		protected bool thisShouldProcessDrag;//returns true if this is the one to handle the drag, false if passed upward
	
		protected void ResetDrag(){
			thisHasDoneDragEvaluation = false;
			thisShouldProcessDrag = false;
		}
		protected override void OnReleaseImple(){
			if(thisShouldProcessDrag){
				CheckForStaticBoundarySnap();
			}else
				base.OnReleaseImple();
			ResetDrag();
		}
		protected override void OnDragImple(ICustomEventData eventData){
			if(!thisHasDoneDragEvaluation)
				EvaluateDrag(eventData);

			if(thisShouldProcessDrag){
				Vector2 dragDeltaPos = CalcDragDeltaPos(eventData.deltaPos);
				DisplaceScrollerElement(dragDeltaPos);
			}else{
				base.OnDragImple(eventData);
			}
		}
		void EvaluateDrag(ICustomEventData eventData){
			thisHasDoneDragEvaluation = true;
			thisShouldProcessDrag = DetermineIfThisShouldProcessDrag(eventData.deltaPos);
		}
		bool DetermineIfThisShouldProcessDrag(Vector2 deltaPos){
			if(thisScrollerAxis == ScrollerAxis.Both)
				return true;
			else{
				if(DeltaPosIsHorizontal(deltaPos))
					return thisScrollerAxis == ScrollerAxis.Horizontal;
				else
					return thisScrollerAxis == ScrollerAxis.Vertical;
			}
		}
		bool DeltaPosIsHorizontal(Vector2 deltaPos){
			return deltaPos.x >= deltaPos.y;
		}
		protected Vector2 CalcDragDeltaPos(Vector2 deltaP){
			if(thisScrollerAxis == ScrollerAxis.Both)
				return deltaP;
			else if(thisScrollerAxis == ScrollerAxis.Horizontal)
				return new Vector2(deltaP.x, 0f);
			else
				return new Vector2(0f, deltaP.y);
		}
		protected virtual void DisplaceScrollerElement(Vector2 deltaP){
			Vector2 newElementLocalPosition = thisScrollerElement.GetLocalPosition() + deltaP;
			if(thisRequiresToCheckForHorizontalAxis){
				if(thisShouldApplyRubberBand[0])
					newElementLocalPosition[0] = CheckAndApplyRubberBand(deltaP[0], newElementLocalPosition[0], 0);
			}
			if(thisRequiresToCheckForVerticalAxis){
				if(thisShouldApplyRubberBand[1])
					newElementLocalPosition[1] = CheckAndApplyRubberBand(deltaP[1], newElementLocalPosition[1], 1);
			}
			thisScrollerElement.SetLocalPosition(newElementLocalPosition);
		}
		float CheckAndApplyRubberBand(float deltaPOnAxis, float scrollerElementLocalPosOnAxis, int dimension){
			float result = scrollerElementLocalPosOnAxis;
			if(ElementIsScrolledToIncreaseCursorOffset(deltaPOnAxis, scrollerElementLocalPosOnAxis, dimension)){
				result = CalcRubberBandedPosOnAxis(scrollerElementLocalPosOnAxis, dimension);
			}
			return result;
		}
		protected bool ElementIsScrolledToIncreaseCursorOffset(float deltaPosOnAxis, float scrollerElementLocalPosOnAxis, int dimension){
			float cursorOffsetInPixel = GetElementCursorOffsetInPixel(scrollerElementLocalPosOnAxis, dimension);
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



		/* Rect calculation */
		protected bool ElementIsUndersizedTo(Vector2 referenceLength, int dimension){
			return thisScrollerElementLength[dimension] <= referenceLength[dimension];
		}
		protected float GetNormalizedPosition(float scrollerElementLocalPosOnAxis, Vector2 referenceLength, Vector2 referenceMin, int dimension){
			/*  (0f, 0f) if cursor rests on top left corner of the element
				(1f, 1f) if cursor rests on bottom right corner of the element
				value below 0f and over 1f indicates the element's displacement beyond cursor bounds
			*/
			if(ElementIsUndersizedTo(referenceLength, dimension)){
				return 0f;
			}else{
				float referenceLengthOnAxis = referenceLength[dimension];
				float referenceMinOnAxis = referenceMin[dimension];
				return (referenceMinOnAxis - scrollerElementLocalPosOnAxis)/ (thisScrollerElementLength[dimension] - referenceLengthOnAxis);

			}
		}
		protected float GetNormalizedCursoredPosition(float scrollerElementLocalPosOnAxis, int dimension){
			return GetNormalizedPosition(scrollerElementLocalPosOnAxis, thisCursorLength, thisCursorLocalPosition, dimension);
		}
		protected float GetNormalizedScrollerPosition(float scrollerElementLocalPosOnAxis, int dimension){
			return GetNormalizedPosition(scrollerElementLocalPosOnAxis, thisRectLength, Vector2.zero, dimension);
		}
		protected float GetElementCursorOffsetInPixel(float scrollerElementLocalPosOnAxis, int dimension){
			/* used to calculate rubberbanding */
			if(ElementIsUndersizedTo(thisCursorLength, dimension)){
				return thisCursorLocalPosition[dimension] - scrollerElementLocalPosOnAxis;
			}
			else{
				float elePosNormToCursor = GetNormalizedCursoredPosition(scrollerElementLocalPosOnAxis, dimension);
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
				float scrollerElementLocalPosOnAxis = thisCursorLocalPosition[dimension] - (normalizedCursoredPositionOnAxis * (thisScrollerElementLength[dimension] - thisCursorLength[dimension]));
				return scrollerElementLocalPosOnAxis;
			}
		}



		/* Swipe */
		protected override void OnSwipeImple(ICustomEventData eventData){
			if(thisShouldProcessDrag){
				Vector2 swipeDeltaPos = CalcDragDeltaPos(eventData.deltaPos);
				if(thisIsEnabledInertia)
					StartInertialScroll(swipeDeltaPos);
			}else
				base.OnSwipeImple(eventData);
			
			ResetDrag();
		}
		readonly protected bool thisIsEnabledInertia;

		protected void SnapTo(float targetNormalizedCursoredPosOnAxis, float initVelOnAxis, int dimension){
			float targetElementLocalPosOnAxis = CalcLocalPositionFromNormalizedCursoredPositionOnAxis(targetNormalizedCursoredPosOnAxis, dimension);
			IScrollerElementSnapProcess newProcess = thisProcessFactory.CreateScrollerElementSnapProcess(this, thisScrollerElement, targetElementLocalPosOnAxis, initVelOnAxis, dimension);
			newProcess.Run();
		}
		/* motor process */
		protected IScrollerElementMotorProcess[] thisRunningScrollerMotorProcess;
		public void SwitchRunningElementMotorProcess(IScrollerElementMotorProcess process, int dimension){
			StopRunningElementMotorProcess(dimension);
			thisRunningScrollerMotorProcess[dimension] = process;
			if(process != null)
				thisScrollerElement.DisableInputRecursively();
		}
		public void ClearScrollerElementMotorProcess(IScrollerElementMotorProcess processToClear, int dimension){
			if(thisRunningScrollerMotorProcess[dimension] == processToClear){
				thisRunningScrollerMotorProcess[dimension] = null;
				CheckAndEnableInputRecursivelyDownScrollerElement();
			}
		}
		void CheckAndEnableInputRecursivelyDownScrollerElement(){
			bool runningProcessFound = false;
			foreach(IScrollerElementMotorProcess process in thisRunningScrollerMotorProcess)
				if(process != null)
					runningProcessFound = true;
			if(!runningProcessFound)
				thisScrollerElement.EnableInputRecursively();
		}
		void StopRunningElementMotorProcess(int dimension){
			if(thisRunningScrollerMotorProcess[dimension] != null)
				thisRunningScrollerMotorProcess[dimension].Stop();
		}
		/*  */
		protected virtual void StartInertialScroll(Vector2 deltaPos){
			if(thisScrollerAxis == ScrollerAxis.Horizontal){
				IInertialScrollProcess process = thisProcessFactory.CreateInertialScrollProcess(deltaPos[0], 1f, this, thisScrollerElement, 0);
				process.Run();
			}else if(thisScrollerAxis == ScrollerAxis.Vertical){
				IInertialScrollProcess process = thisProcessFactory.CreateInertialScrollProcess(deltaPos[1], 1f, this, thisScrollerElement, 1);
				process.Run();
			}else{
				float sine;
				float cosine;
				DKUtility.Calculator.CalcSineAndCosine(deltaPos, out sine, out cosine);
				if(sine < 0f)
					sine *= -1f;
				if(cosine < 0f)
					cosine *= -1f;

				IInertialScrollProcess horizontalProcess = thisProcessFactory.CreateInertialScrollProcess(deltaPos[0], cosine, this, thisScrollerElement, 0);
				horizontalProcess.Run();
				IInertialScrollProcess verticalProcess = thisProcessFactory.CreateInertialScrollProcess(deltaPos[1], sine, this, thisScrollerElement, 1);
				verticalProcess.Run();
			}
		}
		public virtual bool CheckForDynamicBoundarySnap(float deltaPosOnAxis, int dimension){
			float scrollerElementLocalPosOnAxis = thisScrollerElement.GetLocalPosition()[dimension];
			if(deltaPosOnAxis != 0f)
				if(ElementIsScrolledToIncreaseCursorOffset(deltaPosOnAxis, scrollerElementLocalPosOnAxis, dimension)){
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
		protected virtual bool CheckForStaticBoundarySnapOnAxis(int dimension){
			float scrollerElementLocalPosOnAxis = thisScrollerElement.GetLocalPosition()[dimension];
			float cursorOffset = GetElementCursorOffsetInPixel(scrollerElementLocalPosOnAxis, dimension);
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
		protected virtual bool[] CheckForStaticBoundarySnap(){
			bool[] result = new bool[]{false, false};
			for(int i = 0; i < 2; i ++){
				result[i] = CheckForStaticBoundarySnapOnAxis(i);
			}
			return result;
		}
		/* Touch */
		protected override void OnTouchImple(int touchCount){
			/*  Stop scroll if running
			*/
			for(int i = 0; i < 2; i ++)
				StopRunningElementMotorProcess(i);
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
			thisRelativeCursorPos = relativeCursorPosition;
			thisRubberBandLimitMultiplier = rubberBandLimitMultiplier;
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
		readonly bool thisIsEnabledInertia;
		public bool isEnabledInertia{get{return thisIsEnabledInertia;}}
	}
}
