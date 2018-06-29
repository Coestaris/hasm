using System;
using System.Collections.Generic;

namespace HASMLib.Parser.SyntaxTokens.Expressions
{
    /// <summary>
    /// Основной класс для подсчета числовых значений текстовых выражений
    /// </summary>
    public class Expression
    {
        /// <summary>
        /// Список всех операторных символов
        /// </summary>
        public static List<char> OperatorCharaters { get; private set; }

        /// <summary>
        /// Список унарных операторных символов
        /// </summary>
        public static List<char> UnaryOperatorCharaters { get; private set; }

        /// <summary>
        /// Список бинарных операторных символов
        /// </summary>
        public static List<char> BinaryOperatorCharaters { get; private set; }

        /// <summary>
        /// Задает глобальные параметры, необходимые для работы парсера.
        /// Такие как <see cref="OperatorCharaters"/>, <see cref="UnaryOperatorCharaters"/> и <see cref="BinaryOperatorCharaters"/>
        /// Вызывается автоматически, но может быть вызван и пользователем заранее
        /// </summary>
        public static void InitGlobals()
        {
            OperatorCharaters = new List<char>();
            UnaryOperatorCharaters = new List<char>();
            BinaryOperatorCharaters = new List<char>();
            foreach (Operator op in Operators)
            {
                foreach (char c in op.OperatorString)
                {
                    if (!OperatorCharaters.Contains(c))
                        OperatorCharaters.Add(c);

                    if (op.IsUnary && !UnaryOperatorCharaters.Contains(c))
                        UnaryOperatorCharaters.Add(c);

                    if (!op.IsUnary && !BinaryOperatorCharaters.Contains(c))
                        BinaryOperatorCharaters.Add(c);
                }
            }
        }

        /// <summary>
        /// Список всех поддерживаемых операторов.
        /// Добавление своих не приветсвуется (они взяты со спецификации ANSI C), но это возможно
        /// </summary>
        public static readonly List<Operator> Operators = new List<Operator>()
        {
            //Unary 
            new Operator("!", (a) => a == 1 ? 0 : 1),
            new Operator("~", (a) => ~ a),
            //new Operator(14, "-", false, (a) => - a.Value), Шо делать с унарным минусом пока хз

            //Binnary
            new Operator(13, "*", (a, b) => a * b),
            new Operator(13, "/", (a, b) => a / b),

            new Operator(12, "+", (a, b) => a + b),
            new Operator(12, "-", (a, b) => a - b),

            new Operator(11, "<<", (a, b) => a << (int)b),
            new Operator(11, ">>", (a, b) => a >> (int)b),

            new Operator(10, "<", (a, b) => a < b ? 1 : 0),
            new Operator(10, "<=", (a, b) => a <= b ? 1 : 0),
            new Operator(10, ">", (a, b) => a > b ? 1 : 0),
            new Operator(10, ">=", (a, b) => a >= b ? 1 : 0),

            new Operator(9, "!=", (a, b) => a != b ? 1 : 0),
            new Operator(9, "==", (a, b) => a == b ? 1 : 0),

            new Operator(8, "&", (a, b) => a & b),
            new Operator(7, "^", (a, b) => a ^ b),
            new Operator(6, "|", (a, b) => a | b),
            new Operator(5, "&&", (a, b) => a.AsBool() && b.AsBool() ? 1 : 0),
            new Operator(4, "||", (a, b) => a.AsBool() || b.AsBool() ? 1 : 0)
        };

        /// <summary>
        /// Строковое представление выражения
        /// </summary>
        public string Value;

        /// <summary>
        /// Построенный (кешированный) граф выражений
        /// </summary>
        private Token TokenTree;

