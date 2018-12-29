using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Miscellaneous
{
    public class TableGenerator
    {
        public Encoding Encoding { get; set; } = new UTF8Encoding(false);
        public int MaximalWidth { get; set; }
        public string[] Generate(string[,] array, string title, bool tableHasTitles, bool addLinesAbove)
        {
            return Generate(array, title, tableHasTitles, addLinesAbove, new DefaultLineCharacterDecider());
        }
        public string[] Generate(string[,] array, string title, bool tableHasTitles, bool addLinesAbove, ILineCharacterDecider lineCharacterDecider)
        {
            if ((tableHasTitles && array.GetLength(0) == 1) | array.GetLength(0) == 0)
            {
                throw new Exception("Not enough data available!");
            }
            return this.GetTableByArray(array, tableHasTitles, addLinesAbove, lineCharacterDecider);
        }
        public void Generate(string[,] array, string title, string file, bool tableHasTitles, bool addLinesAbove, bool append)
        {
            Generate(array, title, file, tableHasTitles, addLinesAbove, new DefaultLineCharacterDecider());
        }
        public void Generate(string[,] array, string title, string file, bool tableHasTitles, bool addLinesAbove, ILineCharacterDecider lineCharacterDecider)
        {
            Utilities.EnsureFileExists(file);
            List<string> lines = new List<string> { title };
            lines.AddRange(Generate(array, title, tableHasTitles, addLinesAbove, lineCharacterDecider));
            System.IO.File.AppendAllLines(file, lines, Encoding);
        }
        private string[] GetTableByArray(string[,] array, bool tableHasTitles, bool addLinesAbove, ILineCharacterDecider lineCharacterDecider)
        {
            List<List<string>> resultContentLines = new List<List<string>>();
            int[] widths = this.GetMaximalWidthsOfColumns(array);

            for (int rowIndex = 0; rowIndex <= array.GetLength(0) - 1; rowIndex++)
            {
                List<string> newItem = new List<string>();
                for (int lineIndex = 0; lineIndex <= array.GetLength(1) - 1; lineIndex++)
                {
                    newItem.Add(array[rowIndex, lineIndex]);
                }
                resultContentLines.Add(newItem);
            }

            List<string>[] resultContentLinesAsString = resultContentLines.ToArray();
            List<string> result = new List<string>
            {
                this.GetFirstLine(widths)
            };
            if (tableHasTitles)
            {
                result.Add(this.GetContentLine(resultContentLinesAsString[0], widths));
                result.Add(this.GetSecondLine(widths));
                for (int i = 1; i <= resultContentLinesAsString.Length - 1; i++)
                {
                    if (addLinesAbove && (!(i == 1)))
                    {
                        result.Add(this.GetMiddleLine(resultContentLinesAsString[i], lineCharacterDecider, widths));
                    }
                    result.Add(this.GetContentLine(resultContentLinesAsString[i], widths));
                }
            }
            else
            {
                for (int i = 0; i <= resultContentLinesAsString.Length - 1; i++)
                {
                    if (addLinesAbove && (!(i == 0)))
                    {
                        result.Add(this.GetMiddleLine(resultContentLinesAsString[i], lineCharacterDecider, widths));
                    }
                    result.Add(this.GetContentLine(resultContentLinesAsString[i], widths));
                }
            }
            result.Add(this.GetLastLine(widths));
            return result.ToArray();
        }

        private int[] GetMaximalWidthsOfColumns(string[,] array)
        {
            List<int> result = new List<int>();
            for (int i = 0; i <= array.GetLength(1) - 1; i++)
            {
                dynamic maxLengthOfColumn = 0;
                for (int j = 0; j <= array.GetLength(0) - 1; j++)
                {
                    maxLengthOfColumn = Math.Max(maxLengthOfColumn, array[j, i].Length);
                }
                if (maxLengthOfColumn > this.MaximalWidth)
                {
                    maxLengthOfColumn = this.MaximalWidth;
                }
                result.Add(maxLengthOfColumn);
            }
            return result.ToArray();
        }
        private string FillOrCut(string input, int amountOfChars, char fillChar, bool addFillCharAfterInput = true)
        {
            if (string.IsNullOrEmpty(fillChar.ToString()))
            {
                fillChar = ' ';
            }
            if (amountOfChars == 0)
            {
                return string.Empty;
            }
            if (amountOfChars == 1)
            {
                if (string.IsNullOrEmpty(input))
                {
                    return fillChar.ToString();
                }
                else
                {
                    return input.Substring(0, 1);
                }
            }
            if (amountOfChars == 2)
            {
                if (string.IsNullOrEmpty(input))
                {
                    return (fillChar.ToString() + fillChar.ToString());
                }
                else
                {
                    if (input.Length == 1)
                    {
                        if (addFillCharAfterInput)
                        {
                            return input + fillChar;
                        }
                        else
                        {
                            return fillChar + input;
                        }
                    }
                    else
                    {
                        return input.Substring(0, 2);
                    }
                }
            }
            if (amountOfChars == 3)
            {
                if (string.IsNullOrEmpty(input))
                {
                    return (fillChar.ToString() + fillChar + fillChar).ToString();
                }
                else
                {
                    if (input.Length == 1)
                    {
                        if (addFillCharAfterInput)
                        {
                            return input + fillChar + fillChar;
                        }
                        else
                        {
                            return fillChar.ToString() + fillChar + input;
                        }
                    }
                    else
                    {
                        if (input.Length == 2)
                        {
                            if (addFillCharAfterInput)
                            {
                                return input + fillChar;
                            }
                            else
                            {
                                return fillChar + input;
                            }
                        }
                        else
                        {
                            return input.Substring(0, 3);
                        }
                    }
                }
            }
            if (input.Length < amountOfChars)
            {
                if (addFillCharAfterInput)
                {
                    return this.FillOrCut(input + fillChar, amountOfChars, fillChar);
                }
                else
                {
                    return this.FillOrCut(fillChar + input, amountOfChars, fillChar);
                }
            }
            else if (input.Length > amountOfChars)
            {
                return input.Substring(0, amountOfChars - 3) + "...";
            }
            else
            {
                return input;
            }
        }
        public interface ILineCharacterDecider
        {
            char GetChar(string lineTextBelow);

            char GetcTRight(List<string> resultContentLinesAsString);

            char GetcMiddle(List<string> resultContentLinesAsString, int i);

            char GetcTLeft(List<string> resultContentLinesAsString);


        }

        private string GetFirstLine(int[] widths)
        {
            List<string> result = new List<string>
            {
                LeftUpperCornerCharacter.ToString()
            };
            for (int i = 0; i <= widths.Length - 1; i++)
            {
                if (!(i == 0))
                {
                    result.Add(TDownCharacter.ToString());
                }
                result.Add(this.FillOrCut(string.Empty, widths[i], HorizontalLineCharacter));
            }
            result.Add(RightUpperCornerCharacter.ToString());
            return string.Join(string.Empty, result);
        }

        private string GetContentLine(List<string> resultContentLinesAsString, int[] widths)
        {
            List<string> result = new List<string>();
            for (int i = 0; i <= resultContentLinesAsString.Count - 1; i++)
            {
                string currentLine = resultContentLinesAsString[i];
                result.Add(VerticalLineCharacter.ToString());
                result.Add(this.FillOrCut(currentLine, widths[i], ' '));
            }
            result.Add(VerticalLineCharacter.ToString());
            return string.Join("", result);
        }

        private string GetSecondLine(int[] widths)
        {
            List<string> result = new List<string>
            {
                TRightCharacter.ToString()
            };
            for (int i = 0; i <= widths.Length - 1; i++)
            {
                if (!(i == 0))
                {
                    result.Add(CrossCharacterCharacter.ToString());
                }
                result.Add(this.FillOrCut(string.Empty, widths[i], HorizontalLineCharacter));
            }
            result.Add(TLeftCharacter.ToString());
            return string.Join(string.Empty, result);
        }

        private string GetLastLine(int[] widths)
        {
            List<string> result = new List<string>
            {
                LeftLowerCornerCharacter.ToString()
            };
            for (int i = 0; i <= widths.Length - 1; i++)
            {
                if (!(i == 0))
                {
                    result.Add(TUpCharacter.ToString());
                }
                result.Add(this.FillOrCut(string.Empty, widths[i], HorizontalLineCharacter));
            }
            result.Add(RightLowerCornerCharacter.ToString());
            return string.Join(string.Empty, result);
        }
        private string GetMiddleLine(List<string> resultContentLinesAsString, ILineCharacterDecider lineCharacterDecider, int[] widths)
        {
            List<string> result = new List<string>
            {
                lineCharacterDecider.GetcTRight(resultContentLinesAsString).ToString()
            };
            for (int i = 0; i <= resultContentLinesAsString.Count - 1; i++)
            {
                if (!(i == 0))
                {
                    result.Add(lineCharacterDecider.GetcMiddle(resultContentLinesAsString, i).ToString());
                }
                dynamic current = resultContentLinesAsString[i];
                result.Add(FillOrCut(string.Empty, widths[i], lineCharacterDecider.GetChar(current)));
            }
            result.Add(lineCharacterDecider.GetcTLeft(resultContentLinesAsString).ToString());
            return string.Join(string.Empty, result);
        }
        #region Constants
        public const char HorizontalLineCharacter = '─';
        public const char VerticalLineCharacter = '│';
        public const char LeftUpperCornerCharacter = '┌';
        public const char RightUpperCornerCharacter = '┐';
        public const char LeftLowerCornerCharacter = '└';
        public const char RightLowerCornerCharacter = '┘';
        public const char CrossCharacterCharacter = '┼';
        public const char TDownCharacter = '┬';
        public const char TRightCharacter = '├';
        public const char TLeftCharacter = '┤';
        public const char TUpCharacter = '┴';
        #endregion
        public class DefaultLineCharacterDecider : ILineCharacterDecider
        {
            public char GetChar(string lineTextBelow)
            {
                throw new NotImplementedException();
            }

            public char GetcMiddle(List<string> resultContentLinesAsString, int i)
            {
                throw new NotImplementedException();
            }

            public char GetcTLeft(List<string> resultContentLinesAsString)
            {
                throw new NotImplementedException();
            }

            public char GetcTRight(List<string> resultContentLinesAsString)
            {
                throw new NotImplementedException();
            }
        }
    }
}
