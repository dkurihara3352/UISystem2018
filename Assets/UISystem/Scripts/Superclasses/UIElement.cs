using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElement: IUIInputHandler, ISelectabilityStateHandler{
		IUIElement GetParentUIE();
		void SetParentUIE(IUIElement uie, bool worldPositionStays);
		List<IUIElement> GetChildUIEs();
		Vector2 GetPositionInThisSpace(Vector2 worldPos);
		void SetLocalPosition(Vector2 localPos);
		IUIAdaptor GetUIAdaptor();
		IUIImage GetUIImage();
		void Activate();
		void Deactivate();
	}
	public abstract class AbsUIElement: IUIElement{
		public AbsUIElement(IUIElementConstArg arg){
			thisUIM = arg.uim;
			thisUIA = arg.uia;
			thisTool = arg.tool;
			thisImage = arg.image;
			thisSelectabilityEngine = new SelectabilityStateEngine(this, thisUIM.GetProcessFactory());
		}
		protected readonly IUIManager thisUIM;
		public IUIManager GetUIM(){
			return thisUIM;
		}
		protected readonly IUIAdaptor thisUIA;
		public IUIAdaptor GetUIAdaptor(){
			return thisUIA;
		}
		protected readonly IUITool thisTool;
		public IUIElement GetParentUIE(){
			return GetUIAdaptor().GetParentUIE();
		}
		public List<IUIElement> GetChildUIEs(){
			return GetUIAdaptor().GetChildUIEs();
		}
		public IUIImage GetUIImage(){
			return thisImage;
		}
		protected IUIImage thisImage;
		public virtual void Activate(){
			this.ActivateImple();
			foreach(IUIElement childUIE in this.GetChildUIEs()){
				if(childUIE != null)
					childUIE.Activate(); 
			}
		}
		protected virtual void ActivateImple(){
			InitializeSelectabilityState();
		}
		public virtual void Deactivate(){
			this.DeactivateImple();
			foreach(IUIElement childUIE in this.GetChildUIEs()){
				if(childUIE != null)
					childUIE.Deactivate();
			}
		}
		protected virtual void DeactivateImple(){}
		/* SelectabilityState */
			void InitializeSelectabilityState(){
				BecomeSelectable();
			}
			ISelectabilityStateEngine thisSelectabilityEngine;
			public void BecomeSelectable(){
				thisSelectabilityEngine.BecomeSelectable();
			}
			public void BecomeUnselectable(){
				thisSelectabilityEngine.BecomeUnselectable();
			}
			public void BecomeSelected(){
				thisSelectabilityEngine.BecomeSelected();
			}
			public bool IsSelectable(){
				return thisSelectabilityEngine.IsSelectable();
			}
			public bool IsSelected(){
				return thisSelectabilityEngine.IsSelected();
			}
		/* UIInput */
			public virtual void OnTouch( int touchCount){}
			public virtual void OnDelayedTouch(){}
			public virtual void OnRelease(){}
			public virtual void OnDelayedRelease(){}
			public virtual void OnTap( int tapCount){}
			public virtual void OnDrag( ICustomEventData eventData){}
			public virtual void OnHold( float elapsedT){}
			public virtual void OnSwipe( ICustomEventData eventData){}
		/*  */
		public Vector2 GetPositionInThisSpace(Vector2 worldPos){
			return this.thisUIA.GetPositionInThisSpace(worldPos);
		}
		public void SetLocalPosition(Vector2 localPos){
			this.thisUIA.SetLocalPosition(localPos);
		}
		public void SetParentUIE(IUIElement newParentUIE, bool worldPositionStays){
			this.thisUIA.SetParentUIE(newParentUIE, worldPositionStays);
		}
	}
	public interface IUIElementConstArg{
		IUIManager uim{get;}
		IUIAdaptor uia{get;}
		IUIImage image{get;}
		IUITool tool{get;}
	}
	public class UIElementConstArg: IUIElementConstArg{
		readonly IUIManager thisUIM;
		readonly IUIAdaptor thisUIA;
		readonly IUIImage thisImage;
		readonly IUITool thisTool;
		public UIElementConstArg(IUIManager uim, IUIAdaptor uia, IUIImage image, IUITool tool){
			thisUIM = uim;
			thisUIA = uia;
			thisImage = image;
			thisTool = tool;
		}
		public IUIManager uim{get{return thisUIM;}}
		public IUIAdaptor uia{get{return thisUIA;}}
		public IUIImage image{get{return thisImage;}}
		public IUITool tool{get{return thisTool;}}
	}
	public interface IUIInputHandler{
		/* Releasing
			pointer up =>
				OnRelease
				if deltaP over thresh
					OnSwipe
				else
					if stays in-bound && within time frame
						OnTap

		*/
		void OnTouch( int touchCount);
		void OnDelayedTouch();
		void OnRelease();
		void OnDelayedRelease();
		/* called after both OnRelease and OnTap */
		void OnTap( int tapCount);
		void OnDrag( ICustomEventData eventData);
		void OnHold( float deltaT);
		/* called every frame from pointer down to up */
		void OnSwipe( ICustomEventData eventData);
	}
	public interface IVisibilitySwitcher{
		/* subclassed by same uies that are also IRootActivator?
			.Tools, Widgets
			** implementation idea **
			. CanvasGroup is assigned, and tweak its group alpha or like
		 */
		void Show();
		void Hide();
	}
}
