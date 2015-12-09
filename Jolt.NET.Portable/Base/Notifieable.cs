using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Jolt.NET.Base
{
    /// <summary>
    /// This class implements the INotifyPropertyChanged interface for later use in data objects.
    /// </summary>
    public class Notifieable : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <exception cref="System.ArgumentNullException">selector</exception>
        /// <exception cref="System.ArgumentException">The body must be a member expression</exception>
        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> selector)
        {
            CheckSelector(selector);
            OnPropertyChanged((selector.Body as MemberExpression).Member.Name);
        }

        /// <exception cref="System.ArgumentNullException">selector</exception>
        /// <exception cref="System.ArgumentException">The body must be a member expression</exception>
        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName]string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <exception cref="System.ArgumentNullException">selector</exception>
        /// <exception cref="System.ArgumentException">The body must be a member expression</exception>
        protected virtual bool Set<T>(ref T field, T value, Expression<Func<T>> selector)
        {
            CheckSelector(selector);
            return Set(ref field, value, (selector.Body as MemberExpression).Member.Name);
        }

        /// <exception cref="System.ArgumentNullException">selector</exception>
        /// <exception cref="System.ArgumentException">The body must be a member expression</exception>
        private void CheckSelector<T>(Expression<Func<T>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException("selector cannot be null");
            var body = selector.Body as MemberExpression;
            
            if (body == null)
                throw new ArgumentException("The body must be a member expression");
        }
    }
}
