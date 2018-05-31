using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

public class UIAdaptorTest{
	[Test]
	public void PseudoUIAdaptor_FindClosestParentUIA_ParentIsNull_ReturnsNull(){
		PseudoTransform trans = CreatePseudoTransform();
		PseudoUIAdaptor uia = new PseudoUIAdaptor(trans);
		trans.parent = null;

		IUIAdaptor closestParentUIA = uia.FindClosestParentUIAdaptor();
		
		Assert.That(closestParentUIA, Is.Null);
	}
	[Test]
	public void PseudoUIAdaptor_FindClosestParentUIA_ParentHasNoParentNorUIA_ReturnsNull(){
		PseudoTransform uiaTrans = CreatePseudoTransform();
		PseudoUIAdaptor uia = new PseudoUIAdaptor(uiaTrans);
		PseudoTransform parent = CreatePseudoTransform();
		parent.SetUIA(null);
		parent.parent = null;
		uiaTrans.parent = parent;

		IUIAdaptor closestParentUIA = uia.FindClosestParentUIAdaptor();
		
		Assert.That(uiaTrans.parent, Is.SameAs(parent));
		Assert.That(closestParentUIA, Is.Null);
	}
	[Test]
	public void PseudoUIAdaptor_FindClosestParentUIA_OnlyOneAncestorHasUIA_ReturnsIt(){
		PseudoTransform uiaTrans = CreatePseudoTransform();
		PseudoUIAdaptor uia = new PseudoUIAdaptor(uiaTrans);
		PseudoTransform parent1G = CreatePseudoTransform();
			uiaTrans.parent = parent1G;
			parent1G.SetUIA(null);
		PseudoTransform parent2G = CreatePseudoTransform();
			parent1G.parent = parent2G;
			parent2G.SetUIA(null);
		PseudoTransform parent3G = CreatePseudoTransform();/* this one */
			parent2G.parent = parent3G;
			IUIAdaptor expectedUIA = Substitute.For<IUIAdaptor>();
			parent3G.SetUIA(expectedUIA);
		PseudoTransform parent4G = CreatePseudoTransform();
			parent3G.parent = parent4G;
			parent4G.SetUIA(null);
			parent4G.parent = null;/* top */
		
		IUIAdaptor actualUIA = uia.FindClosestParentUIAdaptor();

		Assert.That(actualUIA, Is.SameAs(expectedUIA));
	}
	[Test]
	public void PseudoUIAdaptor_FindClosestParentUIA_SomeAncestorsHaveUIAs_ReturnsClosest(){
		PseudoTransform uiaTrans = CreatePseudoTransform();
		PseudoUIAdaptor uia = new PseudoUIAdaptor(uiaTrans);
		PseudoTransform parent1G = CreatePseudoTransform();
			uiaTrans.parent = parent1G;
			parent1G.SetUIA(null);
		PseudoTransform parent2G = CreatePseudoTransform();
			parent1G.parent = parent2G;/* this one has it */
			IUIAdaptor expectedUIA = Substitute.For<IUIAdaptor>();
			parent2G.SetUIA(expectedUIA);
		PseudoTransform parent3G = CreatePseudoTransform();
			parent2G.parent = parent3G;/* this one also has, but not returned */
			IUIAdaptor falseUIA = Substitute.For<IUIAdaptor>();
			parent3G.SetUIA(falseUIA);
		PseudoTransform parent4G = CreatePseudoTransform();
			parent3G.parent = parent4G;
			parent4G.SetUIA(null);
			parent4G.parent = null;/* top */
		
		IUIAdaptor actualUIA = uia.FindClosestParentUIAdaptor();

		Assert.That(actualUIA, Is.SameAs(expectedUIA));
	}
	[Test]
	public void PseudoUIAdaptor_FindClosestParentUIA_AllParentDoNotHaveUIA_ReturnsNull(){
		PseudoTransform uiaTrans = CreatePseudoTransform();
		PseudoUIAdaptor uia = new PseudoUIAdaptor(uiaTrans);
		PseudoTransform parent1G = CreatePseudoTransform();
			uiaTrans.parent = parent1G;
			parent1G.SetUIA(null);
		PseudoTransform parent2G = CreatePseudoTransform();
			parent1G.parent = parent2G;
			parent2G.SetUIA(null);
		PseudoTransform parent3G = CreatePseudoTransform();
			parent2G.parent = parent3G;
			parent3G.SetUIA(null);
		PseudoTransform parent4G = CreatePseudoTransform();
			parent3G.parent = parent4G;
			parent4G.SetUIA(null);
			parent4G.parent = null;/* top */
		
		IUIAdaptor actualUIA = uia.FindClosestParentUIAdaptor();

		Assert.That(actualUIA, Is.Null);
	}
	[Test]
	public void PseudoUIAdaptor_GetChildUIAdaptors_ChildIsEmpty_ReturnsEmptyList(){
		PseudoTransform uiaTrans = CreatePseudoTransform();
		PseudoUIAdaptor uia = new PseudoUIAdaptor(uiaTrans);
		uiaTrans.children = new List<IPseudoTransform>();

		List<IUIAdaptor> actualChildren = uia.GetChildUIAdaptors();

		Assert.That(actualChildren, Is.Not.Null);
		Assert.That(actualChildren, Is.Empty);
	}
	[Test]
	public void PseudoUIAdaptor_GetChildUIAdaptors_SomeChildHasUIAs_ReturnsThem(){
		IPseudoTransform uiaTrans = CreatePseudoTransform();
		PseudoUIAdaptor uia = new PseudoUIAdaptor(uiaTrans);
		IUIAdaptor uia_01;
		CreateAndAddChildWithUIATo(ref uiaTrans, out uia_01);
		CreateAndAddChildWithoutUIATo(ref uiaTrans);
		CreateAndAddChildWithoutUIATo(ref uiaTrans);
		IUIAdaptor uia_04;
		CreateAndAddChildWithUIATo(ref uiaTrans, out uia_04);
		List<IUIAdaptor> expectedUIAs = new List<IUIAdaptor>();
			expectedUIAs.Add(uia_01);
			expectedUIAs.Add(uia_04);

		List<IUIAdaptor> actualChildren = uia.GetChildUIAdaptors();

		Assert.That(actualChildren, Is.Not.Null);
		Assert.That(actualChildren, Is.EqualTo(expectedUIAs));
	}
		IPseudoTransform CreateAndAddChildWithUIATo(ref IPseudoTransform parent, out IUIAdaptor uia){
			IPseudoTransform child = CreatePseudoTransform();
			uia = Substitute.For<IUIAdaptor>();
			child.SetUIA(uia);
			parent.children.Add(child);
			return child;
		}
		IPseudoTransform CreateAndAddChildWithoutUIATo(ref IPseudoTransform uiaTrans){
			IPseudoTransform child = CreatePseudoTransform();
			child.SetUIA(null);
			uiaTrans.children.Add(child);
			return child;
		}
	[Test]
	public void PseudoUIAdaptor_GetChildUIAdaptors_NoChildHasUIA_ReturnsEmpty(){
		IPseudoTransform uiaTrans = CreatePseudoTransform();
		PseudoUIAdaptor uia = new PseudoUIAdaptor(uiaTrans);
		CreateAndAddChildWithoutUIATo(ref uiaTrans);
		CreateAndAddChildWithoutUIATo(ref uiaTrans);
		CreateAndAddChildWithoutUIATo(ref uiaTrans);
		CreateAndAddChildWithoutUIATo(ref uiaTrans);
		CreateAndAddChildWithoutUIATo(ref uiaTrans);

		List<IUIAdaptor> actualUIAs = uia.GetChildUIAdaptors();

		Assert.That(actualUIAs, Is.Not.Null);
		Assert.That(actualUIAs, Is.Empty);
	}
	[Test]
	public void PseudoUIAdaptor_GetChildUIAdaptors_Complex_ReturnsAccordingly(){
		IPseudoTransform uiaTrans = CreatePseudoTransform();
		PseudoUIAdaptor uia = new PseudoUIAdaptor(uiaTrans);
		IPseudoTransform child_1 = CreateAndAddChildWithoutUIATo(ref uiaTrans);
			IPseudoTransform child_1_1 = CreateAndAddChildWithoutUIATo(ref child_1);
				IPseudoTransform child_1_1_1 = CreateAndAddChildWithoutUIATo(ref child_1_1);
				IUIAdaptor uia_1_1_2;/* This */
				IPseudoTransform child_1_1_2 = CreateAndAddChildWithUIATo(ref child_1_1, out uia_1_1_2);
				IPseudoTransform child_1_1_3 = CreateAndAddChildWithoutUIATo(ref child_1_1);
		IUIAdaptor uia_2;/* This */
		IPseudoTransform child_2 = CreateAndAddChildWithUIATo(ref uiaTrans, out uia_2);
			IUIAdaptor uia_2_1;/* NOT this */
			IPseudoTransform child_2_1 = CreateAndAddChildWithUIATo(ref child_2, out uia_2_1);
		IPseudoTransform child_3 = CreateAndAddChildWithoutUIATo(ref uiaTrans);
			IPseudoTransform child_3_1 = CreateAndAddChildWithoutUIATo(ref child_3);
				IPseudoTransform child_3_1_1 = CreateAndAddChildWithoutUIATo(ref child_3_1);
					IPseudoTransform child_3_1_1_1 = CreateAndAddChildWithoutUIATo(ref child_3_1_1);
						IUIAdaptor uia_3_1_1_1_1;/* this */
						IPseudoTransform child_3_1_1_1_1 = CreateAndAddChildWithUIATo(ref child_3_1_1_1, out uia_3_1_1_1_1);
			IPseudoTransform child_3_2 = CreateAndAddChildWithoutUIATo(ref child_3);
			IPseudoTransform child_3_3 = CreateAndAddChildWithoutUIATo(ref child_3);
				IPseudoTransform child_3_3_1 = CreateAndAddChildWithoutUIATo(ref child_3_3);
				IUIAdaptor uia_3_3_2;/* this */
				IPseudoTransform child_3_3_2 = CreateAndAddChildWithUIATo(ref child_3_3, out uia_3_3_2);
					IUIAdaptor uia_3_3_2_1;/* NOT this */
					IPseudoTransform child_3_3_2_1 = CreateAndAddChildWithUIATo(ref child_3_3_2, out uia_3_3_2_1);
		IPseudoTransform child_4 = CreateAndAddChildWithoutUIATo(ref uiaTrans);
		IUIAdaptor uia_5;/* this */
		IPseudoTransform child_5 = CreateAndAddChildWithUIATo(ref uiaTrans, out uia_5);

		List<IUIAdaptor> expectedUIAs = new List<IUIAdaptor>();
			expectedUIAs.Add(uia_1_1_2);
			expectedUIAs.Add(uia_2);
			expectedUIAs.Add(uia_3_1_1_1_1);
			expectedUIAs.Add(uia_3_3_2);
			expectedUIAs.Add(uia_5);
		
		List<IUIAdaptor> actualUIAs = uia.GetChildUIAdaptors();

		Assert.That(actualUIAs, Is.Not.Null);
		Assert.That(actualUIAs, Is.Not.Empty);
		Assert.That(actualUIAs, Is.EqualTo(expectedUIAs));
	}
	/* Test Support Classes */
		[Test]
		public void PseudoTransform_Construction_WhenCalled_ParentIsSetCorrectly(){
			PsTransConstArg arg;
			PseudoTransform trans = CreatePseudoTransform(out arg);

			Assert.That(trans.parent, Is.SameAs(arg.parent));
		}
		[Test]
		public void PseudoTransform_Construction_WhenCalled_ChildrenIsSetCorrectly(){
			PsTransConstArg arg;
			PseudoTransform trans = CreatePseudoTransform(out arg);

			Assert.That(trans.children, Is.SameAs(arg.children));
		}
		[Test]
		public void PseudoTransform_Construction_WhenCalled_UIAIsSetCorrectly(){
			PsTransConstArg arg;
			PseudoTransform trans = CreatePseudoTransform(out arg);

			Assert.That(trans.GetIUIAdaptorComponent(), Is.SameAs(arg.uia));
		}
		[Test][TestCaseSource(typeof(GetChildTestCase), "validCases")]
		public void PseudoTransform_GetChild_SuppliedWithValidIndex_ReturnsCorrectly(int expectedCount){
			PsTransConstArg arg;
			PseudoTransform trans = CreatePseudoTransform(out arg);
			List<IPseudoTransform> expectedEles = CreatePseudoTransList(expectedCount);
			trans.children = expectedEles;

			Assert.That(trans.childCount, Is.EqualTo(expectedCount));
			for(int i = 0; i < trans.childCount; i ++){
				Assert.That(trans.GetChild(i), Is.SameAs(expectedEles[i]));
			}
		}
			List<IPseudoTransform> CreatePseudoTransList(int count){
				List<IPseudoTransform> result = new List<IPseudoTransform>();
				for(int i = 0; i < count; i ++){
					IPseudoTransform newEle = Substitute.For<IPseudoTransform>();
					result.Add(newEle);
				}
				return result;
			}
			class GetChildTestCase{
				static object[] validCases = {
					new object[]{0},
					new object[]{1},
					new object[]{2},
					new object[]{100}
				};
				static object[] invalidCases = {
					new object[]{2, 3},
					new object[]{2, -1},
					new object[]{100, 100}
				};
			}
		[Test][ExpectedException(typeof(System.ArgumentOutOfRangeException))]
		[TestCaseSource(typeof(GetChildTestCase), "invalidCases")]
		public void PseudoTransform_GetChild_SuppliedWithInvalidIndex_ThrowsException(int count, int invalidId){
			PsTransConstArg arg;
			PseudoTransform trans = CreatePseudoTransform(out arg);
			List<IPseudoTransform> elements = CreatePseudoTransList(count);
			trans.children = elements;
			IPseudoTransform invalidChild = trans.GetChild(invalidId);
		}
		public interface IPseudoTransform{
			IPseudoTransform parent{get;set;}
			List<IPseudoTransform> children{get;set;}
			int childCount{get;}
			IPseudoTransform GetChild(int index);
			IUIAdaptor GetIUIAdaptorComponent();
			void SetUIA(IUIAdaptor uia);
		}
		class PseudoTransform: IPseudoTransform{
			public PseudoTransform(IPseudoTransform parent, List<IPseudoTransform> children, IUIAdaptor uia){
				this._parent = parent;
				this._children = children;
				this._uia = uia;
			}
			protected IPseudoTransform _parent;
			public IPseudoTransform parent{
				get{return _parent;}
				set{_parent = value;}
			}
			protected List<IPseudoTransform> _children;
			public List<IPseudoTransform> children{
				get{return _children;}
				set{_children = value;}
			}
			public int childCount{
				get{
					if(_children != null)
						return _children.Count;
					else
						return 0;
				}
			}
			public IPseudoTransform GetChild(int index){
				if(_children != null){
					if(index < childCount && index >= 0)
						return _children[index];
					else
						throw new System.ArgumentOutOfRangeException();
				}
				throw new System.NullReferenceException();
			}
			protected IUIAdaptor _uia;
			public IUIAdaptor GetIUIAdaptorComponent(){
				return _uia;
			}
			public void SetUIA(IUIAdaptor uia){
				this._uia = uia;
			}
		}

