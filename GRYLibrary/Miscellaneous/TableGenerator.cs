using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Miscellaneous
{
    public class TableGenerator
    {
        public static string[] Generate(string[,] array, string title, bool tableHasTitles, TableOutputType outputType, int maximalColumnWitdh)
        {
            return outputType.Accept(new TableOutputTypeVisitor(array, title, tableHasTitles, maximalColumnWitdh));
        }
        private class TableOutputTypeVisitor : ITableOutputTypeVisitor<string[]>
        {
            public string[,] Array { get; set; }
            public string Title { get; set; }
            public bool TableHasTitles { get; set; }
            public int MaximalWidth { get; set; }

            public TableOutputTypeVisitor(string[,] array, string title, bool tableHasTitles, int maximalWidth)
            {
                this.Array = array;
                this.Title = title.Trim();
                this.TableHasTitles = tableHasTitles;
                this.MaximalWidth = maximalWidth;
            }

            public string[] Handle(ASCIITable tableOutputType)
            {
                List<string> result = new List<string>();
                if (!string.IsNullOrEmpty(this.Title))
                {
                    result.Add(this.Title);
                }
                int[] columnLengths = this.GetColumnLengths(this.Array, this.MaximalWidth);
                //TODO
                result.Add(this.GetFirstLineForASCIITable(tableOutputType, columnLengths));
                result.Add(this.GetHeadlineLineForASCIITable(tableOutputType, columnLengths));
                int startIndex = 0;
                if (this.TableHasTitles)
                {
                    result.Add(this.GetHeadlineDividerLineForASCIITable(tableOutputType, columnLengths));
                    startIndex = 1;
                }
                for (int lineNumber = startIndex; lineNumber < this.Array.GetLength(0); lineNumber++)
                {
                    result.Add(this.GetLineForASCIITable(tableOutputType, columnLengths, lineNumber));
                }
                result.Add(this.GetLastLineForASCIITable(tableOutputType, columnLengths));
                return result.ToArray();
            }

            private string GetLineForASCIITable(ASCIITable tableOutputType, int[] columnLengths, int lineNumber)
            {
                string[] content = new string[columnLengths.Length];
                for (int column = 0; column < this.Array.GetLength(1); column++)
                {
                    content[column] = this.Array[lineNumber, column];
                }
                return this.GetLine(tableOutputType.Characters.VerticalLineCharacter, content, ' ', tableOutputType.Characters.VerticalLineCharacter, tableOutputType.Characters.VerticalLineCharacter, columnLengths);
            }

            private string GetHeadlineDividerLineForASCIITable(ASCIITable tableOutputType, int[] columnLengths)
            {
                return this.GetLine(tableOutputType.Characters.TRightCharacter, this.NTimes(tableOutputType.Characters.HorizontalLineCharacter.ToString(), columnLengths.Length), tableOutputType.Characters.HorizontalLineCharacter, tableOutputType.Characters.CrossCharacter, tableOutputType.Characters.TLeftCharacter, columnLengths);
            }


            private string GetLastLineForASCIITable(ASCIITable tableOutputType, int[] columnLengths)
            {
                return this.GetLine(tableOutputType.Characters.LeftLowerCornerCharacter, this.NTimes(tableOutputType.Characters.HorizontalLineCharacter.ToString(), columnLengths.Length), tableOutputType.Characters.HorizontalLineCharacter, tableOutputType.Characters.TUpCharacter, tableOutputType.Characters.RightLowerCornerCharacter, columnLengths);
            }

            private string GetHeadlineLineForASCIITable(ASCIITable tableOutputType, int[] columnLengths)
            {
                return this.GetLineForASCIITable(tableOutputType, columnLengths, 0);
            }

            private string GetFirstLineForASCIITable(ASCIITable tableOutputType, int[] columnLengths)
            {
                return this.GetLine(tableOutputType.Characters.LeftUpperCornerCharacter, this.NTimes(tableOutputType.Characters.HorizontalLineCharacter.ToString(), columnLengths.Length), tableOutputType.Characters.HorizontalLineCharacter, tableOutputType.Characters.TDownCharacter, tableOutputType.Characters.RightUpperCornerCharacter, columnLengths);
            }
            private string GetLine(char firstChar, string[] content, char fillCharForContent, char separator, char lastChar, int[] columnLengths)
            {
                return $"{firstChar}{this.GetContentOfLine(content, fillCharForContent, separator, columnLengths)}{lastChar}";
            }

            private string GetContentOfLine(string[] content, char fillCharForContent, char separator, int[] columnLengths)
            {
                for (int i = 0; i < content.Length; i++)
                {
                    content[i] = this.AdjustLength(content[i], columnLengths[i]).PadRight(columnLengths[i], fillCharForContent);
                }
                return string.Join(separator.ToString(), content);
            }

            private string AdjustLength(string value, int maximalLength)
            {
                int valueLength = value.Length;
                if (valueLength <= maximalLength)
                {
                    return value;
                }
                else
                {
                    int minimumLengthForCut = 8;
                    if (maximalLength < minimumLengthForCut)
                    {
                        return value.Substring(0, maximalLength);
                    }
                    else
                    {
                        return value.Substring(0, maximalLength - 3) + "...";
                    }
                }
            }

            private T[] NTimes<T>(T value, int amount)
            {
                return Enumerable.Repeat(value, amount).ToArray();
            }
            private int[] GetColumnLengths(string[,] array, int maximalWidth)
            {
                int[] result = this.NTimes(0, array.GetLength(1));
                for (int line = 0; line < array.GetLength(0); line++)
                {
                    for (int column = 0; column < array.GetLength(1); column++)
                    {
                        string currentCellValue = array[line, column];
                        int currentCellValueLength = currentCellValue.Length;
                        if (result[column] == 0 || result[column] < currentCellValueLength)
                        {
                            result[column] = currentCellValueLength;
                        }
                        if (result[column] > maximalWidth)
                        {
                            result[column] = maximalWidth;
                        }
                    }
                }
                return result;
            }

            public string[] Handle(HTMLTable tableOutputType)
            {
                throw new NotImplementedException();
            }
        }
        public interface ITableCharacter
        {
            char HorizontalLineCharacter { get; }
            char VerticalLineCharacter { get; }
            char LeftUpperCornerCharacter { get; }
            char RightUpperCornerCharacter { get; }
            char LeftLowerCornerCharacter { get; }
            char RightLowerCornerCharacter { get; }
            char CrossCharacter { get; }
            char TDownCharacter { get; }
            char TRightCharacter { get; }
            char TLeftCharacter { get; }
            char TUpCharacter { get; }
        }
        public class OneLineTableCharacter : ITableCharacter
        {
            public char HorizontalLineCharacter => '─';
            public char VerticalLineCharacter => '│';
            public char LeftUpperCornerCharacter => '┌';
            public char RightUpperCornerCharacter => '┐';
            public char LeftLowerCornerCharacter => '└';
            public char RightLowerCornerCharacter => '┘';
            public char CrossCharacter => '┼';
            public char TDownCharacter => '┬';
            public char TRightCharacter => '├';
            public char TLeftCharacter => '┤';
            public char TUpCharacter => '┴';
        }
        public class DoubleLineTableCharacter : ITableCharacter
        {
            public char HorizontalLineCharacter => '═';
            public char VerticalLineCharacter => '║';
            public char LeftUpperCornerCharacter => '╔';
            public char RightUpperCornerCharacter => '╗';
            public char LeftLowerCornerCharacter => '╚';
            public char RightLowerCornerCharacter => '╝';
            public char CrossCharacter => '╬';
            public char TDownCharacter => '╦';
            public char TRightCharacter => '╠';
            public char TLeftCharacter => '╣';
            public char TUpCharacter => '╩';
        }
        public abstract class TableOutputType
        {
            public abstract void Accept(ITableOutputTypeVisitor visitor);
            public abstract T Accept<T>(ITableOutputTypeVisitor<T> visitor);

        }
        public interface ITableOutputTypeVisitor
        {
            void Handle(ASCIITable tableOutputType);
            void Handle(HTMLTable tableOutputType);
        }

        public interface ITableOutputTypeVisitor<T>
        {
            T Handle(ASCIITable tableOutputType);
            T Handle(HTMLTable tableOutputType);
        }
        public sealed class ASCIITable : TableOutputType
        {
            public ITableCharacter Characters { get; set; } = new OneLineTableCharacter();

            public override void Accept(ITableOutputTypeVisitor visitor)
            {
                visitor.Handle(this);
            }

            public override T Accept<T>(ITableOutputTypeVisitor<T> visitor)
            {
                return visitor.Handle(this);
            }
        }
        public sealed class HTMLTable : TableOutputType
        {
            public override void Accept(ITableOutputTypeVisitor visitor)
            {
                visitor.Handle(this);
            }

            public override T Accept<T>(ITableOutputTypeVisitor<T> visitor)
            {
                return visitor.Handle(this);
            }
        }

    }
}
