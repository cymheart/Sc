using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace PropertyGridEx
{
    public class CustomPropertyCollection : List<CustomProperty>, ICustomTypeDescriptor
    {
        #region ICustomTypeDescriptor 成员

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection properties = new PropertyDescriptorCollection(null);

            foreach (CustomProperty cp in this)
            {
                List<Attribute> attrs = new List<Attribute>();
                //[Browsable(false)]
                if (!cp.IsBrowsable)
                {
                    attrs.Add(new BrowsableAttribute(cp.IsBrowsable));
                }
                //[ReadOnly(true)]
                if (cp.IsReadOnly)
                {
                    attrs.Add(new ReadOnlyAttribute(cp.IsReadOnly));
                }
                //[Editor(typeof(editor),typeof(UITypeEditor))]
                if (cp.EditorType != null)
                {
                    attrs.Add(new EditorAttribute(cp.EditorType, typeof(System.Drawing.Design.UITypeEditor)));
                }

                properties.Add(new CustomPropertyDescriptor(cp, attrs.ToArray()));
            }
            return properties;
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }
}
