using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IItemTemplate{
		int GetPickUpStepQuantity();
		int GetMaxEquippableQuantity();
		bool IsStackable();
		bool IsSameAs(IItemTemplate itemTemp);
	}
	public abstract class AbsItemTemplate: IItemTemplate{
		public AbsItemTemplate(IItemTemplateConstArg arg){
			CheckConstArgValidity(arg);
			this.pickupStepQuantity = arg.pickupStepQuantity;
			this.maxEquippableQuantity = arg.maxEquippableQuantity;
			this.maxQuantityPerSlot = arg.maxQuantityPerSlot;
		}
		void CheckConstArgValidity(IItemTemplateConstArg arg){
			int pickupStepQuantity = arg.pickupStepQuantity;
			if(pickupStepQuantity < 1)
				throw new System.InvalidOperationException("pickupStepQuantity must be at least 1");
			int maxEquippableQuantity = arg.maxEquippableQuantity;
			if(maxEquippableQuantity < 1)
				throw new System.InvalidOperationException("maxEquippableQuantity must be at least 1");
			int maxQuantityPerSlot = arg.maxQuantityPerSlot;
			if(maxQuantityPerSlot < 1)
				throw new System.InvalidOperationException("maxQuantityPerSlot must be at least 1");
		}
		public int GetPickUpStepQuantity(){
			return pickupStepQuantity;
		}
		readonly int pickupStepQuantity;
		public int GetMaxEquippableQuantity(){
			return maxEquippableQuantity;
		}
		readonly int maxEquippableQuantity;
		public bool IsStackable(){
			return maxQuantityPerSlot > 1;
		}
		readonly int maxQuantityPerSlot;
		public bool IsSameAs(IItemTemplate other){
			return Object.ReferenceEquals(other, this);
		}
	}
	public interface IItemTemplateConstArg{
		int pickupStepQuantity{get;}
		int maxEquippableQuantity{get;}
		int maxQuantityPerSlot{get;}
	}
	public class ItemTemplateArg: IItemTemplateConstArg{
		public ItemTemplateArg(int pickupStepQuantity, int maxEquippableQuantity, int maxQuantityPerSlot){
			this._pickupStepQuantity = pickupStepQuantity;
			this._maxEquippableQuantity = maxEquippableQuantity;
			this._maxQuantityPerSlot = maxQuantityPerSlot;
		}
		public int pickupStepQuantity{get{return _pickupStepQuantity;}}
		readonly int _pickupStepQuantity;
		public int maxEquippableQuantity{get{return _maxEquippableQuantity;}}
		readonly int _maxEquippableQuantity;
		public int maxQuantityPerSlot{get{return _maxQuantityPerSlot;}}
		readonly int _maxQuantityPerSlot;

	}
	public interface IBowTemplate: IItemTemplate{}
	public interface IWearTemplate: IItemTemplate{}
	public interface ICarriedGearTemplate: IItemTemplate{}
}
