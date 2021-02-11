using System.Collections.Generic;
using GenericUnityObjects;
using NUnit.Framework;

namespace GenericScriptableArchitecture.EditorTests
{
    public class VariableEquality
    {
        [Test]
        public void Variables_with_same_values_are_equal()
        {
            var firstVariable = GenericScriptableObject.CreateInstance<Variable<int>>();
            firstVariable.Value = 1;

            var secondVariable = GenericScriptableObject.CreateInstance<Variable<int>>();
            secondVariable.Value = 1;

            Assert.That(firstVariable, Is.EqualTo(secondVariable));
        }

        [Test]
        public void Variables_with_different_values_are_not_equal()
        {
            var firstVariable = GenericScriptableObject.CreateInstance<Variable<int>>();
            firstVariable.Value = 1;

            var secondVariable = GenericScriptableObject.CreateInstance<Variable<int>>();
            secondVariable.Value = 2;

            Assert.That(firstVariable, Is.Not.EqualTo(secondVariable));
        }

        [Test]
        public void Variable_with_same_value_is_not_added_to_set()
        {
            var firstVariable = GenericScriptableObject.CreateInstance<Variable<int>>();
            firstVariable.Value = 1;

            var secondVariable = GenericScriptableObject.CreateInstance<Variable<int>>();
            secondVariable.Value = 1;

            var set = new HashSet<Variable<int>> { firstVariable, secondVariable };

            Assert.That(set.Count, Is.EqualTo(1));
        }
    }
}
