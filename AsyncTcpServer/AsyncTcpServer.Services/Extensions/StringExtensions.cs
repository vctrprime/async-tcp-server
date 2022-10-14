using System;

namespace AsyncTcpServer.Services.Extensions
{
    public static class StringExtensions
    {

        /// <summary>
        /// Если запрашиваемый стартовый индекс подстроки не существует, возвращает пустую строку
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string TrySubstring(this string value, int startIndex, int length)
        {
            try
            {
                var result = value.Substring(startIndex, length);
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static byte[] FromHex(this string hex)
        {
            hex = hex.Replace(" ", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
    }
}
