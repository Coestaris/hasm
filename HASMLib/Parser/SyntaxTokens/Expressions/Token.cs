using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib.Parser.SyntaxTokens.Expressions
{
    public class Token : ICloneable
    {
        public string RawValue;

        public Token LeftSideToken;
        public Token RightSideToken;

        public Operator LeftSideOperator;
        public Operator RightSideOperator;

        public Operator UnaryOperator;

        public bool IsSimple => !RawValue.Contains('(') && RawValue.Intersect(Expression.OperatorCharaters).Count() == 0;

        public override string ToString()
        {
            if (_valueSet) return $"Value: {Value}";
            else return $"Raw Value: {RawValue}";
        }

        public Token(string rawValue)
        {
            RawValue = rawValue;
        }

        private bool _valueSet;

        public bool CanBeCalculated => _valueSet || Subtokens == null || Subtokens.All(p =>p._valueSet) || Subtokens.All(p => p.IsSimple);

        public List<Token> Subtokens;

        public int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        public long Calculate()
        {
            Console.WriteLine("Calculating: {0}", RawValue);
            if (_valueSet)
                return Value;

            if (IsSimple)
            {
                SetValue(Parse());
                Console.WriteLine("Result is: {0}", Value);
                return Value;
            }

            //1. Найти пару с найбольшим приоритетом
            //2. Удалить ее, заменив ее решением
            //Посторять 1-2 пункты пока не останится последний токен

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
                var leftValue = subTokens[maxLeftIndex].Parse();
                var rightValue = subTokens[maxRightIndex].Parse();
                var value = op.BinaryFunc(new Operand(leftValue), new Operand(rightValue));

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

            _valueSet = true;
            Value = subTokens[0].Value;

            Console.WriteLine("Result is: {0}", Value);
            return Value;
        }

        public void SetValue(long value)
        {
            _valueSet = true;
            Value = value;
        }

        public long Parse()
        {
            if (_valueSet)
                return Value;

            return long.Parse(RawValue);
        }

        public Token()
        {

        }

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
                Value = Value,
                 _valueSet = _valueSet
            };
        }

        public long Value;
    }
}
