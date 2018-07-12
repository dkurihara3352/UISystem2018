using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IItemIconImage: IPickableUIImage, IQuantityAnimationHandler{
		void SetItemImage(IUIItem item);
		void SetEmptiness(float emptiness);
		float GetEmptiness();
	}
	public class ItemIconImage: PickableUIImage, IItemIconImage{
		public ItemIconImage(IItemIconImageConstArg arg){
			thisQuantityAnimationEngine = arg.quantityAnimationEngine;
			thisQuantityAnimationEngine.SetUIImage(this);
			SetItemImage(arg.item);
		}
		public void SetEmptiness(float emptiness){
			Color curColor = thisImage.color;
			curColor.a = emptiness;
		}
		public float GetEmptiness(){
			return thisImage.color.a;
		}
		readonly IQuantityAnimationEngine thisQuantityAnimationEngine;
		public void AnimateQuantityImageIncrementally(int sourceQuantity, int targetQuantity){
			thisQuantityAnimationEngine.AnimateQuantityImageIncrementally(sourceQuantity, targetQuantity);
		}
		public void AnimateQuantityImageAtOnce(int sourceQuantity, int targetQuantity){
			thisQuantityAnimationEngine.AnimateQuantityImageAtOnce(sourceQuantity, targetQuantity);
		}
		public void SetItemImage(IUIItem item){	
		}
	}
	public interface IItemIconImageConstArg{
		IUIItem item{get;}
		IQuantityAnimationEngine quantityAnimationEngine{get;}
	}
	public class ItemIconImageConstArg: IItemIconImageConstArg{
		public ItemIconImageConstArg(IUIItem item, IQuantityAnimationEngine quantityAnimationEngine){
			thisItem = item;
			thisQuantityAnimationEngine = quantityAnimationEngine;
		}
		readonly IUIItem thisItem;
		public IUIItem item{get{return thisItem;}}
		readonly IQuantityAnimationEngine thisQuantityAnimationEngine;
		public IQuantityAnimationEngine quantityAnimationEngine{get{return thisQuantityAnimationEngine;}}
	}
}
