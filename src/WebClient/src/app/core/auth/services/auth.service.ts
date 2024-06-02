import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { UserCredentials } from "../models/user-credentials.model";
import { LoginResponse } from "../models/login-response.model";
import { UserInfoBrief } from "../models/user-info-brief.model";
import {
  BehaviorSubject,
  catchError,
  distinctUntilChanged,
  map,
  Observable,
  of,
  shareReplay,
  tap,
  throwError
} from 'rxjs';
import { JwtService } from './jwt.service';
import { AuthError, ValidationError } from "../models/auth-errors.model";
import { JwtTokenData } from '../models/jwt-token-data.model';

export function isValidationError(obj: any): obj is ValidationError {
  return typeof obj.errors === 'object' && obj.message;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  authPath = "api/auth";

  constructor(private jwtService: JwtService, private http: HttpClient) {
    const hasValidToken = jwtService.isAccessTokenValid();
    this._isAuthenticated = new BehaviorSubject<boolean>(hasValidToken);
    this.isAuthenticated$ = this._isAuthenticated.asObservable()
      .pipe(
        distinctUntilChanged(),
        shareReplay(1)
      );
  }

  private readonly _isAuthenticated: BehaviorSubject<boolean>;
  readonly isAuthenticated$: Observable<boolean>;

  getTokenSilent(){
    if (this.jwtService.isAccessTokenValid()) {
      return of(this.jwtService.getAccessToken());
    }

    return this.refreshToken()
      .pipe(map(success => success ? this.jwtService.getAccessToken() : null));
  }

  register(request: UserCredentials) {
    return this.http.post(`${this.authPath}/signup`, request)
      .pipe(catchError(this.handleError));
  }

  logIn(request: UserCredentials) {
    return this.http.post<LoginResponse>(`${this.authPath}/login`, request)
      .pipe(
        tap(response => {
          // TODO: extract
          this.jwtService.saveToken(response.jwt);
          this.saveUserInfo(response.user);

          this._isAuthenticated.next(true);
        }),
        catchError(this.handleError.bind(this))
      );
  }

  refreshToken(): Observable<boolean> {
    const token = this.jwtService.getRefreshToken();
    if (!token) {
      return of(false);
    }

    return this.http.post<JwtTokenData>(`${this.authPath}/refresh`, { refreshToken: token })
      .pipe(
        tap(response => {
          this.jwtService.saveToken(response);

          this._isAuthenticated.next(true);
          console.log('Refresh token successfully');
        }),
        map(() => true),
        catchError(error => {
          console.error('Token refresh failed:', error);
          this._isAuthenticated.next(false);
          return of(false);
        })
      );
  }

  logout() {
    this.jwtService.destroyTokenData();
    this.removeUserInfo();
  }

  getUserInfo(): UserInfoBrief | null {
    const userInfo = localStorage.getItem('user');
    if (!userInfo) return null;

    try {
      return JSON.parse(userInfo);
    } catch (e) {
      console.error(e);
      return null;
    }
  }

  saveUserInfo(userInfo: UserInfoBrief) {
    localStorage["user"] = JSON.stringify(userInfo);
  }

  removeUserInfo() {
    localStorage.removeItem("user");
  }

  private handleError(errorResponse: HttpErrorResponse): Observable<never> {
    let authError: AuthError;

    if (errorResponse.error instanceof ErrorEvent) {
      // A client-side or network error occurred.
      console.error('An error occurred:', errorResponse.error.message);
      authError = {
        message: `Network or client error: ${errorResponse.error.message}`,
        statusCode: 0
      };
    } else {
      console.error(errorResponse.error);

      if (errorResponse.status === 400 && isValidationError(errorResponse.error)) {
        const validationError = errorResponse.error;
        authError = {
          message: "Validation error occurred",
          details: validationError,
          statusCode: errorResponse.status
        };
      } else if (errorResponse.status === 401) {
        authError = {
          message: 'Invalid username or password. Please try again.',
          statusCode: errorResponse.status
        };
        this._isAuthenticated.next(false);
      } else {
        authError = {
          message: 'An unexpected server error occurred. Please try again later.',
          statusCode: errorResponse.status
        };
      }
    }

    return throwError(() => authError);
  }
}
