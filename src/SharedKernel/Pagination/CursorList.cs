namespace SharedKernel.Pagination
{
    public sealed record CursorList<T>(long Cursor, IEnumerable<T> Items)
    {
    }
}