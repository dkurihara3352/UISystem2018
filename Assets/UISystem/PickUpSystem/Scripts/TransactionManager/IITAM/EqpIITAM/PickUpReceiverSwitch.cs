using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
public interface IPickUpReceiverSwitch<T> where T: class, IPickUpReceiver{
		void TrySwitchHoveredPUReceiver(T hovered);
		T GetHoveredPUReceiver();
	}
	public class PickUpReceiverSwitch<T>: IPickUpReceiverSwitch<T> where T: class, IPickUpReceiver{
		public PickUpReceiverSwitch(){
			currentHoveredPUReceiver = null;
		}
		T currentHoveredPUReceiver;
		public T GetHoveredPUReceiver(){return currentHoveredPUReceiver;}
		public void TrySwitchHoveredPUReceiver(T hoveredPURec){
			if(hoveredPURec == null){
				if(currentHoveredPUReceiver == null){
					return;
				}else{
					currentHoveredPUReceiver.BecomeHoverable();
					currentHoveredPUReceiver = null;
				}
			}else{
				if(hoveredPURec.IsHoverable()){
					if(currentHoveredPUReceiver == null){
						currentHoveredPUReceiver = hoveredPURec;
						hoveredPURec.BecomeHovered();
					}else{
						if(hoveredPURec == currentHoveredPUReceiver)
							return;
						else{
							currentHoveredPUReceiver.BecomeHoverable();
							currentHoveredPUReceiver = hoveredPURec;
							hoveredPURec.BecomeHovered();
						}
					}
				}
			}
		}
	}
}
