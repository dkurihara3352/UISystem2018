using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IPickUpSystemUIElement: IUIElement{
	}
	public abstract class AbsPickUpSystemUIElement: UIElement, IPickUpSystemUIElement{
		public AbsPickUpSystemUIElement(IPickUpSystemUIEConstArg arg): base(arg){
		}
		protected IPickUpSystemProcessFactory thisPickUpSystemProcessFactory{get{return (IPickUpSystemProcessFactory)thisProcessFactory;}}
		protected IPickUpSystemUIElementFactory thisPickupSystemUIElementFactory{get{return (IPickUpSystemUIElementFactory)thisUIElementFactory;}}
	}
	public interface IPickUpSystemUIEConstArg: IUIElementConstArg{
		IPickUpSystemProcessFactory pickUpSystemProcessFactory{get;}
		IPickUpSystemUIElementFactory pickUpSystemUIElementFactory{get;}
	}
	public class PickUpSystemUIEConstArg: UIElementConstArg, IPickUpSystemUIEConstArg{
		public PickUpSystemUIEConstArg(
			IUIManager uim, 
			IPickUpSystemProcessFactory pickUpSystemProcessFactory, 
			IPickUpSystemUIElementFactory pickUpSystemUIElementFactory, 
			IUIImage image, 
			IPickUpSystemUIA pickUpSystemUIA,
			ActivationMode activationMode
		): base(
			uim, 
			pickUpSystemProcessFactory, 
			pickUpSystemUIElementFactory, 
			pickUpSystemUIA, 
			image,
			activationMode

		){
			thisPickUpSystemProcessFactory = pickUpSystemProcessFactory;
			thisPickupSystemUIElementFactory = pickUpSystemUIElementFactory;
		}
		readonly IPickUpSystemProcessFactory thisPickUpSystemProcessFactory;
		public IPickUpSystemProcessFactory pickUpSystemProcessFactory{get{return thisPickUpSystemProcessFactory;}}
		readonly IPickUpSystemUIElementFactory thisPickupSystemUIElementFactory;
		public IPickUpSystemUIElementFactory pickUpSystemUIElementFactory{get{return thisPickupSystemUIElementFactory;}}
	}
}
