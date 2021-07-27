using System.Collections.Generic;

namespace Matrix.Services
{
    public interface IWordFinder
    {
        public IEnumerable<string> Find(IEnumerable<string> wordstream);
    }
}
