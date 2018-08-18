using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using UISystem.PickUpUISystem;

[TestFixture, Category("PickUpSystem"), Ignore]
public class EqpToolPoolItemsPanelTest{
    [Test]
    public void EvaluateHoverability_PickedEqpIIIsBowOrWear_DoesNotCallEngineBecomeHoverable(){
        IEquipToolPanelConstArg arg;
        EqpToolPoolItemsPanel panel = CreateEqpToolPoolItemsPanel(out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        pickedEqpII.IsBowOrWearItemIcon().Returns(true);
        IPanelTransactionStateEngine engine = arg.panelTransactionStateEngine;

        panel.EvaluateHoverability(pickedEqpII);

        engine.DidNotReceive().BecomeHoverable();
    }
    [Test]
    public void EvaluateHoverability_PickedEqpIIIsNotBowOrWear_CallsEngineBecomeHoverable(){
        IEquipToolPanelConstArg arg;
        EqpToolPoolItemsPanel panel = CreateEqpToolPoolItemsPanel(out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        pickedEqpII.IsBowOrWearItemIcon().Returns(false);
        IPanelTransactionStateEngine engine = arg.panelTransactionStateEngine;

        panel.EvaluateHoverability(pickedEqpII);

        engine.Received(1).BecomeHoverable();
    }
    public EqpToolPoolItemsPanel CreateEqpToolPoolItemsPanel(out IEquipToolPanelConstArg arg){
        IEquipToolPanelConstArg thisArg = Substitute.For<IEquipToolPanelConstArg>();
        thisArg.uim.Returns(Substitute.For<IUIManager>());
        thisArg.uia.Returns(Substitute.For<IUIAdaptor>());
        thisArg.image.Returns(Substitute.For<IUIImage>());
        thisArg.eqpIITAM.Returns(Substitute.For<IEquippableIITAManager>());
        thisArg.eqpTool.Returns(Substitute.For<IEquipTool>());
        thisArg.panelTransactionStateEngine.Returns(Substitute.For<IPanelTransactionStateEngine>());
        EqpToolPoolItemsPanel panel = new EqpToolPoolItemsPanel(thisArg);

        arg = thisArg;
        return panel;
    }
}
