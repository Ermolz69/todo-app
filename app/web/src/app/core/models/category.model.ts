export interface Category {
  id: string;
  name: string;
  color: string;
  icon: string;
  isArchived: boolean;
}

export interface CreateCategoryRequest {
  name: string;
  color: string;
  icon: string;
}

export interface UpdateCategoryRequest {
  name?: string;
  color?: string;
  icon?: string;
  isArchived?: boolean;
}
