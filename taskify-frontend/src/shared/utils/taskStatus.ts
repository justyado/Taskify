import type { TaskStatus } from "@/shared/api";

export const STATUS_LABELS: Record<TaskStatus, string> = {
  ToDo: "К выполнению",
  InProgress: "В процессе",
  Done: "Завершено",
};

export const STATUS_COLORS: Record<TaskStatus, string> = {
  ToDo: "#3B82F6",
  InProgress: "#F59E0B",
  Done: "#22C55E",
};

export const STATUS_CLASSES: Record<TaskStatus, string> = {
  ToDo: "status-todo",
  InProgress: "status-inprogress",
  Done: "status-done",
};

export const ALL_STATUSES: TaskStatus[] = ["ToDo", "InProgress", "Done"];

export function getStatusLabel(status: TaskStatus): string {
  return STATUS_LABELS[status] || status;
}

export function getStatusColor(status: TaskStatus): string {
  return STATUS_COLORS[status] || "#6B7280";
}

export function getStatusClass(status: TaskStatus): string {
  return STATUS_CLASSES[status] || "";
}

export function isValidStatus(value: string): value is TaskStatus {
  return ALL_STATUSES.includes(value as TaskStatus);
}

export function getNextStatus(currentStatus: TaskStatus): TaskStatus {
  const currentIndex = ALL_STATUSES.indexOf(currentStatus);
  const nextIndex = (currentIndex + 1) % ALL_STATUSES.length;
  return ALL_STATUSES[nextIndex];
}

export function getPreviousStatus(currentStatus: TaskStatus): TaskStatus {
  const currentIndex = ALL_STATUSES.indexOf(currentStatus);
  const previousIndex =
    (currentIndex - 1 + ALL_STATUSES.length) % ALL_STATUSES.length;
  return ALL_STATUSES[previousIndex];
}
