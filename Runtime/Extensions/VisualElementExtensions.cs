using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Hivefive.Utils
{
    public static class VisualElementExtensions
    {
        public static T AddClass<T>(this T visualElement, string className) where T : VisualElement
        {
            Assert.IsNotNull(visualElement);
            className.AssertIsNotNullOrEmpty(nameof(className));

            visualElement.AddToClassList(className);
            return visualElement;
        }

        public static T AddClasses<T>(this T visualElement, params string[] classNames) where T : VisualElement
        {
            Assert.IsNotNull(visualElement);

            if (classNames != null) {
                foreach (var className in classNames) {
                    className.AssertIsNotNullOrEmpty(nameof(className));
                    visualElement.AddToClassList(className);
                }
            }

            return visualElement;
        }

        public static TParent AddChild<TParent, TChild>(this TParent visualElement, TChild child)
            where TParent : VisualElement where TChild : VisualElement
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
                Assert.IsNotNull(child);
                visualElement.AddChild(child);
            }

            return visualElement;
        }

        public static T AddStyleSheet<T>(this T visualElement, StyleSheet styleSheet) where T : VisualElement
        {
            if (visualElement == null) {
                throw new ArgumentNullException(nameof(visualElement));
            }

            if (styleSheet == null) {
                throw new ArgumentNullException(nameof(styleSheet));
            }

            if (!visualElement.styleSheets.Contains(styleSheet)) {
                visualElement.styleSheets.Add(styleSheet);
            }

            return visualElement;
        }

        public static T As<T>(this T visualElement, out T output) where T : VisualElement
        {
            output = visualElement;
            return visualElement;
        }

        public static VisualElement FindAncestor(this VisualElement visualElement, string name)
        {
            while (true) {
                Assert.IsNotNull(visualElement);
                name.AssertIsNotNullOrEmpty();

                if (visualElement.parent != null) {
                    if (visualElement.parent.name == name) {
                        return visualElement.parent;
                    }

                    visualElement = visualElement.parent;
                    continue;
                }

                return null;
            }
        }

        public static bool GetVisible<T>(this T visualElement) where T : VisualElement
        {
            Assert.IsNotNull(visualElement);

            return visualElement.resolvedStyle.display == DisplayStyle.Flex;
        }

        public static T SetBorder<T>(this T visualElement, StyleFloat width, Color color) where T : VisualElement
        {
            SetBorderWidth(visualElement, width);
            SetBorderColor(visualElement, color);
            return visualElement;
        }

        public static T SetBorderColor<T>(this T visualElement, Color color) where T : VisualElement
        {
            Assert.IsNotNull(visualElement);

            visualElement.style.borderBottomColor = color;
            visualElement.style.borderLeftColor = color;
            visualElement.style.borderRightColor = color;
            visualElement.style.borderTopColor = color;

            return visualElement;
        }

        public static T SetBorderRadius<T>(this T visualElement, StyleLength radius) where T : VisualElement
        {
            Assert.IsNotNull(visualElement);

            visualElement.style.borderBottomLeftRadius = radius;
            visualElement.style.borderBottomRightRadius = radius;
            visualElement.style.borderTopLeftRadius = radius;
            visualElement.style.borderTopRightRadius = radius;

            return visualElement;
        }

        public static T SetBorderWidth<T>(this T visualElement, StyleFloat width) where T : VisualElement
        {
            Assert.IsNotNull(visualElement);

            visualElement.style.borderBottomWidth = width;
            visualElement.style.borderLeftWidth = width;
            visualElement.style.borderRightWidth = width;
            visualElement.style.borderTopWidth = width;

            return visualElement;
        }

        public static T SetDisplay<T>(this T visualElement, StyleEnum<DisplayStyle> displayStyle)
            where T : VisualElement
        {
            Assert.IsNotNull(visualElement);

            visualElement.style.display = displayStyle;
            return visualElement;
        }

        public static T SetFlexDirection<T>(this T visualElement, StyleEnum<FlexDirection> flexDirection)
            where T : VisualElement
        {
            Assert.IsNotNull(visualElement);

            visualElement.style.flexDirection = flexDirection;
            return visualElement;
        }

        public static T SetJustifyContent<T>(this T visualElement, StyleEnum<Justify> justifyContent)
            where T : VisualElement
        {
            Assert.IsNotNull(visualElement);

            visualElement.style.justifyContent = justifyContent;
            return visualElement;
        }

        public static T SetName<T>(this T visualElement, string name) where T : VisualElement
        {
            Assert.IsNotNull(visualElement);

            visualElement.name = name ?? "";
            return visualElement;
        }

        public static T SetStyle<T>(this T visualElement, Action<IStyle> action) where T : VisualElement
        {
            Assert.IsNotNull(visualElement);

            action?.Invoke(visualElement.style);
            return visualElement;
        }

        public static T SetText<T>(this T label, string text) where T : Label
        {
            Assert.IsNotNull(label);

            label.text = text;
            return label;
        }

        public static T SetVisible<T>(this T visualElement, bool visible) where T : VisualElement
        {
            Assert.IsNotNull(visualElement);

            visualElement.SetDisplay(visible ? DisplayStyle.Flex : DisplayStyle.None);
            return visualElement;
        }

        public static T WithCallback<T, TEventType>(this T visualElement, EventCallback<TEventType> callback) where T : VisualElement
            where TEventType : EventBase<TEventType>, new()
        {
            Assert.IsNotNull(visualElement);
            Assert.IsNotNull(callback);
            
            visualElement.RegisterCallback(callback);
            return visualElement;
        }
    }
}