using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using UISystem.PickUpUISystem;

[TestFixture, Category("PickUpSystem"), Ignore]
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
    [Test]
    public void TrySwitchHoveredPUReceiver_ArgIsNotNull_ArgIsNotHoverable_DoesNotCallArgBecomeHovered(){
        PickUpReceiverSwitch<ITestPickUpReceiver> purSwitch = new PickUpReceiverSwitch<ITestPickUpReceiver>();
        ITestPickUpReceiver hoveredPUR = Substitute.For<ITestPickUpReceiver>();
        hoveredPUR.IsHoverable().Returns(false);

        purSwitch.TrySwitchHoveredPUReceiver(hoveredPUR);

        hoveredPUR.DidNotReceive().BecomeHovered();
    }
    [Test]
    public void TrySwitchHoveredPUReceiver_ArgIsNotNull_ArgIsHoverable_CallsArgBecomeHovered(){
        PickUpReceiverSwitch<ITestPickUpReceiver> purSwitch = new PickUpReceiverSwitch<ITestPickUpReceiver>();
        ITestPickUpReceiver hoveredPUR = Substitute.For<ITestPickUpReceiver>();
        hoveredPUR.IsHoverable().Returns(true);

        purSwitch.TrySwitchHoveredPUReceiver(hoveredPUR);

        hoveredPUR.Received(1).BecomeHovered();
    }
    [Test]
    public void TrySwitchHoveredPUReceiver_ArgIsNotNull_ArgIsHoverable_ArgAndCurHoveredAreSame_DoesNotCallCurHoveredBecomeHoverable(){
        PickUpReceiverSwitch<ITestPickUpReceiver> purSwitch = new PickUpReceiverSwitch<ITestPickUpReceiver>();
        ITestPickUpReceiver hoveredPUR = Substitute.For<ITestPickUpReceiver>();
        hoveredPUR.IsHoverable().Returns(true);
        purSwitch.TrySwitchHoveredPUReceiver(hoveredPUR);
        Assert.That(purSwitch.GetHoveredPUReceiver(), Is.SameAs(hoveredPUR));
        
        purSwitch.TrySwitchHoveredPUReceiver(hoveredPUR);

        hoveredPUR.DidNotReceive().BecomeHoverable();
    }
    [Test]
    public void TrySwitchHoveredPUReceiver_ArgIsNotNull_ArgIsHoverable_ArgAndCurHoveredAreNotSame_CallsCurHoveredBecomeHoverable(){
        PickUpReceiverSwitch<ITestPickUpReceiver> purSwitch = new PickUpReceiverSwitch<ITestPickUpReceiver>();
        ITestPickUpReceiver hoveredPUR = Substitute.For<ITestPickUpReceiver>();
        hoveredPUR.IsHoverable().Returns(true);
        purSwitch.TrySwitchHoveredPUReceiver(hoveredPUR);
        Assert.That(purSwitch.GetHoveredPUReceiver(), Is.SameAs(hoveredPUR));
        ITestPickUpReceiver otherHoveredPUR = Substitute.For<ITestPickUpReceiver>();
        otherHoveredPUR.IsHoverable().Returns(true);
        Assert.That(purSwitch.GetHoveredPUReceiver(), Is.Not.SameAs(otherHoveredPUR));

        purSwitch.TrySwitchHoveredPUReceiver(otherHoveredPUR);

        hoveredPUR.Received(1).BecomeHoverable();
    }
    [Test]
    public void TrySwitchHoveredPUReceiver_ArgIsNotNull_ArgIsHoverable_ArgAndCurHoveredAreNotSame_SetsTheNewOneHovered(){
        PickUpReceiverSwitch<ITestPickUpReceiver> purSwitch = new PickUpReceiverSwitch<ITestPickUpReceiver>();
        ITestPickUpReceiver hoveredPUR = Substitute.For<ITestPickUpReceiver>();
        hoveredPUR.IsHoverable().Returns(true);
        purSwitch.TrySwitchHoveredPUReceiver(hoveredPUR);
        Assert.That(purSwitch.GetHoveredPUReceiver(), Is.SameAs(hoveredPUR));
        ITestPickUpReceiver otherHoveredPUR = Substitute.For<ITestPickUpReceiver>();
        otherHoveredPUR.IsHoverable().Returns(true);
        Assert.That(purSwitch.GetHoveredPUReceiver(), Is.Not.SameAs(otherHoveredPUR));

        purSwitch.TrySwitchHoveredPUReceiver(otherHoveredPUR);

        Assert.That(purSwitch.GetHoveredPUReceiver(), Is.SameAs(otherHoveredPUR));
    }
    /*  */
    public interface ITestPickUpReceiver: IPickUpReceiver{

    }
}