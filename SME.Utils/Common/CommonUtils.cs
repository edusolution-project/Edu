using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SME.Utils.Common
{
    public static class CommonUtils
    {
        public static bool CheckSpecialChar(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                string specialChar = @"~`!@#$%^&*()[]{}\|';:/?><,.";
                foreach (var item in specialChar)
                {
                    if (input.ToUpper().Contains(item)) return false;
                }
            }
            return true;
        }

        public static bool CheckNumber(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return true;
            }
            Regex regex = new Regex(@"[\d]");
            if (regex.IsMatch(input))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckSpecial(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                string specialChar = @"~`!@#$%^*()[]{}\|';:?><,.";
                foreach (var item in specialChar)
                {
                    if (input.ToUpper().Contains(item)) return false;
                }
            }
            return true;
        }

        public static bool CheckVietNamese(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                string vietNamese = @"ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴ";
                foreach (var item in vietNamese)
                {
                    if (input.ToUpper().Contains(item)) return false;
                }
            }
            return true;
        }
        public static bool CheckVietChar(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                string specialChar = @"ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴ";
                foreach (var item in specialChar)
                {
                    if (input.ToUpper().Contains(item)) return false;
                }
            }
            return true;
        }

        public static bool CheckSpace(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                string specialChar = @" ";
                foreach (var item in specialChar)
                {
                    if (input.Contains(item)) return false;
                }
            }
            return true;
        }
        public static string GetPropertyName<TModel, TProperty>(Expression<Func<TModel, TProperty>> property)
        {
            MemberExpression memberExpression = (MemberExpression)property.Body;

            return memberExpression.Member.Name;
        }

        private static readonly string[] VietnameseSigns = new string[]
        {
        "aAeEoOuUiIdDyY",
        "áàạảãâấầậẩẫăắằặẳẵ",
        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
        "éèẹẻẽêếềệểễ",
        "ÉÈẸẺẼÊẾỀỆỂỄ",
        "óòọỏõôốồộổỗơớờợởỡ",
        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
        "úùụủũưứừựửữ",
        "ÚÙỤỦŨƯỨỪỰỬỮ",
        "íìịỉĩ",
        "ÍÌỊỈĨ",
        "đ",
        "Đ",
        "ýỳỵỷỹ",
        "ÝỲỴỶỸ"
        };

        public static string RemoveSign4VietnameseString(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }
        public static string RemoveExtraSpace(string str)
        {
            str = str.Trim();
            while (str.Contains("  "))
            {
                str = str.Replace("  ", " "); 
            }
            return str;
        }
        public static long? GetParentId(this string input)
        {
            string[] lstParentId = input.Split('/');
            if (lstParentId.Count() > 0)
            {
                return Convert.ToInt64(lstParentId[1]);
            }
            return null;
        }
    }
}
