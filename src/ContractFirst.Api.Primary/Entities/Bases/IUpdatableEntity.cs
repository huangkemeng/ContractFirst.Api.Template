namespace ContractFirst.Api.Primary.Entities.Bases;

public interface IUpdatableEntity : IEntityPrimary
{
    DateTimeOffset? UpdatedOn { get; set; }

    Guid? UpdatedBy { get; set; }
}