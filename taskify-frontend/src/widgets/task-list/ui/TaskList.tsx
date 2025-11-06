import { useState } from "react";
import { observer } from "mobx-react-lite";
import s from "./TaskList.module.css";
import { TaskCard } from "@/entities/task-card";
import { EditTask } from "@/features/edit-task";
import { taskStore } from "@/shared/stores";
import type { Task } from "@/shared/api";

export const TaskList = observer(() => {
  const [selectedTask, setSelectedTask] = useState<Task | null>(null);

  const tasks = taskStore.filteredTasks;

  const isLoading = taskStore.isLoading;

  const error = taskStore.error;

  const handleDeleteTask = async (taskId: string) => {
    const success = await taskStore.deleteTask(taskId);
    if (success) {
      setSelectedTask(null);
    }
  };

  const handleUpdateTask = async (
    taskId: string,
    updates: {
      title?: string;
      description?: string;
      status?: Task["status"];
    }
  ) => {
    if (!selectedTask) return;
    const updateData = {
      title: updates.title ?? selectedTask.title,
      description: updates.description ?? selectedTask.description ?? "",
      status: updates.status ?? selectedTask.status,
    };

    const updatedTask = await taskStore.updateTask(taskId, updateData);
    if (updatedTask) {
      setSelectedTask(updatedTask);
    }
  };

  if (isLoading && tasks.length === 0) {
    return (
      <div className={s.center}>
        <div className={s.loader}>Загрузка задач...</div>
      </div>
    );
  }

  if (error && tasks.length === 0) {
    return (
      <div className={s.center}>
        <div className={s.error}>
          <p>Ошибка: {error}</p>
          <button onClick={() => taskStore.fetchTasks()}>
            Попробовать снова
          </button>
        </div>
      </div>
    );
  }

  if (tasks.length === 0) {
    return (
      <div className={s.center}>
        <div className={s.empty}>
          <p>Задач пока нет</p>
          <p>Создайте первую задачу, нажав кнопку "Добавить задачу"</p>
        </div>
      </div>
    );
  }

  return (
    <>
      <div className={s.grid}>
        {tasks.map((task) => (
          <div
            key={task.id}
            onClick={() => setSelectedTask(task)}
            className={s.cardWrapper}
          >
            <TaskCard
              title={task.title}
              description={task.description || ""}
              status={task.status}
              date={task.createdAt}
            />
          </div>
        ))}
      </div>

      {selectedTask && (
        <EditTask
          open={!!selectedTask}
          onClose={() => setSelectedTask(null)}
          task={selectedTask}
          onDelete={() => handleDeleteTask(selectedTask.id)}
          onChangeStatus={(newStatus) =>
            handleUpdateTask(selectedTask.id, { status: newStatus })
          }
          onUpdate={(updates) => handleUpdateTask(selectedTask.id, updates)}
        />
      )}
    </>
  );
});
