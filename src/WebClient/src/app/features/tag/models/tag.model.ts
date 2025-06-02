import { TagBrief } from './tag-brief.model';
import { TagDetailed } from './tag-detailed.model';

export interface Tag {
  id: string;
  name: string;

  parents: TagBrief[];

  created: Date;
  lastModified: Date;
}

export function toTagModel(detailedTag: TagDetailed): Tag {
  if (!detailedTag) {
    throw new Error('Input detailedTag cannot be null or undefined.');
  }

  const createdDate = new Date(detailedTag.created);
  const lastModifiedDate = new Date(detailedTag.lastModified);

  return {
    ...detailedTag,
    created: createdDate,
    lastModified: lastModifiedDate,
  };
}
