using System.Collections.Generic;
using HiveFive.Utils;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Hivefive.Utils
{
    public static class VisualElementExtensions
    {
        public static T AddClass<T>(this T visualElement, string className) where T : VisualElement

        {
            Assert.IsNotNull(visualElement);
            className.AssertIsNotNullOrEmpty();

            visualElement.AddToClassList(className);
            return visualElement;
        }

        public static T AddChild<T>(this T visualElement, VisualElement child) where T : VisualElement
        {
            Assert.IsNotNull(visualElement);
            Assert.IsNotNull(child);

            visualElement.Add(child);

            return visualElement;
        }

        public static T AddChildren<T>(this T visualElement, IEnumerable<VisualElement> children)
            where T : VisualElement
        {
            Assert.IsNotNull(visualElement);

            foreach (var child in children) {
                visualElement.AddChild(child);
            }

            return visualElement;
        }
    }
}