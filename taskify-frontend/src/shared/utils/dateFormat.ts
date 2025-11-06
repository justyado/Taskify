export function formatDate(dateString: string): string {
  try {
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, "0");
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const year = date.getFullYear();
    return `${day}.${month}.${year}`;
  } catch {
    return dateString;
  }
}

export function formatTime(dateString: string): string {
  try {
    const date = new Date(dateString);
    const hours = String(date.getHours()).padStart(2, "0");
    const minutes = String(date.getMinutes()).padStart(2, "0");
    return `${hours}:${minutes}`;
  } catch {
    return dateString;
  }
}

export function formatDateTime(dateString: string): string {
  try {
    return `${formatDate(dateString)} ${formatTime(dateString)}`;
  } catch {
    return dateString;
  }
}

export function formatRelativeTime(dateString: string): string {
  try {
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMinutes = Math.floor(diffMs / (1000 * 60));
    const diffHours = Math.floor(diffMs / (1000 * 60 * 60));
    const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));

    if (diffMinutes < 1) {
      return "только что";
    } else if (diffMinutes < 60) {
      return `${diffMinutes} ${getPluralForm(
        diffMinutes,
        "минуту",
        "минуты",
        "минут"
      )} назад`;
    } else if (diffHours < 24) {
      return `${diffHours} ${getPluralForm(
        diffHours,
        "час",
        "часа",
        "часов"
      )} назад`;
    } else if (diffDays < 7) {
      return `${diffDays} ${getPluralForm(
        diffDays,
        "день",
        "дня",
        "дней"
      )} назад`;
    } else {
      return formatDate(dateString);
    }
  } catch {
    return dateString;
  }
}

function getPluralForm(
  number: number,
  one: string,
  few: string,
  many: string
): string {
  const mod10 = number % 10;
  const mod100 = number % 100;

  if (mod10 === 1 && mod100 !== 11) {
    return one;
  } else if (mod10 >= 2 && mod10 <= 4 && (mod100 < 10 || mod100 >= 20)) {
    return few;
  } else {
    return many;
  }
}

export function isToday(dateString: string): boolean {
  try {
    const date = new Date(dateString);
    const today = new Date();
    return (
      date.getDate() === today.getDate() &&
      date.getMonth() === today.getMonth() &&
      date.getFullYear() === today.getFullYear()
    );
  } catch {
    return false;
  }
}

export function isYesterday(dateString: string): boolean {
  try {
    const date = new Date(dateString);
    const yesterday = new Date();
    yesterday.setDate(yesterday.getDate() - 1);
    return (
      date.getDate() === yesterday.getDate() &&
      date.getMonth() === yesterday.getMonth() &&
      date.getFullYear() === yesterday.getFullYear()
    );
  } catch {
    return false;
  }
}

export function formatSmartDate(dateString: string): string {
  try {
    if (isToday(dateString)) {
      return `Сегодня в ${formatTime(dateString)}`;
    } else if (isYesterday(dateString)) {
      return `Вчера в ${formatTime(dateString)}`;
    } else {
      return formatDate(dateString);
    }
  } catch {
    return dateString;
  }
}
