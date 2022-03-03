////////////////////////////////////////////
//Лабораторная работа №1 по дисциплине ЛОИС
//Выполнена студентами группы 921701
//Пилипейко Валентин Игоревич
//Драгун Владмир Андреевич
//24.02.2022 - Написание кода, версия 1.0.0
//Использованные источники: 
//1) Справочно проверяющая семантическая система по дисциплине ЛОИС
//   Сслыка: http://scnedu.sourceforge.net/variety/_/index.html?variety=examinator.PSMIS.E.1.json

using System.Linq;

namespace SKNF_Checker
{
    public static class StringExtension
    {
        // Метод разработал Пилипейко Валентин при поддержке Владимира Драгуна
        public static string TryRemoveBrackets(this string line)
        {
            if (line.First() == '(' && line.Last() == ')')
            {
                line = line[1..^1];
            }

            while (line.First() == '(' && line.Count(x => x == '(') > line.Count(x => x == ')'))
            {
                line = line[1..];
            }
            while (line.Last() == ')' && line.Count(x => x == ')') > line.Count(x => x == '('))
            {
                line = line[..^1];
            }

            return line;
        }
    }
}
