import { makeAutoObservable, runInAction } from "mobx";
import { tasksApi } from "@/shared/api";
import type {
  Task,
  CreateTaskDTO,
  UpdateTaskDTO,
  TaskStatus,
} from "@/shared/api";

export class TaskStore {
  tasks: Task[] = [];
  currentFilter: TaskStatus | "all" = "all";
  isLoading = false;
  error: string | null = null;

  constructor() {
    makeAutoObservable(this);
  }

  get filteredTasks(): Task[] {
    if (this.currentFilter === "all") {
      return this.tasks;
    }
    return this.tasks.filter((task) => task.status === this.currentFilter);
  }

  get taskCounts() {
    return {
      all: this.tasks.length,
      ToDo: this.tasks.filter((t) => t.status === "ToDo").length,
      InProgress: this.tasks.filter((t) => t.status === "InProgress").length,
      Done: this.tasks.filter((t) => t.status === "Done").length,
    };
  }

  setFilter(filter: TaskStatus | "all") {
    this.currentFilter = filter;
  }

  async fetchTasks() {
    this.isLoading = true;
    this.error = null;

    try {
      const tasks = await tasksApi.getTasks();
      runInAction(() => {
        this.tasks = tasks;
        this.isLoading = false;
      });
    } catch (error) {
      runInAction(() => {
        this.error =
          error instanceof Error ? error.message : "Ошибка загрузки задач";
        this.isLoading = false;
      });
      console.error("Ошибка при загрузке задач:", error);
    }
  }

  async createTask(taskData: CreateTaskDTO): Promise<Task | null> {
    this.isLoading = true;
    this.error = null;

    try {
      const newTask = await tasksApi.createTask(taskData);

      runInAction(() => {
        this.tasks.unshift(newTask);
        this.isLoading = false;
      });

      return newTask;
    } catch (error) {
      runInAction(() => {
        this.error =
          error instanceof Error ? error.message : "Ошибка создания задачи";
        this.isLoading = false;
      });
      console.error("Ошибка при создании задачи:", error);
      return null;
    }
  }

  async updateTask(id: string, taskData: UpdateTaskDTO): Promise<Task | null> {
    this.isLoading = true;
    this.error = null;

    try {
      const updatedTask = await tasksApi.updateTask(id, taskData);

      runInAction(() => {
        const index = this.tasks.findIndex((task) => task.id === id);
        if (index !== -1) {
          this.tasks[index] = updatedTask;
        }
        this.isLoading = false;
      });

      return updatedTask;
    } catch (error) {
      runInAction(() => {
        this.error =
          error instanceof Error ? error.message : "Ошибка обновления задачи";
        this.isLoading = false;
      });
      console.error("Ошибка при обновлении задачи:", error);
      return null;
    }
  }

  async deleteTask(id: string): Promise<boolean> {
    this.isLoading = true;
    this.error = null;

    try {
      await tasksApi.deleteTask(id);

      runInAction(() => {
        this.tasks = this.tasks.filter((task) => task.id !== id);
        this.isLoading = false;
      });

      return true;
    } catch (error) {
      runInAction(() => {
        this.error =
          error instanceof Error ? error.message : "Ошибка удаления задачи";
        this.isLoading = false;
      });
      console.error("Ошибка при удалении задачи:", error);
      return false;
    }
  }

  clearError() {
    this.error = null;
  }
}

export const taskStore = new TaskStore();
