import { useEffect } from "react";
import { observer } from "mobx-react-lite";
import s from "./Home.module.css";
import { TaskPanel } from "@/widgets/task-panel";
import { TaskList } from "@/widgets/task-list";
import { taskStore } from "@/shared/stores";
import type { TaskStatus } from "@/shared/api";

export const Home = observer(() => {
  useEffect(() => {
    taskStore.fetchTasks();
  }, []);

  const handleFilterChange = (filter: string) => {
    taskStore.setFilter(filter as TaskStatus | "all");
  };

  return (
    <div className={s.wrapper}>
      <TaskList />
      <TaskPanel
        currentFilter={taskStore.currentFilter}
        onFilterChange={handleFilterChange}
      />
    </div>
  );
});
