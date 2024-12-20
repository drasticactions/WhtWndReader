using SQLite;

namespace WhtWndReader.Services;

/// <summary>
/// Database Service.
/// </summary>
public class DatabaseService : IDisposable
{
    private const SQLite.SQLiteOpenFlags Flags =
        SQLite.SQLiteOpenFlags.ReadWrite |
        SQLite.SQLiteOpenFlags.Create |
        SQLite.SQLiteOpenFlags.SharedCache;

    private SQLiteAsyncConnection database;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseService"/> class.
    /// </summary>
    /// <param name="connectionString">Connection String.</param>
    public DatabaseService(string connectionString)
    {
        SQLitePCL.Batteries.Init();
        this.database = new SQLiteAsyncConnection(connectionString, Flags);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="DatabaseService"/> class.
    /// </summary>
    ~DatabaseService()
    {
        this.ReleaseUnmanagedResources();
    }

    /// <summary>
    /// Initialize the database.
    /// </summary>
    /// <returns>Task.</returns>
    public async Task InitializeAsync()
    {
        await this.database.CreateTablesAsync<Models.Author, Models.AuthorEntry>().ConfigureAwait(false);
    }

    public async Task<int> InsertOrReplaceAuthorAsync(Models.Author author)
    {
        return await this.database.InsertOrReplaceAsync(author, typeof(Models.Author)).ConfigureAwait(false);
    }

    public async Task<int> DeleteAllAuthorEntriesAsync(string authorId)
    {
        return await this.database.Table<Models.AuthorEntry>().DeleteAsync(n => n.AuthorId == authorId).ConfigureAwait(false);
    }

    public async Task<int> DeleteAuthorAsync(string authorId)
    {
        await this.DeleteAllAuthorEntriesAsync(authorId).ConfigureAwait(false);
        return await this.database.Table<Models.Author>().DeleteAsync(n => n.Id == authorId).ConfigureAwait(false);
    }

    public async Task<List<Models.AuthorEntry>> GetAuthorEntriesAsync(string authorId, string visibility = "public")
    {
        return await this.database.Table<Models.AuthorEntry>().Where(n => n.AuthorId == authorId && n.Visibility == visibility).OrderByDescending(n => n.CreatedAt).ToListAsync().ConfigureAwait(false);
    }

    public async Task<List<Models.Author>> GetAuthorsAsync()
    {
        return await this.database.Table<Models.Author>().OrderBy(n => n.DisplayName).ToListAsync().ConfigureAwait(false);
    }

    public async Task<int> InsertAllAuthorEntriesAsync(IEnumerable<Models.AuthorEntry> authorEntries)
    {
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        return await this.database.InsertAllAsync(authorEntries, typeof(Models.AuthorEntry));
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
    }

    /// <summary>
    /// Dispose elements.
    /// </summary>
    public void Dispose()
    {
        this.ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources()
    {
        this.database.CloseAsync();
    }
}