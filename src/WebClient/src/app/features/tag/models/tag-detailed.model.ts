import { TagBrief } from './tag-brief.model';

export interface TagDetailed extends TagBrief {
  parents: TagBrief[];

  created: string;
  lastModified: string;
}
