using HASMLib.Core.MemoryZone;
using HASMLib.Core;
using HASMLib.SyntaxTokens;
using HASMLib.SyntaxTokens.Instructions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HASMLib
{
    public class HASMSource
    {
		public string Source { get; set; }

		public HASMSource(HASMMachine machine, FileStream fs)
        {
			byte[] bytes = new byte[fs.Length];
			fs.Read (bytes, 0, (int)fs.Length);
			Source = new string(bytes.Select(p => (char)p).ToArray());
			Machine = machine;
        }

		public HASMSource(HASMMachine machine, string source)
        {
			Source = source;
			Machine = machine;
        }

        public List<Instruction> instructions = new List<Instruction>()
        {
            new InstructionADD(0x1),
            new InstructionJMP(0x2),
            new InstructionMOV(0x3),
            new InstructionNOP(0x4)
        };

		public List<MemZoneFlashElement> ParseResult { get; private set; }
		public HASMMachine Machine { get ; set; }
		public int UsedFlash 
		{
			get
			{
				return ParseResult.Sum (p => p.FixedSize);
			}
		}

		public byte[] OutputCompiled()
		{
			List<byte> bytes = new List<byte> ();
			foreach (var item in ParseResult) {
				bytes.AddRange (item.ToBytes ());
			}
			return bytes.ToArray ();
		}

		public void OutputCompiled(string fileName)
		{
			File.WriteAllBytes (fileName, OutputCompiled ());
		}

		private Regex LabelRegex = new Regex(@"^\w{1,100}:");
		private Regex CommentRegex = new Regex(@";[\d\W\s\w]{0,}$");
		private Regex multipleSpaceRegex = new Regex(@"[ \t]{1,}");
		private Regex commaSpaceRegex = new Regex(@",[ \t]{1,}");

		private List<Tuple<string, byte>> Variables;
		private List<Tuple<string, UInt24, Constant>> _namedConsts;
		private int _constIndex;

		private void ResetGLobals()
		{
			ParseResult = null;
			Variables = new List<Tuple<string, byte>> ();
			_constIndex = 0;
			_namedConsts = new List<Tuple<string, UInt24, Constant>>();
		}

        private List<string> PrepareSource(string input)
        {
			input = multipleSpaceRegex.Replace(input, " ");
			input = commaSpaceRegex.Replace(input, ",");
            return input.Split('\n').ToList();
        }

        private void FindCommentAndLabel(ref string input, out string commentStr, out string labelStr)
        {
            commentStr = "";
            labelStr = "";

            //Поиск вхождений указателя в строке
            Match label = LabelRegex.Match(input);
            //Поиск вхождений коментария в строке
            Match comment = CommentRegex.Match(input);

            //Если в строке был найден указатель, то запомнить его,
            //удалив со строки
            if (label.Success)
            {
                input = LabelRegex.Replace(input, "");
                labelStr = label.Value.TrimEnd(':');
            }

            //Если в строке был найден коментарий, то запомнить его,
            //удалив со строки
            if (comment.Success)
            {
                input = CommentRegex.Replace(input, "");
                commentStr = comment.Value.TrimStart(';');
            }
        }

        public List<MemZoneFlashElement> GetFlashElementsNoArguents(Instruction instruction, string label, string line, int index, out ParseError error)
        {
            var result = new List<MemZoneFlashElement>();
            error = null;

            //Если инструкция не предполагает отсутсвие параметров то ошибка
            if (instruction.ParameterCount != 0)
            {
                error = new ParseError(
                    ParseErrorType.Instruction_WrongParameterCount,
                    index,
                    instruction.Name.Match(line).Index);
                return null;
            }

            //Если указан указатель
			if (!string.IsNullOrEmpty (label)) 
			{
				//Расчет нового индекса константы	
				int constIndex = ++_constIndex;
				//Заносим данную констатнту в список именных констант
				_namedConsts.Add(new Tuple<string, UInt24, Constant>(label, (UInt24)constIndex, new Constant() {Value = index}));
				//Записываем его во флеш память
				result.Add (new MemZoneFlashElementConstantUInt24 ((UInt24)index, constIndex));
			}

            //Закидываем во флеш нашу инструкцию без параметров
			result.Add(new MemZoneFlashElementInstruction(instruction, null));
            return result;
        }

		public List<MemZoneFlashElement> GetFlashElementsWithArguents(Instruction instruction,  string label, string[] stringParts, int index, out ParseError error)
        {
			var result = new List<MemZoneFlashElement>();

            //Выделяем со строки параметры в отдельный массив
            string argumentPart = stringParts[1];
            string[] arguments = argumentPart.Split(',');

            //Если кол-во параметров не совпадает с предполагаемым
            if (arguments.Length != instruction.ParameterCount)
            {
                error = new ParseError(
                    ParseErrorType.Instruction_WrongParameterCount,
                    index,
					instruction.Name.Match(stringParts[0]).Index);
                return null;
            }

			//Если указан указатель
			if (!string.IsNullOrEmpty (label)) {
				//Расчет нового индекса константы	
				int constIndex = ++_constIndex;
				//Заносим данную констатнту в список именных констант
				_namedConsts.Add(new Tuple<string, UInt24, Constant>(label, (UInt24)constIndex, new Constant() {Value = index}));
				//Записываем его во флеш память
				result.Add (new MemZoneFlashElementConstantUInt24 ((UInt24)index, constIndex));
			}

			//Индекс текущего аргумента
			int argIndex = 0;

			//Список последовательных индексов, что используются в инструкции
			// true -  константа, false - переменная
			var usedIndexes = new List<Tuple<UInt24, bool>>();

            //Проверяем типы аргументов
            foreach (var argument in arguments)
            {
				Constant constant = null;

				//Попытка пропарсить константу
				var constError = Constant.Parse(argument, out constant);

				//Грубое определние типа нашего аргумента
				var isConst = constant != null;
				var isVar = Variables.Select (p => p.Item1).Contains (argument);


				//Если допустимо и константа и переменная, то выходит неоднозначность
				if (isVar && isConst && instruction.ParameterTypes[argIndex] == InstructionParameterType.ConstantOrRegister) 
				{
					error = new ParseError (
						ParseErrorType.Syntax_AmbiguityBetweenVarAndConst,
						index,
						label.Length + 2 + stringParts.Take (argIndex).Sum (p => p.Length));
					return null;
				}

				//Если мы определили, что это может быть как переменная, так и коснтанта, 
				//то смотрим что от нас хочет инструкция
				if (isVar && isConst)
				{
					//Если необходима коснтанта
					if (instruction.ParameterTypes [argIndex] == InstructionParameterType.Constant) 
					{
						//Запоминаем индекс константы
						usedIndexes.Add (new Tuple<UInt24, bool> ((UInt24)(++_constIndex), true));
						//Записываем во флеш константу
						result.Add(constant.ToFlashElement(_constIndex));
					};

					//Если необходима переменная
					if (instruction.ParameterTypes [argIndex] == InstructionParameterType.Register) 
					{
						//Получаем индекс переменной со списка переменных
						int varIndex = Variables.Select(p => p.Item1).ToList().IndexOf(argument);
						//Запоминаем индекс переменной
						usedIndexes.Add (new Tuple<UInt24, bool> ((UInt24)varIndex, false));
						//Записываем переменную во флеш
						result.Add(new MemZoneFlashElementVariable(
							(UInt24)varIndex,
							Variables [varIndex].Item2
						));
					}
				}

				//Если это однозначно константа, не переменная
				if (isConst) 
				{

					//А ожидалась константа, то ошибка
					if (instruction.ParameterTypes [argIndex] == InstructionParameterType.Register) {
						error = new ParseError (
							ParseErrorType.Syntax_ExpectedVar,
							index,
							label.Length + 2 + stringParts.Take(argIndex).Sum (p => p.Length));
						return null;						
					}

					//Запоминаем индекс константы
					usedIndexes.Add (new Tuple<UInt24, bool> ((UInt24)(++_constIndex), true));
					//Заносим константу во флеш
					result.Add (constant.ToFlashElement (_constIndex));
				}
				else //Если это не константа... 
				{
					//Если это однозначно переменная...
					if (isVar) 
					{
						
						//А ожидалась константа, то ошибка
						if (instruction.ParameterTypes [argIndex] == InstructionParameterType.Constant) {
							error = new ParseError (
								ParseErrorType.Syntax_ExpectedСonst,
								index,
								label.Length + 2 + stringParts.Take(argIndex).Sum (p => p.Length));
							return null;						
						}

						//Получаем индекс переменной со списка переменных 
						int varIndex = Variables.Select(p => p.Item1).ToList().IndexOf(argument);
						//Запоминаем индекс переменной
						usedIndexes.Add (new Tuple<UInt24, bool> ((UInt24)varIndex, false));
						//Записываем переменную во флеш
						result.Add(new MemZoneFlashElementVariable (
							(UInt24)varIndex,
							Variables [varIndex].Item2
						));
					}
					else //Если это не переменная, а просили константу
					if (instruction.ParameterTypes [argIndex] == InstructionParameterType.ConstantOrRegister ||
					    instruction.ParameterTypes [argIndex] == InstructionParameterType.Constant) 
					{
						//То, возможно, это именная константа...
						if(_namedConsts.Select(p => p.Item1).Contains(argument))
						{
							//Получения индекса константы со списка
							int constantIndex = _namedConsts.Select (p => p.Item1).ToList ().IndexOf (argument);
							//Запоминания индекса
							usedIndexes.Add (new Tuple<UInt24, bool> (_namedConsts[constantIndex].Item2, true));
							//Запись константы во флеш
							result.Add (_namedConsts[constantIndex].Item3.ToFlashElement (_namedConsts[constantIndex].Item2));
						}
					}
					else //Если удалось частично пропарсить константу, но были переполнения и тд...
					if (constError.Type == ParseErrorType.Constant_BaseOverflow || constError.Type == ParseErrorType.Constant_TooLong)
					{
						//Вернуть новую ошибку с типо старой
						error = new ParseError(
							constError.Type, 
							index,
							label.Length + 2 + stringParts.Take(argIndex).Sum (p => p.Length));
						return null;
					} 
					else //Если ничего не известно, то вернем что неивесное имя переменной
					{
						error = new ParseError (
							ParseErrorType.Syntax_UnknownVariableName,
							index,
							label.Length + 2 + stringParts.Take (argIndex).Sum (p => p.Length));
						return null;
					}
				}
				argIndex++;
            }

			result.Add(new MemZoneFlashElementInstruction(instruction, usedIndexes));
			error = null;
			return result;
        }

		private void SetupRegisters(HASMMachine machine)
		{
			Variables.AddRange(machine.GetRegisterNames ().Select(p => new Tuple<string, byte>(p, MemZoneFlashElementVariable.VariableType_Single)));
		}

		public ParseError Parse()
		{
			ParseError err;
			ParseResult = Parse (Machine, out err);

			if (err != null)
				return err;

			if (ParseResult == null)
				return new ParseError (ParseErrorType.Other_UnknownError, 0);

			return null;
		}

        private List<MemZoneFlashElement> Parse(HASMMachine machine, out ParseError parseError)
        {
			// OPT        REQ       OPT            OPT
			//label: instruction a1, a2, a3 ... ; comment
			
			//Examples
			//       instruction a1, a2, a3 ... ; comment
			//label: instruction                ; comment
			//etc

			try 
			{
				//Обнуляем глобальные переменые
				ResetGLobals ();
				//Заносим регистры в список переменных
				SetupRegisters (machine);

	            List<MemZoneFlashElement> result = new List<MemZoneFlashElement>();

	            //Очистка строк от лишних пробелов, табов
	            List<string> lines = PrepareSource(Source);

	            for (var index = 0; index < lines.Count; index++)
	            {
	                string line = lines[index];
					string comment = "";
					string label = "";

	                FindCommentAndLabel(ref line, out comment, out label);

	                //Дополнительно удаляем лишние символы со строки
	                line = line.Trim(' ', '\t');

					//Переменная для отслеживания успеха поиска инструкции
	                bool found = false;

	                foreach (var instruction in instructions)
	                {
	                    //Если регулярка инструкции присутсвует в строке
	                    //Все обязаны иметь в себе "^" (начало строки), чтобы
	                    //избегать некоректный поиск в строке!
	                    if (instruction.Name.Match(line).Success)
	                    {
	                        //Делим строку спейсом, молясь о том, что это сработает!
	                        string[] stringParts = line.Split(' ');

	                        //Если нету аргументов
	                        if (stringParts.Length == 1)
	                        {
								ParseError error;
								//Попытка пропарсить строку
	                            var flashElements = GetFlashElementsNoArguents(instruction, label, line, index, out error);
	                            if(flashElements == null)
	                            {
	                                parseError = error;
	                                return null;
	                            }
								result.AddRange (flashElements);
	                        }
	                        else
	                        {
								ParseError error;
								//Попытка пропарсить строку
								var flashElements = GetFlashElementsWithArguents(instruction, label, stringParts, index, out error);
	                            if (flashElements == null)
	                            {
	                                parseError = error;
	                                return null;
	                            }
								result.AddRange (flashElements);
	                        }
	                        found = true;
	                        break;
	                    }

	                }

	                //Если не было найдено то ошибка
	                if (!found)
	                {
	                    parseError = new ParseError(
	                        ParseErrorType.Instruction_UnknownInstruction,
	                        index,
	                        label.Length + 2);
	                    return null;
	                }
	            }

				//Если размер программы превышает максимально допустимый для этой машины
	            int totalFlashSize = result.Sum(p => p.FixedSize);
	            if(totalFlashSize > machine.Flash)
	            {
	                parseError = new ParseError(ParseErrorType.Other_OutOfFlash, 0, 0);
	                return null;
	            }

	            parseError = null;
				return result;
			} catch 
			{
				parseError = null;
				return null;
			}
		}
    }
}