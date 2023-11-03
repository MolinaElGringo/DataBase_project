
public interface ISupportCRUD<T>{
    public Task<T[]> GetAsync();
    public Task<T> AddAsync(T obj);
    public Task<bool> DeleteAsync(T obj);
    public Task<T> UpdateAsync(T obj);

    
}

