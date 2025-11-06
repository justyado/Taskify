import React from "react";
import styles from "./TaskCard.module.css";
import { formatSmartDate } from "@/shared/utils/dateFormat";
import { getStatusLabel, getStatusColor } from "@/shared/utils/taskStatus";
import type { TaskStatus } from "@/shared/api";

interface TaskCardProps {
  title: string;
  description: string;
  status: TaskStatus;
  date: string;
}

export const TaskCard: React.FC<TaskCardProps> = ({
  title,
  description,
  status,
  date,
}) => {
  return (
    <div className={styles.card}>
      <h3 className={styles.title}>{title}</h3>

      <div className={styles.descriptionBox}>
        <p className={styles.description}>{description}</p>
      </div>

      <div className={styles.footer}>
        <div className={styles.status}>
          <span
            className={styles.dot}
            style={{ backgroundColor: getStatusColor(status) }}
          ></span>
          {getStatusLabel(status)}
        </div>
        <span className={styles.date}>{formatSmartDate(date)}</span>
      </div>
    </div>
  );
};
