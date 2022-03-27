////////////////////////////////////////////
//Лабораторная работа №1 по дисциплине ЛОИС
//Выполнена студентами группы 921701
//Пилипейко Валентин Игоревич
//Драгун Владмир Андреевич
//24.02.2022 - Написание кода Лабораторной работы №1
//24.03.2022 - Написание кода Лабораторной работы №2
//Использованные источники: 
//1) Справочно проверяющая семантическая система по дисциплине ЛОИС
//   Сслыка: http://scnedu.sourceforge.net/variety/_/index.html?variety=examinator.PSMIS.E.1.json

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SKNF_Checker
{
    class Program
    {
        private static bool IsNumericUnaryOperation(string line)
        {
            if (line[0] == '!')
            {
                line = line[1..];
            }

            if (line.Length == 1 && char.IsDigit(line[0]))
            {
                return true;
            }

            return false;
        }

        // Метод разработал Пилипейко Валентин при поддержке Владимира Драгуна
        private static bool IsUnaryOperation(string line)
        {
            if (line[0] == '!')
            {
                line = line[1..];
            }

            if (line.Length == 1 && char.IsLetter(line[0]))
            {
                return true;
            }

            return false;

            // if (!char.IsLetter(line[0]))
            // {
            //     return false;
            // }
            // else
            // {
            //     line = line[1..];
            //     if (string.IsNullOrEmpty(line))
            //     {
            //         return true;
            //     }
            // }
            //
            // if (!char.IsDigit(line[0]) || line[0] == '0')
            // {
            //     return false;
            // }
            // else
            // {
            //     line = line[1..];
            // }
            //
            // for (int i = 0; i < line.Length; ++i)
            // {
            //     if (!char.IsDigit(line[i]))
            //     {
            //         return false;
            //     }
            // }
            //
            // return true;
        }

        // Метод разработал Пилипейко Валентин при поддержке Владимира Драгуна
        private static IList<string> BinaryOperationsString => new List<string>()
        {
            //User has to input just a "->"
            "->",
            "~",
            "/" + "\\",
            "\\/",
        };

        // Метод разработал Пилипейко Валентин при поддержке Владимира Драгуна
        private static void EnsureBinaryAndUnaryOperations(string line)
        {
            int leftBracketsCounter = 0;
            int operationsCounter = 0;

            for (int i = 0; i < line.Length - 1; ++i)
            {
                if (line[i] == '(')
                {
                    ++leftBracketsCounter;
                }
                else if (line[i] == ')')
                {
                    --leftBracketsCounter;
                }

                if (leftBracketsCounter == 1 && BinaryOperationsString.Contains((line[i].ToString() + line[i + 1])))
                {
                    ++operationsCounter;
                }
            }

            if (operationsCounter > 1)
            {
                throw new Exception("Incorrect formula. Contains non-binary or non-unary operation");
            }
        }

        // Метод разработал Пилипейко Валентин при поддержке Владимира Драгуна
        private static string GetLeftSubFormula(string line)
        {
            int leftBracketCounter = 0;

            for (int i = 0; i < line.Length - 1; ++i)
            {
                if (line[i] == '(')
                {
                    ++leftBracketCounter;
                }
                else if (line[i] == ')')
                {
                    --leftBracketCounter;
                }

                if ((BinaryOperationsString.Contains(line[i].ToString()) ||
                     BinaryOperationsString.Contains(line[i].ToString() + line[i + 1])) && leftBracketCounter == 1)
                {
                    return line[1..i];
                }
            }

            throw new Exception("Incorrect formula. Couldn't get left part of formula " + line);
        }

        // Метод разработал Пилипейко Валентин при поддержке Владимира Драгуна
        private static string GetRightSubFormula(string line)
        {
            int rightBracketCounter = 0;

            for (int i = line.Length - 1; i > 0; --i)
            {
                if (line[i] == ')')
                {
                    ++rightBracketCounter;
                }
                else if (line[i] == '(')
                {
                    --rightBracketCounter;
                }

                if (BinaryOperationsString.Contains(line[i - 1].ToString() + line[i]) && rightBracketCounter == 1)
                {
                    return line.Substring(++i, line.Length - i - 1);
                }

                if (BinaryOperationsString.Contains(line[i - 1].ToString()) && rightBracketCounter == 1)
                {
                    return line.Substring(i++, line.Length - i);
                }
            }

            throw new Exception("Incorrect formula. Couldn't get right part of formula " + line);
        }

        // Метод разработал Пилипейко Валентин при поддержке Владимира Драгуна
        private static bool EnsureFormulaCorrent(string formula)
        {
            if (IsUnaryOperation(formula.TryRemoveBrackets()))
            {
                return true;
            }

            EnsureBinaryAndUnaryOperations(formula);

            string leftSubFormula = GetLeftSubFormula(formula);
            string rightSubFormula = GetRightSubFormula(formula);

            return (IsUnaryOperation(leftSubFormula.TryRemoveBrackets()) || IsBinaryOperation(leftSubFormula)) &&
                   (IsUnaryOperation(rightSubFormula.TryRemoveBrackets()) || IsBinaryOperation(rightSubFormula));
        }

        // Метод разработал Пилипейко Валентин при поддержке Владимира Драгуна
        private static bool IsBinaryOperation(string line)
        {
            return EnsureFormulaCorrent(line);
        }

        // Метод разработал Пилипейко Валентин при поддержке Владимира Драгуна
        private static void TryCheckFormula(string formula)
        {
            try
            {
                if (IsUnaryOperation(formula.TryRemoveBrackets()) && !IsUnaryOperation(formula))
                {
                    throw new Exception("Incorrect. Brackets are not required");
                }


                EnsureFormulaCorrent(formula);
                EnsureSknfCorrect(formula);
                Console.WriteLine(formula + "  " + "Correct");
            }
            catch (Exception e)
            {
                Console.WriteLine(formula + "  " + e.Message);
            }
        }

        // Метод разработал Владимир Драгун при поддержке Пилипейко Валентина
        private static void EnsureSknfCorrect(string line)
        {
            var disjunctionElements = GetDisjunctionsElements(line);

            EnsureEachDisjunctionSameSize(disjunctionElements);

            EnsureNoSameDisjunctions(disjunctionElements);

            EnsureDisjunctionsWithSameOperandsIgnoreNegativation(disjunctionElements);
        }

        // Метод разработал Владимир Драгун при поддержке Пилипейко Валентина
        private static void EnsureEachDisjunctionSameSize(List<List<string>> disjunctionElements)
        {
            for (int i = 0; i < disjunctionElements.Count - 1; ++i)
            {
                for (int j = 0; j < disjunctionElements.Count; ++j)
                {
                    if (disjunctionElements[i].Count != disjunctionElements[j].Count)
                    {
                        throw new Exception("Incorrect SKNF. Not same number of operands.");
                    }
                }
            }
        }

        // Метод разработал Владимир Драгун при поддержке Пилипейко Валентина
        private static void EnsureDisjunctionsWithSameOperandsIgnoreNegativation(List<List<string>> disjunctionElements)
        {
            for (int i = 0; i < disjunctionElements.Count - 1; ++i)
            {
                for (int j = i + 1; j < disjunctionElements.Count; ++j)
                {
                    var first = disjunctionElements[i];
                    var second = disjunctionElements[j];

                    for (int k = 0; k < first.Count; ++k)
                    {
                        first[k] = first[k].Replace("!", "");
                    }

                    for (int k = 0; k < second.Count; ++k)
                    {
                        second[k] = second[k].Replace("!", "");
                    }

                    if (!first.OrderBy(e => e).SequenceEqual(second.OrderBy(e => e)))
                    {
                        throw new Exception("Incorrect SKNF. Two same disjunctions");
                    }
                }
            }
        }

        // Метод разработал Владимир Драгун при поддержке Пилипейко Валентина
        private static void EnsureNoSameDisjunctions(List<List<string>> disjunctionElements)
        {
            for (int i = 0; i < disjunctionElements.Count - 1; ++i)
            {
                for (int j = i + 1; j < disjunctionElements.Count; ++j)
                {
                    var first = disjunctionElements[i];
                    var second = disjunctionElements[j];

                    if (first.OrderBy(e => e).SequenceEqual(second.OrderBy(e => e)))
                    {
                        throw new Exception("Incorrect SKNF. Two same disjunctions");
                    }
                }
            }
        }

        // Метод разработал Владимир Драгун при поддержке Пилипейко Валентина
        private static List<List<string>> GetDisjunctionsElements(string line)
        {
            var disjunctions = line.Split("/\\");

            List<List<string>> list = new List<List<string>>(disjunctions.Length);

            for (int j = 0; j < disjunctions.Length; ++j)
            {
                list.Add(new List<string>());
            }

            for (int j = 0; j < disjunctions.Length; ++j)
            {
                var formula = disjunctions[j];

                for (int i = 0; i < formula.Length; ++i)
                {
                    if (char.IsLetter(formula[i]) || formula[i] == '!')
                    {
                        int startPos = i;
                        while (true)
                        {
                            ++i;
                            //TODO: add to same if 
                            if (i > formula.Length - 1)
                            {
                                list[j].Add(formula[startPos..i]);
                                break;
                            }

                            if (!char.IsLetter(formula[i]) && !char.IsDigit(formula[i]))
                            {
                                list[j].Add(formula[startPos..i]);
                                break;
                            }
                        }
                    }
                }
            }

            return list;
        }




        // Метод разработал Пилипейко Валентин
        public static string GenerateSknf(string formula)
        {
            try
            {
                EnsureFormulaCorrent(formula);

                List<string> operands = new List<string>();

                GetFormulaOperands(formula, ref operands);

                //Only unique operands 
                operands = operands.Select(x => x.RemoveNegative())
                    .GroupBy(x => x)
                    .Select(x => x.Key)
                    .ToList();

                List<List<int>> truthTable = GenerateTruthTable(operands.Count);

                List<List<int>> matchedRows = new List<List<int>>();

                for (int i = 0; i < truthTable.Count; ++i)
                {
                    string currentFormula = PrepareFormula(formula, operands, truthTable[i]);

                    if (!IsTruthTableLineTrue(currentFormula))
                    {
                        matchedRows.Add(truthTable[i]);
                    }
                }

                return GenerateResult(operands, matchedRows); ;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        // Метод разработал Пилипейко Валентин
        private static string GenerateResult(List<string> operands, List<List<int>> formulas)
        {
            StringBuilder result = new StringBuilder();

            result.Append("(");

            for (int i = 0; i < formulas.Count; ++i)
            {
                result.Append("(");

                for (int j = 0; j < formulas[i].Count; ++j)
                {
                    if (formulas[i][j] == 1)
                    {
                        result.Append("(!").Append(operands[j]).Append(")");
                    }
                    else if (formulas[i][j] == 0)
                    {
                        result.Append(operands[j]);
                    }

                    if (j + 1 != formulas[i].Count)
                    {
                        result.Append("\\/");
                    }
                }
                result.Append(")");

                if (i + 1 != formulas.Count)
                {
                    result.Append("/\\");
                }
            }

            result.Append(")");

            return result.ToString();
        }

        // Метод разработал Пилипейко Валентин
        private static bool IsTruthTableLineTrue(string formula)
        {
            if (IsNumericUnaryOperation(formula.TryRemoveBrackets()))
            {
                if (IsNegativeUnaryOperation(formula.TryRemoveBrackets()))
                {
                    return int.Parse(formula.TryRemoveBrackets()[1].ToString()) != 1;
                }

                return int.Parse(formula.TryRemoveBrackets()[0].ToString()) == 1;
            }

            string leftSubFormula = GetLeftSubFormula(formula);
            string rightSubFormula = GetRightSubFormula(formula);

            switch (GetOperation(formula))
            {
                case "->":
                {
                    bool leftResult = IsTruthTableLineTrue(leftSubFormula);

                    if (!leftResult)
                        return true;

                    return IsTruthTableLineTrue(rightSubFormula);
                }
                case "~":
                {
                    return IsTruthTableLineTrue(leftSubFormula) == IsTruthTableLineTrue(rightSubFormula);
                }
                case "/\\":
                {
                    return IsTruthTableLineTrue(leftSubFormula) && IsTruthTableLineTrue(rightSubFormula);
                }
                case "\\/":
                {
                    return IsTruthTableLineTrue(leftSubFormula) || IsTruthTableLineTrue(rightSubFormula);
                }
            }

            return true;
        }

        // Метод разработал Пилипейко Валентин
        private static bool IsNegativeUnaryOperation(string formula)
        {
            if (formula[0] == '!')
            {
                return true;
            }

            return false;
        }

        // Метод разработал Пилипейко Валентин
        private static string GetOperation(string line)
        {
            int leftBracketCounter = 0;

            for (int i = 0; i < line.Length - 1; ++i)
            {
                if (line[i] == '(')
                {
                    ++leftBracketCounter;
                }
                else if (line[i] == ')')
                {
                    --leftBracketCounter;
                }

                if (BinaryOperationsString.Contains(line[i].ToString() + line[i + 1]) && leftBracketCounter == 1)
                {
                    return line[i].ToString() + line[i + 1];
                }

                if (BinaryOperationsString.Contains(line[i].ToString()) && leftBracketCounter == 1)
                {
                    return line[i].ToString();
                }
            }

            throw new Exception("Incorrect SKNF. Couldn't get left part of formula " + line);
        }

        // Метод разработал Пилипейко Валентин
        private static string PrepareFormula(string formula, List<string> operands, List<int> values)
        {
            for (int i = 0; i < operands.Count; ++i)
            {
                formula = formula.Replace(operands[i], values[i].ToString());
            }

            return formula;
        }

        // Метод разработал Пилипейко Валентин
        private static List<List<int>> GenerateTruthTable(int operandsCount)
        {
            int size = (int) Math.Pow(2, operandsCount);

            List<List<int>> list = new List<List<int>>(size);

            for (int i = 0; i < size; ++i)
            {
                var tempList = Enumerable.Repeat(0, operandsCount).ToList();

                string currentNumber = Convert.ToString(i, 2);

                for (int j = tempList.Count - currentNumber.Length; j < tempList.Count; ++j)
                {
                    tempList[j] = Convert.ToInt32(int.Parse(currentNumber[0].ToString()));
                    currentNumber = currentNumber[1..];
                }

                list.Add(tempList);
            }

            return list;
        }

        // Метод разработал Пилипейко Валентин
        private static void GetFormulaOperands(string formula, ref List<string> operands)
        {
            string leftSubFormula = GetLeftSubFormula(formula);
            string rightSubFormula = GetRightSubFormula(formula);

            if (IsUnaryOperation(leftSubFormula.TryRemoveBrackets()))
            {
                operands.Add(leftSubFormula.TryRemoveBrackets());
            }
            else if (IsBinaryOperation(leftSubFormula))
            {
                GetFormulaOperands(leftSubFormula, ref operands);
            }

            if (IsUnaryOperation(rightSubFormula.TryRemoveBrackets()))
            {
                operands.Add(rightSubFormula.TryRemoveBrackets());
            }
            else if (IsBinaryOperation(rightSubFormula))
            {
                GetFormulaOperands(rightSubFormula, ref operands);
            }
        }

        // Метод разработал Пилипейко Валентин
        static void Main(string[] args)
        {
            string[] lines =
                File.ReadAllLines(@"c:\users\pilip\source\repos\SKNF_Checker\input_lab2.txt", Encoding.UTF8);
            foreach (var line in lines)
            {
                var result = GenerateSknf(line);
                Console.WriteLine(line + "      " + result);
            }
        }
    }
}