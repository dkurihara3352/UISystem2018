using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface IItemIconEmptifyProcess: IProcess{
	}
	public class ItemIconEmptifyProcess: AbsInterpolatorProcess<IItemIconImageEmptinessInterpolator>, IItemIconEmptifyProcess{
		public ItemIconEmptifyProcess(IItemIconEmptifyProcessConstArg arg): base(arg){
			thisItemIconImage = arg.itemIconImage;
			thisItemIcon = arg.itemIcon;
			thisRemovesEmpty = arg.removesEmpty;
			thisEngine = arg.engine;
		}
		readonly IItemIconImage thisItemIconImage;
		readonly IItemIcon thisItemIcon;
		readonly bool thisRemovesEmpty;
		readonly IItemIconEmptinessStateEngine thisEngine;
		protected override float GetLatestInitialValueDifference(){
			return 0f - thisItemIconImage.GetEmptiness();
		}
		protected override IItemIconImageEmptinessInterpolator InstantiateInterpolatorWithValues(){
			return new ItemIconImageEmptinessInterpolator(thisItemIconImage, thisItemIconImage.GetEmptiness(), 0f);
		}
		protected override void ExpireImple(){
			if(thisRemovesEmpty){
				IIconGroup ig = thisItemIcon.GetIconGroup();
				ig.RemoveIIAndMutate(thisItemIcon);
			}
			thisEngine.SetToWaitingForDisemptifyState();
		}
	}
	public interface IItemIconEmptifyProcessConstArg: IItemIconEmptificationProcessConstArg{
		IItemIcon itemIcon{get;}
		bool removesEmpty{get;}
	}
	public class ItemIconEmptifyProcessConstArg: ItemIconEmptificationProcessConstArg, IItemIconEmptifyProcessConstArg{
		public ItemIconEmptifyProcessConstArg(
			IProcessManager processManager,
			float expireTime,
			IItemIconImage itemIconImage,
			IItemIconEmptinessStateEngine engine,

			IItemIcon itemIcon,
			bool removesEmpty
		): base(
			processManager,
			expireTime,
			itemIconImage,
			engine
		){
			thisItemIcon = itemIcon;
			thisRemovesEmpty = removesEmpty;
		}
		readonly IItemIcon thisItemIcon;
		public IItemIcon itemIcon{get{return thisItemIcon;}}
		readonly bool thisRemovesEmpty;
		public bool removesEmpty{get{return thisRemovesEmpty;}}
	}
}
