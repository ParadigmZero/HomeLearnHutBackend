using System.Collections.Generic;
using System.Threading.Tasks;

public interface IHomework<T>
{
    Task<IEnumerable<T>> GetAll();
    Task Insert(T t);
    Task<Homework> Update(long id, long childId, string image, string comment, string annotation);
}