        /// <summary>
        /// Основной метод разбиение строки на узлы графа. Рекурсивно самовызывается для скобок.
        /// </summary>
        /// <param name="input">Входящая строка для парса</param>
        /// <param name="parentToken">Токен, в который будет ложиться все сабтокены. Для корневого оставить null</param>
        /// <returns>Список сабнодов, полученных с входящей строки</returns>
        private List<Token> CreateTokenTree(string input, Token parentToken)
        {
            //Если токен корневой, то нельзя оставлять его пустым
            if (parentToken == null)
                parentToken = new Token(input);

            //Накопительная строка просматриваемого токена
            string currentToken = "";
            //Накопительная строка просматриваемого оператора
            string currentOperator = "";
            //Унарный оператор, который будет добавлен к следующему токену
            Operator unaryOperatorToAdd = null;
            //Указатель на то, что предыдущий символ был токеном
            bool lastCharWasOperator = false;
            //Указатель на то, что текущий накопительный оператор - унарный
            bool operatorIsUnary = false;

            //Список "сырых" накопительныйх токенов
            List<Token> tokens = new List<Token>();
            //Простой последовательный список всех бинарных операторов
            List<string> operators = new List<string>();
            //Счетчик скобок. Открытая скобка +1, закрытая -1.
            //Когда счетчик равен 0, это означает что можно продолжить накапливать строки
            int bracketCount = 0;

            //Проход по каждому символу строки...
            for (int i = 0; i < input.Length; i++)
            {
                //Октрытая скобка означает, что нам больше не стоит проверять ее содержимое,
                //оставим это для будущих итерраций, пока тупо будем накапливать ее как отдельный токен
                if (input[i] == '(')
                {
                    //Если когда-то, до этого мы находили оператор, то запомним его!
                    if (lastCharWasOperator)
                    {
                        //Если он был унарным, то не будем заносить его в общий список,
                        //а сохраним, подождав пока будет токен, куда его можно запихнуть
                        if (operatorIsUnary)
                        {
                            unaryOperatorToAdd = FindOperator(currentOperator);
                            operatorIsUnary = false;
                        }
                        else operators.Add(currentOperator);

                        //Сбрасываем накопительную переменную
                        currentOperator = "";
                        //Сбрасываем указатель того, что мы читали оператор
                        lastCharWasOperator = false;
                    }

                    //Запрещаем парсинг
                    bracketCount++;
                }

                //Закрытая скобка может означать продолжение парсинга, а может и нет
                //в зависимости от того, сколько скобок было открыто
                if (input[i] == ')')
                {
                    //Следуем вышеуказанному правилу
                    bracketCount--;

                    //Если эта скобка закрывала все предыдущие,
                    //то можно продолжить парс
                    if (bracketCount == 0)
                    {
                        //На всякий случай проверим, а то шото пустые откуда-то
                        //берет токены
                        if (currentToken != "")
                        {
                            //Добавляем наш новый токен (скобку) в список
                            tokens.Add(new Token(AccurateBracketTrim(currentToken + ")"))
                            {
                                //Добавляем унарный всегда, он либо нул либо нет,
                                //какая разница?
                                UnaryOperator = unaryOperatorToAdd
                            });
                            unaryOperatorToAdd = null;
                            currentToken = "";
                        }

                        //Нечего делать дальше. Тупо скипаем этот шаг
                        continue;
                    }
                }

                //Если парс разрешен (мы находимся на нужном уровне)
                if (bracketCount == 0)
                {
                    //Если просматриваемый символ - операторный символ (пока так =\ )
                    if (OperatorCharaters.Contains(input[i]))
                    {
                        //Если просматриваемый операторный символ - символ унарых операторов (пока так =\ )
                        if (UnaryOperatorCharaters.Contains(input[i]))
                        {
                            //Читаем унарный оператор. Значит прошлый, если он был, нужно запомнить
                            if (lastCharWasOperator)
                            {
                                operators.Add(currentOperator);
                                currentOperator = "";
                            }
                            operatorIsUnary = true;
                        }

                        //Читаем оператор, неважно унарный или бинарный
                        currentOperator += input[i];

                        //Если прошлый символ не оператор, а токен не пустой
                        //Основной случай, когда, например, "4 + 2", счетчик с "4"
                        //переходит на "+". 
                        if (!lastCharWasOperator && currentToken != "")
                        {
                            //Запоминаем токен
                            tokens.Add(new Token(AccurateBracketTrim(currentToken))
                            {
                                UnaryOperator = unaryOperatorToAdd
                            });
                            unaryOperatorToAdd = null;
                            currentToken = "";
                        }

                        //Запоминаем, что предыдущий символ был операторным
                        lastCharWasOperator = true;
                    }
                    else //Если это не операторный символ
                    {
                        //Этот символ не операторный, значит раньше мог быть оператор
                        //и его нужно запомнить
                        if (currentOperator != "")
                        {
                            if (operatorIsUnary)
                            {
                                unaryOperatorToAdd = FindOperator(currentOperator);
                                operatorIsUnary = false;
                            }
                            else operators.Add(currentOperator);
                            currentOperator = "";
                        }

                        //Тут тупо накапливаем токен
                        currentToken += input[i];
                        lastCharWasOperator = false;
                    }
                }
                else
                {
                    //Когда запрещен парс, без всвяких вопросов просто накапливаем токен
                    currentToken += input[i];
                }
            }

            //Когда мы прошли все символы, в конце может остаться несохраненный токен,
            //так как мы запоминаем токены только при переходе между 
            //  операторый символ - обычный символ и
            //  обычный символ - операторый символ 
            if (currentToken != "")
            {
                tokens.Add(new Token(currentToken)
                {
                     UnaryOperator = unaryOperatorToAdd
                });
            }

            //Для каждого найденого токена, если он содержит какую-то дрянь, типо
            //скобок, вызывает этот-же метод рекурсивно
            foreach (var item in tokens)
            {
                //Проверка на дополнительные скобки, операторные символы и тд.
                if (!item.IsSimple)
                    //Вызываем этот же метод, где родительским токеном является текущий
                    item.Subtokens = CreateTokenTree(AccurateBracketTrim(item.RawValue), item);
            }

            //Теперь нам нужно установить связи между токенами
            //Например в строке 2 + 4 * 3, связи для "4" выглядят следующим образом
            //  Слева стоит оператор "+" и 2
            //  Справа стоит оператор "*" и 3
            for (int i = 0; i < tokens.Count; i++)
            {
                //Самый левый токен
                if (i == 0 && tokens.Count != 1)
                {
                    tokens[i].LeftSideOperator = null;
                    tokens[i].LeftSideToken = null;
                    tokens[i].RightSideOperator = FindOperator(operators[i]);
                    tokens[i].RightSideToken = tokens[i + 1];
                }
                //Самый правый токен
                else if(i != 0 && i == tokens.Count - 1 && tokens.Count != 1)
                {
                    tokens[i].LeftSideOperator = FindOperator(operators[i - 1]);
                    tokens[i].LeftSideToken = tokens[i - 1];
                    tokens[i].RightSideOperator = null;
                    tokens[i].RightSideToken = null;
                }
                //Токен не скраю
                else if(i != 0 && tokens.Count != 1)
                {
                    tokens[i].LeftSideOperator = FindOperator(operators[i - 1]);
                    tokens[i].LeftSideToken = tokens[i - 1];
                    tokens[i].RightSideOperator = FindOperator(operators[i]);
                    tokens[i].RightSideToken = tokens[i + 1];
                }
                //Единстенный токен в списке
                else if (i == 0 && tokens.Count == 1)
                {
                    tokens[i].LeftSideOperator = null;
                    tokens[i].LeftSideToken = null;
                    tokens[i].RightSideOperator = null;
                    tokens[i].RightSideToken = null;
                }
            }

            //Запоминаем все найденные токены как дочерные данному
            parentToken.Subtokens = tokens;
            return tokens;
        }

