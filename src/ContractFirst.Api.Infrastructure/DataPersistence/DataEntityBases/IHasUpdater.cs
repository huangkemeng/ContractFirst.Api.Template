﻿namespace ContractFirst.Api.Infrastructure.DataPersistence.DataEntityBases;

public interface IHasUpdater : IEntity
{
    DateTime? UpdatedOn { get; set; }

    Guid? UpdatedBy { get; set; }
}