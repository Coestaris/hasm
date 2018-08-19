using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Parser.SyntaxTokens.Constants;
using HASMLib.Parser.SyntaxTokens.Expressions.Exceptions;
using HASMLib.Parser.SyntaxTokens.Preprocessor.Directives;
using HASMLib.Runtime;
using HASMLib.Runtime.Structures;
using System;
using System.Collections.Generic;
using System.Linq;

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
            if (OperatorCharaters != null) return;

            OperatorCharaters = new List<char>();
            UnaryOperatorCharaters = new List<char>();
            BinaryOperatorCharaters = new List<char>();
            foreach (Operator op in Operators)
            {
                if (op.Ignore) continue;

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
        /// Функции, которые допустимо использовать в выражениях
        /// </summary>
        public static readonly List<Function> Functions = new List<Function>()
        {
            new Function(8, "strof", (a) =>
            {
                if(a.Type == TypeReferenceType.Integer)
                    return new Constant(new Core.BaseTypes.Array(a.IntValue.Value.ToString()));

                if(a.ArrayValue.IsString)
                    return new Constant(a.ArrayValue);

                return new Constant(new Core.BaseTypes.Array(a.ArrayValue.ToString()));

            }),
            new Function(1, "double", (a) => new Constant(a.IntValue * (Integer)2)),
            new Function(2, "exp2", (a) => new Constant(a.IntValue * a.IntValue)),
            new Function(8, "log2", (a) => new Constant((Integer)(long)Math.Log((long)a.IntValue, 2))),
            new Function(2, "abs", (a) => new Constant(new Integer((ulong)Math.Abs((long)a.IntValue), a.IntValue.Type))),
            new Function(1, "defined", (a) => a), //TODO!
            new Function(1, "strlen", (a) => new Constant(BaseIntegerType.CommonType)),
        };

        /// <summary>
        /// Количество условных шагов, за которое было выполнено вычисление результата
        /// </summary>
        public int Steps { get; set; }

        /// <summary>
        /// Список всех поддерживаемых операторов.
        /// Добавление своих не приветсвуется (они взяты со спецификации ANSI C), но это возможно
        /// </summary>
        public static readonly List<Operator> Operators = new List<Operator>()
        {
            //Unary 
            new Operator(1, "!", (a) => new Constant(new Integer(a.AsBool() ? 0U : 1U, a.IntValue.Type))),
            new Operator(1, "~", (a) => new Constant(~ a.IntValue)),
            new Operator(1, "-", (a) => new Constant(~ a.IntValue), true),

            //Binnary
            new Operator(2, 13, "*",  (a, b) => new Constant(a.IntValue * b.IntValue)),
            new Operator(2, 13, "/",  (a, b) => new Constant(a.IntValue / b.IntValue)),

            new Operator(2, 12, "%",  (a, b) => new Constant(a.IntValue % b.IntValue)),

            new Operator(2, 11, "+",  (a, b) =>
            {
                if(a.Type == TypeReferenceType.Array || b.Type == TypeReferenceType.Array)
                {
                    string result = "";
                    if(a.Type == TypeReferenceType.Array)
                        result += a.ArrayValue.AsString();
                    else result += a.IntValue;

                    if(b.Type == TypeReferenceType.Array)
                        result += b.ArrayValue.AsString();
                    else result += b.IntValue;

                    return new Constant(new Core.BaseTypes.Array(result));
                }
                else return new Constant(a.IntValue + b.IntValue);
            }),

            new Operator(2, 11, "-",  (a, b) => new Constant(a.IntValue - b.IntValue)),

            new Operator(2, 10, "<<", (a, b) => new Constant(a.IntValue << (int)b.IntValue)),
            new Operator(2, 10, ">>", (a, b) => new Constant(a.IntValue >> (int)b.IntValue)),

            new Operator(1, 9, "<",   (a, b) => new Constant(new Integer(a.IntValue < b.IntValue ? 1U : 0U, Integer.SelectType(a.IntValue, b.IntValue)))),
            new Operator(1, 9, "<=",  (a, b) => new Constant(new Integer(a.IntValue <= b.IntValue ? 1U : 0U, Integer.SelectType(a.IntValue, b.IntValue)))),
            new Operator(1, 9, ">",   (a, b) => new Constant(new Integer(a.IntValue > b.IntValue ? 1U : 0U, Integer.SelectType(a.IntValue, b.IntValue)))),
            new Operator(1, 9, ">=",  (a, b) => new Constant(new Integer(a.IntValue >= b.IntValue ? 1U : 0U, Integer.SelectType(a.IntValue, b.IntValue)))),

            new Operator(1, 8, "!=",  (a, b) => new Constant(new Integer(a.IntValue != b.IntValue ? 1U : 0U, Integer.SelectType(a.IntValue, b.IntValue)))),
            new Operator(1, 8, "==",  (a, b) => new Constant(new Integer(a.IntValue == b.IntValue ? 1U : 0U, Integer.SelectType(a.IntValue, b.IntValue)))),

            new Operator(2, 7, "&",  (a, b) => new Constant(a.IntValue & b.IntValue)),
            new Operator(2, 6, "^",  (a, b) => new Constant(a.IntValue ^ b.IntValue)),
            new Operator(2, 5, "|",  (a, b) => new Constant(a.IntValue | b.IntValue)),
            new Operator(2, 4, "&&", (a, b) => new Constant(new Integer(a.AsBool() && b.AsBool() ? 1U : 0U, Integer.SelectType(a.IntValue, b.IntValue)))),
            new Operator(2, 3, "||", (a, b) => new Constant(new Integer(a.AsBool() || b.AsBool() ? 1U : 0U, Integer.SelectType(a.IntValue, b.IntValue)))),

            new Operator(1, 2, "?",  (a, b) =>
            {
                Operator.ConditionalOperatorResult = a.AsBool();
                Operator.ConditionalSecondOperand = b;
                return a;
            }),
            new Operator(0, 1, ":",  (a, b) => Operator.ConditionalOperatorResult ? Operator.ConditionalSecondOperand : b),
        };


        /// <summary>
        /// Строковое представление выражения
        /// </summary>
        public string Value;

        /// <summary>
        /// Построенный (кешированный) граф выражений
        /// </summary>
        internal Token TokenTree;

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

            bool skipSpaces = true;
            //Накопительная строка просматриваемого токена
            string currentToken = "";
            //Накопительная строка просматриваемого оператора
            string currentOperator = "";
            //Унарный оператор, который будет добавлен к следующему токену
            Operator unaryOperatorToAdd = null;
            //Унарная функция, которая будет добавлена к следующему токену
            Function unaryFunctionToAdd = null;

            //Указатель на то, что предыдущий символ был токеном
            bool lastCharWasOperator = false;
            //Указатель на то, что текущий накопительный оператор - унарный
            bool operatorIsUnary = false;

            //Первая открытая скобка на текущем уровне. Неожиданно!?
            bool firstOpenedBracketInCurrentLevel = true;

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
                if (input[i] == ' ' && skipSpaces)
                    continue;

                if (input[i] == '"') skipSpaces = !skipSpaces;

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
                            unaryOperatorToAdd = FindUnaryOperator(currentOperator, false);
                            operatorIsUnary = false;
                        }
                        else
                        {
                            //Если это первый токен в списке, то логично, что оператор - унарный
                            if (tokens.Count == 0 && currentOperator == "-")
                            {
                                unaryOperatorToAdd = FindUnaryOperator(currentOperator, true);
                                operatorIsUnary = false;
                            }
                            else operators.Add(currentOperator);
                        }

                        //Сбрасываем накопительную переменную
                        currentOperator = "";
                        //Сбрасываем указатель того, что мы читали оператор
                        lastCharWasOperator = false;
                    }
                    //Если прошлый символ был не оператором, значит что-то там таки было
                    //Возмонжно функция
                    else if (bracketCount == 0 && currentToken != "" && firstOpenedBracketInCurrentLevel)
                    {
                        firstOpenedBracketInCurrentLevel = false;

                        //Низя юзать дефайнед, если того не разрешено
                        if (currentToken == "defined")
                        {
                            if (!PreprocessorIf.AllowDefinedFunction)
                                throw new NotAllowedDefinedFunctionException();
                        }

                        if (Functions.Exists(p => p.FunctionString == currentToken))
                        {
                            //Если это таки функция
                            //Запоминаем ее, чтобы добавить позже
                            unaryFunctionToAdd = Functions.Find(p => p.FunctionString == currentToken);
                        }
                        //Иначе тупо ошибка синтаксиса
                        else throw new UnknownFunctionException(currentToken);
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
                        firstOpenedBracketInCurrentLevel = true;

                        //На всякий случай проверим, а то шото пустые откуда-то
                        //берет токены
                        if (currentToken != "")
                        {
                            //Если задана функция, удаляем ее с начала строки, и удалем скобки
                            if (unaryFunctionToAdd != null)
                                currentToken = AccurateBracketTrim(currentToken.Remove(0, unaryFunctionToAdd.FunctionString.Length));


                            //Добавляем наш новый токен (скобку) в список
                            tokens.Add(new Token(AccurateBracketTrim(currentToken + ")"))
                            {
                                //Добавляем унарный всегда, он либо нул либо нет,
                                //какая разница?
                                UnaryOperator = unaryOperatorToAdd,
                                UnaryFunction = unaryFunctionToAdd,
                            });
                            unaryOperatorToAdd = null;
                            unaryFunctionToAdd = null;

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
                            //Если задана функция, удаляем ее с начала строки, и удалем скобки
                            if (unaryFunctionToAdd != null)
                                currentToken = AccurateBracketTrim(currentToken.Remove(0, unaryFunctionToAdd.FunctionString.Length));

                            //Запоминаем токен
                            tokens.Add(new Token(AccurateBracketTrim(currentToken))
                            {
                                UnaryOperator = unaryOperatorToAdd,
                                UnaryFunction = unaryFunctionToAdd,

                            });
                            unaryOperatorToAdd = null;
                            unaryFunctionToAdd = null;
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
                                unaryOperatorToAdd = FindUnaryOperator(currentOperator, false);
                                operatorIsUnary = false;
                            }
                            else
                            {
                                //Если это первый токен в списке, то логично, что оператор - унарный
                                if (tokens.Count == 0 && currentOperator == "-")
                                {
                                    unaryOperatorToAdd = FindUnaryOperator(currentOperator, true);
                                    operatorIsUnary = false;
                                }
                                else operators.Add(currentOperator);
                            }
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
                //Если задана функция, удаляем ее с начала строки, и удалем скобки
                if (unaryFunctionToAdd != null)
                    currentToken = AccurateBracketTrim(currentToken.Remove(0, unaryFunctionToAdd.FunctionString.Length));

                tokens.Add(new Token(currentToken)
                {
                    UnaryOperator = unaryOperatorToAdd,
                    UnaryFunction = unaryFunctionToAdd,

                });
                unaryOperatorToAdd = null;
                unaryFunctionToAdd = null;
            }

            if (tokens.Count != operators.Count + 1 || currentOperator != "")
                throw new WrongOperatorCountException();

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
                    tokens[i].RightSideOperator = FindOperator(operators[i], false);
                    tokens[i].RightSideToken = tokens[i + 1];
                }
                //Самый правый токен
                else if (i != 0 && i == tokens.Count - 1 && tokens.Count != 1)
                {
                    tokens[i].LeftSideOperator = FindOperator(operators[i - 1], false);
                    tokens[i].LeftSideToken = tokens[i - 1];
                    tokens[i].RightSideOperator = null;
                    tokens[i].RightSideToken = null;
                }
                //Токен не скраю
                else if (i != 0 && tokens.Count != 1)
                {
                    tokens[i].LeftSideOperator = FindOperator(operators[i - 1], false);
                    tokens[i].LeftSideToken = tokens[i - 1];
                    tokens[i].RightSideOperator = FindOperator(operators[i], false);
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

            /*if(parentToken.UnaryFunction == null && parentToken.UnaryOperator == null && 
                parentToken.Subtokens != null && parentToken.Subtokens.Count == 1)
            {
                var token = parentToken.Subtokens[0];

                parentToken.UnaryFunction = token.UnaryFunction;
                parentToken.UnaryOperator = token.UnaryOperator;
                parentToken.Subtokens = token.Subtokens;

            }*/

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
        /// Ищет унарный оператор в списке, в случае ошибки выкидывает исключение
        /// </summary>
        /// <param name="name">Имя оператора</param>
        /// <param name="ignoreIngoring">Указывает, стоит ли при поиске игнорировать ингнорирование</param>
        /// <returns>Найденный оператор</returns>
        private Operator FindUnaryOperator(string name, bool ignoreIngoring)
        {
            var a = Operators.Find(p => p.IsUnary && (ignoreIngoring || !p.Ignore) && p.OperatorString == name);
			if(a == null) throw new UnknownOperatorException(name);;
			return a;
        }

        /// <summary>
        /// Ищет оператор в списке, в случае ошибки выкидывает исключение
        /// </summary>
        /// <param name="name">Имя оператора</param>
        /// <returns>Найденный оператор</returns>
        private Operator FindOperator(string name, bool ignoreIngoring)
        {
            var a = Operators.Find(p => (ignoreIngoring || !p.Ignore) && p.OperatorString == name);
			if(a == null) throw new UnknownOperatorException(name);;
			return a;
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
        private void Calculate(RuntimeDataPackage package, Token token, bool isProbe)
        {
            //Если токен можно посчитать сходу то делаем это
            if (token.CanBeCalculated(package))
            {
                token.Calculate(package);
                Steps += token.Steps;
                return;
            }

            if (token.Subtokens == null)
                if (!isProbe) throw new Exception();
                else return;

            //Рекурсивно считаем все дочерные токены, вплоть до самых последних,
            //Которые обязаны считаться!
            foreach (Token subToken in token.Subtokens)
            {
                Calculate(package, subToken, isProbe);
                Steps += subToken.Steps;
            }

            //Если после пересчета возможно посчитать, то считаем
            if (token.CanBeCalculated(package))
            {
                token.Calculate(package);
                Steps += token.Steps;
                return;
            }

            //Ну тут уже безысходность
            if (!isProbe)
                throw new Exception();
        }

        /// <summary>
        /// Очистка расчитаных значений тех токенов, которые зависят от переменных.
        /// Это означает что в следующий раз токены, независящие от Х не будут считаться
        /// </summary>
        /// <param name="token"></param>
        private void ClearCache(Token token)
        {
            //if (token.RawValue.Contains("x")) //TODO:!
            {
                token.ClearValue();
                if (token.Subtokens != null) foreach (Token subToken in token.Subtokens)
                    {
                        ClearCache(subToken);
                    }
            }
        }

        /// <summary>
        /// Расчитывает числовое значение данного выражения
        /// </summary>
        /// <param name="clearCache">Стоит ли чистить числовыые значения токенов независящих от переменных</param>
        /// <param name="isProbe">Указывает на то что данный запуск пробный, и не должен гарантировать получение результата</param>
        /// <returns>Числовое значение выражения</returns>
        public Constant Calculate(RuntimeDataPackage package = null, bool clearCache = false, bool isProbe = false)
        {
            Steps = 0;

            //Пытаемся посчиать его рекурсивно.
            //Там все ссылочно кладется в токены, так что не нужно ничего возвращать
            Calculate(package, TokenTree, isProbe);

            var value = TokenTree.Value;

            //Чистим расчитаные значения для тех токенов, которые зависят от переменных.
            if (clearCache) ClearCache(TokenTree);

            return value;
        }

        /// <summary>
        /// Расчитывает числовое значение данного выражения
        /// </summary>
        /// <returns>Числовое значение выражения</returns>
        public Constant Calculate(RuntimeDataPackage package = null)
        {
            //Пытаемся посчиать его рекурсивно.
            //Там все ссылочно кладется в токены, так что не нужно ничего возвращать
            Calculate(package, TokenTree, false);

            return TokenTree.Value;
        }

        /// <summary>
        /// Создает новый экземпляр класса <see cref="Expression"/>
        /// </summary>
        /// <param name="input">Строковое представление выражения</param>
        internal Expression(string input)
        {
            //Сохраняем входную строку
            Value = input;

            //Строим граф (дерево) нашего выражения
            TokenTree = ParseToken(input);

            //Для выражений, например "4", "!5", состоящих из одной цифры,
            //Берем его единственный сабтокен и кладем его в корневой
            //Клонируем, бикоз не хочу тянуть лишние ссылки
            if (TokenTree.Subtokens.Count == 1)
                TokenTree = (Token)TokenTree.Subtokens[0].Clone();
        }

        /// <summary>
        /// Строковое представление данного выражения
        /// </summary>
        public override string ToString()
        {
            return $"Expression: [{TokenTree.ToString()}]";
        }

        /// <summary>
        /// Рекурсивно проходит по каждому токену, встречая незнакомое имя,
        /// вызывает заданную функцию, запрашивая ссылку на объект. Расчитывает все токены, которые
        /// не зависят от переменных и констант.
        /// </summary>
        /// <param name="ResolveNameFunc">Фукция, которая будет вызываться для каждой неизвестной константы или переменной, встреченной в выражении</param>
        /// <param name="RegisterNewConstant">Необязательная функция, которая будет вызываться, при встече чисел, и в случае ее заданности будет заменять числа на ссылки</param>
        public static void Precompile(Token token, Func<Token, ObjectReference> ResolveNameFunc, Func<Constant, ObjectReference> RegisterNewConstant = null)
        {
            if (token.IsSimple) token.ResolveName(ResolveNameFunc, RegisterNewConstant);
            if (token.Subtokens != null) foreach (Token subToken in token.Subtokens)
                {
                    Precompile(subToken, ResolveNameFunc, RegisterNewConstant);
                }
        }

        /// <summary>
        /// Парсит строку, применяя к результату <see cref="Precompile"/>
        /// </summary>
        /// <param name="input">Входящее строковое представление выражения</param>
        /// <param name="result">Результат разбора строки</param>
        /// <param name="ResolveNameFunc">Фукция, которая будет вызываться для каждой неизвестной константы или переменной, встреченной в выражении</param>
        /// <param name="RegisterNewConstant">Необязательная функция, которая будет вызываться, при встече чисел, и в случае ее заданности будет заменять числа на ссылки</param>
        /// <returns></returns>
        public static ParseError Parse(string input, out Expression result, Func<Token, ObjectReference> ResolveNameFunc, Func<Constant, ObjectReference> RegisterNewConstant = null)
        {
            var error = Parse(input, out result);
            if (error == null)
            {
                Precompile(result.TokenTree, ResolveNameFunc, RegisterNewConstant);
                return null;
            }
            else return error;
        }

        /// <summary>
        /// Преобразовует строку в выражение
        /// </summary>
        /// <param name="input">Входящее строковое представление выражения</param>
        /// <param name="result">Результат разбора строки</param>
        public static ParseError Parse(string input, out Expression result)
        {
            string rawInput = input;

            result = null;

            if (input.Count(p => p == '(') != input.Count(p => p == ')'))
                return new ParseError(ParseErrorType.Syntax_Expression_UnclosedBracket);

            //Если не были просчитаны массивы операторов, делаем это
            if (OperatorCharaters == null)
                InitGlobals();

            //Удаляем лишние пробелы, табы и пр.
            //input = input.Replace(" ", "");
            input = input.Replace("\t", "");
            input = input.Replace("\r", "");

            try
            {
                result = new Expression(input);
            }
            catch (UnknownFunctionException e)
            {
                if (e.FuncName != null)
                    return new ParseError(
                        ParseErrorType.Syntax_Expression_UnknownFunction,
                        rawInput.IndexOf(e.FuncName));
                else return new ParseError(ParseErrorType.Syntax_Expression_UnknownFunction);
            }
            catch (UnknownOperatorException e)
            {
                if (e.OperatorName != null)
                    return new ParseError(
                        ParseErrorType.Syntax_Expression_UnknownOperator,
                        rawInput.IndexOf(e.OperatorName));
                else return new ParseError(ParseErrorType.Syntax_Expression_UnknownOperator);
            }
            catch (WrongOperatorCountException)
            {
                return new ParseError(ParseErrorType.Syntax_Expression_WrongOperatorCount);
            }
            catch (NotAllowedDefinedFunctionException)
            {
                return new ParseError(ParseErrorType.Syntax_Expression_NotAllowToUseDefinedFunction);
            }
            catch (StackOverflowException)
            {
                return new ParseError(ParseErrorType.Syntax_Expression_CantParse);
            }
            catch (WrongTokenException)
            {
                return new ParseError(ParseErrorType.Syntax_Expression_UnknownToken);
            }
            catch(IndexOutOfRangeException)
            {
                return new ParseError(ParseErrorType.Syntax_Expression_CantParse);
            }

            return null;
        }
    }
}
