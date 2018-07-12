using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IReformation{}
	public class Reformation: IReformation{
		public Reformation(IIconGroup iconGroup, List<IItemIcon> newProspectiveIIs, IItemIcon travelTransferII, List<IItemIcon> iisToInit){
			
		}
	}
	public interface ITravelTransferData{}
}

