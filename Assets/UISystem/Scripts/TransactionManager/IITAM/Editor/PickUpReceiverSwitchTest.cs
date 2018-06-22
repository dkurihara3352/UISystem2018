using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

[TestFixture]
public class PickUpReceiverSwitchTest {
    [Test]
    public void Construction_HoveredPUReceiverIsNull(){
        PickUpReceiverSwitch<ITestPickUpReceiver> purSwitch = new PickUpReceiverSwitch<ITestPickUpReceiver>();

        Assert.That(purSwitch.GetHoveredPUReceiver(), Is.Null);
    }
    [Test]
    public void TrySwitchHoveredPUReceiver_ArgIsNotNull_ArgIsHoverable_CurHoveredIsNull_SetItHovered(){
        PickUpReceiverSwitch<ITestPickUpReceiver> purSwitch = new PickUpReceiverSwitch<ITestPickUpReceiver>();
        ITestPickUpReceiver hoveredPUR = Substitute.For<ITestPickUpReceiver>();
        hoveredPUR.IsHoverable().Returns(true);
        purSwitch.TrySwitchHoveredPUReceiver(hoveredPUR);

        Assert.That(purSwitch.GetHoveredPUReceiver(), Is.SameAs(hoveredPUR));
    }
    [Test]
    public void TrySwitchHoveredPUReceiver_ArgIsNull_HoveredIsNotNull_CallsHoveredBecomeHoverable(){
        PickUpReceiverSwitch<ITestPickUpReceiver> purSwitch = new PickUpReceiverSwitch<ITestPickUpReceiver>();
        ITestPickUpReceiver hoveredPUR = Substitute.For<ITestPickUpReceiver>();
        hoveredPUR.IsHoverable().Returns(true);
        purSwitch.TrySwitchHoveredPUReceiver(hoveredPUR);
        Assert.That(purSwitch.GetHoveredPUReceiver(), Is.SameAs(hoveredPUR));

        purSwitch.TrySwitchHoveredPUReceiver(null);

        hoveredPUR.Received(1).BecomeHoverable();
    }
    [Test]
    public void TrySwitchHoveredPUReceiver_ArgIsNull_HoveredIsNotNull_SetsCurHoveredNull(){
        PickUpReceiverSwitch<ITestPickUpReceiver> purSwitch = new PickUpReceiverSwitch<ITestPickUpReceiver>();
        ITestPickUpReceiver hoveredPUR = Substitute.For<ITestPickUpReceiver>();
        hoveredPUR.IsHoverable().Returns(true);
        purSwitch.TrySwitchHoveredPUReceiver(hoveredPUR);
        Assert.That(purSwitch.GetHoveredPUReceiver(), Is.SameAs(hoveredPUR));

        purSwitch.TrySwitchHoveredPUReceiver(null);

        Assert.That(purSwitch.GetHoveredPUReceiver(), Is.Null);
    }

    /*  */
    public interface ITestPickUpReceiver: IPickUpReceiver{

    }
}