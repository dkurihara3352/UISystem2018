using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

[TestFixture]
public class EquipToolPanelTest{
	[Test, TestCaseSource(typeof(EqpToolEquippedItemsPanel_EvaluateHoverability_TestCases), "cases")]
	public void EqpToolEquippedItemsPanel_EvaluateHoverability_PickedTempIsBowOrWear_CallsEngineBecomeHoverable(System.Type tempType){
		IEquipToolPanelConstArg arg;
		EquipToolEquippedItemsPanel eqpItemsPanel = CreateEqpToolEqpItemsPanel(out arg);
		IEquippableItemIcon stubPickedEqpII = CreateStubEqpIIWithItemTemp(tempType);
		IPanelTransactionStateEngine mockEngine = arg.panelTransactionStateEngine;

		eqpItemsPanel.EvaluateHoverability(stubPickedEqpII);

		mockEngine.Received(1).BecomeHoverable();
	}
	public class EqpToolEquippedItemsPanel_EvaluateHoverability_TestCases{
		public static object[] cases = {
			new object[]{typeof(IBowTemplate)},
			new object[]{typeof(IWearTemplate)}
		};
	}
	[Test]
	public void EqpToolEquippedItemsPanel_EvaluateHoverability_PickedTempIsCarriedGear_PickedEqpIsInEqpIG_CallsEngineBecomeHoverable([Values(true, false)]bool isEquipped){
		IEquipToolPanelConstArg arg;
		EquipToolEquippedItemsPanel eqpItemsPanel = CreateEqpToolEqpItemsPanel(out arg);
		IEquippableItemIcon pickedEqpII = CreateStubEqpII(typeof(ICarriedGearTemplate), isInEqpIG:true, isEquipped: isEquipped);
		IPanelTransactionStateEngine mockEngine = arg.panelTransactionStateEngine;

		eqpItemsPanel.EvaluateHoverability(pickedEqpII);

		mockEngine.Received(1).BecomeHoverable();
	}
	[Test]
	public void EqpToolEquippedItemsPanel_EvaluateHoverability_PickedTempIsCarriedGear_PickedEqpIIIsInPoolIG_PickedEqpIIIsEquipped_CallsEngineBecomeHoverable(){
		IEquipToolPanelConstArg arg;
		EquipToolEquippedItemsPanel eqpItemsPanel = CreateEqpToolEqpItemsPanel(out arg);
		IEquippableItemIcon pickedEqpII = CreateStubEqpII(typeof(ICarriedGearTemplate), isInEqpIG:false, isEquipped: true);
		IPanelTransactionStateEngine mockEngine = arg.panelTransactionStateEngine;

		eqpItemsPanel.EvaluateHoverability(pickedEqpII);

		mockEngine.Received(1).BecomeHoverable();

	}
	[Test]
	public void EqpToolEquippedItemsPanel_EvaluateHoverability_PickedTempIsCarriedGear_PickedEqpIIIsInPoolIG_PickedEqpIIIsNotEquipped_RelevantEqpIGSizeIsOne_CallsEngineBecomeHoverable(){
		IEquipToolPanelConstArg arg;
		EquipToolEquippedItemsPanel eqpItemsPanel = CreateEqpToolEqpItemsPanel(out arg);
		IEquippableItemIcon pickedEqpII = CreateStubEqpII(typeof(ICarriedGearTemplate), isInEqpIG:false, isEquipped: false);
		IPanelTransactionStateEngine mockEngine = arg.panelTransactionStateEngine;
		IEqpToolEqpIG<IItemTemplate> relevantIG = Substitute.For<IEqpToolEqpIG<IItemTemplate>>();
		relevantIG.GetSize().Returns(1);
		arg.eqpIITAM.GetRelevantEquipIG(pickedEqpII).Returns(relevantIG);

		eqpItemsPanel.EvaluateHoverability(pickedEqpII);

		mockEngine.Received(1).BecomeHoverable();

	}
	[Test]
	public void EqpToolEquippedItemsPanel_EvaluateHoverability_PickedTempIsCarriedGear_PickedEqpIIIsInPoolIG_PickedEqpIIIsNotEquipped_RelevantEqpIGSizeIsNotOne_relevantIGHasSlotSpace_CallsEngineBecomeHoverable(){
		IEquipToolPanelConstArg arg;
		EquipToolEquippedItemsPanel eqpItemsPanel = CreateEqpToolEqpItemsPanel(out arg);
		IEquippableItemIcon pickedEqpII = CreateStubEqpII(typeof(ICarriedGearTemplate), isInEqpIG:false, isEquipped: false);
		IPanelTransactionStateEngine mockEngine = arg.panelTransactionStateEngine;
		IEqpToolEqpIG<IItemTemplate> relevantIG = Substitute.For<IEqpToolEqpIG<IItemTemplate>>();
		relevantIG.GetSize().Returns(2);
		relevantIG.HasSlotSpace().Returns(true);
		arg.eqpIITAM.GetRelevantEquipIG(pickedEqpII).Returns(relevantIG);

		eqpItemsPanel.EvaluateHoverability(pickedEqpII);

		mockEngine.Received(1).BecomeHoverable();

	}
	[Test]
	public void EqpToolEquippedItemsPanel_EvaluateHoverability_PickedTempIsCarriedGear_PickedEqpIIIsInPoolIG_PickedEqpIIIsNotEquipped_RelevantEqpIGSizeIsNotOne_relevantIGNotHasSlotSpace_CallsEngineBecomeUnhoverable(){
		IEquipToolPanelConstArg arg;
		EquipToolEquippedItemsPanel eqpItemsPanel = CreateEqpToolEqpItemsPanel(out arg);
		IEquippableItemIcon pickedEqpII = CreateStubEqpII(typeof(ICarriedGearTemplate), isInEqpIG:false, isEquipped: false);
		IPanelTransactionStateEngine mockEngine = arg.panelTransactionStateEngine;
		IEqpToolEqpIG<IItemTemplate> relevantIG = Substitute.For<IEqpToolEqpIG<IItemTemplate>>();
		relevantIG.GetSize().Returns(2);
		relevantIG.HasSlotSpace().Returns(false);
		arg.eqpIITAM.GetRelevantEquipIG(pickedEqpII).Returns(relevantIG);

		eqpItemsPanel.EvaluateHoverability(pickedEqpII);

		mockEngine.DidNotReceive().BecomeHoverable();
		mockEngine.Received(1).BecomeUnhoverable();

	}
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
	public IEquippableItemIcon CreateStubEqpIIWithItemTemp(System.Type type){
		IEquippableItemIcon eqpII = Substitute.For<IEquippableItemIcon>();
		IEquippableUIItem item = Substitute.For<IEquippableUIItem>();
		eqpII.GetUIItem().Returns(item);
		if(type == typeof(IBowTemplate))
			item.GetItemTemplate().Returns(Substitute.For<IBowTemplate>());
		else if(type == typeof(IWearTemplate))
			item.GetItemTemplate().Returns(Substitute.For<IWearTemplate>());
		else
			item.GetItemTemplate().Returns(Substitute.For<ICarriedGearTemplate>());
		return eqpII;
	}
	public IEquippableItemIcon CreateStubEqpII(System.Type tempType, bool isInEqpIG, bool isEquipped){
		IEquippableItemIcon eqpII = CreateStubEqpIIWithItemTemp(tempType);
		eqpII.IsInEqpIG().Returns(isInEqpIG);
		eqpII.IsEquipped().Returns(isEquipped);
		return eqpII;
	}
}
