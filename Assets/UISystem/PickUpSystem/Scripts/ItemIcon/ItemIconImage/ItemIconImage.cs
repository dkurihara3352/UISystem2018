using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UISystem.PickUpUISystem{
	public interface IItemIconImage: IPickableUIImage{
		void SetItemImage(IUIItem item);
		void SetEmptiness(float emptiness);
		float GetEmptiness();
	}
	public class ItemIconImage: PickableUIImage, IItemIconImage{
		public ItemIconImage(IUIItem item, Image image, Transform imageTrans, float defaultDarkness, float darkenedDarkness): base(image, imageTrans, defaultDarkness, darkenedDarkness){
			SetItemImage(item);
		}
		readonly IQuantityRoller thisQuantityRoller;
		public void SetEmptiness(float emptiness){
			Color curColor = thisGraphicComponent.color;
			curColor.a = emptiness;
		}
		public float GetEmptiness(){
			return thisGraphicComponent.color.a;
		}
		public void SetItemImage(IUIItem item){

		}
	}
}
