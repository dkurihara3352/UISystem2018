using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility.CurveUtility;
using DKUtility;
namespace UISystem{
	public interface IScroller: IUIElement{
		void SwitchRunningElementMotorProcess(IScrollerElementMotorProcess process, int dimension);
		void SetScrollerElementLocalPosOnAxis(float localPosOnAxis, int dimension);
		float GetElementCursorOffsetInPixel(float scrollerElementLocalPosOnAxis, int dimension);
		float GetNormalizedCursoredPositionOnAxis(float scrollerElementLocalPosOnAxis, int dimension);

		bool IsMovingWithSpeedOverNewScrollThreshold();
		void UpdateVelocity(float velocityOnAxis, int dimension);

		IUIElement GetScrollerElement();
		void SetUpScrollerElement();
		void SetUpCursorTransform();
		bool ScrollerElementIsUndersizedTo(Vector2 referenceLength, int dimension);

		void ResetDrag();
		void ClearTouchPositionCache();
		Vector2 GetVelocity();
		void PauseRunningMotorProcessRecursivelyUp();
		void CheckAndPerformDynamicBoundarySnapOnAxis(float deltaPosOnAxis, float velocity, int dimension);
		void CheckAndPerformStaticBoundarySnap();
	}
	public enum ScrollerAxis{
		Horizontal, Vertical, Both
	}
	public abstract class AbsScroller : UIElement, IScroller{
		public AbsScroller(IScrollerConstArg arg): base(arg){
			/* good here */
			thisScrollerAxis = arg.scrollerAxis;
			thisRelativeCursorPosition = MakeSureRelativeCursorPosIsClampedZeroToOne(arg.relativeCursorPosition);
			thisRubberBandLimitMultiplier = MakeRubberBandLimitMultiplierInRange(arg.rubberBandLimitMultiplier);
			thisIsEnabledInertia = arg.isEnabledInertia;
			thisNewScrollSpeedThreshold = arg.newScrollSpeedThreshold;

			/* good here */
			UpdateRect();
			
			/* non dependent */
			thisRunningScrollerMotorProcess = new IScrollerElementMotorProcess[2];
			thisElementIsScrolledToIncreaseCursorOffsetCalculator = new ElementIsScrolledToIncreaseCursorOffsetCalculator(this);
		}
		/* SetUp */
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
			protected readonly ScrollerAxis thisScrollerAxis;

		/* ScrollerRect */
			public override void UpdateRect(){
				base.UpdateRect();
				SetUpScrollerRect();
				MakeSureRectIsSet(thisRect);
				SetUpRubberBandCalculators();
			}
			protected Rect thisRect;
			protected Vector2 thisRectLength;
			void SetUpScrollerRect(){
				thisRect = thisUIA.GetRect();
				thisRectLength = new Vector2(thisRect.width, thisRect.height);
			}
			protected void MakeSureRectIsSet(Rect rect){
				if(rect.width == 0f || rect.height == 0f)
					throw new System.InvalidOperationException("rect has at least one dimension not set right");
			}
		/* Rubber */
			readonly protected Vector2 thisRubberBandLimitMultiplier;
			Vector2 thisRubberLimit;

			void SetUpRubberBandCalculators(){
				for(int i = 0; i < 2; i ++)
					if(thisShouldApplyRubberBand[i])
						thisRubberBandCalculator = CreateRubberBandCalculator();
			}
			protected abstract bool[] thisShouldApplyRubberBand{get;}// simply return true if wanna apply
			RubberBandCalculator[] thisRubberBandCalculator;		
			RubberBandCalculator[] CreateRubberBandCalculator(){
				RubberBandCalculator[] result = new RubberBandCalculator[2];
				thisRubberLimit = new Vector2();
				for(int i = 0; i < 2; i++){
					float rubberLimit =  thisRubberBandLimitMultiplier[i] * thisRectLength[i];
					thisRubberLimit[i] = rubberLimit;
					result[i] = new RubberBandCalculator(1f, rubberLimit);
				}
				return result;
			}

