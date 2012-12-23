using EntityEnginev2.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace EntityEngineTest
{
    [TestClass]
    public class EntityEngineTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            TestEntity te = new TestEntity(null, "TestEntity");

            //Getting a default component
            te.Body.Default = true;
            Body body = te.GetComponent<Body>();
            te.Body.Default = false;
            Assert.AreEqual(Vector2.One, body.Position);

            //Getting the first non-default component of a type
            Physics physics = te.GetComponent<Physics>();
            Assert.AreEqual(.9f, physics.Drag);

            //Getting a named component
            Body body2 = te.GetComponent<Body>("Body2");
            Assert.AreEqual(Vector2.One * 2, body2.Position);
        }
    }
}