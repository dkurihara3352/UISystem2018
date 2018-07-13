using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IItemIconImage: IPickableUIImage{
		void SetItemImage(IUIItem item);
		void SetEmptiness(float emptiness);
		float GetEmptiness();
	}
	public class ItemIconImage: PickableUIImage, IItemIconImage{
		public ItemIconImage(IItemIconImageConstArg arg){
			SetItemImage(arg.item);
		}
		readonly IQuantityRoller thisQuantityRoller;
		public void SetEmptiness(float emptiness){
			Color curColor = thisImage.color;
			curColor.a = emptiness;
		}
		public float GetEmptiness(){
			return thisImage.color.a;
		}
		public void SetItemImage(IUIItem item){	
		}
	}
	public interface IItemIconImageConstArg{
		IUIItem item{get;}
	}
	public class ItemIconImageConstArg: IItemIconImageConstArg{
		public ItemIconImageConstArg(IUIItem item){
			thisItem = item;
		}
		readonly IUIItem thisItem;
		public IUIItem item{get{return thisItem;}}
	}
}
