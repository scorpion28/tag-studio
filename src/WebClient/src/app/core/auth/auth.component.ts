import { Component, OnInit } from '@angular/core';

import {
  ReactiveFormsModule,
  FormGroup,
  Validators,
  FormControl, FormsModule
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { UserCredentials } from './models/user-credentials.model';
import { AuthService, isValidationError } from './services/auth.service';
import { AuthError } from './models/auth-errors.model';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, FormsModule],
  templateUrl: './auth.component.html',
})
export class AuthComponent implements OnInit {
  isLoginMode = true;

  authForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)]),
  });

  errorMessage: string = "";
  isLoading: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.isLoginMode = this.route.snapshot.url.at(-1)!.path !== "signup";
  }

  onSubmit() {
    this.authForm.markAllAsTouched();
    this.errorMessage = "";

    if (this.authForm.invalid) return;

    this.isLoading = true;

    const formValues = this.authForm.value;
    const credentials: UserCredentials = {
      email: formValues.email as string,
      password: formValues.password as string
    };

    if (this.isLoginMode) {
      this.logIn(credentials);
    } else {
      this.registerUser(credentials);
    }
  }

  logIn(credentials: UserCredentials) {
    this.authService.logIn(credentials)
      .subscribe({
        next: (_) => {
          console.log("Logged in");
          this.router.navigate(["/app"]);
        },
        error: (error: AuthError) => {
          this.isLoading = false;
          console.error('Login failed:', error);
          this.displayError(error);
        }
      });
  }

  registerUser(credentials: UserCredentials) {
    this.authService.register(credentials)
      .subscribe({
        next: _ => {
          this.router.navigate(["/login"]);
        },
        error: (authError: AuthError) => {
          this.isLoading = false;
          console.error('Sign up failed:', authError);

          this.displayError(authError);
        }
      });
  }


  private displayError(authError: AuthError) {
    const details = authError.details;
    if (details) {
      if (isValidationError(details)) {
        Object.values(details.errors).forEach((name) => {
          this.errorMessage += name.join("\n");
        });
      }
    } else {
      this.errorMessage = authError.message;
    }
  }

  get email() {
    return this.authForm.get('email');
  }

  get password() {
    return this.authForm.get('password');
  }
}
