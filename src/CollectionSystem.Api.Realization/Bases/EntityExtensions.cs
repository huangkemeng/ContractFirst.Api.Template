using CollectionSystem.Api.Primary.Entities.Bases;

namespace CollectionSystem.Api.Realization.Bases;

public static class EntityExtensions
{
    public static T Faker<T>(this T eneity, Action<T>? action = null) where T : class
    {
        eneity = BusinessFaker<T>.Create();
        action?.Invoke(eneity);
        return eneity;
    }

    public static List<T> Faker<T>(this T eneity, int number, Action<T, int>? action = null)
        where T : class
    {
        var entities = BusinessFaker<T>.Create(number);
        for (int i = 0; i < entities.Count; i++)
        {
            action?.Invoke(entities[i], i);
        }

        return entities;
    }

    public static List<T> Faker<T>(this T eneity, int number, Action<T> action)
        where T : class
    {
        return eneity.Faker(number, (obj, _) => action.Invoke(obj));
    }
}