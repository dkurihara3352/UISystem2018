using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IItemIcon: IPickableUIE, IPickUpReceiver, IEmptinessStateHandler, IPickabilityStateHandler, IUIItemHandler, ITransferableUIElement, IGhostificationStateHandler{
		void SetUpAsPickedII();

		IIconGroup GetIconGroup();
		void SetSlotID(int id);
		int GetSlotID();

		void RemoveAndMutate();
		void EmptifyAndRemove();
	}
	public abstract class AbsItemIcon : AbsPickableUIE, IItemIcon{
		public AbsItemIcon(IItemIconConstArg arg): base(arg){
			this.iiTAM = arg.iiTAM;
			thisItem = arg.item;
			thisPickUpImplementor = arg.iiPickUpImplementor;
			thisPickUpImplementor.SetItemIcon(this);
			thisIITAStateEngine = arg.iiTAStateEngine;
			thisIITAStateEngine.SetItemIcon(this);
			thisEmptinessStateEngine = arg.emptinessStateEngine;
			thisEmptinessStateEngine.SetItemIcon(this);
		}
		protected override void ActivateImple(){
			base.ActivateImple();
			InitializeTransactionState();
			InitializeEmptinessState();
		}
		/* IITransaction */
			protected readonly IItemIconTransactionManager iiTAM;
			protected IItemIconTransactionStateEngine thisIITAStateEngine;
			void InitializeTransactionState(){
				WaitForPickUp();/* returns immediately in turn */
			}
		/* PickableUIE imple */
			public override void EvaluatePickability(){
				this.UpdateTransferableQuantity(pickedQuantity: 0);
				if( !this.IsEmpty()){
					if( this.IsReorderable() || this.IsTransferable()){
						this.BecomePickable();
						return;
					}
				}
				this.BecomeUnpickable();
			}
			public override void StartImageSmoothFollowDragPosition(){
				base.StartImageSmoothFollowDragPosition();
				if(this.IsWaitingForImageInit())
					InitImage();
			}
			/*  might be better to implement these three below in superclass
			*/
			public override void DeclinePickUp(){}
			public override void BecomeVisuallyPickedUp(){}
			public override void BecomeVisuallyUnpicked(){}
		/* Pickability state handling */
			public void PickUp(){
				thisIITAStateEngine.PickUp();
			}
			public void Drop(){
				thisIITAStateEngine.Drop();
			}
			public void BecomePickable(){
				thisIITAStateEngine.BecomePickable();
			}
			public void BecomeUnpickable(){
				thisIITAStateEngine.BecomeUnpickable();
			}
			public bool IsPickable(){
				return thisIITAStateEngine.IsPickable();
			}
			public bool IsPicked(){
				return thisIITAStateEngine.IsPicked();
			}
		/* Picked Up Behaviour */
			readonly IItemIconPickUpImplementor thisPickUpImplementor;
			public void SetUpAsPickedII(){
				thisPickUpImplementor.SetUpAsPickedII();
			}
		/* Hoverability state handling */
			public void EvaluateHoverability(IPickableUIE pickedUIE){
				IItemIcon pickedII = (IItemIcon)pickedUIE;
				if(this.IsEligibleForHover(pickedII))
					BecomeHoverable();
				else
					BecomeUnhoverable();
			}
			protected abstract bool IsEligibleForHover(IItemIcon pickedII);
			public void WaitForPickUp(){
				thisIITAStateEngine.WaitForPickUp();
			}
			public void BecomeHoverable(){
				thisIITAStateEngine.BecomeHoverable();
			}
			public void BecomeUnhoverable(){
				thisIITAStateEngine.BecomeHoverable();
			}
			public void BecomeHovered(){
				thisIITAStateEngine.BecomeHovered();
			}
			public bool IsHoverable(){
				return thisIITAStateEngine.IsHoverable();
			}
			public bool IsHovered(){
				return thisIITAStateEngine.IsHovered();
			}
			public abstract void CheckForHover();
		/* Emptiness State Handling */
			void InitializeEmptinessState(){
				this.Disemptify(thisItem);
			}
			readonly IItemIconEmptinessStateEngine thisEmptinessStateEngine;
			public bool IsEmpty(){
				return thisEmptinessStateEngine.IsEmpty();
			}
			public void DisemptifyInstantly(IUIItem item){
				thisEmptinessStateEngine.DisemptifyInstantly(item);
			}
			public void EmptifyInstantly(){
				thisEmptinessStateEngine.EmptifyInstantly();
			}
			public void Disemptify(IUIItem item){
				thisEmptinessStateEngine.Disemptify(item);
			}
			public void Emptify(){
				thisEmptinessStateEngine.Emptify();
			}
			public bool IsWaitingForImageInit(){
				return thisEmptinessStateEngine.IsWaitingForImageInit();
			}
			public void InitImage(){
				thisEmptinessStateEngine.InitImage();
			}
			public void IncreaseBy(int quantity, bool doesIncrement){
				thisEmptinessStateEngine.IncreaseBy(quantity, doesIncrement);
			}
			public void DecreaseBy(int quantity, bool doesIncrement, bool removesEmpty){
				thisEmptinessStateEngine.DecreaseBy(quantity, doesIncrement, removesEmpty);
			}
		/* IG */
			protected IIconGroup thisIG;
			public IIconGroup GetIconGroup(){
				return thisIG;
			}
			bool IsReorderable(){
				return thisIG.AllowsInsert() && thisIG.GetSize() > 1;
			}
			int thisSlotID;
			public void SetSlotID(int id){
				thisSlotID = id;
			}
			public int GetSlotID(){
				return thisSlotID;
			}
		/* Item Handling */
			protected IUIItem thisItem;
			public IUIItem GetUIItem(){
				return thisItem;
			}
			public int GetItemQuantity(){
				return thisQuantity;
			}
			protected int thisQuantity{
				get{return thisItem.GetQuantity();}
			}
			void SetQuantity(int q){
				thisItem.SetQuantity(q);
			}
			protected IItemTemplate thisItemTemp{
				get{return thisItem.GetItemTemplate();}
			}
			public IItemTemplate GetItemTemplate(){
				return thisItemTemp;
			}
			public abstract bool HasSameItem(IItemIcon other);
			public abstract bool HasSameItem(IUIItem item);
			public abstract bool LeavesGhost();
			public void UpdateQuantity(int sourceQuantity, int targetQuantity, bool doesIncrement){
				SetQuantity(targetQuantity);
				if(thisItem.IsStackable()){
					thisImage.StopQuantityAnimation();
					if(doesIncrement)
						thisImage.AnimateQuantityImageIncrementally(sourceQuantity, targetQuantity);
					else
						thisImage.AnimateQuantityImageAtOnce(sourceQuantity, targetQuantity);
				}
			}
		/* input handling */
			public override void OnTouch(int touchCount){
				base.OnTouch(touchCount);
				thisPickUpImplementor.CheckAndIncrementPickUpQuantity();
			}
		/* TransferableElement imple */
			public void UpdateTransferableQuantity(int pickedQuantity){
				int transQ = this.CalcTransferableQuantity(pickedQuantity);
				thisTransferableQuantity = transQ;
			}
			int CalcTransferableQuantity(int pickedQuantity){
				int diff = GetMaxTransferableQuantity() - pickedQuantity;
				if(diff >= 0)
					return diff;
				else
					throw new System.InvalidOperationException("pickedQuantity must not exceed max transferable quantity");
			}
			protected abstract int GetMaxTransferableQuantity();
			int thisTransferableQuantity;
			public int GetTransferableQuantity(){
				return thisTransferableQuantity;
			}
			public bool IsTransferable(){
				return thisTransferableQuantity > 0;
			}
			public void TravelTransfer(IIconGroup destIG){
				destIG.ReceiveTravelTransfer(this);
			}
			public void SpotTransfer(IIconGroup destIG){
				destIG.ReceiveSpotTransfer(this);
			}
		/* Travelling */
			public override void HandOverTravel(ITravelableUIE other){
				/*  Update running travel Irper
					update mutation
					pass image trans info to other's ?
				*/
				base.HandOverTravel(other);
				FindAndSwapIIInAllMutations((IItemIcon)other);
			}
			void FindAndSwapIIInAllMutations(IItemIcon targetII){
				GetIconGroup().SwapIIInAllMutations(this, targetII);
			}
		/* Ghostification */
			public void Ghostify(){}
			public void Deghostify(){}
			public bool IsGhostified(){return false;}
		/* mutation */
			public void RemoveAndMutate(){
				thisIG.RemoveIIAndMutate(this);
			}
			public void EmptifyAndRemove(){}
		/*  */
	}
	public interface IItemIconConstArg: IPickableUIEConstArg{
		IItemIconTransactionManager iiTAM{get;}
		IUIItem item{get;}
		IItemIconTransactionStateEngine iiTAStateEngine{get;}
		IItemIconPickUpImplementor iiPickUpImplementor{get;}
		IItemIconEmptinessStateEngine emptinessStateEngine{get;}
	}
	public class ItemIconConstArg: PickableUIEConstArg, IItemIconConstArg{
		public ItemIconConstArg(IUIManager uim, IItemIconUIAdaptor iiUIA, IUIImage image, IDragImageImplementor dragImageImplementor, IItemIconTransactionManager iiTAM, IUIItem item, IItemIconTransactionStateEngine iiTAStateEngine, IItemIconPickUpImplementor pickUpImplementor, IItemIconEmptinessStateEngine emptinessStateEngine): base(uim, iiUIA, image, dragImageImplementor){
			thisIITAM = iiTAM;
			thisItem = item;
			thisIITAStateEngine = iiTAStateEngine;
			thisIIPickUpImplementor = iiPickUpImplementor;
			thisEmptinessStateEngine = emptinessStateEngine;
		}
		readonly IItemIconTransactionManager thisIITAM;
		public IItemIconTransactionManager iiTAM{get{return thisIITAM;}}
		readonly IUIItem thisItem;
		public IUIItem item{get{return thisItem;}}
		readonly IItemIconTransactionStateEngine thisIITAStateEngine;
		public IItemIconTransactionStateEngine iiTAStateEngine{get{return thisIITAStateEngine;}}
		readonly IItemIconPickUpImplementor thisIIPickUpImplementor;
		public IItemIconPickUpImplementor iiPickUpImplementor{get{return thisIIPickUpImplementor;}}
		readonly IItemIconEmptinessStateEngine thisEmptinessStateEngine;
		public IItemIconEmptinessStateEngine emptinessStateEngine{get{return thisEmptinessStateEngine;}}
	}
}
