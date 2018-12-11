using Schwartz.Siemens.Core.Entities.Rigs;

namespace Schwartz.Siemens.Core.ApplicationServices
{
    public interface ICrudServiceRead<T>
    {
        T Read(int id);

        FilteredList<T> ReadAll(int page, int items);
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