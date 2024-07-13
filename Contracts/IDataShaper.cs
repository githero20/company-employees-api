using Entities.Models;

namespace Contracts
{
    public interface IDataShaper<T>
    {
        IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string
            fieldsString); // for a collection of entities
        ShapedEntity ShapeData(T entity, string fieldsString); // for a single entity
    }
}
