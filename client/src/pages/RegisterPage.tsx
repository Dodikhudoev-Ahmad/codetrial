import { zodResolver } from "@hookform/resolvers/zod";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { Link, useNavigate } from "react-router-dom";
import { z } from "zod";
import { extractErrorMessage } from "../api/errors";
import { ErrorBanner } from "../components/form/ErrorBanner";
import { FormCard } from "../components/form/FormCard";
import { SubmitButton } from "../components/form/SubmitButton";
import { TextField } from "../components/form/TextField";
import { PageHeader } from "../components/PageHeader";
import { useAuth } from "../auth/useAuth";

// Mirrors the backend's RegisterRequest DataAnnotations (Email/MaxLength(256),
// Password/MinLength(8)/MaxLength(100), DisplayName/MaxLength(100)) so the client
// rejects the same input the server would, before making a request.
const registerSchema = z
  .object({
    displayName: z.string().min(1, "Введите имя").max(100, "Слишком длинное имя"),
    email: z.string().min(1, "Введите email").max(256, "Слишком длинный email").email("Некорректный email"),
    password: z
      .string()
      .min(8, "Минимум 8 символов")
      .max(100, "Слишком длинный пароль"),
    confirmPassword: z.string().min(1, "Повторите пароль"),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Пароли не совпадают",
    path: ["confirmPassword"],
  });

type RegisterFormValues = z.infer<typeof registerSchema>;

export function RegisterPage() {
  const { register: registerUser } = useAuth();
  const navigate = useNavigate();
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<RegisterFormValues>({ resolver: zodResolver(registerSchema) });

  const onSubmit = async (values: RegisterFormValues) => {
    setServerError(null);
    try {
      await registerUser({
        email: values.email,
        password: values.password,
        displayName: values.displayName,
      });
      navigate("/", { replace: true });
    } catch (error) {
      setServerError(extractErrorMessage(error, { 409: "Этот email уже зарегистрирован." }));
    }
  };

  return (
    <div className="mx-auto max-w-sm">
      <PageHeader title="Регистрация" description="Создайте аккаунт, чтобы начать проходить курсы." />

      <FormCard>
        <form className="space-y-4" onSubmit={handleSubmit(onSubmit)} noValidate>
          <ErrorBanner message={serverError} />

          <TextField
            id="displayName"
            label="Имя"
            autoComplete="name"
            error={errors.displayName?.message}
            {...register("displayName")}
          />

          <TextField
            id="email"
            label="Email"
            type="email"
            autoComplete="email"
            error={errors.email?.message}
            {...register("email")}
          />

          <TextField
            id="password"
            label="Пароль"
            type="password"
            autoComplete="new-password"
            error={errors.password?.message}
            {...register("password")}
          />

          <TextField
            id="confirmPassword"
            label="Повторите пароль"
            type="password"
            autoComplete="new-password"
            error={errors.confirmPassword?.message}
            {...register("confirmPassword")}
          />

          <SubmitButton isLoading={isSubmitting} loadingLabel="Регистрируем…">
            Зарегистрироваться
          </SubmitButton>
        </form>
      </FormCard>

      <p className="mt-4 text-center text-sm text-slate-600">
        Уже есть аккаунт?{" "}
        <Link to="/login" className="font-medium text-brand-600 hover:text-brand-700">
          Войти
        </Link>
      </p>
    </div>
  );
}
