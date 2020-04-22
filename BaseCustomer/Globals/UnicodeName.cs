namespace BaseCustomerEntity.Globals
{
    public static class UnicodeName
    {
        public static string ConvertUnicodeToCode(this string text)
        {
            return text.ConvertUnicodeToCode(string.Empty);
        }
        public static string ConvertUnicodeToCode(this string text, string replaceWhiteSpace)
        {
            return string.IsNullOrEmpty(replaceWhiteSpace) ? RemoveUnicode(text) : RemoveUnicode(text).Replace(" ", replaceWhiteSpace);
        }
        public static string ConvertUnicodeToCode(this string text, string replaceWhiteSpace, bool isLowerCase)
        {
            return isLowerCase ? RemoveUnicode(text).ToLower() : RemoveUnicode(text).Replace(" ", replaceWhiteSpace).ToLower();
        }
        private static string RemoveUnicode(string text)
        {
            try
            {
                string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                                            "đ",
                                            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
                                            "í","ì","ỉ","ĩ","ị",
                                            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
                                            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
                                            "ý","ỳ","ỷ","ỹ","ỵ",};
                string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                                            "d",
                                            "e","e","e","e","e","e","e","e","e","e","e",
                                            "i","i","i","i","i",
                                            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
                                            "u","u","u","u","u","u","u","u","u","u","u",
                                            "y","y","y","y","y",};
                for (int i = 0; i < arr1.Length; i++)
                {
                    text = text.Replace(arr1[i], arr2[i]);
                    text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
                }
                return text;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
