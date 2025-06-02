import { EntryBrief } from './entry-brief.model';
import { TagBrief } from '../../tag/models/tag-brief.model';

export interface EntryDetailed extends EntryBrief {
  description?: string;

  tags: TagBrief[]

  created: string;
  lastModified: string;
}
