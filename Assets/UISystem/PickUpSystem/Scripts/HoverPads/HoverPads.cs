using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	namespace PickUpUISystem{
		/*  Hover Pads Manager is MonoBehaviour? 
				an game object which binds all the children pads needs to be placed
				under IG gameobject, just below ItemIcons gameobjects
		*/		
		public interface IHoverPadsManager{
			void ActivateHoverPads();
			void DeactivateHoverPads();
		}
		public class HoverPadsManager: MonoBehaviour, IHoverPadsManager{
			List<IHoverPad> thisHoverPads;
			public void ActivateHoverPads(){
				foreach(IHoverPad hoverPad in thisHoverPads){
					hoverPad.ActivateRecursively(true);
				}
			}
			public void DeactivateHoverPads(){
				foreach(IHoverPad hoverPad in thisHoverPads){
					hoverPad.DeactivateRecursively(true);
				}
			}
		}
		public interface IHoverPad: IUIElement, IPickUpReceiver{}		
	}
}
