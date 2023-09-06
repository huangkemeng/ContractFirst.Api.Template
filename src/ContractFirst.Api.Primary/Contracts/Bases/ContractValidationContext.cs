using FluentValidation;
using Mediator.Net.Contracts;

namespace ContractFirst.Api.Primary.Contracts.Bases;

public class ContractValidationContext : ValidationContext<IMessage>
{
    public ContractValidationContext(IMessage message) : base(message)
    {
    }
}