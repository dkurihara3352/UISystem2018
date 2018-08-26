using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ISceneObjectElementGroupAdaptor: IUIElementGroupAdaptor{}
	public class SceneObjectElementGroupAdaptor: UIAdaptor, ISceneObjectElementGroupAdaptor{
		public int columnCountConstraint;
		public int rowCountConstraint;
		public bool topToBottom;
		public bool leftToRight;
		public bool rowToColumn;
		public RectTransform referenceRectTransformForRectLength;
		public Vector2 groupElementLength{
			get{
				return referenceRectTransformForRectLength.sizeDelta;
			}
		}
		public Vector2 GetGroupElementLength(){
			return groupElementLength;
		}
		public Vector2 padding;
		public Vector2 GetPadding(){return padding;}
		public bool[] usesFixedPadding = new bool[2]{true, true};
		protected override IUIElement CreateUIElement(IUIImage image){
			Vector2 thisPadding = CheckAndAdjustPadding();
			IUIElementGroupConstArg arg = new UIElementGroupConstArg(
				columnCountConstraint,
				rowCountConstraint,
				topToBottom,
				leftToRight,
				rowToColumn,
				groupElementLength,
				thisPadding,
				usesFixedPadding,

				thisDomainActivationData.uim,
				thisDomainActivationData.processFactory,
				thisDomainActivationData.uiElementFactory,
				this,
				image,
				activationMode
			);
			IGenericUIElementGroup uie = new GenericUIElementGroup(arg);
			return uie;
		}
		Vector2 CheckAndAdjustPadding(){
			Vector2 result = padding;
			if(this.resizeRelativeToScreenSize){				
				for(int i = 0; i < 2; i ++){
					float totalPaddingLength = GetScreenLength(i) - groupElementLength[i];
					result[i] = totalPaddingLength / 2f;
				}
			}
			return result;
		}
		float GetScreenLength(int dimension){
			if(dimension == 0)
				return Screen.width;
			else
				return Screen.height;
		}
		public override void GetReadyForActivation(IUIAActivationData passedData){
			base.GetReadyForActivation(passedData);
			List<IUIElement> childSceneObjectUIEs = GetChildUIEs();
			MakeSureAllChildrenHaveReferenceLength(childSceneObjectUIEs);
			IUIElementGroup uieGroup = (IUIElementGroup)this.GetUIElement();
			uieGroup.SetUpElements(childSceneObjectUIEs);
		}
		void MakeSureAllChildrenHaveReferenceLength(List<IUIElement> childSceneObjectUIEs){
			foreach(IUIElement childUIE in childSceneObjectUIEs){
				IUIAdaptor childUIA = childUIE.GetUIAdaptor();
				RectTransform childRT = childUIA.GetTransform() as RectTransform;
				childRT.sizeDelta = referenceRectTransformForRectLength.sizeDelta;
			}
		}
	}
}

