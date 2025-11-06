import { useState } from "react";
import styles from "./AddTask.module.css";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/shared/ui/kit/dialog";
import { Input } from "@/shared/ui/kit/input";
import { Textarea } from "@/shared/ui/kit/textarea";
import { Button } from "@/shared/ui/kit/button";

interface AddTaskProps {
  open: boolean;
  onClose: () => void;
  onCreate: (title: string, description: string) => void;
}

export const AddTask = ({ open, onClose, onCreate }: AddTaskProps) => {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [error, setError] = useState<string | null>(null);
  const handleSubmit = () => {
    if (!title.trim()) {
      setError("Название задачи обязательно");
      return;
    }
    onCreate(title.trim(), description.trim());
    resetForm();
    onClose();
  };

  const resetForm = () => {
    setTitle("");
    setDescription("");
    setError(null);
  };

  const handleClose = () => {
    resetForm();
    onClose();
  };

  const handleTitleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setTitle(e.target.value);
    if (error) {
      setError(null);
    }
  };

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className={styles.dialog}>
        <DialogHeader>
          <DialogTitle className={styles.title}>Создание задачи</DialogTitle>
          <DialogDescription className={styles.subtitle}>
            Заполните информацию о новой задаче
          </DialogDescription>
          <button className={styles.closeBtn} onClick={handleClose}></button>
        </DialogHeader>

        <div className={styles.form}>
          <div className={styles.field}>
            <label className={styles.label}>
              Название <span className={styles.required}>*</span>
            </label>
            <Input
              placeholder="Введите название задачи"
              value={title}
              onChange={handleTitleChange}
              className={error ? styles.inputError : ""}
            />
            {error && <span className={styles.errorText}>{error}</span>}
          </div>

          <div className={styles.field}>
            <label className={styles.label}>Описание</label>
            <Textarea
              placeholder="Введите описание задачи"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              rows={4}
            />
          </div>

          <Button
            onClick={handleSubmit}
            className={styles.button}
            disabled={!title.trim()}
          >
            Создать задачу
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
};
