﻿using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TagStudio.Tags.Common;
using TagStudio.Tags.Common.Models.Tags;
using TagStudio.Tags.Data;
using TagStudio.Tags.Domain;

namespace TagStudio.Tags.Features.Tags;

public class CreateTagEndpoint(TagsDbContext dbContext, ILogger<CreateTagEndpoint> logger)
    : Endpoint<CreateTagRequest, TagDetailedDto>
{
    public override void Configure()
    {
        Post("/tags");

        Summary(s => s.Summary = "Create Tag");
    }

    public override async Task<TagDetailedDto> ExecuteAsync(CreateTagRequest req, CancellationToken ct)
    {
        var newTag = new Tag
        {
            Name = req.Name,
            OwnerId = req.UserId
        };

        var requestHasParentTags = req.ParentTagsIds?.Count > 0;
        if (requestHasParentTags)
        {
            var parentTags = await dbContext.Tags
                .Where(t => t.OwnerId == req.UserId)
                .Where(t => req.ParentTagsIds!.Contains(t.Id))
                .ToListAsync(ct);

            newTag.Parents.AddRange(parentTags);
        }

        await dbContext.Tags.AddAsync(newTag, ct);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} created a new tag {TagId}", req.UserId, newTag.Id);

        return newTag.ToTagDetailedDto();
    }
}