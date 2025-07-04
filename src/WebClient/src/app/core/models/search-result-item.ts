export interface SearchResultItem {
  id: string;
  type: 'Tag' | 'Entry';
  title: string;
  description?: string;
  thumbnailUrl?: string;
  score: number;
}

export interface SearchRequest {
  query?: string;
  pageNumber: number;
  pageSize: number;
  fromDate?: string;
  toDate?: string;
  extension?: string;
}
