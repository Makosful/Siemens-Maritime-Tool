using System.Collections.Generic;

namespace Schwartz.Siemens.Core.DomainServices
{
    public interface ICrudRepositoryRead<T>
    {
        T Read(int id);

        IEnumerable<T> ReadAll();
    }

    public interface ICrudRepositoryWrite<T>
    {
        T Create(T item);

        T Update(int id, T item);

        T Delete(int id);
    }

    public interface ICrudRepository<T> : ICrudRepositoryRead<T>, ICrudRepositoryWrite<T>
    {
    }
}