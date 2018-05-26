using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using TestTesting;

public class TestScriptTest {

	[Test]
	public void EditorTest() {
		//Arrange
		var gameObject = new GameObject();

		//Act
		//Try to rename the GameObject
		var newGameObjectName = "My game object";
		gameObject.name = newGameObjectName;

		//Assert
		//The object has a new name
		Assert.AreEqual(newGameObjectName, gameObject.name);
	}
	[Test]
	public void TestClass_Instantiation(){
		TestClass testClass = new TestClass();
		Assert.That(testClass, Is.Not.Null);
	}
	[Test]
	public void TestClass_Doubles_WhenCalled_ReturnsDouble(){
		TestClass testClass = new TestClass();
		int result = testClass.Doubles(2);
		Assert.That(result, Is.EqualTo(4));
	}
}
