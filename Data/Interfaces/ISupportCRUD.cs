
public interface ISupportCRUD<Titem>
{
    Task<List<Titem>> GetAllAsync();
    Task<Titem> GetById(Titem entity);
    Task<Titem> AddAsync(Titem entity);
    Task<bool> DeleteAsync(Titem entity);
    Task<Titem> UpdateAsync(Titem entity);

}

