using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace PropertyGridEx
{
    public class CustomPropertyDescriptor : PropertyDescriptor
    {
        private CustomProperty _customProperty = null;

        public CustomPropertyDescriptor(CustomProperty customProperty, Attribute[] attrs)
            : base(customProperty.Name, attrs)
        {
            _customProperty = customProperty;
        }

        public override bool CanResetValue(object component)
        {
            return _customProperty.DefaultValue != null;
        }

        public override Type ComponentType
        {
            get { return _customProperty.GetType(); }
        }

        public override object GetValue(object component)
        {
            return _customProperty.Value;
        }

        public override bool IsReadOnly
        {
            get { return _customProperty.IsReadOnly; }
        }

        public override Type PropertyType
        {
            get { return _customProperty.ValueType; }
        }

        public override void ResetValue(object component)
        {
            _customProperty.ResetValue();
        }

        public override void SetValue(object component, object value)
        {
            _customProperty.Value = value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        //
        public override string Description
        {
            get
            {
                return _customProperty.Description;
            }
        }

        public override string Category
        {
            get
            {
                return _customProperty.Category;
            }
        }

        public override string DisplayName
        {
            get
            {
                return _customProperty.Name;
            }
        }

        public override bool IsBrowsable
        {
            get
            {
                return _customProperty.IsBrowsable;
            }
        }

        public object CustomProperty
        {
            get
            {
                return _customProperty;
            }
        }
    }
}
