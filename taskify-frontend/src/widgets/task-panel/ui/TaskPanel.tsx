import { useState } from "react";
import { observer } from "mobx-react-lite";
import { Button } from "@/shared/ui/kit/button";
import styles from "./TaskPanel.module.css";
import { AddTask } from "@/features/add-task";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui/kit/select";
import { taskStore } from "@/shared/stores";
import type { TaskStatus } from "@/shared/api";

interface TaskPanelProps {
  currentFilter: TaskStatus | "all";
  onFilterChange: (filter: string) => void;
}

export const TaskPanel = observer(
  ({ currentFilter, onFilterChange }: TaskPanelProps) => {
    const [openAdd, setOpenAdd] = useState(false);
    const [createError, setCreateError] = useState<string | null>(null);
    const taskCounts = taskStore.taskCounts;

    const handleCreate = async (title: string, description: string) => {
      try {
        setCreateError(null);
        const newTask = await taskStore.createTask({
          title,
          description,
        });

        if (newTask) {
          setOpenAdd(false);
        } else {
          setCreateError(
            taskStore.error || "Не удалось создать задачу. Попробуйте еще раз."
          );
        }
      } catch (error) {
        const errorMessage =
          error instanceof Error
            ? error.message
            : "Произошла ошибка при создании задачи";
        setCreateError(errorMessage);
        console.error("Ошибка при создании задачи:", error);
      }
    };

    const handleCloseAdd = () => {
      setOpenAdd(false);
      setCreateError(null);
    };

    const handleFilterChange = (value: string) => {
      onFilterChange(value);
    };

    return (
      <div className={styles.panel}>
        <Button
          className={styles.createBtn}
          size={"lg"}
          onClick={() => setOpenAdd(true)}
        >
          Создать задачу
        </Button>

        {createError && (
          <div className={styles.errorMessage}>
            <span className={styles.errorIcon}>⚠️</span>
            {createError}
          </div>
        )}
        <Select onValueChange={handleFilterChange} value={currentFilter}>
          <SelectTrigger className={styles.selectTrigger}>
            <SelectValue placeholder="Фильтр по статусу" />
          </SelectTrigger>
          <SelectContent className={styles.selectContent}>
            <SelectItem value="all">Все ({taskCounts.all})</SelectItem>

            <SelectItem value="ToDo">
              К выполнению ({taskCounts.ToDo})
            </SelectItem>

            <SelectItem value="InProgress">
              В процессе ({taskCounts.InProgress})
            </SelectItem>

            <SelectItem value="Done">Завершено ({taskCounts.Done})</SelectItem>
          </SelectContent>
        </Select>

        <AddTask
          open={openAdd}
          onClose={handleCloseAdd}
          onCreate={handleCreate}
        />
      </div>
    );
  }
);