		/* ScrollerElement */
			public void SetUpScrollerElement(){
				SetTheOnlyChildAsScrollerElement();
				SetUpScrollerElementRect();
				SetUpCursorTransform();
				OnRectsSetUpComplete();
				PlaceScrollerElementAtInitialCursorValue();
			}
			protected virtual void OnRectsSetUpComplete(){
				thisElementCursorOffsetInPixelCalculator = new ElementCursorOffsetInPixelCalculator(
					this,
					thisCursorLength,
					thisCursorLocalPosition,
					thisScrollerElementLength
				);
			}
			protected IUIElement thisScrollerElement;
			public IUIElement GetScrollerElement(){
				return thisScrollerElement;
			}
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
			protected virtual void SetUpScrollerElementRect(){
				IUIAdaptor scrollerElementAdaptor = thisScrollerElement.GetUIAdaptor();
				thisScrollerElementRect = scrollerElementAdaptor.GetRect();
				thisScrollerElementLength = new Vector2(
					thisScrollerElementRect.width, 
					thisScrollerElementRect.height
				);
			}
			/* Cursor Transform */
			public void SetUpCursorTransform(){
				thisCursorLength = CalcCursorLength();
				ClampCursorLengthToThisRect();
				thisCursorLocalPosition = CalcCursorLocalPos();

				Rect cursorRect = new Rect(thisCursorLocalPosition, thisCursorLength);
				((IScrollerAdaptor)thisUIA).ShowCursorRectInGUI(cursorRect);
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
				Vector2 scrollerRectLengthV2 = new Vector2();
				Vector2 cursorLengthV2 = new  Vector2();
				Vector2 diffLV2 = new Vector2();
				for(int i = 0; i < 2; i ++){
					float scrollerRectLength = thisRectLength[i];
						scrollerRectLengthV2[i] = scrollerRectLength;
					float cursorLength = thisCursorLength[i];
						cursorLengthV2[i] = cursorLength;
					float diffL = scrollerRectLength - cursorLength;
						diffLV2[i] = diffL;

					float localPos;
					if(thisRelativeCursorPosition[i] == 0f) 
						localPos = 0f;
					else
						localPos = thisRelativeCursorPosition[i] * diffL;
					result[i] = localPos;
				}
				return result;
			}
			/*  */
			protected void PlaceScrollerElementAtInitialCursorValue(){
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
			protected bool thisShouldProcessDrag;
		
			public void ResetDrag(){
				thisShouldProcessDrag = false;
				ClearTouchPositionCache();
			}
			protected Vector2 thisTouchPosition;
			protected Vector2 thisElementLocalPositionAtTouch;
			Vector2 noTouchPosition = new Vector2(10000f, 10000f);
			public void ClearTouchPositionCache(){
				thisTouchPosition = noTouchPosition;
				thisElementLocalPositionAtTouch = noTouchPosition;
			}
			void CacheTouchPosition(Vector2 touchPosition){
				thisTouchPosition = touchPosition;
				thisElementLocalPositionAtTouch = thisScrollerElement.GetLocalPosition();
			}
			protected override void OnBeginDragImple(ICustomEventData eventData){
				if(thisTopmostScrollerInMotion != null){
					EvaluateDrag(eventData);
					thisUIM.SetInputHandlingScroller(this, UIManager.InputName.BeginDrag);
					if(thisIsTopmostScrollerInMotion){
						CacheTouchPosition(eventData.position);
					}else{
						thisTopmostScrollerInMotion.OnBeginDrag(eventData);
					}
				}else{
					EvaluateDrag(eventData);
					if(thisShouldProcessDrag){
						thisUIM.SetInputHandlingScroller(this, UIManager.InputName.BeginDrag);
						CacheTouchPosition(eventData.position);
					}
					else
						base.OnBeginDragImple(eventData);
				}
			}
			protected override void OnDragImple(ICustomEventData eventData){
				if(thisShouldProcessDrag){
					thisUIM.SetInputHandlingScroller(this, UIManager.InputName.Drag);
					if(thisTopmostScrollerInMotion != null){
						if(thisIsTopmostScrollerInMotion)
							DisplaceScrollerElement(eventData.position);
						else
							thisTopmostScrollerInMotion.OnDrag(eventData);
					}else{
						DisplaceScrollerElement(eventData.position);
					}
				}else{
					base.OnDragImple(eventData);
				}
			}
			void EvaluateDrag(ICustomEventData eventData){
				thisShouldProcessDrag = DetermineIfThisShouldProcessDrag(eventData.deltaPos);
			}
			bool DetermineIfThisShouldProcessDrag(Vector2 deltaPos){
				if(thisTopmostScrollerInMotion != null){
					return true;
				}else{
					if(thisScrollerAxis == ScrollerAxis.Both)
						return true;
					else{
						if(DeltaPosIsHorizontal(deltaPos))
							return thisScrollerAxis == ScrollerAxis.Horizontal;
						else
							return thisScrollerAxis == ScrollerAxis.Vertical;
					}
				}
			}
			bool DeltaPosIsHorizontal(Vector2 deltaPos){
				return Mathf.Abs(deltaPos.x) >= Mathf.Abs(deltaPos.y);
			}
			protected Vector2 CalcDragDeltaPos(Vector2 deltaP){
				if(thisScrollerAxis == ScrollerAxis.Both)
					return deltaP;
				else if(thisScrollerAxis == ScrollerAxis.Horizontal)
					return new Vector2(deltaP.x, 0f);
				else
					return new Vector2(0f, deltaP.y);
			}
			protected virtual void DisplaceScrollerElement(Vector2 dragPosition){
				Vector2 displacement = CalcDragDeltaSinceTouch(dragPosition);
				Vector2 newElementLocalPosition =  GetScrollerElementRubberBandedLocalPosition(displacement);
				thisScrollerElement.SetLocalPosition(newElementLocalPosition);
			}
			protected Vector2 CalcDragDeltaSinceTouch(Vector2 dragPosition){
				Vector2 rawDisplacement = dragPosition - thisTouchPosition;
				if(thisScrollerAxis == ScrollerAxis.Both)
					return rawDisplacement;
				else if(thisScrollerAxis == ScrollerAxis.Horizontal)
					return new Vector2(rawDisplacement.x, 0f);
				else
					return new Vector2(0f, rawDisplacement.y);
				
			}
			protected Vector2 GetScrollerElementRubberBandedLocalPosition(Vector2 displacement){
				Vector2 result = new Vector2();
				for(int i = 0; i < 2; i ++){
					float displacementAfterRubberBand;
					displacementAfterRubberBand = displacement[i];
					float prospectiveElementLocalPosOnAxis = thisElementLocalPositionAtTouch[i] + displacement[i];
					float cursorOffsetInPixel = GetElementCursorOffsetInPixel(prospectiveElementLocalPosOnAxis, i);
					if(cursorOffsetInPixel != 0f){
						float nonRubberedDisplacement = displacement[i];
						float rubberedDisplacement = 0f;
						if(cursorOffsetInPixel < 0f && displacement[i] > 0f){
							cursorOffsetInPixel *= -1f;
							nonRubberedDisplacement = displacement[i] - cursorOffsetInPixel;
							rubberedDisplacement = thisRubberBandCalculator[i].CalcRubberBandValue(cursorOffsetInPixel, invert: false);
						}else if(cursorOffsetInPixel > 0f && displacement[i] < 0f){
							cursorOffsetInPixel *= -1f;
							nonRubberedDisplacement = displacement[i] - cursorOffsetInPixel;
							rubberedDisplacement = thisRubberBandCalculator[i].CalcRubberBandValue(cursorOffsetInPixel, invert: true);
						}
						displacementAfterRubberBand = nonRubberedDisplacement + rubberedDisplacement;
					}		
					result[i] = thisElementLocalPositionAtTouch[i] + displacementAfterRubberBand;
				}
				return result;
			}
			IElementIsScrolledToIncreaseCursorOffsetCalculator thisElementIsScrolledToIncreaseCursorOffsetCalculator;
			protected bool ElementIsScrolledToIncreaseCursorOffset(float deltaPosOnAxis, float scrollerElementLocalPosOnAxis, int dimension){
				return thisElementIsScrolledToIncreaseCursorOffsetCalculator.Calculate(deltaPosOnAxis, scrollerElementLocalPosOnAxis, dimension);
			}
			protected bool thisRequiresToCheckForHorizontalAxis{
				get{return thisScrollerAxis == ScrollerAxis.Horizontal || thisScrollerAxis == ScrollerAxis.Both;}
			}
			protected bool thisRequiresToCheckForVerticalAxis{
				get{return thisScrollerAxis == ScrollerAxis.Vertical || thisScrollerAxis == ScrollerAxis.Both;}
			}
		/* Rect calculation */
			public bool ScrollerElementIsUndersizedTo(Vector2 referenceLength, int dimension){
				return thisScrollerElementLength[dimension] <= referenceLength[dimension];
			}
			protected float GetNormalizedPosition(float scrollerElementLocalPosOnAxis, Vector2 referenceLength, Vector2 referenceMin, int dimension){
				/*  (0f, 0f) if cursor rests on top left corner of the element
					(1f, 1f) if cursor rests on bottom right corner of the element
					value below 0f and over 1f indicates the element's displacement beyond cursor bounds
				*/
				if(ScrollerElementIsUndersizedTo(referenceLength, dimension)){
					return 0f;
				}else{
					float referenceLengthOnAxis = referenceLength[dimension];
					float referenceMinOnAxis = referenceMin[dimension];
					return (referenceMinOnAxis - scrollerElementLocalPosOnAxis)/ (thisScrollerElementLength[dimension] - referenceLengthOnAxis);

				}
			}
			public float GetNormalizedCursoredPositionOnAxis(float scrollerElementLocalPosOnAxis, int dimension){
				return GetNormalizedPosition(scrollerElementLocalPosOnAxis, thisCursorLength, thisCursorLocalPosition, dimension);
			}
			protected float GetNormalizedScrollerPosition(float scrollerElementLocalPosOnAxis, int dimension){
				return GetNormalizedPosition(scrollerElementLocalPosOnAxis, thisRectLength, Vector2.zero, dimension);
			}

			/* ElementCursorOffsetInPixel calculation */
			IElementCursorOffsetInPixelCalculator thisElementCursorOffsetInPixelCalculator;
			public float GetElementCursorOffsetInPixel(float scrollerElementLocalPosOnAxis, int dimension){
				// /* used to calculate rubberbanding */
				return thisElementCursorOffsetInPixelCalculator.Calculate(scrollerElementLocalPosOnAxis, dimension);
			}
			protected float GetNormalizedCursoredPositionFromPosInElementSpace(float positionInElementSpaceOnAxis, int dimension){
				float prospectiveElementLocalPosOnAxis = thisCursorLocalPosition[dimension] - positionInElementSpaceOnAxis;
				return GetNormalizedCursoredPositionOnAxis(prospectiveElementLocalPosOnAxis, dimension);
			}
			protected void PlaceScrollerElement(Vector2 targetCursorValue){
				Vector2 newLocalPos = CalcLocalPositionFromNormalizedCursoredPosition(targetCursorValue);
				thisScrollerElement.SetLocalPosition(newLocalPos);
			}
			protected Vector2 CalcLocalPositionFromNormalizedCursoredPosition(Vector2 normalizedCursoredPosition){
				Vector2 result = new Vector2();
				for(int i = 0; i < 2; i ++){
					result[i] = CalcLocalPositionFromNormalizedCursoredPositionOnAxis(normalizedCursoredPosition[i], i);
				}
				return result;
			}
			protected float CalcLocalPositionFromNormalizedCursoredPositionOnAxis(float normalizedCursoredPositionOnAxis, int dimension){
				if(ScrollerElementIsUndersizedTo(thisCursorLength, dimension))
					return thisCursorLocalPosition[dimension];
				else{
					float scrollerElementLocalPosOnAxis = thisCursorLocalPosition[dimension] - (normalizedCursoredPositionOnAxis * (thisScrollerElementLength[dimension] - thisCursorLength[dimension]));
					return scrollerElementLocalPosOnAxis;
				}
			}

		/* Release */
			protected override void OnReleaseImple(){
				thisUIM.SetInputHandlingScroller(this, UIManager.InputName.Release);
			}
		/* Tap */
			protected override void OnTapImple(int tapCount){
				thisUIM.SetInputHandlingScroller(this, UIManager.InputName.Tap);
			}
		/* Swipe */
			protected override void OnSwipeImple(ICustomEventData eventData){
				if(thisShouldProcessDrag){
					thisUIM.SetInputHandlingScroller(this, UIManager.InputName.Swipe);
					if(thisTopmostScrollerInMotion != null){
						if(thisIsTopmostScrollerInMotion){
							ProcessSwipe(eventData);
						}else{
							thisTopmostScrollerInMotion.OnSwipe(eventData);
							CheckAndPerformStaticBoundarySnap();
							ResetDrag();
						}
					}else{
						ProcessSwipe(eventData);
					}
				}else{
					base.OnSwipeImple(eventData);
					CheckAndPerformStaticBoundarySnap();
					ResetDrag();
				}
			}
			protected virtual void ProcessSwipe(ICustomEventData eventData){
				if(thisIsEnabledInertia){
					StartInertialScroll(eventData.velocity);
					CheckAndPerformStaticBoundarySnapFrom(thisProximateParentScroller);
				}
				else
					CheckAndPerformStaticBoundarySnapFrom(this);
			}
			protected bool InitialVelocityIsOverThreshold(Vector2 velocity){
				return velocity.sqrMagnitude >= thisNewScrollSpeedThreshold * thisNewScrollSpeedThreshold;
			}
			readonly protected bool thisIsEnabledInertia;
			protected virtual void StartInertialScroll(Vector2 swipeVelocity){
				ResetDrag();

				if(InitialVelocityIsOverThreshold(swipeVelocity))
					DisableScrollInputRecursively(this);

				if(thisScrollerAxis == ScrollerAxis.Horizontal){
					IInertialScrollProcess process = thisProcessFactory.CreateInertialScrollProcess(swipeVelocity[0], 1f, this, thisScrollerElement, 0);
					process.Run();
				}else if(thisScrollerAxis == ScrollerAxis.Vertical){
					IInertialScrollProcess process = thisProcessFactory.CreateInertialScrollProcess(swipeVelocity[1], 1f, this, thisScrollerElement, 1);
					process.Run();
				}else{
					float sine;
					float cosine;
					DKUtility.Calculator.CalcSineAndCosine(swipeVelocity, out sine, out cosine);
					if(sine < 0f)
						sine *= -1f;
					if(cosine < 0f)
						cosine *= -1f;

					IInertialScrollProcess horizontalProcess = thisProcessFactory.CreateInertialScrollProcess(swipeVelocity[0], cosine, this, thisScrollerElement, 0);
					horizontalProcess.Run();
					IInertialScrollProcess verticalProcess = thisProcessFactory.CreateInertialScrollProcess(swipeVelocity[1], sine, this, thisScrollerElement, 1);
					verticalProcess.Run();
				}
			}
			public virtual void CheckAndPerformDynamicBoundarySnapOnAxis(float deltaPosOnAxis, float velocity, int dimension){
				float scrollerElementLocalPosOnAxis = thisScrollerElement.GetLocalPosition()[dimension];
				if(deltaPosOnAxis != 0f)
					if(ElementIsScrolledToIncreaseCursorOffset(deltaPosOnAxis, scrollerElementLocalPosOnAxis, dimension)){
						OnDynamicBoundaryCheckSuccess(deltaPosOnAxis, velocity, dimension);
					}
				OnDynamicBoundaryCheckFail(deltaPosOnAxis, velocity, dimension);
			}
			protected virtual void OnDynamicBoundaryCheckSuccess(float deltaPosOnAxis, float velocityOnAxis, int dimension){
				float snapTargetNormPos;
				if(deltaPosOnAxis > 0f)
					snapTargetNormPos = 0f;
				else	
					snapTargetNormPos = 1f;
				SnapTo(snapTargetNormPos, velocityOnAxis, dimension);
				return;
			}
			protected virtual void OnDynamicBoundaryCheckFail(float delatPosOnAxis, float velocityOnAxis, int dimension){
				return;
			}
			protected virtual void CheckAndPerformStaticBoundarySnapOnAxis(int dimension){
				float scrollerElementLocalPosOnAxis = thisScrollerElement.GetLocalPosition()[dimension];
				float cursorOffset = GetElementCursorOffsetInPixel(scrollerElementLocalPosOnAxis, dimension);
				if(cursorOffset != 0f){
					float snapTargetNormPos;
					if(cursorOffset < 0f)
						snapTargetNormPos = 0f;
					else
						snapTargetNormPos = 1f;
					SnapTo(snapTargetNormPos, 0f, dimension);
					return;
				}else{
					OnStaticBoundaryCheckFail(dimension);
				}
			}
			protected virtual void OnStaticBoundaryCheckFail(int dimension){
				for(int i = 0; i < 2; i ++)
					UpdateVelocity(0f, i);
			}
			public void CheckAndPerformStaticBoundarySnap(){
				for(int i = 0; i < 2; i ++)
					CheckAndPerformStaticBoundarySnapOnAxis(i);
			}
			protected void SnapTo(float targetNormalizedCursoredPosOnAxis, float initVelOnAxis, int dimension){

				UpdateVelocity(initVelOnAxis, dimension);
				Vector2 curVelocity = GetVelocity();
				if(InitialVelocityIsOverThreshold(curVelocity))
					DisableScrollInputRecursively(this);
					
				float targetElementLocalPosOnAxis = CalcLocalPositionFromNormalizedCursoredPositionOnAxis(targetNormalizedCursoredPosOnAxis, dimension);
				IScrollerElementSnapProcess newProcess = thisProcessFactory.CreateScrollerElementSnapProcess(this, thisScrollerElement, targetElementLocalPosOnAxis, initVelOnAxis, dimension);
				newProcess.Run();
			}
		/* Scroller Hieracrchy */
			public override void DisableScrollInputRecursively(IScroller disablingScroller){
				if(this == disablingScroller){// initiating
					if(thisUIM.ShowsInputability())
						TurnTo(Color.blue);
				}
				thisTopmostScrollerInMotion = disablingScroller;
				thisScrollerElement.DisableScrollInputRecursively(disablingScroller);
			}
			public override void EnableScrollInputSelf(){
				if(thisIsTopmostScrollerInMotion){
					if(thisUIM.ShowsInputability())
						TurnTo(GetUIImage().GetDefaultColor());
				}
				thisTopmostScrollerInMotion = null;
			}
			protected bool thisIsTopmostScrollerInMotion{
				get{
					if(thisTopmostScrollerInMotion != null)
						return this == thisTopmostScrollerInMotion;
					else
						return false;
				}
			}
		/* motor process & OnTouch */
			protected IScrollerElementMotorProcess[] thisRunningScrollerMotorProcess;
			public void SwitchRunningElementMotorProcess(IScrollerElementMotorProcess process, int dimension){
				PauseRunningElementMotorProcess(dimension);
				thisRunningScrollerMotorProcess[dimension] = process;
			}
			Vector2 thisVelocity;
			public Vector2 GetVelocity(){return thisVelocity;}
			public void UpdateVelocity(float velocityOnAxis, int dimension){
				thisVelocity[dimension] = velocityOnAxis;
				CheckAndTriggerScrollInputEnable();
			}
			void CheckAndTriggerScrollInputEnable(){
				if(thisTopmostScrollerInMotion != null){
					if(thisIsTopmostScrollerInMotion){
						CheckForScrollInputEnable();
					}else
						return;
				}else{//null
					if(this.IsMovingWithSpeedOverNewScrollThreshold())
						CheckForScrollInputEnable();
				}
			}
			public override void CheckForScrollInputEnable(){
				if(thisIsTopmostScrollerInMotion){
					if(!this.IsMovingWithSpeedOverNewScrollThreshold()){
						EnableScrollInputSelf();
						thisScrollerElement.CheckForScrollInputEnable();
					}
				}else{
					if(this.IsMovingWithSpeedOverNewScrollThreshold()){
						DisableScrollInputRecursively(this);
					}else{
						EnableScrollInputRecursively();
					}
				}
			}
			readonly float thisNewScrollSpeedThreshold;
			public bool IsMovingWithSpeedOverNewScrollThreshold(){
				return thisVelocity.sqrMagnitude >= thisNewScrollSpeedThreshold * thisNewScrollSpeedThreshold;
			}

			protected override void OnTouchImple(int touchCount){
				thisUIM.SetInputHandlingScroller(this, UIManager.InputName.Touch);
			}
			public void PauseRunningMotorProcessRecursivelyUp(){
				PauseAllRunningElementMotorProcess();
				if(thisProximateParentScroller != null)
					thisProximateParentScroller.PauseRunningMotorProcessRecursivelyUp();
			}
			void PauseRunningElementMotorProcess(int dimension){
				if(thisRunningScrollerMotorProcess[dimension] != null){
					thisRunningScrollerMotorProcess[dimension].Stop();
				}
			}
			void PauseAllRunningElementMotorProcess(){
				for(int i = 0; i < 2; i ++)
					PauseRunningElementMotorProcess(i);
			}
	}



