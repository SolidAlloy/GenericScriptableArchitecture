namespace GenericScriptableArchitecture.EditorTests
{
    using System.Collections.Generic;
    using GenericUnityObjects;
    using NUnit.Framework;

    public class ReferenceEquality
    {
        [Test]
        public void References_with_same_constant_values_are_equal()
        {
            var firstReference = new Reference<int>(1);

            var secondReference = new Reference<int>(1);

            Assert.That(firstReference, Is.EqualTo(secondReference));
        }

        [Test]
        public void References_with_different_constant_values_are_not_equal()
        {
            var firstReference = new Reference<int>(1);

            var secondReference = new Reference<int>(2);

            Assert.That(firstReference, Is.Not.EqualTo(secondReference));
        }

        [Test]
        public void Reference_with_same_constant_value_is_not_added_to_set()
        {
            var firstReference = new Reference<int>(1);

            var secondReference = new Reference<int>(1);

            var set = new HashSet<Reference<int>> { firstReference, secondReference };

            Assert.That(set.Count, Is.EqualTo(1));
        }

        [Test]
        public void References_with_same_variable_values_are_equal()
        {
            var firstVariable = GenericScriptableObject.CreateInstance<Variable<int>>();
            firstVariable.Value = 1;
            var firstReference = new Reference<int>(firstVariable);

            var secondVariable = GenericScriptableObject.CreateInstance<Variable<int>>();
            secondVariable.Value = 1;
            var secondReference = new Reference<int>(secondVariable);

            Assert.That(firstReference, Is.EqualTo(secondReference));
        }

        [Test]
        public void References_with_different_variable_values_are_not_equal()
        {
            var firstVariable = GenericScriptableObject.CreateInstance<Variable<int>>();
            firstVariable.Value = 1;
            var firstReference = new Reference<int>(firstVariable);

            var secondVariable = GenericScriptableObject.CreateInstance<Variable<int>>();
            secondVariable.Value = 2;
            var secondReference = new Reference<int>(secondVariable);

            Assert.That(firstReference, Is.Not.EqualTo(secondReference));
        }

        [Test]
        public void Reference_with_same_variable_value_is_not_added_to_set()
        {
            var firstVariable = GenericScriptableObject.CreateInstance<Variable<int>>();
            firstVariable.Value = 1;
            var firstReference = new Reference<int>(firstVariable);

            var secondVariable = GenericScriptableObject.CreateInstance<Variable<int>>();
            secondVariable.Value = 1;
            var secondReference = new Reference<int>(secondVariable);

            var set = new HashSet<Reference<int>> { firstReference, secondReference };

            Assert.That(set.Count, Is.EqualTo(1));
        }

        [Test]
        public void References_with_same_constant_and_variable_values_are_equal()
        {
            var firstReference = new Reference<int>(1);

            var secondVariable = GenericScriptableObject.CreateInstance<Variable<int>>();
            secondVariable.Value = 1;
            var secondReference = new Reference<int>(secondVariable);

            Assert.That(firstReference, Is.EqualTo(secondReference));
        }

        [Test]
        public void References_with_different_constant_and_variable_values_are_not_equal()
        {
            var firstReference = new Reference<int>(1);

            var secondVariable = GenericScriptableObject.CreateInstance<Variable<int>>();
            secondVariable.Value = 2;
            var secondReference = new Reference<int>(secondVariable);

            Assert.That(firstReference, Is.Not.EqualTo(secondReference));
        }

        [Test]
        public void Reference_with_same_variable_value_is_not_added_to_set_with_constant_value()
        {
            var firstReference = new Reference<int>(1);

            var secondVariable = GenericScriptableObject.CreateInstance<Variable<int>>();
            secondVariable.Value = 1;
            var secondReference = new Reference<int>(secondVariable);

            var set = new HashSet<Reference<int>> { firstReference, secondReference };

            Assert.That(set.Count, Is.EqualTo(1));
        }
    }
}