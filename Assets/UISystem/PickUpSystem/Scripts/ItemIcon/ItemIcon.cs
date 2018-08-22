using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;
namespace UISystem.PickUpUISystem{
	public interface IItemIcon: IPickableUIE, IPickUpReceiver, IEmptinessStateHandler, IPickabilityStateHandler, IUIItemHandler, ITransferabilityHandler, IGhostificationStateHandler{
		void SetUpAsPickedII();

		IIconGroup GetIconGroup();
		void SetSlotID(int id);
		int GetSlotID();

		void RemoveAndMutate();
		bool IsEmpty();
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
			thisTransferabilityHandlerImplementor = arg.transferabilityHandlerImplementor;
			thisTransferabilityHandlerImplementor.SetItemIcon(this);
			thisQuantityAnimationEngine = new QuantityAnimationEngine(thisProcessFactory);
			thisQuantityAnimationEngine.SetQuantityRoller(arg.quantityRoller);
		}
		protected override void OnUIActivate(){
			InitializeTransactionState();
			InitializeEmptinessState();
		}
		void InitializeTransactionState(){
			WaitForPickUp();/* returns immediately in turn */
		}
		protected readonly IItemIconTransactionManager iiTAM;
		protected IItemIconTransactionStateEngine thisIITAStateEngine;
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
		/* IPickUpReceiver imple */
			public void EvaluateHoverability(IPickableUIE pickedUIE){
				IItemIcon pickedII = (IItemIcon)pickedUIE;
				if(this.IsEligibleForHover(pickedII))
					BecomeHoverable();
				else
					BecomeUnhoverable();
			}
			protected abstract bool IsEligibleForHover(IItemIcon pickedII);
			public abstract void CheckForHover();
		/* Hoverability state handling */
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
			public void Emptify(bool removesEmpty){
				thisEmptinessStateEngine.Emptify(removesEmpty);
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
			protected IItemIconImage thisItemIconImage{
				get{return (IItemIconImage)thisImage;}
			}
			public void SetUIItem(IUIItem item){
				thisItem = item;
			}
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
			public void UpdateQuantity(int targetQuantity, bool doesIncrement){
				SetQuantity(targetQuantity);
				if(thisItem.IsStackable()){
					if(doesIncrement)
						thisQuantityAnimationEngine.AnimateQuantityImageIncrementally(targetQuantity);
					else
						thisQuantityAnimationEngine.AnimateQuantityImageAtOnce(targetQuantity);
				}
			}
			public void SetQuantityInstantly(int targetQuantity){
				SetQuantity(targetQuantity);
				thisQuantityAnimationEngine.SetQuantityImageInstantlyTo(targetQuantity);
			}
			readonly IQuantityAnimationEngine thisQuantityAnimationEngine;
		/* input handling */
			protected override void OnTouchImple(int touchCount){
				base.OnTouchImple(touchCount);
				thisPickUpImplementor.CheckAndIncrementPickUpQuantity();
			}
		/* TransferableElement imple */
			ITransferabilityHandlerImplementor thisTransferabilityHandlerImplementor;
			public void UpdateTransferableQuantity(int pickedQuantity){
				thisTransferabilityHandlerImplementor.UpdateTransferableQuantity(pickedQuantity);;
			}
			public int GetTransferableQuantity(){
				return thisTransferabilityHandlerImplementor.GetTransferableQuantity();
			}
			public bool IsTransferable(){
				return thisTransferabilityHandlerImplementor.IsTransferable();
			}
			public void TravelTransfer(IIconGroup destIG){
				thisTransferabilityHandlerImplementor.TravelTransfer(destIG);
			}
			public void SpotTransfer(IIconGroup destIG){
				thisTransferabilityHandlerImplementor.SpotTransfer(destIG);
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
	}
	public interface IItemIconConstArg: IPickableUIEConstArg{
		IItemIconTransactionManager iiTAM{get;}
		IUIItem item{get;}
		IItemIconTransactionStateEngine iiTAStateEngine{get;}
		IItemIconPickUpImplementor iiPickUpImplementor{get;}
		IItemIconEmptinessStateEngine emptinessStateEngine{get;}
		ITransferabilityHandlerImplementor transferabilityHandlerImplementor{get;}
		IQuantityRoller quantityRoller{get;}
	}
	public class ItemIconConstArg: PickableUIEConstArg, IItemIconConstArg{
		public ItemIconConstArg(IUIManager uim, IPickUpSystemProcessFactory pickUpSystemProcessFactory, IPickUpSystemUIElementFactory pickUpSystemUIElementFactory, IItemIconUIAdaptor iiUIA, IItemIconImage itemIconImage, IUITool tool, IDragImageImplementor dragImageImplementor, IVisualPickednessStateEngine visualPickednessStateEngine, IItemIconTransactionManager iiTAM, IUIItem item, IItemIconTransactionStateEngine iiTAStateEngine, IItemIconPickUpImplementor pickUpImplementor, IItemIconEmptinessStateEngine emptinessStateEngine, ITransferabilityHandlerImplementor transferabilityHandlerImplementor, IQuantityRoller quantityRoller): base(uim, pickUpSystemProcessFactory, pickUpSystemUIElementFactory, iiUIA, itemIconImage, tool, dragImageImplementor, visualPickednessStateEngine){
			thisIITAM = iiTAM;
			thisItem = item;
			thisIITAStateEngine = iiTAStateEngine;
			thisIIPickUpImplementor = iiPickUpImplementor;
			thisEmptinessStateEngine = emptinessStateEngine;
			thisQuantityRoller = quantityRoller;
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
		readonly ITransferabilityHandlerImplementor thisTransferabilityHandlerImplementor;
		public ITransferabilityHandlerImplementor transferabilityHandlerImplementor{get{return thisTransferabilityHandlerImplementor;}}
		readonly IQuantityRoller thisQuantityRoller;
		public IQuantityRoller quantityRoller{get{return thisQuantityRoller;}}
	}
}
