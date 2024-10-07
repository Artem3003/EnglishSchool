using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demo_english_school.Interfaces;

public interface ICrud<TModel>
    where TModel : class
{
    Task<IEnumerable<TModel>> GetAllAsync();

    Task<TModel> GetByIdAsync(int id);

    Task AddAsync(TModel model);

    Task UpdateAsync(TModel model);

    Task DeleteAsync(int modelId);
}