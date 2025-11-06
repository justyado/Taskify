export type TaskStatus = "ToDo" | "InProgress" | "Done";

export interface Task {
  id: string;
  title: string;
  description: string | null;
  status: TaskStatus;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTaskDTO {
  title: string;
  description: string;
}

export interface UpdateTaskDTO {
  title: string;
  description: string;
  status: TaskStatus;
}

export interface ApiResult<T> {
  isSuccess: boolean;
  isFailed: boolean;
  value?: T;
  errors?: ApiError[];
}

export interface ApiError {
  code: string;
  message: string;
}

export interface TaskFilters {
  status?: TaskStatus | "all";
}
