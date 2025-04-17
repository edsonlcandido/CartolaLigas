using System.Text.RegularExpressions;

namespace CartolaLigas.Extensions
{
    public static class StringExtensions
    {
        public static string ToSlug(this string str)
        {
            // Remove special characters and replace spaces with hyphens

            return Regex.Replace(str
                .ToLower()
                .Replace(" ", "-")
                .Replace(".", "") // Remove pontuação
                .Replace(",", "") // Remove pontuação
                .Replace("!", "") // Remove pontuação
                .Replace("?", "") // Remove pontuação
                .Replace("á", "a") // Remove acentuação
                .Replace("à", "a") // Remove acentuação
                .Replace("ã", "a") // Remove acentuação
                .Replace("â", "a") // Remove acentuação
                .Replace("é", "e") // Remove acentuação
                .Replace("ê", "e") // Remove acentuação
                .Replace("í", "i") // Remove acentuação
                .Replace("ó", "o") // Remove acentuação
                .Replace("õ", "o") // Remove acentuação
                .Replace("ô", "o") // Remove acentuação
                .Replace("ú", "u") // Remove acentuação
                .Replace("ü", "u") // Remove acentuação
                .Replace("ç", "c") // Remove acentuação
                , 
                @"[^a-z0-9\s-]"
                , "")
                .Trim();
        }

    }
}
