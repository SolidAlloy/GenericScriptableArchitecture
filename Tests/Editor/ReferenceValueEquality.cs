namespace GenericScriptableArchitecture.EditorTests
{
    using GenericUnityObjects;
    using NUnit.Framework;

    public class ReferenceValueEquality
    {
        [Test]
        public void Variable_and_reference_with_same_constant_value_are_equal()
        {
            var variable = GenericScriptableObject.CreateInstance<Variable<int>>();
            variable.Value = 1;

            var reference = new Reference<int>(1);

            Assert.That(variable, Is.EqualTo(reference));
        }

        [Test]
        public void Variable_and_reference_with_different_constant_value_are_not_equal()
        {
            var variable = GenericScriptableObject.CreateInstance<Variable<int>>();
            variable.Value = 1;

            var reference = new Reference<int>(2);

            Assert.That(variable, Is.Not.EqualTo(reference));
        }

        [Test]
        public void Variable_and_reference_with_same_variable_value_are_equal()
        {
            var firstVariable = GenericScriptableObject.CreateInstance<Variable<int>>();
            firstVariable.Value = 1;

            var secondVariable = GenericScriptableObject.CreateInstance<Variable<int>>();
            secondVariable.Value = 1;
            var reference = new Reference<int>(secondVariable);

            Assert.That(firstVariable, Is.EqualTo(reference));
        }

        [Test]
        public void Variable_and_reference_with_different_variable_value_are_not_equal()
        {
            var firstVariable = GenericScriptableObject.CreateInstance<Variable<int>>();
            firstVariable.Value = 1;

            var secondVariable = GenericScriptableObject.CreateInstance<Variable<int>>();
            secondVariable.Value = 2;
            var reference = new Reference<int>(secondVariable);

            Assert.That(firstVariable, Is.Not.EqualTo(reference));
        }
    }
}