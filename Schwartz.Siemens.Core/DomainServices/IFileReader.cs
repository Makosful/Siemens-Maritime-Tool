using System.Collections.Generic;

namespace Schwartz.Siemens.Core.DomainServices
{
    public interface IFileReader<T>
    {
        IEnumerable<T> GetAllContent(string path);
    }
}