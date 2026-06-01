export interface Task {
  id: string;
  title: string;
  description: string;
  categoryId: string;
  isCompleted: boolean;
  createdAt: string;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  categoryId: string;
}

export interface UpdateTaskRequest {
  title?: string;
  description?: string;
  categoryId?: string;
  isCompleted?: boolean;
}