	public interface IScrollerConstArg: IUIElementConstArg{
		ScrollerAxis scrollerAxis{get;}
		Vector2 relativeCursorPosition{get;}
		Vector2 rubberBandLimitMultiplier{get;}
		bool isEnabledInertia{get;}
		float newScrollSpeedThreshold{get;}
	}
	public abstract class ScrollerConstArg: UIElementConstArg, IScrollerConstArg{
		public ScrollerConstArg(
			ScrollerAxis scrollerAxis, 
			Vector2 relativeCursorPosition, 
			Vector2 rubberBandLimitMultiplier, 
			bool isEnabledInertia, 
			float newScrollSpeedThreshold,

			IUIManager uim, 
			IUISystemProcessFactory processFactory, 
			IUIElementFactory uieFactory, 
			IScrollerAdaptor uia, 
			IUIImage uiImage,
			ActivationMode activationMode
		): base(
			uim, 
			processFactory, 
			uieFactory, 
			uia, 
			uiImage,
			activationMode
		){
			thisScrollerAxis = scrollerAxis;
			thisRelativeCursorPos = relativeCursorPosition;
			thisRubberBandLimitMultiplier = rubberBandLimitMultiplier;
			thisIsEnabledInertia = isEnabledInertia;
			thisNewScrollSpeedThreshold = newScrollSpeedThreshold;
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
		readonly float thisNewScrollSpeedThreshold;
		public float newScrollSpeedThreshold{get{return thisNewScrollSpeedThreshold;}}
	}
}
