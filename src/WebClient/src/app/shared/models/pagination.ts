export interface PaginationInfo {
  pageNumber: number;
  totalPages: number;
  totalCount: number;

  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export type PaginationOptions = {
  page: number;
  itemsPerPage: number;
}
