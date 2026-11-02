export interface CreateCategoryExpenseRequest {
  categoryId: string;
  name: string;
}

export interface UpdateExpenseCategoryRequest {
  categoryId: string;
  newName: string;
}

export interface RemoveCategoryExpenseRequest {
  categoryId: string;
}

export interface CreateExpenseCategoryResponse {
  categoryId: string;
  categoryName: string;
}

export interface GetExpenseCategoriesResponse {
  categoryId: string;
  categoryName: string;
  isPublic: boolean;
}

export interface UpdateExpenseCategoryResponse {
  categoryId: string;
  oldName: string;
  newName: string;
}

export interface RemoveExpenseCategoryResponse {
  categoryId: string;
  categoryName: string;
}