using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vsWork.Utils
{

    /// <summary>
    /// enumの各メンバの文字列を定義する属性クラス
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class EnumTextAttribute : Attribute
    {

        private readonly string text;
        public EnumTextAttribute(string text)
        {
            this.text = text;
        }

        public static string GetText(Enum target)
        {
            Type type = target.GetType();
            string name = Enum.GetName(type, target);
            System.Reflection.FieldInfo fi = type.GetField(name);
            Object[] attrs = fi.GetCustomAttributes(typeof(EnumTextAttribute), false);
            if (attrs.Length > 0)
            {
                EnumTextAttribute attr = (EnumTextAttribute)attrs[0];
                return attr.text;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
