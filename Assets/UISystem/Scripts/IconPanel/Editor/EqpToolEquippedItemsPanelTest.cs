using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

[TestFixture]
public class EqpToolEquippedItemsPanelTest: EquipToolPanelTest{
	[Test]
	public void EvaluateHoverability_PickedTempIsBowOrWear_CallsEngineBecomeHoverable(){
		IEquipToolPanelConstArg arg;
		EquipToolEquippedItemsPanel eqpItemsPanel = CreateEqpToolEqpItemsPanel(out arg);
		IEquippableItemIcon stubPickedEqpII = CreateStubEqpII(isBowOrWear: true, isInEqpIG: false, isEquipped: false);
		IPanelTransactionStateEngine mockEngine = arg.panelTransactionStateEngine;

		eqpItemsPanel.EvaluateHoverability(stubPickedEqpII);

		mockEngine.Received(1).BecomeHoverable();
	}
	[Test]
	public void EvaluateHoverability_PickedEqpIIIsNotBowOrWear_PickedEqpIsInEqpIG_CallsEngineBecomeHoverable([Values(true, false)]bool isEquipped){
		IEquipToolPanelConstArg arg;
		EquipToolEquippedItemsPanel eqpItemsPanel = CreateEqpToolEqpItemsPanel(out arg);
		IEquippableItemIcon pickedEqpII = CreateStubEqpII(isBowOrWear: false, isInEqpIG:true, isEquipped: isEquipped);
		IPanelTransactionStateEngine mockEngine = arg.panelTransactionStateEngine;

		eqpItemsPanel.EvaluateHoverability(pickedEqpII);

		mockEngine.Received(1).BecomeHoverable();
	}
	[Test]
	public void EvaluateHoverability_PickedTempIsCarriedGear_PickedEqpIIIsInPoolIG_PickedEqpIIIsEquipped_CallsEngineBecomeHoverable(){
		IEquipToolPanelConstArg arg;
		EquipToolEquippedItemsPanel eqpItemsPanel = CreateEqpToolEqpItemsPanel(out arg);
		IEquippableItemIcon pickedEqpII = CreateStubEqpII(isBowOrWear: false, isInEqpIG:false, isEquipped: true);
		IPanelTransactionStateEngine mockEngine = arg.panelTransactionStateEngine;

		eqpItemsPanel.EvaluateHoverability(pickedEqpII);

		mockEngine.Received(1).BecomeHoverable();

	}
	[Test]
	public void EvaluateHoverability_PickedTempIsCarriedGear_PickedEqpIIIsInPoolIG_PickedEqpIIIsNotEquipped_RelevantEqpIGSizeIsOne_CallsEngineBecomeHoverable(){
		IEquipToolPanelConstArg arg;
		EquipToolEquippedItemsPanel eqpItemsPanel = CreateEqpToolEqpItemsPanel(out arg);
		IEquippableItemIcon pickedEqpII = CreateStubEqpII(isBowOrWear: false, isInEqpIG:false, isEquipped: false);
		IPanelTransactionStateEngine mockEngine = arg.panelTransactionStateEngine;
		IEqpToolEqpIG relevantIG = Substitute.For<IEqpToolEqpIG>();
		relevantIG.GetSize().Returns(1);
		arg.eqpIITAM.GetRelevantEquipIG(pickedEqpII).Returns(relevantIG);

		eqpItemsPanel.EvaluateHoverability(pickedEqpII);

		mockEngine.Received(1).BecomeHoverable();

	}
	[Test]
	public void EvaluateHoverability_PickedTempIsCarriedGear_PickedEqpIIIsInPoolIG_PickedEqpIIIsNotEquipped_RelevantEqpIGSizeIsNotOne_relevantIGHasSlotSpace_CallsEngineBecomeHoverable(){
		IEquipToolPanelConstArg arg;
		EquipToolEquippedItemsPanel eqpItemsPanel = CreateEqpToolEqpItemsPanel(out arg);
		IEquippableItemIcon pickedEqpII = CreateStubEqpII(isBowOrWear: false, isInEqpIG:false, isEquipped: false);
		IPanelTransactionStateEngine mockEngine = arg.panelTransactionStateEngine;
		IEqpToolEqpIG relevantIG = Substitute.For<IEqpToolEqpIG>();
		relevantIG.GetSize().Returns(2);
		relevantIG.HasSlotSpace().Returns(true);
		arg.eqpIITAM.GetRelevantEquipIG(pickedEqpII).Returns(relevantIG);

		eqpItemsPanel.EvaluateHoverability(pickedEqpII);

		mockEngine.Received(1).BecomeHoverable();

	}
	[Test]
	public void EvaluateHoverability_PickedTempIsCarriedGear_PickedEqpIIIsInPoolIG_PickedEqpIIIsNotEquipped_RelevantEqpIGSizeIsNotOne_relevantIGNotHasSlotSpace_CallsEngineBecomeUnhoverable(){
		IEquipToolPanelConstArg arg;
		EquipToolEquippedItemsPanel eqpItemsPanel = CreateEqpToolEqpItemsPanel(out arg);
		IEquippableItemIcon pickedEqpII = CreateStubEqpII(isBowOrWear: false, isInEqpIG:false, isEquipped: false);
		IPanelTransactionStateEngine mockEngine = arg.panelTransactionStateEngine;
		IEqpToolEqpIG relevantIG = Substitute.For<IEqpToolEqpIG>();
		relevantIG.GetSize().Returns(2);
		relevantIG.HasSlotSpace().Returns(false);
		arg.eqpIITAM.GetRelevantEquipIG(pickedEqpII).Returns(relevantIG);

		eqpItemsPanel.EvaluateHoverability(pickedEqpII);

		mockEngine.DidNotReceive().BecomeHoverable();
		mockEngine.Received(1).BecomeUnhoverable();

	}
	[Test]
	public void HoverDefaultTransactionTargetEqpII_WhenCalled_CallsRelevEqpIGDefTATarEqpIICheckForHover(){
		IEquipToolPanelConstArg arg;
		EquipToolEquippedItemsPanel eqpItemsPanel = CreateEqpToolEqpItemsPanel(out arg);
		IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
		IEqpToolEqpIG relevantIG = Substitute.For<IEqpToolEqpIG>();
		IEquippableItemIcon mockTarEqpII = Substitute.For<IEquippableItemIcon>();
		relevantIG.GetDefaultTATargetEqpII(pickedEqpII).Returns(mockTarEqpII);
		arg.eqpIITAM.GetRelevantEquipIG(pickedEqpII).Returns(relevantIG);

		eqpItemsPanel.HoverDefaultTransactionTargetEqpII(pickedEqpII);

		mockTarEqpII.Received(1).CheckForHover();
	}
    [Test]
    public void CheckAndAddEmptyAddTarget_PickedEqpIIIsBowOrWear_DoesNotCallRelevEqpCGIGAddEmpty(){
        IEquipToolPanelConstArg arg;
        EquipToolEquippedItemsPanel eqpItemsPanel = CreateEqpToolEqpItemsPanel(out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        pickedEqpII.IsBowOrWearItemIcon().Returns(true);
        IEqpToolEqpIG relevEqpCGIG = Substitute.For<IEqpToolEqpIG>();
        arg.eqpIITAM.GetRelevantEqpCGearsIG().Returns(relevEqpCGIG);

        eqpItemsPanel.CheckAndAddEmptyAddTarget(pickedEqpII);

        relevEqpCGIG.DidNotReceive().AddEmptyAddTarget(Arg.Any<IEquippableUIItem>());
    }
    [Test]
    public void CheckAndAddEmptyAddTarget_PickedEqpIIIsNotBowOrWear_RelevEqpCGIGHasSameItem_DoesNotCallRelevEqpCGIGAddEmpty(){
        IEquipToolPanelConstArg arg;
        EquipToolEquippedItemsPanel eqpItemsPanel = CreateEqpToolEqpItemsPanel(out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        pickedEqpII.IsBowOrWearItemIcon().Returns(false);
        IEquippableUIItem pickedEqpItem = Substitute.For<IEquippableUIItem>();
        pickedEqpII.GetEquippableItem().Returns(pickedEqpItem);
        IEqpToolEqpCarriedGearsIG relevEqpCGIG = Substitute.For<IEqpToolEqpCarriedGearsIG>();
        arg.eqpIITAM.GetRelevantEqpCGearsIG().Returns(relevEqpCGIG);
        relevEqpCGIG.GetItemIconFromItem(pickedEqpItem).Returns(Substitute.For<IEquippableItemIcon>());

        eqpItemsPanel.CheckAndAddEmptyAddTarget(pickedEqpII);

        relevEqpCGIG.DidNotReceive().AddEmptyAddTarget(Arg.Any<IEquippableUIItem>());
    }
    [Test]
    public void CheckAndAddEmptyAddTarget_PickedEqpIIIsNotBowOrWear_RelevEqpCGIGDoesNotHaveSameItem_CallRelevEqpCGIGAddEmpty(){
        IEquipToolPanelConstArg arg;
        EquipToolEquippedItemsPanel eqpItemsPanel = CreateEqpToolEqpItemsPanel(out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        pickedEqpII.IsBowOrWearItemIcon().Returns(false);
        IEquippableUIItem pickedEqpItem = Substitute.For<IEquippableUIItem>();
        pickedEqpII.GetEquippableItem().Returns(pickedEqpItem);
        IEqpToolEqpCarriedGearsIG relevEqpCGIG = Substitute.For<IEqpToolEqpCarriedGearsIG>();
        arg.eqpIITAM.GetRelevantEqpCGearsIG().Returns(relevEqpCGIG);
        relevEqpCGIG.GetItemIconFromItem(pickedEqpItem).Returns((IEquippableItemIcon)null);

        eqpItemsPanel.CheckAndAddEmptyAddTarget(pickedEqpII);

        relevEqpCGIG.Received(1).AddEmptyAddTarget(Arg.Any<IEquippableUIItem>());
    }
    [Test]
    public void CheckAndRemoveEmptyEqpIIs_RelevEqpCGIGIsOfBowTemp_DoesNotCallRelevIGRemoveEmpty(){
        IEquipToolPanelConstArg arg;
        EquipToolEquippedItemsPanel eqpItemsPanel = CreateEqpToolEqpItemsPanel(out arg);
        IEqpToolEqpBowIG relevEqpIG = Substitute.For<IEqpToolEqpBowIG>();
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        arg.eqpIITAM.GetPickedEqpII().Returns(pickedEqpII);
        arg.eqpIITAM.GetRelevantEquipIG(pickedEqpII).Returns(relevEqpIG);

        eqpItemsPanel.CheckAndRemoveEmptyEqpIIs();

        relevEqpIG.DidNotReceive().RemoveEmptyIIs();
    }
    [Test]
    public void CheckAndRemoveEmptyEqpIIs_RelevEqpCGIGIsOfWearTemp_DoesNotCallRelevIGRemoveEmpty(){
        IEquipToolPanelConstArg arg;
        EquipToolEquippedItemsPanel eqpItemsPanel = CreateEqpToolEqpItemsPanel(out arg);
        IEqpToolEqpWearIG relevEqpIG = Substitute.For<IEqpToolEqpWearIG>();
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        arg.eqpIITAM.GetPickedEqpII().Returns(pickedEqpII);
        arg.eqpIITAM.GetRelevantEquipIG(pickedEqpII).Returns(relevEqpIG);

        eqpItemsPanel.CheckAndRemoveEmptyEqpIIs();

        relevEqpIG.DidNotReceive().RemoveEmptyIIs();
    }
    [Test]
    public void CheckAndRemoveEmptyEqpIIs_RelevEqpCGIGIsOfCarriedGearsTemp_CallsRelevIGRemoveEmpty(){
        IEquipToolPanelConstArg arg;
        EquipToolEquippedItemsPanel eqpItemsPanel = CreateEqpToolEqpItemsPanel(out arg);
        IEqpToolEqpCarriedGearsIG relevEqpIG = Substitute.For<IEqpToolEqpCarriedGearsIG>();
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        arg.eqpIITAM.GetPickedEqpII().Returns(pickedEqpII);
        arg.eqpIITAM.GetRelevantEquipIG(pickedEqpII).Returns(relevEqpIG);

        eqpItemsPanel.CheckAndRemoveEmptyEqpIIs();

        relevEqpIG.Received(1).RemoveEmptyIIs();
    }
	/*  */
	public EquipToolEquippedItemsPanel CreateEqpToolEqpItemsPanel(out IEquipToolPanelConstArg arg){
		IUIManager uim = Substitute.For<IUIManager>();
		IUIAdaptor uia = Substitute.For<IUIAdaptor>();
		IUIImage image = Substitute.For<IUIImage>();
		IEquippableIITAManager eqpIITAM = Substitute.For<IEquippableIITAManager>();
		IEquipTool eqpTool = Substitute.For<IEquipTool>();
		IPanelTransactionStateEngine engine = Substitute.For<IPanelTransactionStateEngine>();
		IEquipToolPanelConstArg thisArg = new EquipToolPanelConstArg(uim, uia, image, eqpIITAM, eqpTool, engine);
		EquipToolEquippedItemsPanel eqpItemsPanel = new EquipToolEquippedItemsPanel(thisArg);

		arg = thisArg;
		return eqpItemsPanel;
	}
	public IEquippableItemIcon CreateStubEqpII(bool isBowOrWear, bool isInEqpIG, bool isEquipped){
		IEquippableItemIcon eqpII = Substitute.For<IEquippableItemIcon>();
		eqpII.IsBowOrWearItemIcon().Returns(isBowOrWear);
		eqpII.IsInEqpIG().Returns(isInEqpIG);
		eqpII.IsEquipped().Returns(isEquipped);
		return eqpII;
	}
}
