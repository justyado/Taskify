import { apiClient } from "./client";
import type {
  Task,
  CreateTaskDTO,
  UpdateTaskDTO,
  TaskFilters,
  ApiResult,
} from "./types";

export const tasksApi = {
  getTasks: async (filters?: TaskFilters): Promise<Task[]> => {
    const params = new URLSearchParams();

    if (filters?.status && filters.status !== "all") {
      params.append("status", filters.status);
    }

    const queryString = params.toString();
    const url = queryString ? `/tasks?${queryString}` : "/tasks";

    const response = await apiClient.get<ApiResult<Task[]>>(url);

    // TODO: Потом удалить логи (если не забуду)
    console.log("getTasks response:", response.data);

    if (response.data.isSuccess && response.data.value) {
      return response.data.value;
    }

    if (response.data.errors && response.data.errors.length > 0) {
      throw new Error(response.data.errors[0].message);
    }

    throw new Error("Не удалось загрузить задачи");
  },

  getTaskById: async (id: string): Promise<Task> => {
    const response = await apiClient.get<ApiResult<Task>>(`/tasks/${id}`);

    if (response.data.isSuccess && response.data.value) {
      return response.data.value;
    }

    if (response.data.errors && response.data.errors.length > 0) {
      throw new Error(response.data.errors[0].message);
    }

    throw new Error("Не удалось загрузить задачу");
  },

  createTask: async (taskData: CreateTaskDTO): Promise<Task> => {
    const response = await apiClient.post<ApiResult<Task>>("/tasks", taskData);

    if (response.data.isSuccess && response.data.value) {
      return response.data.value;
    }

    if (response.data.errors && response.data.errors.length > 0) {
      throw new Error(response.data.errors[0].message);
    }

    throw new Error("Не удалось создать задачу");
  },

  updateTask: async (id: string, taskData: UpdateTaskDTO): Promise<Task> => {
    const response = await apiClient.put<ApiResult<Task>>(
      `/tasks/${id}`,
      taskData
    );

    if (response.data.isSuccess && response.data.value) {
      return response.data.value;
    }

    if (response.data.errors && response.data.errors.length > 0) {
      throw new Error(response.data.errors[0].message);
    }

    throw new Error("Не удалось обновить задачу");
  },

  deleteTask: async (id: string): Promise<void> => {
    const response = await apiClient.delete<ApiResult<null>>(`/tasks/${id}`);

    if (!response.data.isSuccess) {
      if (response.data.errors && response.data.errors.length > 0) {
        throw new Error(response.data.errors[0].message);
      }
      throw new Error("Не удалось удалить задачу");
    }
  },
};

export default tasksApi;
