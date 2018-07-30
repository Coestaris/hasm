using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens.Constants;
using HASMLib.Parser.SyntaxTokens.Expressions.Exceptions;
using HASMLib.Parser.SyntaxTokens.Preprocessor;
using HASMLib.Parser.SyntaxTokens.Preprocessor.Directives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Parser.SyntaxTokens.Expressions
{
    /// <summary>
    /// Представляет основную расчетную еденицу выражений - токен
    /// </summary>
    public class Token : ICloneable
    {
        ///<summary>
        /// Количество шагов, потраченное на выполненеие оператора
        ///</summary>
        public int Steps { get; internal set; }

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
        /// Ссылка на объект, с которого можно получить значение
        /// </summary>
        public ObjectReference Reference { get; internal set; }

        /// <summary>
        /// Числовое значение данного токена
        /// </summary>
        public Constant Value { get; private set; }

        /// <summary>
        /// Дочерние токены данного
        /// </summary>
        public List<Token> Subtokens { get; internal set; }

        /// <summary>
        /// Установлено ли числовое значение
        /// </summary>
        private bool _valueSet;

        /// <summary>
        /// Установлена ли ссылка на токен
        /// </summary>
        private bool _referenceSet => Reference != null;

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
        public bool CanBeCalculated(MemZone mz = null)
        {
            //Можно посчиатать если:
            //  1. Значение уже подсчитано
            // или
            //  2. Нету дочерных токенов
            //  Если дочерные токены есть, то можно если:
            //  3. Все значениея дочерных уже подсчиатыны
            // или
            //  4. Если ссылки на дочерные установлены 
            //      4.1. И если ссылки - переменные, то если все переменные есть в mz
            // или  
            //  5. Все дочерные значения примитивные, и их можно подсчитать
            // 
            // Но подсчитать нельзя если:
            //   Некоторые дочерние токены имеют унарные функции или операторы

            if (_valueSet)
                return true;

            if (Subtokens != null)
            {
                bool referenceConditional = true;
                bool hasRefsToVar = Subtokens.Exists(p => p.Reference != null && p.Reference.Type == ReferenceType.Variable);
                if (hasRefsToVar) referenceConditional = mz != null &&
                            Subtokens
                            .FindAll(p => p.Reference != null && p.Reference.Type == ReferenceType.Variable)
                            .All(p => mz.RAM.Exists(j => j.Index == p.Reference.Index));

                if (!referenceConditional)
                    return false;

                return (Subtokens.All(p => p._valueSet) || Subtokens.All(p => p.IsSimple)) &&
                        (Subtokens.Exists(p => p.UnaryFunction == null) && Subtokens.Exists(p => p.UnaryOperator == null));
            }
            else if (_referenceSet && Reference.Type == ReferenceType.Variable)
                return mz != null && mz.RAM.Exists(p => p.Index == Reference.Index);
            else
                return true;
        }

        /// <summary>
        /// Строковое представление данного оператора
        /// </summary>
        public override string ToString()
        {
            if (Reference != null)
                return $"Ref to: {Reference}";

            if (_valueSet) return $"Value: {Value}";
            else
            {
                if (UnaryOperator == null)
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
        private static void CalculateValue(Constant value, Token token)
        {
            token._valueSet = true;
            token.Value = value;

            if (token.UnaryFunction != null)
            {
                token.Steps += token.UnaryFunction.FunctionSteps;
                token.Value = token.UnaryFunction.UnaryFunc(token.Value);
                token.UnaryFunction = null;
            }


            if (token.UnaryOperator != null)
            {
                token.Steps += token.UnaryOperator.OperatorSteps;
                token.Value = token.UnaryOperator.UnaryFunc(token.Value);
                token.UnaryOperator = null;
            }
        }

        /// <summary>
        /// Расчитывает числовое значение данного токена. Возможно только в случае если <see cref="CanBeCalculated"/> <see cref="true"/>
        /// </summary>
        /// <returns></returns>
        public Constant Calculate()
        {
            return Calculate(null);
        }

        /// <summary>
        /// Расчитывает числовое значение данного токена. Возможно только в случае если <see cref="CanBeCalculated"/> <see cref="true"/>
        /// </summary>
        /// <returns></returns>
        public Constant Calculate(MemZone zone)
        {
            Steps = 0;

            if (_valueSet)
                return Value;

            if (IsSimple)
            {
                CalculateValue(Parse(zone), this);
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
                CalculateValue(subTokens[maxLeftIndex].Parse(zone), subTokens[maxLeftIndex]);
                CalculateValue(subTokens[maxRightIndex].Parse(zone), subTokens[maxRightIndex]);

                var value = op.BinaryFunc(subTokens[maxLeftIndex].Value, subTokens[maxRightIndex].Value);
                Steps += op.OperatorSteps;

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
        /// Очищает числовое значение данного токена, устанавливая <see cref="_valueSet"/> как false
        /// </summary>
        internal void ClearValue()
        {
            _valueSet = false;
            Value = null;
        }

        /// <summary>
        /// Задает числовое значение данного токена, устанавливая <see cref="_valueSet"/> как true
        /// </summary>
        /// <param name="value"></param>
        internal void SetValue(Constant value)
        {
            _valueSet = true;
            Value = value;
        }

        internal void ResolveName(Func<Token, ObjectReference> ResolveNameFunc, Func<Constant, ObjectReference> RegisterNewConstant)
        {
            if (_valueSet)
                return;

            var error = Constant.Parse(RawValue, out Constant constant);
            if (error != null)
            {
                if (error.Type == ParseErrorType.Syntax_Constant_TooLong || error.Type == ParseErrorType.Syntax_Constant_BaseOverflow)
                    throw new ConstantOverflowException(RawValue, error.Type);

                //Variable
                if (ResolveNameFunc == null)
                    throw new ArgumentNullException("");


                Reference = ResolveNameFunc(this);

                return;
            }
            else
            {
                if (RegisterNewConstant != null)
                {
                    Reference = RegisterNewConstant(constant);
                    return;
                }
            }
        }

        /// <summary>
        /// Получает числовое значение данного токена, если он является примитивным или значение уже подсчитано
        /// </summary>
        private Constant Parse(MemZone zone)
        {
            if (_valueSet)
                return Value;

            if (_referenceSet && Reference.Type == ReferenceType.Define)
            {
                return new Constant(PreprocessorIf.defines.Exists(p => p.Name == RawValue));
            }

            if (_referenceSet && zone != null)
            {
                switch (Reference.Object.Type)
                {
                    case FlashElementType.Variable:
                        return new Constant(zone.RAM.Find(p => p.Index == (Reference.Object as FlashElementVariable).Index));
                    case FlashElementType.Constant:
                        return (Reference.Object as FlashElementConstant).ToConstant();
                    case FlashElementType.Instruction:
                    case FlashElementType.Expression:
                    case FlashElementType.Undefined:
                    default:
                        throw new Exception("Wrong reference object!");
                }

            }
            else
            {
                var error = Constant.Parse(RawValue, out Constant constant);

                if (error != null)
                {
                    if (error.Type == ParseErrorType.Syntax_Constant_TooLong || error.Type == ParseErrorType.Syntax_Constant_BaseOverflow)
                        throw new Exception("Wrong const format");

                    throw new WrongTokenException();
                }
                else
                {
                    return constant;
                }
            }

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
                Reference = Reference,
                _valueSet = _valueSet
            };
        }
    }
}
