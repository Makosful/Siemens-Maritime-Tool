using System.Collections.Generic;

namespace Schwartz.Siemens.Core.ApplicationServices
{
    public interface ICrudServiceRead<T>
    {
        T Read(int id);

        List<T> ReadAll();
    }

    public interface ICrudServiceWrite<T>
    {
        T Create(T item);

        T Update(int id, T item);

        T Delete(int id);
    }

    public interface ICrudService<T> : ICrudServiceRead<T>, ICrudServiceWrite<T>
    {
    }
}