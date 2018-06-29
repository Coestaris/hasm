namespace HASMLib.Parser.SyntaxTokens.Expressions
{
    /// <summary>
    /// Класс расширений
    /// </summary>
    public static class Extention
    {
        /// <summary>
        /// Представление лонга в виде <see cref="bool"/>, пародируя логику 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool AsBool(this long value)
        {
            return value == 1 ? true : false;
        }
    }
}
