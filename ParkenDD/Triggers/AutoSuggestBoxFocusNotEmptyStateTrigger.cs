﻿using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsStateTriggers;

namespace ParkenDD.Triggers
{
    public class AutoSuggestBoxFocusNotEmptyStateTrigger : StateTriggerBase, ITriggerValue
    {
        /// <summary>
        /// Gets or sets the ItemsControl to check the focus state of
        /// </summary>
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Value"/> DependencyProperty
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof (object), typeof (AutoSuggestBoxFocusNotEmptyStateTrigger),
                new PropertyMetadata(true, OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (AutoSuggestBoxFocusNotEmptyStateTrigger) d;
            var val = e.NewValue;
            var uiElement = val as AutoSuggestBox;
            if (uiElement != null)
            {
                uiElement.TextChanged += (sender, args) =>
                {
                    obj.IsActive = obj.HasFocus && uiElement.Text.Trim().Length > 0;
                };
                uiElement.GotFocus += (sender, args) =>
                {
                    obj.HasFocus = true;
                    obj.IsActive = uiElement.Text.Trim().Length > 0;
                };
                uiElement.LostFocus += (sender, args) => obj.HasFocus = obj.IsActive = false;
            }
        }

        public bool HasFocus;

        #region ITriggerValue

        private bool _isActive;

        /// <summary>
        /// Gets a value indicating whether this trigger is active.
        /// </summary>
        /// <value><c>true</c> if this trigger is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return _isActive; }
            private set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    SetActive(value);
                    IsActiveChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="IsActive" /> property has changed.
        /// </summary>
        public event EventHandler IsActiveChanged;

        #endregion ITriggerValue
    }
}
