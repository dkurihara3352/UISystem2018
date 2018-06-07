using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEquipToolElementUIA: IUIAdaptor{
		void SetEquipIITAM(IEquippableIITAManager eqpIITAM);
		void SetEquipTool(IEquipTool tool);
	}
	public interface IEquippableItemIconUIA: IItemIconUIAdaptor, IEquipToolElementUIA{
		void SetEquippableItem(IEquippableUIItem item);
	}
	public class EquippableItemIconUIA: AbsItemIconUIAdaptor, IEquippableItemIconUIA{
		IEquippableItemIcon eqpII;
		public override IUIElement GetUIElement(){
			return this.eqpII;
		}
		IEquippableIITAManager eqpIITAM;
		public void SetEqpIITAM(IEquippableIITAManager eqpIITAM){
			this.eqpIITAM = eqpIITAM;
		}
		IEquipTool eqpTool;
		public void SetEquipTool(IEquipTool tool){
			this.eqpTool = tool;
		}
		protected override void CreateAndSetUIE(IUIManager uim){
			IUIImage image = this.CreateUIImage();
			IEquippableItemIcon eqpII = new EquippableItemIcon(uim, this, image, this.eqpItem, this.eqpIITAM, this.eqpTool);
		}
		protected override IUIImage CreateUIImage(){

		}
		IEquippableUIItem eqpItem;
		public void SetEquippableItem(IEquippableUIItem item){
			this.eqpItem = item;
		}
	}
}

