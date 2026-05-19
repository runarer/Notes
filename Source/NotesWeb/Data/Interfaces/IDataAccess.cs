namespace NotesWeb.Data.Interfaces;

public interface IDataAccess
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}