		public interface IPseudoMonoBehaviour{
		}
		class PseudoMonoBehaviour: IPseudoMonoBehaviour{
			public PseudoMonoBehaviour(IPseudoTransform transform){
				this.transform = transform;
			}
			protected IPseudoTransform transform;
		}
		class PseudoUIAdaptor: PseudoMonoBehaviour{
			public PseudoUIAdaptor(IPseudoTransform transform): base(transform){
			}
			public IUIAdaptor FindClosestParentUIAdaptor(){
				IPseudoTransform parentToExamine = transform.parent;
				while(true){
					if(parentToExamine != null){
						IUIAdaptor parentUIAdaptor = parentToExamine.GetIUIAdaptorComponent();
						if(parentUIAdaptor != null){
							return parentUIAdaptor;
						}else{
							parentToExamine = parentToExamine.parent;
						}
					}else{
						return null;/* top of the hierarchy */
					}
				}
			}
			public List<IUIAdaptor> GetChildUIAdaptors(){
				return this.FindAllClosestChildUIAdaptors(this.transform);
			}
			List<IUIAdaptor> FindAllClosestChildUIAdaptors(IPseudoTransform transToExamine){
				List<IUIAdaptor> result = new List<IUIAdaptor>();
				for(int i = 0; i < transToExamine.childCount; i ++){
					IPseudoTransform child = transToExamine.GetChild(i);
					IUIAdaptor childUIA = child.GetIUIAdaptorComponent();
					if(childUIA != null){
						result.Add(childUIA);
					}else{
						List<IUIAdaptor> allUIAsOfThisChild = FindAllClosestChildUIAdaptors(child);
						if(allUIAsOfThisChild.Count != 0)
							result.AddRange(allUIAsOfThisChild);
					}
				}
				return result;
			}
		}
		class PsTransConstArg{
			public PsTransConstArg(IPseudoTransform parent, List<IPseudoTransform> children, IUIAdaptor uia){
				this.parent = parent;
				this.children = children;
				this.uia = uia;
			}
			public IPseudoTransform parent;
			public List<IPseudoTransform> children;
			public IUIAdaptor uia;
		}
		PseudoTransform CreatePseudoTransform(out PsTransConstArg arg){
			IPseudoTransform parent = Substitute.For<IPseudoTransform>();
			List<IPseudoTransform> children = new List<IPseudoTransform>();
			IUIAdaptor uia = Substitute.For<IUIAdaptor>();
			arg = new PsTransConstArg(parent, children, uia);
			return new PseudoTransform(parent, children, uia);
		}
		PseudoTransform CreatePseudoTransform(){
			IPseudoTransform parent = Substitute.For<IPseudoTransform>();
			List<IPseudoTransform> children = new List<IPseudoTransform>();
			IUIAdaptor uia = Substitute.For<IUIAdaptor>();
			return new PseudoTransform(parent, children, uia);
		}
	/*  */
}
