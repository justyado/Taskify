import React, { useState, useEffect } from "react";
import styles from "./EditTask.module.css";
import { formatSmartDate, formatDateTime } from "@/shared/utils/dateFormat";
import { getStatusLabel, getStatusColor } from "@/shared/utils/taskStatus";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/shared/ui/kit/dialog";
import { Button } from "@/shared/ui/kit/button";
import { Input } from "@/shared/ui/kit/input";
import { Textarea } from "@/shared/ui/kit/textarea";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui/kit/select";
import type { Task, TaskStatus } from "@/shared/api";

interface EditTaskProps {
  open: boolean;
  onClose: () => void;
  task: Task | null;
  onDelete: () => void;
  onChangeStatus: (newStatus: TaskStatus) => void;
  onUpdate?: (updates: { title?: string; description?: string }) => void;
}

export const EditTask: React.FC<EditTaskProps> = ({
  open,
  onClose,
  task,
  onDelete,
  onChangeStatus,
  onUpdate,
}) => {
  const [isEditing, setIsEditing] = useState(false);
  const [editedTitle, setEditedTitle] = useState("");
  const [editedDescription, setEditedDescription] = useState("");
  const [editedStatus, setEditedStatus] = useState<TaskStatus>("ToDo");
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (task) {
      setEditedTitle(task.title);
      setEditedDescription(task.description || "");
      setEditedStatus(task.status);
      setIsEditing(false);
      setError(null);
      setShowDeleteConfirm(false);
    }
  }, [task]);

  if (!task) return null;

  const handleSave = () => {
    if (!editedTitle.trim()) {
      setError("Название задачи не может быть пустым");
      return;
    }

    const titleChanged = editedTitle.trim() !== task.title;
    const descriptionChanged =
      editedDescription.trim() !== (task.description || "");
    const statusChanged = editedStatus !== task.status;

    if ((titleChanged || descriptionChanged) && onUpdate) {
      onUpdate({
        title: titleChanged ? editedTitle.trim() : undefined,
        description: descriptionChanged ? editedDescription.trim() : undefined,
      });
    }

    if (statusChanged) {
      onChangeStatus(editedStatus);
    }

    setIsEditing(false);
    setError(null);
  };

  const handleCancel = () => {
    setEditedTitle(task.title);
    setEditedDescription(task.description || "");
    setEditedStatus(task.status);
    setIsEditing(false);
    setError(null);
  };

  const handleConfirmDelete = () => {
    onDelete();
    setShowDeleteConfirm(false);
  };

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent className={styles.dialog}>
        <DialogHeader>
          {isEditing ? (
            <div className={styles.titleEdit}>
              <Input
                value={editedTitle}
                onChange={(e) => {
                  setEditedTitle(e.target.value);
                  if (error) setError(null);
                }}
                placeholder="Название задачи"
                className={error ? styles.inputError : ""}
              />
              {error && <span className={styles.errorText}>{error}</span>}
            </div>
          ) : (
            <DialogTitle className={styles.title}>{task.title}</DialogTitle>
          )}

          <DialogDescription className={styles.date}>
            Создано: {formatSmartDate(task.createdAt)}
            {task.updatedAt !== task.createdAt && (
              <> · Обновлено: {formatDateTime(task.updatedAt)}</>
            )}
          </DialogDescription>
        </DialogHeader>

        <div className={styles.content}>
          {isEditing ? (
            <>
              <div className={styles.field}>
                <label className={styles.label}>Описание</label>
                <Textarea
                  value={editedDescription}
                  onChange={(e) => setEditedDescription(e.target.value)}
                  placeholder="Описание задачи"
                  rows={5}
                  className={styles.textarea}
                />
              </div>

              <div className={styles.field}>
                <label className={styles.label}>Статус</label>
                <Select
                  value={editedStatus}
                  onValueChange={(value) =>
                    setEditedStatus(value as TaskStatus)
                  }
                >
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="ToDo">К выполнению</SelectItem>
                    <SelectItem value="InProgress">В процессе</SelectItem>
                    <SelectItem value="Done">Завершено</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </>
          ) : (
            <>
              <div className={styles.descriptionBox}>
                <label className={styles.label}>Описание</label>
                <p className={styles.description}>
                  {task.description || "Описание отсутствует"}
                </p>
              </div>

              <div className={styles.statusBox}>
                <label className={styles.label}>Статус</label>
                <div className={styles.statusBadge}>
                  <span
                    className={styles.dot}
                    style={{ backgroundColor: getStatusColor(task.status) }}
                  ></span>
                  <span className={styles.statusText}>
                    {getStatusLabel(task.status)}
                  </span>
                </div>
              </div>
            </>
          )}
        </div>

        <DialogFooter className={styles.footer}>
          {showDeleteConfirm ? (
            <div className={styles.deleteConfirm}>
              <p className={styles.confirmText}>
                Вы уверены, что хотите удалить эту задачу?
              </p>
              <div className={styles.buttonContainer}>
                <Button
                  variant="secondary"
                  onClick={() => setShowDeleteConfirm(false)}
                  className={styles.button}
                >
                  Отмена
                </Button>
                <Button
                  variant="destructive"
                  onClick={handleConfirmDelete}
                  className={styles.button}
                >
                  Да, удалить
                </Button>
              </div>
            </div>
          ) : isEditing ? (
            <div className={styles.buttonContainer}>
              <Button
                variant="secondary"
                onClick={handleCancel}
                className={styles.button}
              >
                Отмена
              </Button>
              <Button
                onClick={handleSave}
                disabled={!editedTitle.trim()}
                className={styles.button}
              >
                Сохранить изменения
              </Button>
            </div>
          ) : (
            <div className={styles.buttonContainer}>
              <Button
                variant="secondary"
                onClick={() => setIsEditing(true)}
                className={styles.button}
              >
                Редактировать
              </Button>
              <Button
                variant="destructive"
                onClick={() => setShowDeleteConfirm(true)}
                className={styles.button}
              >
                Удалить
              </Button>
            </div>
          )}
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};
