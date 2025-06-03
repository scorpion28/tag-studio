import { computed, effect, inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { UserCredentials } from '../models/user-credentials.model';
import { LoginResponse } from '../models/login-response.model';
import { UserInfo } from '../models/user-info-brief.model';
import {
  catchError, EMPTY,
  map,
  Observable,
  of, shareReplay,
  Subject, switchMap,
  tap,
} from 'rxjs';
import { StorageService } from './storage.service';
import { AuthError, ValidationError } from '../models/auth-errors.model';
import { JwtTokenData } from '../models/jwt-token-data.model';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

interface AuthState {
  user: UserInfo | null;
  error: AuthError | null;
  tokenData: JwtTokenData | null;
  loading: boolean;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly authPath = 'api/auth';

  private storageService: StorageService = inject(StorageService);
  private http = inject(HttpClient);

  // state
  private readonly state = signal<AuthState>({
    tokenData: null,
    loading: false,
    error: null,
    user: null,
  });

  // selectors
  user = computed(() => this.state().user);
  tokenData = computed(() => this.state().tokenData);
  isAuthenticated = computed(() => !!this.tokenData());
  loading = computed(() => {
    let loading = this.state().loading;
    console.log(`Loading: ${loading}`);
    return loading;
  });
  error = computed(() => {
    let error = this.state().error;
    if (error) {
      console.log(error.message);
    }
    return error;
  });

  // sources
  login$ = new Subject<UserCredentials>();
  logOut$ = new Subject<void>();
  register$ = new Subject<UserCredentials>();

  private userLoggedIn$ = this.login$
    .pipe(
      switchMap(credentials =>
        this.http.post<LoginResponse>(`${this.authPath}/login`, credentials)
          .pipe(catchError(err => this.handleError(err))),
      ),
    );

  accountRegistered$ = this.register$
    .pipe(
      switchMap(credentials =>
        this.http.post(`${this.authPath}/signup`, credentials)
          .pipe(catchError(err => this.handleError(err))),
      ),
      shareReplay(1),
    );

  constructor() {
    const tokenData = this.storageService.getTokenData();
    const userInfo = this.storageService.getUserInfo();

    if (tokenData && userInfo) {
      this.state.update((state) => ({
          ...state,
          tokenData,
          user: userInfo,
        }),
      );
    }

    this.userLoggedIn$
      .pipe(takeUntilDestroyed())
      .subscribe(res =>
        this.state.update((state) => ({
            ...state,
            tokenData: res.jwt,
            user: res.user,
            loading: false,
          }),
        ),
      );

    this.accountRegistered$
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.state.update(state => ({
            ...state,
            loading: false,
          }),
        ),
      );

    this.logOut$
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.purgeData());

    effect(() => {
        const tokenData = this.tokenData();
        const user = this.user();

        if (tokenData) {
          this.storageService.saveTokenData(tokenData);
        }
        if (user) {
          this.storageService.saveUserInfo(user);
        }
      },
    );
  }

  refreshToken(): Observable<string | null> {
    const refreshToken = this.tokenData()?.refreshToken;
    if (!refreshToken) {
      return of(null);
    }

    return this.http
      .post<JwtTokenData>(`${this.authPath}/refresh`, { refreshToken: refreshToken })
      .pipe(
        catchError(err => this.handleError(err)),
        tap(response => {
          this.state.update(state => ({
            ...state,
            loading: false,
            error: null,
            tokenData: response,
          }));

          console.log('Token refreshed successfully');
        }),
        map((tokenData) => tokenData.refreshToken),
      );
  }

  getTokenSilent(): Observable<string | null> {
    if (this.isAccessTokenValid()) {
      return of(this.tokenData()?.accessToken!);
    }

    return this.refreshToken();
  }

  private isAccessTokenValid(): boolean {
    const tokenData = this.tokenData();
    if (!tokenData) {
      return false;
    }

    return tokenData.expiresAtUtc > Date.now();
  }

  private handleError(errorResponse: HttpErrorResponse) {
    if (!errorResponse) return EMPTY;

    let authError: AuthError;

    if (errorResponse.error instanceof ErrorEvent) {
      // A client-side or network error occurred.
      console.error('An error occurred:', errorResponse.error.message);
      authError = {
        message: `Network or client error: ${errorResponse.error.message}`,
        statusCode: 0,
      };
    } else {
      console.error(errorResponse.error);

      if (errorResponse.status === 400 && isValidationError(errorResponse.error)) {
        const validationError = errorResponse.error;
        authError = {
          message: 'Validation error occurred',
          details: validationError,
          statusCode: errorResponse.status,
        };
      } else if (errorResponse.status === 401) {
        authError = {
          message: 'Invalid username or password. Please try again.',
          statusCode: errorResponse.status,
        };
      } else {
        authError = {
          message: 'An unexpected server error occurred. Please try again later.',
          statusCode: errorResponse.status,
        };
      }
    }

    this.state.update((state) => ({ ...state, error: authError }));

    return EMPTY;
  }

  private purgeData() {
    this.state.update((state) => ({
        ...state,
        tokenData: null,
        loading: false,
        user: null,
        error: null,
      }),
    );

    this.storageService.removeTokenData();
    this.storageService.removeUserInfo();
  }
}

export function isValidationError(obj: any): obj is ValidationError {
  return typeof obj.errors === 'object' && obj.message;
}
