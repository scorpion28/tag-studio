export interface PaginatedList<T> {
  items: T[];
  pageNumber: number;
  totalPages: number;
  totalCount: number;

  hasNextPage: boolean;
  hasPreviousPage: boolean;
}
