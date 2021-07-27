using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Matrix.Services
{
    public class WordFinder : IWordFinder
    {
        private int _matrixRows;
        private int _matrixColumns;
        private const int _maxMatrixRows = 64;
        private const int _maxMatrixColumns = 64;
        private char[,] _matrix = new char[_maxMatrixRows, _maxMatrixColumns];
        private List<string> _HorizontalLetters;
        private List<string> _VerticalLetters;
        public List<Tuple<string, int>> WordCount { get; set; }      

        public WordFinder(IEnumerable<string> matrix)
        {
            _matrixRows = matrix.Count();
            _matrixColumns = matrix.FirstOrDefault() == null ? 0 : matrix.FirstOrDefault().Length;
            LoadMatrix(matrix);
            _HorizontalLetters = GetHorizontalLetters();
            _VerticalLetters = GetVerticalLetters();
        }

        public IEnumerable<string> Find(IEnumerable<string> wordstream)
        {
            // convert all words to lowercase and omit duplicates
            var wordstreamList = wordstream.Select(word => word.ToLower()).Distinct().ToList();
            WordCount = new List<Tuple<string, int>>();
            // load words in auxialiar horizontal and vertical lists

            foreach (var word in wordstreamList)
            {
                WordCount.Add(new Tuple<string, int>(word, FindNumberOfOccurrenceOfWordInMatrix(word)));
            }
            // take 10 first order by descending as response
            return WordCount.OrderByDescending(x => x.Item2).Where(x => x.Item2 > 0).Take(10)
                                              .Select(y => "{ word: " + y.Item1 + " repeated " + y.Item2 + " time(s) }").AsEnumerable();
        }

        #region private     
        private void LoadMatrix(IEnumerable<string> matrix)
        {
            int rowIndex = 0;
            foreach (string line in matrix)
            {
                // save letters in the matrix only as lowercase
                for (int i = 0; i < line.ToLower().Length; i++)
                {
                    _matrix[rowIndex, i] = line[i];
                }
                rowIndex++;
            }
        }

        private int FindNumberOfOccurrenceOfWordInMatrix (string word)
        {
            var count = 0;
            // looking horizontally
            for (int i = 0; i < _HorizontalLetters.Count; i++)
            {
                count += Regex.Matches(_HorizontalLetters[i], word).Count;
            }
            // looking vertically
            for (int j = 0; j < _VerticalLetters.Count; j++)
            {
                count += Regex.Matches(_VerticalLetters[j], word).Count;
            }
            return count;
        }

        private List<string> GetHorizontalLetters()
        {
            var result = new List<string>();
            for (int i = 0; i < _matrixRows; i++)
            {
                var line = string.Empty;
                for (int j = 0; j < _matrixColumns; j++)
                {
                    line += _matrix[i, j].ToString();
                }
                result.Add(line);
            }
            return result;
        }


        private List<string> GetVerticalLetters()
        {
            var result = new List<string>();
            for (int j = 0; j < _matrixColumns; j++)
            {
                var line = string.Empty;
                for (int i = 0; i < _matrixRows; i++)
                {
                    line += _matrix[i, j].ToString();
                }
                result.Add(line);
            }
            return result;
        }
        #endregion
    }
}
