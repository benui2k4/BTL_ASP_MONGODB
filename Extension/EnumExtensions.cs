using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ASP_MongoDB.Extension
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var enumType = enumValue.GetType();
            var member = enumType.GetMember(enumValue.ToString());
            if (member.Length > 0)
            {
                var displayAttr = member[0].GetCustomAttribute<DisplayAttribute>();
                if (displayAttr != null)
                {
                    return displayAttr.Name;
                }
            }
            return enumValue.ToString();
        }
    }
}