        private Token ParseToken(string input)
        {
            //Для удобста использования создадим корневой токен, вложив в него все остальные
            Token token = new Token(input);
            CreateTokenTree(input, token);
            return token;
        }

        /// <summary>
        /// Ищет оператор в списке, в случае ошибки выкидывает исключение
        /// </summary>
        /// <param name="name">Имя оператора</param>
        /// <returns>Найденный оператор</returns>
        private Operator FindOperator(string name)
        {
            var a = Operators.Find(p => p.OperatorString == name);
            return a ?? throw new Exception("Unknown operator");
        }

        /// <summary>
        /// Удаляет только парные скобки с краев строки
        /// </summary>
        /// <param name="input">Входящая строка для удаление скобок</param>
        /// <returns>Строка с удаленными скобками</returns>
        private string AccurateBracketTrim(string input)
        {
            //Удаляем скобки с краев, пока они там есть
            while ((input[0] == '(' && input[input.Length - 1] == ')'))
                input = input.Remove(0, 1).Remove(input.Length - 2, 1);

            return input;
        }

        /// <summary>
        /// Рекурсивный метод расчета значения токенов
        /// </summary>
        /// <param name="token">Токен, который будет расчитываться</param>
        private void Calculate(Token token)
        {
            //Если токен можно посчитать сходу то делаем это
            if (token.CanBeCalculated)
            {
                token.Calculate();
                return;
            }

            //Рекурсивно считаем все дочерные токены, вплоть до самых последних,
            //Которые обязаны считаться!
            foreach (Token subToken in token.Subtokens)
            {
                Calculate(subToken);
            }

            //Если после пересчета возможно посчитать, то считаем
            if (token.CanBeCalculated)
            {
                token.Calculate();
                return;
            }

            //Ну тут уже безысходность
            throw new Exception();
        }

        /// <summary>
        /// Расчитывает числовое значение данного выражения
        /// </summary>
        /// <returns>Числовое значение выражения</returns>
        public long Calculate()
        {
            //Пытаемся посчиать его рекурсивно.
            //Там все ссылочно кладется в токены, так что не нужно ничего возвращать
            Calculate(TokenTree);

            return TokenTree.Value;
        }

        /// <summary>
        /// Создает новый экземпляр класса <see cref="Expression"/>
        /// </summary>
        /// <param name="input">Строковое представление выражения</param>
        public Expression(string input)
        {
            //Если не были просчитаны массивы операторов, делаем это
            if (OperatorCharaters == null)
                InitGlobals();

            //Сохраняем входную строку
            Value = input;

            //Удаляем лишние пробелы, табы и пр.
            input = input.Replace(" ", "");
            input = input.Replace("\t", "");
            input = input.Replace("\r", "");

            //Строим граф (дерево) нашего выражения
            TokenTree = ParseToken(input);

            //Для выражений, например "4", "!5", состоящих из одной цифры,
            //Берем его единственный сабтокен и кладем его в корневой
            //Клонируем, бикоз не хочу тянуть лишние ссылки
            if (TokenTree.Subtokens.Count == 1 && TokenTree.Subtokens[0].Subtokens == null)
                TokenTree = (Token)TokenTree.Subtokens[0].Clone();
        }
    }
}
