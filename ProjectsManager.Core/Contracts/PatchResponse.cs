namespace ProjectsManager.Core.Contracts;

public record PatchResponse<T>(Guid Id, IReadOnlyCollection<PropertyChange> Changes)
    where T : class;

public record PropertyChange(string Property, object? OldValue, object? NewValue);

public static class PatchResponseExtensions
{
    public static PatchResponse<T> CreatePatchResponse<T>(this T oldEntity, T newEntity, Guid id)
        where T : class
    {
        var changes = typeof(T)
            .GetProperties()
            .Where(p => p.CanRead)
            .Where(p => !p.IsDefined(typeof(IgnorePatchAttribute), false))
            .Select(p => new
            {
                Property = p.Name,
                OldValue = p.GetValue(oldEntity),
                NewValue = p.GetValue(newEntity)
            })
            .Where(x => !Equals(x.OldValue, x.NewValue))
            .Select(x => new PropertyChange(x.Property, x.OldValue, x.NewValue))
            .ToList();

        return new PatchResponse<T>(id, changes);
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class IgnorePatchAttribute : Attribute;