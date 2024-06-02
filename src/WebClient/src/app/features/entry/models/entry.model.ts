import { EntryDetailed } from './entry-detailed.model';
import { TagBrief } from '../../tag/models/tag-brief.model';

export interface Entry {
  id: string;

  name: string;
  description?: string;

  tags: TagBrief[];

  created: Date;
  lastModified: Date;
}

export type CreateEntry = Omit<Entry, 'id' | 'tags' | 'lastModified' | 'created'>
export type EditEntry = { id: Entry['id'], data: CreateEntry }
export type RemoveEntry = Entry['id']

export function toEntryModel(entryDto: EntryDetailed): Entry {
  if (!entryDto) {
    throw new Error('Input detailed entry cannot be null or undefined.');
  }

  const createdDate = new Date(entryDto.created);
  const lastModifiedDate = new Date(entryDto.lastModified);

  return {
    id: entryDto.id,
    name: entryDto.name,
    description: entryDto.description,
    tags: entryDto.tags,
    created: createdDate,
    lastModified: lastModifiedDate,
  };
}
