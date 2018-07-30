using HASMLib.Parser.SyntaxTokens.Constants;
using System;

namespace HASMLib.Parser.SyntaxTokens.Expressions
{
    /// <summary>
    /// Представляет унарную функцию для выполнения в выражениях
    /// </summary>
    public class Function
    {
        public int FunctionSteps { get; private set; }

        /// <summary>
        /// Действие, которое будет выполнять функция с операндом
        /// </summary>
        public Func<Constant, Constant> UnaryFunc { get; private set; }

        /// <summary>
        /// Имя, по которой идентифицируется в строке функция
        /// </summary>
        public string FunctionString { get; private set; }

        /// <summary>
        /// Строкове представление функции
        /// </summary>
        public override string ToString()
        {
            return FunctionString;
        }

        /// <summary>
        /// Создает новый экземпляр класса <see cref="Function"/>
        /// </summary>
        /// <param name="functionString">Действие, которое будет выполнять функция с операндом</param>
        /// <param name="unaryFunction">Имя, по которой идентифицируется в строке функция</param>
        public Function(int steps, string functionString, Func<Constant, Constant> unaryFunction)
        {
            FunctionSteps = steps;

            UnaryFunc = unaryFunction;
            FunctionString = functionString;
        }
    }
}
