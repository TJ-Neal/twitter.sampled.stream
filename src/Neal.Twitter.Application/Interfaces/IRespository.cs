namespace Neal.Twitter.Application.Interfaces;

/// <summary>
/// Represents the basic functionality of a data repository.
/// </summary>
/// <typeparam name="T">Type specifier for the repository.</typeparam>
public interface IRepository<T> where T : class
{
    #region Public Methods

    /// <summary>
    /// Add new <typeparamref name="T"/> to repository.
    /// </summary>
    /// <param name="entity">Entity of type <typeparamref name="T"/> to add to repository.</param>
    /// <returns>Response string.</returns>
    Task<string> AddAsync(T entity);

    /// <summary>
    /// Remove a single element <typeparamref name="T"/> specified by identifier to be removed from the repository.
    /// </summary>
    /// <param name="id">Identifier for the single element.</param>
    /// <returns>Response string.</returns>
    Task<string> DeleteAsync(string id);

    /// <summary>
    /// Retrieve all <typeparamref name="T"/> in the repository.
    /// </summary>
    /// <returns>Readonly collection of <typeparamref name="T"/>.</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Retrieves the specified page of all <typeparamref name="T"/> in the repository.
    /// </summary>
    /// <param name="page">Which page to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">Number of elements to return per page. Defaults to 20.</param>
    /// <returns>Readonly collection of <typeparamref name="T"/>.</returns>
    Task<IEnumerable<T>> GetAllPagedAsync(int page = 1, int pageSize = 20);

    /// <summary>
    /// Retrieve single element <typeparamref name="T"/> specified by identifier.
    /// </summary>
    /// <param name="id">Identifier for single element.</param>
    /// <returns>Single element of <typeparamref name="T"/>.</returns>
    Task<T?> GetByIdAsync(string id);

    /// <summary>
    /// Updated <typeparamref name="T"/>.
    /// </summary>
    /// <param name="entity">Entity of type <typeparamref name="T"/> that has been updated.</param>
    /// <returns>Response string.</returns>
    Task<string> UpdateAsync(T entity);

    #endregion Public Methods
}