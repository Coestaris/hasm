using System;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Parser.SyntaxTokens.Expressions
{
    /// <summary>
    /// Представляет основную расчетную еденицу выражений - токен
    /// </summary>
    internal class Token : ICloneable
    {
        /// <summary>
        /// "Сырое", строковое представление данного токена
        /// </summary>
        public string RawValue { get; internal set; }

        /// <summary>
        /// Токен, находящийся слева от данного. Если это самый левый токен, то null
        /// </summary>
        public Token LeftSideToken { get; internal set; }

        /// <summary>
        /// Токен, находящийся справа от данного. Если это самый правый токен, то null
        /// </summary>
        public Token RightSideToken { get; internal set; }

        /// <summary>
        /// Оператор, находящийся между данным токеном и токеном справа. Если это самый правый токен, то null
        /// </summary>
        public Operator LeftSideOperator { get; internal set; }

        /// <summary>
        /// Оператор, находящийся между данным токеном и токеном слева. Если это самый левый токен, то null
        /// </summary>
        public Operator RightSideOperator { get; internal set; }

        /// <summary>
        /// Унарный оператор данного токена
        /// </summary>
        public Operator UnaryOperator { get; internal set; }

        /// <summary>
        /// Унарная функция данного токена
        /// </summary>
        public Function UnaryFunction { get; internal set; }

        /// <summary>
        /// Числовое значение данного токена
        /// </summary>
        public long Value { get; private set; }

        /// <summary>
        /// Дочерние токены данного
        /// </summary>
        public List<Token> Subtokens { get; internal set; }

        /// <summary>
        /// Установлено ли числовое значение
        /// </summary>
        private bool _valueSet;

        /// <summary>
        /// Указывает на простоту данного токена. Если токен не простой, для него необходимо вызвать <see cref="Expression.CreateTokenTree(string, Token)"/>
        /// </summary>
        public bool IsSimple
        {
            get
            {
                //Требует дальнейше обработки токен если:
                //  1. Содержит скобки
                // или
                //  2. Содержит операторные символ
                // или 
                //  3. Дочерние токены имеют унырные операторные символы

                bool result = !RawValue.Contains('(') && RawValue.Intersect(Expression.OperatorCharaters).Count() == 0;
                if (Subtokens != null) result = result && Subtokens.All(p => p.RawValue.Intersect(Expression.UnaryOperatorCharaters).Count() == 0);

                return result;
            }
        }

        /// <summary>
        /// Возможно ли расчитать числовое значение данного токена. Если да, то <see cref="Calculate"/> вернет данное значение
        /// </summary>
        public bool CanBeCalculated
        {
            get
            {
                //Можно посчиатать если:
                //  1. Значение уже подсчитано
                // или
                //  2. Нету дочерных токенов
                //  Если дочерные токены есть, то можно если:
                //  3. Все значениея дочерных уже подсчиатыны
                // или  
                //  4. Все дочерные значения примитивные, и их можно подсчитать
                // 
                // Но подсчитать нельзя если:
                //   Некоторые дочерние токены имеют унарные функции или операторы

                bool result = _valueSet || Subtokens == null;
                if (Subtokens != null)
                    result = result || ((Subtokens.All(p => p._valueSet) || Subtokens.All(p => p.IsSimple)) &&
                         (Subtokens.Exists(p => p.UnaryFunction == null) && Subtokens.Exists(p => p.UnaryOperator == null) ));

                return result;
            }
        }

        /// <summary>
        /// Строковое представление данного оператора
        /// </summary>
        public override string ToString()
        {
            if (_valueSet) return $"Value: {Value}";
            else
            {
                if(UnaryOperator == null)
                    return $"Raw Value: {RawValue}";
                else 
                    return $"Raw Value: {UnaryOperator.OperatorString}({RawValue})";
            }
        }

        /// <summary>
        /// Создает новый экземпляр класса <see cref="Token"/>
        /// </summary>
        /// <param name="rawValue">Строковое представление токена</param>
        public Token(string rawValue)
        {
            RawValue = rawValue;
        }

        /// <summary>
        /// Устанавливает значение функции, применяя унарный оператор и функцию, сохраняя приоритет выполнения
        /// </summary>
        private static void CalculateValue(long value, Token token)
        {
            token._valueSet = true;
            token.Value = value;

            if (token.UnaryFunction != null)
            {
                token.Value = token.UnaryFunction.UnaryFunc(token.Value);
                token.UnaryFunction = null;
            }


            if (token.UnaryOperator != null)
            {
                token.Value = token.UnaryOperator.UnaryFunc(token.Value);
                token.UnaryOperator = null;
            }
        }

        /// <summary>
        /// Расчитывает числовое значение данного токена. Возможно только в случае если <see cref="CanBeCalculated"/> <see cref="true"/>
        /// </summary>
        /// <returns></returns>
        public long Calculate()
        {
            if (_valueSet)
                return Value;

            if (IsSimple)
            {
                CalculateValue(Parse(), this);
                _valueSet = true;
                return Value;
            }

            //1. Найти пару с найбольшим приоритетом
            //2. Удалить ее, заменив ее решением
            //Посторять 1-2 пункты пока не останится последний токен
            //3. Применить унарный оператор

            //Создаем копию саб токенов. Нам не нужно портить изначальный массив.
            var subTokens = Subtokens.Select(p => (Token)p.Clone()).ToList();

            while (subTokens.Count != 1)
            {
                //Индексы в массиве операнов при самом приоритетном операторе
                int maxLeftIndex = -1;
                int maxRightIndex = -1;
                //Приоритет самого приоритетного оператора
                int maxPriority = 0;
                //Самый приоритетный оператор
                Operator op = null;

                //Ищем оператор
                for (int i = 0; i < subTokens.Count; i++)
                {
                    //Ищем по левому, бикоз вай нот
                    if (subTokens[i].LeftSideOperator != null)
                        if (subTokens[i].LeftSideOperator.Priority > maxPriority)
                        {
                            //Запоминаем наши индексы и прочее
                            maxLeftIndex = i - 1;
                            maxRightIndex = i;
                            maxPriority = subTokens[i].LeftSideOperator.Priority;
                            op = subTokens[i].LeftSideOperator;
                        }
                }

                //Хз, можно убрать наверное
                /*if (!subTokens[maxLeftIndex].IsPrimitive)
                    throw new Exception("Token must be primitive");

                if (!subTokens[maxRightIndex].IsPrimitive)
                    throw new Exception("Token must be primitive");*/

                //Получаем числовые значение
                //Учитываем, что операнды могут иметь свои унарные операции и функции. 
                //Их приоритет всегда выше бинарных, потому сразу выполняем их
                CalculateValue(subTokens[maxLeftIndex].Parse(), subTokens[maxLeftIndex]);
                CalculateValue(subTokens[maxRightIndex].Parse(), subTokens[maxRightIndex]);

                var value = op.BinaryFunc(subTokens[maxLeftIndex].Value, subTokens[maxRightIndex].Value);

                //Дебага ради создаем новое строковое значение
                var newRawValue = value.ToString();

                //Важно грамотно сохранить левые и правые токены
                var newToken = new Token(newRawValue)
                {
                    LeftSideOperator = subTokens[maxLeftIndex].LeftSideOperator,
                    LeftSideToken = subTokens[maxLeftIndex].LeftSideToken,

                    RightSideOperator = subTokens[maxRightIndex].RightSideOperator,
                    RightSideToken = subTokens[maxRightIndex].RightSideToken,

                    Value = value,
                    _valueSet = true
                };

                //Подставляем в существующие токены наш новы    
                if (maxLeftIndex - 1 >= 0)
                    subTokens[maxLeftIndex - 1].RightSideToken = newToken;

                if (maxRightIndex + 1 < subTokens.Count)
                    subTokens[maxRightIndex + 1].LeftSideToken = newToken;


                //Удаляем 2 старых
                subTokens.RemoveAt(maxRightIndex);
                subTokens.RemoveAt(maxLeftIndex);
                //Заменяя его на новый
                subTokens.Insert(maxLeftIndex, newToken);
            }

            CalculateValue(subTokens[0].Value, this);
            return Value;
        }

        /// <summary>
        /// Задает числовое значение данного токена, устанавливая <see cref="_valueSet"/> как true
        /// </summary>
        /// <param name="value"></param>
        internal void SetValue(long value)
        {
            _valueSet = true;
            Value = value;
        }

        /// <summary>
        /// Получает числовое значение данного токена, если он является примитивным или значение уже подсчитано
        /// </summary>
        private long Parse()
        {
            if (_valueSet)
                return Value;

            return long.Parse(RawValue);
        }
        
        /// <summary>
        /// Создает новый экземпляр класса <see cref="Token"/>
        /// </summary>
        public Token()
        {

        }

        /// <summary>
        /// Клонирует данный экземпляр класса, создавая новый
        /// </summary>
        public object Clone()
        {
            return new Token(RawValue)
            {
                LeftSideOperator = LeftSideOperator,
                LeftSideToken = LeftSideToken,
                RightSideOperator = RightSideOperator,
                RightSideToken = RightSideToken,
                Subtokens = Subtokens,
                UnaryOperator = UnaryOperator,
                UnaryFunction = UnaryFunction,
                Value = Value,
                 _valueSet = _valueSet
            };
        }
    }
}
