using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Jolt.NET.UI.Common
{
    public class AutoRelayCommand : RelayCommand, IDisposable
    {
        private ISet<string> properties;

        public AutoRelayCommand(Action execute)
            : base(execute)
        {
            Initialize();
        }

        public AutoRelayCommand(Action execute, Func<bool> canExecute)
            : base(execute, canExecute)
        {
            Initialize();
        }

        private void Initialize()
        {
            Messenger.Default.Register<PropertyChangedMessageBase>(this, true, (property) =>
            {
                if (properties != null && properties.Contains(property.PropertyName))
                    RaiseCanExecuteChanged();
            });
        }

        public void DependsOn<T>(Expression<Func<T>> propertyExpression)
        {
            if (properties == null)
                properties = new HashSet<string>();

            properties.Add(GetPropertyName(propertyExpression));
        }

        private string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            var body = propertyExpression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("Invalid argument", "propertyExpression");

            var property = body.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException("Argument is not a property",
                    "propertyExpression");

            return property.Name;
        }

        public void Dispose()
        {
            Messenger.Default.Unregister(this);
        }
    }
}