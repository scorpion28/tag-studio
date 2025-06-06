import { PaginationInfo } from './pagination';

export interface PaginatedList<T> extends PaginationInfo {
  items: T[];
}
