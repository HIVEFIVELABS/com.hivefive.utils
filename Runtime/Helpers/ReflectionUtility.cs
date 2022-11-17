using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Hivefive.Utils
{
    public static class ReflectionUtility
    {
        /// <summary>
        ///     Creates compiled lambda expression to get value of field
        /// </summary>
        /// <param name="fieldInfo">
        ///     Field info
        /// </param>
        /// <typeparam name="TReflected">
        ///     Type of object that contains field
        /// </typeparam>
        /// <typeparam name="TValue">
        ///     Type of field value
        /// </typeparam>
        /// <returns>
        ///     Compiled lambda expression to get value of field
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     fieldInfo.<see cref="FieldInfo.DeclaringType" /> is null
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     fieldInfo is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Cannot convert instance to fieldInfo.<see cref="FieldInfo.DeclaringType" />
        /// </exception>
        public static Func<TReflected, TValue> CreateValueGetter<TReflected, TValue>(this FieldInfo fieldInfo)
        {
            var field = InitFieldExpression<TReflected>(fieldInfo, out var instance);
            Expression body = field;
            if (field.Type != typeof(TValue)) {
                body = MakeTyped(field, typeof(TValue));
            }

            return Expression.Lambda<Func<TReflected, TValue>>(body, instance).Compile();
        }

        /// <summary>
        ///     Creates compiled lambda expression to get value of field
        /// </summary>
        /// <param name="fieldInfo">
        ///     Field info
        /// </param>
        /// <typeparam name="TValue">
        ///     Type of field value
        /// </typeparam>
        /// <returns>
        ///     Compiled lambda expression to get value of field
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     fieldInfo.<see cref="FieldInfo.DeclaringType" /> is null
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     fieldInfo is null
        /// </exception>
        public static Func<TValue> CreateValueGetter<TValue>(this FieldInfo fieldInfo)
        {
            var field = InitStaticFieldExpression(fieldInfo);
            Expression body = field;
            if (field.Type != typeof(TValue)) {
                body = MakeTyped(field, typeof(TValue));
            }

            return Expression.Lambda<Func<TValue>>(body).Compile();
        }

        /// <summary>
        ///     Creates compiled lambda expression to get value of property
        /// </summary>
        /// <param name="propertyInfo">
        ///     Property info
        /// </param>
        /// <typeparam name="TReflected">
        ///     Type of object that contains property
        /// </typeparam>
        /// <typeparam name="TValue">
        ///     Type of property value
        /// </typeparam>
        /// <returns>
        ///     Compiled lambda expression to get value of property
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     property doesn't have getter or propertyInfo.<see cref="PropertyInfo.DeclaringType" /> is null
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     propertyInfo is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Cannot convert instance to propertyInfo.<see cref="PropertyInfo.DeclaringType" />
        /// </exception>
        public static Func<TReflected, TValue> CreateValueGetter<TReflected, TValue>(this PropertyInfo propertyInfo)
        {
            var getMethod = propertyInfo.GetGetMethod(true);
            if (getMethod == null) {
                throw new ArgumentException("Property does not have getter", nameof(propertyInfo));
            }

            if (getMethod.ReturnType.IsByRef) {
                throw new ArgumentException("Property getter returns by ref", nameof(propertyInfo));
            }

            var property = InitPropertyExpression<TReflected>(propertyInfo, out var instance);
            Expression body = property;
            if (property.Type != typeof(TValue)) {
                body = MakeTyped(property, typeof(TValue));
            }

            return Expression.Lambda<Func<TReflected, TValue>>(body, instance).Compile();
        }

        /// <summary>
        ///     Creates compiled lambda expression to get value of property
        /// </summary>
        /// <param name="propertyInfo">
        ///     Property info
        /// </param>
        /// <typeparam name="TValue">
        ///     Type of property value
        /// </typeparam>
        /// <returns>
        ///     Compiled lambda expression to get value of property
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     property doesn't have getter or propertyInfo.<see cref="PropertyInfo.DeclaringType" /> is null
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     propertyInfo is null
        /// </exception>
        public static Func<TValue> CreateValueGetter<TValue>(this PropertyInfo propertyInfo)
        {
            var getMethod = propertyInfo.GetGetMethod(true);
            if (getMethod == null) {
                throw new ArgumentException("Property does not have getter", nameof(propertyInfo));
            }

            if (getMethod.ReturnType.IsByRef) {
                throw new ArgumentException("Property getter returns by ref", nameof(propertyInfo));
            }

            var property = InitStaticPropertyExpression(propertyInfo);
            Expression body = property;
            if (property.Type != typeof(TValue)) {
                body = MakeTyped(property, typeof(TValue));
            }

            return Expression.Lambda<Func<TValue>>(body).Compile();
        }

        /// <summary>
        ///     Creates compiled lambda expression to set value of field
        /// </summary>
        /// <param name="fieldInfo">
        ///     Field to get value from
        /// </param>
        /// <typeparam name="TReflected">
        ///     Type of object that contains field
        /// </typeparam>
        /// <typeparam name="TValue">
        ///     Type of field
        /// </typeparam>
        /// <returns>
        ///     Compiled lambda expression to set value of field
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     field is readonly or fieldInfo.<see cref="FieldInfo.DeclaringType" /> is null
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     fieldInfo is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Cannot convert instance to fieldInfo.<see cref="FieldInfo.DeclaringType" />
        /// </exception>
        public static Action<TReflected, TValue> CreateValueSetter<TReflected, TValue>(this FieldInfo fieldInfo)
        {
            if (fieldInfo.IsInitOnly) {
                throw new ArgumentException("Field is readonly", nameof(fieldInfo));
            }

            var field = InitFieldExpression<TReflected>(fieldInfo, out var instance);
            var value = Expression.Parameter(typeof(TValue), "value");
            var assign = Expression.Assign(field, value);
            return Expression.Lambda<Action<TReflected, TValue>>(assign, instance, value).Compile();
        }

        /// <summary>
        ///     Creates compiled lambda expression to set value of field
        /// </summary>
        /// <param name="fieldInfo">
        ///     Field to get value from
        /// </param>
        /// <typeparam name="TValue">
        ///     Type of field
        /// </typeparam>
        /// <returns>
        ///     Compiled lambda expression to set value of field
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     field is readonly or fieldInfo.<see cref="FieldInfo.DeclaringType" /> is null
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     fieldInfo is null
        /// </exception>
        public static Action<TValue> CreateValueSetter<TValue>(this FieldInfo fieldInfo)
        {
            if (fieldInfo.IsInitOnly) {
                throw new ArgumentException("Field is readonly", nameof(fieldInfo));
            }

            var field = InitStaticFieldExpression(fieldInfo);
            var value = Expression.Parameter(typeof(TValue), "value");
            var assign = Expression.Assign(field, value);
            return Expression.Lambda<Action<TValue>>(assign, value).Compile();
        }

        /// <summary>
        ///     Creates compiled lambda expression to set value of property
        /// </summary>
        /// <param name="propertyInfo">
        ///     Property to get value from
        /// </param>
        /// <typeparam name="TReflected">
        ///     Type of object that contains property
        /// </typeparam>
        /// <typeparam name="TValue">
        ///     Type of property
        /// </typeparam>
        /// <returns>
        ///     Compiled lambda expression to set value of property
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     property doesn't have setter or propertyInfo.<see cref="PropertyInfo.DeclaringType" /> is null
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     propertyInfo is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Cannot convert instance to propertyInfo.<see cref="PropertyInfo.DeclaringType" />
        /// </exception>
        public static Action<TReflected, TValue> CreateValueSetter<TReflected, TValue>(this PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetSetMethod(true) == null) {
                throw new ArgumentException("Property does not have setter", nameof(propertyInfo));
            }

            var property = InitPropertyExpression<TReflected>(propertyInfo, out var instance);
            var value = Expression.Parameter(typeof(TValue), "value");
            var assign = Expression.Assign(property, value);
            return Expression.Lambda<Action<TReflected, TValue>>(assign, instance, value).Compile();
        }

        /// <summary>
        ///     Creates compiled lambda expression to set value of property
        /// </summary>
        /// <param name="propertyInfo">
        ///     Property to get value from
        /// </param>
        /// <typeparam name="TValue">
        ///     Type of property
        /// </typeparam>
        /// <returns>
        ///     Compiled lambda expression to set value of property
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     property doesn't have setter or propertyInfo.<see cref="PropertyInfo.DeclaringType" /> is null
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     propertyInfo is null
        /// </exception>
        public static Action<TValue> CreateValueSetter<TValue>(this PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetSetMethod(true) == null) {
                throw new ArgumentException("Property does not have setter", nameof(propertyInfo));
            }

            var property = InitStaticPropertyExpression(propertyInfo);
            var value = Expression.Parameter(typeof(TValue), "value");
            var assign = Expression.Assign(property, value);
            return Expression.Lambda<Action<TValue>>(assign, value).Compile();
        }

        /// <summary>
        ///     MemberExpression initialization common for
        ///     <see cref="CreateValueGetter{TReflected,TValue}(System.Reflection.FieldInfo)" /> and
        ///     <see cref="CreateValueGetter{TReflected,TValue}(System.Reflection.FieldInfo)" />
        /// </summary>
        /// <param name="fieldInfo">
        ///     Field to get value from
        /// </param>
        /// <param name="instance">
        ///     ParameterExpression of instance
        /// </param>
        /// <typeparam name="TReflected">
        ///     Type of object that contains field
        /// </typeparam>
        /// <returns>
        ///     MemberExpression of field
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     fieldInfo.<see cref="FieldInfo.DeclaringType" /> is null
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     fieldInfo is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Cannot convert instance to fieldInfo.<see cref="FieldInfo.DeclaringType" />
        /// </exception>
        private static MemberExpression InitFieldExpression<TReflected>(FieldInfo fieldInfo,
            out ParameterExpression instance)
        {
            if (fieldInfo == null) {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            if (fieldInfo.DeclaringType == null) {
                throw new ArgumentException($"{fieldInfo.Name}.DeclaringType is null");
            }

            instance = Expression.Parameter(typeof(TReflected), "instance");

            if (fieldInfo.DeclaringType!.IsAssignableFrom(typeof(TReflected))) {
                return Expression.Field(instance, fieldInfo);
            }

            var typedInstance = MakeTyped(instance, fieldInfo.DeclaringType);
            return Expression.Field(typedInstance, fieldInfo);
        }

        /// <summary>
        ///     MemberExpression initialization common for
        ///     <see cref="CreateValueGetter{TReflected,TValue}(System.Reflection.PropertyInfo)" /> and
        ///     <see cref="CreateValueSetter{TReflected,TValue}(System.Reflection.PropertyInfo)" />
        /// </summary>
        /// <param name="propertyInfo">
        ///     Property to get value from
        /// </param>
        /// <param name="instance">
        ///     ParameterExpression of instance
        /// </param>
        /// <typeparam name="TReflected">
        ///     Type of object that contains property
        /// </typeparam>
        /// <returns>
        ///     MemberExpression of property
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     propertyInfo.<see cref="PropertyInfo.DeclaringType" /> is null
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     propertyInfo is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Cannot convert instance to propertyInfo.<see cref="PropertyInfo.DeclaringType" />
        /// </exception>
        private static MemberExpression InitPropertyExpression<TReflected>(PropertyInfo propertyInfo,
            out ParameterExpression instance)
        {
            if (propertyInfo == null) {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (propertyInfo.DeclaringType == null) {
                throw new ArgumentException($"{propertyInfo.Name}.DeclaringType is null");
            }

            instance = Expression.Parameter(typeof(TReflected), "instance");

            if (propertyInfo.DeclaringType!.IsAssignableFrom(typeof(TReflected))) {
                return Expression.Property(instance, propertyInfo);
            }

            var typedInstance = MakeTyped(instance, propertyInfo.DeclaringType);
            return Expression.Property(typedInstance, propertyInfo);
        }

        /// <summary>
        ///     MemberExpression initialization common for
        ///     <see cref="CreateValueGetter{TReflected,TValue}(System.Reflection.FieldInfo)" /> and
        ///     <see cref="CreateValueGetter{TReflected,TValue}(System.Reflection.FieldInfo)" />
        /// </summary>
        /// <param name="fieldInfo">
        ///     Field to get value from
        /// </param>
        /// <returns>
        ///     MemberExpression of field
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     fieldInfo.<see cref="FieldInfo.DeclaringType" /> is null
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     fieldInfo is null
        /// </exception>
        private static MemberExpression InitStaticFieldExpression(FieldInfo fieldInfo)
        {
            if (fieldInfo == null) {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            if (fieldInfo.DeclaringType == null) {
                throw new ArgumentException($"{fieldInfo.Name}.DeclaringType is null");
            }

            return Expression.Field(null, fieldInfo);
        }

        /// <summary>
        ///     MemberExpression initialization common for
        ///     <see cref="CreateValueGetter{TValue}(System.Reflection.PropertyInfo)" /> and
        ///     <see cref="CreateValueSetter{TValue}(System.Reflection.PropertyInfo)" />
        /// </summary>
        /// <param name="propertyInfo">
        ///     Property to get value from
        /// </param>
        /// <returns>
        ///     MemberExpression of property
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     propertyInfo.<see cref="PropertyInfo.DeclaringType" /> is null
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     propertyInfo is null
        /// </exception>
        private static MemberExpression InitStaticPropertyExpression(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (propertyInfo.DeclaringType == null) {
                throw new ArgumentException($"{propertyInfo.Name}.DeclaringType is null");
            }

            return Expression.Property(null, propertyInfo);
        }

        /// <summary>
        ///     Convert instance or unbox value type
        /// </summary>
        /// <param name="instance">
        ///     Expression of instance
        /// </param>
        /// <param name="type">
        ///     The new type of expression to convert or unbox
        /// </param>
        /// <returns>
        ///     Expression of unboxed value type or converted instance to memberInfo.<see cref="MemberInfo.DeclaringType" />
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     Cannot convert instance to memberInfo.<see cref="MemberInfo.DeclaringType" />
        /// </exception>
        private static UnaryExpression MakeTyped(Expression instance, Type type)
        {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsValueType) {
                return Expression.Unbox(instance, type);
            }

            try {
                return Expression.Convert(instance, type);
            }
            catch (InvalidOperationException e) {
                throw new InvalidOperationException($"Cannot convert {instance.Type} to {type}", e);
            }
        }
    }
}