using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface IItemIconDisemptifyProcess: IProcess{
	}
	public class ItemIconDisemptifyProcess: AbsInterpolatorProcess<IItemIconImageEmptinessInterpolator>, IItemIconDisemptifyProcess{
		public ItemIconDisemptifyProcess(IItemIconEmptificationProcessConstArg arg):base(arg){
			thisItemIconImage = arg.itemIconImage;
			thisEngine = arg.engine;
		}
		readonly IItemIconImage thisItemIconImage;
		readonly IItemIconEmptinessStateEngine thisEngine;
		protected override float GetLatestInitialValueDifference(){
			float curEmptiness = thisItemIconImage.GetEmptiness();
			float targetEmptiness = 1f;
			return targetEmptiness - curEmptiness;
		}
		protected override IItemIconImageEmptinessInterpolator InstantiateInterpolatorWithValues(){
			float curEmptiness = thisItemIconImage.GetEmptiness();
			IItemIconImageEmptinessInterpolator irper = new ItemIconImageEmptinessInterpolator(thisItemIconImage, curEmptiness, 1f);
			return irper;
		}
		protected override void ExpireImple(){
			thisEngine.SetToWaitingForEmptifyState();
		}
	}
	public interface IItemIconEmptificationProcessConstArg: IInterpolatorProcesssConstArg{
		IItemIconImage itemIconImage{get;}
		IItemIconEmptinessStateEngine engine{get;}
	}
	public class ItemIconEmptificationProcessConstArg: InterpolatorProcessConstArg, IItemIconEmptificationProcessConstArg{
		public ItemIconEmptificationProcessConstArg(
			IProcessManager processManager,
			float expireTime,
			
			IItemIconImage itemIconImage,
			IItemIconEmptinessStateEngine engine
		): base(
			processManager,
			ProcessConstraint.ExpireTime,
			expireTime,
			false
		){
			thisItemIconImage = itemIconImage;
			thisEngine = engine;
		}
		readonly IItemIconImage thisItemIconImage;
		public IItemIconImage itemIconImage{get{return thisItemIconImage;}}
		readonly IItemIconEmptinessStateEngine thisEngine;
		public IItemIconEmptinessStateEngine engine{get{return thisEngine;}}

	}
}
