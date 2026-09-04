import { zodResolver } from "@hookform/resolvers/zod";
import { useId, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { extractErrorMessage } from "../../api/errors";
import type { AdminLessonDetailDto, UpsertLessonRequest } from "../../api/types";
import { ErrorBanner } from "../form/ErrorBanner";
import { SubmitButton } from "../form/SubmitButton";
import { TextField } from "../form/TextField";

const schema = z.object({
  title: z.string().min(1, "Введите название").max(200),
  theoryMarkdown: z.string().min(1, "Введите текст теории"),
  xpReward: z.number().min(0, "Не может быть отрицательным").max(1000),
  order: z.number().min(1, "Минимум 1"),
  videoUrl: z.string().max(500).optional(),
});

type FormValues = z.infer<typeof schema>;

interface LessonFormProps {
  initial?: AdminLessonDetailDto;
  courseId: string;
  submitLabel: string;
  onSubmit: (request: UpsertLessonRequest) => Promise<void>;
}

export function LessonForm({ initial, courseId, submitLabel, onSubmit }: LessonFormProps) {
  const formId = useId();
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: initial
      ? { ...initial, videoUrl: initial.youTubeVideoId ?? "" }
      : { xpReward: 10, order: 1, title: "", theoryMarkdown: "", videoUrl: "" },
  });

  const submit = async (values: FormValues) => {
    setServerError(null);
    try {
      await onSubmit({ courseId, ...values, videoUrl: values.videoUrl?.trim() || null });
    } catch (error) {
      setServerError(
        extractErrorMessage(error, {
          409: "Урок с таким порядковым номером уже существует в этом курсе.",
          400: "Проверьте ссылку на видео — похоже, это не ссылка на YouTube.",
        }),
      );
    }
  };

  return (
    <form onSubmit={handleSubmit(submit)} className="space-y-4" noValidate>
      <ErrorBanner message={serverError} />

      <TextField id={`${formId}-title`} label="Название урока" error={errors.title?.message} {...register("title")} />

      <div>
        <label htmlFor={`${formId}-theoryMarkdown`} className="mb-1 block text-sm font-medium text-slate-700">
          Теория (Markdown)
        </label>
        <textarea
          id={`${formId}-theoryMarkdown`}
          rows={10}
          {...register("theoryMarkdown")}
          className="w-full rounded-lg border border-slate-300 px-3 py-2 font-mono text-sm outline-none transition-colors focus:border-brand-500 focus:ring-2 focus:ring-brand-100"
        />
        {errors.theoryMarkdown && <p className="mt-1 text-sm text-red-600">{errors.theoryMarkdown.message}</p>}
      </div>

      <TextField
        id={`${formId}-videoUrl`}
        label="Видео с YouTube (необязательно)"
        placeholder="https://www.youtube.com/watch?v=..."
        error={errors.videoUrl?.message}
        {...register("videoUrl")}
      />

      <div className="grid grid-cols-2 gap-4">
        <TextField
          id={`${formId}-xpReward`}
          label="Опыт (XP)"
          type="number"
          error={errors.xpReward?.message}
          {...register("xpReward", { valueAsNumber: true })}
        />
        {initial && (
          <TextField
            id={`${formId}-order`}
            label="Порядок"
            type="number"
            error={errors.order?.message}
            {...register("order", { valueAsNumber: true })}
          />
        )}
      </div>

      <SubmitButton isLoading={isSubmitting}>{submitLabel}</SubmitButton>
    </form>
  );
}
