import { Component, computed, effect, inject } from '@angular/core';

import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService, isValidationError } from './services/auth.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AuthError } from './models/auth-errors.model';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, FormsModule],
  templateUrl: './auth.component.html',
})
export class AuthComponent {
  auth = inject(AuthService);
  router = inject(Router);
  route = inject(ActivatedRoute);
  private formBuilder = inject(FormBuilder);

  isLoginMode = this.route.snapshot.url.at(-1)!.path !== 'signup';

  errors = computed(() => this.formatErrors(this.auth.error()));
  isLoading = this.auth.loading;

  authForm = this.formBuilder.nonNullable.group(
    {
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
    },
  );

  constructor() {
    this.auth.accountRegistered$
      .pipe(takeUntilDestroyed())
      .subscribe(() =>
        this.router.navigate(['/login']),
      );

    effect(() => {
      if (this.auth.isAuthenticated()) {
        this.router.navigate(['/app']);
      }
    });
  }

  onSubmit() {
    this.authForm.markAllAsTouched();
    if (this.authForm.invalid) return;

    const credentials = this.authForm.getRawValue();

    if (this.isLoginMode) {
      this.auth.login$.next(credentials);
    } else {
      this.auth.register$.next(credentials);
    }
  }

  get email() {
    return this.authForm.get('email');
  }

  get password() {
    return this.authForm.get('password');
  }

  private formatErrors(authError: AuthError | null): string[] {
    if (!authError) return [];

    const details = authError.details;
    if (details && isValidationError(details)) {
      return Object.values(details.errors).flat();
    }

    return [authError.message];
  }
}
