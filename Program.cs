////////////////////////////////////////////
//Лабораторная работа №1 по дисциплине ЛОИС
//Выполнена студентами группы 921701
//Пилипейко Валентин Игоревич
//Драгун Владмир Андреевич
//24.02.2022 - Написание кода, версия 1.0.0
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
                throw new Exception("Incorrect SKNF. Contains non-binary or non-unary operation");
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

                if (BinaryOperationsString.Contains((line[i].ToString() + line[i + 1])) && leftBracketCounter == 1)
                {
                    return line[1..i];
                }
            }

            throw new Exception("Incorrect SKNF. Couldn't get left part of formula " + line);
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

                if (BinaryOperationsString.Contains((line[i - 1].ToString() + line[i])) && rightBracketCounter == 1)
                {
                    return line.Substring(++i, line.Length - i - 1);
                }
            }

            throw new Exception("Incorrect SKNF. Couldn't get right part of formula " + line);
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

        // Метод разработал Пилипейко Валентин при поддержке Владимира Драгуна
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"c:\users\pilip\source\repos\SKNF_Checker\input.txt", Encoding.UTF8);
            foreach (var line in lines)
            {
                TryCheckFormula(line);
            }

            // string line = Console.ReadLine();
            // TryCheckFormula(line);

            // string pattern0 = "(!A123)";
            // string pattern1 = "(A\\/B\\/C)";
            // string pattern2 = "(((A12->B)\\/C12)/\\(!(A12->B)\\/C12)";
            // string pattern3 = "((!B)\\/A12)";
            // string pattern4 = "((A\\/B\\/C)/\\((!A)\\/(B\\/C)))";
            // string pattern5 = "((A\\/(B\\/C))/\\((!A)\\/(B\\/C)))";
            // string pattern6 = "(A\\/(B\\/C))";
            // string pattern7 = "((A\\/(B\\/C))/\\((A\\/(B\\/C))/\\(A\\/(B\\/C))))";
            // string pattern8 = "((A\\/(B\\/C))/\\((!A)\\/(B\\/C)))";
            // string pattern9 = "((A\\/(B\\/C))/\\(((!A)\\/(B\\/C))/\\(A\\/(B\\/(!C)))))";
            // string pattern10 = "((A\\/(B\\/C))/\\((!A)\\/(B\\/C))/\\(A\\/(B\\/(!C))))";
            //
            // TryCheckFormula(pattern0);
            // TryCheckFormula(pattern1);
            // TryCheckFormula(pattern2);
            // TryCheckFormula(pattern3);
            // TryCheckFormula(pattern4);
            // TryCheckFormula(pattern5);
            // TryCheckFormula(pattern6);
            // TryCheckFormula(pattern7);
            // TryCheckFormula(pattern8);
            // TryCheckFormula(pattern9);
            // TryCheckFormula(pattern10);
        }
    }
}