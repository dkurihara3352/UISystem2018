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
		public Vector2 padding{
			get{
				return new Vector2((Screen.width - groupElementLength.x) * .5f, (Screen.height - groupElementLength.y) * .5f);
			}
		}
		public Vector2 GetPadding(){return padding;}
		protected override IUIElement CreateUIElement(IUIImage image){
			IUIElementGroupConstArg arg = new UIElementGroupConstArg(
				columnCountConstraint,
				rowCountConstraint,
				topToBottom,
				leftToRight,
				rowToColumn,
				groupElementLength,
				padding,
				thisDomainActivationData.uim,
				thisDomainActivationData.processFactory,
				thisDomainActivationData.uiElementFactory,
				this,
				image
			);
			IGenericUIElementGroup uie = new GenericUIElementGroup(arg);
			return uie;
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

