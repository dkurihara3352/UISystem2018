using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface IVisualPickednessProcess: IProcess{
	}
	public class VisualPickednessProcess: AbsInterpolatorProcess<IPickableUIImageVisualPickednessInterpolator>, IVisualPickednessProcess{
		public VisualPickednessProcess(IVisualPickednessProcessConstArg arg): base(arg){
			thisPickableUIImage = arg.pickableUIImage;
			thisTargetPickedness = arg.targetPickedness;
			thisEngine = arg.engine;
		}
		readonly IPickableUIImage thisPickableUIImage;
		readonly float thisTargetPickedness;
		readonly protected IVisualPickednessStateEngine thisEngine;
		protected override float GetLatestInitialValueDifference(){
			return thisTargetPickedness - thisPickableUIImage.GetVisualPickedness();
		}
		protected override IPickableUIImageVisualPickednessInterpolator InstantiateInterpolatorWithValues(){
			return new PickableUIImageVisualPickednessInterpolator(
				thisPickableUIImage, 
				thisPickableUIImage.GetVisualPickedness(), 
				thisTargetPickedness
			);
		}
	}
	public interface IBecomeVisuallyPickedProcess: IVisualPickednessProcess{}
	public class BecomeVisuallyPickedProcess: VisualPickednessProcess, IBecomeVisuallyPickedProcess{
		public BecomeVisuallyPickedProcess(IVisualPickednessProcessConstArg arg):base(arg){
		}
		protected override void ExpireImple(){
			base.ExpireImple();
			thisEngine.SetToVisuallyPickedUpState();
		}
	}
	public interface IBecomeVisuallyUnpickedrocess: IVisualPickednessProcess{}
	public class BecomeVisuallyUnpickedrocess: VisualPickednessProcess, IBecomeVisuallyUnpickedrocess{
		public BecomeVisuallyUnpickedrocess(IVisualPickednessProcessConstArg arg):base(arg){
		}
		protected override void ExpireImple(){
			base.ExpireImple();
			thisEngine.SetToVisuallyUnpickedState();
		}
	}
	public interface IPickableUIImageVisualPickednessInterpolator: IInterpolator{}
	public class PickableUIImageVisualPickednessInterpolator: AbsInterpolator, IPickableUIImageVisualPickednessInterpolator{
		public PickableUIImageVisualPickednessInterpolator(
			IPickableUIImage pickableUIImage, 
			float sourceVisualPickedness, 
			float targetVisualPickedness
		){
			thisPickableUIImage = pickableUIImage;
			thisSourceVisualPickedness = sourceVisualPickedness;
			thisTargetVisualPickedness = targetVisualPickedness;
		}
		readonly IPickableUIImage thisPickableUIImage;
		readonly float thisSourceVisualPickedness;
		readonly float thisTargetVisualPickedness;
		protected override void InterpolateImple(float normalizedT){
			float newVisualPickedness = Mathf.Lerp(thisSourceVisualPickedness, thisTargetVisualPickedness, normalizedT);
			thisPickableUIImage.SetVisualPickedness(newVisualPickedness);
		}
		public override void Terminate(){
			thisPickableUIImage.SetVisualPickedness(thisTargetVisualPickedness);
		}
	}





	public interface IVisualPickednessProcessConstArg: IInterpolatorProcesssConstArg{
		IPickableUIImage pickableUIImage{get;}
		float targetPickedness{get;}
		IVisualPickednessStateEngine engine{get;}
	}
	public class VisualPickednessProcessConstArg: InterpolatorProcessConstArg, IVisualPickednessProcessConstArg{
		public VisualPickednessProcessConstArg(
			IProcessManager processManager,
			ProcessConstraint processConstraint,
			float constraintValue,
			bool useSpringT,

			IPickableUIImage pickableUIImage,
			float targetPickedness,
			IVisualPickednessStateEngine engine
		): base(
			processManager,
			processConstraint,
			constraintValue,
			useSpringT
		){
			thisPickableUIImage = pickableUIImage;
			thisTargetPickedness = targetPickedness;
			thisEngine = engine;
		}
		readonly IPickableUIImage thisPickableUIImage;
		public IPickableUIImage pickableUIImage{get{return thisPickableUIImage;}}
		readonly float thisTargetPickedness;
		public float targetPickedness{get{return thisTargetPickedness;}}
		readonly IVisualPickednessStateEngine thisEngine;
		public IVisualPickednessStateEngine engine{get{return thisEngine;}}
	}
}
