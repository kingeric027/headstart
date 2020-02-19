export interface ListArgs {
  search?: string;
  searchOn?: string;
  sortBy?: string;
  page?: number;
  pageSize?: number;
  filters?: ListFilters;
}

export interface ListFilters {
  [key: string]: string | string[];
}
