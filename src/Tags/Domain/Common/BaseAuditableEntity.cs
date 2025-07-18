﻿namespace TagStudio.Tags.Domain.Common;

public class BaseAuditableEntity : BaseEntity
{
    public DateTimeOffset Created { get; set; }

    public DateTimeOffset LastModified { get; set; }
}