using System;

namespace HASMLib.Parser.SyntaxTokens.Expressions
{
    /// <summary>
    /// Предсавляет оператор, для применения к токенам
    /// </summary>
    public class Operator
    {
        /// <summary>
        /// Числовой приоритет выполнения оператора
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// Строка, используемая для поиска оператора в строках
        /// </summary>
        public string OperatorString { get; private set; }

        /// <summary>
        /// Унарный ли оператор
        /// </summary>
        public bool IsUnary { get; private set; }

        /// <summary>
        /// Функция выполнения для бинарных операторов. Иными словами, действие оператора
        /// </summary>
        public Func<long, long, long> BinaryFunc { get; private set; }

        /// <summary>
        /// Функция выполнения для унарных операторов. Иными словами, действие оператора
        /// </summary>
        public Func<long, long> UnaryFunc { get; private set; }

        /// <summary>
        /// Строковое представление <see cref="Operator"/>
        /// </summary>
        public override string ToString()
        {
            return OperatorString;
        }

        /// <summary>
        /// Создает новый экземпляр класса <see cref="Operator"/>. Создаст унарный оператор
        /// </summary>
        /// <param name="operatorString">Строковое представление оператора</param>
        /// <param name="function">Функция этого оператора</param>
        public Operator(string operatorString, Func<long, long> function)
        {
            //Унарные операторы, будем считать, всегда одинаково самые приоритетные
            Priority = int.MaxValue;
            OperatorString = operatorString;
            IsUnary = true;
          
            UnaryFunc = function;
        }

        /// <summary>
        /// Создает новый экземпляр класса <see cref="Operator"/>. Создаст бинарный оператор
        /// </summary>
        /// <param name="priority">Приоритет данного бинарного оператора</param>
        /// <param name="operatorString">Строковое представление оператора</param>
        /// <param name="function">Функция этого оператора</param>
        public Operator(int priority, string operatorString, Func<long, long, long> function)
        {
            Priority = priority;
            OperatorString = operatorString;
            IsUnary = false;

            BinaryFunc = function;
        }
    }
}
