import { zodResolver } from "@hookform/resolvers/zod";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { Link, useLocation, useNavigate, type Location } from "react-router-dom";
import { z } from "zod";
import { extractErrorMessage } from "../api/errors";
import { ErrorBanner } from "../components/form/ErrorBanner";
import { FormCard } from "../components/form/FormCard";
import { SubmitButton } from "../components/form/SubmitButton";
import { TextField } from "../components/form/TextField";
import { PageHeader } from "../components/PageHeader";
import { useAuth } from "../auth/useAuth";

const loginSchema = z.object({
  email: z.string().min(1, "Введите email").email("Некорректный email"),
  password: z.string().min(1, "Введите пароль"),
});

type LoginFormValues = z.infer<typeof loginSchema>;

export function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormValues>({ resolver: zodResolver(loginSchema) });

  const onSubmit = async (values: LoginFormValues) => {
    setServerError(null);
    try {
      await login(values);
      const from = (location.state as { from?: Location } | null)?.from;
      navigate(from ? `${from.pathname}${from.search}` : "/", { replace: true });
    } catch (error) {
      setServerError(extractErrorMessage(error, { 401: "Неверный email или пароль." }));
    }
  };

  return (
    <div className="mx-auto max-w-sm">
      <PageHeader title="Вход" description="Войдите, чтобы записываться на курсы и проходить уроки." />

      <FormCard>
        <form className="space-y-4" onSubmit={handleSubmit(onSubmit)} noValidate>
          <ErrorBanner message={serverError} />

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
            autoComplete="current-password"
            error={errors.password?.message}
            {...register("password")}
          />

          <SubmitButton isLoading={isSubmitting} loadingLabel="Входим…">
            Войти
          </SubmitButton>
        </form>
      </FormCard>

      <p className="mt-4 text-center text-sm text-slate-600">
        Нет аккаунта?{" "}
        <Link to="/register" className="font-medium text-brand-600 hover:text-brand-700">
          Зарегистрироваться
        </Link>
      </p>
    </div>
  );
}
