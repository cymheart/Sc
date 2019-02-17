using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace PropertyGridEx
{
    public class CustomProperty
    {
        #region Private Variables
        private string _name = string.Empty;
        private object _defaultValue = null;
        private object _value = null;
        private object _objectSource = null;
        private PropertyInfo[] _propertyInfos = null;
        #endregion

        #region Contructors
        public CustomProperty()
        {
        }

        public CustomProperty(string name, string category, string description, object defaultValue, object objectSource)
            : this(name, name, null, category, description, defaultValue, objectSource, null)
        {
        }

        public CustomProperty(string name, string propertyName, string category, string description, object defaultValue, object objectSource)
            : this(name, propertyName, null, category, description, defaultValue, objectSource, null)
        {
        }

        public CustomProperty(string name, string propertyName, string category, string description, object defaultValue, object objectSource, Type editorType)
            : this(name, propertyName, null, category, description, defaultValue, objectSource, editorType)
        {
        }

        public CustomProperty(string name, string propertyName, Type valueType, string category, string description,
            object defaultValue, object objectSource, Type editorType)
            : this(name, new string[] { propertyName }, valueType, null, false, true, category, description, defaultValue, objectSource, editorType)
        {
        }

        public CustomProperty(string name, string[] propertyNames, string category, string description, object defaultValue, object objectSource)
            : this(name, propertyNames, category, description, defaultValue, objectSource, null)
        {
        }

        public CustomProperty(string name, string[] propertyNames, string category, string description, object defaultValue, object objectSource, Type editorType)
            : this(name, propertyNames, null, category, description, defaultValue, objectSource, editorType)
        {
        }

        public CustomProperty(string name, string[] propertyNames, Type valueType, string category, string description,
            object defaultValue, object objectSource, Type editorType)
            : this(name, propertyNames, valueType, null, false, true, category, description, defaultValue, objectSource, editorType)
        {
        }

        public CustomProperty(string name, string[] propertyNames, Type valueType, object value,
            bool isReadOnly, bool isBrowsable, string category, string description, object defaultValue, object objectSource, Type editorType)
        {
            Name = name;
            PropertyNames = propertyNames;
            ValueType = valueType;
            _value = value;
            DefaultValue = defaultValue;
            IsReadOnly = isReadOnly;
            IsBrowsable = isBrowsable;
            Category = category;
            Description = description;
            ObjectSource = objectSource;
            EditorType = editorType;
        }
        #endregion

        #region Public Properties

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;

                if (PropertyNames == null)
                {
                    PropertyNames = new string[] { _name };
                }
            }
        }

        public string[] PropertyNames { get; set; }

        public Type ValueType { get; set; }

        public object DefaultValue
        {
            get { return _defaultValue; }
            set
            {
                _defaultValue = value;
                if (_defaultValue != null)
                {
                    if (_value == null) _value = _defaultValue;
                    if (ValueType == null) ValueType = _defaultValue.GetType();
                }
            }
        }

        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;

                OnValueChanged();
            }
        }

        public bool IsReadOnly { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public bool IsBrowsable { get; set; }

        public object ObjectSource
        {
            get { return _objectSource; }
            set
            {
                _objectSource = value;

                if (_objectSource != null)
                    OnObjectSourceChanged();
            }
        }

        public Type EditorType { get; set; }
        #endregion

        #region Protected Functions

        protected void OnObjectSourceChanged()
        {
            if (_objectSource == null)
                return;

            if (PropertyInfos.Length == 0) return;

            object value = PropertyInfos[0].GetValue(_objectSource, null);
            if (_defaultValue == null) DefaultValue = value;
            _value = value;
        }

        protected void OnValueChanged()
        {
            if (_objectSource == null) return;

            foreach (PropertyInfo propertyInfo in PropertyInfos)
            {
                propertyInfo.SetValue(_objectSource, _value, null);
            }
        }

        protected PropertyInfo[] PropertyInfos
        {
            get
            {
                if (_propertyInfos == null)
                {
                    Type type = ObjectSource.GetType();
                    _propertyInfos = new PropertyInfo[PropertyNames.Length];
                    for (int i = 0; i < PropertyNames.Length; i++)
                    {
                        _propertyInfos[i] = type.GetProperty(PropertyNames[i]);
                    }
                }
                return _propertyInfos;
            }
        }
        #endregion

        #region Prublic Functions
        public void ResetValue()
        {
            Value = DefaultValue;
        }
        #endregion
    }
}
