import { zodResolver } from "@hookform/resolvers/zod";
import { useId, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { extractErrorMessage } from "../../api/errors";
import type { AdminCourseDetailDto, UpsertCourseRequest } from "../../api/types";
import { ErrorBanner } from "../form/ErrorBanner";
import { SubmitButton } from "../form/SubmitButton";
import { TextField } from "../form/TextField";

const schema = z.object({
  title: z.string().min(1, "Введите название").max(200),
  slug: z
    .string()
    .min(1, "Введите slug")
    .max(200)
    .regex(/^[a-z0-9-]+$/, "Только строчные латинские буквы, цифры и дефис"),
  description: z.string().min(1, "Введите описание").max(2000),
  level: z.enum(["Beginner", "Intermediate", "Advanced"]),
  language: z.string().min(1, "Введите язык").max(50),
  isPublished: z.boolean(),
});

type FormValues = z.infer<typeof schema>;

interface CourseFormProps {
  initial?: AdminCourseDetailDto;
  submitLabel: string;
  onSubmit: (request: UpsertCourseRequest) => Promise<void>;
}

export function CourseForm({ initial, submitLabel, onSubmit }: CourseFormProps) {
  const formId = useId();
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: initial ?? { level: "Beginner", language: "", isPublished: false },
  });

  const submit = async (values: FormValues) => {
    setServerError(null);
    try {
      await onSubmit(values);
    } catch (error) {
      setServerError(
        extractErrorMessage(error, {
          409: "Курс с таким названием или slug уже существует.",
          400: "Нельзя опубликовать курс без уроков, и без хотя бы одного вопроса в каждом уроке.",
        }),
      );
    }
  };

  return (
    <form onSubmit={handleSubmit(submit)} className="space-y-4" noValidate>
      <ErrorBanner message={serverError} />

      <TextField id={`${formId}-title`} label="Название" error={errors.title?.message} {...register("title")} />
      <TextField id={`${formId}-slug`} label="Slug (для URL)" error={errors.slug?.message} {...register("slug")} />

      <div>
        <label htmlFor={`${formId}-description`} className="mb-1 block text-sm font-medium text-slate-700">
          Описание
        </label>
        <textarea
          id={`${formId}-description`}
          rows={3}
          {...register("description")}
          className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none transition-colors focus:border-brand-500 focus:ring-2 focus:ring-brand-100"
        />
        {errors.description && <p className="mt-1 text-sm text-red-600">{errors.description.message}</p>}
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div>
          <label htmlFor={`${formId}-level`} className="mb-1 block text-sm font-medium text-slate-700">
            Уровень
          </label>
          <select
            id={`${formId}-level`}
            {...register("level")}
            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none transition-colors focus:border-brand-500 focus:ring-2 focus:ring-brand-100"
          >
            <option value="Beginner">Начальный</option>
            <option value="Intermediate">Средний</option>
            <option value="Advanced">Продвинутый</option>
          </select>
        </div>
        <TextField id={`${formId}-language`} label="Язык" error={errors.language?.message} {...register("language")} />
      </div>

      <label className="flex items-center gap-2 text-sm text-slate-700">
        <input type="checkbox" {...register("isPublished")} className="h-4 w-4 rounded accent-brand-600" />
        Опубликован
      </label>

      <SubmitButton isLoading={isSubmitting}>{submitLabel}</SubmitButton>
    </form>
  );
}
